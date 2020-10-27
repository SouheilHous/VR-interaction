using UnityEditor;
using UnityEngine;
using xDocBase.CustomData;
using xDocBase.UI;
using xDocEditorBase.UI;


namespace xDocEditorBase.CustomData {

	[CustomPropertyDrawer(typeof(DataFieldTypeTime))]
	public class DataFieldTypeTimeDrawer : PropertyDrawer
	{
		DataFieldTypeTimePropertyEditor sPropTime;

		public override void OnGUI(
			Rect position,
			SerializedProperty property,
			GUIContent label
		)
		{
			// this has to be retrieved before any controlId's are requested / assigned, so it does
			// not change and stays the parents control id.
			var currentControlId = Focus.FocusId.GetCurrentControlName();

			sPropTime = new DataFieldTypeTimePropertyEditor(property);

			using (new StateSaver.TextFieldAlignment(TextAnchor.UpperRight))
			using (new EditorGUI.PropertyScope(position, label, property)) {
				Vector2 size1 = EditorStyles.textField.CalcSize(new GUIContent("00"));
				Vector2 size2 = EditorStyles.label.CalcSize(new GUIContent(":"));

				float neededWidth = 2 * size1.x + size2.x;
				float wScale = Mathf.Min(1f, position.width / neededWidth);
				size1.x *= wScale;
				size2.x *= wScale;

				Rect cRect = EditorGUI.PrefixLabel(position, label);

				cRect.width = size1.x;
				using (new XoxEditorGUI.ChangeCheck(ValidateAndRectifyHourValue)) {
					Focus.FocusId.SetSubId(currentControlId, "H");
					EditorGUI.PropertyField(cRect, sPropTime.hour, GUIContent.none);	
				}
		
				cRect.x += cRect.width;
				cRect.width = size2.x;
				EditorGUI.LabelField(cRect, ":");	

				cRect.x += cRect.width;
				cRect.width = size1.x;
				using (new XoxEditorGUI.ChangeCheck(ValidateAndRectifyMinuteValue)) {
					Focus.FocusId.SetSubId(currentControlId, "M");
					EditorGUI.PropertyField(cRect, sPropTime.minute, GUIContent.none);	
				}
			}
		}

		void ValidateAndRectifyHourValue()
		{
			sPropTime.hourValue = Mathf.Clamp(sPropTime.hourValue, 0, 23);
		}

		void ValidateAndRectifyMinuteValue()
		{
			sPropTime.minuteValue = Mathf.Clamp(sPropTime.minuteValue, 0, 59);
		}

	}
}
