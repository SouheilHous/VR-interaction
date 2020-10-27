using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xDocBase;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.Search;
using xDocEditorBase.UI;


namespace xDocEditorBase.Windows {

	public class XDocWindowSearchTab : XoxEditorGUI.SinglePanelLayout
	{
		public override string name { get { return "Search"; } }

		public override bool requestConstantRepaint { get { return false; } }


#region Main

		public XDocWindowSearchTab(
			XDocWindow parent
		)
			: base(
				parent
			)
		{
			InitSearch();
			InitDraw();
		}

#endregion


#region Search

		string searchString = "";
		bool searchDone = false;
		List<SearchMatchedAnnotation> listOfMatchedItems;
		SearchStringParser searchStringParser;
		ResultsPager pager;

		void InitSearch()
		{
			listOfMatchedItems = new List<SearchMatchedAnnotation>();
			searchStringParser = new SearchStringParser();
			pager = new ResultsPager();
		}


		void Search()
		{
			searchDone = true;

			listOfMatchedItems.Clear();
			if (searchString.Equals(string.Empty)) {
				Repaint();
				return;
			}

			searchStringParser.SetSearchString(searchString);
			var maxItems = AssetManager.settings.capTotalSearchResults;

			var allAnnotationsArray = Resources.FindObjectsOfTypeAll<XDocAnnotationBase>();
			var allAnnotationsArrayLength = allAnnotationsArray.Length;
			using (var pb = new XoxEditorGUI.ProgressBarIntCancelScan(allAnnotationsArrayLength, "annotations")) {
				for (int i = 0; i < allAnnotationsArrayLength; i++) {
					var annotation = allAnnotationsArray[i];
					var stringToSearch = annotation.commentStripped.ToLower();
					stringToSearch += "\n" + annotation.GetCustomDataString().ToLower();
					stringToSearch += "\n" + annotation.GetTagLayerStaticStringToSearch().ToLower();
					stringToSearch += "\n" + annotation.GetComponentsStringToSearch().ToLower();
					stringToSearch += "\n" + annotation.GetParentsStringToSearch().ToLower();
					stringToSearch += "\n" + annotation.GetChildrenStringToSearch().ToLower();
					stringToSearch += "\n" + annotation.GetGameObjectAndTypeStringToSearch().ToLower();
					if (searchStringParser.IsMatching(stringToSearch)) {
						listOfMatchedItems.Add(new SearchMatchedAnnotation(annotation));
						if (listOfMatchedItems.Count >= maxItems) {
							break;
						}
					}
					if (pb.SetCurrent(i)) {
						break;
					}
				}
			}

			pager.Initialize(listOfMatchedItems.Count, AssetManager.settings.searchResultsPerPage);

			Repaint();
		}

#endregion


#region Draw

		Vector2 scrollPos;
		GUIContent noMatchesMessage;
		float noMatchesMessageWidth;

		void InitDraw()
		{
			// no matches msg
			noMatchesMessage = new GUIContent("No matches.");
			Vector2 s = AssetManager.settings.styleSearchItemText.style.CalcSize(noMatchesMessage);
			noMatchesMessageWidth = s.x;
		}

		protected override void DrawPanel(
			Rect rect
		)
		{
			// --------------------------------------------------------------------------
			// --- just for a whiter bg of the windows
			// --------------------------------------------------------------------------
			var rect2 = AssetManager.settings.styleEditorWindowXContent.style.margin.Add(rect);
			rect2.yMin -= AssetManager.settings.styleEditorWindowXContent.style.padding.top;

			using (new StateSaver.GuiColor(AssetManager.settings.backgroundColorMain)) {
				GUI.DrawTexture(rect2, AssetManager.settings.whitePixel, ScaleMode.StretchToFill);
			}

			// --------------------------------------------------------------------------
			// --- functional UI
			// --------------------------------------------------------------------------
			if (listOfMatchedItems.Count == 0) {
				DrawEmptySearch(rect);
			} else {
				DrawSearchResults(rect2);
			}
		}

		void DrawEmptySearch(
			Rect rect
		)
		{
			// Logo
			Rect tRect = new Rect(rect);
			tRect.yMin += 100;
			tRect.x += Mathf.Floor((tRect.width - AssetManager.settings.xDocLogoBig.width) / 2);
			tRect.width = AssetManager.settings.xDocLogoBig.width;
			tRect.height = AssetManager.settings.xDocLogoBig.height;
//		GUI.DrawTexture(tRect, AssetManager.guiPrefs.xDocLogoBig, ScaleMode.StretchToFill);
			GUI.Box(tRect, AssetManager.settings.xDocLogoBig, GUIStyle.none);



			// Search Entry Field
			Rect sRect = new Rect(rect);
			const float fieldWidth = 300;
			sRect.yMin = tRect.yMax + EditorGUIUtility.singleLineHeight * 2;
			sRect.height = AssetManager.settings.styleSearchEntryField.style.fixedHeight;
			// height is removed too, bc its the width of the search button
			sRect.x += (sRect.width - fieldWidth - sRect.height) / 2;
			sRect.width = fieldWidth;

			using (new StateSaver.BgColor(AssetManager.settings.styleSearchEntryField.bgColor)) {
				searchString = EditorGUI.TextField(sRect, searchString, AssetManager.settings.styleSearchEntryField.style);
			}
			// Search button
			sRect.x += sRect.width;
			//		sRect.width = EditorGUIUtility.singleLineHeight;
			sRect.width = AssetManager.settings.styleSearchEntryField.style.fixedHeight;
			sRect.height = AssetManager.settings.styleSearchEntryField.style.fixedHeight;
			EditorGUIUtility.AddCursorRect(sRect, MouseCursor.Link);
			if (GUI.Button(sRect, AssetManager.settings.iconSearch, GUIStyle.none)) {
				Search();
			}

			Rect mRect = new Rect(rect);
			mRect.yMin = sRect.yMax + EditorGUIUtility.singleLineHeight;
			mRect.height = EditorGUIUtility.singleLineHeight;
			mRect.x += (mRect.width - noMatchesMessageWidth) / 2;
			mRect.width = noMatchesMessageWidth;
			if (searchDone) {
				if (!searchString.Equals(string.Empty))
					EditorGUI.LabelField(mRect, noMatchesMessage, AssetManager.settings.styleSearchItemText.style);
			}
			// Catch Return in the Entryfield
			if (Event.current.isKey) {
				searchDone = false;
				if (Event.current.keyCode == KeyCode.Return) { 
					Search();
				}
			}
		}

		void DrawSearchResults(
			Rect rect
		)
		{
			// the -20 accounts for the possible scrollbar - easiest solution
			var contentWidth = rect.width - 20;

			using (new GUILayout.AreaScope(rect)) {
				using (var sv = new EditorGUILayout.ScrollViewScope(scrollPos)) {
					scrollPos = sv.scrollPosition;

					// =================================================================================================================
					// === Top Search Line
					// =================================================================================================================
					var tRect = EditorGUILayout.GetControlRect(false, AssetManager.settings.searchTopLineHeight);
					tRect.yMin = 0;
					tRect.xMin = 0;
					tRect.xMax = EditorGUIUtility.currentViewWidth;
					DrawTopSearchLine(tRect);

					// =================================================================================================================
					// === Number of Results
					// =================================================================================================================
					var resultsOverviewString = listOfMatchedItems.Count + " results";
					if (listOfMatchedItems.Count == AssetManager.settings.capTotalSearchResults) {
						resultsOverviewString += " (capped)";

					}
					resultsOverviewString += ". Showing "
					+ pager.startElementHumanReadable + " - " + pager.endElementHumanReadable
					+ " (page " + pager.currentPage + ")";
					GUILayout.Label(resultsOverviewString, AssetManager.settings.styleSearchOverviewLine.style);

					// =================================================================================================================
					// === Results
					// =================================================================================================================
					for (int i = pager.startElement; i < pager.endElement; i++) {
						// --------------------------------------------------------------------------
						// --- Check, if something changed (scene load, deletion of objects, etc)
						// --------------------------------------------------------------------------
						var matchedAnnotation = listOfMatchedItems[i];
						if (matchedAnnotation.isInvalid) {
							EditorGUILayout.LabelField("Object Reference lost.");
							continue;
						}
						matchedAnnotation.DrawTitleLine(contentWidth);
						matchedAnnotation.DrawParents();
						matchedAnnotation.DrawChildren();
						matchedAnnotation.DrawExcerpt();
						matchedAnnotation.DrawComponents();
						matchedAnnotation.DrawCustomData();
						matchedAnnotation.DrawGOAttributes();
					}

					// =================================================================================================================
					// === Pager
					// =================================================================================================================
					if (pager.Draw()) {
						scrollPos = Vector2.zero;
					}

				}
			}

			if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) { 
				Search();
			}
		}

		void DrawTopSearchLine(
			Rect rect
		)
		{
			using (new StateSaver.GuiColor(AssetManager.settings.backgroundColorTitle))
				GUI.DrawTexture(rect, AssetManager.settings.whitePixel, ScaleMode.StretchToFill);

			// Logo
			Rect tRect = new Rect(rect);
//		tRect.width = AssetManager.guiPrefs.styleSearchItemLine.style.padding.left - 10;
			tRect.width = AssetManager.settings.xDocLogoBig2.width;
			tRect.xMin += 5;
//		GUI.DrawTexture(tRect, AssetManager.guiPrefs.xDocLogoBig2, ScaleMode.ScaleToFit);
			// _FIXME misuse of the pagerstyle
			GUI.Box(tRect, AssetManager.settings.xDocLogoBig2, AssetManager.settings.styleSearchPager.style);
			// draw an invisible button over the logo, which returns the user to the start page
			if (GUI.Button(tRect, GUIContent.none, GUIStyle.none)) {
				searchString = string.Empty;
				listOfMatchedItems.Clear();
				pager.Initialize(0, AssetManager.settings.searchResultsPerPage);
				Repaint();
			}
			EditorGUIUtility.AddCursorRect(tRect, MouseCursor.Link);

			// Search Entry Field
			var lineHeight = AssetManager.settings.styleSearchEntryField.style.fixedHeight;

			rect.xMin = AssetManager.settings.styleSearchItemLine.style.padding.left;
			rect.xMax -= AssetManager.settings.searchEntryFieldRightPadding;
			rect.width = Mathf.Clamp(
				rect.width,
				AssetManager.settings.searchEntryFieldMinWidth,
				AssetManager.settings.searchEntryFieldMaxWidth);
			rect.yMin += (rect.height - lineHeight) / 2;
			rect.height = lineHeight;
			DrawSearchField(rect);

		}

		void DrawSearchField(
			Rect rect
		)
		{
			using (new StateSaver.GuiColor(AssetManager.settings.backgroundColorMain))
				GUI.DrawTexture(rect, AssetManager.settings.whitePixel, ScaleMode.StretchToFill);
		
			using (new StateSaver.BgColor(AssetManager.settings.styleSearchEntryField.bgColor)) {
				searchString = EditorGUI.TextField(rect, searchString, AssetManager.settings.styleSearchEntryField.style);
			}
			// Search button
			rect.x += rect.width;
			EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
			if (GUI.Button(rect, AssetManager.settings.iconSearch, GUIStyle.none)) {
				Search();
			}		
		}

#endregion

	}
}

