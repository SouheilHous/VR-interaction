using UnityEditor;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class AnnotationTypeStyleTextEditor : AnnotationTypeStyleBaseEditor
	{
		readonly SerializedProperty showTextArea;
		readonly SerializedProperty wideView;

		override protected ATStyleEditorType GetEditorType ()
		{
			return ATStyleEditorType.TEXT;
		}

		public AnnotationTypeStyleTextEditor (
			SerializedProperty serializedProperty
		)
			: base (
				serializedProperty
			)
		{
			showTextArea = serializedProperty.FindPropertyRelative ("showTextArea");
			wideView = serializedProperty.FindPropertyRelative ("wideView");
		}

		override protected void DrawOptions (
			XoxGUIRect currentRect
		)
		{
			
			currentRect.SetToLineHeight (1);
			EditorGUI.LabelField (currentRect.rect, "Text Area Options", EditorStyles.boldLabel);
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, showTextArea);
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, wideView);
		}

		static public float GetHeight ()
		{
			return XoxGUIRect.GetHeightOfLines (3);
		}

	}
}