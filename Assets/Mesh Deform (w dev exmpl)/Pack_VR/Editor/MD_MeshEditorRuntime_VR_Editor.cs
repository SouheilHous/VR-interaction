using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MD_MeshEditorRuntime_VR))]
[CanEditMultipleObjects]
public class MD_MeshEditorRuntime_VR_Editor : Editor
{

    SerializedProperty pInput_Device;

    SerializedProperty ppTargetController;
    SerializedProperty ppVertexControlMode;

    SerializedProperty ppHoldingEffect;
    SerializedProperty ppMakeInterativePoint;

    SerializedProperty ppEffectRate;
    SerializedProperty ppEnableRigidbodyAfterHit;

    SerializedProperty ppVertexSpeed;

    SerializedProperty ppRaycastSpecificPoints;
    SerializedProperty ppSpecialTag;

    SerializedProperty ppPointRay;
    SerializedProperty ppSphericalRayRadius;

    SerializedProperty ppShowGraphic, ppGraphic_Size, UseDebugSizeBySculptingRadius;

    SerializedProperty pp_SculptingRuntimeFunctions, ppSculpt_Radius, ppSculpt_Strength;
    //---Controls - Sculpting
    SerializedProperty ppInput_Sculpt_Raise, ppInput_Sculpt_Lower, ppInput_Sculpt_Revert;

    private void OnEnable()
    {
        MD_MeshEditorRuntime_VR m = (MD_MeshEditorRuntime_VR)target;

        pInput_Device = serializedObject.FindProperty("pInput_Device");

        ppTargetController = serializedObject.FindProperty("ppTargetController");

        ppVertexControlMode = serializedObject.FindProperty("ppVertexControlMode");

        ppHoldingEffect = serializedObject.FindProperty("ppHoldingEffect");
        ppMakeInterativePoint = serializedObject.FindProperty("ppMakeInterativePoint");

        ppEffectRate = serializedObject.FindProperty("ppEffectRate");
        ppEnableRigidbodyAfterHit = serializedObject.FindProperty("ppEnableRigidbodyAfterHit");

        ppVertexSpeed = serializedObject.FindProperty("ppVertexSpeed");

        ppRaycastSpecificPoints = serializedObject.FindProperty("ppRaycastSpecificPoints");
        ppSpecialTag = serializedObject.FindProperty("ppSpecialTag");

        ppPointRay = serializedObject.FindProperty("ppPointRay");
        ppSphericalRayRadius = serializedObject.FindProperty("ppSphericalRayRadius");

        ppShowGraphic = serializedObject.FindProperty("ppShowGraphic");
        ppGraphic_Size = serializedObject.FindProperty("ppGraphic_Size");
        UseDebugSizeBySculptingRadius = serializedObject.FindProperty("UseDebugSizeBySculptingRadius");

        pp_SculptingRuntimeFunctions = serializedObject.FindProperty("pp_SculptingRuntimeFunctions");
        ppInput_Sculpt_Raise = serializedObject.FindProperty("ppInput_Sculpt_Raise");
        ppInput_Sculpt_Lower = serializedObject.FindProperty("ppInput_Sculpt_Lower");
        ppInput_Sculpt_Revert = serializedObject.FindProperty("ppInput_Sculpt_Revert");
        ppSculpt_Radius = serializedObject.FindProperty("ppSculpt_Radius");
        ppSculpt_Strength = serializedObject.FindProperty("ppSculpt_Strength");
    }

    public override void OnInspectorGUI()
    {
        MD_MeshEditorRuntime_VR m = (MD_MeshEditorRuntime_VR)target;

        GUILayout.Space(10);

        GUI.color = Color.white;
        GUILayout.BeginVertical("Box");

        ppDraw_Property(pInput_Device, "Input Device");
        GUILayout.EndVertical();
        GUILayout.Space(5);
        ppDraw_Property(ppTargetController, "Use Target Controller", "If disabled, the controller will be THIS object");
        if (m.ppTargetController)
            ppDraw_Property("TargetController", "Target Controller");

        GUILayout.Space(5);
        ppDraw_Property(ppVertexControlMode, "Runtime Editor Control Mode", "Choose a control mode for editor at runtime");
        Color c = Color.white;
        ColorUtility.TryParseHtmlString("#e1e69f", out c);
        GUI.color = c;

        if (m.ppVertexControlMode == MD_MeshEditorRuntime_VR._VertexControlMode.MoveVertex)
        {
            GUILayout.BeginVertical("Box");
            ppDraw_Property(ppMakeInterativePoint, "Interactive Point", "The selected vertex will move to the raycasted point [if possible]");
            ppDraw_Property(ppHoldingEffect, "Vertex Effect", "If the vertex is selected, the included particle effect will be occured");
            GUILayout.EndVertical();
        }
        else if (m.ppVertexControlMode != MD_MeshEditorRuntime_VR._VertexControlMode.SculptVertex)
        {
            GUILayout.BeginVertical("Box");
            ppDraw_Property(ppEffectRate, "Update Interval [in Seconds]", "How often will the effect be occured");
            ppDraw_Property(ppEnableRigidbodyAfterHit, "Add Physics After Hit");
            GUILayout.EndVertical();
        }
        else if (m.ppVertexControlMode == MD_MeshEditorRuntime_VR._VertexControlMode.SculptVertex)
            m.ppPointRay = true;

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");

        if (m.ppVertexControlMode != MD_MeshEditorRuntime_VR._VertexControlMode.SculptVertex)
                ppDraw_Property("pInput_Action", "Input Action", "Enter input button to make the selected effect happen");
        else
        {
            ppDraw_Property(pp_SculptingRuntimeFunctions, "Allowed Sculpting Functions");
            if (m.pp_SculptingRuntimeFunctions == MD_MeshEditorRuntime_VR.pp_RuntimeFunctions_Internal.UseRaiseOnly)
            {
                ppDraw_Property(ppInput_Sculpt_Raise, "Raise Input");
            }
            else if (m.pp_SculptingRuntimeFunctions == MD_MeshEditorRuntime_VR.pp_RuntimeFunctions_Internal.UseRaiseLowerOnly)
            {
                ppDraw_Property(ppInput_Sculpt_Raise, "Raise Input");
                ppDraw_Property(ppInput_Sculpt_Lower, "Lower Input");
            }
            else if (m.pp_SculptingRuntimeFunctions == MD_MeshEditorRuntime_VR.pp_RuntimeFunctions_Internal.UseRaiseLowerRevertOnly)
            {
                ppDraw_Property(ppInput_Sculpt_Raise, "Raise Input");
                ppDraw_Property(ppInput_Sculpt_Lower, "Lower Input");
                ppDraw_Property(ppInput_Sculpt_Revert, "Revert Input");
            }
        }

        GUILayout.Space(5);

        if (m.ppVertexControlMode != MD_MeshEditorRuntime_VR._VertexControlMode.SculptVertex)
        {
            if (m.ppVertexControlMode != MD_MeshEditorRuntime_VR._VertexControlMode.MoveVertex)
                ppDraw_Property(ppVertexSpeed, "Vertex Move Speed", "Speed of vertexes during manipulation");
        }
        ppDraw_Property(ppRaycastSpecificPoints, "Raycast Only With Tag", "If enabled, the raycast will accept only colliders with tag below...");
        if (m.ppRaycastSpecificPoints)
            ppDraw_Property(ppSpecialTag, "Collider Tag", "Specific accepted tag to raycast [for points only]");

        GUILayout.Space(5);

        if (m.ppVertexControlMode != MD_MeshEditorRuntime_VR._VertexControlMode.SculptVertex)
        {
            ppDraw_Property(ppPointRay, "Point Raycast", "If disabled, the raycast will be spherical");
            if (!m.ppPointRay)
                ppDraw_Property(ppSphericalRayRadius, "Spherical Raycast Radius");
        }
        else if (m.ppVertexControlMode == MD_MeshEditorRuntime_VR._VertexControlMode.SculptVertex)
        {
            ppDraw_Property(ppSculpt_Radius, "Sculpting Radius");
            ppDraw_Property(ppSculpt_Strength, "Sculpting Strength");
            GUILayout.Space(5);
            ppDraw_Box("Please notice: the sculpting control mode will work only on objects with Sculpting Pro component.");
        }

        GUILayout.EndVertical();

        GUILayout.Space(5);

        ppDraw_Property(ppShowGraphic, "Show Debug Graphics");
        if (m.ppShowGraphic)
        {
            ppDraw_Property(ppGraphic_Size, "Graphics Size");
            if (m.ppVertexControlMode == MD_MeshEditorRuntime_VR._VertexControlMode.SculptVertex)
                ppDraw_Property(UseDebugSizeBySculptingRadius, "Control Debug Size By Sculpt Radius");
        }

        serializedObject.Update();
    }

    private void ppDraw_Property(SerializedProperty p, string Text, string ToolTip = "", bool includeChilds = false)
    {
        EditorGUILayout.PropertyField(p, new GUIContent(Text, ToolTip), includeChilds, null);
        serializedObject.ApplyModifiedProperties();
    }
    private void ppDraw_Property(string p, string Text, string ToolTip = "", bool includeChilds = false)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(p), new GUIContent(Text, ToolTip), includeChilds, null);
        serializedObject.ApplyModifiedProperties();
    }
    private void ppDraw_Box(string Text, MessageType type = MessageType.Info)
    {
        EditorGUILayout.HelpBox(Text, type);
    }
}