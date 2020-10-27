using UnityEditor;
using UnityEngine;

namespace dlobo.Seek
{
	public class SeekAssetPostProcessor : AssetPostprocessor
	{
		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (SeekWindow.Instance != null) {
				SeekWindow.Instance.DoRepeatSearch();
			}
		}
	}
}