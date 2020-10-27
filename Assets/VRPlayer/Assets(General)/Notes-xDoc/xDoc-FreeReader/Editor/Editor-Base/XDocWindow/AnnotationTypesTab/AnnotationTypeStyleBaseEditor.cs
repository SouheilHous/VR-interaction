using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.Extensions;
using xDocEditorBase.UI;
using System;


namespace xDocEditorBase.AnnotationTypeModule
{

	public abstract class AnnotationTypeStyleBaseEditor : PropertyEditor
	{
		protected enum ATStyleEditorType
		{
			NONE,
			ICON,
			TITLE,
			TEXT,
			SCENE,
			HIERARCHY
		}

		abstract protected ATStyleEditorType GetEditorType ();

		public GUIStyleXEditor styleXEditor;

		Vector2 scrollPositionLeft;
		Vector2 scrollPositionRight;

		protected AnnotationTypeStyleBaseEditor (
			SerializedProperty baseProperty
		)
			: base (
				baseProperty
			)
		{
			styleXEditor = new GUIStyleXEditor (baseProperty.FindPropertyRelative ("styleX"));
		}

		// HACK override removed
		override public void Draw (
			Rect rect
		)
		{
//		Update();

			var leftRightRect = XoxGUI.SplitHorizontally (
				                    rect,
				                    AssetManager.settings.styleEditorWindowXContentSub.style);

			// SUPER HACK
			using ( var cs = new ATStyleLeftGUI (
				                 leftRightRect[0], 
				                 scrollPositionLeft,
				                 this
			                 ) ) {
				scrollPositionLeft = cs.scrollPosition;				
			}
				
			using ( var cs = new ATStyleRightGUI (
				                 leftRightRect[1], 
				                 scrollPositionRight,
				                 this
			                 ) ) {
				scrollPositionRight = cs.scrollPosition;
			}
		}

		class ATStyleLeftGUI : XoxEditorGUI.ContentScope
		{
			static Rect viewRect = new Rect (0, 0, 500, 500);
			static readonly float scrollbarWidth;

			static ATStyleLeftGUI ()
			{
				GUIStyle st = GUI.skin.GetStyle ("verticalScrollbar");
				scrollbarWidth = st.fixedWidth + st.margin.left + st.margin.right;
			}

			public ATStyleLeftGUI (
				Rect aRect,
				Vector2 aScrollPosition,
				AnnotationTypeStyleBaseEditor parent
			) :
				base (
					aRect,
					aScrollPosition,
					GetAndInitViewRect (aRect, parent)
				)
			{
				using ( new XoxEditorGUI.ChangeCheck (parent.ApplyProps) ) {
					parent.DrawOptions (currentRect);
				}

				currentRect.MoveDown ();
				currentRect.AddVSpace (5);

				parent.styleXEditor.DrawStyleBrief (currentRect);

			}

			public static Rect GetAndInitViewRect (
				Rect aPositionRect,
				AnnotationTypeStyleBaseEditor aParent
			)
			{
				XoxGUIRect.SetSaveWidth (ref viewRect, aPositionRect.width);

				viewRect.height = GetHeightOfVPadding () +
				AnnotationTypeStyleBaseEditor.GetHeight (aParent) +
				XoxGUIRect.GetHeightOfLines (1) +
				XoxGUIRect.GetHeightOfMoveDownSpace () +
				XoxGUIRect.GetHeightOfVSpace (5) +
				aParent.styleXEditor.GetHeightBrief (aParent.styleXEditor);

				if ( viewRect.height > aPositionRect.height ) {
					XoxGUIRect.SetSaveWidth (ref viewRect, viewRect.width - scrollbarWidth);
				}

				return viewRect;
			}

		}

		class ATStyleRightGUI : XoxEditorGUI.ContentScope
		{
			static Rect viewRect = new Rect (0, 0, 500, 500);
			static readonly float scrollbarWidth;

			static ATStyleRightGUI ()
			{
				GUIStyle st = GUI.skin.GetStyle ("verticalScrollbar");
				scrollbarWidth = st.fixedWidth + st.margin.left + st.margin.right;
			}

			public ATStyleRightGUI (
				Rect aRect,
				Vector2 aScrollPosition,
				AnnotationTypeStyleBaseEditor parent
			) :
				base (
					aRect,
					aScrollPosition,
					GetAndInitViewRect (aRect, parent)
				)
			{
				parent.styleXEditor.DrawSyncUtility (currentRect);
				currentRect.MoveDown ();
				parent.styleXEditor.DrawStyleVerbose (currentRect);
			}

			public static Rect GetAndInitViewRect (
				Rect aPositionRect,
				AnnotationTypeStyleBaseEditor aParent
			)
			{
				XoxGUIRect.SetSaveWidth (ref viewRect, aPositionRect.width);

				viewRect.height = GetHeightOfVPadding () +
				aParent.styleXEditor.GetHeightSyncUtility (aParent.styleXEditor) +
				aParent.styleXEditor.GetHeightVerbose (aParent.styleXEditor);

				if ( viewRect.height > aPositionRect.height ) {
					XoxGUIRect.SetSaveWidth (ref viewRect, viewRect.width - scrollbarWidth);
				}

				return viewRect;
			}

		}

		void ApplyProps ()
		{
			ApplyModifiedProperties ();
			EditorApplication.RepaintHierarchyWindow ();
			SceneView.RepaintAll ();
		}

		virtual protected void DrawOptions (
			XoxGUIRect currentRect
		)
		{
		}

		static protected float GetHeight (
			AnnotationTypeStyleBaseEditor aParent
		)
		{
			switch ( aParent.GetEditorType () ) {
			case ATStyleEditorType.ICON:
				return AnnotationTypeStyleIconEditor.GetHeight ();
			case ATStyleEditorType.TITLE:
				return AnnotationTypeStyleTitleEditor.GetHeight ();
			case ATStyleEditorType.TEXT:
				return AnnotationTypeStyleTextEditor.GetHeight ();
			case ATStyleEditorType.SCENE:
				return AnnotationTypeStyleSceneEditor.GetHeight ();
			case ATStyleEditorType.HIERARCHY:
				return AnnotationTypeStyleHierarchyTextEditor.GetHeight ();
			default:
				return 0;
			}
		}
			
	}
}

