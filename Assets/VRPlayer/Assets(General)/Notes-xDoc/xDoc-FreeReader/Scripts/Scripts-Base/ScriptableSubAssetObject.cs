using UnityEditor;
using UnityEngine;


namespace xDocBase.Extensions {

	public class ScriptableSubAssetObject : ScriptableObject
	{
		// #################################################################################################################
		// ### Instance functions
		// #################################################################################################################

		// =================================================================================================================
		// === "Constructors"
		// =================================================================================================================

		/// <summary>
		/// 2. Part of the OnEnable call - the defacto constructor for scriptable objects. 
		/// ! OnEnable / CreateInstance do not take argument and the call _sub_ OnEnable / CreateInstance
		/// of child objects before we can pass any arguments. Thus we have to split the OnEnable call, so
		/// that we have time to set arguments in between.
		/// 
		/// The other reason we need this function is bc. the current objects needs to be already added to
		/// the asset file, before we can add child objects (we pass 'this' as argument to the AddObjectToAsset
		/// call. So we have to make sure 'this' is already part of an asset.
		/// 
		/// Remark: The need to pass arguments in the construction phase has gone, since we now use
		/// this and Undo.GetCurrentGroupName() as arguments for child assets. Nonetheless the remarks in
		/// the previous paragraph are still valid.
		/// </summary>
		public virtual void OnEnableSubAssets()
		{
		}

		public T CreateSubInstanceWithUndo<T>()
		where T : ScriptableSubAssetObject
		{
			return CreateInstanceWithUndo<T>(this, Undo.GetCurrentGroupName());
		}

		// =================================================================================================================
		// === "Destructors"
		// =================================================================================================================

		public void DestroyImmediateRecursively()
		{
			DestroyImmediateSubAssets();
			DestroyImmediate(this, true);
		}

		protected virtual void DestroyImmediateSubAssets()
		{

		}

		// #################################################################################################################
		// ### Static Class functions
		// #################################################################################################################

		// =================================================================================================================
		// === "Constructors"
		// =================================================================================================================

		public static T CreateInstanceWithUndo<T>(
			Object assetObject,
			string undoTitle
		)
		where T : ScriptableSubAssetObject
		{
			if (assetObject == null) {
				Debug.LogError("Asset Object is null!");
				return null;
			}

			// Create Annotation Type
			var obj = CreateRegularInstanceWithUndo<T>(assetObject, undoTitle);

			obj.OnEnableSubAssets();

			return obj;
		}

		public static T CreateRegularInstanceWithUndo<T>(
			Object assetObject,
			string undoTitle
		)
			where T : ScriptableObject
		{
			if (assetObject == null) {
				Debug.Log("'assetObject' is null");
				return null;
			}

			// Create Object
			var obj = CreateInstance<T>();

			// Register Undo for the created object
			Undo.RegisterCreatedObjectUndo(obj, undoTitle);

			// NO HACK - we want the AnnotationTypes shown in the hierarchy view
			// obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInBuild;
			// obj.hideFlags = HideFlags.None;

			// Add created annotationType to Asset File
			AssetDatabase.AddObjectToAsset(obj, assetObject);

			// Tell unity that there is stu	ff in the editor, that needs to be saved.
			EditorUtility.SetDirty(obj);

			return obj;
		}

	}
}
