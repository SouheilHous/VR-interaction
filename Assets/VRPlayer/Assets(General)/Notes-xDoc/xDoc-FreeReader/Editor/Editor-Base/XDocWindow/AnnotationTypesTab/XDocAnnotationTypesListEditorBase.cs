using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using xDocBase;
using xDocBase.AssetManagement;
using xDocBase.AnnotationTypeModule;
using xDocBase.Extensions;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	//	[CustomEditor(typeof(XDocAnnotationTypesListBase))]
	public abstract class XDocAnnotationTypesListEditorBase : Editor
	{
		#region Main

		// --------------------------------------------------------------------------
		// --- the target object
		// --------------------------------------------------------------------------
		// The object consists just out of the list anyway. So we directly just
		// keep the reference to the list.
		List<XDocAnnotationTypeBase> annotationTypesList;

		// --------------------------------------------------------------------------
		// --- serialized properties in the serialized object of the target object
		//		currently we dont have use for this
		// --------------------------------------------------------------------------
		// SerializedProperty spAnnotationTypesList;
		// This has to go into OnEnable
		// spAnnotationTypesList = serializedObject.FindProperty("annotationTypesList");

		// --------------------------------------------------------------------------
		// --- helper variables
		// --------------------------------------------------------------------------

		// this one hold the currently selected. We update it in the OnSelected callback
		// for the reorderable list, so we know the transition from-element to to-element.
		// the reorderable list gives us just the to-element. But we also need the
		// from-element so we can do some data validation
		XDocAnnotationTypeBase selectedAnnotationType;
		XDocAnnotationTypeEditorBase selectedAnnotationTypeEditor;

		void OnEnable ()
		{
			if ( target == null ) {
				return;
			}

			annotationTypesList = (target as XDocAnnotationTypesListBase).annotationTypesList;

			InitDrawList ();
		}

		public void OnLostFocus ()
		{
			if ( ListSelectionIsNotValid () ) {
				return;
			}
			if ( selectedAnnotationTypeEditor == null ) {
				return;
			}
			selectedAnnotationTypeEditor.OnLostFocus ();
		}

		// _HACK --- just utility, must be removed before realease
		//	void RemAll()
		//	{
		//		var gg = Resources.FindObjectsOfTypeAll<AnnotationType>();
		//
		//		foreach ( var g in gg ) {
		//			g.DestroyImmediateRecursively();
		//		}
		//		annotationTypesList.Clear();
		//		AssetDatabase.SaveAssets();
		//
		//	}

		#endregion


		#region drawList

		// HACK public was private
		public ReorderableList drawList;

		void InitDrawList ()
		{
			drawList = new ReorderableList (annotationTypesList, typeof(XDocAnnotationTypeBase), true, true, true, true);

			drawList.onAddCallback = Add;
			drawList.onRemoveCallback = Remove;
			drawList.drawElementCallback = DrawElement;
			drawList.drawHeaderCallback = DrawHeader;
			drawList.onCanRemoveCallback = CanBeRemoved;
			drawList.onSelectCallback = OnSelected;
		}

		public void EnsureUniquenessOfAnnotationTypeNamesCurrent ()
		{
			EnsureUniquenessOfAnnotationTypeNames (drawList.index);
		}

		public void EnsureUniquenessOfAnnotationTypeNames (
			int annotationTypeIndexToBeChecked
		)
		{
			annotationTypesList.UniquifyName (annotationTypeIndexToBeChecked, "Annotation", " v");
		}

		void DrawHeader (
			Rect rect
		)
		{
			EditorGUI.LabelField (rect, "Annotation Types");
		}

		void DrawElement (
			Rect rect,
			int index,
			bool isActive,
			bool isFocused
		)
		{
			if ( annotationTypesList[index] == null ) {
				EditorGUI.LabelField (rect, "<None> - Error, please contact support.");
				return;
			}
			EditorGUI.LabelField (rect, annotationTypesList[index].name);
		}

		void OnSelectedStart ()
		{
			if ( selectedAnnotationType != null ) {
				int index = annotationTypesList.IndexOf (selectedAnnotationType);
				EnsureUniquenessOfAnnotationTypeNames (index);
			}
		}

		void OnSelectedEnd (
			ReorderableList list
		)
		{
			selectedAnnotationType = annotationTypesList[list.index];
			EnsureUniquenessOfAnnotationTypeNames (list.index);
			selectedAnnotationTypeEditor = Editor.CreateEditor (selectedAnnotationType) as XDocAnnotationTypeEditorBase;
			selectedAnnotationTypeEditor.SetParent (this);
		}

		void OnSelected (
			ReorderableList list
		)
		{
			OnSelectedStart ();
			OnSelectedEnd (list);
		}

		void Add (
			ReorderableList list
		)
		{
			Focus.FocusUtility.LoseFocus ();
			OnSelectedStart ();
		 
			// Register Undo for changes in the list (the list is contained in this object)
			const string undoTitle = "Add Annotation Type";
			Undo.RegisterFullObjectHierarchyUndo (target, undoTitle);

			// Create the new asset - Annotation Type
			// just to remember, when all this wouldnt need to be split into a base and mother class			
			// var at = ScriptableSubAssetObject.CreateInstanceWithUndo<XDocAnnotationTypeBase>(target, undoTitle);
			var at = AssetManager.createXDocAnnotationTypeCallback (target, undoTitle);

			// Add the created annotationType to the list (in asset file)
			annotationTypesList.Add (at);

			// select the newly created annotationType in the drawn list; we have added the new element at the end
			list.index = list.count - 1;
			// The newly created AnnotationType must have a unique name; the next statement takes care of this.
			OnSelectedEnd (list);

			// Tell unity that there is stu	ff in the editor, that needs to be saved.
			EditorUtility.SetDirty (target);
		}

		void Remove (
			ReorderableList list
		)
		{
			/* There is no need to check for the validity of the index. The remove 
			 * button of the reordeable list is only enabled, when an list element
			 * is selected. 
			 * None the less we need to check, if there are currently annotations
			 * referencing to this type. If so, the user needs to delete all of them
			 * or change their types
			 */

			int index = drawList.index;

			if ( EditorUtility.DisplayDialog ("xDoc Warning: Delete Annotation Type!", 
				     "Are you sure you want to delete the annotation type '" + annotationTypesList[index].name + "'?", 
				     "Yes", "No") ) {

				Focus.FocusUtility.LoseFocus ();

				// Set effective AnnotationTypes to null
				BulkOperator bo = new BulkOperator ();
				bo.scope = BulkOperationScope.Scope.CurrentlyLoadedScenes;
				bo.Run (SetAnnotationTypeToNull, annotationTypesList[index]);

				// Register Undo for changes in the list (the list is contained in this object)
				const string undoTitle = "Remove Annotation Type";
				Undo.RegisterFullObjectHierarchyUndo (target, undoTitle);

				Undo.DestroyObjectImmediate (annotationTypesList[index]);
				ReorderableList.defaultBehaviours.DoRemoveButton (list);

				OnSelected (list);
				EditorApplication.RepaintHierarchyWindow ();
				SceneView.RepaintAll ();
			}
		}

		bool SetAnnotationTypeToNull (
			XDocAnnotationBase annotation
		)
		{
			annotation.SetAnnotationType (null);
			return false;
		}

		bool CanBeRemoved (
			ReorderableList list
		)
		{
			return list.count > 1;
		}

		#endregion


		#region Draw

		Vector2 scrollPos;

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector ();
		}

		public void DrawList (
			Rect rect
		)
		{
			if ( Event.current.type == EventType.ValidateCommand )
			if ( Event.current.commandName.StartsWith ("UndoRedoPerformed") ) {
				for ( int i = 0 ; i < drawList.count ; i++ ) {
					EnsureUniquenessOfAnnotationTypeNames (i);
				}
			}

			serializedObject.Update ();

//			drawList.DoList (rect);
			using ( new XoxEditorGUILayout.AreaScope (rect) ) {
				using ( var sv = new XoxEditorGUILayout.EditorScrollViewScope (scrollPos) ) {
					scrollPos = sv.scrollPosition;

					drawList.DoLayoutList ();

					// _HACK --- remove before release
//				if (GUILayout.Button("Remove")) {
//					RemAll();
//				}
				}	
			}		
		}

		//		class ATListGUI : XoxEditorGUI.ContentScope {
		//
		//		}

		// HACK: Needed for debug
		public XDocAnnotationTypeBase GetSelectedAnnotationType ()
		{
//			return annotationTypesList [drawList.index];
			return annotationTypesList[0];
		}


		public void DrawSelected (
			Rect rect
		)
		{
			if ( ListSelectionIsNotValid () ) {
				DrawNothingSelectedMessage (rect);
				return;
			}
			// check if a details editor has been created already - if not, create one on the fly
			// this is the best way to deal with this issue, as the undo mechanisms wont restore 
			// the editor.
			var at = annotationTypesList[drawList.index];
			if ( selectedAnnotationTypeEditor == null ) {
				selectedAnnotationTypeEditor = Editor.CreateEditor (at) as XDocAnnotationTypeEditorBase;
				selectedAnnotationTypeEditor.SetParent (this);
			}

			selectedAnnotationTypeEditor.Draw (rect);
		}

		bool ListSelectionIsNotValid ()
		{
			if ( annotationTypesList == null ) {
				return true;
			}
			if ( drawList == null ) {
				return true;
			} 
			if ( drawList.index >= drawList.count ) {
				drawList.index = -1;
			}
			if ( drawList.index < 0 ) {
				return true;
			}
			return annotationTypesList[drawList.index] == null;
		}

		static void DrawNothingSelectedMessage (
			Rect rect
		)
		{
			using ( new GUILayout.AreaScope (rect) ) {
				EditorGUILayout.HelpBox ("Please, select an Annotation Type from the list to edit the details.", MessageType.Info);
			}
		}

		#endregion


		#region Utility functions for the annotation type list

		/// <summary>
		/// <para>
		/// Returns an unique name for the annotation type.
		/// </para>
		/// 
		/// The reason for this funtion to be in this class, is that this class has a 
		/// reference to the List of annotation types.
		/// The editable text field is managed by an AnnotationTypeEditor, but this
		/// class manages just a single annotationType. For calculating a unique
		/// name we of course need the list.
		/// </summary>
		/// <returns>The unique annotation type name.</returns>
		/// <param name="baseName">Base name.</param>
		/// <param name="index">Index.</param>
		public string GetUniqueDataFieldName (
			string baseName,
			int index = -1
		)
		{
			return annotationTypesList.GetUniqueName (baseName, index, " v");
		}

		public string GetUniqueDataFieldName (
			string baseName,
			XDocAnnotationTypeBase at
		)
		{
			int index = annotationTypesList.IndexOf (at);
			return annotationTypesList.GetUniqueName (baseName, index, " v");
		}


		#endregion

	}
}

