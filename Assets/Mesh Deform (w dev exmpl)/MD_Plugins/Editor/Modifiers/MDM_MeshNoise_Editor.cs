using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_MeshNoise))]
[CanEditMultipleObjects]
public class MDM_MeshNoise_Editor : ModifiersEditorController
{

    SerializedProperty ppNoiseType;

    SerializedProperty ppMeshNoiseAmount, ppMeshNoiseSpeed, ppMeshNoiseIntensity;

    MDM_MeshNoise m;

    private void OnEnable()
    {
        ppNoiseType = serializedObject.FindProperty("ppNoiseType");

        ppMeshNoiseAmount = serializedObject.FindProperty("ppMeshNoiseAmount");
        ppMeshNoiseSpeed = serializedObject.FindProperty("ppMeshNoiseSpeed");
        ppMeshNoiseIntensity = serializedObject.FindProperty("ppMeshNoiseIntensity");
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        m = (MDM_MeshNoise)target;

        GUILayout.Space(10);
        ppDraw_Property(ppNoiseType, "Noise Type");

        EditorGUI.indentLevel++;
        ppDraw_Property(ppMeshNoiseIntensity, "Intensity");
        ppDraw_Property(ppMeshNoiseSpeed, "Speed");
        if (m.ppNoiseType == MDM_MeshNoise.NoiseType.VerticalNoise)
        {
            ppDraw_Property(ppMeshNoiseAmount, "Amount");

            if(GUILayout.Button("Update Noise in Editor"))
            {
                if (!EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to update this modification in Editor? This will deform your whole mesh and there is no way back...", "Yes", "No"))
                    return;
                m.MeshNoise_UpdateVerticalNoise();
            }
        }
        else
        {
            if (GUILayout.Button("Update Noise in Editor"))
            {
                if (!EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to update this modification in Editor? This will deform your whole mesh and there is no way back...", "Yes", "No"))
                    return;
                m.MeshNoise_UpdateOverallNoise();
            }
        }
        EditorGUI.indentLevel--;


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
