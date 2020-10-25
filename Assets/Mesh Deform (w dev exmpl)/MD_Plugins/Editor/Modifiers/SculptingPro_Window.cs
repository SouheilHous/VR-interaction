using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

public class SculptingPro_Window : EditorWindow
{
    [MenuItem("Window/MD_Plugin/Sculpting Pro")]
    public static void Init()
    {
        SculptingPro_Window win = (SculptingPro_Window)EditorWindow.GetWindow(typeof(SculptingPro_Window));
        win.minSize = new Vector2(200, 240);
        win.maxSize = new Vector2(210, 245);
        win.Show();
    }

    GUIStyle style;

    public static float B_Radius;
    public static float B_Strength;
    public static bool B_EditMode;

    public static MDM_SculptingPro ssSource;

    void OnGUI()
    {
        style = new GUIStyle();
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        GUILayout.BeginVertical();
        GUILayout.Space(15);
        GUILayout.Label("Sculpting Pro Version 1.3", style);
        GUILayout.EndVertical();
        GUI.color = Color.white;

        style.fontSize = 10;

        if (Selection.gameObjects.Length == 0 || Selection.gameObjects.Length > 1)
        {
            GUILayout.Space(10);
            GUILayout.Label("There must be one object selected.", style);
            return;
        }
        else if (Selection.gameObjects.Length > 0)
        {
            GUILayout.Space(10);
            if (!Selection.gameObjects[0].gameObject.GetComponent<MeshFilter>())
            {
                GUILayout.Label("The object must contains\nMesh Filter.", style);
                return;
            }
            else if (!Selection.gameObjects[0].gameObject.GetComponent<MDM_SculptingPro>())
            {
                GUILayout.Label("The object must contains Sculpting\nPro modifier. You can add\nSculpting Pro below.", style);
                if (GUILayout.Button("Add Scultping Pro"))
                {
                    if (Selection.activeGameObject.GetComponent<MD_MeshProEditor>())
                    {
                        Selection.activeGameObject.GetComponent<MD_MeshProEditor>().MeshEditor_ClearVerticeEditor();
                        DestroyImmediate(Selection.activeGameObject.GetComponent<MD_MeshProEditor>());
                    }
                    Selection.activeGameObject.AddComponent<MDM_SculptingPro>().SSCreateNewReference = true;
                    return;
                }
            }
        }

        GUILayout.Space(15);

        if(B_EditMode)
        {
            if (GUILayout.Button("Disable Edit Mode"))
                B_EditMode = false;
            GUI.color = Color.white;
        }
        else
        {
            if (GUILayout.Button("Enable Edit Mode"))
                B_EditMode = true;
            GUI.color = Color.black;
        }

        GUILayout.Space(12);

        GUILayout.Label("Brush Size - "+B_Radius.ToString());
        B_Radius = GUILayout.HorizontalSlider(B_Radius, 0, 100);

        GUILayout.Space(8);

        GUILayout.Label("Brush Strength - " + B_Strength.ToString());
        B_Strength = GUILayout.HorizontalSlider(B_Strength, 0, 10);

        if(ssSource!=null)
        {
            ssSource.SS_BrushSize = B_Radius;
            ssSource.SS_BrushStrength = B_Strength;
            ssSource.SS_InEditMode = B_EditMode;
        }
    }
}
