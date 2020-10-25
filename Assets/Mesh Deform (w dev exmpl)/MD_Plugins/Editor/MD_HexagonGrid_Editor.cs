using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MD_Plugin.HexagonGrid))]
public class MD_HexagonGrid_Editor : ModifiersEditorController {

    SerializedProperty  Count, Offset, CellSize, OffsetX, OffsetZ, ppDynamicMesh, RandomHeightRange, PlanarHexagon, Invert;

    private void OnEnable()
    {
        Count = serializedObject.FindProperty("Count");
        Offset = serializedObject.FindProperty("Offset");

        CellSize = serializedObject.FindProperty("CellSize");

        OffsetX = serializedObject.FindProperty("OffsetX");
        OffsetZ = serializedObject.FindProperty("OffsetZ");

        RandomHeightRange = serializedObject.FindProperty("RandomHeightRange");
        PlanarHexagon = serializedObject.FindProperty("PlanarHexagon");
        Invert = serializedObject.FindProperty("Invert");

        ppDynamicMesh = serializedObject.FindProperty("ppDynamicMesh");
    }

    public override void OnInspectorGUI()
    {
        MD_Plugin.HexagonGrid hg = (MD_Plugin.HexagonGrid)target;

        Color c = Color.white;
        ColorUtility.TryParseHtmlString("#b6d1e0", out c);

        GUI.color = Color.white;
        GUILayout.BeginVertical("Box");
        ppDraw_Property(Count, "Hexagon Count", false);
        ppDraw_Property(CellSize, "Cell Size", false);
        ppDraw_Property(PlanarHexagon, "Planar", false);
        if(hg.PlanarHexagon == false)
            ppDraw_Property(Invert, "Invert", false);
        GUILayout.Space(5);
        ppDraw_Property(Offset, "Pivot Offset", false);
        GUILayout.Space(5);
        ppDraw_Property(OffsetX, "Offset X", false);
        ppDraw_Property(OffsetZ, "Offset Z", false);
        if (!hg.ppDynamicMesh && !hg.PlanarHexagon)
        {
            GUILayout.Space(5);
            if (GUILayout.Button("Randomize Height"))
                hg.hgRandomizeHeight(hg.RandomHeightRange);
            ppDraw_Property(RandomHeightRange, "Random Height Range", false);
        }
        GUILayout.Space(5);
        ppDraw_Property(ppDynamicMesh, "Update Mesh", false);
        GUILayout.EndVertical();

        GUILayout.Space(15);

        serializedObject.Update();

        ppBackToMeshEditor(hg);
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
