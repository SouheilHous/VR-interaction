using UnityEditor;
using UnityEngine;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class AnnotationTypeStyleTitleEditor : AnnotationTypeStyleBaseEditor
	{
		readonly SerializedProperty showTitle;
		readonly SerializedProperty titleFormat;
		readonly SerializedProperty titleFix;

		override protected ATStyleEditorType GetEditorType ()
		{
			return ATStyleEditorType.TITLE;
		}


		public AnnotationTypeStyleTitleEditor (
			SerializedProperty serializedProperty
		)
			: base (
				serializedProperty
			)
		{
			showTitle = serializedProperty.FindPropertyRelative ("showTitle");
			titleFormat = serializedProperty.FindPropertyRelative ("titleFormat");
			titleFix = serializedProperty.FindPropertyRelative ("titleFix");
		}

		override protected void DrawOptions (
			XoxGUIRect currentRect
		)
		{
			currentRect.SetToLineHeight (1);

			EditorGUI.LabelField (currentRect.rect, "Title Options", EditorStyles.boldLabel);
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, showTitle);
			currentRect.MoveDown ();

			using ( new EditorStateSaver.Indent (1) ) {
				GUI.enabled = showTitle.boolValue;

				EditorGUI.PropertyField (currentRect.rect, titleFormat);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, titleFix);

				GUI.enabled = true;
			}
		}

		static public float GetHeight ()
		{
			return XoxGUIRect.GetHeightOfLines (4);
		}


	}
}