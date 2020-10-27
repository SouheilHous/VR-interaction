using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace dlobo.Seek
{
	public class SeekSettingsWindow : EditorWindow
	{
		private SeekSavedData data;

		private GlobalConfig globalConfig {
			get { return data.globalConfig; }
		}

		private Color fileNameColor;
		private Color fileTypeColor;
		private Color fileSizeColor;
		private Color fileCreationTimeColor;
		private Color fileLastWriteTimeColor;
		private Color guidColor;

		private GUIStyle _textStyle;
		private GUIStyle textStyle {
			get {
				if (_textStyle == null) {
					_textStyle = new GUIStyle(GUI.skin.label);
					_textStyle.wordWrap = true;
					_textStyle.richText = true;
				}
				return _textStyle;
			}
		}

		[MenuItem("Window/Seek/Settings", priority = 2)]
		public static void OpenSeekSettingsWindow()
		{
			EditorWindow.GetWindow<SeekSettingsWindow>("Seek Settings");
		}

		void OnEnable()
		{
			var script = MonoScript.FromScriptableObject(this);
			string seekFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));

			string dataFilePath = seekFolder + "/Seek Config.asset";
			data = SeekSavedData.Load(dataFilePath);

			tryParseHexToColor(globalConfig.fileNameColor, out fileNameColor);
			tryParseHexToColor(globalConfig.fileTypeColor, out fileTypeColor);
			tryParseHexToColor(globalConfig.fileSizeColor, out fileSizeColor);
			tryParseHexToColor(globalConfig.fileCreationTimeColor, out fileCreationTimeColor);
			tryParseHexToColor(globalConfig.fileLastWriteTimeColor, out fileLastWriteTimeColor);
			tryParseHexToColor(globalConfig.guidColor, out guidColor);
		}

		void OnGUI()
		{
			// ---------------------------------------------------------------------

			GUILayout.Space(5);

			EditorGUILayout.BeginHorizontal();

			GUILayout.Label("<b>Collapsed sections</b> (when clicking the header)", textStyle);

			GUILayout.Space(8);
			if (GUILayout.Button("R", GUILayout.MaxWidth(20)))
			{
				globalConfig.doCollapseSearchType = true;
				globalConfig.doCollapseAssetTypes = true;
				globalConfig.doCollapseMatchingType = true;
				globalConfig.doCollapseSortType = true;
				globalConfig.doCollapseDisplayOptions = true;
				globalConfig.doCollapseSpecialFilters = true;
				globalConfig.doCollapseSavedSearches = true;

				if (SeekWindow.Instance != null) {
					SeekWindow.Instance.DoRepaint();
				}
			}

			EditorGUILayout.EndHorizontal();

			// The binary ORs (|) force all fields to be drawn even if one has changed. If we used || instead, Unity would complain about layout mismatches.
			if (drawCheckbox("Search using",    ref globalConfig.doCollapseSearchType)
			  | drawCheckbox("Asset types",     ref globalConfig.doCollapseAssetTypes)
			  | drawCheckbox("Matching type",   ref globalConfig.doCollapseMatchingType)
			  | drawCheckbox("Sort by",         ref globalConfig.doCollapseSortType)
			  | drawCheckbox("Display options", ref globalConfig.doCollapseDisplayOptions)
			  | drawCheckbox("Special filters", ref globalConfig.doCollapseSpecialFilters)
			  | drawCheckbox("Saved searches",  ref globalConfig.doCollapseSavedSearches)
			) {
				if (SeekWindow.Instance != null) {
					SeekWindow.Instance.DoRepaint();
				}
			}

			// ---------------------------------------------------------------------

			GUILayout.Space(5);
			GUILayout.Label("<b>Text colors</b>", textStyle);

			// The binary ORs (|) force all fields to be drawn even if one has changed. If we used || instead, Unity would complain about layout mismatches.
			if (drawColorField("Name",            ref globalConfig.fileNameColor,          ref fileNameColor,          GlobalConfig.default_fileNameColor)
			  | drawColorField("Creation time",   ref globalConfig.fileCreationTimeColor,  ref fileCreationTimeColor,  GlobalConfig.default_fileCreationTimeColor)
			  | drawColorField("Last write time", ref globalConfig.fileLastWriteTimeColor, ref fileLastWriteTimeColor, GlobalConfig.default_fileLastWriteTimeColor)
			  | drawColorField("Type",            ref globalConfig.fileTypeColor,          ref fileTypeColor,          GlobalConfig.default_fileTypeColor)
			  | drawColorField("Size",            ref globalConfig.fileSizeColor,          ref fileSizeColor,          GlobalConfig.default_fileSizeColor)
			  | drawColorField("GUID",                 ref globalConfig.guidColor,              ref guidColor,              GlobalConfig.default_guidColor)
			) {
				if (SeekWindow.Instance != null) {
					SeekWindow.Instance.UpdateRepresentations();
				}
			}

			// ---------------------------------------------------------------------

			GUILayout.Space(5);
			GUILayout.Label("<b>Other</b>", textStyle);

			drawFloatField("Delay for slow searches", ref globalConfig.delayForSlowSearch, GlobalConfig.default_searchDelayForSlowOptions);

			if (globalConfig.delayForSlowSearch <= 0.1f) {
				EditorGUILayout.LabelField("<color=yellow>Warning:</color> This value is only for slow searches and it is strongly recommended to keep it above your typing speed, otherwise each character will cause a (slow) search before you can type the next one.", textStyle);
			}

			// ---------------------------------------------------------------------
		}

		private bool drawColorField(string label, ref string hexString, ref Color color, string defaultValue)
		{
			bool hasChanged = false;

			EditorGUILayout.BeginHorizontal();

			string newHexString = EditorGUILayout.TextField(label, hexString);
			if (newHexString != hexString) {
				tryParseHexToColor(newHexString, out color);
				hexString = newHexString;
				hasChanged = true;
			}

			Color newColor = EditorGUILayout.ColorField("", color, GUILayout.MaxWidth(90), GUILayout.MinWidth(50));
			if (newColor != color) {
				color = newColor;
				hexString = "#" + colorToHex(color);
				hasChanged = true;
			}

			GUILayout.Space(8);
			if (GUILayout.Button("R", GUILayout.MaxWidth(20))) {
				hexString = defaultValue;
				tryParseHexToColor(hexString, out color);
				GUI.FocusControl("");
				hasChanged = true;
			}

			EditorGUILayout.EndHorizontal();

			return hasChanged;
		}

		private bool drawFloatField(string label, ref float value, float defaultValue)
		{
			bool hasChanged = false;

			EditorGUILayout.BeginHorizontal();

			float newValue = EditorGUILayout.FloatField(label, value);
			if (newValue != value) {
				value = newValue;
				hasChanged = true;
			}

			GUILayout.Space(8);
			if (GUILayout.Button("R", GUILayout.MaxWidth(20))) {
				value = defaultValue;
				GUI.FocusControl("");
				hasChanged = true;
			}

			EditorGUILayout.EndHorizontal();

			return hasChanged;
		}

		private bool drawCheckbox(string label, ref bool value)
		{
			bool hasChanged = false;

			EditorGUILayout.BeginHorizontal();

			bool newValue = GUILayout.Toggle(value, label);
			if (newValue != value) {
				value = newValue;
				hasChanged = true;
			}

			EditorGUILayout.EndHorizontal();

			return hasChanged;
		}

		// mostly the same as ColorUtility.ToHtmlStringRGBA, but that only exists from Unity 5.2 on
		private static string colorToHex(Color color)
		{
			string rs = Mathf.RoundToInt(255*color.r).ToString("x2");
			string gs = Mathf.RoundToInt(255*color.g).ToString("x2");
			string bs = Mathf.RoundToInt(255*color.b).ToString("x2");

			if (color.a == 1) {
				if (rs[0] == rs[1] && gs[0] == gs[1] && bs[0] == bs[1]) {
					rs = rs.Substring(0,1);
					gs = gs.Substring(0,1);
					bs = bs.Substring(0,1);
				}
				return rs+gs+bs;
			} else {
				string al = Mathf.RoundToInt(255*color.a).ToString("x2");

				if (rs[0] == rs[1] && gs[0] == gs[1] && bs[0] == bs[1] && al[0] == al[1]) {
					rs = rs.Substring(0,1);
					gs = gs.Substring(0,1);
					bs = bs.Substring(0,1);
					al = al.Substring(0,1);
				}
				return rs+gs+bs+al;
			}
		}

		// mostly the same as ColorUtility.TryParseHtmlString, but that only exists from Unity 5.2 on
		private static bool tryParseHexToColor(string hex, out Color color)
		{
			hex = hex.ToLower();

			if (hex[0] != '#')
			{
				switch (hex)
				{
					case "aqua":
					case "cyan":      hex = "#00ffff"; break;
					case "black":     hex = "#000000"; break;
					case "blue":      hex = "#0000ff"; break;
					case "brown":     hex = "#a52a2a"; break;
					case "darkblue":  hex = "#0000a0"; break;
					case "fuchsia":
					case "magenta":   hex = "#ff00ff"; break;
					case "green":     hex = "#008000"; break;
					case "gray":
					case "grey":      hex = "#808080"; break;
					case "lightblue": hex = "#add8e6"; break;
					case "lime":      hex = "#00ff00"; break;
					case "maroon":    hex = "#800000"; break;
					case "navy":      hex = "#000080"; break;
					case "olive":     hex = "#808000"; break;
					case "orange":    hex = "#ffa500"; break;
					case "purple":    hex = "#800080"; break;
					case "red":       hex = "#ff0000"; break;
					case "silver":    hex = "#c0c0c0"; break;
					case "teal":      hex = "#008080"; break;
					case "white":     hex = "#ffffff"; break;
					case "yellow":    hex = "#ffff00"; break;

					default:
						color = Color.black;
						return false;
				}
			}

			try {
				byte r, g, b, a;

				if (hex.Length == 4) {	// #rgb
					r = Convert.ToByte(new string(hex[1], 2), 16);
					g = Convert.ToByte(new string(hex[2], 2), 16);
					b = Convert.ToByte(new string(hex[3], 2), 16);
					a = 255;
				}
				else if (hex.Length == 5) {	// #rgba
					r = Convert.ToByte(new string(hex[1], 2), 16);
					g = Convert.ToByte(new string(hex[2], 2), 16);
					b = Convert.ToByte(new string(hex[3], 2), 16);
					a = Convert.ToByte(new string(hex[4], 2), 16);
				}
				else if (hex.Length == 7) {	// #rrggbb
					r = Convert.ToByte(hex.Substring(1,2), 16);
					g = Convert.ToByte(hex.Substring(3,2), 16);
					b = Convert.ToByte(hex.Substring(5,2), 16);
					a = 255;
				}
				else if (hex.Length == 9) {	// #rrggbbaa
					r = Convert.ToByte(hex.Substring(1,2), 16);
					g = Convert.ToByte(hex.Substring(3,2), 16);
					b = Convert.ToByte(hex.Substring(5,2), 16);
					a = Convert.ToByte(hex.Substring(7,2), 16);
				}
				else {
					color = Color.black;
					return false;
				}

				color = new Color32(r,g,b,a);
				return true;
			}
			catch (System.FormatException) {
				color = Color.black;
				return false;
			}
		}
	}
}
