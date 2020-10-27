// ----------------------------------------------------------------------
// File: 		VTextEditorStyle
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.AnimatedValues;

/// <summary>
/// this class handles all editor stuff for the style of the text (font, materials, etc)
/// </summary>
public class VTextEditorStyle : AbstractVTextEditorComponent
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS
    /// <summary>
    /// the width of the labels in front of controls
    /// </summary>
    private const float LABEL_WIDTH = 100;

    /// <summary>
    /// the width of images in the help sections
    /// </summary>
    private const float HELP_IMAGE_WIDTH = 50.0f;

    /// <summary>
    /// the height of the scroll view which shows the help text (in the help regions)
    /// </summary>
    private const float HELP_SCROLLVIEW_HEIGHT = 60.0f;
	#endregion // CONSTANTS


	#region FIELDS
    SerializedProperty _style;                      // the style parameter component of the vtext object (materials etc)
    SerializedProperty _meshParam;                  // the mesh parameter component of the vtext object
    SerializedProperty _layout;                     // the layout parameter component of the vtext object

    SerializedProperty _pFontname;                  // the font name of the mesh
    SerializedProperty _lSize;                      // the size of the text

    VTextInterface _vtextInterface;                 // the reference to the used vtext interface

    private string[] _fontNames;                    // all available font names (including "none" at index 0)
    private int _currentFontIndex;                  // the current selected font index

    #region INFOFIELDS
    private AnimBool _showFontInfo = new AnimBool();                        // show or hide the font info
    private AnimBool _showSizeInfo = new AnimBool();                        // show or hide the size info
    private AnimBool _showFaceMaterialInfo = new AnimBool();                // show or hide the face material info
    private AnimBool _showSideMaterialInfo = new AnimBool();                // show or hide the side material info
    private AnimBool _showBevelMaterialInfo = new AnimBool();               // show or hide the bevel material info

    private Texture _fontInfoHelpImage;             // the image which is shown in the font info help box
    private Texture _sizeInfoHelpImage;                                     // the image which is shown in the size help box
    private Texture _faceMaterialHelpImage;         // the image which is shown in the face material help box
    private Texture _sideMaterialHelpImage;         // the image which is shown in the side material help box
    private Texture _bevelMaterialHelpImage;        // the image which is shown in the bevel material help box

    Vector2 _fontInfoHelpTextScrollPosition = Vector2.zero;                 // the scrollview position for the font help text
    Vector2 _sizeInfoHelpTextScrollPosition = Vector2.zero;                 // the scrollview position for the size help text
    Vector2 _faceMaterialHelpTextScrollPosition = Vector2.zero;             // the scrollview position for the face material help text
    Vector2 _sideMaterialHelpTextScrollPosition = Vector2.zero;             // the scrollview position for the side material help text
    Vector2 _bevelMaterialHelpTextScrollPosition = Vector2.zero;            // the scrollview position for the bevel material help text

    #endregion // INFOFIELDS

	#endregion // FIELDS


	#region PROPERTIES

	#endregion // PROPERTIES


	#region CONSTRUCTORS

    public VTextEditorStyle(SerializedObject obj, Editor currentEditor) 
	{
        _vtextInterface = obj.targetObject as VTextInterface;

        _style = obj.FindProperty("materials");
        _meshParam = obj.FindProperty("parameter");
        _layout = obj.FindProperty("layout");
        _lSize = _layout.FindPropertyRelative("m_size");

        _pFontname = _meshParam.FindPropertyRelative("m_fontname");

        // the images in the help screens
        _fontInfoHelpImage = Resources.Load("Images/Icons/Help/FontImage") as Texture;
        _sizeInfoHelpImage = Resources.Load("Images/Icons/Help/text_fontSize") as Texture;
        _faceMaterialHelpImage = Resources.Load("Images/Icons/Help/Letter_T_3D_FaceMaterial") as Texture;
        _bevelMaterialHelpImage = Resources.Load("Images/Icons/Help/Letter_T_3D_BevelMaterial") as Texture;
        _sideMaterialHelpImage = Resources.Load("Images/Icons/Help/Letter_T_3D_SideMaterial") as Texture;

        // add repaints if the animated values are changed
        _showFontInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showSizeInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showFaceMaterialInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showSideMaterialInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showBevelMaterialInfo.valueChanged.AddListener(currentEditor.Repaint);

	}

	#endregion // CONSTRUCTORS


	#region METHODS
    /// <summary>
    /// Setups the default font.
    /// </summary>
    public void SetupDefaultFont() {
        FillFonts(_vtextInterface.parameter.Fontname);
        if (_fontNames.Length > 1) { 
            _currentFontIndex = 1;
            _vtextInterface.parameter.Fontname = _fontNames[_currentFontIndex];
            _pFontname.stringValue = _fontNames[_currentFontIndex];
            _vtextInterface.CheckRebuild(true, false, false, false);
        }
    }

    /// <summary>
    /// draw the ui for this component
    /// 
    /// returns true if this aspect of the VText should be updated (mesh, layout, physics, etc)
    /// </summary>
    public override bool DrawUI()
    {
        bool updateMesh = false;

        EditorGUIUtility.labelWidth = LABEL_WIDTH;

        #region FONT
        EditorGUILayout.BeginHorizontal ();
        FillFonts(_vtextInterface.parameter.Fontname);
        int fc = EditorGUILayout.Popup("Font:", _currentFontIndex, _fontNames);

        if(fc != _currentFontIndex) {
            // Debug.Log("fontChoice " + fc);
            _currentFontIndex = fc;
            if(fc > 0) {
                _vtextInterface.parameter.Fontname = _fontNames[fc];
                _pFontname.stringValue = _fontNames[fc];
            } else {
                _vtextInterface.parameter.Fontname = string.Empty;
                _pFontname.stringValue = string.Empty;
            }
            updateMesh = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showFontInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showFontInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Font:") + "\n\n" +
                "Here you can select the font which is used to generate the 3D Text.\n" +
                "You can simply add additional fonts (TrueType or OTF fonts) by copying it into the" +
                "<b><i>Assets/StreamingAssets/Fonts</i></b> folder.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowWarning("Be careful") + " when deleting or renaming fonts " +
                    "in this folder. If the fonts you are deleting or renaming are in use then the corresponding VText objects set their " +
                    "<b>Font</b> parameter to <i>'None'</i>. This will result in a not visible text the next time you rebuild the VText object (for " +
                    "instance at the start of the game).";
            
            DrawHelpWindow(_fontInfoHelpImage, txt, ref _fontInfoHelpTextScrollPosition, ref _showFontInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // FONT

        #region SIZE
        GUILayout.BeginHorizontal();
        float nSize = EditorGUILayout.FloatField (new GUIContent("Size:", "The size of the text"), _lSize.floatValue);
        if (nSize != _lSize.floatValue) {
            _lSize.floatValue = nSize;
            updateMesh = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showSizeInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showSizeInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Size:") + "\n\n" +
                "This parameter defines the size of the generated text in meters.";

            DrawHelpWindow(_sizeInfoHelpImage, txt, ref _sizeInfoHelpTextScrollPosition, ref _showSizeInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // SIZE

        #region MATERIALS
        // face material
        GUILayout.BeginHorizontal();
        Object faceMat = _style.GetArrayElementAtIndex(0).objectReferenceValue;
        Object nFaceMat = EditorGUILayout.ObjectField("Face material:", faceMat, typeof(Material), false);
        if(nFaceMat != faceMat) {
            _style.GetArrayElementAtIndex(0).objectReferenceValue = nFaceMat;
            updateMesh = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showFaceMaterialInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showFaceMaterialInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Face material:") + "\n\n" +
                "The material here is used for the front- and (if defined) backfaces of the generated 3D text.";
            DrawHelpWindow(_faceMaterialHelpImage, txt, ref _faceMaterialHelpTextScrollPosition, ref _showFaceMaterialInfo);
        }
        EditorGUILayout.EndFadeGroup();

        // side material
        GUILayout.BeginHorizontal();
        Object sideMat = _style.GetArrayElementAtIndex(1).objectReferenceValue;
        Object nSideMat = EditorGUILayout.ObjectField("Side material:", sideMat, typeof(Material), false);
        if(nSideMat != sideMat) {
            _style.GetArrayElementAtIndex(1).objectReferenceValue = nSideMat;
            updateMesh = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showSideMaterialInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showSideMaterialInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Side material:") + "\n\n" +
                "The material here is used for the sides of the generated 3D text if it has any <b>depth</b>. You can change the depth of the 3D text in the " +
                VTextEditorGUIHelper.ConvertStringToHelpWindowCategoryLink("Mesh") + " category.";
            DrawHelpWindow(_sideMaterialHelpImage, txt, ref _sideMaterialHelpTextScrollPosition, ref _showSideMaterialInfo);
        }
        EditorGUILayout.EndFadeGroup();

        // bevel material
        GUILayout.BeginHorizontal();
        Object currentBevelMaterial = _style.GetArrayElementAtIndex(2).objectReferenceValue;
        Object newBevelMaterial = EditorGUILayout.ObjectField("Bevel material:", currentBevelMaterial, typeof(Material), false);
        if(newBevelMaterial != currentBevelMaterial) {
            _style.GetArrayElementAtIndex(2).objectReferenceValue = newBevelMaterial;
            updateMesh = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showBevelMaterialInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showBevelMaterialInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Bevel material:") + "\n\n" +
            "The material here is used for the <b>bevel</b> of the generated 3D text. The bevel is the smoothly rounded part between the front/back-side of "+
                "the 3D-Text and its sides. You can change the size of the <b>bevel</b> in the " + VTextEditorGUIHelper.ConvertStringToHelpWindowCategoryLink("Mesh") + 
                " category. It will only be available if the 3D-Text has a <b>depth</b>. You can change the <b>depth</b> of the 3D text " + 
                "in the " + VTextEditorGUIHelper.ConvertStringToHelpWindowCategoryLink("Mesh") + " category too.";

                DrawHelpWindow(_bevelMaterialHelpImage, txt, ref _bevelMaterialHelpTextScrollPosition, ref _showBevelMaterialInfo);
        }
        EditorGUILayout.EndFadeGroup();



        #endregion // MATERIALS

        return updateMesh;
    }

    protected void AppendFontname(string fn)
    {
        string [] nfn = new string[_fontNames.Length+1];
        for(int k=0; k < _fontNames.Length; k++) {
            nfn[k] = _fontNames[k];
        }
        nfn[_fontNames.Length] = fn;
        _fontNames = nfn;
    }

    protected void FillFonts(string oldname)
    {
        DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"));
        FileInfo[] fiarray = di.GetFiles("*.*");
        _fontNames = new string[] { "(none)" };
        int fc = 0;
        // fontChoice = 0;

        foreach(FileInfo fi in fiarray) {
            // Debug.Log(fi.Name + " ext: " + fi.Extension);
            if(".ttf" == fi.Extension) {
                if(oldname == fi.Name) {
                    fc = _fontNames.Length;
                }
                AppendFontname(fi.Name);
            } else if(".otf" == fi.Extension) {
                if(oldname == fi.Name) {
                    fc = _fontNames.Length;
                }
                AppendFontname(fi.Name);
            }
        }
        if(fc != _currentFontIndex) {
            _currentFontIndex = fc;
        }
        // Debug.Log(fontnames);
    }

    #region HELP WINDOWS
    /// <summary>
    /// Draws the help window with the specified parameters
    /// </summary>
    private void DrawHelpWindow(Texture helpImage, string helpText, ref Vector2 helpTextScrollbarPosition, ref AnimBool showHelpWindowVariable) {
        int currentIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();

        // the image
        VTextEditorGUIHelper.DrawBorderedImage(helpImage, HELP_IMAGE_WIDTH);
        float imgHeight = (float) helpImage.height / helpImage.width * HELP_IMAGE_WIDTH;
        float borderOffset = 6.0f;      // there is a 3-pixel space to each side when put the image into a border (like we do)

        // the help text
        helpTextScrollbarPosition = GUILayout.BeginScrollView(helpTextScrollbarPosition, "box", GUILayout.Height(imgHeight + borderOffset));
        EditorGUILayout.LabelField(helpText, VTextEditorGUIHelper.HelpTextStyle);
        GUILayout.EndScrollView();

        // close button
        if (GUILayout.Button(new GUIContent("x", "Close help"), GUILayout.ExpandWidth(false))) {
            showHelpWindowVariable.target = false;
        }
        GUILayout.Space(5);     // space 5 pixel
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = currentIndent;
    }
    #endregion

	#endregion // METHODS


	#region EVENT HANDLERS

	#endregion // EVENT HANDLERS
}
