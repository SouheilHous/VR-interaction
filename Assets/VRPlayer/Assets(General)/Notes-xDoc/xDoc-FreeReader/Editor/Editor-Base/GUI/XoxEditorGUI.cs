using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.Focus;
using xDocEditorBase.AnnotationTypeModule;


namespace xDocEditorBase.UI
{

	public static class XoxEditorGUI
	{
		static public readonly Rect hiddenRect = new Rect (-100, 0, 0, 0);
		static public readonly GUIStyle hideStyle;

		static XoxEditorGUI ()
		{
			hideStyle = new GUIStyle ();
			hideStyle.fixedHeight = 0.0001f;
			hideStyle.fixedWidth = 0.0001f;
			hideStyle.stretchHeight = false;
			hideStyle.stretchWidth = false;
			hideStyle.padding = new RectOffset ();
			hideStyle.overflow = new RectOffset ();
			hideStyle.margin = new RectOffset ();
			hideStyle.border = new RectOffset ();
			hideStyle.contentOffset = new Vector2 ();
			hideStyle.clipping = TextClipping.Clip;
			hideStyle.fontSize = -1;
		}

		public static float DrawInvalidTypesWarning (
			Rect position,
			string message,
			MessageType type
		)
		{
			var hContent = new GUIContent (message, EditorGUIUtility.FindTexture ("console.warnicon"));
			var hRect = new Rect (position);
			hRect.height = EditorStyles.helpBox.CalcHeight (hContent, position.width);
			EditorGUI.HelpBox (hRect, message, type);
			return hRect.height;
		}

		public class WarningMessage
		{
			readonly string message;
			readonly MessageType type;
			readonly GUIContent hContent;

			public WarningMessage (
				string message,
				MessageType type
			)
			{
				this.type = type;
				this.message = message;
				hContent = new GUIContent (message, EditorGUIUtility.FindTexture ("console.warnicon"));
			}

			public float GetHeight (
				float width
			)
			{
				return EditorStyles.helpBox.CalcHeight (hContent, width);
			}

			public float GetHeight (
				Rect rect
			)
			{
				return EditorStyles.helpBox.CalcHeight (hContent, rect.width);
			}

			public void Draw (
				Rect position
			)
			{
				EditorGUI.HelpBox (position, message, type);
			}

			public float DrawInJustNeededSpace (
				Rect position
			)
			{
				position.height = GetHeight (position.width);
				EditorGUI.HelpBox (position, message, type);
				return position.height;
			}
		}


		public class ProgressBarIntCancelScan : GUI.Scope
		{
			readonly int max;
			readonly string what;

			public ProgressBarIntCancelScan (
				int max,
				string what
			)
			{
				this.what = what;
				this.max = max;
			}

			protected override void CloseScope ()
			{
				EditorUtility.ClearProgressBar ();	
			}

			public bool SetCurrent (
				int current,
				string preMessage = ""
			)
			{
				current++;
				var retval = EditorUtility.DisplayCancelableProgressBar (
					             "Processing " + what + ".", 
					             preMessage + current + " " + what + " of total " + max + " " + what + " processed.", 
					             (float) current / (float) max
				             );
				return retval;
			}
		}

		public class ChangeCheck: GUI.Scope
		{
			public delegate void ChangeCheckCallback ();

			readonly ChangeCheckCallback changeCallback;

			public ChangeCheck (
				ChangeCheckCallback changeCallback
			)
			{
				this.changeCallback = changeCallback;
				EditorGUI.BeginChangeCheck ();
			}

			protected override void CloseScope ()
			{
				if ( EditorGUI.EndChangeCheck () ) {
					changeCallback ();
				}
			}
		}

		public class ChangeCheck<T>: GUI.Scope
		{
			public delegate T ChangeCheckCallback ();

			readonly ChangeCheckCallback changeCallback;

			public ChangeCheck (
				ChangeCheckCallback changeCallback
			)
			{
				this.changeCallback = changeCallback;
				EditorGUI.BeginChangeCheck ();
			}

			protected override void CloseScope ()
			{
				if ( EditorGUI.EndChangeCheck () ) {
					changeCallback ();
				}
			}
		}

		public class ContentScope: ContentScopeNoScroll
		{
			public ContentScope (
				Rect aRect,
				Vector2 aScrollPosition,
				Color aBgColor,
				Rect aViewRect
			)
				: base (
					aRect,
					aBgColor,
					aViewRect
				)
			{
				scrollPosition = GUI.BeginScrollView (aRect, aScrollPosition, aViewRect);
			}

			public ContentScope (
				Rect aRect,
				Vector2 aScrollPosition,
				Rect aViewRect
			)
				: base (
					aRect,
					aViewRect 
				)
			{
				scrollPosition = GUI.BeginScrollView (aRect, aScrollPosition, aViewRect);
			}

			protected override void CloseScope ()
			{
				GUI.EndScrollView ();
				base.CloseScope ();
			}

			public Vector2 scrollPosition { 
				get;
				private set;
			}
		}

		public class ContentScopeNoScroll : GUI.Scope
		{
			
			EditorStateSaver.Indent indent;
			StateSaver.BgColor bg;
			//TIDYUP
			//			readonly GUILayout.AreaScope areaScope;

			// currentRect is the rect used to draw the GUI elements. It has already
			// some correct padding and can be moved around.
			// It is only here for convenience,.. so we dont have to create one everywhere else.
			public XoxGUIRect currentRect;

			static float vPadding;

			static ContentScopeNoScroll ()
			{
				vPadding = AssetManager.settings.styleEditorWindowXContentSub.style.padding.top +
				AssetManager.settings.styleEditorWindowXContentSub.style.padding.bottom;
			}

			public ContentScopeNoScroll (
				Rect rect,
				Color bgColor
			)
			{
				Initialize (rect, bgColor, rect);
			}

			public ContentScopeNoScroll (
				Rect rect
			)
			{
				Initialize (rect, AssetManager.settings.styleEditorWindowXContentSub.bgColor, rect);
			}

			public ContentScopeNoScroll (
				Rect rect,
				Color bgColor,
				Rect vRect
			)
			{
				Initialize (rect, bgColor, vRect);
			}

			public ContentScopeNoScroll (
				Rect rect,
				Rect vRect
			)
			{
				Initialize (rect, AssetManager.settings.styleEditorWindowXContentSub.bgColor, vRect);
			}

			void Initialize (
				Rect aRect,
				Color aBgColor,
				Rect aViewRect
			)
			{
				indent = new EditorStateSaver.Indent (0);
				bg = new StateSaver.BgColor (aBgColor);
				GUI.Box (aRect,
					GUIContent.none,
					AssetManager.settings.styleEditorWindowXContentSub.style);
				bg.Reset ();

				currentRect = new XoxGUIRect (aViewRect, AssetManager.settings.styleEditorWindowXContentSub);
			}

			protected override void CloseScope ()
			{
				// DEBUG
//				if (XoxEditorScopeAccountant.emergency)
//					Debug.Log ("Closing");

//				areaScope.Dispose ();
				bg.Dispose ();
				indent.Dispose ();
			}

			public static float GetHeightOfVPadding ()
			{
				return	vPadding;
			}

		}

		public abstract class EditorWindowX : EditorWindow
		{
			protected bool showToolbar;
			int previousTabIndex = 0;
			int mainTabIndex = 0;
			string[] toolbarNames;

			List<EditorWindowXContent> contentList;

			protected void OnEnable ()
			{
				// needed for the 2PanelLayout -> resizer
				wantsMouseMove = true;

				toolbarNames = new string[0];
				contentList = new List<EditorWindowXContent> ();

			}

			protected void AddContent (
				EditorWindowXContent content
			)
			{
				contentList.Add (content);
				toolbarNames = new string[contentList.Count];
				for ( int i = 0 ; i < contentList.Count ; i++ ) {
					toolbarNames[i] = contentList[i].name;
				}
				content.OnEnable ();
			}

			public float toolbarHeight { 
				get { return AssetManager.settings.styleToolbarButton.style.fixedHeight; } 
			}

			void OnDestroy ()
			{
				for ( int i = 0 ; i < contentList.Count ; i++ ) {
					contentList[i].OnDestroy ();
				}
			}

			void OnGUI ()
			{
				if ( !AssetManager.isFunctional ) {
					// most probably asset resources couldnt be loaded due to missing / lost script assignments
					EditorGUILayout.HelpBox (AssetManager.errMessageCantLoadAssetResources, MessageType.Error);
					return;
				}
					
				using ( new XoxEditorGUILayout.HorizontalScope () ) {
					mainTabIndex = GUILayout.Toolbar (mainTabIndex, toolbarNames, AssetManager.settings.styleToolbarButton.style);
					if ( utilityButtonCommand != null ) {
						using ( new StateSaver.BgColor (AssetManager.settings.toolbarAddAnnotationButtonBG) ) {
							if ( GUILayout.Button (
								     utilityButtonContent,
								     AssetManager.settings.styleToolbarButton.style,
								     GUILayout.Width (toolbarHeight)) ) {
								utilityButtonCommand ();
							}
						}
					}
				}
				if ( previousTabIndex != mainTabIndex ) {
					previousTabIndex = mainTabIndex;
					FocusUtility.LoseFocus ();
				}
				// Just to be sure that the index is in range
				mainTabIndex = Mathf.Clamp (mainTabIndex, 0, contentList.Count);
				contentList[mainTabIndex].Draw ();


				/*
				 * This code had worked
				xDocEditorBase.Windows.XDocWindowAnnotationTypesTab xx = contentList [2] as 
					xDocEditorBase.Windows.XDocWindowAnnotationTypesTab;
				Rect rr = new Rect (100, 100, 200, 200);
				EditorGUI.LabelField (rr, "asdkfjhasdjkhf");
				Rect rrr = new Rect (100, 200, 200, 200);
				var at = xx.annotationTypesListEditor.GetSelectedAnnotationType ();
				if (at != null) {
					var ed = Editor.CreateEditor (at) as XDocAnnotationTypeEditorBase;
					var ed2 = ed.iconEditor;
					var styleXEd = ed2.styleXEditor;
					using (new XoxEditorGUI.ChangeCheck (styleXEd.ApplyProps)) {
						EditorGUILayout.PropertyField (styleXEd.spFgColor, new GUIContent ("Foreground Color"));
					}
				}
				*/

			}


			#region UtilityButton

			public delegate void UtilityButtonCommand ();

			GUIContent utilityButtonContent = null;
			UtilityButtonCommand utilityButtonCommand = null;

			public void SetUtilityButton (
				GUIContent content,
				UtilityButtonCommand command
			)
			{
				utilityButtonContent = content;
				utilityButtonCommand = command;
			}

			#endregion


			void OnLostFocus ()
			{
				if ( mainTabIndex >= contentList.Count ) {
					// this can happen during de-/serialization (code changed e.g.)
					return;
				}
				contentList[mainTabIndex].OnLostFocus ();
			}

			int extraRepaintCall;
			const int extraRepaintCallSkipper = 0;

			/// <summary>
			/// This gets called by unity 10 times a second, we will make sure our windows update 
			/// 3 times a seconds
			/// </summary>
			public void OnInspectorUpdate ()
			{
				if ( !AssetManager.isFunctional ) {
					// most probably asset resources couldnt be loaded due to missing / lost script assignments
					return;
				}

				if ( !contentList[mainTabIndex].requestConstantRepaint ) {
					return;
				}
				extraRepaintCall++;
				if ( extraRepaintCall < extraRepaintCallSkipper ) {
					return;
				}
				extraRepaintCall = 0;
				// Call Repaint on OnInspectorUpdate as it repaints the windows
				// less times as if it was OnGUI/Update
				Repaint ();
			}
		}

		public abstract class EditorWindowXContent
		{
			protected readonly EditorWindowX parent;

			public virtual bool requestConstantRepaint { get { return true; } }

			protected EditorWindowXContent (
				EditorWindowX parent
			)
			{
				this.parent = parent;
			}

			public abstract string name { get; }

			public abstract void Draw ();

			public virtual void OnEnable ()
			{
			}

			public virtual void OnLostFocus ()
			{
			}

			public virtual void OnDestroy ()
			{
			}

			public void Repaint ()
			{
				parent.Repaint ();
			}

		}

		public abstract class SinglePanelLayout : EditorWindowXContent
		{
			readonly GUIStyle windowStyle;
			readonly float toolbarHeight;

			Rect areaRect;

			protected virtual void DrawPanel (
				Rect rect
			)
			{
			}

			protected SinglePanelLayout (
				EditorWindowX parent
			)
				: base (
					parent
				)
			{
				toolbarHeight = parent.toolbarHeight;
				windowStyle = AssetManager.settings.styleEditorWindowXContent.style;
			}

			public override void Draw ()
			{
				CalculateGeometry ();
				DrawPanel (areaRect);	
			}

			void CalculateGeometry ()
			{
				// === area
				areaRect = new Rect (
					windowStyle.padding.left, 
					toolbarHeight + windowStyle.padding.top, 
					parent.position.width - windowStyle.padding.left - windowStyle.padding.right, 
					parent.position.height - windowStyle.padding.top - windowStyle.padding.bottom - toolbarHeight
				);
			}
		}

		/// <summary>
		/// Provides a two panel layout, with a left and right panel; and a splitter.
		/// The splitter can be used to redistribute the area horizontally between the left
		/// and right panel: just click on it and drag it.
		/// A class wanting to use this layout must inherit from it, set the reference to the
		/// EditorWindow, which in which these GUI elements will be drawn and implement the
		/// DrawLeftPanel and DrawRightPanel functions.
		/// The parent EditorWindow must call the Draw function in its OnGUI cycle.
		/// There is a ready made class EditorWindowX, which inherits from EditorWindow and
		/// already draws the toolbar and calls this Draw function. So this class should best
		/// be used together with the EditorWindowX class.
		/// </summary>
		public abstract class TwoPanelLayout : EditorWindowXContent
		{
			readonly GUIStyle splitterStyle;
			readonly GUIStyle windowStyle;
			readonly float toolbarHeight;

			float leftPanelWidth = 150;

			Rect areaRect;
			Rect leftPanelRect;
			Rect rightPanelRect;
			Rect splitterRect;
			#pragma warning disable 0649
			// These pragmas suppress the following unnecessary warning:
			// Assets/Editor Default Resources/xDoc-FreeReader/Editor/Editor-Base/XoxEditorGUI.cs(470,9): warning CS0649: Field `xDocEditorBase.UI.XoxEditorGUI.TwoPanelLayout.splitterDrawRect' is never assigned to, and will always have its default value
			// splitterDrawRect is a struct and its members are directly set in 'CalculateGeometry'
			Rect splitterDrawRect;
			#pragma warning restore 0649

			bool isResizing = false;

			/// <summary>
			/// This function is not intended to be used to draw GUI elements, but
			/// to initialize the draw cycle, in case it is needed; e.g. setting up
			/// the focus manager.
			/// </summary>
			protected virtual void DrawCycleStart ()
			{
			}

			/// <summary>
			/// This function is not intended to be used to draw GUI elements, but
			/// to finalize the draw cycle, in case it is needed.
			/// </summary>
			protected virtual void DrawCycleEnd ()
			{
			}

			/// <summary>
			/// Override this function to create the GUI elements in the left panel.
			/// </summary>
			/// <param name="rect">Rect.</param>
			protected virtual void DrawLeftPanel (
				Rect rect
			)
			{
			}

			/// <summary>
			/// Override this function to create the GUI elements in the right panel.
			/// </summary>
			/// <param name="rect">Rect.</param>
			protected virtual void DrawRightPanel (
				Rect rect
			)
			{
			}

			protected TwoPanelLayout (
				EditorWindowX parent
			)
				: base (
					parent
				)
			{
				toolbarHeight = parent.toolbarHeight;
				splitterStyle = AssetManager.settings.styleVerticalSplitter.style;
				windowStyle = AssetManager.settings.styleEditorWindowXContent.style;
			}

			public override void Draw ()
			{
				DrawCycleStart ();
				TwoPanelOnGUIUpdate ();
				DrawLeftPanel (leftPanelRect);	
				DrawRightPanel (rightPanelRect);	
				DrawCycleEnd ();
			}

			/// <summary>
			/// Calculates the geometry of this Layout.
			/// Most importantly the left and right panel, but also the rects for the splitter.
			/// </summary>
			void CalculateGeometry ()
			{
				// === area, which is available in the window. We assume there will be always a toolbar.
				areaRect = new Rect (
					windowStyle.padding.left, 
					toolbarHeight + windowStyle.padding.top, 
					parent.position.width - windowStyle.padding.left - windowStyle.padding.right, 
					parent.position.height - windowStyle.padding.top - windowStyle.padding.bottom - toolbarHeight
				);

				// === left panel
				// limit resizing, so that none of the panels gets invalid, invis, etc.
				const float leftPanelMinAbs = 100f;
				const float leftPanelMaxAbs = 300f;
				const float leftPanelMinRel = 0.3f;
				const float leftPanelMaxRel = 0.7f;
				float leftPanelMinWidth = Mathf.Min (leftPanelMinAbs, leftPanelMinRel * parent.position.width);
				float leftPanelMaxWidth = Mathf.Min (leftPanelMaxAbs, leftPanelMaxRel * parent.position.width);
				leftPanelWidth = Mathf.Clamp (leftPanelWidth, leftPanelMinWidth, leftPanelMaxWidth);

				leftPanelRect = new Rect (areaRect.x, areaRect.y, leftPanelWidth, areaRect.height);

				// === splitter, hit area
				float splitterX = leftPanelRect.x + leftPanelRect.width;
				float splitterWidth = splitterStyle.fixedWidth + splitterStyle.padding.left + splitterStyle.padding.right;

				splitterRect = new Rect (splitterX, areaRect.y, splitterWidth, areaRect.height);

				// === splitter DRAW RECT
				splitterDrawRect.x = splitterRect.x + splitterStyle.padding.left;
				splitterDrawRect.width = splitterStyle.fixedWidth;
				splitterDrawRect.y = splitterRect.y + splitterStyle.padding.top;
				splitterDrawRect.yMax = splitterRect.yMax - splitterStyle.padding.bottom;

				// === right panel
				float rightPanelX = splitterX + splitterWidth;
				float rightPanelWidth = areaRect.width - rightPanelX + windowStyle.padding.left;

				rightPanelRect = new Rect (rightPanelX, areaRect.y, rightPanelWidth, areaRect.height);
			}

			void TwoPanelOnGUIUpdate ()
			{
				CalculateGeometry ();

				using ( new StateSaver.BgColor (splitterStyle.normal.textColor) ) {
					// draw splitter
					GUI.Box (splitterDrawRect, GUIContent.none, AssetManager.settings.styleVerticalSplitter.style);

					// show resizing cursor
					if ( isResizing ) {
						EditorGUIUtility.AddCursorRect (areaRect, MouseCursor.ResizeHorizontal);
					} else {
						EditorGUIUtility.AddCursorRect (splitterRect, MouseCursor.ResizeHorizontal);
					}

					// event handling
					switch ( Event.current.type ) {
					case EventType.MouseDown:
						isResizing = splitterRect.Contains (Event.current.mousePosition);
						parent.Repaint ();
						break;
					case EventType.MouseUp:
						isResizing = false;
						parent.Repaint ();
						break;
					case EventType.MouseMove:
						parent.Repaint ();
						break;
					case EventType.MouseDrag:
					// this is where resizing is really done
						if ( isResizing ) {
							leftPanelWidth += Event.current.delta.x;
						}
						parent.Repaint ();
						break;
					}
				}
			}

		}

	
	}
}
