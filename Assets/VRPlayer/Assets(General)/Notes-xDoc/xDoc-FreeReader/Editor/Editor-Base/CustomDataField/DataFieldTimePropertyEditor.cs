using UnityEditor;
using xDocEditorBase.Extensions;


namespace xDocEditorBase.CustomData {

	public class DataFieldTypeTimePropertyEditor : PropertyEditor
	{
		public readonly SerializedProperty hour;
		public readonly SerializedProperty minute;

		public DataFieldTypeTimePropertyEditor(
			SerializedProperty sPropTime
		)
			: base(
				sPropTime
			)
		{
			hour = sPropTime.FindPropertyRelative("hour");
			minute = sPropTime.FindPropertyRelative("minute");
		}

		public int hourValue {
			get { return hour.intValue; }
			set { hour.intValue = value; }
		}

		public int minuteValue {
			get { return minute.intValue; }
			set { minute.intValue = value; }
		}
	}

}
