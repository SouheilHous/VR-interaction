#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MD_MeshPaint_VR_Input))]
public class MD_MeshPaint_VR_InputEditor : Editor {

    SerializedProperty TargetMeshPaint;

    SerializedProperty pInput_Device;
    SerializedProperty pInput_Action;

    private void OnEnable()
    {
        TargetMeshPaint = serializedObject.FindProperty("TargetMeshPaint");
        pInput_Device = serializedObject.FindProperty("pInput_Device");
        pInput_Action = serializedObject.FindProperty("pInput_Action");
    }

    public override void OnInspectorGUI()
    {
        MD_MeshPaint_VR_Input mp = (MD_MeshPaint_VR_Input)target;

        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(TargetMeshPaint, "Target Mesh Paint", "Assign Mesh Paint Controller");
        GUILayout.EndVertical();

        GUILayout.Space(5);

        ppDraw_Property(pInput_Device, "Input Device");
        ppDraw_Property(pInput_Action, "Input Action");

        GUILayout.Space(10);
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