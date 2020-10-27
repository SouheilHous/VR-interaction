using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace xDocEditorBase.UI
{

	public static class XoxEditorGUILayout
	{
		#region Separator

		public const float spacerWidth = 2f;
		public const float xMinWideView = 4f;
		public const float xMinNormalView = 14;
		public const float xMaxDistanceFromEdgeNoScrollbar = 5;

		public static void VSeparator (
			float space = 0
		)
		{
			space = (space - spacerWidth) / 2;
			GUILayout.Space (space);
			GUILayout.Label ("", EditorStyles.textField, 
				GUILayout.Width (spacerWidth), 
				GUILayout.ExpandWidth (false),
				GUILayout.ExpandHeight (true));			
			GUILayout.Space (space);

		}

		public static void HSeparator (
			float space = 0
		)
		{
			space = (space - spacerWidth) / 2;
			GUILayout.Space (space);
			GUILayout.Label ("", EditorStyles.textField, 
				GUILayout.Height (spacerWidth), 
				GUILayout.ExpandWidth (true),
				GUILayout.ExpandHeight (false));			
			GUILayout.Space (space);
		}

		#endregion

		#region HorizontalScope - Disposable

		public class HorizontalScope : GUI.Scope
		{
			GUILayout.HorizontalScope hs;

			public HorizontalScope ()
			{
				hs = new GUILayout.HorizontalScope ();
			}

			protected override void CloseScope ()
			{
				// DEBUG
				//				if (XoxEditorScopeAccountant.emergency)
				//					Debug.Log ("Closing");

				hs.Dispose ();
			}
		}

		#endregion

		#region AreaScope - Disposable

		public class AreaScope : GUI.Scope
		{
			GUILayout.AreaScope ars;

			public AreaScope (
				Rect rect
			)
			{
				ars = new GUILayout.AreaScope (rect);
			}

			protected override void CloseScope ()
			{
				// DEBUG
				//				if (XoxEditorScopeAccountant.emergency)
				//					Debug.Log ("Closing");

				ars.Dispose ();
			}
		}

		#endregion

		#region EditorScrollViewScope - Disposable

		public class EditorScrollViewScope : GUI.Scope
		{
			EditorGUILayout.ScrollViewScope svs;

			public EditorScrollViewScope (
				Vector2 sp
			)
			{
				svs = new EditorGUILayout.ScrollViewScope (sp);
//				Debug.Log (XoxEditorScopeAccountant.ToMyString ());

			}

			protected override void CloseScope ()
			{
				// DEBUG
				//				if (XoxEditorScopeAccountant.emergency)
				//					Debug.Log ("Closing");

				svs.Dispose ();
			}

			public Vector2 scrollPosition { get { return svs.scrollPosition; } }
		}

		#endregion

		#region HorizontalScopeWithLineBreak

		public class HorizontalScopeWithLineBreak
		{
			readonly GUIContent label;
			readonly GUIContent separator;
			readonly GUIStyle scopeStyle;
			readonly GUIStyle itemStyle;

			readonly float separatorWidth;
			readonly float labelWidth;

			float currentXMax;
			const int indent = 15;

			public HorizontalScopeWithLineBreak (
				string label,
				string separator,
				GUIStyle scopeStyle,
				GUIStyle itemStyle
			)
			{
				this.itemStyle = itemStyle;
				this.scopeStyle = scopeStyle;
				this.separator = new GUIContent (separator);
				this.label = new GUIContent (label);

				separatorWidth = itemStyle.CalcSize (this.separator).x;
				labelWidth = itemStyle.CalcSize (this.label).x;
			}

			public void Draw (
				List<Object> list,
				float aMaxX
			)
			{
				if ( list.Count == 0 ) {
					// nothing to do
					return;
				}
				float maxX = aMaxX - scopeStyle.padding.horizontal;
				
				using ( new EditorGUILayout.HorizontalScope (scopeStyle) ) {
					Object obj;

					// draw the label
					GUILayout.Label (label, itemStyle);
					// and the first item
					obj = list[0];
					DrawButton (obj);
					currentXMax = labelWidth + GetNeededSpace (obj);

					for ( int i = 1 ; i < list.Count ; i++ ) {
						obj = list[i];

						var ns = GetNeededSpace (obj);

						if ( currentXMax + ns > maxX ) {
							GUILayout.EndHorizontal ();
							GUILayout.BeginHorizontal (scopeStyle);
							GUILayout.Space (indent);
							currentXMax = indent;
						}

						GUILayout.Label (separator, itemStyle);
						DrawButton (obj);
						currentXMax += ns;
					}
				}			
			}

			public void DrawSeparatorKeptWithPredecessor (
				List<Object> list,
				float aMaxX
			)
			{
				if ( list.Count == 0 ) {
					// nothing to do
					return;
				}
				float maxX = aMaxX - scopeStyle.padding.horizontal;

				using ( new EditorGUILayout.HorizontalScope (scopeStyle) ) {
					Object obj;

					// draw the label
					GUILayout.Label (label, itemStyle);
					// and the first item
					obj = list[0];
					DrawButton (obj);
					currentXMax = labelWidth + GetNeededSpace (obj);

					for ( int i = 1 ; i < list.Count ; i++ ) {
						obj = list[i];

						var ns = GetNeededSpace (obj);

						// This is not really 100% correctly implemented in terms of calculated widths,..
						// but as separators are not that wide, we shall not care atm
						GUILayout.Label (separator, itemStyle);

						if ( currentXMax + ns > maxX ) {
							GUILayout.EndHorizontal ();
							GUILayout.BeginHorizontal (scopeStyle);
							GUILayout.Space (indent);
							currentXMax = indent;
						}

						DrawButton (obj);
						currentXMax += ns;
					}
				}			
			}

			float GetNeededSpace (
				Object obj
			)
			{
				return itemStyle.CalcSize (new GUIContent (obj.name)).x + separatorWidth;
			}

			void DrawButton (
				Object obj
			)
			{
				if ( GUILayout.Button (obj.name, itemStyle) ) {
					EditorGUIUtility.PingObject (obj);
					Selection.activeObject = obj;
				}
				EditorGUIUtility.AddCursorRect (GUILayoutUtility.GetLastRect (), MouseCursor.Link);
			}
		}

		#endregion


		#region WidthTester

		public class WidthTester
		{
			// currentRect values are "inofficial" until they are the same as the
			// lastRect values - so we have to force Repaints until the situation
			// setttles
			// Values are only updated in Repaint cycles as they are the only runs,
			// which deliver right data
			// If in a layout or event run currentRect and lastRect differ,
			// a new Repaint is requested
			// Update has to be called at the end of the draw cycle, so that there is
			// no change between layout and Repaint
			// width and xMin is the stuff we are after and should be used where needed.
			Rect currentRect;
			Rect lastRect;
			readonly Editor aEditor;



			public float width {
				get { return currentRect.width; }
			}

			public float xMin {
				get { return currentRect.xMin; }
			}

			public float xMax {
				get { return currentRect.xMax; }
			}

			public WidthTester (
				Editor aEditor
			)
			{
				this.aEditor = aEditor;
				lastRect = new Rect (0, 0, 1, 1);
				currentRect = new Rect (-100, -100, 1, 1);
			}

			public void Update (
				bool wideView
			)
			{
				// Request a repaint, if we cant be sure at the last Repaint has
				// been done with stable values
				if ( Event.current.type != EventType.Repaint ) {
					if ( !currentRect.Equals (lastRect) ) {
						aEditor.Repaint ();
					}
				}
				// we have to draw, even if we dont use it,.. layout needs it (no changes btw
				// Layout and Repaint)
				using ( new EditorGUILayout.HorizontalScope (GUIStyle.none, GUILayout.Height (0f)) ) {
					GUILayout.FlexibleSpace ();
				}

				// just over the data handling we may decide, young padawan
				// only update the data, if it is the Repaint cycle
				if ( Event.current.type != EventType.Repaint ) {
					return;
				}

				lastRect = currentRect;
				currentRect = GUILayoutUtility.GetLastRect ();

				if ( wideView ) {
					currentRect.xMin = xMinWideView;
				}
				// Request a repaint, if we cant be sure at the last Repaint has
				// been done with stable values
				if ( !currentRect.Equals (lastRect) ) {
					aEditor.Repaint ();
				}
			}
		}

		#endregion

	}
}
