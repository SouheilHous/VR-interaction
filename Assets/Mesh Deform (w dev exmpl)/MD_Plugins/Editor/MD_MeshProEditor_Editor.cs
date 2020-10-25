#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

[CustomEditor(typeof(MD_MeshProEditor))]
public class MD_MeshProEditor_Editor : Editor
{

    private bool _HaveMeshFilter;
    private bool _HaveMeshSkinned;

    SerializedProperty ppAnimationMode;
    SerializedProperty ppNewReferenceAfterCopy, ppDynamicMesh, ppOptimizeMesh;

    SerializedProperty ppCustomVerticePattern, ppVerticePatternObject, ppCustomVerticeColor, ppUseCustomColor;

    SerializedProperty ppINFO_MeshName;

    SerializedProperty ppPerformanceZone, ppEnablePerformanceSaver, ppPerformanceRadius;

    private void OnEnable()
    {
        ppEnablePerformanceSaver = serializedObject.FindProperty("ppEnablePerformanceSaver");
        ppPerformanceZone = serializedObject.FindProperty("ppPerformanceZone");
        ppPerformanceRadius = serializedObject.FindProperty("ppPerformanceRadius");

        ppAnimationMode = serializedObject.FindProperty("ppAnimationMode");
        ppNewReferenceAfterCopy = serializedObject.FindProperty("ppNewReferenceAfterCopy");
        ppDynamicMesh = serializedObject.FindProperty("ppDynamicMesh");
        ppOptimizeMesh = serializedObject.FindProperty("ppOptimizeMesh");

        ppCustomVerticePattern = serializedObject.FindProperty("ppCustomVerticePattern");
        ppVerticePatternObject = serializedObject.FindProperty("ppVerticePatternObject");
        ppCustomVerticeColor = serializedObject.FindProperty("ppCustomVerticeColor");
        ppUseCustomColor = serializedObject.FindProperty("ppUseCustomColor");

        ppINFO_MeshName = serializedObject.FindProperty("ppINFO_MeshName");
        Foldout = new bool[3];
    }


    //----GUI Stuff-------------
    GUIStyle style = new GUIStyle();

    public GUIStyle styleTest;
    public Texture2D _Decorate_VerticesIcon;
    public Texture2D _Decorate_ColliderIcon;
    public Texture2D _Decorate_IdentityIcon;
    public Texture2D _Decorate_ModifyIcon;

    public Texture2D _Decorate_SmoothIcon;
    public Texture2D _Decorate_SubdivisionIcon;
    //---------------------------

    //----Adds-------------------
    private bool[] Foldout = new bool[3];
    public float SmoothMeshIntens = 0.5f;
    public int[] DivisionLevel = new int[] { 0, 2, 3, 4, 6, 8, 9, 12, 16, 18, 24 };
    public int DivisionlevelSelection = 1;
    //---------------------------

    private void OnSceneGUI()
    {
        MD_MeshProEditor m = (MD_MeshProEditor)target;

        if (m.ppEnablePerformanceSaver && m.SelectedModification == MD_MeshProEditor.SelectedModification_.Vertices)
        {
            Vector3 Zone = m.ppPerformanceZone;
            float radius = m.ppPerformanceRadius;

            Handles.color = Color.magenta;
            Handles.CircleHandleCap(0,Zone, Quaternion.identity, radius,EventType.DragUpdated);
            Handles.CircleHandleCap(0, Zone, Quaternion.Euler(0,90,0), radius, EventType.DragUpdated);

            EditorGUI.BeginChangeCheck();
            Handles.color = Color.magenta;
            Handles.DrawWireDisc(Zone, Vector3.up, radius);
            Handles.DrawWireDisc(Zone, Vector3.right, radius);
            Zone = Handles.DoPositionHandle(Zone, m.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                m.ppPerformanceRadius = radius;
                m.ppPerformanceZone = Zone;
            }
        }
    }
   
    private void ClearVerticeEditor()
    {
        MD_MeshProEditor m = (MD_MeshProEditor)target;

        if (m == null)
            return;

        if (m.ppAnimationMode)
            return;

        if (m.points.Count > 0)
            m.points.Clear();

        if (m.ppVerticesRoot != null)
            DestroyImmediate(m.ppVerticesRoot.gameObject);
    }
    public override void OnInspectorGUI()
    {
        if (target == null)
            return;
        
        style.richText = true;
        MD_MeshProEditor m = (MD_MeshProEditor)target;

        _HaveMeshFilter = m.GetComponent<MeshFilter>();
        _HaveMeshSkinned = m.GetComponent<SkinnedMeshRenderer>();

        if (m == null)
        {
            EditorGUIUtility.ExitGUI();
            return;
        }
        if(!_HaveMeshFilter)
        {
            if (_HaveMeshSkinned)
            {
                if (GUILayout.Button("Compile to Mesh Filter"))
                    m.MeshEditor_CompileToMeshFilter();
                ppDraw_Box("Skinned Mesh Renderer is a component to control your mesh by bones. Press 'Compile To Mesh Filter' to start editing its mesh source.", MessageType.Info);
            }
            else
                ppDraw_Box("No Mesh Identity... Object must contains Mesh Filter or Skinned Mesh Renderer component to access mesh editor.", MessageType.Error);
            return;
        }

        GUILayout.Space(20);

        #region UpperCategories
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Vertices", _Decorate_VerticesIcon, "Vertices Modification")))
        {
            if (m.SelectedModification == MD_MeshProEditor.SelectedModification_.Vertices)
            {
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.None;
                ClearVerticeEditor();
            }
            else
            {
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.Vertices;
                m.MeshEditor_CreateVerticeEditor();
            }
        }
        if (GUILayout.Button(new GUIContent("Collider", _Decorate_ColliderIcon, "Collider Modification")))
        {
            ClearVerticeEditor();
            if (m.SelectedModification == MD_MeshProEditor.SelectedModification_.Collider)
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.None;
            else
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.Collider;
        }
        if (GUILayout.Button(new GUIContent("Identity", _Decorate_IdentityIcon, "Identity Modification")))
        {
            ClearVerticeEditor();
            if (m.SelectedModification == MD_MeshProEditor.SelectedModification_.Identity)
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.None;
            else
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.Identity;
        }
        if (GUILayout.Button(new GUIContent("Mesh", _Decorate_ModifyIcon, "Mesh Modification")))
        {
            ClearVerticeEditor();
            if (m.SelectedModification == MD_MeshProEditor.SelectedModification_.Mesh)
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.None;
            else
                m.SelectedModification = MD_MeshProEditor.SelectedModification_.Mesh;
        }
        GUILayout.EndHorizontal();
        #endregion


        #region Category_Vertices
        if(m.SelectedModification == MD_MeshProEditor.SelectedModification_.Vertices)
        {
            Color c = new Color();
            ColorUtility.TryParseHtmlString("#f2d3d3", out c);
            GUI.color = c;
            GUILayout.Label("| Vertices Modification");
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal("Box");
            ppDraw_Property(ppAnimationMode, "Animation Mode", "If enabled, the program will not refresh vertices and the mesh will keep generated points");
            GUILayout.EndHorizontal();
            ppDraw_Property(ppCustomVerticePattern, "Custom Vertice Pattern", "If enabled, you will be able to choose your own vertice object pattern");
            if (m.ppCustomVerticePattern)
            {
                ppDraw_Property(ppVerticePatternObject, "Vertice Object Pattern");
                ppDraw_Property(ppUseCustomColor, "Enable Custom Color");
                if(m.ppUseCustomColor)
                    ppDraw_Property(ppCustomVerticeColor, "Custom Vertice Color");
                ppDraw_Box("To show new vertice pattern, refresh vertice editor by clicking on Vertices Modification");
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Open Vertex Tool Window"))
                MD_VertexTool.Init();
            GUILayout.Space(5);
            if(m.ppINFO_Vertices>MD_MeshProEditor.VerticesLimit)
            {
                GUI.color = Color.yellow;
                GUILayout.BeginVertical("Box");
                ppDraw_Box("Your mesh has more than " + MD_MeshProEditor.VerticesLimit.ToString() + " vertices. All points have been automatically hidden. Use Performance Saver Mode to show only selected points or show all vertices on your own 'slow performance' risk. You will be very limited if the mesh has more than 10 000 vertices. Instead of this, you can use Sculpting Pro to edit extremely complex meshes.");
                if (GUILayout.Button("Activate All Points"))
                {
                    if (m.meshF.sharedMesh.vertices.Length > 10000)
                    {
                        EditorUtility.DisplayDialog("I'm Sorry", "The mesh has too many vertices [" + m.meshF.sharedMesh.vertices.Length + "]. You won't be able to proceed this function. It would really freeze your computer. [This message can be disabled in the code]", "OK");
                        return;
                    }
                    if (m.points.Count > 0)
                    {
                        foreach (Transform p in m.points)
                            p.gameObject.SetActive(true);
                    }
                    else
                        m.MeshEditor_CreateVerticeEditor(true);
                }
                if (GUILayout.Button("Deactivate All Points"))
                {
                    if (m.meshF.sharedMesh.vertices.Length > 10000)
                    {
                        EditorUtility.DisplayDialog("I'm Sorry", "The mesh has too many vertices [" + m.meshF.sharedMesh.vertices.Length + "]. You won't be able to proceed this function. It would really freeze your computer. [This message can be disabled in the code]", "OK");
                        return;
                    }
                    if (m.points.Count > 0)
                    {
                        foreach (Transform p in m.points)
                            p.gameObject.SetActive(false);
                    }
                }
                GUILayout.EndVertical();
            }
            ColorUtility.TryParseHtmlString("#f2d3d3", out c);
            GUI.color = c;
            GUILayout.Space(5);
            ppDraw_Property(ppEnablePerformanceSaver, "Performance Saver Mode", "If the mesh has too many vertices, you can save your performance by showing up only selected vertices according to zone position and radius");
            if (m.ppEnablePerformanceSaver)
            {
                GUILayout.BeginVertical("Box");
                ppDraw_Property(ppPerformanceZone, "Performance Zone");
                ppDraw_Property(ppPerformanceRadius, "Field Radius");

                GUILayout.BeginHorizontal("Box");
                if (GUILayout.Button("Generate Points In Zone"))
                {
                    if(m.meshF.sharedMesh.vertices.Length>10000)
                    {
                        EditorUtility.DisplayDialog("I'm Sorry", "The mesh has too many vertices [" + m.meshF.sharedMesh.vertices.Length + "]. You won't be able to proceed this function. It would really freeze your computer. [This message can be disabled in the code]", "OK");
                        return;
                    }
                    m.MeshEditor_CreateVerticeEditor(true);
                    if(m.points.Count>0)
                        for (int i = 0; i < m.points.Count; i++)
                        {
                            if (Vector3.Distance(m.points[i].transform.position, m.ppPerformanceZone) > m.ppPerformanceRadius)
                                m.points[i].gameObject.SetActive(false);
                            else
                                m.points[i].gameObject.SetActive(true);
                        }
                    return;
                }
                if(m.points.Count>0)
                    if (GUILayout.Button("Show All Points"))
                    {
                        if (m.meshF.sharedMesh.vertices.Length > 10000)
                        {
                            EditorUtility.DisplayDialog("I'm Sorry", "The mesh has too many vertices [" + m.meshF.sharedMesh.vertices.Length + "]. You won't be able to proceed this function. It would really freeze your computer. [This message can be disabled in the code]", "OK");
                            return;
                        }
                        if (m.points.Count > 0)
                            for (int i = 0; i < m.points.Count; i++)
                                m.points[i].gameObject.SetActive(true);
                        return;
                    }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                if (GUILayout.Button("Reset Zone Position"))
                    m.ppPerformanceZone = m.transform.position;
                GUILayout.EndVertical();
            }
            GUILayout.Space(5);
            if(m.ppAnimationMode)
            {
                GUILayout.BeginVertical("Box");
                GUILayout.Label("Animation Mode | Vertices Manager");
                GUILayout.BeginHorizontal("Box");
                if (GUILayout.Button("Show Vertices"))
                    m.MeshEditor_ShowHideVertices(true);
                if (GUILayout.Button("Hide Vertices"))
                    m.MeshEditor_ShowHideVertices(false);
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal("Box");
                if (GUILayout.Button("Ignore Raycast"))
                    m.MeshEditor_IgnoreRaycastVertices(true);
                if (GUILayout.Button("Default Layer"))
                    m.MeshEditor_IgnoreRaycastVertices(false);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        #endregion

        #region Category_Collider
        if (m.SelectedModification == MD_MeshProEditor.SelectedModification_.Collider)
        {
            Color c = new Color();
            ColorUtility.TryParseHtmlString("#7beb99", out c);
            GUI.color = c;
            GUILayout.Label("| Collider Modification");
            if (!m.GetComponent<MD_MeshColliderRefresher>())
            {
                GUILayout.BeginVertical("Box");

                if (GUILayout.Button("Add Mesh Collider"))
                    m.gameObject.AddComponent<MD_MeshColliderRefresher>();

                GUILayout.EndVertical();
            }
            else
                ppDraw_Box("The selected object already contains MD Mesh Collider component");
        }
        #endregion

        #region Category_Identity
        if (m.SelectedModification == MD_MeshProEditor.SelectedModification_.Identity)
        {
            Color c = new Color();
            ColorUtility.TryParseHtmlString("#baefff", out c);
            GUI.color = c;
            GUILayout.Label("| Identity Modification");

            GUILayout.BeginVertical("Box");

            if (GUILayout.Button("Create New Mesh Reference"))
            {
                m.MeshEditor_CreateNewReference();
                return;
            }
            if (m.transform.childCount > 0 && m.transform.GetChild(0).GetComponent<MeshFilter>())
            {
                if (GUILayout.Button("Combine All SubMeshes"))
                { 
                    m.MeshEditor_CombineMesh();
                    return;
                }
            }
            if (GUILayout.Button("Save Mesh To Assets"))
                m.MD_INTERNAL_TECH_SaveMeshToAssets();

            GUILayout.Space(5);
            if (GUILayout.Button("Recalculate Normals") && m.meshF)
                m.meshF.sharedMesh.RecalculateNormals();
            GUILayout.Space(5);

            ppDraw_Property(ppNewReferenceAfterCopy, "Create New Reference After Copy-Paste", "If enabled, the new mesh reference will be created with clearly new mesh data.");
            ppDraw_Property(ppDynamicMesh, "Update Mesh Every Frame", "If enabled, the mesh will be updated every frame and you will be able to deform the mesh at runtime if it's possible.");
            if(m.ppDynamicMesh)
                ppDraw_Property(ppOptimizeMesh, "Optimize Mesh", "If enabled, the mesh will reduce refreshing and recalculating of Bounds and Normals.");
            GUILayout.EndVertical();
        }
        #endregion

        #region Category_Mesh
        if (m.SelectedModification == MD_MeshProEditor.SelectedModification_.Mesh)
        {
            Color c = new Color();
            ColorUtility.TryParseHtmlString("#dee7ff", out c);
            GUI.color = c;
            GUILayout.Label("| Mesh Modification");

            GUILayout.BeginVertical("Box");

            GUILayout.Label("Internal Mesh Modifiers");
            EditorGUI.indentLevel++;
            Foldout[0] = EditorGUILayout.Foldout(Foldout[0], new GUIContent("Mesh Smooth", _Decorate_SmoothIcon, "Smooth mesh by the smooth level"));
            if (Foldout[0])
            {
                EditorGUI.indentLevel++;
                SmoothMeshIntens = EditorGUILayout.Slider("Subdivision Level", SmoothMeshIntens,0.5f,0.05f);
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 10);
                if (GUILayout.Button(new GUIContent("Smooth Mesh", _Decorate_SmoothIcon)))
                    m.MeshEditor_SmoothMesh(SmoothMeshIntens);
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
            Foldout[1] = EditorGUILayout.Foldout(Foldout[1], new GUIContent("Mesh Subdivision", _Decorate_SubdivisionIcon,"Subdivide mesh by the subdivision level"));
            if (Foldout[1])
            {
                EditorGUI.indentLevel++;
                DivisionlevelSelection = EditorGUILayout.IntSlider("Subdivision Level", DivisionlevelSelection, 0,DivisionLevel[DivisionLevel.Length-1]);
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 10);
                if (GUILayout.Button(new GUIContent("Subdivide Mesh", _Decorate_SubdivisionIcon)))
                    m.MeshEditor_SubdivideMesh(DivisionlevelSelection);
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            serializedObject.Update();
            GUILayout.Space(10);

            GUILayout.Label("External Mesh Modifiers");
            EditorGUI.indentLevel++;
            Foldout[2] = EditorGUILayout.Foldout(Foldout[2], "Modifiers");
            if (Foldout[2])
            {
                EditorGUI.indentLevel++;

                ColorUtility.TryParseHtmlString("#c2c2c2", out c);
                GUI.color = c;
                GUILayout.Label("Logical Deformers");
                if (GUILayout.Button(new GUIContent("Mesh Noise")))
                {
                    m.gameObject.AddComponent<MDM_MeshNoise>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }
                if (GUILayout.Button(new GUIContent("Mesh Morpher")))
                {
                    m.gameObject.AddComponent<MDM_Morpher>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }
                if (GUILayout.Button(new GUIContent("Mesh FFD & Simple Deformer")))
                {
                    m.gameObject.AddComponent<MDM_FFD>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }

                ColorUtility.TryParseHtmlString("#dedba0", out c);
                GUI.color = c;
                GUILayout.Label("World Interactive");
                if (GUILayout.Button(new GUIContent("Interactive Landscape")))
                {
                    m.gameObject.AddComponent<MDM_InteractiveLandscape>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }
                if (GUILayout.Button(new GUIContent("Landscape Tracking")))
                {
                    m.gameObject.AddComponent<MDM_LandscapeTracking>();
                    DestroyImmediate(m);
                    return;
                }
                if (GUILayout.Button(new GUIContent("Mesh Damage")))
                {
                    m.gameObject.AddComponent<MDM_MeshDamage>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }
                if (GUILayout.Button(new GUIContent("Mesh Fit")))
                {
                    m.gameObject.AddComponent<MDM_MeshFit>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }
                if (GUILayout.Button(new GUIContent("Melt Controller")))
                {
                    m.gameObject.AddComponent<MDM_MeltController>();
                    DestroyImmediate(m);
                    return;
                }

                ColorUtility.TryParseHtmlString("#ebebeb", out c);
                GUI.color = c;
                GUILayout.Label("Mesh Events");
                if (GUILayout.Button(new GUIContent("Raycast Event")))
                {
                    m.gameObject.AddComponent<MDM_RaycastEvent>();
                    DestroyImmediate(m);
                    return;
                }

                ColorUtility.TryParseHtmlString("#aae0b2", out c);
                GUI.color = c;
                GUILayout.Label("Basics");
                if (GUILayout.Button(new GUIContent("Twist")))
                {
                    m.gameObject.AddComponent<MDM_Twist>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }
                if (GUILayout.Button(new GUIContent("Bend")))
                {
                    m.gameObject.AddComponent<MDM_Bend>().ppCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }

                ColorUtility.TryParseHtmlString("#aad2e0", out c);
                GUI.color = c;
                GUILayout.Label("Sculpting");
                if (GUILayout.Button(new GUIContent("Sculpting Pro")))
                {
                    m.gameObject.AddComponent<MDM_SculptingPro>().SSCreateNewReference = false;
                    DestroyImmediate(m);
                    return;
                }

                ColorUtility.TryParseHtmlString("#dee7ff", out c);
                GUI.color = c;
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("All external modifiers run in play mode.", MessageType.Info);
            }
            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
        }
        #endregion


        #region Bottom Categories
        GUILayout.Space(20);
        GUI.color = Color.white;
        GUILayout.Label("Mesh Information");
        GUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        ppDraw_Property(ppINFO_MeshName, "Mesh Name","Mesh name... Change mesh name and Refresh Identity.");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Vertices:");
        GUILayout.TextField(m.ppINFO_Vertices.ToString());
        GUILayout.Label("Triangles:");
        GUILayout.TextField(m.ppINFO_Triangles.ToString());
        GUILayout.Label("Normals:");
        GUILayout.TextField(m.ppINFO_Normals.ToString());
        GUILayout.Label("UVs:");
        GUILayout.TextField(m.ppINFO_Uvs.ToString());
        GUILayout.EndHorizontal();

        if(GUILayout.Button("Restore Original Mesh"))
            m.MeshEditor_RestoreMeshToOriginal();
        GUILayout.EndVertical();
        #endregion
        GUILayout.Space(10);

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
#endif
