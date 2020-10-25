#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_MeshFit))]
public class MDM_MeshFit_Editor : ModifiersEditorController
{

    SerializedProperty UpdateManually, ppMODIF_MeshFitterOffset, ppMODIF_MeshFitterSurfaceDetection, ppMODIF_MeshFitterType, ppMODIF_MeshFitter_SelectedVertexes, ppMODIF_MeshFitterContinuousEffect;

    private void OnEnable()
    {
        UpdateManually = serializedObject.FindProperty("UpdateManually");
        ppMODIF_MeshFitterOffset = serializedObject.FindProperty("ppMODIF_MeshFitterOffset");
        ppMODIF_MeshFitterSurfaceDetection = serializedObject.FindProperty("ppMODIF_MeshFitterSurfaceDetection");
        ppMODIF_MeshFitterType = serializedObject.FindProperty("ppMODIF_MeshFitterType");
        ppMODIF_MeshFitter_SelectedVertexes = serializedObject.FindProperty("ppMODIF_MeshFitter_SelectedVertexes");
        ppMODIF_MeshFitterContinuousEffect = serializedObject.FindProperty("ppMODIF_MeshFitterContinuousEffect");
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        MDM_MeshFit m = (MDM_MeshFit)target;

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Interactive Points"))
            m.MeshFit_GeneratePoints();
        if (GUILayout.Button(new GUIContent("Refresh Mesh Transform", "Set default local rotation and local scale to 1")))
            m.MeshFit_BakeMesh();
        GUILayout.EndHorizontal();

        if (m.points.Count == 0)
            return;

        GUILayout.Space(10);

        GUILayout.BeginHorizontal("Box");
        if (GUILayout.Button("Show Interactive Points"))
            m.MeshFit_ShowHidePoints(true);
        if (GUILayout.Button("Hide Interactive Points"))
            m.MeshFit_ShowHidePoints(false);
        if (GUILayout.Button("Clear Interactive Points"))
            m.MeshFit_ClearPoints();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");
        GUILayout.Space(5);
        ppDraw_Property(ppMODIF_MeshFitterType, "Type");
        ppDraw_Property(UpdateManually, "Update Manually");
        if(m.UpdateManually)
        {
            if (GUILayout.Button("Update Mesh State"))
                m.MeshFit_UpdateMeshState();
        }
        GUILayout.Space(3);
        ppDraw_Property(ppMODIF_MeshFitterOffset, "Raycast Offset","Vertex position offset after raycast");
        ppDraw_Property(ppMODIF_MeshFitterSurfaceDetection, "Raycast Distance","Interactivity radius amount");
        ppDraw_Property(ppMODIF_MeshFitterContinuousEffect, "Continuous Effect","If disabled, the current vertex position will be set to original");
        GUILayout.EndVertical();
        if (m.ppMODIF_MeshFitterType == MDM_MeshFit.MeshFitterMode.FitSpecificVertices)
        {
            ppDraw_Property(ppMODIF_MeshFitter_SelectedVertexes, "Selected Vertices", "", true);
            if (GUILayout.Button("Open Vertices Assignator"))
            {
                MD_VerticeSelectorTool mdvTool = new MD_VerticeSelectorTool();
                mdvTool.minSize = new Vector2(400, 20);
                mdvTool.maxSize = new Vector2(410, 20);
                m.transform.parent = null;
                mdvTool.Show();
                mdvTool.Sender = m;
            }

            if(m.ppMODIF_MeshFitter_SelectedVertexes!=null && m.ppMODIF_MeshFitter_SelectedVertexes.Length>0)
                if (GUILayout.Button("Clear Selected Points"))
                {
                    m.ppMODIF_MeshFitter_SelectedVertexes = null;
                    m.MeshFit_ShowHidePoints(true);
                }
        }
        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        if (GUILayout.Button("Restore Mesh"))
            m.MeshFit_RestoreOriginal();
        GUILayout.EndVertical();

        ppBackToMeshEditor(m);
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