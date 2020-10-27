using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.Extensions;


namespace xDocEditorBase.AnnotationModule {

	public class TextAreaMenuColorButtonRow
	{
		const int numButtons = 7;
		const int numButtons2 = (numButtons + 1) / 2;
		readonly GUIStyle style;
		Rect colorButtonRowRect;
		Rect colorButtonRect;
		TextAreaMenuColor.ColorChooserCallback callback;

		public static float height {
			get {
				return AssetManager.settings.styleColorChooserButtonRow.style.fixedHeight +
				AssetManager.settings.styleColorChooserButtonRow.style.padding.top +
				AssetManager.settings.styleColorChooserButtonRow.style.padding.bottom;
			}
		}

		public static float width {
			get {
				return numButtons * AssetManager.settings.styleColorChooserButtonRow.style.fixedWidth +
				AssetManager.settings.styleColorChooserButtonRow.style.padding.left * 3 +
				AssetManager.settings.styleColorChooserButtonRow.style.padding.right;
			}
		}

		public static float WindowWidth()
		{
			return width +
			AssetManager.settings.styleColorChooserButtonRow.style.margin.left +
			AssetManager.settings.styleColorChooserButtonRow.style.margin.right;
		}

		public static float WindowHeight(
			int numRows
		)
		{
			return (height +
			AssetManager.settings.styleColorChooserButtonRow.style.margin.top +
			AssetManager.settings.styleColorChooserButtonRow.style.margin.bottom) * numRows +
			EditorGUIUtility.standardVerticalSpacing;
		}

		void CreateButton(
			Color col
		)
		{
			GUI.backgroundColor = col;
			if (GUI.Button(colorButtonRect, GUIContent.none, AssetManager.settings.styleColorChooserButton.style)) {
				if (callback != null) {
					callback(col.HexString());
				}
			}
			MoveButtonRect();
		}

		public TextAreaMenuColorButtonRow(
			Color midColor,
			TextAreaMenuColor.ColorChooserCallback callback
		)
		{
			this.callback = callback;

			style = AssetManager.settings.styleColorChooserButtonRow.style;
			// Frame
			colorButtonRowRect = GUILayoutUtility.GetRect(width, height, style);
			using (new StateSaver.GuiColor(style.normal.textColor)) {
				GUI.DrawTexture(colorButtonRowRect, EditorGUIUtility.whiteTexture);
			}

			// Buttons
			InitFirstButtonRect();
			const float lFactor = 1f / (numButtons2);

			for (int i = 1; i < numButtons2; i++) {
				using (var bgc = new StateSaver.BgColor()) {
					Color col = Color.Lerp(Color.black, midColor, i * lFactor);
					CreateButton(col);
				}
			}
			MoveButtonRectByPadding();
			using (var bgc = new StateSaver.BgColor()) {
				CreateButton(midColor);
			}
			MoveButtonRectByPadding();
			for (int i = 1; i < numButtons2; i++) {
				using (var bgc = new StateSaver.BgColor()) {
					Color col = Color.Lerp(midColor, Color.white, i * lFactor);
					CreateButton(col);
				}
			}
		}

		public TextAreaMenuColorButtonRow(
			Color startColor,
			Color endColor,
			bool capEndColor,
			TextAreaMenuColor.ColorChooserCallback callback
		)
		{
			this.callback = callback;

			style = AssetManager.settings.styleColorChooserButtonRow.style;
			// Frame
			colorButtonRowRect = GUILayoutUtility.GetRect(width, height, style);
			using (new StateSaver.GuiColor(style.normal.textColor)) {
				GUI.DrawTexture(colorButtonRowRect, EditorGUIUtility.whiteTexture);
			}

			// Buttons
			InitFirstButtonRect();
			float lFactor;
			if (capEndColor) {
				lFactor = 1f / (numButtons);
			} else {
				lFactor = 1f / (numButtons - 1f);
			}
			for (int i = 0; i < numButtons; i++) {
				using (var bgc = new StateSaver.BgColor()) {
					Color col = Color.Lerp(startColor, endColor, i * lFactor);
					if (i == numButtons2 - 1) {
						MoveButtonRectByPadding();
						CreateButton(col);
						MoveButtonRectByPadding();
					} else {
						CreateButton(col);
					}
				}
			}
		}

		void InitFirstButtonRect()
		{
			colorButtonRect = new Rect(colorButtonRowRect);
			colorButtonRect.x += style.padding.left;
			colorButtonRect.y += style.padding.top;
			colorButtonRect.width = style.fixedWidth;
			colorButtonRect.height = style.fixedHeight;
		}

		void MoveButtonRect()
		{
			colorButtonRect.x += style.fixedWidth;
		}

		void MoveButtonRectByPadding()
		{
			colorButtonRect.x += style.padding.left;
		}

	}

	public class TextAreaMenuColorWindow : EditorWindow
	{
		static Color bgColor;

		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(TextAreaMenuColorWindow), true);
		}

		public static float width {
			get {
				return TextAreaMenuColorButtonRow.WindowWidth();
			}
		}

		public static float height {
			get {
				return TextAreaMenuColorButtonRow.WindowHeight(7);
			}
		}

		TextAreaMenuColor.ColorChooserCallback callback;

		public void SetCallback(
			TextAreaMenuColor.ColorChooserCallback callback
		)
		{
			this.callback = callback;
		}

		void OnEnable()
		{
			// we want to be able to spawn Repaint request, so that the OnHover bg is shown as 
			// promptly as possible
			wantsMouseMove = true;
			bgColor = new Color(.9f, .9f, .9f);
		}

		void OnGUI()
		{
			switch (Event.current.type) {
			case EventType.MouseMove:
				Repaint();
				break;
			}

			// just for a whiter bg of the windows
			using (new StateSaver.BgColor(bgColor))
				GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), AssetManager.settings.whitePixel, ScaleMode.StretchToFill);

			// Analysis disable once ObjectCreationAsStatement
			new TextAreaMenuColorButtonRow(Color.black, Color.white, false, callback);
			// Analysis disable once ObjectCreationAsStatement
			new TextAreaMenuColorButtonRow(Color.green, callback);
			// Analysis disable once ObjectCreationAsStatement
			new TextAreaMenuColorButtonRow(Color.blue, callback);
			// Analysis disable once ObjectCreationAsStatement
			new TextAreaMenuColorButtonRow(Color.red, callback);
			// Analysis disable once ObjectCreationAsStatement
			new TextAreaMenuColorButtonRow(Color.yellow, callback);
			// Analysis disable once ObjectCreationAsStatement
			new TextAreaMenuColorButtonRow(Color.magenta, callback);
			// Analysis disable once ObjectCreationAsStatement
			new TextAreaMenuColorButtonRow(new Color(1, 165f / 255f, 0), callback); // orange
		}

	}

	public class TextAreaMenuColor : Inspectorbar.ButtonBase
	{
		readonly XDocAnnotationEditorBase aData;
		TextAreaMenuColorWindow colorChooserWindow;

		public delegate void ColorChooserCallback(string color);

		public TextAreaMenuColor(
			XDocAnnotationEditorBase aData
		)
			: base(
				"<Color>",
				AssetManager.settings.styleToolbarDropDown.style
			)
		{
			this.aData = aData;
		}

		protected override void ButtonAction()
		{
			colorChooserWindow = ScriptableObject.CreateInstance<TextAreaMenuColorWindow>();
			colorChooserWindow.SetCallback(SetColor);
			var screenXY = GUIUtility.GUIToScreenPoint(new Vector2(currentPosition.x, currentPosition.y));
			var screenRect = new Rect(screenXY, new Vector2(currentPosition.width, currentPosition.height));
			colorChooserWindow.ShowAsDropDown(
				screenRect,
				new Vector2(
					TextAreaMenuColorWindow.width,
					TextAreaMenuColorWindow.height));
		}

		void SetColor(
			string col
		)
		{
			aData.annotationInspectorTextArea.AddTags("color", "=" + col);
			colorChooserWindow.Close();
		}
	}
}

