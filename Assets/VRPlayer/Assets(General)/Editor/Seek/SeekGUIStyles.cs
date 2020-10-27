using UnityEngine;

namespace dlobo.Seek
{
	public class GUIStyles
	{
		// unique styles
		public GUIStyle resultButtonStyle;
		public GUIStyle resultIconStyle;
		public GUIStyle resultsScrollViewStyle;
		public GUIStyle savedSearchButtonStyle;
		public GUIStyle headerStyle;

		private GUIStyle _separatorStyle;
		public GUIStyle separatorStyle {
			get {
				if (_separatorStyle == null) {
					_separatorStyle = new GUIStyle(GUI.skin.box);
					_separatorStyle.border.top = 0;
					_separatorStyle.border.bottom = 1;
				}
				return _separatorStyle;
			}
		}

		private GUIStyle _messageLabelStyle;
		public GUIStyle messageLabelStyle {
			get {
				if (_messageLabelStyle == null) {
					_messageLabelStyle = new GUIStyle(GUI.skin.label);
					_messageLabelStyle.wordWrap = true;
					_messageLabelStyle.richText = true;
					_messageLabelStyle.padding = new RectOffset(3, 3, 4, 0);
				}
				return _messageLabelStyle;
			}
		}

		private GUIStyle _errorLabelStyle;
		public GUIStyle errorLabelStyle {
			get {
				if (_errorLabelStyle == null) {
					_errorLabelStyle = new GUIStyle(GUI.skin.label);
					_errorLabelStyle.wordWrap = true;
					_errorLabelStyle.richText = true;
					// _errorLabelStyle.fontStyle = FontStyle.Bold;
					_errorLabelStyle.padding = new RectOffset(5, 5, 8, 8);
				}
				return _errorLabelStyle;
			}
		}

		public GUIStyles(GUISkin skin)
		{
			if (skin == null) {
				return;
			}
			resultButtonStyle = skin.GetStyle("resultButton");
			resultIconStyle = skin.GetStyle("resultIcon");
			resultsScrollViewStyle = skin.GetStyle("resultsScrollView");
			savedSearchButtonStyle = skin.GetStyle("savedSearchButton");
			headerStyle = skin.GetStyle("header");
		}
	}
}
