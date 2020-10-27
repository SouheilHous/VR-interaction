using System;


namespace xDocBase.CustomData
{

	[System.Serializable]
	public class DataFieldTypeDateSpan : DataFieldTypeDate
	{
		// --------------------------------------------------------------------------
		// --- constructors
		// --------------------------------------------------------------------------
		public DataFieldTypeDateSpan ()
		{
		}

		public DataFieldTypeDateSpan (
			DataFieldTypeDateSpan src
		)
			: base (
				src
			)
		{
		}

		public DataFieldTypeDateSpan (
			int day,
			int month,
			int year
		)
			: base (
				day,
				month,
				year
			)
		{
		}

		// --------------------------------------------------------------------------
		// --- dateSpan specialisation
		// --------------------------------------------------------------------------
		public int dateSpan {
			get { return (GetDateTime () - DateTime.Today).Days; }
			set { 
				DateTime newDate = DateTime.Today.AddDays (value);
				Set (newDate);
			}
		}


		#region Object

		public override string ToString ()
		{
			return dateSpan + " days";
		}

		#endregion
	}
}
