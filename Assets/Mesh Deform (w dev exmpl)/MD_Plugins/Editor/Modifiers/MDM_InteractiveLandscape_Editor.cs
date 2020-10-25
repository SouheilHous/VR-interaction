using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_InteractiveLandscape))]
[CanEditMultipleObjects]
public class MDM_InteractiveLandscape_Editor : ModifiersEditorController
{

    SerializedProperty ppDynamicMesh, ppAllowRigidbodies;

    SerializedProperty ppEnableCustomInteractionSpeed;
    SerializedProperty ppInteractionSpeed;
    SerializedProperty ppCustomInteraction_Continuous;

    SerializedProperty ppMultithreadingSupported;

    SerializedProperty ppVerticeDirection;
    SerializedProperty ppRadius;
    SerializedProperty ppFitToObjectSize;
    SerializedProperty ppForceDetection;

    SerializedProperty ppRepair;
    SerializedProperty ppRepairSpeed;

    SerializedProperty ppCollisionWithSpecificTag;
    SerializedProperty ppCollisionTag;

    MDM_InteractiveLandscape m;

    public bool enableDebug = true;

    private void OnEnable()
    {
        m = (MDM_InteractiveLandscape)target;
        ppDynamicMesh = serializedObject.FindProperty("ppDynamicMesh");
        ppAllowRigidbodies = serializedObject.FindProperty("ppAllowRigidbodies");

        ppEnableCustomInteractionSpeed = serializedObject.FindProperty("ppEnableCustomInteractionSpeed");
        ppInteractionSpeed = serializedObject.FindProperty("ppInteractionSpeed");
        ppCustomInteraction_Continuous = serializedObject.FindProperty("ppCustomInteraction_Continuous");

        ppMultithreadingSupported = serializedObject.FindProperty("ppMultithreadingSupported");

        ppVerticeDirection = serializedObject.FindProperty("ppVerticeDirection");
        ppRadius = serializedObject.FindProperty("ppRadius");
        ppFitToObjectSize = serializedObject.FindProperty("ppFitToObjectSize");
        ppForceDetection = serializedObject.FindProperty("ppForceDetection");

        ppRepair = serializedObject.FindProperty("ppRepair");
        ppRepairSpeed = serializedObject.FindProperty("ppRepairSpeed");

        ppCollisionWithSpecificTag = serializedObject.FindProperty("ppCollisionWithSpecificTag");
        ppCollisionTag = serializedObject.FindProperty("ppCollisionTag");
    }

    private void OnSceneGUI()
    {
        if (!enableDebug)
            return;
        float radius = m.ppRadius;
        float forceDetect = m.ppForceDetection;
        Handles.color = Color.magenta;
        Handles.DrawWireDisc(m.transform.position, Vector3.up, radius);
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, m.transform.position, Quaternion.Euler(-90,0,0), forceDetect, EventType.Repaint);

        EditorGUI.BeginChangeCheck();
        Handles.color = Color.magenta;
        radius = Handles.ScaleValueHandle(radius, m.transform.position + new Vector3(radius, 0, 0), m.transform.rotation, 1, Handles.SphereHandleCap, 0.1f);
        Handles.color = Color.red;
        forceDetect = Handles.ScaleValueHandle(forceDetect, m.transform.position + new Vector3(0, forceDetect, 0), m.transform.rotation, 1, Handles.CircleHandleCap, 0.1f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Change Radius");
            m.ppRadius = radius;
            m.ppForceDetection = forceDetect;
        }
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppDynamicMesh, "Dynamic Mesh","The mesh will refresh its surface and detect collision every frame");
        ppDraw_Property(ppMultithreadingSupported, "Multithreading Supported", "If enabled, the mesh will be ready for complex operations.");
        GUILayout.EndVertical();

        GUILayout.Space(5);

        ppDraw_Property(ppAllowRigidbodies, "Allow Rigidbodies", "Allow Collision Enter & Collision Stay functions for Rigidbodies");

        GUILayout.Space(5);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppVerticeDirection, "Vertice Direction","Direction for vertices after interaction");
        GUILayout.Space(5);
        ppDraw_Property(ppFitToObjectSize, "Fit To Collision Object Size", "Configure and fit radius size by collided object size");
        if(!m.ppFitToObjectSize)
            ppDraw_Property(ppRadius, "Interactive Radius", "Radius of vertices to be interacted");
        if (m.ppAllowRigidbodies)
        {
            GUILayout.Space(5);
            ppDraw_Property(ppForceDetection, "Force Detection Level", "In which rigidbody velocity will the object detects collision");
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppCollisionWithSpecificTag, "Collision With Specific Tag", "If enabled, collision will be allowed for objects with tag below...");
        if (m.ppCollisionWithSpecificTag)
            ppDraw_Property(ppCollisionTag, "Collision Tag");
        GUILayout.EndVertical();
        GUILayout.Space(15);

        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppEnableCustomInteractionSpeed, "Custom Interaction Speed", "If enabled, you will be able to customize vertices speed after its interaction/ collision");
        if (m.ppEnableCustomInteractionSpeed)
        {
            ppDraw_Property(ppInteractionSpeed, "Interaction Speed");
            ppDraw_Property(ppCustomInteraction_Continuous, "Enable Continuous Effect", "If enabled, vertices of the mesh will be able to go over their origin");
        }

        GUILayout.Space(5);

        ppDraw_Property(ppRepair, "Repair Mesh", "Repair Mesh after some time and interval");
        if(m.ppRepair)
            ppDraw_Property(ppRepairSpeed, "Repair Speed");
        GUILayout.EndVertical();

        GUILayout.Space(10);
        enableDebug = GUILayout.Toggle(enableDebug, "Enable Scene Debug");

        if (target != null)
            serializedObject.Update();

        ppAddMeshColliderRefresher(m);
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
