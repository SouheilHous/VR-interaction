using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace dlobo.Seek
{
	public class SeekSavedData : ScriptableObject
	{
		public GlobalConfig globalConfig;
		public SearchConfig config;
		public SearchConfig searchConfigAutosave;
		public List<SearchConfig> savedConfigs = new List<SearchConfig>();
		public string lastSelectedSavedConfig;

		public static SeekSavedData Load(string filePath)
		{
			var data = (SeekSavedData) AssetDatabase.LoadAssetAtPath(filePath, typeof(SeekSavedData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<SeekSavedData>();
				AssetDatabase.CreateAsset(data, filePath);
			}
			return data;
		}

		public static void SetDirty(SeekSavedData data)
		{
			EditorUtility.SetDirty(data);
		}

		public static void Save(SeekSavedData data)
		{
			EditorUtility.SetDirty(data);
			AssetDatabase.SaveAssets();
		}
	}
}
