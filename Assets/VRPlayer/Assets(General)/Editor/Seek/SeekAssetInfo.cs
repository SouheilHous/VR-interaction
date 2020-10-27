namespace dlobo.Seek
{
	public class AssetInfo
	{
		public string Path;
		public string GUID;

		public AssetInfo(string path, string guid)
		{
			this.Path = path;
			this.GUID = guid;
		}
	}
}