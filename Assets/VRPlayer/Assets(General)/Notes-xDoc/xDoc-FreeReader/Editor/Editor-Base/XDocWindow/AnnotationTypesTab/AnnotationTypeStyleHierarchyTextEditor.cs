using UnityEditor;
using UnityEngine;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class AnnotationTypeStyleHierarchyTextEditor : AnnotationTypeStyleBaseEditor
	{
		readonly SerializedProperty showTextInHierarchyView;
		readonly SerializedProperty textWidthInHierarchyView;

		readonly GUIContent l1;
		readonly GUIContent l2;

		override protected ATStyleEditorType GetEditorType ()
		{
			return ATStyleEditorType.HIERARCHY;
		}

		public AnnotationTypeStyleHierarchyTextEditor (
			SerializedProperty serializedProperty
		)
			: base (
				serializedProperty
			)
		{
			showTextInHierarchyView = serializedProperty.FindPropertyRelative ("showTextInHierarchyView");
			textWidthInHierarchyView = serializedProperty.FindPropertyRelative ("textWidthInHierarchyView");

			l1 = new GUIContent ("Show Text", "If enabled shows a text field in the hierarchy view, which can be directly edited."
				+ " Only the textfield of the top most annotation in a game object will be shown.");
			l2 = new GUIContent ("Start Position in %", "The text field is layed out relative to the window width.");
		}

		override protected void DrawOptions (
			XoxGUIRect currentRect
		)
		{
			currentRect.SetToLineHeight (1);

			EditorGUI.LabelField (currentRect.rect, "Hierarchy View Text Options", EditorStyles.boldLabel);
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, showTextInHierarchyView, l1);
			currentRect.MoveDown ();

			using ( new EditorStateSaver.Indent (1) ) {
				GUI.enabled = showTextInHierarchyView.boolValue;
				EditorGUI.PropertyField (currentRect.rect, textWidthInHierarchyView, l2);
				textWidthInHierarchyView.floatValue = Mathf.Clamp (textWidthInHierarchyView.floatValue, 10, 90);
				GUI.enabled = true;
			}
		}

		static public float GetHeight ()
		{
			return XoxGUIRect.GetHeightOfLines (3);
		}

	}
}