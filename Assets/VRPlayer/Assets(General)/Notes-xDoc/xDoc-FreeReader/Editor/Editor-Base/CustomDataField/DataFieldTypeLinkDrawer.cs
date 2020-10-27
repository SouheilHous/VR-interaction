using UnityEditor;
using UnityEngine;
using xDocBase.CustomData;


namespace xDocEditorBase.CustomData {

	[CustomPropertyDrawer(typeof(DataFieldTypeLink))]
	public class DataFieldTypeLinkDrawer : PropertyDrawer
	{
		DataFieldTypeLinkPropertyEditor sPropLink;
		static readonly GUIContent goButtonContent = new GUIContent(">");

		public override void OnGUI(
			Rect position,
			SerializedProperty property,
			GUIContent label
		)
		{
			sPropLink = new DataFieldTypeLinkPropertyEditor(property);

			using (new EditorGUI.PropertyScope(position, label, property)) {
				Vector2 goButtonSize = EditorStyles.miniButton.CalcSize(goButtonContent);

				Rect cRect = EditorGUI.PrefixLabel(position, label);

				cRect.width -= goButtonSize.x;
				EditorGUI.PropertyField(cRect, sPropLink.link, GUIContent.none);	

				cRect.x += cRect.width;
				cRect.width = goButtonSize.x;
				if (GUI.Button(cRect, goButtonContent, EditorStyles.miniButton)) {
					Application.OpenURL(sPropLink.linkValue);
				}	
			}
		}
	}
}