using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xDocBase;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.UI;


namespace xDocEditorBase.Search
{

	public class SearchMatchedAnnotation
	{
		readonly XDocAnnotationBase annotation;
		readonly XDocAnnotationTypeBase annotationType;
		readonly Transform transform;
	
		readonly GUIStyle itemStyle;
		readonly GUIStyle itemLineIconStyle;
		readonly GUIStyle textStyle;
		readonly GUIStyle componentStyle;
	
		readonly GUIContent itemLineTextContent;
		readonly float itemLineIconHeight;
		readonly Texture itemLineIcon;
		readonly Color itemLineIconBGColor;
	
		readonly List<Object> parentsList;
		readonly bool parentsAvailable = false;
		readonly XoxEditorGUILayout.HorizontalScopeWithLineBreak pDrawer;
	
		readonly List<Object> childrenList;
		readonly bool childrenAvailable = false;
		readonly XoxEditorGUILayout.HorizontalScopeWithLineBreak cDrawer;
	
		readonly string excerpt;
		readonly string componentsString;
		readonly string customDataString;
		readonly bool hasCustomData;
		readonly string goAttributes;
	
		float contentWidth;

		public SearchMatchedAnnotation (
			XDocAnnotationBase annotation
		)
		{
			this.annotation = annotation;
			annotationType = annotation.annotationTypeNotNull;
			transform = annotation.transform;
	
			itemStyle = AssetManager.settings.styleSearchItemLine.style;
	
			// --------------------------------------------------------------------------
			// --- GameObject - Title Line
			// --------------------------------------------------------------------------
			itemLineTextContent = new GUIContent (annotation.GetGameObjectAndTypeString ());
			itemLineIcon = annotation.annotationTypeNotNull.icon.icon;
			itemLineIconHeight = itemStyle.CalcSize (itemLineTextContent).y;
			itemLineIconBGColor = annotationType.icon.styleX.bgColor;
			itemLineIconStyle = annotationType.icon.styleX.style;
	
			// --------------------------------------------------------------------------
			// --- parents link list
			// --------------------------------------------------------------------------
			parentsList = new List<Object> ();
			var parentItem = transform.parent;
			while (parentItem != null) {
				parentsList.Add (parentItem);						
				parentItem = parentItem.transform.parent;
			}
			if (parentsList.Count > 0) {
				parentsList.Reverse ();
				parentsAvailable = true;
				pDrawer = new XoxEditorGUILayout.HorizontalScopeWithLineBreak (
					"Parent object(s):",
					" > ", 
					AssetManager.settings.styleSearchItemParent.style,
					AssetManager.settings.styleSearchItemParentSubItem.style
				);
			}
	
			// --------------------------------------------------------------------------
			// --- children link list
			// --------------------------------------------------------------------------
			childrenList = new List<Object> ();
			for (int i = 0; i < transform.childCount; i++) {
				childrenList.Add (transform.GetChild (i));						
			}
			if (childrenList.Count > 0) {
				childrenAvailable = true;
				cDrawer = new XoxEditorGUILayout.HorizontalScopeWithLineBreak (
					"Child object(s): ",
					", ", 
					AssetManager.settings.styleSearchItemParent.style,
					AssetManager.settings.styleSearchItemChildSubItem.style
				);
			}
	
			// --------------------------------------------------------------------------
			// --- Excerpt
			// --------------------------------------------------------------------------
			textStyle = AssetManager.settings.styleSearchItemText.style;
			excerpt = annotation.GetExcerpt ();
	
			// --------------------------------------------------------------------------
			// --- Components
			// --------------------------------------------------------------------------
			componentStyle = AssetManager.settings.styleSearchItemComponents.style;
			componentsString = annotation.GetComponentsString ();
	
			// --------------------------------------------------------------------------
			// --- CustomData
			// --------------------------------------------------------------------------
			customDataString = annotation.GetCustomDataString ();
			hasCustomData = customDataString.Length > 0;
	
			// --------------------------------------------------------------------------
			// --- Tag, Layer
			// --------------------------------------------------------------------------
			goAttributes = annotation.GetTagLayerStaticString ();
	
		}

		public bool isInvalid { get { return annotation == null; } }

	
		public void DrawTitleLine (
			float contentWidth
		)
		{
			this.contentWidth = contentWidth;
	
			// --------------------------------------------------------------------------
			// --- Space
			// --------------------------------------------------------------------------
			GUILayout.Space (15);
	
			// --------------------------------------------------------------------------
			// --- link to gameObject / object
			// --------------------------------------------------------------------------
			using (new EditorGUILayout.HorizontalScope (GUILayout.ExpandWidth (true))) {
				// text
				if (GUILayout.Button (itemLineTextContent, itemStyle)) {
					EditorGUIUtility.PingObject (annotation);
					Selection.activeObject = annotation;
				}
				EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
	
				// icon
				if (itemLineIcon != null) {
					using (new StateSaver.BgColor (itemLineIconBGColor)) {
						GUILayout.Box (new GUIContent (itemLineIcon), 
							itemLineIconStyle,
							GUILayout.Height (itemLineIconHeight), 
							GUILayout.Width (itemLineIconHeight));
					}
					GUILayout.Space (itemLineIconHeight);
				}
			}			
		}

		public void DrawParents ()
		{
			if (parentsAvailable) {
				pDrawer.Draw (parentsList, contentWidth);
			}				
	
		}

		public void DrawChildren ()
		{
			if (childrenAvailable) {
				cDrawer.DrawSeparatorKeptWithPredecessor (childrenList, contentWidth);
			}				
	
		}

	
		public void DrawExcerpt ()
		{
			EditorGUILayout.LabelField (excerpt, 
				textStyle, 
				GUILayout.Width (contentWidth));
		}

	
		public void DrawComponents ()
		{
			EditorGUILayout.LabelField (componentsString, 
				componentStyle, 
				GUILayout.Width (contentWidth));
	
		}

		public void DrawCustomData ()
		{
			if (hasCustomData) {
				EditorGUILayout.LabelField (customDataString, 
					componentStyle, 
					GUILayout.Width (contentWidth));
			}
		}

	
		public void DrawGOAttributes ()
		{
			EditorGUILayout.LabelField (goAttributes, 
				componentStyle, 
				GUILayout.Width (contentWidth));
		}
	
	}
}

