namespace xDocBase.CustomData
{

	[System.Serializable]
	public class DataFieldTypeLink
	{
	
		public string link;

		public DataFieldTypeLink ()
		{
		}

		public DataFieldTypeLink (
			DataFieldTypeLink src
		)
		{
			link = src.link;
		}

		public bool IsSameValue (
			DataFieldTypeLink other
		)
		{
			return link == other.link;
		}

		public override string ToString ()
		{
			return link;
		}
	
	}
}
