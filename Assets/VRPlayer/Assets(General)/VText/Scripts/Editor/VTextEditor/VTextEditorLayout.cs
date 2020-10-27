// ----------------------------------------------------------------------
// File: 		VTextEditorLayout
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;

/// <summary>
/// this class handles all layout stuff of the text (alignment, spacing, bending, etc)
/// </summary>
public class VTextEditorLayout : AbstractVTextEditorComponent
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS
    /// <summary>
    /// the width of the labels in front of controls
    /// </summary>
    private const float LABEL_WIDTH = 110;

    /// <summary>
    /// the width of the labels in front of controls in the curve bendig section
    /// </summary>
    private const float CURVE_BENDING_LABEL_WIDTH = 85;

    /// <summary>
    /// the width of the labels in front of controls in the circular bendig section
    /// </summary>
    private const float CIRCULAR_BENDING_LABEL_WIDTH = 140;

    /// <summary>
    /// the minimum width of an curve editor control
    /// </summary>
    private const float ANIMATION_CURVE_EDITOR_MIN_WIDTH = 100;

    /// <summary>
    /// the width of the "Align to curve" button
    /// </summary>
    private const float ALIGN_TO_CURVE_BUTTON_WIDTH = 90;

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
    private bool _showCommonParamters = true;                                           // show/hide the common parameters
    private bool _showBendingParameters = true;                                         // show/hide the bending parameters
    private AnimBool _showCircularBendingOptions = new AnimBool();                      // show/hide the circular bending parameters
    private AnimBool _showCircularBendingAnimateRadiusOptions = new AnimBool();         // show/hide the animate radius options in the circular bending section
    Texture _resetIcon;                                                                 // the icon used for resetting the bending curves

    SerializedProperty _layout;                                                         // the main layout component
    SerializedProperty _lHorizontal;
    SerializedProperty _lMajor;
    SerializedProperty _lMinor;
    SerializedProperty _lSpacing;
    SerializedProperty _gSpacing;
    SerializedProperty _curveXZ;
    SerializedProperty _curveXY;
    SerializedProperty _orientXZ;
    SerializedProperty _orientXY;

    SerializedProperty _isCircular;
    SerializedProperty _startAngle;
    SerializedProperty _endAngle;
    SerializedProperty _circleRadius;
    SerializedProperty _animateRadius;
    SerializedProperty _radiusCurve;

    #region INFOFIELDS
    private AnimBool _showHorizontalInfo = new AnimBool();                  // show or hide the horizontal info
    private AnimBool _showMajorInfo = new AnimBool();                       // show or hide the major info
    private AnimBool _showMinorInfo = new AnimBool();                       // show or hide the minor info
    private AnimBool _showLineSpaceInfo = new AnimBool();                   // show or hide the line space info
    private AnimBool _showGlyphSpaceInfo = new AnimBool();                  // show or hide the glyph space info
    private AnimBool _showCurveBendingXZInfo = new AnimBool();              // show or hide the curve bending (XZ) info
    private AnimBool _showCurveBendingXYInfo = new AnimBool();              // show or hide the curve bending (XY) info
    private AnimBool _showCircularBendingInfo = new AnimBool();             // show or hide the circular bending info

    private Texture _horizontalInfoHelpImage;                               // the image which is shown in the horizontal help box
    private Texture _majorInfoHelpImage;                                    // the image which is shown in the major help box
    private Texture _minorInfoHelpImage;                                    // the image which is shown in the minor help box
    private Texture _lineSpaceInfoHelpImage;                                // the image which is shown in the line space help box
    private Texture _glyphSpaceInfoHelpImage;                               // the image which is shown in the glyph space help box
    private Texture _curveBendingXZInfoHelpImage;                           // the image which is shown in the curve bending (XZ) help box
    private Texture _curveBendingXYInfoHelpImage;                           // the image which is shown in the curve bending (XY) help box
    private Texture _circularBendingInfoHelpImage;                          // the image which is shown in the circular bending help box

    Vector2 _horizontalInfoHelpTextScrollPosition = Vector2.zero;           // the scrollview position for the horizontal help text
    Vector2 _majorInfoHelpTextScrollPosition = Vector2.zero;                // the scrollview position for the major help text
    Vector2 _minorInfoHelpTextScrollPosition = Vector2.zero;                // the scrollview position for the minor help text
    Vector2 _lineSpaceInfoHelpTextScrollPosition = Vector2.zero;            // the scrollview position for the line space help text
    Vector2 _glyphSpaceInfoHelpTextScrollPosition = Vector2.zero;           // the scrollview position for the glyph space help text
    Vector2 _curveBendingXZInfoHelpTextScrollPosition = Vector2.zero;       // the scrollview position for the curve bending (XZ) help text
    Vector2 _curveBendingXYInfoHelpTextScrollPosition = Vector2.zero;       // the scrollview position for the curve bending (XY) help text
    Vector2 _circularBendingInfoHelpTextScrollPosition = Vector2.zero;      // the scrollview position for the circular help text
    #endregion // INFOFIELDS
    #endregion // FIELDS

	#region PROPERTIES

	#endregion // PROPERTIES


	#region CONSTRUCTORS

    public VTextEditorLayout(SerializedObject obj, Editor currentEditor) 
	{
        _layout = obj.FindProperty("layout");

        _lHorizontal = _layout.FindPropertyRelative("m_horizontal");
        _lMajor = _layout.FindPropertyRelative("m_major");
        _lMinor = _layout.FindPropertyRelative("m_minor");

        _lSpacing = _layout.FindPropertyRelative("m_spacing");
        _gSpacing = _layout.FindPropertyRelative("m_glyphSpacing");
        _curveXZ = _layout.FindPropertyRelative("m_curveXZ");
        _curveXY = _layout.FindPropertyRelative("m_curveXY");
        _orientXZ = _layout.FindPropertyRelative("m_orientXZ");
        _orientXY = _layout.FindPropertyRelative("m_orientXY");

        _isCircular = _layout.FindPropertyRelative("m_isCircular");
        _startAngle = _layout.FindPropertyRelative("m_startRadius");
        _endAngle = _layout.FindPropertyRelative("m_endRadius");
        _circleRadius = _layout.FindPropertyRelative("m_circleRadius");
        _animateRadius = _layout.FindPropertyRelative("m_animateRadius");
        _radiusCurve = _layout.FindPropertyRelative("m_curveRadius");

        // the images in the help screens
        if (EditorGUIUtility.isProSkin) {
            _resetIcon = Resources.Load("Images/Icons/Help/icon_reset") as Texture;
        } else {
            _resetIcon = Resources.Load("Images/Icons/Help/icon_reset_dark") as Texture;    
        }

        _horizontalInfoHelpImage = Resources.Load("Images/Icons/Help/layout_horizontal_vertical") as Texture;
        _majorInfoHelpImage = Resources.Load("Images/Icons/Help/text_alignment") as Texture;
        _minorInfoHelpImage = Resources.Load("Images/Icons/Help/text_alignment_minor") as Texture;

        _lineSpaceInfoHelpImage = Resources.Load("Images/Icons/Help/text_lineSpace") as Texture;
        _glyphSpaceInfoHelpImage = Resources.Load("Images/Icons/Help/text_glyphSpace") as Texture;
        _curveBendingXZInfoHelpImage = Resources.Load("Images/Icons/Help/text_bendCurveXZ") as Texture;
        _curveBendingXYInfoHelpImage = Resources.Load("Images/Icons/Help/text_bendCurveXY") as Texture;
        _circularBendingInfoHelpImage = Resources.Load("Images/Icons/Help/text_circularBending") as Texture;

        // add repaints if the animated values are changed
        _showCircularBendingOptions.valueChanged.AddListener(currentEditor.Repaint);
        _showCircularBendingAnimateRadiusOptions.valueChanged.AddListener(currentEditor.Repaint);

        _showHorizontalInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showMajorInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showMinorInfo.valueChanged.AddListener(currentEditor.Repaint);

        _showLineSpaceInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showGlyphSpaceInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showCurveBendingXYInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showCurveBendingXZInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showCircularBendingInfo.valueChanged.AddListener(currentEditor.Repaint);
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
        bool updateLayout = false;

        #region COMMON PARAMETERS
        _showCommonParamters = EditorGUILayout.Foldout(_showCommonParamters, "Common");
        if (_showCommonParamters) {
            EditorGUI.indentLevel++;
            updateLayout |= DrawCommonParameters();
            EditorGUI.indentLevel--;
        }
        #endregion // COMMON PARAMETERS

        #region BENDING PARAMETERS
        _showBendingParameters = EditorGUILayout.Foldout(_showBendingParameters, "Bending");

        if (_showBendingParameters) {
            EditorGUI.indentLevel++;
            updateLayout |= DrawBendingCurves();
            GUILayout.Space(10.0f);
            updateLayout |= DrawCircularBendingParameters();
            EditorGUI.indentLevel--;
        }

        #endregion // BENDING PARAMETERS

        return updateLayout;
    }

    /// <summary>
    /// Draws the common parameters.
    /// </summary>
    /// <returns><c>true</c>, if common parameters was drawn, <c>false</c> otherwise.</returns>
    private bool DrawCommonParameters() {
        bool updateLayout = false; 

        #region HORIZONTAL
        EditorGUIUtility.labelWidth = LABEL_WIDTH;

        GUILayout.BeginHorizontal();

        bool nHorizontal = EditorGUILayout.Toggle (new GUIContent("Horizontal:", "Text direction (horizontal or vertical)"), _lHorizontal.boolValue);
        if (nHorizontal != _lHorizontal.boolValue) {
            _lHorizontal.boolValue = nHorizontal;
            updateLayout = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showHorizontalInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showHorizontalInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Horizontal:") + "\n\n" +
                "If this is set to true then the text is layouted horizontally. Otherwise the text is layouted vertically.";

            DrawHelpWindow(_horizontalInfoHelpImage, txt, ref _horizontalInfoHelpTextScrollPosition, ref _showHorizontalInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // HORIZONTAL

        #region MAJOR
        GUILayout.BeginHorizontal();

        int nMajor = (int)(VTextLayout.align)EditorGUILayout.EnumPopup(new GUIContent("Major:", "Major layout mode"), 
            (VTextLayout.align)System.Enum.GetValues (typeof(VTextLayout.align)).GetValue (_lMajor.enumValueIndex));
        if (_lMajor.enumValueIndex != nMajor) {
            _lMajor.enumValueIndex = nMajor;
            updateLayout = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showMajorInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showMajorInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Major:") + "\n\n" +
                "This defines the alignment of the text. It is called <i>'Major'</i> because it depends on the general layout of your text " +
                "(see the <b>Horizontal</b> parameter). If it is layouted horizontally this parameter means the alignment in X-direction. " +
                "If the text is layouted vertically it means the alignment in Y-direction.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Base: ") + "This aligns the baseline of the text to the parent's transform. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Start: ") + "This aligns the position of the first letter of the text to the " +
                    "parent's transform. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Center: ") + "The center of each line is aligned to its parent's transform. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("End: ") + "The end of each line is aligned to its parent's transform.";
            DrawHelpWindow(_majorInfoHelpImage, txt, ref _majorInfoHelpTextScrollPosition, ref _showMajorInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // MAJOR

        #region MINOR
        GUILayout.BeginHorizontal();

        int nMinor = (int)(VTextLayout.align)EditorGUILayout.EnumPopup (new GUIContent("Minor:", "Minor layout mode"),
            (VTextLayout.align)System.Enum.GetValues (typeof(VTextLayout.align)).GetValue (_lMinor.enumValueIndex));
        if (_lMinor.enumValueIndex != nMinor) {
            _lMinor.enumValueIndex = nMinor;
            updateLayout = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showMinorInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showMinorInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Minor:") + "\n\n" +
                "This defines the alignment of the text. It is called <i>'Minor'</i> because it depends on the general layout of your text " +
                "(see the <b>Horizontal</b> parameter). The alignment direction is the " + VTextEditorGUIHelper.ConvertStringToHelpWindowWarning("opposite") + 
                " to the <i>'Major'</i> alignment. So if your text is layouted horizontally then this describes the alignment in y-direction. If your text is " +
                "layouted vertically then this parameter describes the alignment in x-direction\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Base: ") + "This aligns the baseline of the text to the parent's transform. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Start: ") + "This aligns the position of the first letter of the text to the " +
                "parent's transform. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Center: ") + "The center of each line is aligned to its parent's transform. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("End: ") + "The end of each line is aligned to its parent's transform.";

            DrawHelpWindow(_minorInfoHelpImage, txt, ref _minorInfoHelpTextScrollPosition, ref _showMinorInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // MINOR

        #region LINE SPACE
        GUILayout.BeginHorizontal();
        float nSpacing = EditorGUILayout.FloatField (new GUIContent("Line space:", "The space between two lines"), _lSpacing.floatValue);
        if (nSpacing != _lSpacing.floatValue) {
            _lSpacing.floatValue = nSpacing;
            updateLayout = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showLineSpaceInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showLineSpaceInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Line space:") + "\n\n" +
                "This parameter defines the distance of the <b>base line</b> of one line to the <b>base line</b> of the next line. \n" +
                "This means that a line space of 0 will put all lines on top of each other. In this way you can create overlapping texts too. \n" +
                "A value of 1 means the “<i>normal</i>” line space as they are defined by the font. A value of 2 for instance doubles the distance " +
                "between two lines.";

            DrawHelpWindow(_lineSpaceInfoHelpImage, txt, ref _lineSpaceInfoHelpTextScrollPosition, ref _showLineSpaceInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // LINE SPACE

        #region LETTER SPACE
        GUILayout.BeginHorizontal();
        float nGlyphSpacing = EditorGUILayout.FloatField (new GUIContent("Letter space:", "The space between two letters"), _gSpacing.floatValue);
        if (nGlyphSpacing != _gSpacing.floatValue) {
            _gSpacing.floatValue = nGlyphSpacing;
            updateLayout = true;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showGlyphSpaceInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showGlyphSpaceInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Letter space:") + "\n\n" +
                "This parameter defines the additional distance between two letters of the text. \n" +
                "A letter space of 0 will generate the distance which is provided by the choosen font (most likely no extra space between letters).";

            DrawHelpWindow(_glyphSpaceInfoHelpImage, txt, ref _glyphSpaceInfoHelpTextScrollPosition, ref _showGlyphSpaceInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // LETTER SPACE

        return updateLayout;
    }

    /// <summary>
    /// Draws the bending parameters.
    /// </summary>
    public bool DrawBendingCurves() {
        EditorGUIUtility.labelWidth = CURVE_BENDING_LABEL_WIDTH;

        bool updateLayout = false;

        #region CURVE BENDING XZ
        EditorGUILayout.BeginHorizontal();
        AnimationCurve cxz = _curveXZ.animationCurveValue;
        AnimationCurve ncxz = EditorGUILayout.CurveField(new GUIContent("Bend XZ:", "Bend the text around the Y-axis"), cxz, GUILayout.MinWidth(ANIMATION_CURVE_EDITOR_MIN_WIDTH));
        if (!ncxz.Equals (cxz)) {
            _curveXZ.animationCurveValue = ncxz;
            updateLayout = true;
        }

        if ( GUILayout.Button(new GUIContent("Align to curve", "Align letters to curve"), 
            _orientXZ.boolValue ? VTextEditorGUIHelper.ToggleButtonStyleToggled : VTextEditorGUIHelper.ToggleButtonStyleNormal, GUILayout.Width(ALIGN_TO_CURVE_BUTTON_WIDTH)))
        {
            _orientXZ.boolValue = !_orientXZ.boolValue;
            updateLayout = true;
        }

        if (GUILayout.Button(new GUIContent(_resetIcon, "Reset curve"), GUILayout.Width(20), GUILayout.Height(VTextEditorGUIHelper.HELP_BUTTON_HEIGHT))) {
            _curveXZ.animationCurveValue = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
            _orientXZ.boolValue = false;
            updateLayout = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showCurveBendingXZInfo);       
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showCurveBendingXZInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Curve bending (in XZ direction):") + "\n\n" +
                "This bends your text according to the specified curve in xz direction. Your text will stay on the ground (more specific it will not " +
                "change the y-values of the letters. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Curve: ") + "The bending curve. The x-values of the <b>curve</b> should be between <b>0</b> " +
                "and <b>1</b> where 0 is the beginning of the text and 1 is the end of the text. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Align to curve: ") + "If you enable this option the letters of the text will be " +
                "rotated accordingly to the curves direction. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Reset: ") + "This will reset the <b>curve</b> to its original values with no bending at all and " +
                "sets the <b>Align to curve</b> parameter to false.";
            DrawHelpWindow(_curveBendingXZInfoHelpImage, txt, ref _curveBendingXZInfoHelpTextScrollPosition, ref _showCurveBendingXZInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // CURVE BENDING XZ

        #region CURVE BENDING XY
        EditorGUILayout.BeginHorizontal ();
        AnimationCurve cxy = _curveXY.animationCurveValue;
        AnimationCurve ncxy = EditorGUILayout.CurveField (new GUIContent("Bend XY:", "Bend the text around the Z-axis"), cxy, GUILayout.MinWidth(ANIMATION_CURVE_EDITOR_MIN_WIDTH));
        if (!ncxy.Equals (cxy)) {
            _curveXY.animationCurveValue = ncxy;
            updateLayout = true;
        }
        if ( GUILayout.Button(new GUIContent("Align to curve", "Align letters to curve"), 
            _orientXY.boolValue ? VTextEditorGUIHelper.ToggleButtonStyleToggled : VTextEditorGUIHelper.ToggleButtonStyleNormal, GUILayout.Width(ALIGN_TO_CURVE_BUTTON_WIDTH)))
        {
            _orientXY.boolValue = !_orientXY.boolValue;
            updateLayout = true;
        }

        if (GUILayout.Button(new GUIContent(_resetIcon, "Reset curve"), GUILayout.Width(20), GUILayout.Height(VTextEditorGUIHelper.HELP_BUTTON_HEIGHT))) {
            _curveXY.animationCurveValue = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
            _orientXY.boolValue = false;
            updateLayout = true;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showCurveBendingXYInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showCurveBendingXYInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Curve bending (in XY direction):") + "\n\n" +
                "This bends your text according to the specified curve in xy direction. The letters of your text will move up and down depending on the " +
                "specified curve. The z-position of the letters will stay the same. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Curve: ") + "The bending curve. The x-values of the <b>curve</b> should be between <b>0</b> " +
                "and <b>1</b> where 0 is the beginning of the text and 1 is the end of the text. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Align to curve: ") + "If you enable this option the letters of the text will be " +
                "rotated accordingly to the curves direction. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Reset: ") + "This will reset the <b>curve</b> to its original values with no bending at all and " +
                "sets the <b>Align to curve</b> parameter to false.";

            DrawHelpWindow(_curveBendingXYInfoHelpImage, txt, ref _curveBendingXYInfoHelpTextScrollPosition, ref _showCurveBendingXYInfo);
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // CURVE BENDING XY

        return updateLayout;
    }

    /// <summary>
    /// Draws the circular bending parameters.
    /// </summary>
    public bool DrawCircularBendingParameters() {
        #region CIRCULAR BENDING
        EditorGUIUtility.labelWidth = CIRCULAR_BENDING_LABEL_WIDTH;

        bool updateLayout = false;

        GUILayout.BeginHorizontal();
        bool isCircle = EditorGUILayout.Toggle(new GUIContent ("Circular bending:", "Enable/Disable circular bending"), _isCircular.boolValue);
        if (isCircle != _isCircular.boolValue) {
            _isCircular.boolValue = isCircle;
            updateLayout = true;
        }

        if (_isCircular.boolValue != _showCircularBendingOptions.value) {
            _showCircularBendingOptions.target = _isCircular.boolValue;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showCircularBendingInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showCircularBendingOptions.faded))
        {
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            float start = EditorGUILayout.FloatField (new GUIContent ("Start angle:", "The start angle"), _startAngle.floatValue);
            if (start != _startAngle.floatValue) {
                _startAngle.floatValue = start;
                updateLayout = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            float end = EditorGUILayout.FloatField (new GUIContent ("End angle:", "The end angle"), _endAngle.floatValue);
            if (end != _endAngle.floatValue) {
                _endAngle.floatValue = end;
                updateLayout = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            float radius = EditorGUILayout.FloatField (new GUIContent ("Radius:", "The size of the circle"), _circleRadius.floatValue);
            if (radius != _circleRadius.floatValue) {
                _circleRadius.floatValue = radius;
                updateLayout = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            EditorGUILayout.EndHorizontal();

            bool animR = EditorGUILayout.Toggle(new GUIContent ("Animate radius:", "Change the radius along the text."), _animateRadius.boolValue);
            if (animR != _animateRadius.boolValue) {
                _animateRadius.boolValue = animR;
                updateLayout = true;
            }
            if (_animateRadius.boolValue != _showCircularBendingAnimateRadiusOptions.value) {
                _showCircularBendingAnimateRadiusOptions.target = _animateRadius.boolValue;
            }
            if (EditorGUILayout.BeginFadeGroup(_showCircularBendingAnimateRadiusOptions.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                AnimationCurve cR = _radiusCurve.animationCurveValue;
                AnimationCurve ncR = EditorGUILayout.CurveField("Radius curve:", cR, GUILayout.MinWidth(ANIMATION_CURVE_EDITOR_MIN_WIDTH));
                if (!ncR.Equals(cR)) {
                    _radiusCurve.animationCurveValue = ncR;
                    updateLayout = true;
                }
                if (GUILayout.Button(new GUIContent(_resetIcon, "Reset curve"), GUILayout.Width(20), GUILayout.Height(VTextEditorGUIHelper.HELP_BUTTON_HEIGHT))) {
                    _radiusCurve.animationCurveValue = new AnimationCurve(new Keyframe (0, 0), new Keyframe (1, 0));
                    updateLayout = true;
                }
                GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
                EditorGUILayout.EndHorizontal ();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();


            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        if (EditorGUILayout.BeginFadeGroup(_showCircularBendingInfo.faded))
        {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Circular bending:") + "\n\n" +
                "This allows you to bend your text perfectly around a circle. The following parameters can be used to change the bending: \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Start angle: ") + "Use this parameter to define the position of the first " +
                "letter of your text on the circle. This is specified in degree. You can also use values larger than 360 or less then 0 if you want " +
                "to bend your text more than one time around the circle.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("End angle: ") + "This defines the position of the last letter of your text on the circle. " +
                "It is defined in degree and you can also use values larger than 360 or less then 0 if you want to bend your text more than one time " +
                "around the circle. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Radius: ") + "This defines the radius of the circle and therefor how far the letters are " +
                "away from the center of the circle (esp. the position of the VText object). \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Animate radius: ") + "This section allows you to change the <b>radius of the circle</b> along " +
                "the text. The <b>curves</b> x values should be between <b>0</b> and <b>1</b> where 0 means the beginning of the text and 1 its ending. \n" +
                "The <b>curves</b> y values are multiplied by the specified <b>radius</b> of the circle. \n" +
                "Use the <b>Reset</b> button to reset your curve to its default values which results in a fixed radius (defined by the <b>radius</b> parameter) " +
                "for the circle. \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowWarning("Tip: ") + "This allows you to bend your text like a spiral for instance. Furthermore you can " +
                "combine it with the <b>curve bending in XY direction</b> to achieve a tornado-like effect for your text (set the <b>end angle</b> to a value larger " +
                "than 360 degree (and/or modify the <b>start angle</b> accordingly) to make the text bend a couple of times around the circle.";

            DrawHelpWindow(_circularBendingInfoHelpImage, txt, ref _circularBendingInfoHelpTextScrollPosition, ref _showCircularBendingInfo);
        }
        EditorGUILayout.EndFadeGroup();

        return updateLayout;
        #endregion // CIRCULAR BENDING

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
