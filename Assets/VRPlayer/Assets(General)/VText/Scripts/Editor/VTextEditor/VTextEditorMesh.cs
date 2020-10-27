// ----------------------------------------------------------------------
// File: 		VTextEditorMesh
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

/// <summary>
/// this class handles all mesh associated aspects of the text (like depth, bevel, shadow-casting/receiving, etc)
/// </summary>
public class VTextEditorMesh : AbstractVTextEditorComponent
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS
    /// <summary>
    /// the width of the labels in front of controls
    /// </summary>
    public const float LABEL_WIDTH = 100;

    /// <summary>
    /// the width of images in the help sections
    /// </summary>
    private const float HELP_IMAGE_WIDTH = 50.0f;
	#endregion // CONSTANTS


	#region FIELDS
    SerializedProperty _meshParam;                  // the mesh parameter component of the vtext object

    SerializedProperty _pDepth;                     // the depth of the text
    SerializedProperty _pBevel;                     // the size of the bevel of the text
    SerializedProperty _pNeedTangents;
    SerializedProperty _pBackface;                  // should backfaces being created
    SerializedProperty _pQuality;                   // the quality of the tesselation
    SerializedProperty _pShadowCast;                // the shadow casting mode (in Unity 5 and higher)
    SerializedProperty _pReceiveShadows;            // should the mesh receive shadows?
    SerializedProperty _pUseLightProbes;            // do we use light probes for the mesh?
    #if CREASE_OK
    SerializedProperty _pCrease;
    #endif

    #region INFOFIELDS
    private AnimBool _showDepthInfo = new AnimBool();                       // show or hide the help for the depth parameter
    private AnimBool _showBevelInfo = new AnimBool();                       // show or hide the help for the bevel parameter
    private AnimBool _showBackfaceInfo = new AnimBool();                    // show or hide the help for the backface parameter
    private AnimBool _showQualityInfo = new AnimBool();                     // show or hide the help for the quality parameter
    private AnimBool _showShadowCastInfo = new AnimBool();                  // show or hide the help for the shadow casting mode parameter (Unity5 and above)
    private AnimBool _showShadowCastInfoV4 = new AnimBool();                // show or hide the help for the shadow casting mode parameter (Unity4 - Unity5)
    private AnimBool _showReceiveShadowInfo = new AnimBool();               // show or hide the help for the receive shadow parameter
    private AnimBool _showUseLightProbesInfo = new AnimBool();              // show or hide the help for the use lightprobes parameter
    private AnimBool _showTangentsInfo = new AnimBool();                    // show or hide the help for the tangents parameter


    private Texture _depthHelpImage;                                        // the image which is shown in the depth help box
    private Texture _bevelHelpImage;                                        // the image which is shown in the bevel help box
    private Texture _backfaceHelpImage;                                     // the image which is shown in the backface help box
    private Texture _qualityHelpImage;                                      // the image which is shown in the quality help box
    private Texture _shadowCastHelpImage;                                   // the image which is shown in the shadow casting mode help box (Unity5 and above)
    private Texture _shadowCastHelpImageV4;                                 // the image which is shown in the shadow casting mode help box (Unity4 - Unity5)
    private Texture _receiveShadowHelpImage;                                // the image which is shown in the receive shadow help box
    private Texture _useLightProbesHelpImage;                               // the image which is shown in the use lightprobes help box
    private Texture _tangentsHelpImage;                                     // the image which is shown in the tangents help box

    Vector2 _depthInfoHelpTextScrollPosition = Vector2.zero;                // the scrollview position for the depth help text
    Vector2 _bevelInfoHelpTextScrollPosition = Vector2.zero;                // the scrollview position for the bevel help text
    Vector2 _backfaceInfoHelpTextScrollPosition = Vector2.zero;             // the scrollview position for the backface help text
    Vector2 _qualityInfoHelpTextScrollPosition = Vector2.zero;              // the scrollview position for the quality help text
    Vector2 _shadowCastInfoHelpTextScrollPosition = Vector2.zero;           // the scrollview position for the shadow casting mode help text (Unity5 and above)
    Vector2 _shadowCastInfoHelpTextScrollPositionV4 = Vector2.zero;         // the scrollview position for the shadow casting mode help text (Unity4 - Unity5)
    Vector2 _receiveShadowInfoHelpTextScrollPosition = Vector2.zero;        // the scrollview position for the receive shadow help text
    Vector2 _useLightProbesInfoHelpTextScrollPosition = Vector2.zero;       // the scrollview position for the use lightprobes help text
    Vector2 _tangentsInfoHelpTextScrollPosition = Vector2.zero;             // the scrollview position for the tangents help text
    #endregion // INFOFIELDS
	#endregion // FIELDS


	#region PROPERTIES

	#endregion // PROPERTIES


	#region CONSTRUCTORS

    public VTextEditorMesh(SerializedObject obj, Editor currentEditor) 
	{
        _meshParam = obj.FindProperty("parameter");

        _pDepth = _meshParam.FindPropertyRelative("m_depth");
        _pBevel = _meshParam.FindPropertyRelative("m_bevel");
        _pNeedTangents = _meshParam.FindPropertyRelative("m_needTangents");
        _pBackface = _meshParam.FindPropertyRelative("m_backface");
        _pQuality = _meshParam.FindPropertyRelative("m_quality");

        _pShadowCast = _meshParam.FindPropertyRelative("m_shadowCastMode");
        _pReceiveShadows = _meshParam.FindPropertyRelative("m_receiveShadows");
        _pUseLightProbes = _meshParam.FindPropertyRelative("m_useLightProbes");
        #if CREASE_OK
        _pCrease = _meshParam.FindPropertyRelative("m_crease");
        #endif

        // the images in the help screens
        _depthHelpImage = Resources.Load("Images/Icons/Help/Letter_T_3D_SideMaterial") as Texture;
        _bevelHelpImage = Resources.Load("Images/Icons/Help/Letter_T_3D_BevelMaterial") as Texture;
        _backfaceHelpImage = Resources.Load("Images/Icons/Help/Letter_T_3D_FaceMaterial") as Texture;
        _qualityHelpImage = Resources.Load("Images/Icons/Help/text_quality") as Texture;
        _shadowCastHelpImage = Resources.Load("Images/Icons/Help/Letter_DropShadow") as Texture;
        _shadowCastHelpImageV4 = Resources.Load("Images/Icons/Help/Letter_DropShadow") as Texture;
        _receiveShadowHelpImage = Resources.Load("Images/Icons/Help/Letter_ReceiveShadow") as Texture;
        _useLightProbesHelpImage = Resources.Load("Images/Icons/Help/Letter_M_3D_LightProbes") as Texture;
        _tangentsHelpImage = Resources.Load("Images/Icons/Help/Text_NormalMapping") as Texture;

        // add repaints if the animated values are changed
        _showDepthInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showBevelInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showBackfaceInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showQualityInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showShadowCastInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showShadowCastInfoV4.valueChanged.AddListener(currentEditor.Repaint);
        _showReceiveShadowInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showUseLightProbesInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showTangentsInfo.valueChanged.AddListener(currentEditor.Repaint);
	}

	#endregion // CONSTRUCTORS


	#region METHODS

    /// <summary>
    /// draw the ui for this component
    /// 
    /// returns true if this aspect of the VText should be updated (mesh, layout, physics, etc)
    /// </summary>
    public override bool DrawUI()
    {
        bool updateMesh = false;
        EditorGUIUtility.labelWidth = LABEL_WIDTH;

        #region DEPTH
        // depth
        GUILayout.BeginHorizontal();
        float nDepth = EditorGUILayout.FloatField ("Depth:", _pDepth.floatValue);
        if (nDepth < 0.0f) {
            nDepth = 0.0f;
        }

        if (nDepth != _pDepth.floatValue) {
            _pDepth.floatValue = nDepth;
            updateMesh = true;
        }

        if ((nDepth - Mathf.Epsilon) < 0.0f) {
            _pBevel.floatValue = 0.0f;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showDepthInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showDepthInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Depth:") + "\n\n" +
                "The <b>depth</b> defines the amount of extrusion of the font. \n" +
                "Keep in mind that you can <b>bevel</b> your text only if a depth is set.";
            DrawHelpWindow(_depthHelpImage, txt, ref _depthInfoHelpTextScrollPosition, ref _showDepthInfo);
        }
        EditorGUILayout.EndFadeGroup();

        #endregion // DEPTH

        #region BEVEL
        GUI.enabled = ((_pDepth.floatValue - Mathf.Epsilon) > 0.0f);
        GUILayout.BeginHorizontal();
        float nBevel = Mathf.Clamp01 (EditorGUILayout.FloatField ("Bevel:", _pBevel.floatValue));
        if (nBevel != _pBevel.floatValue) {
            _pBevel.floatValue = nBevel;
            updateMesh = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showBevelInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showBevelInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Bevel:") + "\n\n" +
                "The <b>bevel</b> is a smoothly rounded geometry between the front/back-faces of the text and it's side (see <b>depth</b>).\n" +
                "You can set the bevel <b><color=#cc2222>only</color></b> if a <b>depth</b> is set.";
            DrawHelpWindow(_bevelHelpImage, txt, ref _bevelInfoHelpTextScrollPosition, ref _showBevelInfo);
        }
        EditorGUILayout.EndFadeGroup();
        GUI.enabled = true;
        #endregion // BEVEL

        #region BACKFACE
        GUILayout.BeginHorizontal();
        bool nBackface = EditorGUILayout.Toggle ("Backface:", _pBackface.boolValue);
        if (nBackface != _pBackface.boolValue) {
            _pBackface.boolValue = nBackface;
            updateMesh = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showBackfaceInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showBackfaceInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Backface:") + "\n\n" +
                "For performance reasons we normally do not create faces on the backside of the text " +
                "because most of the texts are only visible from the front (menus, scores, etc).\n" + 
                "But in a couple of cases (for instance if you rotate your text) you want to see the backside too. " +
                "Here you can enable the backfaces. The corresponding <b>bevels</b> will be created too.";
            DrawHelpWindow(_backfaceHelpImage, txt, ref _backfaceInfoHelpTextScrollPosition, ref _showBackfaceInfo);
        }
        EditorGUILayout.EndFadeGroup();

        #endregion // BACKFACE

        #region QUALITY
        GUILayout.BeginHorizontal();
        int nQuality = EditorGUILayout.IntSlider("Quality:", _pQuality.intValue, 0, 100);
        if (nQuality != _pQuality.intValue)
        {
            _pQuality.intValue = nQuality;
            updateMesh = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showQualityInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showQualityInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Quality:") + "\n\n" +
                "With this parameter you can increase the number of triangles which are generated for this text.\n" +
                "The idea is simple ... the higher the quality the smoother the geometry but the worse the performance.\n" +
                "We already increase the number of triangles only for the curved parts of a letter.\n" +
                "So here is your parameter to find a tradeoff which fits your needs. To see the results its a good idea to change " + 
                "the rendermode of the " + VTextEditorGUIHelper.ConvertStringToHelpWindowCategoryLink("Scene view") + 
                " to <b>Wireframe</b> or <b>Shaded wireframe</b>.";
            DrawHelpWindow(_qualityHelpImage, txt, ref _qualityInfoHelpTextScrollPosition, ref _showQualityInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // QUALITY
       
        #region SHADOW CASTING MODE (UNITY 5 AND ABOVE)
        GUILayout.BeginHorizontal();
        if (EditorGUILayout.PropertyField(_pShadowCast, new GUIContent("Cast shadows:")))
        {
            updateMesh = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showShadowCastInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showShadowCastInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Cast shadow modes:") + "\n\n" +
                "Here you can set the way how Unity renders shadows for the generated text. \n" +
                "The following modes are available:\n\n" + 
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("OFF: ") + "No shadows are cast from this text.\n\n" + 
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("ON: ") + "Shadows are cast from this text.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("TwoSided: ") + "Shadows are cast from this text, treating it as two-sided. This way you can see the shadows also from inside a letter. " +
                "Normally you don't need this.\n\n" + 
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("ShadowsOnly: ") + "The text will cast shadows, but is invisible otherwise in the scene.";
            DrawHelpWindow(_shadowCastHelpImage, txt, ref _shadowCastInfoHelpTextScrollPosition, ref _showShadowCastInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // SHADOW CASTING MODE (UNITY 5 AND ABOVE)

        #region RECEIVE SHADOWS
        GUILayout.BeginHorizontal();
        bool nReceiveShadows = EditorGUILayout.Toggle ("Receive shadows:", _pReceiveShadows.boolValue);
        if (nReceiveShadows != _pReceiveShadows.boolValue) {
            _pReceiveShadows.boolValue = nReceiveShadows;
            updateMesh = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showReceiveShadowInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showReceiveShadowInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Receive shadows:") + "\n\n" +
                "Enable this if you want your text to receive shadows from other objects.";
            DrawHelpWindow(_receiveShadowHelpImage, txt, ref _receiveShadowInfoHelpTextScrollPosition, ref _showReceiveShadowInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // RECEIVE SHADOWS

        #region USE LIGHTPROBES
        GUILayout.BeginHorizontal();
        bool nUseLightProbes = EditorGUILayout.Toggle ("Use lightprobes:", _pUseLightProbes.boolValue);
        if (nUseLightProbes != _pUseLightProbes.boolValue) {
            _pUseLightProbes.boolValue = nUseLightProbes;
            updateMesh = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showUseLightProbesInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showUseLightProbesInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Use lightprobes:") + "\n\n" +
                "If you are using Unity's lightprobes and want them to affect the text then you should enable this parameter.";
            DrawHelpWindow(_useLightProbesHelpImage, txt, ref _useLightProbesInfoHelpTextScrollPosition, ref _showUseLightProbesInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // USE LIGHTPROBES

        #region TANGENTS
        GUILayout.BeginHorizontal();
        bool nNeedTangents = EditorGUILayout.Toggle("Create tangents:", _pNeedTangents.boolValue);
        if (nNeedTangents != _pNeedTangents.boolValue) {
            _pNeedTangents.boolValue = nNeedTangents;
            updateMesh = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showTangentsInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showTangentsInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Create tangents:") + "\n\n" +
                "Some shader (for instance normal map shaders) require tangents to work correctly. If you use such a shader for the text you should enable " +
                "this parameter.";
            DrawHelpWindow(_tangentsHelpImage, txt, ref _tangentsInfoHelpTextScrollPosition, ref _showTangentsInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // TANGENTS

        #if CREASE_OK
        float nCrease = EditorGUILayout.FloatField("Crease Angle", _pCrease.floatValue);
        if(nCrease != _pCrease.floatValue) {
            if(nCrease >= 0f) {
                if(nCrease < 90f) {
                    _pCrease.floatValue = nCrease;
                    updateMesh = true;
                }
            }
        }
        #endif
        return updateMesh;
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
