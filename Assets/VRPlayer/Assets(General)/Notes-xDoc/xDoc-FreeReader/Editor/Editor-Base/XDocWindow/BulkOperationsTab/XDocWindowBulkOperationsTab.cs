using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using xDocBase;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.Extensions;
using xDocBase.UI;
using xDocEditorBase.AnnotationTypeModule;
using xDocEditorBase.Focus;
using xDocEditorBase.Search;
using xDocEditorBase.UI;


namespace xDocEditorBase.Windows {

	public class XDocWindowBulkOperationsTab : XoxEditorGUI.TwoPanelLayout
	{
		public override string name { get { return "Bulk Operations"; } }

		class SelectableAnnotation
		{

			public bool selected;
			public XDocAnnotationBase annotation;
			public XDocAnnotationTypeBase annotationType;

			public SelectableAnnotation(
				XDocAnnotationBase annotation
			)
			{
				this.annotation = annotation;
				annotationType = annotation.annotationTypeNotNull;
			}

			public static float height { 
				get { 
					return 4 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)
					- EditorGUIUtility.standardVerticalSpacing;
				}
			}

			static void DrawBg(
				Rect rect,
				Color bgColor
			)
			{
				using (new StateSaver.BgColor(bgColor)) {
					GUI.Box(rect, GUIContent.none);
				}
			}

			public static void DrawEmpty(
				Rect rect
			)
			{
				DrawBg(rect, AssetManager.settings.backgroundColorAnnotation);
				using (new StateSaver.BgColor(AssetManager.settings.backgroundColorEmpty)) {
					GUI.Box(rect, GUIContent.none);
				}

				// General Layout
				RectOffset ro = new RectOffset(5, 5, 2, 2);
				Rect cRect = ro.Remove(rect);
				Rect[] selContentRects = XoxGUI.SplitHorizontally(cRect, EditorStyles.toggle, 10);

				EditorGUI.LabelField(selContentRects[1], "List is empty - no matches.");
			}

			public void Draw(
				Rect rect
			)
			{			
				if (annotation == null) {
					DrawBg(rect, AssetManager.settings.backgroundColorAnnotation);
				} else if (annotation.gameObject.IsPrefab()) {
					DrawBg(rect, AssetManager.settings.backgroundColorPrefab);
				} else {
					DrawBg(rect, AssetManager.settings.backgroundColorAnnotation);
				}

				// General Layout
				RectOffset ro = new RectOffset(5, 5, 2, 2);
				Rect cRect = ro.Remove(rect);
				Rect[] selContentRects = XoxGUI.SplitHorizontally(cRect, EditorStyles.toggle, 10);

				if (annotation == null) {
					EditorGUI.LabelField(selContentRects[1], "Object Reference lost.");
					return;
				}

				// selection toggle
				selContentRects[0].height = EditorGUIUtility.singleLineHeight;
				selected = EditorGUI.Toggle(selContentRects[0], selected);

				// GameObject
				Rect aRect = selContentRects[1];
				aRect.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.ObjectField(aRect, annotation.gameObject, typeof(UnityEngine.Object), true);

				// Comment text area
				aRect.y += aRect.height + EditorGUIUtility.standardVerticalSpacing;
				aRect.height = 3 * EditorGUIUtility.singleLineHeight;
				using (new StateSaver.BgColor(annotationType.text.styleX.bgColor)) {
					EditorGUI.TextArea(
						aRect,
						annotation.comment,
						annotationType.text.styleX.style);
				}
			}
		}


#region Main

		public ReorderableList listOfAnnotationTypes;
		private Vector2 scrollPosForListOfAnnotationTypes;
		readonly List<SelectableAnnotation> listOfMatchedAnnotations;

		ResultsPagerSimple pager;

		BulkOperator bulkOperator;

		public XDocWindowBulkOperationsTab(
			XDocWindow parent
		)
			: base(
				parent
			)
		{
			listOfMatchedAnnotations = new List<SelectableAnnotation>();
			InitReorderableList();
			CreateFocusManager();

			bulkOperator = new BulkOperator();
			bulkOperator.scope = BulkOperationScope.Scope.CurrentlyLoadedScenes;

			InitWarnings();
			pager = new ResultsPagerSimple();

//		Refresh();

			// _HACK --- Undo.undoRedoPerformed += UndoDetected;
		}

		public override void OnDestroy()
		{
			// _HACK --- Undo.undoRedoPerformed -= UndoDetected;	
		}

		protected override void DrawCycleStart()
		{
			focusManager.ForceKeyPressPreCheck();
			// _FREQ --- 'TECH' this may need a redesign; get rid of the following and use the focusmanager
			if (Event.current != null) {
				switch (Event.current.type) {
				case EventType.KeyDown:
					if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
						// Analysis disable once ConvertIfToOrExpression
						if (focusManager.HasFocus(filterBase)) {
							filterApplyRequested = true;
						}
					}
					break;
				}
			}
		}

		protected override void DrawCycleEnd()
		{
			// This does not work, as the first KeyReturn in a textfield is used up and does not reach the 
			// end if the cycle. focusManager would need to be extended
			// instead we manually process events.
			// none the less we need the focusmanager update, so we can query the current focus
			focusManager.ForceUpdate();

			// _FREQ --- 'TECH' see DrawCycleStart comment about redesign
			switch (Event.current.type) {
			case EventType.Repaint:
				if (filterApplyRequested) {
					Refresh();
					filterApplyRequested = false;
				}
				break;
			}
		}

		/// <summary>
		/// Draws the left panel: List of Annotation Types to select from
		/// </summary>
		/// <param name="rect">Rect.</param>
		protected override void DrawLeftPanel(
			Rect rect
		)
		{
			using (new GUILayout.AreaScope(rect)) {
				using (var sv = new EditorGUILayout.ScrollViewScope(scrollPosForListOfAnnotationTypes)) {
					scrollPosForListOfAnnotationTypes = sv.scrollPosition;
					listOfAnnotationTypes.DoLayoutList();

					Color buttonColorInvalidTypes = GUI.backgroundColor;
					Color buttonColorAllTypes = GUI.backgroundColor;
					switch (listOfAnnotationTypes.index) {
					case -1:
						buttonColorInvalidTypes = GUI.skin.settings.selectionColor;
						break;
					case -2:
						buttonColorAllTypes = GUI.skin.settings.selectionColor;
						break;
					}

					using (new StateSaver.BgColor(buttonColorInvalidTypes)) {
						if (GUILayout.Button("Invalid Ann.Types")) {
							listOfAnnotationTypes.index = -1;
							Refresh();
						}
					}
					using (new StateSaver.BgColor(buttonColorAllTypes)) {
						if (GUILayout.Button("All Ann.Types")) {
							listOfAnnotationTypes.index = -2;
							Refresh();
						}
					}
				}	
			}
		}

		protected override void DrawRightPanel(
			Rect rect
		)
		{
			if (CheckAbort(rect)) {
				return;
			}
			var masterRect = XoxGUI.RemoveMargins(rect, AssetManager.settings.styleEditorWindowXContent.style);
			var filterResultsRects = XoxGUI.SplitVertically(
				                         masterRect,
				                         AssetManager.settings.styleEditorWindowXContentSub.style,
				                         filterHeight);
		
			DrawFilter(filterResultsRects[0]);
			DrawResults(filterResultsRects[1]);
		}

		const int numLinesFilter = 3;

		float filterHeight {
			get {
				return (numLinesFilter) * EditorGUIUtility.singleLineHeight
				+ (numLinesFilter + 1) * EditorGUIUtility.standardVerticalSpacing
				+ BulkOperationScope.scopeSelectionHeight + 2 * EditorGUIUtility.standardVerticalSpacing;
			}
		}

#endregion


#region Focus

		FocusManager	focusManager;
		const string filterBase = "XoxFilter";
		const string filterGOControlName = filterBase + "GO";
		const string filterAnnotationControlName = filterBase + "AC";

		void CreateFocusManager()
		{
			focusManager = new FocusManager();

			// _tidyup --- some outdoced stuff - keep it for later probable look up 
			// see comment in DrawCycleEnd!!
//		focusManager.AddOnKeyPressReturnTriggerStringCallback(
//			Refresh,
//			filterGOControlName);
//		focusManager.AddOnKeyPressReturnTriggerStringCallback(
//			Refresh,
//			filterAnnotationControlName);

		}


#endregion


#region Reorderable List Of Annotation Types

		void InitReorderableList()
		{
			listOfAnnotationTypes = new ReorderableList(
				AssetManager.annotationTypesAsset.annotationTypesList,
				typeof(XDocAnnotationTypeBase),
				false,
				true,
				false,
				false);
			listOfAnnotationTypes.onSelectCallback = CallbackAnnotationTypeSelected;
			listOfAnnotationTypes.drawHeaderCallback = CallbackDrawAnnotationTypesHeader;
			listOfAnnotationTypes.drawElementCallback = CallbackDrawElement;
		}

		void CallbackDrawElement(
			Rect rect,
			int index,
			bool isActive,
			bool isFocused
		)
		{
			try {
				EditorGUI.LabelField(rect, AssetManager.annotationTypesAsset.annotationTypesList[index].name);
			} catch {
				EditorGUI.LabelField(rect, "Error ATL@" + index);
			}
		}

		void CallbackDrawAnnotationTypesHeader(
			Rect rect
		)
		{
			EditorGUI.LabelField(rect, "Annotation Types");
		}

		void CallbackAnnotationTypeSelected(
			ReorderableList list
		)
		{
			Refresh();
		}

#endregion


#region List of Annotations

		bool CheckAbort(
			Rect rightPanelRect
		)
		{
			// check, if there is a valid selection from the list annotation types
			// otherwise abort
			bool abort = false;
			if (listOfAnnotationTypes == null)
				abort = true;
			// the next abort condition is taken out, bc. -1 are invalid types and -2 are all types for
			// bulk operation
//		else if (listOfAnnotationTypes.index < 0)
//			abort = true;
			if (abort) {
				using (new GUILayout.AreaScope(rightPanelRect)) {
					EditorGUILayout.HelpBox("Please, select an Annotation Type from the list to edit the details.", MessageType.Info);
				}
				return true;
			}
			return false;
		}

		readonly char[] wordSeps = {
			' ',
			'\t'
		};


		XDocAnnotationTypeBase selectedTypeForRefresh;
		string[] subSearchStringArrayForRefresh;

		/// <summary>
		/// Refresh list of matched annotations.
		/// The parameter is NOT needed for any functionality. It is not processed.
		/// 
		/// But this function is used as callback delegate, which takes a string as
		/// argument.
		/// </summary>
		/// <param name="controlName">Control name.</param>
		// Analysis disable once UnusedParameter
		void Refresh(
			string controlName = ""
		)
		{
			int selectedIndex = listOfAnnotationTypes.index;
			// Analysis disable once ConvertIfStatementToConditionalTernaryExpression
			if (selectedIndex < 0) {
				selectedTypeForRefresh = null;
			} else {
				selectedTypeForRefresh = AssetManager.annotationTypesAsset.annotationTypesList[selectedIndex];
			}

			listOfMatchedAnnotations.Clear();

			// If scope is all scenes there is no filter and no list of annotations will be 
			// shown to select annotations for a bulk operation. Just any encountered 
			// annotation will be processed in a requested bulk operation. 
			// So we can stop the refresh here.
			if (bulkOperator.isFullScope) {
				Repaint();
				return;
			}

			subSearchStringArrayForRefresh = filterAnnotationText.Split(wordSeps, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < subSearchStringArrayForRefresh.Length; i++) {
				subSearchStringArrayForRefresh[i] = subSearchStringArrayForRefresh[i].ToLower();
			}

			bulkOperator.Run(RefreshAddToList);

			pager.Initialize(listOfMatchedAnnotations.Count, AssetManager.settings.selectionItemsPerPage);

			Repaint();
		}

		bool allAnnotationTypesIsSelected {
			get { return listOfAnnotationTypes.index == -2; }
		}

		bool RefreshAddToList(
			XDocAnnotationBase annotation
		)
		{
			if (listOfMatchedAnnotations.Count >= AssetManager.settings.capTotalSelectionList) {
				return true;
			}
			if (annotation.annotationType == selectedTypeForRefresh || allAnnotationTypesIsSelected) {
				// 1. filter: GO name
				// _FREQ - allow search with substrings
				if (filterGameObject.Length > 0) {
					if (!annotation.gameObject.name.ToLower().Contains(filterGameObject.ToLower()))
						return false;
				}
				// 2. filter: annotation text
				bool matched = true;
				foreach ( var subSearchString in subSearchStringArrayForRefresh ) {
					if (!annotation.commentStripped.ToLower().Contains(subSearchString)) {
						matched = false;
						break;
					}
				}
				if (matched) {
					listOfMatchedAnnotations.Add(new SelectableAnnotation(annotation));
				}
			}
			return false;
		}

		void ClearFilter()
		{
			GUI.FocusControl("");
			filterGameObject = "";
			filterAnnotationText = "";
			Refresh();
		}

		void MainSelectChanged()
		{
			foreach ( var item in listOfMatchedAnnotations ) {
				item.selected = filterSelected;
			}
		}

		void UnselectAll()
		{
			filterSelected = false;
			foreach ( var item in listOfMatchedAnnotations ) {
				item.selected = false;
			}
		}

#endregion


#region BulkOperation

		XDocAnnotationTypeBase selectedAnnotationTypeForBulkOperation;

		void BulkChangeTypeShowMenu()
		{
			var typesMenu = new GenericMenu();

			List<XDocAnnotationTypeBase> atList = AssetManager.annotationTypesAsset.annotationTypesList;
			for (int i = 0; i < atList.Count; i++) {
				XDocAnnotationTypeBase runner = atList[i];
				if (selectedTypeForRefresh == runner) {
					// disabled
					typesMenu.AddDisabledItem(new GUIContent(runner.Name));
				} else {
					// enabled
					typesMenu.AddItem(new GUIContent(runner.Name), false, BulkChangeTypeShowMenuAction, i);
				}
			}

			typesMenu.DropDown(changeTypeButtonPosition);
		}

		void BulkChangeTypeShowMenuAction(
			object typeIndex
		)
		{
			var selectedIndex = (int)typeIndex;
			selectedAnnotationTypeForBulkOperation = AssetManager.annotationTypesAsset.annotationTypesList[selectedIndex];

			if (bulkOperator.isFullScope) {
				if (bulkOperator.confirmBulkOperation) {
					bulkOperator.Run(SetAnnotationTypeWithTypeCheck);
					FinalizeBulkOperation();
				}
			} else {
				var n = RunOverSelectedListItems(SetAnnotationType);
				if (n > 0) {
					FinalizeBulkOperation();
				} else {
					EditorUtility.DisplayDialog(
						"xDoc Change Annotation Types",
						"No annotation has been selected. Please select annotations to change their type.",
						"Close");					
				}
			}
		}

		void BulkDelete()
		{
			if (bulkOperator.isFullScope) {
				if (bulkOperator.confirmBulkOperation) {
					bulkOperator.Run(DeleteAnnotationWithTypeCheck);
					FinalizeBulkOperation();
				}
			} else {
				var n = RunOverSelectedListItems(DeleteAnnotation);
				if (n > 0) {
					FinalizeBulkOperation();
				} else {
					EditorUtility.DisplayDialog(
						"xDoc Info: Delete Annotations",
						"No annotation has been selected. Please select the annotations, you want to delete.",
						"Close");	
				}

				//			List<SelectableAnnotation> selList = new List<SelectableAnnotation>();
				//
				//			foreach ( var item in listOfMatchedAnnotations ) {
				//				if (item.selected)
				//				if (item.annotation != null)
				//					selList.Add(item);
				//			}
				//
				//			if (selList.Count == 0) {
				//				EditorUtility.DisplayDialog(
				//					"xDoc Info: Delete Annotations",
				//					"No annotation has been selected. Please select the annotations, you want to delete.",
				//					"Close");	
				//				return;
				//			}
				//
				//			if (EditorUtility.DisplayDialog("xDoc Warning: Delete Annotations!", 
				//				    "Are you sure you want to delete the selected annotations? In total "
				//				    + selList.Count + " annotations have been selected.", 
				//				    "Yes", "No")) {
				//				foreach ( var item in selList ) {
				//					DeleteAnnotation(item.annotation);
				//				}
				//				FinalizeBulkOperation();
				//			}
			}

		}

		int RunOverSelectedListItems(
			BulkOperator.SingleOperation singleOperation
		)
		{
			int selectedItems = 0;
			foreach ( var item in listOfMatchedAnnotations ) {
				if (item.selected)
				if (item.annotation != null) {
					selectedItems++;
					singleOperation(item.annotation);
				}
			}
			return selectedItems;

			// Implementation with lot of confirm dialogs, which is not really consistent,
			// as these bulk operations are undo-able
			//		List<SelectableAnnotation> selList = new List<SelectableAnnotation>();
			//
			//		foreach ( var item in listOfMatchedAnnotations ) {
			//			if (item.selected)
			//			if (item.annotation != null)
			//				selList.Add(item);
			//		}
			//
			//		if (selList.Count == 0) {
			//			EditorUtility.DisplayDialog(
			//				"xDoc Change Annotation Types",
			//				"No annotation has been selected. Please select annotations to change their type.",
			//				"Close");	
			//			return;
			//		}
			//
			//		foreach ( var item in selList ) {
			//			singleOperation(item.annotation);
			//		}
		}

		void FinalizeBulkOperation()
		{
			Repaint();
			EditorApplication.RepaintHierarchyWindow();		
			SceneView.RepaintAll();
			UnselectAll();
			Refresh();
		}

		bool DeleteAnnotationWithTypeCheck(
			XDocAnnotationBase annotation
		)
		{
			if (annotation.annotationType == selectedTypeForRefresh || allAnnotationTypesIsSelected) {
				return DeleteAnnotation(annotation);
			}
			return false;
		}

		bool DeleteAnnotation(
			XDocAnnotationBase annotation
		)
		{
			Undo.DestroyObjectImmediate(annotation);
			return false;
		}

		bool SetAnnotationTypeWithTypeCheck(
			XDocAnnotationBase annotation
		)
		{
			if (annotation.annotationType == selectedTypeForRefresh || allAnnotationTypesIsSelected) {
				return SetAnnotationType(annotation);
			}
			return false;
		}

		bool SetAnnotationType(
			XDocAnnotationBase annotation
		)
		{
			annotation.SetAnnotationType(selectedAnnotationTypeForBulkOperation);
			return false;
		}

#endregion


#region Draw

		Vector2 scrollPosition;

		bool filterSelected;
		string filterGameObject = "";
		string filterAnnotationText = "";
		// Analysis disable once RedundantDefaultFieldInitializer
		bool filterApplyRequested = false;
		// needed for the drop down menu
		Rect changeTypeButtonPosition;

		void DrawFilter(
			Rect rect
		)
		{
			// --------------------------------------------------------------------------
			// --- Draw a Bg for the filter section
			// --------------------------------------------------------------------------
			using (new StateSaver.BgColor(AssetManager.settings.backgroundColorFilter)) {
				GUI.Box(rect, GUIContent.none);
			}

			// --------------------------------------------------------------------------
			// --- Calc General Layout
			// --------------------------------------------------------------------------
			var ro = new RectOffset(5, 5, 2, 2);
			Rect cRect = ro.Remove(rect);
			// scope toolbar rect
			var scopeRect = new Rect(cRect);
			scopeRect.height = BulkOperationScope.scopeSelectionHeight;
			// rest of the area for the filter content
			cRect.yMin += BulkOperationScope.scopeSelectionHeight + 2 * EditorGUIUtility.standardVerticalSpacing;
			// slice between the filter toggle and the filter fields+buttons
			Rect[] selContentRects = XoxGUI.SplitHorizontally(cRect, EditorStyles.toggle, 10);

			// --------------------------------------------------------------------------
			// --- Draw
			// --------------------------------------------------------------------------

			// Scope Selection
			bulkOperator.DrawScopeSelection(scopeRect);
			if (bulkOperator.scopeHasChanged) {
				Refresh();
			}

			// selection toggle
			if (!bulkOperator.isFullScope) {
				using (new XoxEditorGUI.ChangeCheck(MainSelectChanged)) {
					selContentRects[0].height = EditorGUIUtility.singleLineHeight;
					filterSelected = EditorGUI.Toggle(selContentRects[0], filterSelected);
				}
			}

			// GameObject
			Rect aRect = selContentRects[1];
			if (!bulkOperator.isFullScope) {
				aRect.height = EditorGUIUtility.singleLineHeight;
				Focus.FocusId.SetNextId(filterGOControlName);
				filterGameObject = EditorGUI.TextField(aRect, "GameObject Name Filter", filterGameObject);

				// Comment text area
				aRect.y += aRect.height + EditorGUIUtility.standardVerticalSpacing;
				Focus.FocusId.SetNextId(filterAnnotationControlName);
				filterAnnotationText = EditorGUI.TextField(aRect, "Annotation Content Filter", filterAnnotationText);
			} else {		
				// warning
				aRect.height = 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				EditorGUI.HelpBox(aRect,
					"The scope of the operation is 'All Scenes'. " +
					"Filtering and selecting is not possible in this mode.", 
					MessageType.Info);
			}

			

			// Buttons
			aRect.y += aRect.height + EditorGUIUtility.standardVerticalSpacing;
			aRect.height = EditorGUIUtility.singleLineHeight;
			const int numButtons = 4;
			aRect.width = Mathf.Floor((aRect.width - EditorGUIUtility.standardVerticalSpacing * (numButtons - 1)) / numButtons);

			if (!bulkOperator.isFullScope)
			if (GUI.Button(aRect, "Apply Filter")) {
				Refresh();
			}

			aRect.x += aRect.width + EditorGUIUtility.standardVerticalSpacing;
			if (!bulkOperator.isFullScope)
			if (GUI.Button(aRect, "Clear Filter")) {
				ClearFilter();
			}

			aRect.x += aRect.width + EditorGUIUtility.standardVerticalSpacing;
			if (GUI.Button(aRect, "Bulk Change Type", EditorStyles.popup)) {
				BulkChangeTypeShowMenu();
			}
			changeTypeButtonPosition = new Rect(aRect);

			aRect.x += aRect.width + EditorGUIUtility.standardVerticalSpacing;
			if (GUI.Button(aRect, "Bulk Delete")) {
				BulkDelete();
			}
		}

		void DrawResults(
			Rect rect
		)
		{
			var vRect = new Rect(rect);
			vRect.xMax -= 20;
			const int spacing = 5;
			// +1 is for the pager
			vRect.height = (SelectableAnnotation.height + spacing) * (pager.numElements + 1);


			if (InvalidTypesWarningHasToBeShown()) {
				var hHeight = invalidTypesWarning.DrawInJustNeededSpace(rect);
				rect.yMin += hHeight + spacing;
			}
			if (ApplyToAllWarningHasToBeShown()) {
				var hHeight = appliedToAllWarning.DrawInJustNeededSpace(rect);
				rect.yMin += hHeight + spacing;
			}

			if (!bulkOperator.isFullScope) {
				using (var sv = new GUI.ScrollViewScope(rect, scrollPosition, vRect)) {
					scrollPosition = sv.scrollPosition;

					var itemRect = new Rect(vRect);
					itemRect.height = SelectableAnnotation.height;
					if (listOfMatchedAnnotations.Count > 0) {
						for (int i = pager.startElement; i < pager.endElement; i++) {
							var item = listOfMatchedAnnotations[i];
							item.Draw(itemRect);
							itemRect.y += itemRect.height + spacing;
						}

						// =================================================================================================================
						// === Number of Results
						// =================================================================================================================
						var resultsOverviewString = listOfMatchedAnnotations.Count + " results";
						if (listOfMatchedAnnotations.Count == AssetManager.settings.capTotalSelectionList) {
							resultsOverviewString += " (capped)";

						}
						resultsOverviewString += ". Showing "
						+ pager.startElementHumanReadable + " - " + pager.endElementHumanReadable
						+ " (page " + pager.currentPage + ")";

						// =================================================================================================================
						// === Pager
						// =================================================================================================================
						if (pager.Draw(itemRect, resultsOverviewString)) {
							scrollPosition = Vector2.zero;
						}
					} else {
						SelectableAnnotation.DrawEmpty(itemRect);
					}
				}
			}

		}

		XoxEditorGUI.WarningMessage invalidTypesWarning;
		XoxEditorGUI.WarningMessage appliedToAllWarning;

		bool InvalidTypesWarningHasToBeShown()
		{
			if (bulkOperator.isFullScope) {
				return listOfAnnotationTypes.index == -1; 
			} else {
				if (listOfMatchedAnnotations.Count > 0) {
					return listOfAnnotationTypes.index == -1; 
				} else {
					return false;
				}
			}
		}

		bool ApplyToAllWarningHasToBeShown()
		{
			return bulkOperator.isFullScope;
		}

		void InitWarnings()
		{
			const string invalidTypesMessage = "These Annotations don't have valid Annotation Types. " +
			                                   "Please, either assign Annotation Types or delete them.";
			const string appliedToAllMessage = "Bulk operation will be applied to all annotations " +
			                                   "of the selected type in ALL scenes and ALL assets. ";
			
			invalidTypesWarning = new XoxEditorGUI.WarningMessage(invalidTypesMessage, MessageType.Warning);
			appliedToAllWarning = new XoxEditorGUI.WarningMessage(appliedToAllMessage, MessageType.Warning);
		}

#endregion
	}
}

