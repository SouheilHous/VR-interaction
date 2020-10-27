using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace dlobo.Seek
{
	public enum SearchType {
		Path, Name, Unity, GUID
	}

	public enum MatchingType {
		Exact, Greedy, Regex
	}

	public enum SortType {
		Path, Name, Size, AssetType, CreationTime, ModificationTime, LatestTime
	}

	public enum SpecialFilterType {
		None, TextureFormat, ComponentsInPrefabs
	}

	public class SeekWindow : EditorWindow, IHasCustomMenu
	{
		public static SeekWindow Instance;

		private SeekSavedData data;
		private SearchConfig config { get { return data.config; } }
		private GlobalConfig globalConfig { get { return data.globalConfig; } }

		// saved queries
		private Vector2 savedSearchesScrollPosition;

		// results
		private IEnumerable<AssetInfo> assets;
		private List<Result> results = new List<Result>();
		private double baseTimeForSearch;
		private string errorsString;
		[SerializeField] string lastSearch;
		[SerializeField] Vector2 resultsScrollPosition;
		private int iniShowIndex;
		private int endShowIndex;
		private Rect resultsScrollArea;
		private System.Func<int, Result> getResult;

		// item selection
		private ListSelections selections = new ListSelections();
		private ListSelectionsInput listSelectionsInput;
		private HashSet<int> selectedIndexesBeforePressDown;
		private Vector2 mouseDownPosition;
		private int mouseDownIndex = -1;

		// unique styles
		private GUIStyles styles;
		private GUIContent headerContent;

		private List<string> importantErrors;

		// internal
		private string[] searchOptions = new string[] {
			"Path",
			"Name",
			"Unity search",
			"GUID",
		};
		private string[] matchingOptions = new string[] {
			"Exact",
			"Greedy",
			"Regex",
		};
		private string[] sortOptions = new string[] {
			"Path",
			"Name",
			"Size (Slow! Reads size of all assets)",
			#if UNITY_5_5_OR_NEWER
			"Asset type",
			#else
			"Asset type (Extremely slow! Loads ALL assets)",
			#endif
			"Time - Creation (Slow! Queries all files)",
			"Time - Modification (Slow! Queries all files)",
			"Time - Latest of creation and modification (Slow! Queries all files)",
		};
		private string[] specialFiltersOptions = new string[] {
			"None",
			"Filter by texture format (very slow!)",
			"Search components in prefabs (experimental)"
		};
		private string[] textureFormatOptions;
		private TextureFormat[] textureFormats;

		private string[] unityAssetTypeStrings = new string[] {
			"Animation",
			"AudioClip",
			"AudioMixer",
			"Font",
			"GUISkin",
			"Material",
			"Mesh",
			"Model",
			"PhysicMaterial",
			"Prefab",
			"Scene",
			"Script",
			"Shader",
			"Sprite",
			"Texture",
			"VideoClip",
		};

		private bool previousShowSizesValue;
		private bool previousShowTypesValue;
		private bool previousShowCreationTimeValue;
		private bool previousShowLastWriteTimeValue;
		private bool shouldFocusSearchField;

		private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

		// change keyboard shortcut here (default is #&k)
		// % is ctrl on Windows, cmd on OS X; # is shift, & is alt
		[MenuItem("Window/Seek/Seek &#k", priority = 1)]
		static void OpenSeekWindow()
		{
			EditorWindow.GetWindow<SeekWindow>("Seek");
		}

		void OnEnable()
		{
			var script = MonoScript.FromScriptableObject(this);
			string seekFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));

			var seekSkin          = (GUISkin) AssetDatabase.LoadAssetAtPath(seekFolder+"/Seek Assets/Seek GUI Skin.guiskin",   typeof(GUISkin));
			var seekTabIcon       = (Texture) AssetDatabase.LoadAssetAtPath(seekFolder+"/Seek Assets/Seek Icon.png",           typeof(Texture));
			var seekHeaderTexture = (Texture) AssetDatabase.LoadAssetAtPath(seekFolder+"/Seek Assets/Seek Header Texture.png", typeof(Texture));

			importantErrors = new List<string>();

			if (seekSkin == null) {
				importantErrors.Add("<color=#ff0><b>seekSkin is null.</b></color> Please make sure that <b>Seek GUI Skin</b> is inside \"Seek/Seek Assets\".");
			}
			if (seekHeaderTexture == null) {
				importantErrors.Add("<color=#ff0><b>seekHeaderTexture is null.</b></color> Please make sure that <b>Seek Header Texture</b> is inside \"Seek/Seek Assets\".");
			}
			if (seekTabIcon == null) {
				importantErrors.Add("<color=#ff0><b>seekTabIcon is null.</b></color> Please make sure that <b>Seek Icon</b> is inside \"Seek/Seek Assets\".");
			}

			#if UNITY_5_3_OR_NEWER
			titleContent = new GUIContent("Seek", seekTabIcon);
			#else
			SeekWindow_Reflection.ChangeEditorWindowIcon(this, seekTabIcon);
			#endif

			styles = new GUIStyles(seekSkin);
			headerContent = new GUIContent("", seekHeaderTexture);

			textureFormatOptions = System.Enum.GetNames(typeof(TextureFormat));
			textureFormats = (TextureFormat[]) System.Enum.GetValues(typeof(TextureFormat));

			string dataFilePath = seekFolder + "/Seek Config.asset";
			data = SeekSavedData.Load(dataFilePath);
			setupAfterConfigLoad();

			setFunctionForGetResult(config.sortInReverse);

			selections.GetSelectable = (index) => getResult(index);
			selections.GetNumberOfSelectables = () => results.Count;

			listSelectionsInput = new ListSelectionsInput(selections);
			listSelectionsInput.CanChangeListPosition = true;
			listSelectionsInput.GetDoubleClickMaxTime = () => 0.3f;
			listSelectionsInput.GetListItemHeight = () => globalConfig.itemSize;
			listSelectionsInput.GetListHeight = () => resultsScrollArea.height;
			listSelectionsInput.GetListYPosition = () => resultsScrollPosition.y;
			listSelectionsInput.SetListYPosition = (yPos) => resultsScrollPosition.y = yPos;

			if (lastSearch != null) {
				updateDatabaseAndSearch(doResetScroll: false);
			}

			Instance = this;
		}

		void OnDisable()
		{
			Instance = null;
		}

		void OnFocus()
		{
			shouldFocusSearchField = true;
		}

		void OnGUI()
		{
			if (importantErrors.Count > 0) {
				drawCriticalErrors();
				return;
			}

			bool didSelectionsChange = listSelectionsInput.ProcessKeyboardInput(Event.current);

			drawHeader();

			drawQueryInput();

			if (globalConfig.showOptions || !globalConfig.doCollapseSearchType)     drawSearchType();
			if (globalConfig.showOptions || !globalConfig.doCollapseAssetTypes)     drawAssetTypes();
			if (globalConfig.showOptions || !globalConfig.doCollapseMatchingType)   drawMatchingType();
			if (globalConfig.showOptions || !globalConfig.doCollapseSortType)       drawSortType();
			if (globalConfig.showOptions || !globalConfig.doCollapseDisplayOptions) drawDisplayOptions();

			if (globalConfig.showOptions || !globalConfig.doCollapseSpecialFilters) drawSpecialFilters();
			if (globalConfig.showOptions) drawSeparator();
			if (globalConfig.showOptions || !globalConfig.doCollapseSavedSearches)  drawSavedSearches();

			if (globalConfig.showOptions) drawSeparator();

			if (globalConfig.showOptions) drawItemSizeSliderAtBottom();

			if (config.search != lastSearch) {
				lastSearch = config.search;
				if (isSlowSearch() && globalConfig.delayForSlowSearch > 0) {
					baseTimeForSearch = EditorApplication.timeSinceStartup;
				} else {
					updateDatabaseAndSearch();
				}
			}

			drawWarningsAndErrors();
			drawResults();

			if (globalConfig.showOptions) {
				drawItemSizeSlider();
			}

			if (didSelectionsChange) {
				Repaint();
				setSelectedObjects();
			}
		}

		public void DoRepeatSearch()
		{
			updateDatabaseAndSearch(doResetScroll: false);
			Repaint();
		}

		public void UpdateRepresentations()
		{
			for (int i = 0; i < results.Count; i++) {
				results[i].Representation = null;
			}
			Repaint();
		}

		public void DoRepaint()
		{
			Repaint();
		}

		private void drawCriticalErrors()
		{
			string error = "<color=red><b>ERROR! Something weird happened...</b></color>\n\n";
			foreach (string err in importantErrors) {
				error += err + "\n\n";
			}
			error += "Afterwards, please close and open the Seek window again.";
			GUILayout.Label(error, styles.errorLabelStyle);
		}

		private void drawHeader()
		{
			if (GUILayout.Button(headerContent, styles.headerStyle, GUILayout.Height(24), GUILayout.ExpandWidth(true))) {
				globalConfig.showOptions = !globalConfig.showOptions;
			}
			GUILayout.Space(2);
		}

		private void drawQueryInput()
		{
			GUILayout.BeginHorizontal();
			{
				GUI.SetNextControlName("SearchField");

				config.search = EditorGUILayout.TextField("Search", config.search);

				if (shouldFocusSearchField) {
					EditorGUI.FocusTextInControl("SearchField");
					shouldFocusSearchField = false;
				}

				if (GUILayout.Button("C ", GUILayout.MaxWidth(25), GUILayout.MaxHeight(15))) {
					config.search = "";
					GUI.FocusControl("");
					shouldFocusSearchField = true;
				}
				if (GUILayout.Button("Save", GUILayout.MaxWidth(45), GUILayout.MaxHeight(15))) {
					data.savedConfigs.Add(SearchConfig.Clone(config));
					SeekSavedData.SetDirty(data);
				}
			}
			GUILayout.EndHorizontal();
		}

		private void drawSearchType()
		{
			updateFromPopup(ref config.searchType, "Search using", searchOptions);
		}

		private void drawAssetTypes()
		{
			if (config.specialFilterType == SpecialFilterType.TextureFormat || config.specialFilterType == SpecialFilterType.ComponentsInPrefabs) {
				GUI.enabled = false;
			}

			int newMask = EditorGUILayout.MaskField("Asset types", config.assetTypesMask, unityAssetTypeStrings);
			if (newMask != config.assetTypesMask) {
				config.assetTypesMask = newMask;
				updateDatabaseAndSearch();
			}

			if (config.specialFilterType == SpecialFilterType.TextureFormat || config.specialFilterType == SpecialFilterType.ComponentsInPrefabs) {
				GUI.enabled = true;
			}
		}

		private void drawMatchingType()
		{
			bool isGreyedOut = (config.searchType == SearchType.Unity || config.searchType == SearchType.GUID);
			if (isGreyedOut) {
				GUI.enabled = false;
			}

			GUILayout.BeginHorizontal();
			{
				updateFromPopup(ref config.matchingType, "Matching type", matchingOptions);
				updateFromToggleButton(ref config.isCaseSensitive, "Case sensitive");
			}
			GUILayout.EndHorizontal();

			if (isGreyedOut) {
				GUI.enabled = true;
			}
		}

		private void drawSortType()
		{
			GUILayout.BeginHorizontal();
			{
				SortType previousSortType = config.sortType;

				if (updateFromPopup(ref config.sortType, "Sort by", sortOptions))
				{
					// deactivate options on just-left sort type
					switch (previousSortType)
					{
						case SortType.Size:
							config.showSizes = previousShowSizesValue;
							break;

						case SortType.AssetType:
							config.showTypes = previousShowTypesValue;
							break;

						case SortType.CreationTime:
							config.showCreationTime = previousShowCreationTimeValue;
							break;

						case SortType.ModificationTime:
							config.showLastWriteTime = previousShowLastWriteTimeValue;
							break;

						case SortType.LatestTime:
							config.showCreationTime = previousShowCreationTimeValue;
							config.showLastWriteTime = previousShowLastWriteTimeValue;
							break;
					}

					// activate options on just-entered sort type
					switch (config.sortType)
					{
						case SortType.Size:
							previousShowSizesValue = config.showSizes;
							config.showSizes = true;
							break;

						case SortType.AssetType:
							previousShowTypesValue = config.showTypes;
							config.showTypes = true;
							break;

						case SortType.CreationTime:
							previousShowCreationTimeValue = config.showCreationTime;
							config.showCreationTime = true;
							break;

						case SortType.ModificationTime:
							previousShowLastWriteTimeValue = config.showLastWriteTime;
							config.showLastWriteTime = true;
							break;

						case SortType.LatestTime:
							previousShowCreationTimeValue = config.showCreationTime;
							config.showCreationTime = true;
							previousShowLastWriteTimeValue = config.showLastWriteTime;
							config.showLastWriteTime = true;
							break;
					}
				}

				if (updateFromToggleButton(ref config.sortInReverse, "Reverse")) {
					setFunctionForGetResult(config.sortInReverse);
				}
			}
			GUILayout.EndHorizontal();
		}

		private void drawDisplayOptions()
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					updateFromToggleButton(ref config.showNameFirst, "Name first");
					updateFromToggleButton(ref config.showFolders, "Show folders");
					// updateFromToggleButton(ref config.showSubObjects, "Show sub objects");
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					if (updateFromToggleButton(ref config.showSizes, "Show sizes")) {
						previousShowSizesValue = config.showSizes;
					}
					if (updateFromToggleButton(ref config.showTypes, "Show types")) {
						previousShowTypesValue = config.showTypes;
					}
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					if (updateFromToggleButton(ref config.showCreationTime, "Show creation time")) {
						previousShowCreationTimeValue = config.showCreationTime;
					}
					if (updateFromToggleButton(ref config.showLastWriteTime, "Show modification time")) {
						previousShowLastWriteTimeValue = config.showLastWriteTime;
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		private void drawSpecialFilters()
		{
			updateFromPopup(ref config.specialFilterType, "Special filters", specialFiltersOptions);

			if (config.specialFilterType == SpecialFilterType.None) {
				return;
			}

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(20);

				GUILayout.BeginVertical();
				{
					if (config.specialFilterType == SpecialFilterType.TextureFormat)
					{
						updateFromPopup(ref config.textureFormat, "Format ", textureFormatOptions);
					}
					else if (config.specialFilterType == SpecialFilterType.ComponentsInPrefabs)
					{
						bool didSomethingChange = false;

						didSomethingChange |= drawStringMatchingConfigForSearchComponentsInPrefabs("Component name", config.componentName);
						didSomethingChange |= drawStringMatchingConfigForSearchComponentsInPrefabs("Variable name", config.variableName);
						didSomethingChange |= drawStringMatchingConfigForSearchComponentsInPrefabs("Variable value", config.variableValue);
						didSomethingChange |= updateFromToggleButton(ref config.doSearchChildren, "Also search in children");

						if (didSomethingChange)
						{
							if (isSlowSearch() && globalConfig.delayForSlowSearch > 0) {
								baseTimeForSearch = EditorApplication.timeSinceStartup;
							} else {
								updateDatabaseAndSearch();
							}
						}
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		private bool drawStringMatchingConfigForSearchComponentsInPrefabs(string label, StringMatchingConfig matching)
		{
			bool didSomethingChange = false;

			GUILayout.BeginHorizontal();
			{
				string newValue = EditorGUILayout.TextField(label, matching.String, GUILayout.ExpandWidth(true));
				if (newValue != matching.String) {
					matching.String = newValue;
					didSomethingChange = true;
				}

				didSomethingChange |= updateFromPopup(ref matching.MatchingType, "", matchingOptions, GUILayout.Width(60));
				didSomethingChange |= updateFromToggleButton(ref matching.IsCaseSensitive, "Case sens.", GUILayout.Width(80));
			}
			GUILayout.EndHorizontal();

			return didSomethingChange;
		}

		private void drawSavedSearches()
		{
			globalConfig.showSavedSearches = EditorGUILayout.Foldout(globalConfig.showSavedSearches, "Saved searches");
			if (!globalConfig.showSavedSearches) {
				return;
			}

			float height = getHeightOfSavedSearchesScrollView();

			savedSearchesScrollPosition = GUILayout.BeginScrollView(savedSearchesScrollPosition, false, false,
				GUIStyle.none, GUI.skin.verticalScrollbar, styles.resultsScrollViewStyle, GUILayout.Height(Mathf.Min(height, 5*22f)));
			{
				if (data.searchConfigAutosave != null)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(-1);

						if (GUILayout.Button("[auto-save] " + data.searchConfigAutosave.Representation, styles.savedSearchButtonStyle)) {
							data.config = SearchConfig.Clone(data.searchConfigAutosave);
							setupAfterConfigLoad();
							updateDatabaseAndSearch();
						}
					}
					GUILayout.EndHorizontal();
				}

				int toDeleteIndex = -1;

				for (int i = 0; i < data.savedConfigs.Count; i++)
				{
					SearchConfig savedConfig = data.savedConfigs[i];

					GUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("x", GUILayout.MaxWidth(24))) {
							toDeleteIndex = i;
						}

						GUILayout.Space(-6);

						if (GUILayout.Button(savedConfig.Representation, styles.savedSearchButtonStyle)) {
							if (data.lastSelectedSavedConfig == null || data.lastSelectedSavedConfig != config.search) {
								data.searchConfigAutosave = SearchConfig.Clone(config);
							}
							data.lastSelectedSavedConfig = savedConfig.search;
							data.config = SearchConfig.Clone(savedConfig);
							setupAfterConfigLoad();
							updateDatabaseAndSearch();
						}
					}
					GUILayout.EndHorizontal();
				}

				if (toDeleteIndex != -1) {
					data.savedConfigs.RemoveAt(toDeleteIndex);
				}
			}
			GUILayout.EndScrollView();
		}

		private void drawWarningsAndErrors()
		{
			#if !UNITY_5_5_OR_NEWER
			string warningsString = null;
			if (config.sortType == SortType.AssetType) {
				warningsString = "<color=yellow><b>WARNING:</b></color> Unity version is older than 5.5, so Seek loads ALL assets into memory to sort by type!";
			} else if (config.showTypes) {
				warningsString = "<color=yellow><b>WARNING:</b></color> Unity version is older than 5.5, so Seek loads each asset into memory to display its type!";
			}
			if (!string.IsNullOrEmpty(warningsString)) {
				GUILayout.Label(warningsString, styles.messageLabelStyle);
			}
			#endif

			if (!string.IsNullOrEmpty(errorsString)) {
				GUILayout.Label("<color=red><b>ERROR:</b></color> " + errorsString, styles.messageLabelStyle);
			}
		}

		private void drawResults()
		{
			bool hasErrors = !string.IsNullOrEmpty(errorsString);

			if (!hasErrors) {
				if (results.Count > 0) {
					GUILayout.Label("Showing " + results.Count + " results.", EditorStyles.boldLabel);
				} else {
					GUILayout.Label("No results to show.", EditorStyles.boldLabel);
				}
			}

			resultsScrollPosition = GUILayout.BeginScrollView(resultsScrollPosition, styles.resultsScrollViewStyle);
			{
				if (!hasErrors) {
					showSearchResults();
				}
			}
			GUILayout.EndScrollView();

			if (Event.current.type == EventType.Repaint) {
				resultsScrollArea = GUILayoutUtility.GetLastRect();
			}
		}

		// This method is a hack that allows creating the item size slider before the results list
		// but still placing it at the bottom of the window. It does not use GUILayout but mimics its sizes.
		// It's needed because the GUILayout slider becomes stuck at certain values while dragging its handle
		// because of the dynamically changing content above it (somehow!).
		private void drawItemSizeSliderAtBottom()
		{
			const float height = 18;
			float y = this.position.height - height;
			float x;

			const float widthOfPreSlider = 61;
			const float widthOfPostSlider = 43;
			float widthOfSlider = this.position.width - widthOfPreSlider - widthOfPostSlider;

			x = 3;
			GUI.Label(new Rect(x, y, widthOfPreSlider, height), "Item size");

			x += widthOfPreSlider;
			globalConfig.itemSize = (int) GUI.HorizontalSlider(new Rect(x, y, widthOfSlider, height), globalConfig.itemSize, 13, 36);

			x += widthOfSlider + 4;
			GUI.Label(new Rect(x, y, widthOfPostSlider, height), globalConfig.itemSize + "px");
		}

		// The commented-out part is what the method SHOULD have been. Read more above.
		private void drawItemSizeSlider()
		{
			// GUILayout.BeginHorizontal();
			// {
			// 	GUILayout.Label("Item size", GUILayout.ExpandWidth(false));
			// 	globalConfig.itemSize = (int) GUILayout.HorizontalSlider(globalConfig.itemSize, 13, 36);
			// 	// GUILayout.FlexibleSpace();
			// 	GUILayout.Label(globalConfig.itemSize + "px", GUILayout.ExpandWidth(false));
			// }
			// GUILayout.EndHorizontal();
			GUILayout.Label("", GUILayout.Height(18));
		}

		private void drawSeparator()
		{
			GUILayout.Box("", styles.separatorStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1));
		}

		private void drawSpaceSeparator()
		{
			GUILayout.Space(7);
		}

		// ---------------------------------------------------------------------
		// ---------------------------------------------------------------------

		// called many times a second (docs mention 100, actually varies)
		void Update()
		{
			if (baseTimeForSearch > 0) {
				if (EditorApplication.timeSinceStartup - baseTimeForSearch > globalConfig.delayForSlowSearch) {
					updateDatabaseAndSearch();
					Repaint();
					baseTimeForSearch = 0;
				}
			}
		}

		private void setupAfterConfigLoad()
		{
			previousShowSizesValue = config.showSizes;
			previousShowTypesValue = config.showTypes;
			previousShowCreationTimeValue = config.showCreationTime;
			previousShowLastWriteTimeValue = config.showLastWriteTime;
		}

		private bool isSlowSearch()
		{
			return (config.searchType != SearchType.Unity && config.matchingType == MatchingType.Regex)	// regex is slow
				|| (config.sortType != SortType.Path && config.sortType != SortType.Name	// sort types that are not path or name are slow
					#if UNITY_5_5_OR_NEWER
					&& config.sortType != SortType.AssetType	// on Unity 5.5 or newer, asset type sorting is also fast
					#endif
				)
				|| config.specialFilterType == SpecialFilterType.TextureFormat	// special texture type filter is slow
				|| config.specialFilterType == SpecialFilterType.ComponentsInPrefabs	// special prefab search is slow
				#if !UNITY_5_5_OR_NEWER
				|| config.showTypes
				#endif
			;
		}

		// ---------------------------------------------------------------------
		// ---------------------------------------------------------------------

		private float getHeightOfSavedSearchesScrollView()
		{
			int savedSearchesCount = data.savedConfigs.Count;
			if (data.searchConfigAutosave != null) {
				savedSearchesCount++;
			}
			return savedSearchesCount * 22f + 2;
		}

		private bool updateFromToggleButton(ref bool value, string label, params GUILayoutOption[] layoutOptions)
		{
			bool newValue = GUILayout.Toggle(value, label, layoutOptions);
			if (newValue != value) {
				value = newValue;
				updateDatabaseAndSearch();
				return true;
			}
			return false;
		}

		private bool updateFromPopup<T>(ref T type, string label, string[] displayedOptions, params GUILayoutOption[] layoutOptions)
			where T : struct, System.IConvertible, System.IComparable
		{
			T newType = (T) (object) EditorGUILayout.Popup(label, System.Convert.ToInt32(type), displayedOptions, layoutOptions);
			if (newType.CompareTo(type) != 0) {
				type = newType;
				updateDatabaseAndSearch();
				return true;
			}
			return false;
		}

		private void updateDatabaseAndSearch(bool doResetScroll = true)
		{
			SeekSavedData.SetDirty(data);
			setFunctionForGetResult(config.sortInReverse);
			updateAssetDatabase();
			if (doResetScroll) {
				resultsScrollPosition = Vector2.zero;
			}
			doSearch();
		}

		private void updateAssetDatabase()
		{
			string query = "";

			if (config.searchType == SearchType.Unity) {
				query = config.search;
			}

			if (config.specialFilterType == SpecialFilterType.TextureFormat) {
				query += "t:Texture";
			} else if (config.specialFilterType == SpecialFilterType.ComponentsInPrefabs) {
				query += "t:Prefab";
			} else if (config.assetTypesMask != 0) {
				query += Utils.GetAssetTypesFilter((uint) config.assetTypesMask, unityAssetTypeStrings);
			}

			string[] guids = AssetDatabase.FindAssets(query, null);

			if (guids.Length > 0) {
				IEnumerable<string> newGuids;
				if (config.showSubObjects) {
					newGuids = guids;
				} else {
					newGuids = Utils.SkipConsecutiveDuplicates(guids);
				}
				assets = Utils.Map(newGuids, (guid) => new AssetInfo(AssetDatabase.GUIDToAssetPath(guid), guid));
			} else {
				assets = new AssetInfo[0];
			}
		}

		private void doSearch()
		{
			try
			{
				calculatePreliminarySearchResults();

				if (config.specialFilterType == SpecialFilterType.TextureFormat)
				{
					var newResults = new List<Result>();

					TextureFormat format = textureFormats[config.textureFormat];
					foreach (Result result in results)
					{
						var asset = AssetDatabase.LoadAssetAtPath(result.FullPath, typeof(Texture2D)) as Texture2D;
						if (asset != null) {
							if (asset.format == format) {
								newResults.Add(result);
							}
						}
					}

					results = newResults;
				}
				else if (config.specialFilterType == SpecialFilterType.ComponentsInPrefabs)
				{
					results = SeekWindow_Reflection.DoIntrospectionSearchOnPrefabs(results, config);
				}

				if (!config.showFolders)
				{
					var resultsWithoutFolders = new List<Result>(results.Count);
					foreach (var result in results) {
						#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
						if (!Directory.Exists(result.FullPath))
						#else
						if (!AssetDatabase.IsValidFolder(result.FullPath))
						#endif
						{
							resultsWithoutFolders.Add(result);
						}
					}

					results = resultsWithoutFolders;
				}

				sortResults();
				selections.ResetSelections();

				errorsString = null;
			}
			catch (System.Exception e)
			{
				errorsString = e.Message;
				errorsString += e.StackTrace;
				Debug.LogException(e);
			}
		}

		private void calculatePreliminarySearchResults()
		{
			results.Clear();

			// if search matches pattern for a GUID, try finding as GUID first
			if (config.search.Length == 32 && config.searchType != SearchType.GUID) {
				results.AddRange(Matching.FindMatchesInAssets_ByGUID(config.search, assets));
			}

			if (config.searchType == SearchType.Unity) {
				results.AddRange(Matching.GetResultsFromAllAssets(assets));
			} else if (config.searchType == SearchType.GUID) {
				results.AddRange(Matching.FindMatchesInAssets_ByGUID(config.search, assets));
			} else {
				System.Func<string, string> pathFilter = null;
				if (config.searchType == SearchType.Name) {
					pathFilter = (path) => Path.GetFileName(path);
				}

				if (config.matchingType == MatchingType.Greedy) {
					results.AddRange(Matching.FindGreedyMatchesInAssets(config.search, assets, !config.isCaseSensitive, pathFilter));
				} else if (config.matchingType == MatchingType.Regex) {
					results.AddRange(Matching.FindRegexMatchesInAssets(config.search, assets, !config.isCaseSensitive, pathFilter));
				} else {
					results.AddRange(Matching.FindMatchesInAssets(config.search, assets, !config.isCaseSensitive, pathFilter));
				}
			}
		}

		private void sortResults()
		{
			switch (config.sortType)
			{
				case SortType.Path:
					break;

				case SortType.Name:
					loadAllResultFileNames(results);
					results.Sort((a,b) => a.FileName.CompareTo(b.FileName));
					break;

				case SortType.Size:
					loadAllResultFileSizes(results);
					results.Sort((a,b) => a.FileSize.CompareTo(b.FileSize));
					break;

				case SortType.AssetType:
					loadAllResultTypes(results);
					results.Sort((a,b) => {
						if (a.TypeString == null) return -1;
						if (b.TypeString == null) return +1;
						return a.TypeString.CompareTo(b.TypeString);
					});
					break;

				case SortType.CreationTime:
					loadAllResultFileInfos(results);
					results.Sort((a,b) => b.CreationTime.CompareTo(a.CreationTime));	// reverse order because it's likely the most useful
					break;

				case SortType.ModificationTime:
					loadAllResultFileInfos(results);
					results.Sort((a,b) => b.LastWriteTime.CompareTo(a.LastWriteTime));	// reverse order because it's likely the most useful
					break;

				case SortType.LatestTime:
					loadAllResultFileInfos(results);
					results.Sort((a,b) => {
						// find the most recent time between creation and modification, for the two assets
						System.DateTime aLatestTime = a.CreationTime.CompareTo(a.LastWriteTime) > 0 ? a.CreationTime : a.LastWriteTime;
						System.DateTime bLatestTime = b.CreationTime.CompareTo(b.LastWriteTime) > 0 ? b.CreationTime : b.LastWriteTime;
						return bLatestTime.CompareTo(aLatestTime);	// reverse order because it's likely the most useful
					});
					break;
			}
		}

		private void showSearchResults()
		{
			if (Event.current.type == EventType.Used) {
				return;
			}

			// keep track of event values now, to be safe from Event.current changes while processing the results list
			int eventButton = Event.current.button;
			#if UNITY_EDITOR_OSX
			if (Event.current.control && eventButton == 0) {
				eventButton = 1;
			}
			#endif
			Vector2 mousePosition = Event.current.mousePosition;
			EventType eventType = Event.current.type;

			GUILayout.Space(3);

			Vector2 originalIconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(globalConfig.itemSize, globalConfig.itemSize));

			int trueItemSize = globalConfig.itemSize;

			if (eventType == EventType.Layout)
			{
				iniShowIndex = (int) (resultsScrollPosition.y / trueItemSize);
				iniShowIndex = Mathf.Clamp(iniShowIndex, 0, results.Count);
				endShowIndex = iniShowIndex + (int) Mathf.Ceil(this.position.height / trueItemSize);
				// endShowIndex = iniShowIndex + (int) Mathf.Ceil(resultsScrollArea.height / trueItemSize);
				endShowIndex = Mathf.Clamp(endShowIndex, 0, results.Count);
			}

			GUILayout.Space(iniShowIndex * trueItemSize);

			for (int i = iniShowIndex; i < endShowIndex; i++)
			{
				Result result = null;
				try {
					result = getResult(i);
				} catch {
					Debug.LogWarning("Index " + i + " is out of range [0," + results.Count + "]\nresultsScrollPosition.y is " + resultsScrollPosition.y);
					continue;
				}

				if (result.Representation == null) {
					loadResult(result);
				}

				// could be inside previous "if" if Unity didn't set cached icons to null for unknown reasons...
				if (result.Icon == null) {
					loadResultIcon(result);
				}

				GUILayout.BeginHorizontal(styles.resultsScrollViewStyle);

				GUILayout.Box(result.Icon, styles.resultIconStyle, GUILayout.Height(trueItemSize-4), GUILayout.Width(trueItemSize-4));

				if (eventType == EventType.MouseDown && result.Rect.Contains(mousePosition))
				{
					// keep track of selections before down, in case this transforms into a drag and we need to reset to those selections
					selectedIndexesBeforePressDown = new HashSet<int>(selections.GetSelectedIndexes());
					mouseDownPosition = mousePosition;
					mouseDownIndex = i;

					if (eventButton == 0) {
						if (!result.IsSelected || listSelectionsInput.AreModifiersPressed()) {
							listSelectionsInput.Select(i);
						}
					} else if (eventButton == 1) {
						if (!result.IsSelected) {
							listSelectionsInput.Select(i);
							Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(result.FullPath);
							EditorGUIUtility.PingObject(Selection.activeObject);
							setSelectedObjects();
						}
					}
				}

				if (mouseDownIndex != -1)
				{
					if (eventType == EventType.MouseUp && result.Rect.Contains(mousePosition))
					{
						if (eventButton == 0) {
							ClickType clickType = listSelectionsInput.Click(mouseDownIndex, doSelect: !listSelectionsInput.AreModifiersPressed());

							if (clickType == ClickType.Click) {
								Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(result.FullPath);
								EditorGUIUtility.PingObject(Selection.activeObject);
							} else if (clickType == ClickType.DoubleClick) {
								AssetDatabase.OpenAsset(Selection.activeObject);
							}

							setSelectedObjects();
						} else if (eventButton == 1) {
							showAssetsContextMenu();
						}
					}

					if (Event.current.type == EventType.MouseDrag && Vector2.Distance(this.mouseDownPosition, Event.current.mousePosition) > 6f)
					{
						mouseDownIndex = -1;
						dragResults();
					}
				}

				// return value doesn't matter, as we handle down and up ourselves above
				GUILayout.Toggle(result.IsSelected, result.Representation, styles.resultButtonStyle, GUILayout.Height(globalConfig.itemSize), GUILayout.Height(globalConfig.itemSize));

				if (eventType == EventType.Repaint) {
					result.Rect = GUILayoutUtility.GetLastRect();
				}

				GUILayout.EndHorizontal();
			}

			if (Event.current.type == EventType.DragExited)
			{
				if (selectedIndexesBeforePressDown != null) {
					selections.DeselectAll();
					foreach (int index in selectedIndexesBeforePressDown) {
						selections.Select(index);
					}
				}
			}

			if (eventType == EventType.MouseUp) {
				mouseDownIndex = -1;
			}

			GUILayout.Space((results.Count - endShowIndex) * trueItemSize);

			GUILayout.Space(3);

			EditorGUIUtility.SetIconSize(originalIconSize);	// fix for Unity 2017
		}

		private void dragResults()
		{
			// Clear out drag data
			DragAndDrop.PrepareStartDrag ();
			DragAndDrop.visualMode = DragAndDropVisualMode.Move;

			HashSet<int> selectedIndexes = selections.GetSelectedIndexes();
			var selectedResults = new List<Result>(selectedIndexes.Count);
			foreach (int index in selectedIndexes) {
				selectedResults.Add(getResult(index));
			}

			int nSelectedAssets = selectedResults.Count;
			var objs = new Object[nSelectedAssets];
			var paths = new string[nSelectedAssets];

			// Debug.Log("Dragging " + nSelectedAssets + " objects");

			for (int i = 0; i < nSelectedAssets; i++) {
				string path = selectedResults[i].FullPath;
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
				objs[i] = obj;
				paths[i] = path;
			}

			GUIUtility.hotControl = 0;
			DragAndDrop.objectReferences = objs;
			DragAndDrop.paths = paths;

			if (paths.Length == 1) {
				DragAndDrop.StartDrag(selectedResults[0].Path);
			} else {
				DragAndDrop.StartDrag("<Multiple>");
			}

			Event.current.Use();
		}

		private void setSelectedObjects()
		{
			HashSet<int> selectedIndexes = selections.GetSelectedIndexes();
			var objects = new Object[selectedIndexes.Count];

			int i = 0;
			foreach (int index in selectedIndexes) {
				objects[i] = AssetDatabase.LoadMainAssetAtPath(getResult(index).FullPath);
				i++;
			}

			Selection.objects = objects;
		}

		private void setFunctionForGetResult(bool sortInReverse)
		{
			if (sortInReverse) {
				getResult = ((index) => results[results.Count-1-index]);
			} else {
				getResult = ((index) => results[index]);
			}
		}

		private void showAssetsContextMenu()
		{
			Vector2 mousePos = Event.current.mousePosition;
			EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Assets/", null);
		}

		private void loadResult(Result result)
		{
			var sb = new System.Text.StringBuilder();

			if (config.sortType == SortType.LatestTime && config.showCreationTime && config.showLastWriteTime)
			{
				if (!result.HasLoadedFileInfo) {
					loadResultFileInfo(result);
				}

				System.DateTime latestTime;
				string textColor;
				if (result.CreationTime.CompareTo(result.LastWriteTime) > 0) {
					latestTime = result.CreationTime;
					textColor = globalConfig.fileCreationTimeColor;
				} else {
					latestTime = result.LastWriteTime;
					textColor = globalConfig.fileLastWriteTimeColor;
				}
				sb.Append(string.Format("<color={1}>{0}</color> ", latestTime.ToString(DateTimeFormat), textColor));
			}
			else {
				if (config.showCreationTime)
				{
					if (!result.HasLoadedFileInfo) {
						loadResultFileInfo(result);
					}
					sb.Append(string.Format("<color={1}>{0}</color> ", result.CreationTime.ToString(DateTimeFormat), globalConfig.fileCreationTimeColor));
				}

				if (config.showLastWriteTime)
				{
					if (!result.HasLoadedFileInfo) {
						loadResultFileInfo(result);
					}
					sb.Append(string.Format("<color={1}>{0}</color> ", result.LastWriteTime.ToString(DateTimeFormat), globalConfig.fileLastWriteTimeColor));
				}
			}

			if (config.showTypes)
			{
				if (result.TypeString == null) {
					loadResultType(result);
				}
				sb.Append(string.Format("<color={1}>{0}</color> ", result.TypeString, globalConfig.fileTypeColor));
			}

			if (config.showSizes)
			{
				if (result.FileSize == 0) {
					loadResultFileSize(result);
				}
				if (result.FileSize != -1) {
					long val = result.FileSize;
					string unit = Utils.ConvertSizeToReadableUnit(ref val);
					sb.Append(string.Format("<color={2}>{0}{1}</color> ", val, unit, globalConfig.fileSizeColor));
				}
			}

			sb.Append(richTextResult(result));

			if (config.searchType == SearchType.GUID) {
				sb.Append(string.Format(" <color={1}>{0}</color>", result.GUID, globalConfig.guidColor));
			}

			result.Representation = sb.ToString();
		}

		private static void loadResultType(Result result)
		{
			#if UNITY_5_5_OR_NEWER
			result.TypeString = AssetDatabase.GetMainAssetTypeAtPath(result.FullPath).ToString();
			#else
			Object obj = AssetDatabase.LoadMainAssetAtPath(result.FullPath);
			if (obj != null) {
				result.TypeString = obj.GetType().ToString();
			}
			#endif

			if (result.TypeString == null) {
				// set a default to avoid loading the asset every OnGUI
				result.TypeString = "[unknown]";
			} else {
				result.TypeString = result.TypeString.Replace("UnityEngine.", "");
				// result.TypeString = result.TypeString.Replace("UnityEditor.", "");
			}
		}

		private static void loadResultFileSize(Result result)
		{
			result.FileSize = Utils.GetCompressedFileSize(result.GUID);
		}

		private static void loadResultFileInfo(Result result)
		{
			result.CreationTime = System.IO.File.GetCreationTime(result.FullPath);
			result.LastWriteTime = System.IO.File.GetLastWriteTime(result.FullPath);
			result.HasLoadedFileInfo = true;
		}

		private static void loadResultIcon(Result result)
		{
			result.Icon = AssetDatabase.GetCachedIcon(result.FullPath);
		}

		private static void loadAllResultFileNames(List<Result> results)
		{
			foreach (var result in results) {
				result.FileName = Path.GetFileName(result.Path);
			}
		}

		private static void loadAllResultTypes(List<Result> results)
		{
			foreach (var result in results) {
				loadResultType(result);
			}
		}

		private static void loadAllResultFileSizes(List<Result> results)
		{
			foreach (var result in results) {
				loadResultFileSize(result);
			}
		}

		private static void loadAllResultFileInfos(List<Result> results)
		{
			foreach (var result in results) {
				loadResultFileInfo(result);
			}
		}

		private string richTextResult(Result result)
		{
			List<Slice> slices;

			if (result.GetType() == typeof(ScatteredResult)) {
				slices = ((ScatteredResult) result).Slices;
			} else {
				var simpleResult = (SimpleResult) result;
				slices = new List<Slice>();
				slices.Add(new Slice {
					Index = simpleResult.Index,
					EndIndex = simpleResult.Index + simpleResult.Length
				});
			}

			string folderName = Path.GetDirectoryName(result.Path);
			int fileNameIndex;
			int fileNameEndIndex;
			string resultText;

			if (!config.showNameFirst)
			{
				resultText = result.Path;
				fileNameIndex = folderName.Length;
				if (result.Path[folderName.Length] == '/') {
					fileNameIndex++;
				}
				fileNameEndIndex = result.Path.Length;
			}
			else
			{
				if (result.Path[folderName.Length] == '/') {
					folderName += '/';
				}

				string fileName = result.Path.Substring(folderName.Length);
				resultText = fileName + " (" + folderName + ")";
				fileNameIndex = 0;
				fileNameEndIndex = fileName.Length;

				// copy slices instead of modifying them directly over the result
				if (result.GetType() == typeof(ScatteredResult))
				{
					var newSlices = new List<Slice>(slices.Count);
					foreach (var slice in slices) {
						newSlices.Add(new Slice(slice));
					}
					slices = newSlices;
				}

				fixSlicesForNameFirst(slices, folderName.Length, fileName.Length);
			}

			var richTextBuilder = new RichTextBuilder();
			richTextBuilder.AddColorSection(fileNameIndex, fileNameEndIndex, globalConfig.fileNameColor);
			foreach (var slice in slices) {
				richTextBuilder.AddBoldSection(slice.Index, slice.EndIndex);
			}
			return richTextBuilder.ProcessSections(resultText);
		}

		private void fixSlicesForNameFirst(List<Slice> slices, int folderNameLength, int fileNameLength)
		{
			for (int i = 0; i < slices.Count; i++)
			{
				Slice slice = slices[i];

				if (slice.EndIndex <= folderNameLength) {
					slice.Index    += fileNameLength + 2; // 2 chars = ' ' and '('
					slice.EndIndex += fileNameLength + 2; // 2 chars = ' ' and '('
				} else if (slice.Index >= folderNameLength) {
					slice.Index    -= folderNameLength;
					slice.EndIndex -= folderNameLength;
				} else {	// match covers folder and file
					slices.Add(new Slice {	// add using old indexes, because foreach will still cover it
						Index = folderNameLength,
						EndIndex = slice.EndIndex
					});
					slice.Index    += fileNameLength + 2; // 2 chars = ' ' and '('
					slice.EndIndex = folderNameLength;
					slice.EndIndex += fileNameLength + 2; // 2 chars = ' ' and '('
				}
			}
		}

		void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Settings"), false, () => SeekSettingsWindow.OpenSeekSettingsWindow());
		}
	}
}
