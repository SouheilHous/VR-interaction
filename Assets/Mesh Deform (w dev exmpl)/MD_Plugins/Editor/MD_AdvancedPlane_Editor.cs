using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MD_Plugin.AdvancedPlane))]
public class MD_AdvancedPlane_Editor : ModifiersEditorController {

    SerializedProperty ppPlaneSizeAngle;
    SerializedProperty ppPlaneSize;
    SerializedProperty ppPlaneOffset;
    SerializedProperty ppDynamicMesh;

    SerializedProperty ppEnableAngle;
    SerializedProperty ppAngle;
    SerializedProperty ppAngleDensity;

    SerializedProperty ppEnableLandscapeFitter;
    SerializedProperty ppTranslationSpeed;

    private void OnEnable()
    {
        ppPlaneSizeAngle = serializedObject.FindProperty("ppPlaneSizeAngle");
        ppPlaneSize = serializedObject.FindProperty("ppPlaneSize");
        ppPlaneOffset = serializedObject.FindProperty("ppPlaneOffset");
        ppDynamicMesh = serializedObject.FindProperty("ppDynamicMesh");

        ppEnableAngle = serializedObject.FindProperty("ppEnableAngle");
        ppAngle = serializedObject.FindProperty("ppAngle");
        ppAngleDensity = serializedObject.FindProperty("ppAngleDensity");

        ppEnableLandscapeFitter = serializedObject.FindProperty("ppEnableLandscapeFitter");
        ppTranslationSpeed = serializedObject.FindProperty("ppTranslationSpeed");
    }

    public override void OnInspectorGUI()
    {
        MD_Plugin.AdvancedPlane ap = (MD_Plugin.AdvancedPlane)target;

        Color c = Color.white;
        ColorUtility.TryParseHtmlString("#b6d1e0", out c);

        GUI.color = Color.white;
        GUILayout.BeginVertical("Box");
        ppDraw_Text("Mesh Transform");
        if(ap.ppEnableAngle)
            ppDraw_Property(ppPlaneSizeAngle, "Plane Size", false);
        else
            ppDraw_Property(ppPlaneSize, "Plane Size", false);
        ppDraw_Property(ppPlaneOffset, "Plane Pivot Offset", false);
        ppDraw_Property(ppDynamicMesh, "Dynamic Mesh At Runtime", false);
        GUILayout.EndVertical();

        GUILayout.Space(15);

        if (!ap.GetComponent<MD_Plugin.MD_MeshColliderRefresher>())
        {
            ColorUtility.TryParseHtmlString("#7beb99", out c);
            GUI.color = c;
            GUILayout.BeginVertical("Box");
            if (GUILayout.Button("Add Mesh Collider Refresher"))
                ap.gameObject.AddComponent<MD_Plugin.MD_MeshColliderRefresher>();
            GUILayout.EndVertical();

            GUILayout.Space(15);
        }
        ColorUtility.TryParseHtmlString("#b6d1e0", out c);
        GUI.color = Color.white;
        ppDraw_Property(ppEnableAngle, "Enable Angle Property", false);
        if (ap.ppEnableAngle)
        {
            GUI.color = c;
            GUILayout.BeginVertical("Box");
            ppDraw_Property(ppAngle, "Angle", false);
            ppDraw_Property(ppAngleDensity, "Angle Density", false);
            GUILayout.EndVertical();
        }

        if (ap.ppEnableAngle)
        {
            GUILayout.Space(10);

            GUI.color = Color.white;
            ppDraw_Property(ppEnableLandscapeFitter, "Enable Landscape Fitter", false);
            if (ap.ppEnableLandscapeFitter)
            {
                GUI.color = c;
                GUILayout.BeginVertical("Box");
                ppDraw_Property(ppTranslationSpeed, "Translation Speed", false);
                GUILayout.EndVertical();
            }
        }

        serializedObject.Update();

        ppBackToMeshEditor(ap);
    }


    void ppDraw_Property(SerializedProperty p, string Text,bool includeChilds)
    {
        EditorGUILayout.PropertyField(p, new GUIContent(Text), includeChilds,null);
        serializedObject.ApplyModifiedProperties();
    }
    void ppDraw_Text(string Text)
    {
        GUILayout.Label(Text);
    }
}
