using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocEditorBase.Inspectorbar {

	public abstract class ButtonBase
	{
		protected string title;
		protected int titleLen;
		protected GUIStyle style;
		protected Vector2 size;
		protected Rect currentPosition;
		protected float widthLimit;

		protected ButtonBase(
			string title,
			GUIStyle style
		)
		{
			this.style = style;
			SetNewTitle(title);
		}

		public void SetNewTitle(
			string title
		)
		{
			this.title = title;
			titleLen = title.Length;
			size = style.CalcSize(new GUIContent(title));
		}

		/// <summary>
		/// Draws the button, but does not care about the provided position.width.
		/// It will take the needed space and reposition the provided rect, so that
		/// the next element can be placed to the right next to it.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="scaleFactor">Scale factor.</param>
		public void Draw(
			ref Rect position,
			float scaleFactor
		)
		{
			// This is the width we can use
			position.width = limitedWidth * scaleFactor;

			currentPosition = position;
			if (GUI.Button(currentPosition, title, style)) {
				ButtonAction();	
			}
			position.x += position.width;
		}

		protected abstract void ButtonAction();

		public float LimitWidthBy(
			float deltaWidth,
			float minWidth
		)
		{
			minWidth = Mathf.Min(minWidth, size.x);
			widthLimit = Mathf.Max(size.x - deltaWidth, minWidth);
			return size.x - widthLimit;
		}

		public float baseWidth {
			get { return size.x; }
		}

		public float limitedWidth {
			get { 
				return widthLimit == 0 ? size.x : Mathf.Min(size.x, widthLimit);
			}
		}
	}

	public class Bumper
	{
		protected GUIStyle style;
		protected Vector2 size;
		protected float	width;

		public Bumper(
			int width
		)
		{
			this.width = width;
			if (AssetManager.isFunctional) {
				style = AssetManager.settings.styleToolbar.style;
			} else {
				style = new GUIStyle();
			}
			size = style.CalcSize(new GUIContent("M"));
			size.x = width;
		}

		public void Draw(
			ref Rect position
		)
		{
			position.width = size.x;
			position.height = size.y;
			GUI.Box(position, GUIContent.none, style);
			position.x += position.width;
		}

		public float baseWidth {
			get { return size.x; }
		}
	}

	public class Filler
	{
		protected GUIStyle style;

		public Filler()
		{
			style = AssetManager.settings.styleToolbar.style;
		}

		public void Draw(
			Rect position,
			Rect barPosition
		)
		{
			barPosition.xMin = position.x + 1;
			GUI.Box(barPosition, GUIContent.none, style);
		}
	}

}