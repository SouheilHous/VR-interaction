using UnityEditor;
using UnityEngine;


namespace xDocBase.Extensions {

	public static class XoxPrefabUtility
	{
		public static bool IsPrefab(
			this Object obj
		)
		{
			switch (PrefabUtility.GetPrefabType(obj)) {
			case PrefabType.Prefab:
				return true;
			case PrefabType.ModelPrefab:
				return true;
			default:
				return false;
			}
			// old implementation
			//return PrefabUtility.GetPrefabParent(obj) == null
			//&& PrefabUtility.GetPrefabObject(obj) != null; 
		}

		public static bool IsConnectedPrefabInstance(
			this Object obj
		)
		{
			switch (PrefabUtility.GetPrefabType(obj)) {
			case PrefabType.PrefabInstance:
				return true;
			case PrefabType.ModelPrefabInstance:
				return true;
			default:
				return false;
			}
		}
	}
}

