namespace xDocBase.CustomData
{

	[System.Serializable]
	public class DataFieldTypeTime
	{

		public int hour;
		public int minute;

		public DataFieldTypeTime ()
		{

		}

		public DataFieldTypeTime (
			DataFieldTypeTime src
		)
		{
			hour = src.hour;
			minute = src.minute;
		}

		public bool IsSameValue (
			DataFieldTypeTime other
		)
		{
			return minute == other.minute && hour == other.hour;
		}

		public override string ToString ()
		{
			return hour + ":" + minute;
		}

	}
}
