using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_SculptingPro))]
public class SculptingPro_Editor : ModifiersEditorController
{

    SerializedProperty SS_InEditMode, SS_AtRuntime, SS_MobileSupport;

    SerializedProperty SS_UseBrushProjection;
    SerializedProperty SS_BrushProjection;

    SerializedProperty SS_BrushSize, SS_BrushStrength;

    SerializedProperty SS_MultithreadingSupported,SS_MultithreadingProcessDelay;

    SerializedProperty SS_State, SS_MeshSculptMode;
    SerializedProperty SS_CustomDirection, SS_UpdateColliderAfterRelease, SS_CustomDirectionObject, SS_CustomDirObjDirection;
    SerializedProperty SS_SculptingType;

    SerializedProperty SS_UseInput;
    SerializedProperty SS_UseRaiseFunct, SS_UseLowerFunct, SS_UseRevertFunct, SS_UseNoiseFunct;

    SerializedProperty SS_SculptingRaiseInput;
    SerializedProperty SS_SculptingLowerInput;
    SerializedProperty SS_SculptingRevertInput;
    SerializedProperty SS_SculptingNoiseInput;

    SerializedProperty SS_NoiseFunctionDirections;

    SerializedProperty SS_SculptFromCursor;
    SerializedProperty SS_SculptOrigin;

    SerializedProperty SS_EnableHeightLimitations;
    SerializedProperty SS_HeightLimitations;

    MDM_SculptingPro ss;

    private void OnEnable()
    {
        SS_AtRuntime = serializedObject.FindProperty("SS_AtRuntime");
        SS_InEditMode = serializedObject.FindProperty("SS_InEditMode");
        SS_MobileSupport = serializedObject.FindProperty("SS_MobileSupport");

        SS_UseBrushProjection = serializedObject.FindProperty("SS_UseBrushProjection");
        SS_BrushProjection = serializedObject.FindProperty("SS_BrushProjection");

        SS_BrushSize = serializedObject.FindProperty("SS_BrushSize");
        SS_BrushStrength = serializedObject.FindProperty("SS_BrushStrength");

        SS_MultithreadingSupported = serializedObject.FindProperty("SS_MultithreadingSupported");
        SS_MultithreadingProcessDelay = serializedObject.FindProperty("SS_MultithreadingProcessDelay");

        SS_State = serializedObject.FindProperty("SS_State");
        SS_MeshSculptMode = serializedObject.FindProperty("SS_MeshSculptMode");

        SS_CustomDirection = serializedObject.FindProperty("SS_CustomDirection");
        SS_UpdateColliderAfterRelease = serializedObject.FindProperty("SS_UpdateColliderAfterRelease");
        SS_CustomDirectionObject = serializedObject.FindProperty("SS_CustomDirectionObject");
        SS_CustomDirObjDirection = serializedObject.FindProperty("SS_CustomDirObjDirection");

        SS_SculptingType = serializedObject.FindProperty("SS_SculptingType");

        SS_UseInput = serializedObject.FindProperty("SS_UseInput");

        SS_UseRaiseFunct = serializedObject.FindProperty("SS_UseRaiseFunct");
        SS_UseLowerFunct = serializedObject.FindProperty("SS_UseLowerFunct");
        SS_UseRevertFunct = serializedObject.FindProperty("SS_UseRevertFunct");
        SS_UseNoiseFunct = serializedObject.FindProperty("SS_UseNoiseFunct");

        SS_SculptingRaiseInput = serializedObject.FindProperty("SS_SculptingRaiseInput");
        SS_SculptingLowerInput = serializedObject.FindProperty("SS_SculptingLowerInput");
        SS_SculptingRevertInput = serializedObject.FindProperty("SS_SculptingRevertInput");
        SS_SculptingNoiseInput = serializedObject.FindProperty("SS_SculptingNoiseInput");
        SS_NoiseFunctionDirections = serializedObject.FindProperty("SS_NoiseFunctionDirections");

        SS_SculptFromCursor = serializedObject.FindProperty("SS_SculptFromCursor");
        SS_SculptOrigin = serializedObject.FindProperty("SS_SculptOrigin");

        SS_EnableHeightLimitations = serializedObject.FindProperty("SS_EnableHeightLimitations");
        SS_HeightLimitations = serializedObject.FindProperty("SS_HeightLimitations");

        ss = (MDM_SculptingPro)target;

        SculptingPro_Window.B_EditMode = ss.SS_InEditMode;
        SculptingPro_Window.B_Radius = ss.SS_BrushSize;
        SculptingPro_Window.B_Strength = ss.SS_BrushStrength;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(15);
        GUILayout.BeginVertical("Box");
        DrawProperty(SS_AtRuntime, "Sculpting At Runtime", "If enabled, the script will work only at runtime");
        GUILayout.Space(5);
        DrawProperty(SS_MobileSupport, "Sculpting Editor For Mobile", "If enabled, the system will work only for mobile devices");
        GUILayout.EndVertical();

        if (!ss.SS_AtRuntime)
        {
            GUILayout.BeginVertical("Box");
            DrawProperty(SS_InEditMode, "In Edit Mode", "If enabled, the selection will be locked to the object and you are free to sculpt the mesh");
            EditorGUILayout.HelpBox("Left Mouse: Raise, Shift+Left Mouse: Lower, Control+Left Mouse: Revert, R: Restart Mesh", MessageType.Info);
            GUILayout.EndVertical();
        }
        GUILayout.Space(3);
        DrawProperty(SS_State, "Brush State");
        if (ss.SS_UseInput && !ss.SS_MultithreadingSupported)
            DrawProperty(SS_UpdateColliderAfterRelease, "Update Collider After Release", "If disabled, the collider will be updated every frame if the mouse is down");
        else if (ss.SS_MultithreadingSupported && !ss.SS_UpdateColliderAfterRelease)
            ss.SS_UpdateColliderAfterRelease = true;

        GUILayout.Space(5);

        GUILayout.BeginVertical("Box");
        DrawProperty(SS_MultithreadingSupported, "Multithreading Supported", "If enabled, the sculpting system will be ready for advanced & complex meshes. See more 'Multithreading' in docs.");
        if(ss.SS_MultithreadingSupported)
           DrawProperty(SS_MultithreadingProcessDelay,"Process Delay [millisec]","Multithreading process delay [default: 10] - the bigger number is, the smoother result should be.");
        if(ss.SS_MultithreadingSupported)
        {
            if (GUILayout.Button(new GUIContent("Fix Brush Incorrection","If the multithreading method is enabled, sculpting brush might be in another scale-ratio. This could be fixed after clicking this button. If not, you must adjust brush scale by yourself [Create empty object, group empty object with target brush graphic and assign empty object to the Brush Projection].")))
            {
                if(EditorUtility.DisplayDialog("Info","You can choose between 2 methods\n1. Would you like to reset the mesh matrix transform? This will set the current transform scale to ONE but it could take more than 1 minute.\n\n2. Adjust child brush scale if it's possible. This will adjust child brush scale to 0.1.","I'll choose 1","I'll choose 2"))
                ss.SS_Funct_BakeMesh();
                else
                {
                    if(ss.SS_BrushProjection.transform.childCount>0)
                    {
                        foreach (Transform t in ss.SS_BrushProjection.transform)
                            t.transform.localScale = Vector3.one * 0.1f;
                    }
                }
            }
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");
        DrawProperty(SS_UseBrushProjection, "Show Brush Projection","The brush projection will be disabled only If the brush projection contains Mesh Renderer!");
        DrawProperty(SS_BrushProjection, "Brush Projection");

        DrawProperty(SS_BrushSize, "Brush Size");
        DrawProperty(SS_BrushStrength, "Brush Strength");
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");
        DrawProperty(SS_MeshSculptMode, "Sculpt Mode");
        if (ss.SS_MeshSculptMode == MDM_SculptingPro.SS_MeshSculptMode_Internal.CustomDirection)
            DrawProperty(SS_CustomDirection, "Custom Direction");
        else if (ss.SS_MeshSculptMode == MDM_SculptingPro.SS_MeshSculptMode_Internal.CustomDirectionObject)
        {
            DrawProperty(SS_CustomDirectionObject, "Custom Direction Object");
            DrawProperty(SS_CustomDirObjDirection, "Direction Towards Object");
        }
        GUILayout.Space(5);
        DrawProperty(SS_EnableHeightLimitations, "Height Limitations","If enabled, you will be able to set vertices Y Limit [height]");
        if(ss.SS_EnableHeightLimitations)
            DrawProperty(SS_HeightLimitations, "Height Limitations", "Minimum[X] and Maximum[Y] height limitation [In local space]");
        GUILayout.Space(5);
        DrawProperty(SS_SculptingType, "Sculpting Type", "Choose a sculpting type.");
        GUILayout.EndVertical();

        if (ss.SS_AtRuntime && !ss.SS_MobileSupport)
        {
            GUILayout.Space(10);

            GUILayout.BeginVertical("Box");
            DrawProperty(SS_UseInput, "Use Input","Use custom sculpt input controls. Otherwise, you can use internal API functions to interact the mesh sculpt.");
            if(ss.SS_UseInput)
            {
                GUILayout.BeginVertical("Box");
                DrawProperty(SS_UseRaiseFunct, "Use Raise", "Use Raise sculpting function");
                DrawProperty(SS_UseLowerFunct, "Use Lower", "Use Lower sculpting function");
                DrawProperty(SS_UseRevertFunct, "Use Revert", "Use Revert sculpting function");
                DrawProperty(SS_UseNoiseFunct, "Use Noise", "Use Noise sculpting function");
                if (ss.SS_UseNoiseFunct)
                {
                    GUILayout.BeginVertical("Box");
                    DrawProperty(SS_NoiseFunctionDirections, "Noise Direction", "Choose a noise direction");
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GUILayout.Space(2);
                if (ss.SS_UseRaiseFunct)
                    DrawProperty(SS_SculptingRaiseInput, "Raise Input");
                if (ss.SS_UseLowerFunct)
                    DrawProperty(SS_SculptingLowerInput, "Lower Input");
                if (ss.SS_UseRevertFunct)
                    DrawProperty(SS_SculptingRevertInput, "Revert Input");
                if (ss.SS_UseNoiseFunct)
                    DrawProperty(SS_SculptingNoiseInput, "Noise Input");

                GUILayout.Space(5);
                DrawProperty(SS_SculptFromCursor, "Sculpt Origin Is Cursor","If enabled, the raycast origin will be cursor");
                GUILayout.Space(3);
                if (!ss.SS_SculptFromCursor)
                DrawProperty(SS_SculptOrigin, "Sculpt Origin Object", "The raycast origin");
            }
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Show Editor Tool Window"))
                SculptingPro_Window.Init();
            GUILayout.Space(10);
        }

        SculptingPro_Window.ssSource = ss;

        if (ss != null)
            serializedObject.Update();

        ppAddMeshColliderRefresher(ss);
        ppBackToMeshEditor(ss);
       
    }

    private void DrawProperty(SerializedProperty p, string Text, string Tooltip = "", bool childs = false)
    {
        EditorGUILayout.PropertyField(p, new GUIContent(Text, Tooltip), childs);
        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        if (ss.SS_BrushProjection == null || !ss.SS_InEditMode || ss.SS_AtRuntime)
        {
            if (!Application.isPlaying)
                ss.CheckForThread(false);
            return;
        }
        if (ss.SS_BrushProjection.GetComponent<Collider>())
            DestroyImmediate(ss.SS_BrushProjection.GetComponent<Collider>());

        if (!Application.isPlaying)
        {
            if (!ss.SS_MultithreadingSupported)
                ss.CheckForThread(false);
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit = new RaycastHit();
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Tools.current = Tool.None;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == ss.transform.GetComponent<Collider>())
            {
                if (!ss.SS_UseBrushProjection)
                {
                    if (ss.SS_BrushProjection.activeInHierarchy)
                        ss.SS_BrushProjection.SetActive(false);
                }
                else
                ss.SS_BrushProjection.SetActive(true);
                ss.SS_BrushProjection.transform.position = hit.point;
                ss.SS_BrushProjection.transform.rotation = Quaternion.FromToRotation(-Vector3.forward,hit.normal);
                ss.SS_BrushProjection.transform.localScale = new Vector3(ss.SS_BrushSize, ss.SS_BrushSize, ss.SS_BrushSize);

                switch (ss.SS_State)
                {
                    case MDM_SculptingPro.SS_State_Internal.Raise:
                        ss.SS_Funct_DoSculpting(hit.point, ss.SS_BrushProjection.transform.forward, ss.SS_BrushSize, ss.SS_BrushStrength, MDM_SculptingPro.SS_State_Internal.Raise);
                        if (!ss.SS_UpdateColliderAfterRelease)
                            ss.SS_Funct_RefreshMeshCollider();
                        break;
                    case MDM_SculptingPro.SS_State_Internal.Lower:
                        ss.SS_Funct_DoSculpting(hit.point, ss.SS_BrushProjection.transform.forward, ss.SS_BrushSize, ss.SS_BrushStrength, MDM_SculptingPro.SS_State_Internal.Lower);
                        if (!ss.SS_UpdateColliderAfterRelease)
                            ss.SS_Funct_RefreshMeshCollider();
                        break;


                    case MDM_SculptingPro.SS_State_Internal.Revert:
                        ss.SS_Funct_DoSculpting(hit.point, ss.SS_BrushProjection.transform.forward, ss.SS_BrushSize, ss.SS_BrushStrength, MDM_SculptingPro.SS_State_Internal.Revert);
                        if (!ss.SS_UpdateColliderAfterRelease)
                            ss.SS_Funct_RefreshMeshCollider();
                        break;
                }
            }
            else
                ss.SS_BrushProjection.SetActive(false);
        }
        else
            ss.SS_BrushProjection.SetActive(false);



        #region Editor Hotkeys
        if (Application.isPlaying)
            return;
        if (ss.SS_InEditMode)
        {
            //---Mouse
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt)
            {
                if (!Event.current.control)
                {
                    if (!Event.current.shift)
                        ss.SS_State = MDM_SculptingPro.SS_State_Internal.Raise;
                    else
                        ss.SS_State = MDM_SculptingPro.SS_State_Internal.Lower;
                }
                else
                    ss.SS_State = MDM_SculptingPro.SS_State_Internal.Revert;
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                ss.SS_State = MDM_SculptingPro.SS_State_Internal.None;
                ss.SS_Funct_RefreshMeshCollider();
                if (!Application.isPlaying)
                {
                    if (ss.SS_MultithreadingSupported)
                        ss.CheckForThread();
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                ss.SS_State = MDM_SculptingPro.SS_State_Internal.None;
                ss.SS_Funct_RefreshMeshCollider();
            }

            //---Keys
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.R:
                        ss.SS_Funct_RestoreOriginal();
                        break;
                }
            }
        }

        #endregion
    }
}
