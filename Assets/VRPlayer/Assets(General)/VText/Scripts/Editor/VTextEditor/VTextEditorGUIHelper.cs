// ----------------------------------------------------------------------
// File: 		VTextEditorGUIStyles
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

/// <summary>
/// here we define the different GUIStyles for the VText editor
/// </summary>
public static class VTextEditorGUIHelper 
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS 
    /// <summary>
    /// the width of the help button
    /// </summary>
    public const float HELP_BUTTON_WIDTH = 20;

    /// <summary>
    /// the height of the help button
    /// </summary>
    public const float HELP_BUTTON_HEIGHT = 18;
	#endregion // CONSTANTS


	#region FIELDS
    private static GUIStyle _helpTextStyle = new GUIStyle();                // the text style for the help texts (white or black + wordwrap + richtext)

    private static GUIStyle _toggleButtonStyleNormal = "Button";            // the style of a toggle button (which is an ordninary push button) if it is NOT toggled
    private static GUIStyle _toggleButtonStyleToggled = null;               // the style of a toggle button (which is an ordninary push button) if it is toggled
	#endregion // FIELDS


	#region PROPERTIES
    public static GUIStyle HelpTextStyle {
        get { 
            if (EditorGUIUtility.isProSkin) {
                _helpTextStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            } else {
                _helpTextStyle.normal.textColor = Color.black;
            }

            _helpTextStyle.wordWrap = true;
            _helpTextStyle.richText = true;
            return _helpTextStyle;
        }
    }

    public static GUIStyle ToggleButtonStyleNormal {
        get {                         
            return _toggleButtonStyleNormal;
        }
    }

    public static GUIStyle ToggleButtonStyleToggled {
        get { 
            if (_toggleButtonStyleToggled == null) {
                _toggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);

                Color textColor = Color.white;
                Color backgroundColor = new Color().HexToColor("16576d");

                if (EditorGUIUtility.isProSkin) {
                    textColor = new Color().HexToColor("cccccc");
                    backgroundColor = new Color().HexToColor("0d3441");
                }

                Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, backgroundColor);
                tex.Apply();
                _toggleButtonStyleToggled.normal.background = tex;

                _toggleButtonStyleToggled.normal.textColor = textColor;

            }
            return _toggleButtonStyleToggled;
        }
    }

	#endregion // PROPERTIES


	#region CONSTRUCTORS

	#endregion // CONSTRUCTORS


	#region METHODS
    /// <summary>
    /// convert a hex string (without the "#") to a color32 fully opaque
    /// </summary>
    /// <returns>The to color.</returns>
    /// <param name="c">C.</param>
    /// <param name="hex">Hex.</param>
    public static Color HexToColor(this Color c, string hex) {
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        return new Color32 (r,g,b, 255);
    }

    /// <summary>
    /// draws an image with a border which is colored depending on the UnityEditor skin (personal (light) or professional (dark))
    /// draws the image with the specified width and calculates its height to keep the correct aspect ratio
    /// </summary>
    /// <param name="image">Image.</param>
    /// <param name="width">Width.</param>
    public static void DrawBorderedImage(Texture image, float width) {
        Color c = GUI.backgroundColor;
        if (EditorGUIUtility.isProSkin) {
            GUI.backgroundColor = new Color().HexToColor("ffffff");
        } else {
            GUI.backgroundColor = new Color().HexToColor("ffffff");
        }

        EditorGUILayout.BeginVertical("box");
        float aspectRatio = (float) image.height / image.width;
        EditorGUILayout.LabelField(new GUIContent(image), GUILayout.Width(width), GUILayout.Height(width * aspectRatio));
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = c;
    }

    /// <summary>
    /// Draws the help button and toggles the specified help window variable
    /// </summary>
    /// <param name="helpWindow">Help window.</param>
    public static void DrawHelpButton(ref AnimBool showHelpWindowVariable) {        
        Color c = GUI.backgroundColor;
        GUI.backgroundColor = new Color().HexToColor("34ccff");
        if (GUILayout.Button(new GUIContent("?", "Show or hide the help window"), GUILayout.Width(HELP_BUTTON_WIDTH), GUILayout.Height(HELP_BUTTON_HEIGHT))) {
            showHelpWindowVariable.target = !showHelpWindowVariable.target;
        }
        GUI.backgroundColor = c;
    }

    /// <summary>
    /// Convert the string into an bold and colored string ... depending on the used UnityEditor skin (personal (light) or professional (dark))
    /// used for the headers in the help windows
    /// </summary>
    /// <returns>The help window header string.</returns>
    /// <param name="text">Text.</param>
    public static string ConvertStringToHelpWindowHeader(string text) {
        string color = "#34ccff";
        if (!EditorGUIUtility.isProSkin) {
            color = "#2592b7";
        }

        text = string.Format("<color={0}><b>{1}</b></color>", color, text);
        return text;
    }

    /// <summary>
    /// Convert the string into an bold and colored string ... depending on the used UnityEditor skin (personal (light) or professional (dark))
    /// used for the category links (which are not links at all) in the help windows
    /// </summary>
    /// <returns>The colorized string.</returns>
    /// <param name="text">Text.</param>
    public static string ConvertStringToHelpWindowCategoryLink(string text) {
        string color = "#ffcc34";
        if (!EditorGUIUtility.isProSkin) {
            color = "#d6a513";
        }

        text = string.Format("<color={0}><b>{1}</b></color>", color, text);
        return text;
    }

    /// <summary>
    /// Convert the string into an bold and colored string ... depending on the used UnityEditor skin (personal (light) or professional (dark))
    /// used for warnings in the help windows
    /// </summary>
    /// <returns>The colorized string.</returns>
    /// <param name="text">Text.</param>
    public static string ConvertStringToHelpWindowWarning(string text) {
        string color = "#ffcc34";
        if (!EditorGUIUtility.isProSkin) {
            color = "#d6a513";
        }

        text = string.Format("<color={0}><b>{1}</b></color>", color, text);
        return text;
    }

    /// <summary>
    /// Convert the string into an bold and colored string ... depending on the used UnityEditor skin (personal (light) or professional (dark))
    /// used for the category links (which are not links at all) in the help windows
    /// </summary>
    /// <returns>The help window header string.</returns>
    /// <param name="text">Text.</param>
    public static string ConvertStringToHelpWindowListItem(string text) {
        string color = "#34ccff";
        if (!EditorGUIUtility.isProSkin) {
            color = "#2592b7";
        }

        text = string.Format("<color={0}><b>{1}</b></color>", color, text);
        return text;
    }
	#endregion // METHODS


	#region EVENT HANDLERS

	#endregion // EVENT HANDLERS
}
