using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_RaycastEvent))]
[CanEditMultipleObjects]
public class MDM_RaycastEvent_Editor : ModifiersEditorController
{

    SerializedProperty ppUpdateRayPerFrame;

    SerializedProperty ppDistanceRay;
    SerializedProperty ppPointRay;
    SerializedProperty ppSphericalRadius;

    SerializedProperty ppRaycastWithSpecificTag;
    SerializedProperty ppRaycastTag;

    SerializedProperty ppEventAfterRaycast, ppEventAfterRaycastExit;

    SerializedProperty ppSavePerformanceByRigidbody;
    SerializedProperty ppTargetRigidbody, ppTargetVelocitySpeed;

    MDM_RaycastEvent m;
    private void OnEnable()
    {
        m = (MDM_RaycastEvent)target;
        ppUpdateRayPerFrame = serializedObject.FindProperty("ppUpdateRayPerFrame");
        ppDistanceRay = serializedObject.FindProperty("ppDistanceRay");

        ppPointRay = serializedObject.FindProperty("ppPointRay");
        ppSphericalRadius = serializedObject.FindProperty("ppSphericalRadius");

        ppRaycastWithSpecificTag = serializedObject.FindProperty("ppRaycastWithSpecificTag");
        ppRaycastTag = serializedObject.FindProperty("ppRaycastTag");

        ppEventAfterRaycast = serializedObject.FindProperty("ppEventAfterRaycast");
        ppEventAfterRaycastExit = serializedObject.FindProperty("ppEventAfterRaycastExit");

        ppSavePerformanceByRigidbody = serializedObject.FindProperty("ppSavePerformanceByRigidbody");
        ppTargetRigidbody = serializedObject.FindProperty("ppTargetRigidbody");
        ppTargetVelocitySpeed = serializedObject.FindProperty("ppTargetVelocitySpeed");
    }

    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        GUILayout.Space(10);

        ppDraw_Property(ppUpdateRayPerFrame, "Update Ray Per Frame","If disabled, you are able to invoke your own method to Update ray state...");

        GUILayout.Space(5);

        ppDraw_Property(ppDistanceRay, "Ray Distance");

        GUILayout.Space(5);

        ppDraw_Property(ppPointRay, "Point Ray","If disabled, raycast will be generated as Spherical Ray!");
        if(!m.ppPointRay)
            ppDraw_Property(ppSphericalRadius, "Radius");

        GUILayout.Space(5);

        ppDraw_Property(ppRaycastWithSpecificTag, "Raycast Specific Tag", "If disabled, raycast will accept every object with collider");
        if(m.ppRaycastWithSpecificTag)
            ppDraw_Property(ppRaycastTag, "Raycast Tag");

        GUILayout.Space(5);

        ppDraw_Property(ppSavePerformanceByRigidbody, "Save Performance By Rigidbody", "If enabled, you must enter target rigidbody. If the rigidbody is assigned, the script will be updated only if the rigidbody velocity will be greater than 0.");
        if (m.ppSavePerformanceByRigidbody)
        {
            ppDraw_Property(ppTargetRigidbody, "Target Rigidbody");
            ppDraw_Property(ppTargetVelocitySpeed, "Target Velocity");
        }

        GUILayout.Space(10);

        ppDraw_Property(ppEventAfterRaycast, "Event Raycast Hit", "What happes after raycasting?");
        ppDraw_Property(ppEventAfterRaycastExit, "Event Raycast Exit", "What happes after raycast exit?");

        if (target != null)
            serializedObject.Update();

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
