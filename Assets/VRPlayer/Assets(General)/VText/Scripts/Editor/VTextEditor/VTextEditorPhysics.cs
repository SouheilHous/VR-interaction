// ----------------------------------------------------------------------
// File: 		VTextEditorPhysics
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

/// <summary>
/// 
/// </summary>
public class VTextEditorPhysics : AbstractVTextEditorComponent
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS
    /// <summary>
    /// the width of the labels in front of controls
    /// </summary>
    private const float LABEL_WIDTH = 125;

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
    private AnimBool _showRigidBody = new AnimBool();                           // show or hide the rigid body parameters
    private AnimBool _showCollider = new AnimBool();                            // show or hide the collider parameters

    #region RIGIDBODY
    SerializedProperty _physics;                                                // the physics component of this vtext object
    SerializedProperty _createRigidbody;                                        // create a rigidbody?
    SerializedProperty _rigidbodyMass;                                          // the mass of the rigidbody
    SerializedProperty _rigidbodyDrag;                                          // the drag value of the rigidbody
    SerializedProperty _rigidbodyAngularDrag;                                   // the angular drag component of the rigidbody
    SerializedProperty _rigidbodyGravity;                                       // use gravity?
    SerializedProperty _rigidbodyIsKinematic;                                   // determines if the rigidbody is kinematic or not
    #endregion RIGIDBODY

    #region COLLIDER
    SerializedProperty _colliderIsTrigger;                                      // determines if the collider is setup as a trigger
    SerializedProperty _colliderIsConvex;                                       // determines if the collider is convex (only use for MeshCollider!!!)

    SerializedProperty _colliderType;                                           // the collider type (none, box, mesh-collider, etc)
    SerializedProperty _colliderMaterial;                                       // the physics material for this collider
    #endregion // COLLIDER
	
    #region INFOFIELDS
    private AnimBool _showRigidBodyInfo = new AnimBool();                       // show or hide the rigidbody info
    private AnimBool _showColliderInfo = new AnimBool();                        // show or hide the rigidbody info

    private Texture _rigidBodyInfoHelpImage;                                    // the image which is shown in the rigidbody help box
    private Texture _colliderInfoHelpImage;                                     // the image which is shown in the rigidbody help box

    private Vector2 _rigidBodyInfoHelpTextScrollPosition = Vector2.zero;        // the scrollview position for the rigidbody help text
    private Vector2 _colliderInfoHelpTextScrollPosition = Vector2.zero;         // the scrollview position for the collider help text
    #endregion // INFOFIELDS

    #endregion // FIELDS


	#region PROPERTIES

	#endregion // PROPERTIES


	#region CONSTRUCTORS

    public VTextEditorPhysics(SerializedObject obj, Editor currentEditor) 
	{
        _physics = obj.FindProperty("Physics");
        _createRigidbody = _physics.FindPropertyRelative("_createRigidBody");
        _rigidbodyMass = _physics.FindPropertyRelative("_rigidbodyMass");
        _rigidbodyDrag = _physics.FindPropertyRelative("_rigidbodyDrag");
        _rigidbodyAngularDrag = _physics.FindPropertyRelative("_rigidbodyAngularDrag");
        _rigidbodyGravity = _physics.FindPropertyRelative("_rigidbodyUseGravity");
        _rigidbodyIsKinematic = _physics.FindPropertyRelative("_rigidbodyIsKinematic");

        _colliderIsTrigger = _physics.FindPropertyRelative("_colliderIsTrigger");
        _colliderIsConvex =  _physics.FindPropertyRelative("_colliderIsConvex");
        _colliderType = _physics.FindPropertyRelative("_colliderType");
        _colliderMaterial = _physics.FindPropertyRelative("_colliderMaterial");

        // the images in the help screens
        _rigidBodyInfoHelpImage = Resources.Load("Images/Icons/Help/physics_rigidBody") as Texture;
        _colliderInfoHelpImage = Resources.Load("Images/Icons/Help/physics_collider") as Texture;

        // add repaints if the animated values are changed
        _showRigidBody.valueChanged.AddListener(currentEditor.Repaint);
        _showCollider.valueChanged.AddListener(currentEditor.Repaint);

        _showRigidBodyInfo.valueChanged.AddListener(currentEditor.Repaint);
        _showColliderInfo.valueChanged.AddListener(currentEditor.Repaint);
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
        bool updatePhysics = false;
        updatePhysics |= ShowRigidbodyEditor();
        updatePhysics |= ShowColliderEditor();
        return updatePhysics;
    }

    /// <summary>
    /// Shows the rigidbody editor.
    /// </summary>
    private bool ShowRigidbodyEditor() {
        EditorGUIUtility.labelWidth = LABEL_WIDTH;

        bool updatePhysics = false;

        GUILayout.BeginHorizontal();
        bool useRigidBody = EditorGUILayout.Toggle(new GUIContent ("RigidBody:", "Enable/Disable rigidbodies for letters"), _createRigidbody.boolValue);
        if (useRigidBody != _createRigidbody.boolValue) {
            _createRigidbody.boolValue = useRigidBody;
            updatePhysics = true;
        }

        if (_createRigidbody.boolValue != _showRigidBody.value) {
            _showRigidBody.target = _createRigidbody.boolValue;
        }
        VTextEditorGUIHelper.DrawHelpButton(ref _showRigidBodyInfo);
        GUILayout.EndHorizontal();

        if (EditorGUILayout.BeginFadeGroup(_showRigidBody.faded))
        {
            EditorGUI.indentLevel++;

            // mass
            GUILayout.BeginHorizontal();
            float mass = EditorGUILayout.FloatField(new GUIContent ("Mass:", "The mass of this rigidbody"), _rigidbodyMass.floatValue);
            if (mass != _rigidbodyMass.floatValue) {
                _rigidbodyMass.floatValue = mass;
                updatePhysics = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();

            // drag
            GUILayout.BeginHorizontal();
            float drag = EditorGUILayout.FloatField(new GUIContent ("Drag:", "The drag amount of this rigidbody"), _rigidbodyDrag.floatValue);
            if (drag != _rigidbodyDrag.floatValue) {
                _rigidbodyDrag.floatValue = drag;
                updatePhysics = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();

            // angular drag
            GUILayout.BeginHorizontal();
            float angularDrag = EditorGUILayout.FloatField(new GUIContent ("Angular drag:", "The angular drag amount of this rigidbody"), _rigidbodyAngularDrag.floatValue);
            if (angularDrag != _rigidbodyAngularDrag.floatValue) {
                _rigidbodyAngularDrag.floatValue = angularDrag;
                updatePhysics = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();

            // use gravity
            GUILayout.BeginHorizontal();
            bool gravity = EditorGUILayout.Toggle(new GUIContent ("Use gravity:", "Use gravity?"), _rigidbodyGravity.boolValue);
            if (gravity != _rigidbodyGravity.boolValue) {
                _rigidbodyGravity.boolValue = gravity;
                updatePhysics = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();

            // is kinematic
            GUILayout.BeginHorizontal();
            bool isKinematic = EditorGUILayout.Toggle(new GUIContent ("Is kinematic:", "Is this rigidbody kinematic?"), _rigidbodyIsKinematic.boolValue);
            if (isKinematic != _rigidbodyIsKinematic.boolValue) {
                _rigidbodyIsKinematic.boolValue = isKinematic;
                updatePhysics = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        if (EditorGUILayout.BeginFadeGroup(_showRigidBodyInfo.faded)) {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("RigidBody:") + "\n\n" +
                "This allows you to automatically add rigidbodies to each letter of the text. You can apply the most common parameters of Unity's rigidbodies as " +
                "there are: \n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Mass: ") + "The mass of the rigidbody.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Drag: ") + "The drag amount of the rigidbody.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Angular drag: ") + "The angular drag amount of the rigidbody.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Use gravity: ") + "Determines if gravity should be applied to the letter.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Is kinematic: ") + "Controls whether physics affects the rigidbody or not. " +
                    "If this value is set to <b>true</b> then <b>no</b> physics are applied to the letter.";
            
            DrawHelpWindow(_rigidBodyInfoHelpImage, txt, ref _rigidBodyInfoHelpTextScrollPosition, ref _showRigidBodyInfo);
        }
        EditorGUILayout.EndFadeGroup();
        return updatePhysics;
    }

    /// <summary>
    /// Shows the collider editor.
    /// </summary>
    private bool ShowColliderEditor() {
        bool updatePhysics = false;

        EditorGUILayout.BeginHorizontal ();
        int colliderTypeID = (int)(VTextPhysics.ColliderType)EditorGUILayout.EnumPopup(new GUIContent("Collider type:", "The type of the collider added to each letter"),
            (VTextPhysics.ColliderType)System.Enum.GetValues(typeof(VTextPhysics.ColliderType)).GetValue(_colliderType.enumValueIndex));
        if (_colliderType.enumValueIndex != colliderTypeID) {
            _colliderType.enumValueIndex = colliderTypeID;
            updatePhysics |= true;
        }

        if ((VTextPhysics.ColliderType)_colliderType.enumValueIndex != VTextPhysics.ColliderType.None && _showCollider.value == false) {
            _showCollider.target = true;
        } else if ((VTextPhysics.ColliderType)_colliderType.enumValueIndex == VTextPhysics.ColliderType.None && _showCollider.value == true) {
            _showCollider.target = false;
        }

        VTextEditorGUIHelper.DrawHelpButton(ref _showColliderInfo);
        EditorGUILayout.EndHorizontal ();

        if (EditorGUILayout.BeginFadeGroup(_showCollider.faded)) {
            EditorGUI.indentLevel++;

            if ((VTextPhysics.ColliderType)_colliderType.enumValueIndex == VTextPhysics.ColliderType.Box) {
                updatePhysics |= ShowBoxColliderParameters();
            } else if ((VTextPhysics.ColliderType)_colliderType.enumValueIndex == VTextPhysics.ColliderType.Mesh) {
                updatePhysics |= ShowMeshColliderParameters();
            }

            // physics material ... this will work for all colliders
            if ((VTextPhysics.ColliderType)_colliderType.enumValueIndex != VTextPhysics.ColliderType.None) {
                GUILayout.BeginHorizontal();
                PhysicMaterial colMat = EditorGUILayout.ObjectField("Physic material:", _colliderMaterial.objectReferenceValue, typeof(PhysicMaterial), false) as PhysicMaterial;
                if (colMat != _colliderMaterial.objectReferenceValue) {
                    _colliderMaterial.objectReferenceValue = colMat;
                    updatePhysics |= true;
                }
                GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        if (EditorGUILayout.BeginFadeGroup(_showColliderInfo.faded)) {
            string txt = VTextEditorGUIHelper.ConvertStringToHelpWindowHeader("Collider:") + "\n\n" +
                "Here you can specify a collider for each letter of your text. Actually the following types are supported:\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("None: ") + "Don't add a collider at all.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Box: ") + "Add a box collider to each letter. You can specify the <b>IsTrigger</b> " +
                "property of it. A trigger doesn't register a collision with an incoming Rigidbody. Instead, it sends OnTriggerEnter, OnTriggerExit and " +
                "OnTriggerStay message when a rigidbody enters or exits the trigger volume.\n\n" +
                VTextEditorGUIHelper.ConvertStringToHelpWindowListItem("Mesh: ") + "Add a mesh collider to each letter. Here you can specify if it is " +
                "<b>convex</b> or not. You can see convex mesh colliders as simplified colliders without holes or entrances. If (<b>and only if</b>) you " +
                "define the mesh collider to be convex then you can set its <b>IsTrigger</b> property as well. A trigger doesn't register a collision with " +
                "an incoming Rigidbody. Instead, it sends OnTriggerEnter, OnTriggerExit and OnTriggerStay message when a rigidbody enters or exits the " +
                "trigger volume. \n\n" +
                "If you have defined a collider you can also set the colliders <b>physics material</b> to change its physics behaviour.";

            DrawHelpWindow(_colliderInfoHelpImage, txt, ref _colliderInfoHelpTextScrollPosition, ref _showColliderInfo);
        }
        EditorGUILayout.EndFadeGroup();

        return updatePhysics;
    }

    /// <summary>
    /// Shows the box collider parameters.
    /// </summary>
    /// <returns><c>true</c>, if box collider parameters was shown, <c>false</c> otherwise.</returns>
    private bool ShowBoxColliderParameters() {
        bool updatePhysics = false;

        // is trigger
        GUILayout.BeginHorizontal();
        bool isTrigger = EditorGUILayout.Toggle(new GUIContent("Is trigger:", "Is this collider is a trigger?"), _colliderIsTrigger.boolValue);
        if (isTrigger != _colliderIsTrigger.boolValue) {
            _colliderIsTrigger.boolValue = isTrigger;
            updatePhysics = true;
        }

        GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
        GUILayout.EndHorizontal();

        return updatePhysics;
    }

    /// <summary>
    /// Shows the mesh collider parameters.
    /// </summary>
    /// <returns><c>true</c>, if mesh collider parameters was shown, <c>false</c> otherwise.</returns>
    private bool ShowMeshColliderParameters() {
        bool updatePhysics = false;

        // is convex
        GUILayout.BeginHorizontal();
        bool isConvex = EditorGUILayout.Toggle(new GUIContent("Is convex:", "Should this mesh collider be convex?"), _colliderIsConvex.boolValue);
        if (isConvex != _colliderIsConvex.boolValue) {
            _colliderIsConvex.boolValue = isConvex;
            updatePhysics = true;
        }
        GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
        GUILayout.EndHorizontal();

        if (_colliderIsConvex.boolValue) {
            // is trigger
            GUILayout.BeginHorizontal();
            bool isTrigger = EditorGUILayout.Toggle(new GUIContent("Is trigger:", "Is this collider is a trigger?"), _colliderIsTrigger.boolValue);
            if (isTrigger != _colliderIsTrigger.boolValue) {
                _colliderIsTrigger.boolValue = isTrigger;
                updatePhysics = true;
            }
            GUILayout.Space(VTextEditorGUIHelper.HELP_BUTTON_WIDTH);
            GUILayout.EndHorizontal();
        } else {
            _colliderIsTrigger.boolValue = false;
            updatePhysics = true;
        }

        return updatePhysics;
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
