#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MD_MeshPaint))]
public class MD_MeshPaint_Editor : Editor
{

    public Texture IMG_BrushSettings;
    public Texture IMG_LogicSettings;
    public Texture IMG_AppearanceSettings;

    SerializedProperty MP_Platform;
    SerializedProperty MP_Platform_Oculus;

    //INPUT
    //--PC
    SerializedProperty MP_INPUT_PC_MeshPaintInput;

    //BRUSH & MESH SETTINGS
    SerializedProperty MP_BrushSize, MP_ShapeType;

    SerializedProperty MP_SmoothBrushMovement;
    SerializedProperty MP_BSmoothMSpeed;
    SerializedProperty MP_SmoothBrushRotation;
    SerializedProperty MP_BSmoothRSpeed;

    SerializedProperty MP_DistanceLimitation;
    SerializedProperty MP_MinDistanceLimit;

    SerializedProperty MP_MeshPaintType;
    SerializedProperty MP_RotationMode, MP_RotationmodeOffset;

    SerializedProperty MP_ConnectMeshOnRelease;

    //---Type - screen
    SerializedProperty MP_TypeScreen_UseMainCamera;
    SerializedProperty MP_TypeScreen_TargetCamera;
    SerializedProperty MP_TypeScreen_Depth;
    //---Type - raycast
    SerializedProperty MP_TypeRaycast_RaycastFromCursor;
    SerializedProperty MP_TypeRaycast_RaycastOriginFORWARD;
    SerializedProperty MP_TypeRaycast_AllowedLayers;
    SerializedProperty MP_TypeRaycast_CastAllObjects;
    SerializedProperty MP_TypeRaycast_TagForRaycast;
    SerializedProperty MP_TypeRaycast_BrushOffset;
    SerializedProperty MP_TypeRaycast_IgnoreSelfCasting;
    //---Type - custom
    SerializedProperty MP_TypeCustom_DRAW;
    SerializedProperty MP_TypeCustom_DRAWStart;
    SerializedProperty MP_TypeCustom_CustomBrushTransform;
    SerializedProperty MP_TypeCustom_EnableSmartRotation;
    SerializedProperty MP_TypeCustom_BrushParent;

    //MESH APPEARANCE
    SerializedProperty MP_CurrentlySelectedAppearanceSlot;
    SerializedProperty MP_MaterialSlots;
    SerializedProperty MP_Color_AvailableMaterials;
    SerializedProperty MP_Color_AvailableColors;

    SerializedProperty MP_FollowBrushTransform;
    SerializedProperty MP_ObjectForFollowing, MP_HideCustomBrushIfNotRaycasting;

    SerializedProperty MP_RefreshMeshCollider;

    private void OnEnable()
    {
        MP_Platform = serializedObject.FindProperty("MP_Platform");
        MP_Platform_Oculus = serializedObject.FindProperty("MP_Platform_Oculus");
        MP_INPUT_PC_MeshPaintInput = serializedObject.FindProperty("MP_INPUT_PC_MeshPaintInput");
        MP_BrushSize = serializedObject.FindProperty("MP_BrushSize");
        MP_ShapeType = serializedObject.FindProperty("MP_ShapeType");
        MP_SmoothBrushMovement = serializedObject.FindProperty("MP_SmoothBrushMovement");
        MP_BSmoothMSpeed = serializedObject.FindProperty("MP_BSmoothMSpeed");
        MP_SmoothBrushRotation = serializedObject.FindProperty("MP_SmoothBrushRotation");
        MP_BSmoothRSpeed = serializedObject.FindProperty("MP_BSmoothRSpeed");
        MP_DistanceLimitation = serializedObject.FindProperty("MP_DistanceLimitation");
        MP_MinDistanceLimit = serializedObject.FindProperty("MP_MinDistanceLimit");
        MP_MeshPaintType= serializedObject.FindProperty("MP_MeshPaintType");
        MP_RotationMode = serializedObject.FindProperty("MP_RotationMode");
        MP_RotationmodeOffset = serializedObject.FindProperty("MP_RotationmodeOffset");
        MP_ConnectMeshOnRelease = serializedObject.FindProperty("MP_ConnectMeshOnRelease");
        MP_TypeScreen_UseMainCamera = serializedObject.FindProperty("MP_TypeScreen_UseMainCamera");
        MP_TypeScreen_TargetCamera = serializedObject.FindProperty("MP_TypeScreen_TargetCamera");
        MP_TypeScreen_Depth = serializedObject.FindProperty("MP_TypeScreen_Depth");
        MP_TypeRaycast_RaycastFromCursor = serializedObject.FindProperty("MP_TypeRaycast_RaycastFromCursor");
        MP_TypeRaycast_RaycastOriginFORWARD = serializedObject.FindProperty("MP_TypeRaycast_RaycastOriginFORWARD");
        MP_TypeRaycast_AllowedLayers = serializedObject.FindProperty("MP_TypeRaycast_AllowedLayers");
        MP_TypeRaycast_CastAllObjects = serializedObject.FindProperty("MP_TypeRaycast_CastAllObjects");
        MP_TypeRaycast_TagForRaycast = serializedObject.FindProperty("MP_TypeRaycast_TagForRaycast");
        MP_TypeRaycast_BrushOffset = serializedObject.FindProperty("MP_TypeRaycast_BrushOffset");
        MP_TypeRaycast_IgnoreSelfCasting = serializedObject.FindProperty("MP_TypeRaycast_IgnoreSelfCasting");
        MP_TypeCustom_DRAW = serializedObject.FindProperty("MP_TypeCustom_DRAW");
        MP_TypeCustom_CustomBrushTransform = serializedObject.FindProperty("MP_TypeCustom_CustomBrushTransform");
        MP_TypeCustom_EnableSmartRotation = serializedObject.FindProperty("MP_TypeCustom_EnableSmartRotation");
        MP_TypeCustom_BrushParent = serializedObject.FindProperty("MP_TypeCustom_BrushParent");
        MP_CurrentlySelectedAppearanceSlot = serializedObject.FindProperty("MP_CurrentlySelectedAppearanceSlot");
        MP_MaterialSlots = serializedObject.FindProperty("MP_MaterialSlots");
        MP_Color_AvailableMaterials = serializedObject.FindProperty("MP_Color_AvailableMaterials");
        MP_Color_AvailableColors = serializedObject.FindProperty("MP_Color_AvailableColors");
        MP_FollowBrushTransform = serializedObject.FindProperty("MP_FollowBrushTransform");
        MP_ObjectForFollowing = serializedObject.FindProperty("MP_ObjectForFollowing");
        MP_HideCustomBrushIfNotRaycasting = serializedObject.FindProperty("MP_HideCustomBrushIfNotRaycasting");
        MP_RefreshMeshCollider = serializedObject.FindProperty("MP_RefreshMeshCollider");
    }

    public override void OnInspectorGUI()
    {
        MD_MeshPaint mp = (MD_MeshPaint)target;

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");
        ppDraw_Property(MP_Platform, "Target Platform", "Please choose one of the target platforms");

        if (mp.MP_Platform == MD_MeshPaint.MP_PlatformInternal.PC)
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            ppDraw_Property(MP_INPUT_PC_MeshPaintInput, "Paint Input");
            GUILayout.EndVertical();
        }
        else if (mp.MP_Platform == MD_MeshPaint.MP_PlatformInternal.VR)
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            ppDraw_Property(MP_Platform_Oculus, "VR Platform Oculus?", "If enabled, target VR platform will be OCULUS, if disabled, target VR platform will be VIVE");
            ppDraw_Box("To customize VR Input, add Mesh Paint VR Input component on target controller.");
            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label(IMG_BrushSettings);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(MP_BrushSize, "Brush Size");

        GUILayout.Space(5);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(MP_SmoothBrushMovement, "Brush Smooth Movement");
        if (mp.MP_SmoothBrushMovement)
            ppDraw_Property(MP_BSmoothMSpeed, "Smooth Movement Speed");
        ppDraw_Property(MP_SmoothBrushRotation, "Brush Smooth Rotation");
        if (mp.MP_SmoothBrushRotation)
            ppDraw_Property(MP_BSmoothRSpeed, "Smooth Rotation Speed");
        GUILayout.EndVertical();
        GUILayout.Space(5);

        ppDraw_Property(MP_DistanceLimitation, "Distance Limitation", "If enabled, mesh will be refreshed & created after some values typed below");
        if (mp.MP_DistanceLimitation)
            ppDraw_Property(MP_MinDistanceLimit, "Minimal Distance Limit", "Minimal distance limit - how smooth will be the mesh");

        GUILayout.Space(5);

        ppDraw_Property(MP_ConnectMeshOnRelease, "Connect Mesh On Release", "Created mesh will be connected on release");

        GUILayout.Space(5);

        ppDraw_Property(MP_RotationMode, "Brush Rotation Mode", "Choose one of the rotation modes. Each mode is unique. One Axis - better for 2D drawing, Spatial Axis - better for 3D drawing");
        if (mp.MP_RotationMode == MD_MeshPaint.MP_RotationModeInternal.FollowOneAxis)
            ppDraw_Property(MP_RotationmodeOffset, "Rotation Offset","Additional rotation parameters [default: 0 0 1 = FORWARD]");

        GUILayout.Space(5);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(MP_ShapeType, "Shape Type", "Choose one of the shapes to draw");
        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label(IMG_LogicSettings);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(MP_MeshPaintType, "Mesh Painting Type", "Choose one of the mesh painting types.");
        if (mp.MP_MeshPaintType == MD_MeshPaint.MP_MeshPaintTypeInternal.DrawOnScreen)
        {
            GUILayout.Label("Type: On Screen");
            GUILayout.BeginVertical("Box");
            ppDraw_Property(MP_TypeScreen_UseMainCamera, "Use MainCamera", "If enabled, script will find Camera.main object [camera with tag MainCamera]. Otherwise you can choose your own camera");
            if (!mp.MP_TypeScreen_UseMainCamera)
                ppDraw_Property(MP_TypeScreen_TargetCamera, "Target Camera");
            ppDraw_Property(MP_TypeScreen_Depth, "Painting Depth", "Z Value [distance from camera]");
            GUILayout.EndVertical();
        }
        else if (mp.MP_MeshPaintType == MD_MeshPaint.MP_MeshPaintTypeInternal.DrawOnRaycastHit)
        {
            GUILayout.Label("Type: On Raycast Hit");
            GUILayout.BeginVertical("Box");

            ppDraw_Property(MP_TypeRaycast_AllowedLayers, "Allowed Layers");
            ppDraw_Property(MP_TypeRaycast_CastAllObjects, "Cast All Objects", "If enabled, all objects will receive raycast function");
            if (!mp.MP_TypeRaycast_CastAllObjects)
                ppDraw_Property(MP_TypeRaycast_TagForRaycast, "Tag For Raycast Objects");

            GUILayout.Space(5);

            ppDraw_Property(MP_TypeRaycast_RaycastFromCursor, "Raycast From Cursor", "If enabled, raycast origin will be set to cursor");
            if (!mp.MP_TypeRaycast_RaycastFromCursor)
                ppDraw_Property(MP_TypeRaycast_RaycastOriginFORWARD, "Raycast Origin [FORWARD Direction]", "Assign target direction for raycast [raycast direction will be this object FORWARD]");

            GUILayout.Space(5);

            ppDraw_Property(MP_TypeRaycast_BrushOffset, "Brush Offset");
            ppDraw_Property(MP_TypeRaycast_IgnoreSelfCasting, "Ignore Self Casting", "If enabled, raycast will ignore painted meshes");

            GUILayout.EndVertical();
        }
        else if (mp.MP_MeshPaintType == MD_MeshPaint.MP_MeshPaintTypeInternal.CustomDraw)
        {
            GUILayout.Label("Type: Custom");
            GUILayout.BeginVertical("Box");

            ppDraw_Property(MP_TypeCustom_DRAW, "PAINT", "If enabled, the script will start painting the mesh");
            ppDraw_Property(MP_TypeCustom_CustomBrushTransform, "Customize Brush Transform", "If enabled, you will be able to customize brush parent and its rotation behaviour");
            if (mp.MP_TypeCustom_CustomBrushTransform)
            {
                ppDraw_Property(MP_TypeCustom_EnableSmartRotation, "Smart Rotation", "If enabled, smart rotation will be allowed - brush will rotate to the direction of its movement");
                ppDraw_Property(MP_TypeCustom_BrushParent, "Brush Parent", "If you won't to parent brush, leave it empty. If the parent is assigned, brush will be automatically set to ZERO local position");
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label(IMG_AppearanceSettings);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(MP_CurrentlySelectedAppearanceSlot, "Appearance Index", "Index of the selected appearance slot - Material/ Color [according on the boolean below]");
        ppDraw_Property(MP_MaterialSlots, "Material Slots", "If enabled, color slots will be hidden");
        if (mp.MP_MaterialSlots)
            ppDraw_Property(MP_Color_AvailableMaterials, "Available Materials", "", true);
        else
            ppDraw_Property(MP_Color_AvailableColors, "Available Colors", "", true);

        GUILayout.Space(5);

        ppDraw_Property(MP_RefreshMeshCollider, "Add & Refresh Mesh Collider");

        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label("Additional");
        GUILayout.BeginVertical("Box");
        ppDraw_Property(MP_FollowBrushTransform, "Custom Brush Transform", "If enabled, you can assign your own custom brush to follow hidden brush");
        if (mp.MP_FollowBrushTransform)
        {
            ppDraw_Property(MP_ObjectForFollowing, "Custom Brush", "Custom brush that will follow hidden brush");
            if (mp.MP_MeshPaintType == MD_MeshPaint.MP_MeshPaintTypeInternal.DrawOnRaycastHit)
                ppDraw_Property(MP_HideCustomBrushIfNotRaycasting, "Hide Brush if not Raycasting", "Custom brush will be hidden if there is no raycast hit available");
        }

        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.Label("Presets");
        if (GUILayout.Button("2D Ready Preset"))
            SetTo2D();
        if (GUILayout.Button("3D Ready Preset"))
            SetTo3D();

        serializedObject.Update();
    }


    private void SetTo2D()
    {
        MD_MeshPaint mp = (MD_MeshPaint)target;

        mp.MP_RotationMode = MD_MeshPaint.MP_RotationModeInternal.FollowOneAxis;
        mp.MP_MeshPaintType = MD_MeshPaint.MP_MeshPaintTypeInternal.DrawOnScreen;
        mp.MP_TypeScreen_UseMainCamera = true;
        mp.MP_TypeScreen_Depth = 10;

        mp.MP_CurrentlySelectedAppearanceSlot = 0;
        mp.MP_MaterialSlots = false;
        mp.MP_Color_AvailableColors = new Color[] {Color.white,Color.black, Color.blue,Color.red,Color.yellow,Color.green,Color.cyan };

        mp.MP_RefreshMeshCollider = false;
    }
    private void SetTo3D()
    {
        MD_MeshPaint mp = (MD_MeshPaint)target;

        mp.MP_RotationMode = MD_MeshPaint.MP_RotationModeInternal.FollowSpatialAxis;
        mp.MP_MeshPaintType = MD_MeshPaint.MP_MeshPaintTypeInternal.DrawOnRaycastHit;
        mp.MP_TypeRaycast_AllowedLayers = 1;
        mp.MP_TypeRaycast_CastAllObjects = true;
        mp.MP_TypeRaycast_RaycastFromCursor = true;
        mp.MP_TypeRaycast_BrushOffset = new Vector3(0, 0.2f, 0);
        mp.MP_TypeRaycast_IgnoreSelfCasting = true;

        mp.MP_CurrentlySelectedAppearanceSlot = 0;
        mp.MP_MaterialSlots = false;
        mp.MP_Color_AvailableColors = new Color[] { Color.white, Color.black, Color.blue, Color.red, Color.yellow, Color.green, Color.cyan };

        mp.MP_RefreshMeshCollider = true;
    }



    private void ppDraw_Property(SerializedProperty p, string Text, string ToolTip = "", bool includeChilds = false)
    {
        EditorGUILayout.PropertyField(p, new GUIContent(Text, ToolTip), includeChilds, null);
        serializedObject.ApplyModifiedProperties();
    }
    private void ppDraw_Box(string Text, MessageType type = MessageType.Info)
    {
        EditorGUILayout.HelpBox(Text, type);
    }
}
#endif
