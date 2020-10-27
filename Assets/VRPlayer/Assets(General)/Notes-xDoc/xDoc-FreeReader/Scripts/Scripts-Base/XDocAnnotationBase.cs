using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.CustomData;
using xDocBase.Extensions;
using xDocBase.UI;
using Object = UnityEngine.Object;


// _FREQ --- make annotations for project path items
// _FREQ --- project view and hierarchy with icons
// _FREQ --- full color tinted annotation style
// _FREQ --- show pdf files
// _FREQ --- show StyleX sample formatted text above of the settings (in the Settings Windows-Annotation Types)
// _FREQ -- Open calendar on date data type
// _FREQ -- add types: image, imageAlpha, imagePreview, textArea?
// _FREQ - persistance for playmode changes
// _FREQ -- multiple object add
// _FREQ -- remove unnecessary funtionality from MultiTool (sizers e.g.)
// _FREQ - Display annotations in sceneview for selected objects at a fixed position in the view e.g. lower left

namespace xDocBase {

	public abstract class XDocAnnotationBase : MonoBehaviour, IComparable
	{

#region Main

		/// <summary>
		/// Reset this instance. 
		/// Reset is called to initialize the script’s properties when it is first attached 
		/// to the object and also when the Reset command is used.
		/// </summary>
		void Reset()
		{
			SetAnnotationType(AssetManager.defaultAnnotationType);
		}

#endregion


#region Data Referenced: Annotation Type

		/// <summary>
		/// The type of the annotation. It defines various styles of representation and
		/// available custom data.
		/// 
		/// Initialization is done by the inspector/editor. If the inspector finds a null value,
		/// it will assign the default annotationType, which it gets from the AssetManager.
		/// In principle one would like to initialize here, but with serialization and
		/// being a MonoBehaviour and thus not being able to use the constructor other trys
		/// to initialize fails - also because the assetManager is not set up yet during 
		/// serialization (which happens during IDE start, code change, play, stop, etc -
		/// nearly all the time)
		/// 
		/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		/// !!! DO NOT SET THE ANNOTATION TYPE DIRECTLY
		/// !!! use the SetAnnotationType function
		/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		/// </summary>
		public XDocAnnotationTypeBase annotationType;

		public XDocAnnotationTypeBase annotationTypeNotNull {
			get {
				return annotationType ?? AssetManager.invalidAnnotationType;
			}
		}

		public void SetAnnotationType(
			XDocAnnotationTypeBase annotationType
		)
		{

			Undo.RegisterFullObjectHierarchyUndo(this, "Set Annotation Type");
			this.annotationType = annotationType;
			if (annotationType != null) {
				MigrateAndAdaptCustomData(false);
			}
			if (gameObject.IsConnectedPrefabInstance()) {
				PrefabUtility.RecordPrefabInstancePropertyModifications(this);
			} else if (gameObject.IsPrefab()) {
				EditorUtility.SetDirty(gameObject);
//				PrefabUtility.MergeAllPrefabInstances(gameObject);
			}
		}

		/// <summary>
		/// Migrates and adapts custom data.
		/// </summary>
		/// <returns><c>true</c>, if custom data was migrated, <c>false</c> otherwise.</returns>
		public bool MigrateAndAdaptCustomData(
			bool registerUndo = true
		)
		{
			if (registerUndo) {
				Undo.RegisterFullObjectHierarchyUndo(this, "Apply Custom Data");
			}
		
			// this is the list of custom data (types) in the annotationType! - status how it should be - without the data
			List<DataField> refList = annotationType.data.dataFieldList;

			// both list have to match in order and content of the MASTER-Attributes
			if (!refList.IsMasterAttributeSyncedWithOtherList(dataFieldList)) {
				// if there is a mismatch, the list in the annotation has to be rebuild
				dataFieldList = refList.GetMasterAttributeAndIdAndNameSyncedList<DataField, QuasiUniqueId>(dataFieldList);
				return true;
			}
			return false;
		}

#endregion


#region Search Utility Functions

		/* Search Areas:
	 * Game Object Name
	 * Annotation Type (Name)
	 * Prefab
	 * All Custom Data Fields
	 * isStatic
	 * layer
	 * tag
	 * parents (name)
	 * - hasParent
	 * childs (name)
	 * - hasChilds
	 * Annotation Content/Comment/Excerpt
	 * -Components (names)
	 * */

		public string GetCustomDataString()
		{
			if (dataFieldList.Count == 0) {
				return "";
			}
			string retVal = dataFieldList[0].GetNameValuePairAsString();
			for (int i = 1; i < dataFieldList.Count; i++) {
				retVal += "; " + dataFieldList[i].GetNameValuePairAsString();
			}
			return retVal;
		}

		public static Regex regexWhiteSpaceDenser = new Regex(@"\s+", RegexOptions.None);

		public string GetExcerpt()
		{
			// Annotation comment excerpt
			// format the content string: dense down whitespaces, shorten string if needed
			string excerpt = commentStripped;
			int excerptMaxLen = AssetManager.settings.excerptLength;
			if (excerpt.Length >= excerptMaxLen) {
				excerpt = excerpt.Remove(excerptMaxLen);
				excerpt += "...";
			}
			excerpt = excerpt.Trim();
			excerpt = regexWhiteSpaceDenser.Replace(excerpt, @" ");
			return excerpt;
		}

		static string GetComponentName(
			Component component
		)
		{
			if (component.GetType().IsSubclassOf(typeof(XDocAnnotationBase))) {
				return ((XDocAnnotationBase)component).annotationTypeNotNull.Name;
			} else {
				return component.GetType().Name;
			}
		}

		public string GetComponentsString()
		{
			var cArray = transform.GetComponents<Component>();
			var retVal = "Components: " + GetComponentName(cArray[0]);
			for (int i = 1; i < cArray.Length; i++) {
				Component c = cArray[i];
				retVal += ", " + GetComponentName(c);
			}
			return retVal;
		}

		public string GetComponentsStringToSearch()
		{
			var cArray = transform.GetComponents<Component>();
			// This is the transform
			var retVal = "Component: " + GetComponentName(cArray[0]);
			for (int i = 1; i < cArray.Length; i++) {
				Component c = cArray[i];
				retVal += ", Component: " + GetComponentName(c);
			}
			return retVal;
		}

		public string GetGameObjectAndTypeString()
		{
			var retVal = name + " (" + annotationTypeNotNull.Name + ")";
			// old implementation
//			if (gameObject.IsPrefab()) {
//				retVal += " * PREFAB *";
//			}		
			var prefabType = PrefabUtility.GetPrefabType(gameObject);
			switch (prefabType) {
			case PrefabType.None:
				break;
			default:
				retVal += " * " + prefabType + " * ";
				break;
			}

			return retVal;
		}

		public string GetGameObjectAndTypeStringToSearch()
		{
			var retVal = "Name: " + name +
			             ", Type: " + annotationTypeNotNull.Name;
			if (gameObject.IsPrefab()) {
				retVal += ", isPrefab";
			} else {
				retVal += ", noPrefab";
			}
			// --------------------------------------------------------------------------
			// --- added
			// --------------------------------------------------------------------------
			var prefabType = PrefabUtility.GetPrefabType(gameObject);
			switch (prefabType) {
			case PrefabType.None:
				break;
			default:
				retVal += " * " + prefabType + " * ";
				break;
			}
			// --------------------------------------------------------------------------
			return retVal;
		}

		public string GetTagLayerStaticString()
		{
			return "Tag: " + transform.tag +
			", Layer: " + LayerMask.LayerToName(gameObject.layer) +
			(gameObject.IsPrefab() ? "" : ", Scene: " + gameObject.scene.name) +
			(gameObject.isStatic ? ", static" : "");
		}

		public string GetTagLayerStaticStringToSearch()
		{
			return "Tag: " + transform.tag +
			", Layer: " + LayerMask.LayerToName(gameObject.layer) +
			(gameObject.IsPrefab() ? "" : ", Scene: " + gameObject.scene.name) +
			(gameObject.isStatic ? ", isStatic" : ", isntStatic");
		}

		public string GetParentsStringToSearch()
		{
			var retVal = "";
			var parentItem = transform.parent;
			while (parentItem != null) {
				retVal += "Parent: " + parentItem.name + ", ";						
				parentItem = parentItem.transform.parent;
			}
			if (retVal.Length > 0) {
				retVal += "hasParent, isChild";
			} else {
				retVal += "noParent, isntChild";
			}
			return retVal;
		}

		public string GetChildrenStringToSearch()
		{
			var retVal = "";
			for (int i = 0; i < transform.childCount; i++) {
				retVal += "Child: " + transform.GetChild(i) + ", ";						
			}
			if (retVal.Length > 0) {
				retVal += "hasChildren, isParent";
			} else {
				retVal += "noChildren, isntParent";
			}
			return retVal;
		}

#endregion


#region Data: Value Based Content

		// real content data members
		public string comment = "";
		public string commentStripped = "";
		public List<DataField>	dataFieldList;
		public List<Object>	objectReferenceList;

		// local inspector style
		public bool richText = true;
		public bool wordwarp = true;
		public float maxHeight = 200;
		public float maxHeightCD = 200;

		// local scene view style
		public bool richTextSceneView = true;
		public bool wordwarpSceneView = true;
		public float maxHeightSceneView = 0;
		public float maxWidthSceneView = 0;

#endregion


#region IComparable implementation

		/// <summary>
		/// Compares the current instance with another object of the same type and returns an integer that indicates whether
		/// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
		/// We need this funtionality to order lists of annotations.
		/// </summary>
		/// <returns>The result of comparing the (gameo)bject names</returns>
		/// <param name="obj">Object.</param>
		public int CompareTo(
			object obj
		)
		{
			XDocAnnotationBase other = (XDocAnnotationBase)obj;
			return gameObject.name.CompareTo(other.gameObject.name);
		}

#endregion


#region Gizmo

		// This array will contain all annotations in the current gameObject
		[System.NonSerialized]
		XDocAnnotationBase[] cAnnotationsArray;
		[System.NonSerialized]
		int ownIndex = -1;
		[System.NonSerialized]
		float offsetSceneView;

		void CalcAnnotationOrderAndPositionInGameObject()
		{
			// The object is in viewing distance and we have to draw the annotations in scene view.
			// First we need to get all Annotations in the object and their order and the relative 
			// position of this annotation in relation to the other annotations.
			ownIndex = -1;
			cAnnotationsArray = transform.GetComponents<XDocAnnotationBase>();
			// get the own positions in the list
			for (int i = 0; i < cAnnotationsArray.Length; i++) {
				if (cAnnotationsArray[i] == this) {
					ownIndex = i;
				}
			}
			// own index should never be still -1 at this point
			if (ownIndex < 0) {
				throw new InvalidOperationException("Annotation not registered as component in gameObject.");
			}
		}

		bool IsNotInViewingDistance()
		{
			var at = annotationTypeNotNull;

			Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;
			float viewDistance = Vector3.Distance(sceneViewCamera.transform.position, transform.position);
			if (viewDistance > at.scene.sceneViewMaxDistance)
				return true;
			if (viewDistance < at.scene.sceneViewMinDistance)
				return true;
			// means, it is in viewing distance
			return false;
		}

		public float DrawAnnotationInSceneView(
			float offset
		)
		{
			var at = annotationTypeNotNull;

			// Check, if the object is in viewing distance. If not, we can just abort.
			if (IsNotInViewingDistance()) {
				return 0;
			}

			// --------------------------------------------------------------------------
			// --- Raycasts
			// --------------------------------------------------------------------------
			DrawRaycasts();

			// --------------------------------------------------------------------------
			// --- Text Labels
			// --------------------------------------------------------------------------

			// But first we need to adjust the style
			var style = at.scene.styleX.style;
			var bgColor = at.scene.styleX.bgColor;
			style.wordWrap = wordwarpSceneView;
			style.richText = richTextSceneView;

			// abort if this annotationType should not be drawn in scene view
			if (!at.scene.showInSceneView) {
				return 0;
			}

			Rect r;
			// now draw
			using (new StyleSaver.FixedHeight(style))
			using (new StyleSaver.FixedWidth(style))
			using (new XoxGUI.HandlesScope())
			using (new StateSaver.BgColor(bgColor)) {
				if (maxHeightSceneView != 0) {
					style.fixedHeight = maxHeightSceneView - style.padding.vertical;
				}
				if (maxWidthSceneView != 0) {
					style.fixedWidth = maxWidthSceneView - style.padding.horizontal;
				}
				if (style.fixedWidth < 0)
					style.fixedWidth = 0;
				if (style.fixedHeight < 0)
					style.fixedHeight = 0;
				string sceneText = comment;
				if (dataFieldList != null) {
					foreach ( var data in dataFieldList ) {
						if (data.showInSceneMode) {
							sceneText += "\n* " + data.GetNameValuePairAsString();
						}
					}
				}
				if (style.fixedWidth > 0) {
					r = HandleUtility.WorldPointToSizedRect(transform.position, new GUIContent(sceneText), style);
					r.y += offset;
					float size = style.CalcHeight(new GUIContent(sceneText), style.fixedWidth);
					r.height = size;
				} else {
					r = HandleUtility.WorldPointToSizedRect(transform.position, new GUIContent(sceneText), style);
					r.y += offset;
				}
				GUI.TextField(r, sceneText, style);
			}

			return r.height;
		}

		void DrawRaycasts()
		{
			var at = annotationTypeNotNull;

			if (objectReferenceList == null)
				return;		
			if (!at.data.useObjectReferencesList)
				return;
			if (!at.scene.raycastToGameobjectsInObjectsList)
				return;
			foreach ( var obj in objectReferenceList ) {
				if (obj == null)
					continue;
				if (obj.GetType() != typeof(GameObject))
					continue;
				GameObject gObj = (GameObject)obj;
				using (new StateSaver.GizmoColor(at.scene.raycastColor))
					Gizmos.DrawLine(transform.position, gObj.transform.position);
			}
		}

		void OnDrawGizmos()
		{
			CalcAnnotationOrderAndPositionInGameObject();
			// Now we have the list of all annotations in the gameObject. When drawing we 
			// have to make sure they do not overlap, just are drawn one below the other.
			// The topmost annotation in the gameObject will orchester all GizmoDrawing.
			// It is the one with the index 0 (first in the list) and if this annotation
			// is not at index 0, we will stop furter processing.
			// own index should never be still -1 at this point
			if (ownIndex < 0) {
				throw new InvalidOperationException("Annotation not initialized correctly in gameObject. Call to 'CalcAnnotationOrderAndPositionInGameObject' may be missing.");
			}
			if (ownIndex > 0) {
				return;
			}

			// Now we know we are in the topmost annotation and the gameObject is in viewing
			// distance. Thus we must draw and initiate drawing at the right positions, so the
			// Gizmas/Handles appear below each other.
			offsetSceneView = 0;
			for (int i = 0; i < cAnnotationsArray.Length; i++) {
				offsetSceneView += cAnnotationsArray[i].DrawAnnotationInSceneView(offsetSceneView);
			}
		}

#endregion


	}
}
