using UnityEditor;
using UnityEngine;
using xDocBase.AnnotationTypeModule;


namespace xDocBase.AssetManagement {

	//[InitializeOnLoad]
	public static class AssetManager
	{
#region Defaults

		public const string versionLabel = "1.1";
		public const string versionDate = "6. June 2016";
		public const string productName = "xDoc";
		public const string companyName = "xox interactive";
		public const string supportForumURL = "http://xoxinteractive.com/questions/category/xdoc/";
		public const string tutorialsURL = "http://xoxinteractive.com/utilities/xdoc/xdoc-tutorials/";
		public const string companySiteURL = "http://www.xoxinteractive.com/";
		public const string assetStoreURL = "https://www.assetstore.unity3d.com/#!/content/59702";

		public const string menuPathWindow = "Window/";

		// --------------------------------------------------------------------------
		// --- Assets
		// --------------------------------------------------------------------------
		const string adbPath = "Assets/Editor Default Resources/";
		const string projectPath = "xDoc-FreeReader/";
		const string configDir = "Config/";
		const string assetManagerDataFile = "xDocAssetManagerData.asset";

		const string assetManagerDataFileFullPath = adbPath + projectPath + configDir + assetManagerDataFile;
		const string assetManagerDataFileIdKey = "xDocBase.AssetManager.InstanceID";

		public static XDocAssetManagerDataBase assetManagerData;

		public const string errMessageCantLoadAssetResources = 
			"xDoc can't load its Asset Resources!\n" +
			"Most probably the script assignments of xDoc's scriptable objects got lost. " +
			"These objects are located in the " +
			"'Assets\\Editor Default Resources\\xDoc-FreeReader\\Config' folder. " +
			"Either reassign them, reimport a working backup or contact support.\n" +
			"If you need the support link or email addresses, " +
			"please refer to the xDoc tab in the Unity Preferences: " +
			"'Edit' menu -> 'Preferences...' -> 'xDoc'.";

#endregion


#region Scriptable Objects

		static XDocSettingsBase _guiPrefs;
		static XDocAnnotationTypesListBase _annotationTypesAsset;
		static XDocAnnotationTypeBase _invalidAnnotationType;
		static XDocWriterBase _writerAsset;

		public static XDocSettingsBase settings {
			get {
				ForceInit();
				return _guiPrefs;
			}
			set {
				_guiPrefs = value;
			}
		}

		public static XDocAnnotationTypesListBase annotationTypesAsset {
			get {
				ForceInit();
				return _annotationTypesAsset;
			}
			set {
				_annotationTypesAsset = value; 
			}
		}

		public static XDocAnnotationTypeBase invalidAnnotationType {
			get {
				ForceInit();
				return _invalidAnnotationType;
			}
			set {
				_invalidAnnotationType = value; 
			}
		}

		public static XDocWriterBase writer {
			get {
				ForceInit();
				return _writerAsset;
			}
			set {
				_writerAsset = value; 
			}
		}

		public static XDocAnnotationTypeBase defaultAnnotationType {
			get { return annotationTypesAsset.annotationTypesList[0]; }
		}

#endregion


#region Manual

		public static int GetXDocManualID()
		{
			return settings.xDocManualA4.GetInstanceID();
		}

		public static int GetXDocManualIDLetter()
		{
			return settings.xDocManualLetter.GetInstanceID();
		}

#endregion


#region BaseType Hooks

		public delegate XDocAnnotationTypeBase CreateXDocAnnotationTypeCallback(
			Object assetObject,
			string undoTitle
			);
		public static CreateXDocAnnotationTypeCallback createXDocAnnotationTypeCallback;

		public delegate void CreateXDocAnnotation();
		public static CreateXDocAnnotation createXDocAnnotationCallback;

#endregion


#region Main

//		public static bool canWrite {
//			get;
//			private set;
//		}

		public static bool canWrite {
			get { 
				try {
					return writer.isWriter;
				} catch (System.Exception) {
					return false;
				}			
			}
		}

		public static bool isFunctional {
			get;
			private set;
		}

//		public static bool HasWriter() {
//			try {
//				return writer.isWriter;
//			} catch (System.Exception) {
//				return false;
//			}
//		}

		/// <summary>
		/// Initializes the <see cref="xDocBase.AssetManagement.AssetManager"/> class.
		/// </summary>
		static AssetManager()
		{
			ForceInit();
						 
//			try {
//				
//				if (assetManagerData.writer == null) {
//					canWrite = false;
//				} else {
//					canWrite = true;
//				}
//			} catch (System.Exception) {
//				canWrite = false;
//			}
		}

		static void LoadAsset<T>(
			ref T objReferencer,
			string instanceIdKey,
			string path
		)
			where T : Object
		{

			//Debug.Log("Check, if it is already a known reference");
			if (objReferencer != null) {
				// the obj reference is already available - nothing to do, exit
				//Debug.Log("Asset already referenced: " + objReferencer.name);
				return;
			}
	
			// Get the instance id, if it exists
			//Debug.Log("Get the Id from key: " + instanceIdKey);
			int iId = EditorPrefs.GetInt(instanceIdKey);
	
			// try to get the obj by the "preserved" instanceId
			//Debug.Log("Try to get it by Id: " + iId);
			T obj;
			obj = EditorUtility.InstanceIDToObject(iId) as T;
	
			if (obj != null) {
				// We found the asset and have the reference; we can exit now
				objReferencer = obj;
				//Debug.Log("Asset found by Id: " + objReferencer.name + ", id: " + iId);
				return;
			}
	
			// try to load
			//Debug.Log("Try to load it");
			obj = EditorGUIUtility.Load(path) as T;
			if (obj != null) {
				// We loaded the asset; we can exit now
				objReferencer = obj;
				iId = objReferencer.GetInstanceID();
				EditorPrefs.SetInt(instanceIdKey, iId);
				//Debug.Log("Loaded by path: " + pathToScriptableObjects + ", " + objReferencer.name + ", id: " + iId);
				return;
			}
	
			//Debug.Log("All getting grab on asset attemps failed!");
	
			//		Resources.UnloadUnusedAssets();
			//		guiPrefs = Resources.Load<GUISettings>("");
			//		var o = Resources.FindObjectsOfTypeAll<GUISettings>();
			//		EditorUtility.UnloadUnusedAssetsImmediate();
			//		EditorApplication.playmodeStateChanged += PlayModeChangeHandler;
			//		guiPrefs = AssetDatabase.FindAssets("t:GUISettings");
			//		guiPrefs = AssetDatabase.LoadAssetAtPath(
			//			xDocGUISettingsFilePath,
			//			typeof(GUISettings)
			//		) as GUISettings;
			//		annotationTypesAsset = Object.FindObjectOfType<AnnotationTypesList>();
	
		}

		public static void ForceInit()
		{
			isFunctional = true;
			LoadAsset<XDocAssetManagerDataBase>(ref assetManagerData, assetManagerDataFileIdKey, assetManagerDataFileFullPath);

			if (assetManagerData == null) {
				Debug.LogError("xDoc Error: " + errMessageCantLoadAssetResources);
				isFunctional = false;
				return;
			}

			settings = assetManagerData.settings;
			annotationTypesAsset = assetManagerData.annotationTypeList;
			invalidAnnotationType = assetManagerData.invalidAnnotationType;
			writer = assetManagerData.writer;

		}

#endregion

	}
}
