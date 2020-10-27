using System;
using UnityEditor;
using UnityEngine;
using xDocEditorBase.Extensions;


namespace xDocEditorBase.CustomData {

	public class DataFieldTypeDatePropertyEditor : PropertyEditor
	{
		public readonly SerializedProperty day;
		public readonly SerializedProperty month;
		public readonly SerializedProperty year;

		public DataFieldTypeDatePropertyEditor(
			SerializedProperty sPropDate
		)
			: base(
				sPropDate
			)
		{
			day = sPropDate.FindPropertyRelative("day");
			month = sPropDate.FindPropertyRelative("month");
			year = sPropDate.FindPropertyRelative("year");	
		}

		public int dayValue {
			get { return day.intValue; }
			set { day.intValue = value; }
		}

		public int monthValue {
			get { return month.intValue; }
			set { month.intValue = value; }
		}

		public int yearValue {
			get { return year.intValue; }
			set { year.intValue = value; }
		}

		public void ClampDayValue()
		{
			dayValue = Mathf.Clamp(dayValue, 1, 31);
			ApplyModifiedProperties();
		}

		public void ClampMonthValue()
		{
			monthValue = Mathf.Clamp(monthValue, 1, 12);
			ApplyModifiedProperties();
		}

		public void ClampYearValue()
		{
			yearValue = Mathf.Clamp(yearValue, 1, 9999);
			ApplyModifiedProperties();
		}

		public void ValidateDefaultValues()
		{
			dayValue = Mathf.Min(dayValue, DateTime.DaysInMonth(yearValue, monthValue));
			ApplyModifiedProperties();
		}

		public void SetToday()
		{
			dayValue = DateTime.Today.Day;
			monthValue = DateTime.Today.Month;
			yearValue = DateTime.Today.Year;		
		}

		public void Set111()
		{
			dayValue = 1;
			monthValue = 1;
			yearValue = 1;		
		}

	}
}
