using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocEditorBase.AssetManagement;
using xDocEditorBase.UI;
using xDocBase.UI;
using xDocEditorBase.AnnotationTypeModule;


namespace xDocEditorBase.Windows
{

	public class XDocWindowSettingsTab : XoxEditorGUI.SinglePanelLayout
	{
		public override string name { get { return "Settings"; } }

		public static XDocSettingsEditorBase guiSettingsEditor;

		// HACK
		// internal property variable
		//		XDocAnnotationTypesListEditorBase _annotationTypesListEditor;


		Vector2 scrollPositionLeft;
		Vector2 scrollPositionRight;

		public XDocWindowSettingsTab (
			XDocWindow parent
		)
			: base (
				parent
			)
		{
			guiSettingsEditor = Editor.CreateEditor (AssetManager.settings) as XDocSettingsEditorBase;

			// HACK
//			_annotationTypesListEditor = 
//				Editor.CreateEditor (AssetManager.annotationTypesAsset) as XDocAnnotationTypesListEditorBase;
//			_annotationTypesListEditor.drawList.index = 3;
			
		}

		protected override void DrawPanel (
			Rect rect
		)
		{
			if ( guiSettingsEditor == null ) {
				EditorGUILayout.HelpBox ("xDoc Error: XDocSettingsEditor not created!", MessageType.Error);
				return;
			}

			var leftRightRect = XoxGUI.SplitHorizontally (
				                    rect,
				                    AssetManager.settings.styleEditorWindowXContentSub.style);

			using ( var cs = new GUISettingsGUI (
				                 leftRightRect[0], 
				                 scrollPositionLeft
			                 ) ) {
				scrollPositionLeft = cs.scrollPosition;
			}

			// HACK viewRect has to be Rect.zero !!!!!!!!!!!!!!!!!!!!
			using ( var cs = new XoxEditorGUI.ContentScope (leftRightRect[1], 
				                 scrollPositionRight, 
				                 Rect.zero
//				                 new Rect (0, 0, 500, 500)
			                 ) ) {
				scrollPositionRight = cs.scrollPosition;

				// HACK !!!!!!!!!!!!!!!!!!!!! must be empty
//				try {
//					_annotationTypesListEditor.DrawList (rect);
//				} catch ( System.Exception ex ) {
//					EditorGUI.HelpBox (rect, "xDoc Error: Can't draw Annotations Types List; left panel.\n" +
//						ex.Message + "\n" + ex.StackTrace, MessageType.Error);
//				}
//				try {
//					_annotationTypesListEditor.DrawSelected (cs.currentRect.rect);
//				} catch ( System.Exception ex ) {
//					EditorGUI.HelpBox (cs.currentRect.rect, "xDoc Error: Can't draw Annotations Types List; right panel.\n" +
//						ex.Message + "\n" + ex.StackTrace, MessageType.Error);
//				}

			}
		}
	}

	class GUISettingsGUI : XoxEditorGUI.ContentScope
	{

		static Rect viewRect = new Rect (0, 0, 500, 500);
		static readonly float scrollbarWidth;

		static GUISettingsGUI ()
		{
			GUIStyle st = GUI.skin.GetStyle ("verticalScrollbar");
			scrollbarWidth = st.fixedWidth + st.margin.left + st.margin.right;
		}

		public GUISettingsGUI (
			Rect aRect,
			Vector2 aScrollPosition
		) :
			base (
				aRect,
				aScrollPosition,
				GetAndInitViewRect (aRect)
			)
		{
			XDocWindowSettingsTab.guiSettingsEditor.Draw (currentRect);
		}

		public static Rect GetAndInitViewRect (
			Rect aPositionRect
		)
		{
			XoxGUIRect.SetSaveWidth (ref viewRect, aPositionRect.width);

			viewRect.height = GetHeightOfVPadding () +
			XDocWindowSettingsTab.guiSettingsEditor.GetHeight ();

			if ( viewRect.height >= aPositionRect.height ) {
				XoxGUIRect.SetSaveWidth (ref viewRect, viewRect.width - scrollbarWidth);
			}

			return viewRect;
		}

	}

}

