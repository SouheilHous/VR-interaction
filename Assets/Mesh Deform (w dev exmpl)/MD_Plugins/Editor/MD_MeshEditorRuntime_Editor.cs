using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MD_MeshEditorRuntime))]
[CanEditMultipleObjects]
public class MD_MeshEditorRuntime_Editor : Editor
{
    SerializedProperty ppMobileSuppored;

    SerializedProperty ppVertexControlMode;

    SerializedProperty ppHoldingEffect;
    SerializedProperty ppMakeInterativePoint;
    SerializedProperty ppAttachToSender;

    SerializedProperty ppEffectRate;
    SerializedProperty ppEnableRigidbodyAfterHit;

    SerializedProperty ppVertexSpeed;

    SerializedProperty ppRaycastSpecificPoints;
    SerializedProperty ppSpecialTag;

    SerializedProperty ppInput;

    SerializedProperty ppCursorIsOrigin;
    SerializedProperty ppLockAndHideCursor;

    SerializedProperty ppPointRay;
    SerializedProperty ppSphericalRayRadius;

    SerializedProperty ppShowGraphic,ppGraphic_Size;

    SerializedProperty ppLockAxis_X, ppLockAxis_Y, ppLockAxis_Z;

    SerializedProperty pp_SculptingRuntimeFunctions, ppInput_Sculpt_Raise, ppInput_Sculpt_Lower, ppInput_Sculpt_Revert, ppSculpt_Radius, ppSculpt_Strength;


    SerializedProperty ppAXIS_EDITOR;

    SerializedProperty ppAXIS_AxisObject;

    SerializedProperty ppAXIS_TargetObject;

    SerializedProperty ppAXIS_SelectionInput, ppAXIS_AddPointsInput, ppAXIS_RemovePointsInput;

    SerializedProperty ppAXIS_LocalSpace, ppAXIS_Speed;
    SerializedProperty ppAXIS_SelectedPointColor, ppAXIS_SelectionGridColor;

    private void OnEnable()
    {
        ppAXIS_EDITOR = serializedObject.FindProperty("ppAXIS_EDITOR");

        ppAXIS_AxisObject = serializedObject.FindProperty("ppAXIS_AxisObject");

        ppAXIS_TargetObject = serializedObject.FindProperty("ppAXIS_TargetObject");

        ppAXIS_SelectionInput = serializedObject.FindProperty("ppAXIS_SelectionInput");
        ppAXIS_RemovePointsInput = serializedObject.FindProperty("ppAXIS_RemovePointsInput");
        ppAXIS_AddPointsInput = serializedObject.FindProperty("ppAXIS_AddPointsInput");

        ppAXIS_LocalSpace = serializedObject.FindProperty("ppAXIS_LocalSpace");
        ppAXIS_Speed = serializedObject.FindProperty("ppAXIS_Speed");
        ppAXIS_AddPointsInput = serializedObject.FindProperty("ppAXIS_AddPointsInput");
        ppAXIS_SelectionGridColor = serializedObject.FindProperty("ppAXIS_SelectionGridColor");
        ppAXIS_SelectedPointColor = serializedObject.FindProperty("ppAXIS_SelectedPointColor");

        ppVertexControlMode = serializedObject.FindProperty("ppVertexControlMode");

        ppMobileSuppored = serializedObject.FindProperty("ppMobileSuppored");

        ppHoldingEffect = serializedObject.FindProperty("ppHoldingEffect");
        ppMakeInterativePoint = serializedObject.FindProperty("ppMakeInterativePoint");
        ppAttachToSender = serializedObject.FindProperty("ppAttachToSender");

        ppEffectRate = serializedObject.FindProperty("ppEffectRate");
        ppEnableRigidbodyAfterHit = serializedObject.FindProperty("ppEnableRigidbodyAfterHit");

        ppVertexSpeed = serializedObject.FindProperty("ppVertexSpeed");

        ppRaycastSpecificPoints = serializedObject.FindProperty("ppRaycastSpecificPoints");
        ppSpecialTag = serializedObject.FindProperty("ppSpecialTag");

        ppInput = serializedObject.FindProperty("ppInput");

        ppCursorIsOrigin = serializedObject.FindProperty("ppCursorIsOrigin");
        ppLockAndHideCursor = serializedObject.FindProperty("ppLockAndHideCursor");

        ppPointRay = serializedObject.FindProperty("ppPointRay");
        ppSphericalRayRadius = serializedObject.FindProperty("ppSphericalRayRadius");

        ppShowGraphic = serializedObject.FindProperty("ppShowGraphic");
        ppGraphic_Size = serializedObject.FindProperty("ppGraphic_Size");

        ppLockAxis_X = serializedObject.FindProperty("ppLockAxis_X");
        ppLockAxis_Y = serializedObject.FindProperty("ppLockAxis_Y");
        ppLockAxis_Z = serializedObject.FindProperty("ppLockAxis_Z");

        pp_SculptingRuntimeFunctions = serializedObject.FindProperty("pp_SculptingRuntimeFunctions");
        ppInput_Sculpt_Raise = serializedObject.FindProperty("ppInput_Sculpt_Raise");
        ppInput_Sculpt_Lower = serializedObject.FindProperty("ppInput_Sculpt_Lower");
        ppInput_Sculpt_Revert = serializedObject.FindProperty("ppInput_Sculpt_Revert");
        ppSculpt_Radius = serializedObject.FindProperty("ppSculpt_Radius");
        ppSculpt_Strength = serializedObject.FindProperty("ppSculpt_Strength");
    }

    public override void OnInspectorGUI()
    {
        MD_MeshEditorRuntime m = (MD_MeshEditorRuntime)target;

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");

        ppDraw_Property(ppAXIS_EDITOR, "Axis Editor Mode", "If enabled, the script will be set to the AXIS EDITOR");

        GUILayout.EndVertical();


        if (m.ppAXIS_EDITOR)
        {
            GUILayout.Space(10);

            GUILayout.BeginVertical("Box");

            ppDraw_Property(ppAXIS_TargetObject, "Target Object", "Required target object to edit");
            GUILayout.Space(5);
            ppDraw_Property(ppAXIS_AxisObject, "Editor Axis Object", "Required 'Movable' Axis object for Axis Editor");
            GUIStyle st = new GUIStyle();
            st.normal.textColor = Color.gray;
            st.richText = true;
            st.fontSize = 10;
            st.fontStyle = FontStyle.Italic;
            if (m.ppAXIS_AxisObject != null)
                GUILayout.Label("Required axis child naming: <color=red>AXIS_X</color> - <color=lime>AXIS_Y</color> - <color=cyan>AXIS_Z</color>",st);
            else
            {
                if(GUILayout.Button("Create Axis Object Automatically"))
                {
                    GameObject AxisRoot = new GameObject("AxisObject_Root");
                    AxisRoot.transform.position = Vector3.zero;
                    AxisRoot.transform.rotation = Quaternion.identity;

                    GameObject X_Axis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject Y_Axis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject Z_Axis = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    X_Axis.name = "AXIS_X";
                    Y_Axis.name = "AXIS_Y";
                    Z_Axis.name = "AXIS_Z";

                    X_Axis.transform.parent = AxisRoot.transform;
                    Y_Axis.transform.parent = AxisRoot.transform;
                    Z_Axis.transform.parent = AxisRoot.transform;

                    X_Axis.transform.localPosition = new Vector3(0.6f, 0, 0);
                    X_Axis.transform.localRotation = Quaternion.Euler(-90, 0, -90);
                    X_Axis.transform.localScale = new Vector3(0.15f, 1, 0.15f);

                    Y_Axis.transform.localPosition = new Vector3(0, 0.6f, 0);
                    Y_Axis.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    Y_Axis.transform.localScale = new Vector3(0.15f, 1, 0.15f);

                    Z_Axis.transform.localPosition = new Vector3(0, 0, -0.6f);
                    Z_Axis.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    Z_Axis.transform.localScale = new Vector3(0.15f, 1, 0.15f);

                    Material mat1 = new Material(Shader.Find("Diffuse"));
                    mat1.color = Color.red;
                    X_Axis.GetComponent<Renderer>().material = mat1;
                    Material mat2 = new Material(Shader.Find("Diffuse"));
                    mat2.color = Color.green;
                    Y_Axis.GetComponent<Renderer>().material = mat2;
                    Material mat3 = new Material(Shader.Find("Diffuse"));
                    mat3.color = Color.blue;
                    Z_Axis.GetComponent<Renderer>().material = mat3;

                    m.ppAXIS_AxisObject = AxisRoot;
                    return;
                }
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical("Box");

            ppDraw_Property(ppAXIS_SelectionInput, "Selection Input");
            GUILayout.Space(3);
            ppDraw_Property(ppAXIS_AddPointsInput, "Add Input","If you have selected points, you can add more points from selection by holding this input and holding the selection input.");
            ppDraw_Property(ppAXIS_RemovePointsInput, "Remove Input", "If you have selected points, you can remove points from selection by holding this input and holding the selection input.");
            GUILayout.Space(5);
            ppDraw_Property(ppAXIS_LocalSpace, "Local Space", "Axis object orientation");
            ppDraw_Property(ppAXIS_Speed, "Move Speed", "Axis Object move speed");
            ppDraw_Property(ppAXIS_SelectedPointColor, "Selection Color");
            ppDraw_Property(ppAXIS_SelectionGridColor, "Selection Grid Color");
            
            GUILayout.EndVertical();

            serializedObject.Update();
            return;
        }

        ppDraw_Property(ppMobileSuppored, "Runtime Editor For Mobile", "If enabled, the script will work only for Mobile Devices");

        GUILayout.Space(10);

        ppDraw_Property(ppVertexControlMode, "Runtime Editor Control Mode", "Choose a control mode for editor at runtime");
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(5);

        GUI.color = Color.white;

        if (m.ppVertexControlMode == MD_MeshEditorRuntime._VertexControlMode.MoveVertex)
        {
            GUILayout.BeginVertical("Box");
            ppDraw_Property(ppMakeInterativePoint, "Interactive Point", "The selected vertex will move to the raycast point [if possible]");
            if (!m.ppMobileSuppored)
                ppDraw_Property(ppAttachToSender, "Attach To Sender", "After selecting some point, the point will be attached to the sender object [self] for better manipulation...");
            else
                m.ppAttachToSender = true;
            ppDraw_Property(ppHoldingEffect, "Vertex Effect", "If the vertex is selected, the included particle effect will be played");
            GUILayout.Space(5);
            ppDraw_Property(ppLockAxis_X, "Lock X Axis", "If the axis is locked, selected point won't be able to move in the axis direction.");
            ppDraw_Property(ppLockAxis_Y, "Lock Y Axis", "If the axis is locked, selected point won't be able to move in the axis direction.");
            ppDraw_Property(ppLockAxis_Z, "Lock Z Axis", "If the axis is locked, selected point won't be able to move in the axis direction.");
            GUILayout.EndVertical();
        }
        else if (m.ppVertexControlMode != MD_MeshEditorRuntime._VertexControlMode.SculptVertex)
        {
            GUILayout.BeginVertical("Box");
            ppDraw_Property(ppEffectRate, "Update Interval [in Seconds]");
            ppDraw_Property(ppEnableRigidbodyAfterHit, "Add Physics After Hit");
            GUILayout.EndVertical();
        }
        else if (m.ppVertexControlMode == MD_MeshEditorRuntime._VertexControlMode.SculptVertex)
            m.ppPointRay = true;

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");

        if (m.ppVertexControlMode != MD_MeshEditorRuntime._VertexControlMode.SculptVertex)
        {
            if (!m.ppMobileSuppored)
            {
                ppDraw_Property(ppInput, "Control Input", "Enter input key to make the selected effect happen");
                GUILayout.Space(5);
            }
            if (!m.ppAttachToSender)
                ppDraw_Property(ppVertexSpeed, "Vertex Move Speed", "Speed of vertexes during manipulation");
        }
        else if (m.ppVertexControlMode == MD_MeshEditorRuntime._VertexControlMode.SculptVertex)
        {
            ppDraw_Property(pp_SculptingRuntimeFunctions, "Allowed Sculpting Functions");
            if (m.pp_SculptingRuntimeFunctions == MD_MeshEditorRuntime.pp_RuntimeFunctions_Internal.UseRaiseOnly)
            {
                ppDraw_Property(ppInput_Sculpt_Raise, "Raise Input");
            }
            else if (m.pp_SculptingRuntimeFunctions == MD_MeshEditorRuntime.pp_RuntimeFunctions_Internal.UseRaiseLowerOnly)
            {
                ppDraw_Property(ppInput_Sculpt_Raise, "Raise Input");
                ppDraw_Property(ppInput_Sculpt_Lower, "Lower Input");
            }
            else if (m.pp_SculptingRuntimeFunctions == MD_MeshEditorRuntime.pp_RuntimeFunctions_Internal.UseRaiseLowerRevertOnly)
            {
                ppDraw_Property(ppInput_Sculpt_Raise, "Raise Input");
                ppDraw_Property(ppInput_Sculpt_Lower, "Lower Input");
                ppDraw_Property(ppInput_Sculpt_Revert, "Revert Input");
            }

            GUILayout.Space(5);

            ppDraw_Property(ppSculpt_Radius, "Sculpting Radius");
            ppDraw_Property(ppSculpt_Strength, "Sculpting Strength");
            GUILayout.Space(5);
        }
        ppDraw_Property(ppRaycastSpecificPoints, "Raycast With Tag Only", "If enabled, the raycast will accept only colliders with tag below...");
        if (m.ppRaycastSpecificPoints)
            ppDraw_Property(ppSpecialTag, "Collider Tag", "Specific accepted tag to raycast [for points only]");
        GUILayout.Space(5);
        if (!m.ppMobileSuppored)
        {
            ppDraw_Property(ppCursorIsOrigin, "Raycast from Cursor", "If disabled, the raycast origin will be the transforms position [direction = transform.forward]");
            if (!m.ppCursorIsOrigin)
                ppDraw_Property(ppLockAndHideCursor, "Hide & Lock Cursor");
        }
        if (m.ppVertexControlMode != MD_MeshEditorRuntime._VertexControlMode.SculptVertex)
        {
            ppDraw_Property(ppPointRay, "Point Raycast", "If disabled, the raycast will be spherical");
            if (!m.ppPointRay)
                ppDraw_Property(ppSphericalRayRadius, "Spherical Raycast Radius");
        }
        else if(m.ppVertexControlMode == MD_MeshEditorRuntime._VertexControlMode.SculptVertex)
            ppDraw_Box("Please notice: the sculpting control mode will work only on objects with Sculpting Pro component.");
        GUILayout.EndVertical();

        GUILayout.Space(5);

        ppDraw_Property(ppShowGraphic, "Show Debug Graphics");
        if (m.ppShowGraphic)
            ppDraw_Property(ppGraphic_Size, "Graphics Size");

        serializedObject.Update();
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
