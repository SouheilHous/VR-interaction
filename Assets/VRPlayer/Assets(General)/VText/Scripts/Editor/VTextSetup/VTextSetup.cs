/*
 * $Id: VTextSetup.cs 225 2015-04-29 13:56:16Z dirk $
 *
 * Virtence VText package
 * Copyright 2015 by Virtence GmbH
 * http://www.virtence.com
 * 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Timers;

/// <summary>
/// the setup tool
/// IT REQUIRES Unity 5 and above
/// </summary>
public class VTextSetup : EditorWindow
{
    #region CONSTANTS
    private const string EMAIL_STRING = "<color=#23aacc><b>support@virtence.com</b></color>";                           // the string to our email adress
    private const string HOMEPAGE_VIRTENCE_STRING = "<color=#23aacc><b>http://www.virtence.com</b></color>";            // the string to our main homepage
    private const string HOMEPAGE_VTEXT_STRING = "<color=#23aacc><b>http://www.virtence.com/products/unity-erweiterungen/vtext/</b></color>";           // the string to our vtext homepage
    #endregion // CONSTANTS

    #region FIELDS
    private Texture _vtextLogo;                             // the logo image of the header
    private Texture _finishLogo;                            // the logo which is shown in the finish screen 
    private Dictionary<VTextInstallationSteps, string> _installationStepToLabelMap = new Dictionary<VTextInstallationSteps, string>();      // mapping of the installation step to a readable string
    private VTextInstallationSteps _currentInstallationStep = VTextInstallationSteps.Unknown;       // the current installation step

    private string _logMessage;                                     // the log message (esp. for error message during the installation process)
    #region SLIDESHOW
    private List<Texture> _slideShowImages;                         // the list of images shown in the slideshow
    private static float _slideShowImageDuration = 3.0f;            // the duration of how long an image is rendered in seconds
    private int _currentSlideShowIndex;                             // the index of the current shown image in _slideShowImages
    private float _slideShowStartTime;                              // the start time of the slideshow (esp. the time when a new picture is shown up)
    #endregion SLIDESHOW

    Vector2 _welcomeScrollViewPosition = new Vector2();             // the scroll view position in the welcome screen
    Vector2 _licenseAgreementScrollViewPosition = new Vector2();    // the scroll view position in the license agreement screen
    Vector2 _fontInstallationScrollViewPosition = new Vector2();    // the scroll view position in the fonts installation screen
    Vector2 _finishScrollViewPosition = new Vector2();              // the scroll view position in the finish screen

    private GUIStyle _currentInstallationStepTextStyle = new GUIStyle();                     // the text style for the current installation step texts (white or black + wordwrap + richtext)
    private GUIStyle _currentInstallationStepHeaderTextStyle = new GUIStyle();               // the text style for the current installation step header


    bool _installationStarted = false;                              // determines if the installation of plugins and folders was already started      
    bool _installationSuccessful = false;                           // determines if the installation of plugins and folders was already started      
    #endregion // FIELDS

    #region PROPERTIES
    private VTextInstallationSteps CurrentInstallationStep {
        get { return _currentInstallationStep; }
        set { 
            _currentInstallationStep = value;
            if (_currentInstallationStep == VTextInstallationSteps.Welcome) {               
                _currentSlideShowIndex = 0;
                _slideShowStartTime = (float) EditorApplication.timeSinceStartup;
            } 

            if (_currentInstallationStep != VTextInstallationSteps.Installation) {
                _installationSuccessful = false;
            }
        }
    }
    #endregion // PROPERTIES

    #region METHODS
    [MenuItem("Window/Virtence/VText Setup...")]
    static void Init()
    {
        VTextSetup window = EditorWindow.GetWindow(typeof(VTextSetup)) as VTextSetup;

        window.minSize = new Vector2(400, 220);
        window.Show();
    }

    /// <summary>
    /// do initial stuff here
    /// </summary>
    void OnEnable() {        
        _vtextLogo = Resources.Load("Images/VTextLogo") as Texture;
        _finishLogo = Resources.Load("Images/Setup/Creativity") as Texture;
        _installationStarted = false;

        // map the current installation step to a readable string
        // if you miss one then the value of the enum is simply converted to string
        _installationStepToLabelMap[VTextInstallationSteps.Welcome] = "Welcome";
        _installationStepToLabelMap[VTextInstallationSteps.LegalInformations] = "Legal informations";
        _installationStepToLabelMap[VTextInstallationSteps.Installation] = "Install";
        _installationStepToLabelMap[VTextInstallationSteps.InstallFonts] = "Install fonts";
        _installationStepToLabelMap[VTextInstallationSteps.Finish] = "Congratulations";

        // setup the style for the texts for the current installation step
        if (EditorGUIUtility.isProSkin) {
            _currentInstallationStepTextStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        } else {
            _currentInstallationStepTextStyle.normal.textColor = Color.black;
        }
        _currentInstallationStepTextStyle.wordWrap = true;
        _currentInstallationStepTextStyle.richText = true;

        // setup the style for the headers of the current installation step
        _currentInstallationStepHeaderTextStyle.alignment = TextAnchor.MiddleCenter;
        _currentInstallationStepHeaderTextStyle.fontSize = 20;
        _currentInstallationStepHeaderTextStyle.fontStyle = FontStyle.Bold;

        // setup slideshow images
        _slideShowImages = new List<Texture>() {
            Resources.Load("Images/Setup/Creativity") as Texture,
            Resources.Load("Images/Setup/SlideShow/image01") as Texture,
            Resources.Load("Images/Setup/SlideShow/image02") as Texture,
            Resources.Load("Images/Setup/SlideShow/image03") as Texture,
            Resources.Load("Images/Setup/SlideShow/image04") as Texture,
            Resources.Load("Images/Setup/SlideShow/image05") as Texture,
            Resources.Load("Images/Setup/SlideShow/image06") as Texture,
        };
        _slideShowStartTime = (float) EditorApplication.timeSinceStartup;

        CurrentInstallationStep = VTextInstallationSteps.Welcome;
    }
        
    void Update() {
        if (_currentInstallationStep == VTextInstallationSteps.Welcome) {
            Repaint();          // force repainting for the slide show
        }
    }
    void OnGUI() {
        GUILayout.Space(10);
        DrawLogo();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        DrawInstallationSteps();
        DrawCurrentInstallationStep();
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// returns the path to the parent folder with the specified name (parent name) ... starting at the specified path (path)
    /// </summary>
    /// <returns>The parent.</returns>
    /// <param name="path">Path.</param>
    /// <param name="parentName">Parent name.</param>
    private string GetParentFolder(string path, string parentName)
    {
        var dir = new DirectoryInfo(path);

        if (dir.Parent == null) {
            return null;
        }

        if (dir.Parent.Name == parentName) {
            return dir.Parent.FullName;
        }

        return this.GetParentFolder(dir.Parent.FullName, parentName);
    }

    /// <summary>
    /// Draws the installation step header.
    /// </summary>
    /// <param name="header">Header.</param>
    private void DrawInstallationStepHeader(string header) {
        Color c = _currentInstallationStepHeaderTextStyle.normal.textColor;
        _currentInstallationStepHeaderTextStyle.normal.textColor = new Color().HexToColor("34ccff");
        GUILayout.Space(10);
        EditorGUILayout.LabelField(header, _currentInstallationStepHeaderTextStyle);
        GUILayout.Space(10);
        _currentInstallationStepHeaderTextStyle.normal.textColor = c;
    }

    /// <summary>
    /// converts the specfied text into a subtitle
    /// </summary>
    /// <returns>The sub title string.</returns>
    /// <param name="txt">Text.</param>
    private string GetSubTitleString(string txt) {
        return string.Format("<color=#23aacc><b>{0}</b></color>", txt);
    }

    /// <summary>
    /// converts the specfied text into a error
    /// </summary>
    /// <returns>The sub title string.</returns>
    /// <param name="txt">Text.</param>
    private string GetErrorString(string txt) {
        return string.Format("<color=#aa2323><b>{0}</b></color>", txt);
    }

    /// <summary>
    /// enters the next installation step
    /// </summary>
    private void GotoNextInstallationStep() {
        int idx = (int) _currentInstallationStep + 1;
        GotoInstallationStep(idx);
    }

    /// <summary>
    /// enters the previous installation step
    /// </summary>
    private void GotoPreviousInstallationStep() {
        int idx = (int) _currentInstallationStep - 1;
        GotoInstallationStep(idx);
    }

    /// <summary>
    /// go to the installation step with the specified index (if it exists of cause :))
    /// </summary>
    /// <param name="idx">Index.</param>
    private void GotoInstallationStep(int idx) {        
        if (idx > 0 && idx < Enum.GetNames(typeof(VTextInstallationSteps)).Length) {
            _logMessage = string.Empty;         // reset the log message

            CurrentInstallationStep = (VTextInstallationSteps)idx;
        }
    }

    /// <summary>
    /// Draws the header (most likely the VText or the Virtence logo).
    /// </summary>
    private void DrawLogo() {
        if (_vtextLogo != null) {

            float aspectRatio = (float) _vtextLogo.height / _vtextLogo.width;
            float desiredWidth = EditorGUIUtility.currentViewWidth * 0.4f;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            TextAnchor currentAlignment = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(_vtextLogo, GUILayout.Width(desiredWidth), GUILayout.Height(desiredWidth * aspectRatio));
            GUI.skin.label.alignment = currentAlignment;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Draws the installation steps.
    /// </summary>
    private void DrawInstallationSteps() {
        GUILayout.BeginVertical("box", GUILayout.Width(150), GUILayout.ExpandHeight(true));
        int index = 0;
        foreach (VTextInstallationSteps value in Enum.GetValues(typeof(VTextInstallationSteps))) {
            GUIStyle style = EditorStyles.label;
            if (value == _currentInstallationStep)
                style = EditorStyles.boldLabel;

            if (value != VTextInstallationSteps.Unknown) {
                if (_installationStepToLabelMap.ContainsKey(value)) {
                    GUILayout.Label(string.Format("{0}. {1}", ((int) value), _installationStepToLabelMap[value]), style);
                } else {
                    GUILayout.Label(string.Format("{0}. {1}", ((int) value), value.ToString()), style);
                }
            }
            index++;
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draws the current installation step.
    /// </summary>
    private void DrawCurrentInstallationStep() {
        GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

        switch (_currentInstallationStep) {
        case VTextInstallationSteps.Welcome:
            DrawWelcomeStep();
            break;
        case VTextInstallationSteps.LegalInformations:
            DrawLicenseAgreementStep();
            break;
        case VTextInstallationSteps.Installation:
            DrawInstallationStep();
            break;
        case VTextInstallationSteps.InstallFonts:
            DrawInstallFontsStep();
            break;
        case VTextInstallationSteps.Finish:
            DrawFinishStep();
            break;
        }

        GUILayout.EndVertical();
    }

    #region WELCOME
    /// <summary>
    /// Draws the welcome step.
    /// </summary>
    private void DrawWelcomeStep() {
        DrawInstallationStepHeader(_installationStepToLabelMap[VTextInstallationSteps.Welcome]);

        DrawSlideShow();

        _welcomeScrollViewPosition = GUILayout.BeginScrollView(_welcomeScrollViewPosition);
        string str = "<b>VText</b> provides you with an easy to use way to create dynamic 3D texts in Unity. So just be creative and create your ingame texts or " +
            "menus or whatever you have in mind. \n\n" +
            "We wish you much fun with <b>VText</b> and if you have any questions just send an email to: \n" +
            EMAIL_STRING + "\n\n" +
            "Or visit our hompage at: \n" +
            HOMEPAGE_VIRTENCE_STRING + "\n\n" +
            "Or visit the VText homepage directly: \n" +
            HOMEPAGE_VTEXT_STRING;
        GUILayout.Label(str, _currentInstallationStepTextStyle);
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Next")) {
            GotoNextInstallationStep();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the slide show in the welcome screen.
    /// </summary>
    private void DrawSlideShow() {
        float imageHeight = 100;
        float boxHeight = imageHeight + 20;

        if (EditorApplication.timeSinceStartup - _slideShowStartTime > _slideShowImageDuration) {            
            _currentSlideShowIndex = (_currentSlideShowIndex + 1) % _slideShowImages.Count;
            _slideShowStartTime = (float) EditorApplication.timeSinceStartup;
        }           
        if (_currentSlideShowIndex < _slideShowImages.Count) {
            Texture image = _slideShowImages[_currentSlideShowIndex];
            if (image != null) {
                float aspectRatio = (float)image.height / image.width;

                GUILayout.BeginVertical("box", GUILayout.Height(boxHeight));
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                TextAnchor currentAlignment = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(image, GUILayout.Width(imageHeight / aspectRatio), GUILayout.Height(imageHeight));
                GUI.skin.label.alignment = currentAlignment;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
        }
    }

    #endregion // WELCOME

    #region LICENSE AGREEMENT
    private void DrawLicenseAgreementStep() {
        DrawInstallationStepHeader(_installationStepToLabelMap[VTextInstallationSteps.LegalInformations]);

        _licenseAgreementScrollViewPosition = GUILayout.BeginScrollView(_licenseAgreementScrollViewPosition);
        string str = "This package is copyright ©2014 by Virtence GmbH.\n\n" +
            GetSubTitleString("3rd party\n\n") + 
            "This package uses the following 3rd party products:\n\n" +
            "<b>freetype2</b>\n" +
            "Portions of this software are copyright ©2014 The FreeType Project (www.freetype.org). All rights reserved.\n\n" +
            GetSubTitleString("Fonts\n\n") + 
            "This package contains the following fonts, which are licensed under the SIL Open Font License:\n" +
            "  CostaRica.otf\n" +
            "  Dotrice-Bold-Condensed.otf\n" +
            "  Lack.otf\n" +
            "  Lobster.otf\n" +
            "  Segment14.otf\n" +
            "  Xolonium-Bold.otf\n" +
            "  RacingSansOne-Regular.ttf\n\n";
        GUILayout.Label(str, _currentInstallationStepTextStyle);
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Next")) { 
            GotoNextInstallationStep();
        }
        GUILayout.EndHorizontal();
    }
    #endregion // LICENSE AGREEMENT

    #region INSTALLATION
    /// <summary>
    /// Draws the installation step. (creating folders and check plugins)
    /// </summary>
    private void DrawInstallationStep() {
        DrawInstallationStepHeader(_installationStepToLabelMap[VTextInstallationSteps.Installation]);

        if (!_installationStarted) {
            _installationSuccessful = SetupFolders(ref _logMessage);
            _installationSuccessful &= CheckPluginsValid(ref _logMessage);

            _installationStarted = true;
        }
         
        // force repaint if installation was successful to force the next installation step happen
        // Unity throws an error if you don't force it
        if (_installationSuccessful) {
            Repaint();
        }
        if (Event.current.type == EventType.Repaint && _installationSuccessful) {             
            GotoNextInstallationStep();
        } else {
            string txt = GetErrorString("ERROR: \n") + _logMessage + "\n\n" +
                "Please send us an email with the errors and we will contact you and fix the problems:\n" +
                EMAIL_STRING;
            GUILayout.Label(txt, _currentInstallationStepTextStyle);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Restart")) {
                _installationStarted = false;
                GotoInstallationStep((int)VTextInstallationSteps.Welcome);
            }
            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// setup the necessary folders
    /// </summary>
    private bool SetupFolders(ref string errorMessage) {
        DirectoryInfo di = Directory.CreateDirectory(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"));
        AssetDatabase.Refresh(); 

        if (di.Exists) {
            return true;
        } else {
            errorMessage += "- Could <b>not</b> setup the '<i>Assets/StreamingAssets/Fonts</i>' folder. \n";
            return false;
        }
    }

    /// <summary>
    /// Checks if the plugins are valid and set the correct platform settings
    /// </summary>
    /// <returns><c>true</c>, if plugins valid was checked, <c>false</c> otherwise.</returns>
    private bool CheckPluginsValid(ref string errorMessage)
    {
        PluginImporter[] plugs = PluginImporter.GetAllImporters();
        if (null != plugs) {
            foreach (PluginImporter plug in plugs) {
                FileInfo fi = new FileInfo(plug.assetPath);
                bool refresh = false;
                bool anyPlatform = false;
                // Debug.Log(plug.assetPath + " dir: " + fi.Directory.Name);
                switch (fi.Directory.Name) {
                case "Plugins":
                    if ("VText.bundle" == fi.Name) {
                        anyPlatform = plug.GetCompatibleWithAnyPlatform();
                        if (anyPlatform) {
                            plug.SetCompatibleWithAnyPlatform(false);
                            plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, true);
                            Debug.Log(Application.platform);
                            switch (Application.platform) {
                            case RuntimePlatform.OSXEditor:
                                plug.SetCompatibleWithEditor(true);
                                break;
                            default:
                                plug.SetCompatibleWithEditor(false);
                                break;
                            }
                            refresh = true;
                        }
                    }
                    break;
                case "Android":
                    if ("libVText.so" == fi.Name) {
                        anyPlatform = plug.GetCompatibleWithAnyPlatform();
                        // Debug.Log("Android CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                        if (anyPlatform) {
                            plug.SetCompatibleWithAnyPlatform(false);
                            plug.SetCompatibleWithEditor(false);
                            plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.Android, true);
                            plug.SetPlatformData(BuildTarget.Android, "CPU", "ARMv7");
                            refresh = true;
                        }
                    }
                    break;
                case "iOS":
                    if ("libVTextIOS.a" == fi.Name) {
                        anyPlatform = plug.GetCompatibleWithAnyPlatform();
                        // Debug.Log("iOS CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                        if (anyPlatform) {
                            plug.SetCompatibleWithAnyPlatform(false);
                            plug.SetCompatibleWithEditor(false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.Android, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.iOS, true);
                            refresh = true;
                        }
                    }
                    break;
                case "x86":
                    switch (fi.Name) {
                    case "VText.dll":
                        anyPlatform = plug.GetCompatibleWithAnyPlatform();
                        // Debug.Log("W32 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                        if (anyPlatform) {
                            plug.SetCompatibleWithAnyPlatform(false);
                            plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, true);
                            switch (Application.platform) {
                            case RuntimePlatform.WindowsEditor:
                                #if UNITY_EDITOR_32
                                plug.SetCompatibleWithEditor(true);
                                #else
                                plug.SetCompatibleWithEditor(false);
                                #endif
                                break;
                            default:
                                plug.SetCompatibleWithEditor(false);
                                break;
                            }
                            refresh = true;
                            // Debug.Log("W32 CPU " + plug.GetPlatformData(BuildTarget.StandaloneWindows, "CPU"));
                            plug.SetPlatformData(BuildTarget.StandaloneWindows, "CPU", "x86");
                        }
                        break;
                    case "libVText.so":
                        anyPlatform = plug.GetCompatibleWithAnyPlatform();
                        // Debug.Log("L32 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                        if (anyPlatform) {
                            plug.SetCompatibleWithAnyPlatform(false);
                            plug.SetCompatibleWithEditor(false);
                            plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, true);
                            // Debug.Log("L32 CPU " + plug.GetPlatformData(BuildTarget.StandaloneLinux, "CPU"));
                            plug.SetPlatformData(BuildTarget.StandaloneLinux, "CPU", "x86");
                            refresh = true;
                        }
                        break;
                    }
                    break;
                case "x86_64":
                    switch (fi.Name) {
                    case "VText.dll":
                        anyPlatform = plug.GetCompatibleWithAnyPlatform();
                        // Debug.Log("W64 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                        if (anyPlatform) {
                            plug.SetCompatibleWithAnyPlatform(false);
                            plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, true);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, true);
                            switch (Application.platform) {
                            case RuntimePlatform.WindowsEditor:
                                #if UNITY_EDITOR_32
                                plug.SetCompatibleWithEditor(false);
                                #else
                                plug.SetCompatibleWithEditor(true);
                                #endif
                                break;
                            default:
                                plug.SetCompatibleWithEditor(false);
                                break;
                            }
                            plug.SetPlatformData(BuildTarget.StandaloneLinux, "CPU", "x86_64");
                            refresh = true;
                        }
                        break;
                    case "libVText.so":
                        anyPlatform = plug.GetCompatibleWithAnyPlatform();
                        // Debug.Log("L64 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                        if (anyPlatform) {
                            plug.SetCompatibleWithAnyPlatform(false);
                            plug.SetCompatibleWithEditor(false);
                            plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                            plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, true);
                            plug.SetPlatformData(BuildTarget.StandaloneLinux, "CPU", "x86_64");
                            refresh = true;
                        }
                        break;
                    }
                    break;
                }
                if (refresh) {
                    plug.SaveAndReimport();
                }
            }
            return true;
        }
        errorMessage += "- Could <b>not</b> adjust the platform dependencies of the plugins.";
        return false;
    }
    #endregion // INSTALLATION

    #region INSTALL FONTS

    /// <summary>
    /// Draws the install fonts step.
    /// </summary>
    private void DrawInstallFontsStep() {
        DrawInstallationStepHeader(_installationStepToLabelMap[VTextInstallationSteps.InstallFonts]);

        _fontInstallationScrollViewPosition = GUILayout.BeginScrollView(_fontInstallationScrollViewPosition);
        string str = "All folders and plugins are installed. Now you can add some fonts which you want to use. \n" +
            "It is <b>highly</b> recommended that you have <b>at least one</b> font in the '<i>Assets/StreamingAssets/Fonts/</i>' folder.\n" +
            "For new VText objects we use the first font in that folder to render the text (you can change it after the creation of cause but we" +
            "need a default font.\n\n" +
            "For our <b>demo scenes</b> or if you don't have any fonts on hand you should install the following fonts.";
        GUILayout.Label(str, _currentInstallationStepTextStyle);
        GUILayout.Space(20);

        if (GUILayout.Button("Install demo fonts")) {
            InstallDemoFonts();
        }
        GUILayout.Space(20);
        str = "If you want to install some of your own fonts right now you can use the following button. <b><color=#23aacc>You can install additional " +
            "fonts easily later by copying them into the '<i>Assets/StreamingAssets/Fonts/</i>' folder</color></b>. There is no need to stop or restart Unity.";
        GUILayout.Label(str, _currentInstallationStepTextStyle);
        GUILayout.Space(20);
        if (GUILayout.Button("Install custom fonts")) {
            InstallCustomFonts();
        }
        GUILayout.Space(20);

        int fontCount = GetInstalledFontsCount();
        if (fontCount > 0) {
            str = string.Format("Actually you have <color=#23cc23>{0}</color> fonts installed.", GetInstalledFontsCount());
        } else {
            str = string.Format("Actually you have <color=#cc2323>{0}</color> fonts installed. You should install at least one font by one of the " +
                "buttons above.", GetInstalledFontsCount());
        }
        GUILayout.Label(str, _currentInstallationStepTextStyle);
        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();


        if (GUILayout.Button("Next")) {
            GotoNextInstallationStep();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Gets the virtence font folder ... relative to the folder which holds this editor window script
    /// </summary>
    /// <returns>The virtence font folder.</returns>
    private string GetVirtenceFontFolder() {
        MonoScript ms = MonoScript.FromScriptableObject(this);
        GUILayout.Label(string.Format("current vtext folder: <b><i>{0}</i></b>",AssetDatabase.GetAssetPath(ms)), _currentInstallationStepTextStyle);

        string vtextFolder = GetParentFolder(AssetDatabase.GetAssetPath(ms), "VText");
        if (vtextFolder != null) {
            string fontsPath = vtextFolder + "/Fonts";
            if (Directory.Exists(fontsPath)) {
                return (vtextFolder + "/Fonts");
            }
        }

        return null;
    }

    /// <summary>
    /// get the number of fonts in the Assets/StreamingAssets/Fonts folder
    /// </summary>
    /// <returns>The installed fonts count.</returns>
    private int GetInstalledFontsCount() {
        int fontCount = 0;

        DirectoryInfo dif = new DirectoryInfo(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"));
        if (dif.Exists) {
            FileInfo[] fiarray = dif.GetFiles("*.*");
            foreach (FileInfo fi in fiarray) {
                if (fi.Extension.ToUpper() == ".TTF" || fi.Extension.ToUpper() == ".OTF") {
                    fontCount++;
                }
            }
        }
        return fontCount;
    }

    /// <summary>
    /// install all fonts from VText/Fonts folder (esp. copy them to Assets/StreamingAssets/Fonts folder)
    /// </summary>
    private void InstallDemoFonts() {
        DirectoryInfo di = new  DirectoryInfo(GetVirtenceFontFolder());
        if (di.Exists) {
            FileInfo[] fiarray = di.GetFiles ("*.*");

            foreach (FileInfo fi in fiarray) {
                if (".TTF" == fi.Extension.ToUpper() || ".OTF" == fi.Extension.ToUpper()) {
                    string fileName = fi.FullName;
                    fileName = fileName.Replace('\\','/');
                    InstallFont(fileName);
                } 
            }

            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// install custom fonts
    /// </summary>
    private void InstallCustomFonts() {
        string fname = EditorUtility.OpenFilePanel("Font", GetVirtenceFontFolder(), "ttf,otf");
        if(fname != string.Empty)
        {
            InstallFont(fname);
            AssetDatabase.Refresh();
        }
    }
    /// <summary>
    /// copy the specified font to Assets/StreamingAssets/Fonts folder
    /// </summary>
    /// <param name="fontname">Fontname.</param>
    void InstallFont(string fontname)
    {
        try
        {
            FileInfo fi = new FileInfo(fontname);
            FileUtil.CopyFileOrDirectory(fontname, System.IO.Path.Combine(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"), fi.Name));
        }
        catch (IOException)
        {            
        }
    }
    #endregion // INSTALL FONTS

    #region FINISH
    /// <summary>
    /// Draws the welcome step.
    /// </summary>
    private void DrawFinishStep() {
        DrawInstallationStepHeader(_installationStepToLabelMap[VTextInstallationSteps.Finish]);

        _finishScrollViewPosition = GUILayout.BeginScrollView(_finishScrollViewPosition);

        string str = "You finished the installation process and now can use VText in your projects. \n\n" +
            "To add a new VText object to your scene just choose <i>'GameObject->Virtence->VText'</i>' from the menubar.\n\n"+
            "We wish you much fun with <b>VText</b> and if you have any questions just send an email to: \n" +
            EMAIL_STRING + "\n\n" +
            "Or visit our hompage at: \n" +
            HOMEPAGE_VIRTENCE_STRING + "\n\n" +
            "Or visit the VText homepage directly: \n" +
            HOMEPAGE_VTEXT_STRING;
        GUILayout.Label(str, _currentInstallationStepTextStyle);

        GUILayout.Space(10);

        float aspectRatio = (float) _finishLogo.height / _finishLogo.width;
        float desiredWidth = EditorGUIUtility.currentViewWidth * 0.4f;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        TextAnchor currentAlignment = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label(_finishLogo, GUILayout.Width(desiredWidth), GUILayout.Height(desiredWidth * aspectRatio));
        GUI.skin.label.alignment = currentAlignment;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Done")) {
            this.Close();
        }
        GUILayout.EndHorizontal();
    }
    #endregion // FINISH

    #endregion // METHODS
}
