using UnityEngine;
using xDocBase.UI;


namespace xDocBase.AssetManagement {

	// outdoced, bc this is an abstract class
	// [InitializeOnLoad]
	// [ExecuteInEditMode]
	[System.Serializable]
	public abstract class XDocSettingsBase : ScriptableObject
	{

#region Configurable by the user

		// --------------------------------------------------------------------------
		// --- User Configurable GUI Settings
		//     an Editor has to be provided for the user
		// --------------------------------------------------------------------------

		[Header("Search Tab")]
		public Color backgroundColorMain;
		public Color backgroundColorTitle;
		public int excerptLength;
		public int searchResultsPerPage;
		public int capTotalSearchResults;

		[Header("Bulk Operations")]
		public Color backgroundColorFilter;
		public Color backgroundColorAnnotation;
		public Color backgroundColorPrefab;
		public Color backgroundColorEmpty;
		public int selectionItemsPerPage;
		public int capTotalSelectionList;

#endregion


#region 'Fixed' Settings

		// --------------------------------------------------------------------------
		// --- Fixed GUI Settings
		//     just to be set by the developer, no (user) editor needed
		// --------------------------------------------------------------------------

		[Header("GUI")]
		public Texture xDocLogo;
		public Texture2D xDocLogoBig;
		public Texture2D xDocLogoBig2;
		public Texture2D whitePixel;
		public Texture2D iconSearch;
		public Texture2D iconToday;

		[Header("Search")]
		public Texture pagerXD;
		public Texture pagerYellowO;
		public Texture pagerRedO;
		public Texture pagerCLE;
		public Texture pagerPrevious;
		public Texture pagerNext;

		[Header("Search Entry Field")]
		public float searchEntryFieldRightPadding;
		public float searchEntryFieldMinWidth;
		public float searchEntryFieldMaxWidth;
		public float searchTopLineHeight;

		[Header("Initial Annotation Type Style")]
		public Texture2D defaultBGText;
		public Texture2D defaultBGSceneView;
		public Texture2D defaultBGIcon;
		public Texture2D defaultBGHierarchyText;

		[Header("Help")]
		public TextAsset helpText;
		public Object xDocManualA4;
		public Object xDocManualLetter;
		public Texture2D xDocManualA4Button;
		public Texture2D xDocManualLetterButton;
		public Texture2D xDocSupportForumButton;
		public Texture2D xDocTutorialsButton;
		public Texture2D xDocXoxInteractiveButton;
		public Texture2D xDocAssetStoreButton;

		[Header("Toolbar")]
		public Color	toolbarAddAnnotationButtonBG;

		[Header("Custom Data")]
		public Color	customDataBGColor;

		[Header("GUI StyleX's")]
		public GUIStyleX styleToolbar;
		public GUIStyleX styleToolbarExpanded;
		public GUIStyleX styleToolbarButton;
		public GUIStyleX styleToolbarPopup;
		public GUIStyleX styleToolbarDropDown;

		public GUIStyleX styleColorChooserButton;
		public GUIStyleX styleColorChooserButtonRow;

		public GUIStyleX styleVerticalSplitter;

		public GUIStyleX styleMiniButton;

		public GUIStyleX styleSearchEntryField;
		public GUIStyleX styleSearchPager;

		public GUIStyleX styleBulkOperationsPager;

		public GUIStyleX styleSearchOverviewLine;
		public GUIStyleX styleSearchItemLine;
		public GUIStyleX styleSearchItemText;
		public GUIStyleX styleSearchItemComponents;
		public GUIStyleX styleSearchItemParent;
		public GUIStyleX styleSearchItemParentSubItem;
		public GUIStyleX styleSearchItemChildSubItem;

		public GUIStyleX styleEditorWindowXContent;
		public GUIStyleX styleEditorWindowXContentSub;

		public GUIStyleX styleHelpText;

#endregion


	}
}
