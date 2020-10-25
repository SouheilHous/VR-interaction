using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_Bend))]
[CanEditMultipleObjects]
public class MDM_Bend_Editor : ModifiersEditorController
{

    SerializedProperty ppBendDirection;

    SerializedProperty ppAmount;

    MDM_Bend m;

    private void OnEnable()
    {
        ppBendDirection = serializedObject.FindProperty("ppBendDirection");

        ppAmount = serializedObject.FindProperty("ppAmount");
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        m = (MDM_Bend)target;

        GUILayout.Space(10);
        ppDraw_Property(ppBendDirection, "Bend Direction");

        ppDraw_Property(ppAmount, "Amount");

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
