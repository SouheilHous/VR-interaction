using System;
using UnityEngine;


namespace xDocBase.CustomData
{

	[System.Serializable]
	public class DataFieldTypeDate
	{
		public int day = 1;
		public int month = 1;
		public int year = 1;


		#region helper for the inspectors

		public bool isInitialized = false;
		// --------------------------------------------------------------------------
		// --- helper for the inspector
		// --------------------------------------------------------------------------
		[NonSerialized]	public int undoGroupIndex = 0;
		[NonSerialized]	public bool dateHadFocus = false;
		[NonSerialized]	public bool dateHasFocus = false;

		#endregion


		#region Constructors

		public DataFieldTypeDate ()
		{	
		}

		public DataFieldTypeDate (
			DataFieldTypeDate src
		)
		{	
			day = src.day;
			month = src.month;
			year = src.year;
		}

		public DataFieldTypeDate (
			int day,
			int month,
			int year
		)
		{	
			this.day = day;
			this.month = month;
			this.year = year;
		}

		#endregion


		#region Utility

		public bool InitializeIfNeeded ()
		{
			if (!isInitialized) {
				isInitialized = true;
				if (day == 1 && month == 1 && year == 1) {
					Set (DateTime.Today);
					return true;
				}
			}
			return false;
		}

		public void RectifyIfNeeded ()
		{
			day = Mathf.Clamp (day, 1, 31);
			month = Mathf.Clamp (month, 1, 12);
			year = Mathf.Clamp (year, 1, 9999);

			day = Mathf.Min (day, DateTime.DaysInMonth (year, month));
		}

		public bool DateIsBogus ()
		{
			if (year < 1)
				return false;
			if (year > 9999)
				return false;
			if (month < 1)
				return false;
			if (month > 12)
				return false;
			if (day < 1)
				return false;
			if (day > DateTime.DaysInMonth (year, month))
				return false;
			return true;
		}

		public bool IsSameValue (
			DataFieldTypeDate other
		)
		{
			return day == other.day && month == other.month && year == other.year;
		}

		#endregion


		#region Setters/Getters

		public DateTime GetDateTime ()
		{
			return new DateTime (year, month, day);
		}

		public void Set (
			DateTime src
		)
		{
			day = src.Day;
			month = src.Month;
			year = src.Year;
		}

		public void Set (
			int day,
			int month,
			int year
		)
		{
			this.day = day;
			this.month = month;
			this.year = year;
		}

		#endregion


		#region Object

		public override string ToString ()
		{
			return day + "." + month + "." + year;
		}

		#endregion

	}
}
