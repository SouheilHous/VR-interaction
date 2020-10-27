#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;


namespace xDoc {

	[System.Serializable]
	[InitializeOnLoad]
	[ExecuteInEditMode]
	public class XDocSettings : XDocSettingsBase
	{
	}
}
#endif