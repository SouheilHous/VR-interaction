#if UNITY_EDITOR
using UnityEditor;
using xDocBase.AssetManagement;
using xDocBase.AnnotationTypeModule;
using UnityEngine;
using xDocBase.Extensions;


namespace xDoc {

	[InitializeOnLoad]
	public static class XDocAssetManagerAssistant
	{
		static XDocAssetManagerAssistant()
		{
			AssetManager.createXDocAnnotationTypeCallback = CreateXDocAnnotationType;
			AssetManager.createXDocAnnotationCallback = CreateXDocAnnotation;
		}

		public static XDocAnnotationTypeBase CreateXDocAnnotationType(
			Object assetObject,
			string undoTitle
		)
		{
			return ScriptableSubAssetObject.CreateInstanceWithUndo<XDocAnnotationType>(assetObject, undoTitle);
		}

		public static void CreateXDocAnnotation() {
			EditorApplication.ExecuteMenuItem("Component/" + Annotation.menuPath);
		}
	}
}
#endif
