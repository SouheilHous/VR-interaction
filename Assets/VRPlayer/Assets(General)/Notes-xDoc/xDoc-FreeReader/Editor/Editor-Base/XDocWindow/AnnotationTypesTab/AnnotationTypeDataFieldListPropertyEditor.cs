using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using xDocBase;
using xDocBase.AnnotationTypeModule;
using xDocBase.CustomData;
using xDocBase.Extensions;
using xDocEditorBase.CustomData;
using xDocEditorBase.Extensions;
using xDocEditorBase.Focus;
using xDocEditorBase.UI;
using xDocBase.AssetManagement;
using UnityEditor.VersionControl;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class AnnotationTypeDataFieldListPropertyEditor : PropertyEditor
	{

		#region Main

		public readonly AnnotationTypeDataEditor parent;
		BulkOperator bulkOperator;

		public AnnotationTypeDataFieldListPropertyEditor (
			SerializedProperty baseProperty, 
			AnnotationTypeDataEditor parent
		)
			: base (
				baseProperty
			)
		{
			this.parent = parent;
			bulkOperator = new BulkOperator ();
			CreateReorderableList ();
			CreateFocusManager ();
		}

		public void Draw (
			XoxGUIRect currentRect
		)
		{
			// Start Draw Cycle
			focusManager.ForceKeyPressPreCheck ();

			// ReorderableList
			currentRect.SetToHeight (dataFieldList.GetHeight ());
			dataFieldList.DoList (currentRect.rect);
			currentRect.MoveDown ();

			// Warning
			// old: ShowWarningApplyCustomData ();
			const string msg1 = "Changes on the custom data elements will have only effect on new annotations " +
			                    "or if the annotation type is changed - " +
			                    "existing annotations are not changed. " +
			                    "You have to 'Apply' the changes to take effect on existing annotations. ";
			GUIStyle style = EditorStyles.helpBox;
			Texture2D warnIcon = EditorGUIUtility.FindTexture ("console.warnicon");
			float height1 = style.CalcHeight (new GUIContent (msg1, warnIcon), currentRect.rect.width);
			currentRect.SetToHeight (height1);
			EditorGUI.HelpBox (currentRect.rect, msg1, MessageType.Warning);
			currentRect.MoveDown ();

			// Check/Apply Custom Data - Label
			currentRect.AddVSpace (5);
			currentRect.SetToLineHeight (1);
			EditorGUI.LabelField (currentRect.rect, "Check / Apply Custom Data Fields to Existing Annotations in", EditorStyles.boldLabel);
			currentRect.MoveDown ();

			// bulk Operator
			currentRect.SetToButtonHeight ();
			bulkOperator.DrawScopeSelection (currentRect.rect);
			currentRect.MoveDown ();

			// 2nd optional Warning
			if ( !bulkOperator.isFullScope ) {
				const string msg2 = "For the sake of data integrity, it is strongly recommended to apply " +
				                    "the changes to annotations of the relevant type in ALL SCENES.";
				float height2 = style.CalcHeight (new GUIContent (msg2, warnIcon), currentRect.rect.width);
				currentRect.SetToHeight (height2);
				EditorGUI.HelpBox (currentRect.rect, msg2, MessageType.Warning);
				currentRect.MoveDown ();
			}

			// indented Buttons
			currentRect.rect.xMin += EditorGUIUtility.labelWidth;

			// Check Data Integrity Button
			currentRect.SetToButtonHeight ();
			if ( GUI.Button (currentRect.rect, "Check Data Integrity") ) {
				CheckAndApplyCustomData (false);
			}
			currentRect.MoveDown ();

			// Apply Custom Data Button
			if ( GUI.Button (currentRect.rect, "Apply Custom Data to Existing Annotations") ) {
				if ( EditorUtility.DisplayDialog ("Apply Custom Data", 
					     "Please, be aware that 'Apply' is a bulk operation on existing annotations and " +
					     "that their data will be changed and/or lost (e.g. in case of deleted data fields). " +
					     "Do you want to continue?",
					     "Yes", "No") ) {
					CheckAndApplyCustomData (true);
				}
			}

			// End Draw Cycle
			focusManager.Update ();
		}

		public float GetHeight (
			float aWidth
		)
		{
			float height = 0;

			// ReorderableList
			height += dataFieldList.GetHeight ();
			height += XoxGUIRect.GetHeightOfMoveDownSpace ();

			// Warning
			// old: ShowWarningApplyCustomData ();
			const string msg1 = "Changes on the custom data elements will have only effect on new annotations " +
			                    "or if the annotation type is changed - " +
			                    "existing annotations are not changed. " +
			                    "You have to 'Apply' the changes to take effect on existing annotations. ";
			GUIStyle style = EditorStyles.helpBox;
			Texture2D warnIcon = EditorGUIUtility.FindTexture ("console.warnicon");
			height += style.CalcHeight (new GUIContent (msg1, warnIcon), aWidth);
			height += XoxGUIRect.GetHeightOfMoveDownSpace ();

			// Check/Apply Custom Data - Label
			height += XoxGUIRect.GetHeightOfVSpace (5);
			height += XoxGUIRect.GetHeightOfLines (1);
			height += XoxGUIRect.GetHeightOfMoveDownSpace ();

			// bulk Operator
			height += XoxGUIRect.GetHeightOfButton ();
			height += XoxGUIRect.GetHeightOfMoveDownSpace ();

			// 2nd optional Warning
			if ( !bulkOperator.isFullScope ) {
				const string msg2 = "For the sake of data integrity, it is strongly recommended to apply " +
				                    "the changes to annotations of the relevant type in ALL SCENES.";
				height += style.CalcHeight (new GUIContent (msg2, warnIcon), aWidth);
				height += XoxGUIRect.GetHeightOfMoveDownSpace ();
			}

			// Check Data Integrity Button
			height += XoxGUIRect.GetHeightOfButton ();
			height += XoxGUIRect.GetHeightOfMoveDownSpace ();

			// Apply Custom Data Button
			height += XoxGUIRect.GetHeightOfButton ();

			return height;
		}

		void CheckAndApplyCustomData (
			bool checkOrApply
		)
		{
			if ( !bulkOperator.confirmBulkOperation ) {
				return;
			}

			// create the data migration manager
			var customDataMigrationManager = new CustomDataMigrationManager (parent.annotationType, checkOrApply);
			// run the bulk operator
			bulkOperator.Run (customDataMigrationManager.Check);
			// display results
			customDataMigrationManager.DisplayResult (bulkOperator);
			SceneView.RepaintAll ();
		}

		class CustomDataMigrationManager
		{
			int totalAnnotations = 0;
			int relevantAnnotations = 0;
			int annotationsWithCustomDataNotMigrated = 0;

			readonly XDocAnnotationTypeBase annotationType;
			readonly bool checkAndApply;

			public CustomDataMigrationManager (
				XDocAnnotationTypeBase annotationType,
				bool checkAndApply
			)
			{
				this.checkAndApply = checkAndApply;
				this.annotationType = annotationType;
			}

			public bool Check (
				XDocAnnotationBase annotation
			)
			{
				totalAnnotations++;
				if ( annotation.annotationType == annotationType ) {
					relevantAnnotations++;
					if ( !annotation.dataFieldList.IsMasterAttributeSyncedWithOtherList (
						     annotationType.data.dataFieldList) ) {
						annotationsWithCustomDataNotMigrated++;
						if ( checkAndApply ) {
							if ( annotation.MigrateAndAdaptCustomData () ) {
								// Analysis disable once AccessToStaticMemberViaDerivedType
								EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
							}
						}
					}
				}
				return false;
			}

			public void DisplayResult (
				BulkOperationScope scope
			)
			{
				string additionalInformation = "";
				if ( scope.isSelection ) {
					additionalInformation =	scope.selectionInformation + "\n\n";
				}

				if ( checkAndApply ) {
					EditorUtility.DisplayDialog (
						"Annotations Custom Data Apply",
						"Annotation Type: " + annotationType.Name + "\n\n" +
						additionalInformation +
						"Total annotations: " + totalAnnotations + "\n" +
						"Annotations of relevant type: " + relevantAnnotations + "\n\n" +
						"Rectified annotations: " + annotationsWithCustomDataNotMigrated + "\n" 
					,
						"Close"
					);
				} else {
					EditorUtility.DisplayDialog (
						"Annotations Custom Data Check",
						"Annotation Type: " + annotationType.Name + "\n\n" +
						additionalInformation +
						"Total annotations: " + totalAnnotations + "\n" +
						"Annotations of relevant type: " + relevantAnnotations + "\n\n" +
						"Annotations, which need custom data re-applied: " + annotationsWithCustomDataNotMigrated + "\n" 
					,
						"Close"
					);
				}

			}
		}

		#endregion


		#region ReorderableList

		ReorderableList dataFieldList;

		void CreateReorderableList ()
		{
			dataFieldList = new ReorderableList (serializedObject, baseProperty, true, true, true, true);
			dataFieldList.drawElementCallback = DrawDataFieldElement;
			dataFieldList.drawHeaderCallback = DataFieldPropertyEditor.ReorderableListDrawSettingsHeader;
			dataFieldList.onAddCallback = AddDataField;
			dataFieldList.onRemoveCallback = RemoveDataField;
		}

		void DrawDataFieldElement (
			Rect rect,
			int index,
			bool isActive,
			bool isFocused
		)
		{
			DataFieldPropertyEditor dataField =
				new DataFieldPropertyEditor (dataFieldList.serializedProperty.GetArrayElementAtIndex (index));

			FocusId.SetNextId (annontationDataFieldFocusGroupName, index);
			dataField.ReorderableListDrawSettingsElement (rect, this, index);
		}

		public void SelectDataField (
			int dataFieldIndex
		)
		{
			if ( dataFieldList == null )
				return;
			dataFieldList.index = dataFieldIndex;
		}

		void AddDataField (
			ReorderableList list
		)
		{
			// We need to loose the focus from an possibly edited textfield, if we 
			// possibly want to change its content from script. And as we dont need the 
			// focus on any textfield after a new entry is created, we can just lose
			// the focus without any if's.
			FocusUtility.LoseFocus ();
		
			// 1) apply not applied changes from the serialized object to the data structures
			//		probably something else was just being edited just before the add-button 
			//		was pressed.
			ApplyModifiedProperties ();

			// 2) This name check is for the possible currently edited
			CorrectDataFieldName (list.index);

			// 3) now create the new item
			ReorderableList.defaultBehaviours.DoAddButton (list);
			SerializedProperty dataFieldProperty = baseProperty.GetArrayElementAtIndex (list.count - 1);
			DataFieldPropertyEditor dataFieldPropertyEditor = new DataFieldPropertyEditor (dataFieldProperty);
			// the call to get a new unique name works, bc. we havent committed our changes yet (-> ApplyModifiedValues)
			// otherwise the new element would be in the list as well, which dont wont during this call
			// Same situation for requested id
			dataFieldPropertyEditor.Init (
				parent.GetUniqueDataFieldName (DataField.defaultName)
			);

			// commit changes
			ApplyModifiedProperties ();
			list.GrabKeyboardFocus ();
		}

		void RemoveDataField (
			ReorderableList list
		)
		{
			// We need to loose the focus from an possibly edited textfield, if we 
			// possibly want to change its content from script. And as we dont need the 
			// focus on any textfield after a new entry is created, we can just lose
			// the focus without any if's.
			FocusUtility.LoseFocus ();

			list.GrabKeyboardFocus (); 
			ReorderableList.defaultBehaviours.DoRemoveButton (list);
			ApplyModifiedProperties ();
		}

		#endregion


		#region FocusManager

		public const string annontationDataFieldFocusGroupName = "XoxDF";
		public FocusManager	focusManager;

		void CreateFocusManager ()
		{
			focusManager = new FocusManager ();

			// Datafield Name
			focusManager.AddOnLostFocusTriggerIntCallback (
				CorrectDataFieldName,
				annontationDataFieldFocusGroupName);
			focusManager.AddOnKeyPressReturnTriggerIntCallback (
				CorrectDataFieldName,
				annontationDataFieldFocusGroupName);

			// Got Focus for Datafields sub elements
			//   if any sub element on a data field is going to be selected the index of the 
			//   data field list has to be moved to that data field
			focusManager.AddOnGotFocusTriggerIntCallback (
				SelectDataField,
				annontationDataFieldFocusGroupName);

		}

		void ValidateDefaultValues (
			int index
		)
		{
			if ( index < 0 ) {
				return;
			}
			if ( index >= dataFieldList.count ) {
				return;
			}

			var dataField =	new DataFieldPropertyEditor (dataFieldList.serializedProperty.GetArrayElementAtIndex (index));

			switch ( (DataField.DataFieldType) dataField.dataFieldType.enumValueIndex ) {
			case DataField.DataFieldType.Date:
				var datePropertyEditor = new DataFieldTypeDatePropertyEditor (dataField.dateValue);
				datePropertyEditor.ValidateDefaultValues ();
				break;
			}
		}

		public void CorrectDataFieldName (
			int index
		)
		{
			if ( index < 0 ) {
				return;
			}
			if ( index >= dataFieldList.count ) {
				return;
			}

			var suggestedName = parent.GetUniqueDataFieldName (DataField.defaultName, index);	
			SerializedProperty dataFieldProperty = baseProperty.GetArrayElementAtIndex (index);
			DataFieldPropertyEditor dataFieldPropertyEditor = new DataFieldPropertyEditor (dataFieldProperty);
			if ( dataFieldPropertyEditor.name.stringValue != suggestedName ) {
				dataFieldPropertyEditor.name.stringValue = suggestedName;
				ApplyModifiedProperties ();
			}
		}

		public void CommitChanges ()
		{
			FocusUtility.LoseFocus ();
			CorrectDataFieldName (dataFieldList.index);
			ValidateDefaultValues (dataFieldList.index);
		}

		#endregion
	
	}
}

