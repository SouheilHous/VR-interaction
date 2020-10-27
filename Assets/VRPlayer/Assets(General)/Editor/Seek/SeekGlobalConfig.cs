namespace dlobo.Seek
{
	[System.Serializable]
	public class GlobalConfig
	{
		public bool showOptions = true;
		public int itemSize = 18;
		public bool showSavedSearches = false;

		public float delayForSlowSearch = default_searchDelayForSlowOptions;

		// rich text colors
		public string fileNameColor = default_fileNameColor;
		public string fileTypeColor = default_fileTypeColor;
		public string fileSizeColor = default_fileSizeColor;
		public string fileCreationTimeColor = default_fileCreationTimeColor;
		public string fileLastWriteTimeColor = default_fileLastWriteTimeColor;
		public string guidColor = default_guidColor;

		// rich text colors
		public bool doCollapseSearchType = true;
		public bool doCollapseAssetTypes = true;
		public bool doCollapseMatchingType = true;
		public bool doCollapseSortType = true;
		public bool doCollapseDisplayOptions = true;
		public bool doCollapseSpecialFilters = true;
		public bool doCollapseSavedSearches = true;

		public const float default_searchDelayForSlowOptions = 0.25f;
		public const string default_fileNameColor = "#ff0";
		public const string default_fileTypeColor = "#0f0";
		public const string default_fileSizeColor = "#0ff";
		public const string default_fileCreationTimeColor = "#f60";
		public const string default_fileLastWriteTimeColor = "#fa0";
		public const string default_guidColor = "#999";
	}
}
