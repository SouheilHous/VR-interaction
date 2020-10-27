using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace dlobo.Seek
{
	public static class Matching
	{
		public static List<Result> GetResultsFromAllAssets(IEnumerable<AssetInfo> assets)
		{
			var results = new List<Result>();

			foreach (AssetInfo asset in assets)
			{
				string path = asset.Path.Substring(7);
				var result = new SimpleResult {
					GUID = asset.GUID,
					Path = path,
					FullPath = asset.Path,
				};
				results.Add(result);
			}
			return results;
		}

		public static List<Result> FindMatchesInAssets(string search, IEnumerable<AssetInfo> assets, bool ignoreCase, Func<string, string> pathFilter = null)
		{
			var results = new List<Result>();

			if (ignoreCase) {
				search = search.ToLower();
			}

			foreach (AssetInfo asset in assets)
			{
				string path = asset.Path.Substring(7);
				string truePath;
				if (pathFilter != null) {
					truePath = pathFilter(path);
				} else {
					truePath = path;
				}

				int index = (ignoreCase ? truePath.ToLower().IndexOf(search) : truePath.IndexOf(search));
				if (index != -1) {
					var result = new SimpleResult {
						GUID = asset.GUID,
						Path = path,
						FullPath = asset.Path,
						Index = index + path.Length - truePath.Length,
						Length = search.Length,
					};
					results.Add(result);
				}
			}

			return results;
		}

		public static List<Result> FindRegexMatchesInAssets(string search, IEnumerable<AssetInfo> assets, bool ignoreCase, Func<string, string> pathFilter = null)
		{
			var regex = new Regex(search, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
			var results = new List<Result>();

			foreach (AssetInfo asset in assets)
			{
				string path = asset.Path.Substring(7);
				string truePath;
				if (pathFilter != null) {
					truePath = pathFilter(path);
				} else {
					truePath = path;
				}

				Match match = regex.Match(truePath);
				if (match.Success) {
					var result = new SimpleResult {
						GUID = asset.GUID,
						Path = path,
						FullPath = asset.Path,
						Index = match.Index + path.Length - truePath.Length,
						Length = match.Length,
					};
					results.Add(result);
				}
			}
			return results;
		}

		public static List<Result> FindGreedyMatchesInAssets(string search, IEnumerable<AssetInfo> assets, bool ignoreCase, Func<string, string> pathFilter = null)
		{
			var results = new List<Result>();

			if (ignoreCase) {
				search = search.ToLower();
			}

			foreach (AssetInfo asset in assets)
			{
				string path = asset.Path.Substring(7);
				string truePath;
				int indexAdd;
				if (pathFilter != null) {
					truePath = pathFilter(path);
					indexAdd = path.Length - truePath.Length;
				} else {
					truePath = path;
					indexAdd = 0;
				}

				int i = 0;
				int j = 0;
				var slices = new List<Slice>();
				Slice lastSlice = null;

				while (j < search.Length && i < truePath.Length)
				{
					if (search[j] == truePath[i] || (ignoreCase && search[j] == char.ToLower(truePath[i])))
					{
						if (lastSlice == null || indexAdd + i > lastSlice.EndIndex) {
							lastSlice = new Slice {
								Index = indexAdd + i,
								EndIndex = indexAdd + i+1
							};
							slices.Add(lastSlice);
						} else {
							lastSlice.EndIndex++;
						}
						j++;
					}
					i++;
				}

				if (j >= search.Length)
				{
					var result = new ScatteredResult {
						GUID = asset.GUID,
						Path = path,
						FullPath = asset.Path,
						Slices = slices,
					};
					results.Add(result);
				}
			}
			return results;
		}

		public static List<Result> FindMatchesInAssets_ByGUID(string search, IEnumerable<AssetInfo> assets)
		{
			var results = new List<Result>();

			// check if search is a valid GUID
			for (int i = 0; i < search.Length; i++) {
				char c = search[i];
				if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9')) {
					return results;
				}
			}

			foreach (AssetInfo asset in assets)
			{
				string truePath = asset.Path.Substring(7);

				bool isMatch = asset.GUID.IsMatch_Exact(search, ignoreCase: true);
				if (isMatch) {
					var result = new SimpleResult {
						GUID = asset.GUID,
						Path = truePath,
						FullPath = asset.Path,
						Index = 0,
						Length = 0,
					};
					results.Add(result);
				}
			}

			return results;
		}
	}
}
