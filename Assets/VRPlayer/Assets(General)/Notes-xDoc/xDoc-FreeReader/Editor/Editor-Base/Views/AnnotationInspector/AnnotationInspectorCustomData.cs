using UnityEditor;
using UnityEngine;
using xDocBase.CustomData;
using xDocBase.UI;
using xDocEditorBase.AnnotationModule;
using xDocEditorBase.UI;


namespace xDocEditorBase.CustomData {

	public class AnnotationInspectorCustomData
	{
		readonly XDocAnnotationEditorBase annotationEditor;
		Vector2 scrollPosition;

		public AnnotationInspectorCustomData(
			XDocAnnotationEditorBase annotationEditor
		)
		{
			this.annotationEditor = annotationEditor;
		}


#region Draw

		public void Draw()
		{
			if (!annotationEditor.annotationType.data.showDataArea) {
				return;
			}
			float scrollAreaHeight = Mathf.Min(annotationEditor.annotation.maxHeightCD, CalculateHeight());

			using (var sv = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.Height(scrollAreaHeight))) {
				scrollPosition = sv.scrollPosition;
				DrawDataFieldList();
				if (annotationEditor.annotationType.data.useObjectReferencesList) {
					DrawObjectReferenceList();
				}
			}
		}

		float CalculateHeight()
		{
			float lh = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			float h = EditorGUIUtility.standardVerticalSpacing;
			if (annotationEditor.guiStateManager.isInViewMode) {
				var numDataFields = annotationEditor.annotation.dataFieldList.Count;
				for (int i = 0; i < numDataFields; i++) {
					if (annotationEditor.annotation.dataFieldList[i].showInViewMode)
						h += lh;
				}
			} else {
				var numDataFields = annotationEditor.annotation.dataFieldList.Count;
				for (int i = 0; i < numDataFields; i++) {
					if (annotationEditor.annotation.dataFieldList[i].showInEditMode)
						h += lh;
				}
			}
			if (annotationEditor.annotationType.data.useObjectReferencesList) {
				h += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(annotationEditor.spObjectReferenceList);
			}
			return h;
		}

		void DrawObjectReferenceList()
		{
			using (new EditorStateSaver.Indent(EditorGUI.indentLevel + 1))
			using (new StateSaver.BgColor(annotationEditor.annotationType.text.styleX.bgColor)) {
				EditorGUILayout.PropertyField(annotationEditor.spObjectReferenceList, true);
			}
		}

		void DrawDataFieldList()
		{
			// this is the list of custom data in the annotation - status quo - with the data
			//		List<DataField> dataList = aData.annotation.dataFieldList;
			// this is the list of custom data (types) in the annotationType! - status how it should be - without the data
			//		List<DataField> refList = aData.annotationType.data.dataFieldList;
			// old impl: if (!refList.IsIdSyncedWithOtherList(dataList)) {

			// both list have to match in order and content of the MASTER-Attributes
			//		if (!refList.IsMasterAttributeSyncedWithOtherList(dataList)) {
			//			// if there is a mismatch, the list in the annotation has to be rebuild
			//			aData.annotation.dataFieldList = refList.GetMasterAttributeSyncedList(dataList);
			//			aData.serializedObject.ApplyModifiedProperties();
			//			aData.serializedObject.Update();
			//		}
			// just now we can draw the list of custom data
			int maxItem = annotationEditor.spDataFieldList.arraySize;
			for (int i = 0; i < maxItem; i++) {
				DrawSingleDataField(annotationEditor.spDataFieldList.GetArrayElementAtIndex(i), i);
			}
		}

		void DrawSingleDataField(
			SerializedProperty spDataField,
			int index
		)
		{
			var dataField = annotationEditor.annotation.dataFieldList[index];

			if (annotationEditor.guiStateManager.isInViewMode) {
				// in view mode
				if (!dataField.showInViewMode) {
					return;
				}
			} else {
				// in edit mode
				if (!dataField.showInEditMode) {
					return;
				}
			}

			var dataFieldEditor = new DataFieldPropertyEditor(spDataField);
			
			string focusId = Focus.FocusId.SetNextId(annotationEditor.controlGroupBaseId, "CD", index);
			using (new StateSaver.BgColor(annotationEditor.annotationType.text.styleX.bgColor)) {
				EditorGUILayout.PropertyField(dataFieldEditor.value, new GUIContent(dataFieldEditor.nameValue));
			}

			switch (dataField.dataFieldType) {
			case DataField.DataFieldType.DateSpan:
				ValidateProcessDateField(dataField.dateSpanValue, focusId);
				break;
			case DataField.DataFieldType.Date:
				ValidateProcessDateField(dataField.dateValue, focusId);
				break;
			}
		}

#endregion


#region Date Validation

		void ValidateProcessDateField(
			DataFieldTypeDate dateValue,
			string focusId
		)
		{
			dateValue.dateHadFocus = dateValue.dateHasFocus;
			dateValue.dateHasFocus = annotationEditor.focusManager.HasFocus(focusId);

			// check, if the date field is being displayed/created the first time
			// if it has the date 1.1.1 it then will be set to today
			if (dateValue.InitializeIfNeeded()) {
				annotationEditor.serializedObject.Update();
			}

			// check if the date has been edited and has now an invalid value.
			// in this case rectify.
			// As we want to have undo, all gets a bit more complicated, esp. when to consider
			// not to interfere with currently being edited fields
			if (dateValue.dateHasFocus) {
				if (!dateValue.dateHadFocus) {
					dateValue.undoGroupIndex = Undo.GetCurrentGroup();
				}
			} else {
				if (dateValue.dateHadFocus)
				if (dateValue.DateIsBogus()) {
					Undo.RegisterFullObjectHierarchyUndo(annotationEditor.annotation, "Set Date");
					dateValue.RectifyIfNeeded();
					Undo.CollapseUndoOperations(dateValue.undoGroupIndex);
					annotationEditor.serializedObject.Update();
				}
			}
		}

#endregion
		
	}
}

