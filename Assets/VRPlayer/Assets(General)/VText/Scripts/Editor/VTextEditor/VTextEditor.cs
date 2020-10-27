/*
 * $Id: VTextEditor.cs 174 2015-03-16 09:55:03Z dirk $
 * 
 * Virtence VFont package
 * Copyright 2014 .. 2016 by Virtence GmbH
 * http://www.virtence.com
 * 
 */


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;


#if UNITY_5
using UnityEngine.Rendering;
#endif

[CustomEditor (typeof (VTextInterface))] 
[CanEditMultipleObjects]
public class VTextEditor : Editor {
    #region CONSTANTS
    private float TEXT_INPUT_SCROLLVIEW_HEIGHT = 90;    // the height of the scrollview in which we enter the text to render
    #endregion // CONSTANTS

	#region FIELDS
	private static bool _setupDefaultFont;				// determines if we should setup a default font ... this happens normally after a new VText object is created


    private AbstractVTextEditorComponent _header;                       // the header UI (mainly the logo, etc)
    private AbstractVTextEditorComponent _toolbar;                      // the toolbar UI (select different categories
    private AbstractVTextEditorComponent _style;                        // the style parameters (materials, font, etc)
    private AbstractVTextEditorComponent _mesh;                         // the mesh parameters (tesselation quality, bevel, shadows, backfaces, tangents, etc)
    private AbstractVTextEditorComponent _layout;                       // the layout parameters, bending, etc
    private AbstractVTextEditorComponent _physics;                      // the physics parameters (collider, rigidbodies, etc) for each glyph
    private AbstractVTextEditorComponent _additionalMonoBehaviours;     // additional monobehaviours added to each glyph

    private AnimBool _showStyle = new AnimBool();                       // show style parameters like font or materials
    private AnimBool _showMesh = new AnimBool();                        // show mesh parameters like depth, bevel, etc
    private AnimBool _showLayout = new AnimBool();                      // show layout parameters like alignment, orientation, etc
    private AnimBool _showPhysics = new AnimBool();                     // show or hide the "physics" foldout
    private AnimBool _showMonoBehaviours = new AnimBool();              // show the additional components for each character

	private Vector2 _textScrollViewPosition = new Vector2(0f,0f);       // the position of the scrollbars of the text input scrollview

	#endregion // FIELDS

    #region METHODS
	[MenuItem ("GameObject/Virtence/VText")]
	static void Create() {
		GameObject go = new GameObject("VText");
		VTextInterface s = go.AddComponent<VTextInterface>();

		_setupDefaultFont = true;

		s.Rebuild();
		Selection.activeGameObject = go;
	}

    void OnEnable() {       
        _header = new VTextEditorHeader(null, this);
        _style = new VTextEditorStyle(serializedObject, this);
        if (_setupDefaultFont) {
            (_style as VTextEditorStyle).SetupDefaultFont();
            _setupDefaultFont = false;
        }

        _toolbar = new VTextEditorToolbar(null, this);
        _mesh = new VTextEditorMesh(serializedObject, this);
        _layout = new VTextEditorLayout(serializedObject, this);
        _physics = new VTextEditorPhysics(serializedObject, this);
        _additionalMonoBehaviours = new VTextEditorAdditionalComponents(serializedObject, this);

        _showStyle.valueChanged.AddListener(Repaint);
        _showMesh.valueChanged.AddListener(Repaint);
        _showLayout.valueChanged.AddListener(Repaint);
        _showPhysics.valueChanged.AddListener(Repaint);
        _showMonoBehaviours.valueChanged.AddListener(Repaint);

        Undo.undoRedoPerformed -= OnUndoRedo;
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    public override void OnInspectorGUI () { 
        // which parts of the text should be updated
        bool updateMesh = false;
        bool updateLayout = false;
        bool updatePhysics = false;
        bool updateAdditionalComponents = false;

        // draw the header
        if (_header != null) {
            _header.DrawUI();
        }

        serializedObject.Update();

		// the text which should be generated
        SerializedProperty rtext = serializedObject.FindProperty("RenderText");

        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Enter your text here:");
        _textScrollViewPosition = EditorGUILayout.BeginScrollView(_textScrollViewPosition, GUILayout.Height(TEXT_INPUT_SCROLLVIEW_HEIGHT));
        string rt = EditorGUILayout.TextArea(rtext.stringValue, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        if(rt != rtext.stringValue) {
			rtext.stringValue = rt;
			updateMesh = true;
		}

        #region TOOLBAR
        EditorGUILayout.Space();
        _toolbar.DrawUI();
        EditorGUILayout.Space();
        _showStyle.target = (_toolbar as VTextEditorToolbar).CurrentToolbarValue == VTextEditorTools.Style;
        _showMesh.target = (_toolbar as VTextEditorToolbar).CurrentToolbarValue == VTextEditorTools.Mesh;
        _showLayout.target = (_toolbar as VTextEditorToolbar).CurrentToolbarValue == VTextEditorTools.Layout;
        _showPhysics.target = (_toolbar as VTextEditorToolbar).CurrentToolbarValue == VTextEditorTools.Physics;
        _showMonoBehaviours.target = (_toolbar as VTextEditorToolbar).CurrentToolbarValue == VTextEditorTools.Scripts;

        #endregion // TOOLBAR

        #region STYLE AND FONT PARAMETERS
        if (EditorGUILayout.BeginFadeGroup(_showStyle.faded))
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            updateMesh |= _style.DrawUI();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();

        #endregion // STYLE AND FONT PARAMETERS

        #region MESH PARAMETERS
        if (EditorGUILayout.BeginFadeGroup(_showMesh.faded)) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            updateMesh |= _mesh.DrawUI();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // MESH PARAMETERS

        #region LAYOUT
		// Layout
        if (EditorGUILayout.BeginFadeGroup(_showLayout.faded)) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            updateLayout |= _layout.DrawUI();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
		}
        EditorGUILayout.EndFadeGroup();
        #endregion // LAYOUT

        #region PHYSICS
        if (EditorGUILayout.BeginFadeGroup(_showPhysics.faded)) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            updatePhysics |= _physics.DrawUI();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // PHYSICS

        #region ADDITIONAL COMPONENTS
        if (EditorGUILayout.BeginFadeGroup(_showMonoBehaviours.faded)) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            updateAdditionalComponents |= _additionalMonoBehaviours.DrawUI();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();
        #endregion // ADDITIONAL COMPONENTS

        #region FOOTER
        EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal ();
		// Rebuild mesh when user click the Rebuild button
        Color c = GUI.backgroundColor;
        GUI.backgroundColor = new Color(1f, 0.35f, 0f);
        if (GUILayout.Button(new GUIContent("Rebuild", "Completely rebuild this text"))) {
            (target as VTextInterface).Rebuild();
		}
        GUI.backgroundColor = c;
		EditorGUILayout.EndHorizontal ();
        #endregion // FOOTER

		if(serializedObject.ApplyModifiedProperties()) {			
            (target as VTextInterface).CheckRebuild(updateMesh, updateLayout, updatePhysics, updateAdditionalComponents);

            foreach (Object modifiedObject in targets) {
                (modifiedObject as VTextInterface).CheckRebuild(updateMesh, updateLayout, updatePhysics, updateAdditionalComponents);

                if (updateMesh || updateLayout || updatePhysics || updateAdditionalComponents) {
                    if (!Application.isPlaying) {
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }
                }
            }
		}
	}
    #endregion // METHODS

    #region EVENTHANDLERS
    /// <summary>
    /// this is called if an undo or redo operation was performed
    /// </summary>
    void OnUndoRedo() {
        foreach (Object modifiedObject in targets) {
            (modifiedObject as VTextInterface).Rebuild(true);
        }
    }
    #endregion //EVENTHANDLERS
}
