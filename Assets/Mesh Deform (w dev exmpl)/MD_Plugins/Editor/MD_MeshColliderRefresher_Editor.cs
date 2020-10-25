using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MD_MeshColliderRefresher))]
[CanEditMultipleObjects]
public class MD_MeshColliderRefresher_Editor : Editor
{

    SerializedProperty RefreshType;

    SerializedProperty IntervalSeconds;
    SerializedProperty Convex_MeshCollider;
    SerializedProperty IgnoreRaycast;

    SerializedProperty ColliderOffset;

    private void OnEnable()
    {
        RefreshType = serializedObject.FindProperty("RefreshType");

        IntervalSeconds = serializedObject.FindProperty("IntervalSeconds");
        Convex_MeshCollider = serializedObject.FindProperty("Convex_MeshCollider");
        IgnoreRaycast = serializedObject.FindProperty("IgnoreRaycast");

        ColliderOffset = serializedObject.FindProperty("ColliderOffset");
    }

    public override void OnInspectorGUI()
    {
        MD_MeshColliderRefresher m = (MD_MeshColliderRefresher)target;

        GUILayout.Space(5);

        Color c = new Color();
        ColorUtility.TryParseHtmlString("#9fe6b2", out c);
        GUI.color = c;
       GUILayout.BeginVertical("Box");
            ppDraw_Property(RefreshType, "Collider Refresh Type", "Choose a type of mesh collider refresher. How often, when and how will be the collider refreshed...");
        if (m.RefreshType == MD_MeshColliderRefresher.RefreshType_.Interval)
            ppDraw_Property(IntervalSeconds, "Interval [in Seconds]", "Set the interval value for mesh refreshing in seconds");
        else if (m.RefreshType == MD_MeshColliderRefresher.RefreshType_.Once)
            ppDraw_Property(ColliderOffset, "Collider Offset", "The ColliderOffset can be set at the Start method. This will move your mesh collider to the specific offset position");

        GUILayout.Space(10);

        ppDraw_Property(Convex_MeshCollider, "Convex Mesh Collider");

        GUILayout.Space(5);

        ppDraw_Property(IgnoreRaycast, "Ignore Raycast","If enabled, the objects layer mask will be set to 2 [Ignore raycast]");
        GUILayout.EndVertical();
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
