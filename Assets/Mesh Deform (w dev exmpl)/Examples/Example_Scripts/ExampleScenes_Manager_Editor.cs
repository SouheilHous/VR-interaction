#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(ExampleScenes_Manager))]
public class ExampleScenes_Manager_Editor : Editor {


    SerializedProperty ManagerType, SceneDestription, DescColor, YOffset, XOffset;

    //----Basic Shapes---
    SerializedProperty CenterToInstantiate;

    SerializedProperty _EnablePhysics;
    SerializedProperty _EnableAutoMove;


    //----Example Game---
    SerializedProperty ParticleSystem;

    private void OnEnable()
    {
        ManagerType = serializedObject.FindProperty("ManagerType");
        SceneDestription = serializedObject.FindProperty("SceneDestription");
        DescColor = serializedObject.FindProperty("DescColor");
        YOffset = serializedObject.FindProperty("YOffset");
        XOffset = serializedObject.FindProperty("XOffset");

        CenterToInstantiate = serializedObject.FindProperty("CenterToInstantiate");
        _EnablePhysics = serializedObject.FindProperty("_EnablePhysics");
        _EnableAutoMove = serializedObject.FindProperty("_EnableAutoMove");

        ParticleSystem = serializedObject.FindProperty("ParticleSystem");
    }

    public override void OnInspectorGUI()
    {
        ExampleScenes_Manager m = (ExampleScenes_Manager)target;

        GUILayout.Space(15);
        EditorGUILayout.PropertyField(ManagerType, new GUIContent("Manager Type"));
        EditorGUILayout.PropertyField(SceneDestription, new GUIContent("Description"));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(DescColor, new GUIContent("Color"));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(YOffset, new GUIContent("Y Offset"));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(XOffset, new GUIContent("X Offset"));
        serializedObject.ApplyModifiedProperties();

        if (m.ManagerType == ExampleScenes_Manager._ManagerType.BasicShapesDeformation)
        {
            GUILayout.Space(15);
            GUILayout.Label("Basic Shapes Scene Manager");
            GUILayout.Space(15);

            EditorGUILayout.PropertyField(CenterToInstantiate, new GUIContent("Center To Instantiate"));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(_EnableAutoMove, new GUIContent("Auto Move Toggle"));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(_EnablePhysics, new GUIContent("Physics Toggle"));
            serializedObject.ApplyModifiedProperties();
        }


        if (m.ManagerType == ExampleScenes_Manager._ManagerType.ExampleGame)
        {
            GUILayout.Space(15);
            GUILayout.Label("Example Game Scene Manager");
            GUILayout.Space(15);

            EditorGUILayout.PropertyField(ParticleSystem, new GUIContent("Fire Effect"));
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.ApplyModifiedProperties();

        if (target != null)
            serializedObject.Update();
    }
}
#endif