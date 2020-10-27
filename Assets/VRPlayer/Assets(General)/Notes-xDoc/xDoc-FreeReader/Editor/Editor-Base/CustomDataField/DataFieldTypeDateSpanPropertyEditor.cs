using System;
using UnityEditor;
using xDocBase.CustomData;


namespace xDocEditorBase.CustomData {

	public class DataFieldTypeDateSpanPropertyEditor : DataFieldTypeDatePropertyEditor
	{
		public DataFieldTypeDateSpanPropertyEditor(
			SerializedProperty sProp
		)
			: base(
				sProp
			)
		{
		}

		public int dateSpan {
			get {
				// we might need to deal with "bad" dates, while the user is still editing
				try {
					DataFieldTypeDateSpan dl = new DataFieldTypeDateSpan();
					dl.Set(dayValue, monthValue, yearValue);
					return dl.dateSpan;
				} catch (ArgumentOutOfRangeException) {
					return 0;
				}
			}
			set { 
				DataFieldTypeDateSpan dl = new DataFieldTypeDateSpan();
				dl.dateSpan = value;
				dayValue = dl.day;
				monthValue = dl.month;
				yearValue = dl.year;
			}
		}

	}
}

