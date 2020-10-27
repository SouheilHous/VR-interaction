using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace dlobo.Seek
{
	[System.Serializable]
	public class SearchConfig : ISerializationCallbackReceiver
	{
		// search
		public string search = "";
		public SearchType searchType;
		public int assetTypesMask;
		public MatchingType matchingType = MatchingType.Greedy;
		public bool isCaseSensitive;

		// display
		public bool showNameFirst;
		public bool showFolders = true;
		public bool showSizes;
		public bool showTypes;
		public bool showSubObjects;
		public bool showCreationTime;
		public bool showLastWriteTime;
		public SortType sortType;
		public bool sortInReverse;

		// special filters
		public SpecialFilterType specialFilterType = SpecialFilterType.None;

		public int textureFormat;

		public StringMatchingConfig componentName;
		public StringMatchingConfig variableName;
		public StringMatchingConfig variableValue;

		public bool doSearchChildren = true;

		// Old variables. May be removed in future Seek versions.
		public bool useSpecialFilters = false;
		public bool filterTextureFormat;

		[System.NonSerialized]
		private string representation;
		public string Representation {
			get {
				if (representation == null) {
					representation = generateRepresentation();
				}
				return representation;
			}
		}

		public static SearchConfig Clone(SearchConfig config)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, config);
				stream.Position = 0;
				return (SearchConfig) formatter.Deserialize(stream);
			}
		}

		// Callback to convert the old special filter values to new format.
		// Old variables may be removed in future versions. In that case, remove this method and OnBeforeSerialize.
		public void OnAfterDeserialize()
		{
			if (useSpecialFilters) {
				useSpecialFilters = false;
				if (filterTextureFormat) {
					filterTextureFormat = false;
					specialFilterType = SpecialFilterType.TextureFormat;
				}
			}
		}

		public void OnBeforeSerialize()
		{
		}

		private string generateRepresentation()
		{
			var sb = new System.Text.StringBuilder();
			sb.Append("<color=#0f0>" + search + "</color>");
			sb.Append(" [");
			sb.Append(searchType.ToString() + " search");
			// if (assetTypesMask != 0) { sb.Append(", " + string.Join(" ", Utils.GetAssetTypes(assetTypesMask).ToArray())); }
			if (assetTypesMask != 0) { sb.Append(", use type filter"); }
			sb.Append(", " + matchingType.ToString()+" match");
			if (isCaseSensitive) { sb.Append(" (case sensitive)"); }
			sb.Append(", " + sortType.ToString() + " sort");
			if (sortInReverse) { sb.Append(" (reversed)"); }
			if (specialFilterType != SpecialFilterType.None) { sb.Append(", <color=#f00>"+ specialFilterType.ToString() +"</color>"); }
			// if (showNameFirst)    { sb.Append(", name first"); }
			// if (showFolders)      { sb.Append(", folders"); }
			// if (showSizes)        { sb.Append(", sizes"); }
			// if (showTypes)        { sb.Append(", types"); }
			// if (showSubObjects)   { sb.Append(", sub-objects"); }
			sb.Append("]");
			return sb.ToString();
		}

		public override string ToString()
		{
			return representation;
		}
	}

	[System.Serializable]
	public class StringMatchingConfig
	{
		public string String = "";
		public MatchingType MatchingType;
		public bool IsCaseSensitive;
	}
}
