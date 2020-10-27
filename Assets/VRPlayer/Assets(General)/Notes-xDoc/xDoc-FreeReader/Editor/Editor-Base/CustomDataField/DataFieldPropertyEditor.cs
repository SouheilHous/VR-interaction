using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using xDocBase.CustomData;
using xDocBase.UI;
using xDocEditorBase.AnnotationTypeModule;
using xDocEditorBase.Extensions;
using xDocEditorBase.UI;
using GridLayout = xDocEditorBase.UI.GridLayout;

namespace xDocEditorBase.CustomData {

	public class DataFieldPropertyEditor : PropertyEditor
	{
#region Main

		public DataFieldPropertyEditor(
			SerializedProperty sPropDataField
		)
			: base(
				sPropDataField
			)
		{
			name = sPropDataField.FindPropertyRelative("name");
			dataFieldType = sPropDataField.FindPropertyRelative("dataFieldType");
			id = sPropDataField.FindPropertyRelative("id");

			showInViewMode = sPropDataField.FindPropertyRelative("showInViewMode");
			showInEditMode = sPropDataField.FindPropertyRelative("showInEditMode");
			showInSceneMode = sPropDataField.FindPropertyRelative("showInSceneMode");

			InitValueHolderProperties(sPropDataField);
		}

		public void Init(
			string dataFieldName
		)
		{
			var idEditor = new QuasiUniqueIdPropertyEditor(id);
			idEditor.InitializeAsNewId();

			name.stringValue = dataFieldName;

			showInEditMode.boolValue = true;
			showInViewMode.boolValue = true;
			showInSceneMode.boolValue = false;

			// Set Values for date. The other have valid defaults
			var datePE = new DataFieldTypeDatePropertyEditor(dateValue);
			datePE.Set111();
			// Set Values for date. The other have valid defaults
			var dateSpanPE = new DataFieldTypeDateSpanPropertyEditor(dateSpanValue);
			dateSpanPE.Set111();
		}

#endregion


#region Attributes

		public SerializedProperty name;
		public SerializedProperty dataFieldType;
		// we need an id, for the case that the name property is changed
		public SerializedProperty id;

		public SerializedProperty showInViewMode;
		public SerializedProperty showInEditMode;
		public SerializedProperty showInSceneMode;

		public string nameValue { get { return name.stringValue; } }

#endregion


#region Value Holders

		public SerializedProperty stringValue;
		public SerializedProperty dateValue;
		public SerializedProperty dateSpanValue;
		public SerializedProperty timeValue;
		public SerializedProperty boolValue;
		public SerializedProperty intValue;
		public SerializedProperty floatValue;
		public SerializedProperty linkValue;
		public SerializedProperty gameObjectValue;
		public SerializedProperty objectValue;

		void InitValueHolderProperties(
			SerializedProperty sPropDataField
		)
		{
			stringValue = sPropDataField.FindPropertyRelative("stringValue");
			dateValue = sPropDataField.FindPropertyRelative("dateValue");
			dateSpanValue = sPropDataField.FindPropertyRelative("dateSpanValue");
			timeValue = sPropDataField.FindPropertyRelative("timeValue");
			boolValue = sPropDataField.FindPropertyRelative("boolValue");
			intValue = sPropDataField.FindPropertyRelative("intValue");
			floatValue = sPropDataField.FindPropertyRelative("floatValue");
			linkValue = sPropDataField.FindPropertyRelative("linkValue");
			gameObjectValue = sPropDataField.FindPropertyRelative("gameObjectValue");
			objectValue = sPropDataField.FindPropertyRelative("objectValue");
		}

		public SerializedProperty value {
			get {
				switch ((DataField.DataFieldType)dataFieldType.enumValueIndex) {
				case DataField.DataFieldType.String:
					return stringValue;
				case DataField.DataFieldType.Date:
					return dateValue;
				case DataField.DataFieldType.DateSpan:
					return dateSpanValue;
				case DataField.DataFieldType.Time:
					return timeValue;
				case DataField.DataFieldType.Bool:
					return boolValue;
				case DataField.DataFieldType.Int:
					return intValue;
				case DataField.DataFieldType.Float:
					return floatValue;
				case DataField.DataFieldType.Link:
					return linkValue;
				case DataField.DataFieldType.GameObject:
					return gameObjectValue;
				default:
					return objectValue;
				}
			}
		}

#endregion


#region Draw

		readonly static int[] glCols = {
			3,
			2,
			3,
			1,
			1,
			1
		};

		public static void ReorderableListDrawSettingsHeader(
			Rect rect
		)
		{
			rect.xMin += ReorderableList.Defaults.dragHandleWidth - ReorderableList.Defaults.padding;
			var gridLayout = new GridLayout(rect, glCols, EditorGUIUtility.standardVerticalSpacing);

			EditorGUI.LabelField(gridLayout.GetRectSingleRow(0), "Name");
			EditorGUI.LabelField(gridLayout.GetRectSingleRow(1), "Type");
			EditorGUI.LabelField(gridLayout.GetRectSingleRow(2), "Default Value");
			EditorGUI.LabelField(gridLayout.GetRectSingleRow(3), "Insp.");
			EditorGUI.LabelField(gridLayout.GetRectSingleRow(4), "Edit");
			EditorGUI.LabelField(gridLayout.GetRectSingleRow(5), "Scene");
		}

		public void ReorderableListDrawSettingsElement(
			Rect rect,
			AnnotationTypeDataFieldListPropertyEditor parent,
			int index
		)
		{
			this.parent = parent;
			var dataField = parent.parent.annotationType.data.dataFieldList[index];

			// this has to be retrieved before any controlId's are requested / assigned, so it does
			// not change and stays the parents control id.
			var currentControlId = Focus.FocusId.GetCurrentControlName();
			string focusIdDefaultValue;

			using (new EditorStateSaver.Indent()) {
				rect.height = EditorGUIUtility.singleLineHeight;
				rect.y += EditorGUIUtility.standardVerticalSpacing;
				GridLayout rt = new GridLayout(rect, glCols, EditorGUIUtility.standardVerticalSpacing);

				// name
				Focus.FocusId.SetSubId(currentControlId, "N");
				EditorGUI.PropertyField(rt.GetRectSingleRow(0), name, GUIContent.none);

				// type
				Focus.FocusId.SetSubId(currentControlId, "T");
				EditorGUI.PropertyField(rt.GetRectSingleRow(1), dataFieldType, GUIContent.none);

				// default value
				focusIdDefaultValue = Focus.FocusId.SetSubId(currentControlId, "D");
				EditorGUI.PropertyField(rt.GetRectSingleRow(2), value, GUIContent.none);
				DefaultValueValidator(dataField, focusIdDefaultValue);

				// show in view mode
				Focus.FocusId.SetSubId(currentControlId, "V");
				EditorGUI.PropertyField(rt.GetRectSingleRow(3), showInViewMode, GUIContent.none);

				// show in edit mode
				Focus.FocusId.SetSubId(currentControlId, "E");
				EditorGUI.PropertyField(rt.GetRectSingleRow(4), showInEditMode, GUIContent.none);

				// show in scene
				Focus.FocusId.SetSubId(currentControlId, "S");
				EditorGUI.PropertyField(rt.GetRectSingleRow(5), showInSceneMode, GUIContent.none);
			}	


		}

#endregion


#region Date Validation

		void DefaultValueValidator(
			DataField dataField,
			string focusIdDefaultValue
		)
		{
			switch (dataField.dataFieldType) {
			case DataField.DataFieldType.DateSpan:
				ValidateProcessDateField(dataField.dateSpanValue, focusIdDefaultValue);
				break;
			case DataField.DataFieldType.Date:
				ValidateProcessDateField(dataField.dateValue, focusIdDefaultValue);
				break;
			}
		}

		AnnotationTypeDataFieldListPropertyEditor parent;

		void ValidateProcessDateField(
			DataFieldTypeDate aDateValue,
			string focusId
		)
		{
			aDateValue.dateHadFocus = aDateValue.dateHasFocus;
			aDateValue.dateHasFocus = parent.focusManager.HasFocus(focusId);

			// no initialization in the annotation type for date objects

			// check if the date has been edited and has now an invalid value.
			// in this case rectify.
			// As we want to have undo, all gets a bit more complicated, esp. when to consider
			// not to interfere with currently being edited fields
			if (aDateValue.dateHasFocus) {
				if (!aDateValue.dateHadFocus) {
					aDateValue.undoGroupIndex = Undo.GetCurrentGroup();
				}
			} else {
				if (aDateValue.dateHadFocus)
				if (aDateValue.DateIsBogus()) {
					Undo.RegisterFullObjectHierarchyUndo(serializedObject.targetObject, "Set Date");
					aDateValue.RectifyIfNeeded();
					Undo.CollapseUndoOperations(aDateValue.undoGroupIndex);
					serializedObject.Update();
				}
			}
		}

#endregion

	}
}
