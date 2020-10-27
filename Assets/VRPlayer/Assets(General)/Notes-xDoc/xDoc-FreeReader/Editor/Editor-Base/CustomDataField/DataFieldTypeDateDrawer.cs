using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocBase.CustomData;
using xDocBase.UI;
using xDocEditorBase.Focus;
using xDocEditorBase.UI;


namespace xDocEditorBase.CustomData {

	[CustomPropertyDrawer(typeof(DataFieldTypeDate))]
	public class DataFieldTypeDateDrawer : PropertyDrawer
	{
		DataFieldTypeDatePropertyEditor sPropDate;
		bool showTodayButton;
		bool validationNeeded;
		string validationNeededFor = "";
		string currentControlId = "";

		public override void OnGUI(
			Rect position,
			SerializedProperty property,
			GUIContent label
		)
		{

			// this has to be retrieved before any controlId's are requested / assigned, so it does
			// not change and stays the parents control id.
			currentControlId = FocusId.GetCurrentControlName();
			sPropDate = new DataFieldTypeDatePropertyEditor(property);

			using (new StateSaver.TextFieldAlignment(TextAnchor.UpperRight))
			using (new EditorGUI.PropertyScope(position, label, property)) {
				Vector2 size1 = EditorStyles.textField.CalcSize(new GUIContent("00"));
				Vector2 size2 = EditorStyles.label.CalcSize(new GUIContent("."));
				Vector2 size3 = EditorStyles.textField.CalcSize(new GUIContent("0000"));
				var todayContent = new GUIContent(AssetManager.settings.iconToday);
				Vector2 size4 = AssetManager.settings.styleMiniButton.style.CalcSize(todayContent);

				float neededWidth = 2 * size1.x + 2 * size2.x + size3.x;
				float wScale = Mathf.Min(1f, position.width / neededWidth);
				size1.x *= wScale;   
				size2.x *= wScale;
				size3.x *= wScale;

				Rect cRect = EditorGUI.PrefixLabel(position, label);

				// --------------------------------------------------------------------------
				// --- day
				// --------------------------------------------------------------------------
				string dayControlName;

				cRect.width = size1.x;
				using (new XoxEditorGUI.ChangeCheck(DayValueChanged)) {
					dayControlName = FocusId.SetSubId(currentControlId, "D");
					EditorGUI.PropertyField(cRect, sPropDate.day, GUIContent.none);	
				}

				cRect.x += cRect.width;
				cRect.width = size2.x;
				EditorGUI.LabelField(cRect, ".");	

				// --------------------------------------------------------------------------
				// --- month
				// --------------------------------------------------------------------------
				cRect.x += cRect.width;
				cRect.width = size1.x;
				using (new XoxEditorGUI.ChangeCheck(MonthValueChanged)) {
					FocusId.SetSubId(currentControlId, "M");
					EditorGUI.PropertyField(cRect, sPropDate.month, GUIContent.none);	
				}

				cRect.x += cRect.width;
				cRect.width = size2.x;
				EditorGUI.LabelField(cRect, ".");	

				// --------------------------------------------------------------------------
				// --- year
				// --------------------------------------------------------------------------
				cRect.x += cRect.width;
				cRect.width = size3.x;
				using (new XoxEditorGUI.ChangeCheck(YearValueChanged)) {
					FocusId.SetSubId(currentControlId, "Y");
					EditorGUI.PropertyField(cRect, sPropDate.year, GUIContent.none);	
				}

				// --------------------------------------------------------------------------
				// --- today button
				// --------------------------------------------------------------------------
				cRect.x += cRect.width + AssetManager.settings.styleMiniButton.style.margin.left; 
				cRect.width = size4.x;
				cRect.xMax = Mathf.Min(cRect.xMax, position.xMax);
				if (showTodayButton) {
					// only show the button, if there is enough space
					if (GUI.Button(cRect, todayContent, AssetManager.settings.styleMiniButton.style)) {
						sPropDate.SetToday();
						GUI.FocusControl(dayControlName);
						validationNeeded = true;
					}
				}
				switch (Event.current.type) {
				case EventType.Repaint:
				// decide, if today button can be shown
					if (cRect.width > 15) {
						showTodayButton = true;
					} else {
						showTodayButton = false;
					}

				// decide, if date can be validated
				// This logic takes into account that the same drawer OBJECT is reused for ALL
				// drawers of that type on the screen (or window).
				// so we have problems to carry information in class variables from one call to 
				// the other call for the SAME serialized object/property
					if (validationNeeded)
					if (!FocusUtility.HasFocus(validationNeededFor))
					if (validationNeededFor.Equals(currentControlId)) {
						sPropDate.ValidateDefaultValues();
						validationNeeded = false;
					}

					break;
				}
			}
		}

		void DayValueChanged()
		{
			sPropDate.ClampDayValue();
			validationNeeded = true;
			validationNeededFor = currentControlId;
		}

		void MonthValueChanged()
		{
			sPropDate.ClampMonthValue();
			validationNeeded = true;
			validationNeededFor = currentControlId;
		}

		void YearValueChanged()
		{
			sPropDate.ClampYearValue();
			validationNeeded = true;
			validationNeededFor = currentControlId;
		}
	}
}
