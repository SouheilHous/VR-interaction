using UnityEditor;
using xDocEditorBase.Extensions;


namespace xDocEditorBase.CustomData {

	public class DataFieldTypeLinkPropertyEditor : PropertyEditor
	{
		public readonly SerializedProperty link;

		public DataFieldTypeLinkPropertyEditor(
			SerializedProperty sPropLink
		)
			: base(
				sPropLink
			)
		{
			link = sPropLink.FindPropertyRelative("link");
		}

		public string linkValue {
			get { return link.stringValue; }
			set { link.stringValue = value; }
		}
	}
}
