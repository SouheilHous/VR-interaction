using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MDM_LandscapeTracking))]
[CanEditMultipleObjects]
public class MDM_LandscapeTracking_Editor : ModifiersEditorController
{
    MDM_LandscapeTracking m;

    SerializedProperty LT_virtualTrackCamera, LT_TrackerSource, LT_ViewSize, LT_VirtCameraHeight;

    private void OnEnable()
    {
        m = (MDM_LandscapeTracking)target;

        LT_virtualTrackCamera = serializedObject.FindProperty("LT_virtualTrackCamera");

        LT_TrackerSource = serializedObject.FindProperty("LT_TrackerSource");

        LT_ViewSize = serializedObject.FindProperty("LT_ViewSize");
        LT_VirtCameraHeight = serializedObject.FindProperty("LT_VirtCameraHeight");

    }

    private string LT_LayerName;
    private bool LT_Choose;
    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        ppDraw_Box("Object should contains MD_EasyMeshTracker shader.");
        GUILayout.Space(10);
        GUILayout.BeginVertical("Box");

        if (m.NotSet)
        {
            ppDraw_Box("The Tracking system is not yet set. Please write down your custom Tracking Layer name. The layer that all trackable objects will have an access to.", MessageType.Warning);
            if (!LT_Choose)
            {
                LT_LayerName = GUILayout.TextField(LT_LayerName);
                GUILayout.Label("Or you can choose an exists layer manually");
            }
            LT_Choose = GUILayout.Toggle(LT_Choose, "Choose layer manually");

            GUILayout.Space(10);

            if(GUILayout.Button("Apply Layer & Create All Requirements [RT, Camera]"))
            {
                if (!LT_Choose)
                {
                    if (string.IsNullOrEmpty(LT_LayerName))
                    {
                        EditorUtility.DisplayDialog("Error", "Please fill the layer name", "OK");
                        return;
                    }
                    LT_Internal_CreateLayer(LT_LayerName);
                }

                LT_Internal_CreateCamera();
                LT_Internal_CreateRT();
                m.NotSet = false;
            }
            return;
        }

        if(!m.LT_virtualTrackCamera)
        {
            ppDraw_Box("There is no Virtual Track Camera. Hit the reset button or fill the missing field!",MessageType.Error);
            ppDraw_Property(LT_virtualTrackCamera, "Virtual Track Camera", "");
            return;
        }
        if (!m.LT_TrackerSource)
        {
            ppDraw_Box("There is no Tracking RT Source. Hit the reset button or fill the missing field!", MessageType.Error);
            ppDraw_Property(LT_TrackerSource, "VT Tracker Source", "");
            return;
        }

        GUI.color = Color.gray;
        ppDraw_Property(LT_virtualTrackCamera, "Virtual Track Camera", "");
        ppDraw_Property(LT_TrackerSource, "VT Tracker Source", "");

        GUILayout.Space(10);

        GUI.color = Color.white;
        GUILayout.BeginVertical("Box");
        ppDraw_Property(LT_ViewSize, "VT Camera View Size", "");
        ppDraw_Property(LT_VirtCameraHeight, "VT Camera Height", "");
        GUILayout.EndVertical();

        GUILayout.EndVertical();

        if(GUILayout.Button("Clean Tracker Source RT"))
            m.LT_TrackerSource.Release();

        if (target != null)
            serializedObject.Update();

        ppAddMeshColliderRefresher(m);
        ppBackToMeshEditor(m);
    }

    private void LT_Internal_CreateCamera()
    {
        GameObject newCamera = new GameObject("TrackingCamera_"+m.name);
        Camera c = newCamera.AddComponent<Camera>();
        c.gameObject.layer = 30;
        c.transform.parent = m.transform;
        c.orthographic = true;
        c.clearFlags = CameraClearFlags.Nothing;
        c.nearClipPlane = 0.1f;
        c.farClipPlane = 200;
        c.depth = 0;
        c.allowHDR = false;
        c.allowMSAA = false;
        c.useOcclusionCulling = false;
        c.targetDisplay = 0;
        c.cullingMask = 1<<30;
        m.LT_virtualTrackCamera = c;
        newCamera.transform.parent = m.transform;
    }
    private void LT_Internal_CreateRT()
    {
        RenderTexture rt = new RenderTexture(500, 500, 0, RenderTextureFormat.Depth);
        rt.antiAliasing = 1;
        rt.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        AssetDatabase.CreateAsset(rt, "Assets/MDM_LT_"+m.name+"_RT.renderTexture");
        AssetDatabase.Refresh();
        if (m.gameObject.GetComponent<Renderer>() && m.gameObject.GetComponent<Renderer>().sharedMaterial && m.gameObject.GetComponent<Renderer>().sharedMaterial.HasProperty("_DispTex"))
            m.gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_DispTex", rt);
        else
        {
            try
            {
                Material mat = new Material(Shader.Find("Matej Vanco/Mesh Deformation Package/MD_EasyMeshTracker"));
                m.gameObject.GetComponent<Renderer>().sharedMaterial = mat;
                m.gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_DispTex", rt);
            }
            catch
            {
                Debug.LogError("Landscape Tracking error: your object doesn't contain EasyMeshTracker shader. Please check it properly! Create object with mesh filter and add material with shader Easy Mesh Tracker.");
            }
        }
        m.LT_TrackerSource = rt;
    }
    private void LT_Internal_CreateLayer(string LayerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");
        if (layers == null || !layers.isArray)
            return;

        SerializedProperty layerSP = layers.GetArrayElementAtIndex(30);
        layerSP.stringValue = LayerName;

        tagManager.ApplyModifiedProperties();
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
