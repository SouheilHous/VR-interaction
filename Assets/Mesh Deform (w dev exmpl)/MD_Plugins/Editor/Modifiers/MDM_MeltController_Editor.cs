using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_MeltController))]
[CanEditMultipleObjects]
public class MDM_MeltController_Editor : ModifiersEditorController
{

    SerializedProperty ppSelfHeightValue;

    SerializedProperty ppTargetHeightValue;

    SerializedProperty ppMeltBySurfaceRaycast;
    SerializedProperty ppRaycastOriginOffset;
    SerializedProperty ppRaycastDirection;
    SerializedProperty ppRaycastDistance;
    SerializedProperty ppAllowedLayerMasks;

    SerializedProperty ppEnableLinearInterpolationBlend;
    SerializedProperty ppLinearInterpolationSpeed;

    SerializedProperty ppShowEditorGraphic;

    MDM_MeltController m;

    private void OnEnable()
    {
        ppSelfHeightValue = serializedObject.FindProperty("ppSelfHeightValue");

        ppTargetHeightValue = serializedObject.FindProperty("ppTargetHeightValue");

        ppMeltBySurfaceRaycast = serializedObject.FindProperty("ppMeltBySurfaceRaycast");
        ppRaycastOriginOffset = serializedObject.FindProperty("ppRaycastOriginOffset");
        ppRaycastDirection = serializedObject.FindProperty("ppRaycastDirection");
        ppRaycastDistance = serializedObject.FindProperty("ppRaycastDistance");
        ppAllowedLayerMasks = serializedObject.FindProperty("ppAllowedLayerMasks");

        ppEnableLinearInterpolationBlend = serializedObject.FindProperty("ppEnableLinearInterpolationBlend");
        ppLinearInterpolationSpeed = serializedObject.FindProperty("ppLinearInterpolationSpeed");

        ppShowEditorGraphic = serializedObject.FindProperty("ppShowEditorGraphic");
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        m = (MDM_MeltController)target;

        ppDraw_Box("Object should contains MD_Melt shader.");
        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppSelfHeightValue, "Set Height By Self","If enabled, the Y value will be set to this position Y");
        if(!m.ppSelfHeightValue)
            ppDraw_Property(ppTargetHeightValue, "Target of Height");

        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");

        ppDraw_Property(ppMeltBySurfaceRaycast, "Melt by Raycast", "If enabled, the Y value will be set to modified hit point Y");
        if (m.ppMeltBySurfaceRaycast)
        {
            ppDraw_Property(ppAllowedLayerMasks, "Allowed Layer Masks");
            ppDraw_Property(ppRaycastOriginOffset, "Raycast Origin Offset");
            ppDraw_Property(ppRaycastDirection, "Raycast Direction");
            ppDraw_Property(ppRaycastDistance, "Raycast Distance");

            GUILayout.Space(5);

            ppDraw_Property(ppEnableLinearInterpolationBlend, "Linear Interpolation");
            if (m.ppEnableLinearInterpolationBlend)
                ppDraw_Property(ppLinearInterpolationSpeed, "Linear Interpolation Speed");
        }

        GUILayout.EndVertical();

        GUILayout.Space(10);

        ppDraw_Property(ppShowEditorGraphic, "Show Editor Graphic");

        ppBackToMeshEditor(m);
        if (target != null)
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
