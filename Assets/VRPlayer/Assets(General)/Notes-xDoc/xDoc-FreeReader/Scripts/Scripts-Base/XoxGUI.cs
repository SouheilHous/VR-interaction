using UnityEditor;
using UnityEngine;


namespace xDocBase.UI
{

	public static  class XoxGUI
	{
		public class HandlesScope: GUI.Scope
		{
			public HandlesScope ()
			{
				Handles.BeginGUI ();
			}

			protected override void CloseScope ()
			{
				Handles.EndGUI ();			
			}
		}

		public static Rect RemoveMargins (
			Rect rect,
			GUIStyle style
		)
		{
			rect.xMin += style.margin.left;
			rect.xMax -= style.margin.right;
			rect.yMin += style.margin.top;
			rect.yMax -= style.margin.bottom;
			return rect;
		}

		public static Rect[] SplitVertically (
			Rect rect,
			GUIStyle style,
			float height
		)
		{
			Rect[] rects = new Rect[2];
			rects[0] = new Rect (rect);
			rects[0].height = height;
			rects[1] = new Rect (rect);
			rects[1].yMin += height + style.margin.top + style.margin.bottom;
			return rects;
		}

		public static Rect[] SplitHorizontally (
			Rect rect,
			GUIStyle style,
			float width
		)
		{
			Rect[] rects = new Rect[2];
			rects[0] = new Rect (rect);
			rects[0].width = width;
			rects[1] = new Rect (rect);
			rects[1].xMin += width + style.margin.horizontal;
			return rects;
		}

		public static Rect[] SplitHorizontally (
			Rect rect,
			GUIStyle style
		)
		{
			float w = Mathf.Floor ((rect.width - style.margin.left - style.margin.right) / 2);
			Rect[] rects = new Rect[2];
			rects[0] = new Rect (rect);
			rects[0].width = w;
			rects[1] = new Rect (rect);
			rects[1].xMin += rect.width - w;
			return rects;
		}

	}
}