using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_Morpher))]
[CanEditMultipleObjects]
public class MDM_Morpher_Editor : ModifiersEditorController
{

    SerializedProperty ppDynamicMesh;

    // SerializedProperty MorphCaptures;
    SerializedProperty ppBlendValue;

    SerializedProperty ppMultithreadingSupported;

    SerializedProperty ppTargetMorphMeshes;
    SerializedProperty ppIndexOfTargetMesh;
    SerializedProperty ppInterpolationSpeed, ppInterpolation, ppRestartVertState;

    MDM_Morpher m;

    private void OnEnable()
    {
        ppDynamicMesh = serializedObject.FindProperty("ppDynamicMesh");

        ppBlendValue = serializedObject.FindProperty("ppBlendValue");

        ppMultithreadingSupported = serializedObject.FindProperty("ppMultithreadingSupported");

        ppTargetMorphMeshes = serializedObject.FindProperty("ppTargetMorphMeshes");
        ppIndexOfTargetMesh = serializedObject.FindProperty("ppIndexOfTargetMesh");

        ppInterpolation = serializedObject.FindProperty("ppInterpolation");
        ppInterpolationSpeed = serializedObject.FindProperty("ppInterpolationSpeed");

        ppRestartVertState = serializedObject.FindProperty("ppRestartVertState");
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        m = (MDM_Morpher)target;

        GUILayout.Space(10);

        ppDraw_Property(ppDynamicMesh, "Dynamic Morpher", "The morpher will be applied at runtime");
        GUILayout.Space(5);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppMultithreadingSupported, "Multithreading Supported", "If enabled the morph system will be ready for complex mesh operations.");
        if (m.ppMultithreadingSupported)
            GUILayout.Label("If multithreading is enabled, morpher will run at runtime only.");
        ppDraw_Property(ppInterpolation, "Enable Interpolation", "enable smooth movement of vertices");
        if(m.ppInterpolation)
            ppDraw_Property(ppInterpolationSpeed, "Interpolation Speed");

        GUILayout.Space(5);
        ppDraw_Property(ppBlendValue, "Blend Value", "Blend weight");
        ppDraw_Property(ppTargetMorphMeshes, "Target Meshes","",true);
        ppDraw_Property(ppIndexOfTargetMesh, "Active Index Morph");
        ppDraw_Property(ppRestartVertState, "Restart Vertex State", "Restart mesh state after changing index");

        GUILayout.Space(8);
        if (GUILayout.Button("Register Target Morphs"))
        {
            m.Morpher_RefreshTargetMeshes();
            m.ModeSwitched = m.ppMultithreadingSupported;
        }
        if (m.ModeSwitched != m.ppMultithreadingSupported)
            ppDraw_Box("^ Please register target meshes ^");
        GUILayout.EndVertical();
        GUILayout.Space(5);

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
