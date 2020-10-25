using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MD_Plugin.AdvancedShape))]
public class MD_AdvancedShapes_Editor : ModifiersEditorController
{

    SerializedProperty ShapeType, ppDynamicMesh;

    SerializedProperty G_Plane_length, G_Plane_width, G_Plane_resX;

    SerializedProperty G_Box_length, G_Box_height, G_Box_width;

    SerializedProperty G_Cone_height, G_Cone_botRadius , G_Cone_topRadius , G_Cone_verticalSides;

    SerializedProperty G_Torus_radius0, G_Torus_radius1, G_Torus_segments, G_Torus_sides;

    SerializedProperty G_Sphere_radius0, G_Sphere_segments, G_Sphere_stack, G_Sphere_sliceMax, G_Sphere_verticalMax;

    SerializedProperty  G_Tube_radius0, G_Tube_radius1, G_Tube_height, G_Tube_segments;

    private void OnEnable()
    {
        ShapeType = serializedObject.FindProperty("ShapeType");
        ppDynamicMesh = serializedObject.FindProperty("ppDynamicMesh");

        G_Plane_length = serializedObject.FindProperty("G_Plane_length");
        G_Plane_width = serializedObject.FindProperty("G_Plane_width");
        G_Plane_resX = serializedObject.FindProperty("G_Plane_resX");

        G_Box_length = serializedObject.FindProperty("G_Box_length");
        G_Box_height = serializedObject.FindProperty("G_Box_height");
        G_Box_width = serializedObject.FindProperty("G_Box_width");

        G_Cone_height = serializedObject.FindProperty("G_Cone_height");
        G_Cone_botRadius = serializedObject.FindProperty("G_Cone_botRadius");
        G_Cone_topRadius = serializedObject.FindProperty("G_Cone_topRadius");
        G_Cone_verticalSides = serializedObject.FindProperty("G_Cone_verticalSides");

        G_Torus_radius0 = serializedObject.FindProperty("G_Torus_radius0");
        G_Torus_radius1 = serializedObject.FindProperty("G_Torus_radius1");
        G_Torus_segments = serializedObject.FindProperty("G_Torus_segments");
        G_Torus_sides = serializedObject.FindProperty("G_Torus_sides");

        G_Sphere_radius0 = serializedObject.FindProperty("G_Sphere_radius0");
        G_Sphere_segments = serializedObject.FindProperty("G_Sphere_segments");
        G_Sphere_stack = serializedObject.FindProperty("G_Sphere_stack");
        G_Sphere_sliceMax = serializedObject.FindProperty("G_Sphere_sliceMax");
        G_Sphere_verticalMax = serializedObject.FindProperty("G_Sphere_verticalMax");

        G_Tube_radius0 = serializedObject.FindProperty("G_Tube_radius0");
        G_Tube_radius1 = serializedObject.FindProperty("G_Tube_radius1");
        G_Tube_height = serializedObject.FindProperty("G_Tube_height");
        G_Tube_segments = serializedObject.FindProperty("G_Tube_segments");
    }

    public override void OnInspectorGUI()
    {
        MD_Plugin.AdvancedShape asp = (MD_Plugin.AdvancedShape)target;

        ppDraw_Property(ShapeType, "Shape Type",false);
        ppDraw_Property(ppDynamicMesh, "Dynamic Mesh At Runtime?", false);

        if(asp.ShapeType == MD_Plugin.AdvancedShape.ShapeType_.Plane)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical("Box");

            ppDraw_Property(G_Plane_length, "Length", false);
            ppDraw_Property(G_Plane_width, "Width", false);
            GUILayout.Space(6);
            ppDraw_Property(G_Plane_resX, "Resolution", false);

            GUILayout.EndVertical();
        }
        else if (asp.ShapeType == MD_Plugin.AdvancedShape.ShapeType_.Box)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical("Box");

            ppDraw_Property(G_Box_length, "Length", false);
            ppDraw_Property(G_Box_height, "Height", false);
            ppDraw_Property(G_Box_width, "Width", false);

            GUILayout.EndVertical();
        }
        else if (asp.ShapeType == MD_Plugin.AdvancedShape.ShapeType_.Cone)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical("Box");

            ppDraw_Property(G_Cone_height, "Height", false);
            GUILayout.Space(6);
            ppDraw_Property(G_Cone_botRadius, "Radius - Bottom", false);
            ppDraw_Property(G_Cone_topRadius, "Radius - Top", false);
            GUILayout.Space(6);
            ppDraw_Property(G_Cone_verticalSides, "Sides Count", false);

            GUILayout.EndVertical();
        }
        else if (asp.ShapeType == MD_Plugin.AdvancedShape.ShapeType_.Torus)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical("Box");

            ppDraw_Property(G_Torus_radius0, "Radius - Length", false);
            ppDraw_Property(G_Torus_radius1, "Radius - Size", false);
            GUILayout.Space(6);
            ppDraw_Property(G_Torus_segments, "Count Of Segments", false);
            ppDraw_Property(G_Torus_sides, "Count Of Sides", false);

            GUILayout.EndVertical();
        }
        else if (asp.ShapeType == MD_Plugin.AdvancedShape.ShapeType_.Sphere)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical("Box");

            ppDraw_Property(G_Sphere_radius0, "Radius", false);
            ppDraw_Property(G_Sphere_segments, "Segments", false);
            ppDraw_Property(G_Sphere_stack, "Stack", false);
            GUILayout.Space(6);
            ppDraw_Property(G_Sphere_sliceMax, "Slice Max", false);
            ppDraw_Property(G_Sphere_verticalMax, "Vertical Slice Max", false);

            GUILayout.EndVertical();
        }
        else if (asp.ShapeType == MD_Plugin.AdvancedShape.ShapeType_.Tube)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical("Box");

            ppDraw_Property(G_Tube_radius0, "Outer Radius", false);
            ppDraw_Property(G_Tube_radius1, "Inner Radius", false);
            ppDraw_Property(G_Tube_height, "Height", false);
            GUILayout.Space(6);
            ppDraw_Property(G_Tube_segments, "Segments Count", false);

            GUILayout.EndVertical();
        }

        serializedObject.Update();

        ppAddMeshColliderRefresher(asp);
        ppBackToMeshEditor(asp);
    }

    void ppDraw_Property(SerializedProperty p, string Text, bool includeChilds)
    {
        EditorGUILayout.PropertyField(p, new GUIContent(Text), includeChilds, null);
        serializedObject.ApplyModifiedProperties();
    }
    void ppDraw_Text(string Text)
    {
        GUILayout.Label(Text);
    }
}
