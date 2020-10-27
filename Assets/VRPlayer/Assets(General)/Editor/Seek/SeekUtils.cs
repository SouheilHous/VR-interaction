using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;

namespace dlobo.Seek
{
	public static class Utils
	{
		private static string metadataBasePath = Application.dataPath + "../../Library/metadata/";

		public static string ConvertSizeToReadableUnit(ref long sizeInBytes)
		{
			int divisions = 0;

			while (sizeInBytes >= 1024 && divisions < 3) {
				sizeInBytes /= 1024;
				divisions++;
			}

			switch (divisions) {
				case 0: return "B";
				case 1: return "KB";
				case 2: return "MB";
				default: return "GB";
			}
		}

		public static long GetCompressedFileSize(string guid)
		{
			try {
				string path = metadataBasePath + guid.Substring(0, 2) + "/" + guid;
				return new FileInfo(path).Length;
			} catch {
				return -1L;
			}
		}

		public static IEnumerable<string> SkipConsecutiveDuplicates(IEnumerable<string> items)
		{
			IEnumerator<string> enumerator = items.GetEnumerator();
			enumerator.MoveNext();

			string previous = enumerator.Current;
			yield return previous;

			foreach (string item in items) {
				if (item != previous) {
					yield return item;
					previous = item;
				}
			}
		}

		public static string GetAssetTypesFilter(uint assetTypesMask, string[] assetTypeNames)
		{
			var sb = new StringBuilder();
			int type = 0;

			while (assetTypesMask != 0 && type < assetTypeNames.Length) {
				if ((assetTypesMask & 1) == 1) {
					sb.Append(" t:" + assetTypeNames[type]);
				}
				assetTypesMask >>= 1;
				type++;
			}

			return sb.ToString();
		}

		public static IEnumerable<R> Map<T, R>(IEnumerable<T> enumerable, Func<T, R> mapper)
		{
			foreach (T item in enumerable) {
				yield return mapper(item);
			}
		}

		// public static List<string> GetAssetTypes(uint assetTypesMask)
		// {
		// 	var types = new List<string>();
		// 	int type = 0;

		// 	while (assetTypesMask != 0) {
		// 		if ((assetTypesMask & 1) == 1) {
		// 			types.Add(((UnityAssetType) type).ToString());
		// 		}
		// 		assetTypesMask >>= 1;
		// 		type++;
		// 	}

		// 	return types;
		// }
	}
}
