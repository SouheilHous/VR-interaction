using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xDocBase;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.UI;


namespace xDocEditorBase.AnnotationModule {

	[InitializeOnLoad]
	public static class HierarchyViewManager
	{
		static HierarchyViewManager()
		{
			// The first statement should not be necessary, but for just in case,..
			// it also does no harm in the sense of throwing errors.
			EditorApplication.hierarchyWindowItemOnGUI -= CallbackHierarchyItemDraw;
			EditorApplication.hierarchyWindowItemOnGUI += CallbackHierarchyItemDraw;
		}

		static void CallbackHierarchyItemDraw(
			int instanceID,
			Rect rect
		)
		{
			if (!AssetManager.isFunctional) {
				// most probably asset resources couldnt be loaded due to missing / lost script assignments
				return;
			}

			// --------------------------------------------------------------------------
			// --- Find annotations in the currently handled gameObject
			// --------------------------------------------------------------------------
			// get the object from the handed over instanceID 
			Object obj = EditorUtility.InstanceIDToObject(instanceID);
			// and then get the corresponding gameObject
			GameObject gObj = (GameObject)obj;
			// If this entry is not a gameObject, quit the function
			if (gObj == null) {
				return;
			}
			// and now all the annotations in the gameObject
			XDocAnnotationBase[] aArray = gObj.GetComponents<XDocAnnotationBase>();
			// if there are no Annotations, we can stop further processing
			if (aArray.Length == 0) {
				return;
			}

			// --------------------------------------------------------------------------
			// --- Find annotaions for hierarchy view: 2 task - icons and text
			// --------------------------------------------------------------------------
			// extract all annotations, which have an icon and request an
			// icon drawn in hierarchy view. The following list will hold them
			List<XDocAnnotationBase> hIconList = new List<XDocAnnotationBase>();
			// This object references the annotation which will be drawn as text area
			XDocAnnotationBase textAnnotation = null;
			// start the search
			for (int i = 0; i < aArray.Length; i++) {
				XDocAnnotationTypeBase at = aArray[i].annotationType;
				if (at == null) {
					at = AssetManager.invalidAnnotationType;
				}
				// there is an icon draw request and an icon, add this annotation to the icon list
				if (at.icon.showIconInHierarchyView && at.icon.icon != null) {
					hIconList.Add(aArray[i]);
				}
				// there is a request to draw the annotation as textfield and no other request exists yet
				// remember this annotation to be drawn as textfield
				// only one/the first textfield request of an annotation will be considered
				if (textAnnotation == null && at.hierarchyText.showTextInHierarchyView) {
					textAnnotation = aArray[i];
				}
			}

			// --------------------------------------------------------------------------
			// --- Draw Icons
			// --------------------------------------------------------------------------
			// now get rid of all those annotations, which are not of 
			// high prio in case there are more than 1 icon to be displayed
			if (hIconList.Count > 1) {
				for (int i = hIconList.Count - 1; i >= 0; i--) {
					XDocAnnotationTypeBase at = hIconList[i].annotationType;
					if (at == null) {
						at = AssetManager.invalidAnnotationType;
					}
					if (!at.icon.highPriorityInHierarchyView) {
						hIconList.RemoveAt(i);
					}
					if (hIconList.Count == 1) {
						break;
					}
				}
			}

			// Now draw. The rect we got passed to us is up to the edge on the right side and
			// starts where the text starts - taking indentation into account
			var iconRect = new Rect(rect);
			// no padding to the right, we cannot waste space here
			iconRect.xMin = iconRect.xMax - iconRect.height;
			for (int i = hIconList.Count - 1; i >= 0; i--) {
				XDocAnnotationTypeBase at = hIconList[i].annotationType;
				if (at == null) {
					at = AssetManager.invalidAnnotationType;
				}
				using (new StateSaver.BgColor(at.icon.styleX.bgColor)) {
					string ttt = hIconList[i].commentStripped;
					if (ttt.Equals(string.Empty)) {
						ttt = "<Empty>";
					}
					EditorGUI.LabelField(iconRect, new GUIContent(at.icon.icon, ttt), at.icon.styleX.style);
					iconRect.x -= iconRect.width;
				}
			}

			// --------------------------------------------------------------------------
			// --- Draw Text Input
			// --------------------------------------------------------------------------
			if (textAnnotation != null) {
				XDocAnnotationTypeBase at = textAnnotation.annotationType;
				if (at == null) {
					at = AssetManager.invalidAnnotationType;
				}
				float cXMin = EditorGUIUtility.currentViewWidth *
				              at.hierarchyText.textWidthInHierarchyView / 100f;
				iconRect.xMin = Mathf.Min(cXMin, iconRect.xMin);

				var annotationEditor = Editor.CreateEditor(textAnnotation) as XDocAnnotationEditorBase;
				annotationEditor.DrawSingleLineCommentEntry(iconRect);
			}
		}

	}
}
