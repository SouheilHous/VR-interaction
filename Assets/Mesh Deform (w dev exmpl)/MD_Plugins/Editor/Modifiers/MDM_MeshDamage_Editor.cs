using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_MeshDamage))]
[CanEditMultipleObjects]
public class MDM_MeshDamage_Editor : ModifiersEditorController
{

    SerializedProperty ppDynamicMesh;
    SerializedProperty ppRigidbody;

    SerializedProperty ppAutoGenerateRadius, ppRadius;

    SerializedProperty ppForceDetection, ppForceAmount, ppAutoForce;

    SerializedProperty ppContinousDamage;

    SerializedProperty ppCollisionWithSpecificTag;
    SerializedProperty ppCollisionTag;

    SerializedProperty ppEnableEvent;
    SerializedProperty ppEvent;

    MDM_MeshDamage m;

    private void OnEnable()
    {
        m = (MDM_MeshDamage)target;
        ppDynamicMesh = serializedObject.FindProperty("ppDynamicMesh");
        ppRigidbody = serializedObject.FindProperty("ppRigidbody");

        ppAutoGenerateRadius = serializedObject.FindProperty("ppAutoGenerateRadius");
        ppRadius = serializedObject.FindProperty("ppRadius");

        ppAutoForce = serializedObject.FindProperty("ppAutoForce");
        ppForceAmount = serializedObject.FindProperty("ppForceAmount");
        ppForceDetection = serializedObject.FindProperty("ppForceDetection");

        ppContinousDamage = serializedObject.FindProperty("ppContinousDamage");

        ppCollisionWithSpecificTag = serializedObject.FindProperty("ppCollisionWithSpecificTag");
        ppCollisionTag = serializedObject.FindProperty("ppCollisionTag");

        ppEnableEvent = serializedObject.FindProperty("ppEnableEvent");
        ppEvent = serializedObject.FindProperty("ppEvent");
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        GUILayout.Space(10);
        ppDraw_Property(ppDynamicMesh, "Dynamic Mesh", "The mesh will refresh its surface and detect collision every frame");
        if(m.ppForceDetection>0)
        ppDraw_Property(ppRigidbody, "Target Rigidbody", "Target rigidbody of Force detection level [if empty, rigidbody will be reached from the gameobject]");

        GUILayout.Space(10);


        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppAutoGenerateRadius, "Auto Generate Radius", "The collision hit radius will be generated automatically");
        if (!m.ppAutoGenerateRadius)
            ppDraw_Property(ppRadius, "Radius Collision");
        GUILayout.Space(5);
        ppDraw_Property(ppAutoForce, "Auto Force Multiplier", "If enabled, the script will detect the force multiplier automatically [collided rigidbody velocity]");
        if(!m.ppAutoForce)
        ppDraw_Property(ppForceAmount, "Force Multiplier");
        GUILayout.Space(5);
        ppDraw_Property(ppForceDetection, "Force Detection Level", "In which rigidbody velocity will the object detects collision");
        GUILayout.EndVertical();


        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");
        ppDraw_Property(ppContinousDamage, "Continuous Effect", "If enabled, vertices of the mesh will be able to go over their origin");
        GUILayout.Space(5);
        ppDraw_Property(ppCollisionWithSpecificTag, "Collision With Specific Tag", "If enabled, collision will be allowed for objects with tag below...");
        if (m.ppCollisionWithSpecificTag)
            ppDraw_Property(ppCollisionTag, "Collision Tag");
        GUILayout.EndVertical();

        if (target != null)
            serializedObject.Update();

        GUILayout.Space(10);
        ppDraw_Property(ppEnableEvent, "Enable Event System", "If enabled, the event entry will show up");
        if(m.ppEnableEvent)
            ppDraw_Property(ppEvent, "Event After Collision", "Event will be occured after collision enter");

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
