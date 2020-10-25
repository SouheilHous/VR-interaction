using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Mesh Pro Editor")]
    public class MD_MeshProEditor : MonoBehaviour 
    {

        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Collection): Mesh Pro Editor = Component for objects with Mesh Renderer
        //---Main mesh editor & controller
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        //---Basic Selection
        public enum SelectedModification_ { None, Vertices, Collider, Identity, Mesh };
        public SelectedModification_ SelectedModification;

        //---Basic Booleans to Set Up
        public bool ppNewReferenceAfterCopy = true;
        public bool ppDynamicMesh = true;
        public bool ppAnimationMode = false;
        public bool ppOptimizeMesh = false;

        #region Vertices Editor Variables
        public Transform ppVerticesRoot;
        public bool ppCustomVerticePattern = false;
        public bool ppUseCustomColor = true;
        public Color ppCustomVerticeColor = Color.red;
        public GameObject ppVerticePatternObject;
        #endregion

        #region Basic Mesh Info Variables
        public string ppINFO_MeshName;
        public int ppINFO_Vertices = 0;
        public int ppINFO_Triangles = 0;
        public int ppINFO_Normals = 0;
        public int ppINFO_Uvs = 0;
        //---Stored Original Mesh
        public Vector3[] originalVertices;
        public int[] originalTriangles;
        public Vector3[] originalNormals;
        public Vector2[] originalUVS;
        #endregion

        //---Vertices & Points for editing
        public List<Vector3> verts = new List<Vector3>();
        public List<Transform> points = new List<Transform>();

        //---Default Material
        private Material DefaultMaterial;
        //---Vertices limit to edit [YOU CAN EDIT VERTICES LIMIT COUNT HERE]------------------------------------
        public static int VerticesLimit = 2000;
        private bool DeselectObjectAfterVerticeLimitAgreement;

        public bool _AlreadyAwake = false;
        public bool _BornAsSkinnedMesh = false;

        public MeshFilter meshF;

        public bool ppEnablePerformanceSaver = false;
        public Vector3 ppPerformanceZone;
        public float ppPerformanceRadius = 0.5f;

        public Vector3 myStartupBounds;

        //---Awake Start Up & Getting Ready
        void Awake () 
        {
            if (meshF == null && GetComponent<MeshFilter>() && GetComponent<MeshFilter>().sharedMesh != null)
                meshF = GetComponent<MeshFilter>();

            if (!MD_INTERNAL_TECH_CheckModifiers(this.gameObject))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayDialog("Error", "The Mesh Editor cannot be applied to this object, because the object already contains modifiers. Please, remove modifier to access mesh editor.", "OK");
#endif
                DestroyImmediate(this);
                return;
            }

            if(DefaultMaterial == null)
                DefaultMaterial = new Material(Shader.Find("Standard"));

            if (!_AlreadyAwake)
            {
                if (string.IsNullOrEmpty(ppINFO_MeshName))
                    ppINFO_MeshName = "NewMesh" + Random.Range(1, 9999).ToString();

                if (GetComponent<MeshFilter>() && GetComponent<MeshFilter>().sharedMesh != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying && UnityEditor.EditorUtility.DisplayDialog("Create a New Reference?", "Would you like to create a new reference? If you agree [recommended], you will create a new mesh reference... If you disagree, exist mesh references will share the same data as the new mesh reference...", "yes", "no"))
                        MD_INTERNAL_TECH_ResetMeshReference();
#else
                    if (!ppAnimationMode)
                        MD_INTERNAL_TECH_ResetMeshReference();
#endif
                    myStartupBounds = meshF.sharedMesh.bounds.max;
                    MD_INTERNAL_TECH_GetMeshInfo();
                    //ShowStartDebug = true;
                }

                _AlreadyAwake = true;
            }
            else if (ppNewReferenceAfterCopy)
                MD_INTERNAL_TECH_ResetMeshReference();
        }

        #region INTERNAL - TECHNICAL - Methods

        //---Checking modifiers [with modifiers you can't edit mesh]
        public static bool MD_INTERNAL_TECH_CheckModifiers(GameObject Sender, string ExceptionBehaviour = "")
        {
            foreach (MonoBehaviour beh in Sender.GetComponents(typeof(MonoBehaviour)))
            {
                if (beh == null)
                    continue;

                if (string.IsNullOrEmpty(ExceptionBehaviour))
                {
                    if (beh.GetType().Name == "AdvancedPlane" || beh.GetType().Name == "AdvancedShape" || beh.GetType().Name.Contains("MDM_"))
                        return false;
                }
                else
                {
                    if ((beh.GetType().Name == "AdvancedPlane" || beh.GetType().Name == "AdvancedShape" || beh.GetType().Name.Contains("MDM_")) && beh.GetType().Name != ExceptionBehaviour)
                        return false;
                }
            }
            return true;
        }

        //---Reset Mesh Reference & Identity
        private void MD_INTERNAL_TECH_ResetMeshReference()
        {
            if (meshF == null || !GetComponent<MeshFilter>())
                return;

            if (SelectedModification == SelectedModification_.Vertices)
                MeshEditor_CreateVerticeEditor();
            else
                MeshEditor_ClearVerticeEditor();

            Mesh newMesh = new Mesh();

            newMesh.vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
            newMesh.triangles = GetComponent<MeshFilter>().sharedMesh.triangles;
            newMesh.normals = GetComponent<MeshFilter>().sharedMesh.normals;
            newMesh.uv = GetComponent<MeshFilter>().sharedMesh.uv;
            newMesh.bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();
            newMesh.MarkDynamic();

            newMesh.name = ppINFO_MeshName;
            GetComponent<MeshFilter>().sharedMesh = newMesh;

            MD_INTERNAL_TECH_GetMeshInfo();
        }

        //---Refresh & Get Basic Mesh Info
        private void MD_INTERNAL_TECH_GetMeshInfo()
        {
            Mesh myMesh = GetComponent<MeshFilter>().sharedMesh;
            ppINFO_Vertices = myMesh.vertices.Length;
            ppINFO_Triangles = myMesh.triangles.Length;
            ppINFO_Normals = myMesh.normals.Length;
            ppINFO_Uvs = myMesh.uv.Length;

            if (!_AlreadyAwake)
            {
                originalVertices = myMesh.vertices;
                originalTriangles = myMesh.triangles;
                originalNormals = myMesh.normals;
                originalUVS = myMesh.uv;
            }
            meshF = GetComponent<MeshFilter>();
        }

        //---Save mesh to assets if in editor
        public void MD_INTERNAL_TECH_SaveMeshToAssets()
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

#if UNITY_EDITOR
            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Please enter the path to save your Mesh to Assets as Prefab", ppINFO_MeshName, "asset", "Please enter path");

            if (!string.IsNullOrEmpty(path))
            {
                string UniquePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);

                try
                {
                    UnityEditor.AssetDatabase.CreateAsset(meshF.sharedMesh, UniquePath);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();

                    MD_Debug.Debug(this, "Mesh has been successfully saved to: " + path, MD_Debug.DebugType.Information);
                }
                catch (UnityException e)
                {
                    Debug.LogError(e.Message + "_______To Unique Path: " + UniquePath);
                }
            }
            else
                return;
#endif

        }

        #endregion

        //---Update Mesh State [Where the magic happens!]
        void Update () 
        {
            if (ppDynamicMesh)
                MD_INTERNAL_FUNCT_UpdateMeshState();
        }

        public void MD_INTERNAL_FUNCT_UpdateMeshState()
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                if(Application.isPlaying)
                    MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            if (ppVerticesRoot == null)
                return;
            if (verts.Count > 0 && points.Count > 0)
            {
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;

                MeshFilter mesh = meshF;
                for (int i = 0; i < verts.Count; i++)
                {
                    if (points.Count > 0 && points.Count > i)
                    {
                        if(points[i] != null)
                            verts[i] = new Vector3(points[i].position.x - (mesh.transform.position.x - Vector3.zero.x), points[i].position.y - (mesh.transform.position.y - Vector3.zero.y), points[i].position.z - (mesh.transform.position.z - Vector3.zero.z));
                    }
                }
                mesh.sharedMesh.vertices = verts.ToArray();
            }

            if (!ppOptimizeMesh)
            {
                meshF.sharedMesh.RecalculateNormals();
                meshF.sharedMesh.RecalculateBounds();
            }
        }


        #region PUBLIC - Vertices, Identity - Methods

        /// <summary>
        /// Public function: Hide or Show generated points on the mesh
        /// </summary>
        /// <param name="Activation">Activate - true, Deactivate - false</param>
        public void MeshEditor_ShowHideVertices(bool Activation)
        {
            if (ppVerticesRoot == null)
                return;
            foreach (Renderer r in ppVerticesRoot.GetComponentsInChildren<Renderer>())
            {
                if (r != null)
                    r.enabled = Activation;
            }
        }
        /// <summary>
        /// Public function: Ignore raycast layer for vertices
        /// </summary>
        public void MeshEditor_IgnoreRaycastVertices(bool IgnoreRaycast)
        {
            if (ppVerticesRoot == null)
                return;
            foreach (Renderer r in ppVerticesRoot.GetComponentsInChildren<Renderer>())
            {
                if (r != null)
                {
                    if (IgnoreRaycast)
                        r.gameObject.layer = 2;
                    else
                        r.gameObject.layer = 0;
                }
            }
        }

        /// <summary>
        /// Public function: Create vertice editor
        /// </summary>
        /// <param name="verticesLimitAsk">Notification box will show up if the vertices limit is greater than the constant [only in Unity Editor]</param>
        public void MeshEditor_CreateVerticeEditor(bool PassTheVerticeLimit = false)
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            if (ppAnimationMode)
                return;

            MeshEditor_ClearVerticeEditor();

            if (this.meshF.sharedMesh.vertices.Length > VerticesLimit && !PassTheVerticeLimit)
            {
                DeselectObjectAfterVerticeLimitAgreement = true;
                SelectedModification = SelectedModification_.Vertices;
                ppEnablePerformanceSaver = true;
                ppPerformanceZone = transform.position + Vector3.one;
                return;
            }
            transform.parent = null;

            Vector3 LastScale = transform.localScale;
            Quaternion LastRotation = transform.rotation;

            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            GameObject _VertexRoot = new GameObject(name + "_VertexRoot");
            ppVerticesRoot = _VertexRoot.transform;
            _VertexRoot.transform.position = Vector3.zero;

            verts.Clear();
            points.Clear();

            //---Generating Points & Vertices
            var vertices = meshF.sharedMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                GameObject gm = null;

                if (ppCustomVerticePattern && ppVerticePatternObject != null)
                {
                    gm = Instantiate(ppVerticePatternObject);
                    if (gm.GetComponent<Renderer>() && ppUseCustomColor)
                        gm.GetComponent<Renderer>().sharedMaterial.color = ppCustomVerticeColor;
                }
                else
                {
                    Material new_Mat = new Material(Shader.Find("Unlit/Color"));
                    new_Mat.color = ppCustomVerticeColor;
                    gm = OctahedronMesh_Generator.Generate();
                    gm.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    gm.GetComponentInChildren<Renderer>().material = new_Mat;
                }

                gm.transform.parent = _VertexRoot.transform;

                gm.transform.position = vertices[i];
                verts.Add(vertices[i]);
                points.Add(gm.transform);

                gm.name = "P" + i.ToString();
            }

            //---Fixing Point Hierarchy
            foreach (Transform vertice in points)
            {
                foreach (Transform vertice2 in points)
                {
                    if (vertice.transform.position == vertice2.transform.position)
                    {
                        vertice.hideFlags = HideFlags.HideInHierarchy;
                        vertice.transform.parent = vertice2.transform;
                        vertice.gameObject.SetActive(false);
                    }
                }
            }

            //---Renaming Points
            int counter = 1;
            foreach (Transform vertice in _VertexRoot.transform)
            {
                vertice.hideFlags = HideFlags.None;
                vertice.gameObject.SetActive(true);
                vertice.name = "P" + counter.ToString();
                counter++;
            }

            meshF.sharedMesh.vertices = vertices;
            meshF.sharedMesh.RecalculateBounds();
            meshF.sharedMesh.RecalculateNormals();
            meshF.sharedMesh.MarkDynamic();

            _VertexRoot.transform.parent = meshF.transform;
            _VertexRoot.transform.localPosition = Vector3.zero;

            if (!_BornAsSkinnedMesh)
            {
                _VertexRoot.transform.localScale = LastScale;
                _VertexRoot.transform.rotation = LastRotation;
            }

            MD_INTERNAL_TECH_GetMeshInfo();

#if UNITY_EDITOR
            if (DeselectObjectAfterVerticeLimitAgreement)
            {
                UnityEditor.Selection.activeObject = null;
                foreach (Transform p in points)
                    p.gameObject.SetActive(false);
            }
            DeselectObjectAfterVerticeLimitAgreement = false;
#endif
        }
        /// <summary>
        /// Public function: Clear vertice editor
        /// </summary>
        public void MeshEditor_ClearVerticeEditor()
        {
            if (ppAnimationMode)
            {
                MD_Debug.Debug(this, "Object " + this.name + " is in Animation Mode. Vertices cannot be cleared!");
                return;
            }

            if (points.Count > 0)
                points.Clear();

            if (ppVerticesRoot != null)
                DestroyImmediate(ppVerticesRoot.gameObject);
        }

        /// <summary>
        /// Public function: Combine all meshes that the current mesh has as a parent
        /// </summary>
        public void MeshEditor_CombineMesh()
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            MeshEditor_ClearVerticeEditor();
#if UNITY_EDITOR
                if (!Application.isPlaying && UnityEditor.EditorUtility.DisplayDialog("Are you sure you want to combine meshes?", "If you combine the meshes, materials & all components will be lost. Are you sure you want to combine meshes?", "Yes, proceed", "No, cancel"))
                { }
                else
                    return;
#endif
            transform.parent = null;
            Vector3 Last_POS = transform.position;

            transform.position = Vector3.zero;

            MeshFilter[] meshes_ = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combiners_ = new CombineInstance[meshes_.Length];

            int counter_ = 0;
            while (counter_ < meshes_.Length)
            {
                combiners_[counter_].mesh = meshes_[counter_].sharedMesh;
                combiners_[counter_].transform = meshes_[counter_].transform.localToWorldMatrix;
                if (meshes_[counter_].gameObject != this.gameObject)
                    DestroyImmediate(meshes_[counter_].gameObject);

                counter_++;
            }

            GameObject newgm = new GameObject();
            newgm.AddComponent<MeshFilter>();
            newgm.AddComponent<MeshRenderer>();
            newgm.name = name;

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combiners_);

            newgm.GetComponent<MeshFilter>().sharedMesh = newMesh;
            newgm.GetComponent<MeshFilter>().sharedMesh.name = ppINFO_MeshName;
            newgm.GetComponent<Renderer>().material = DefaultMaterial;
            newgm.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
            newgm.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
            newgm.AddComponent<MD_MeshProEditor>().ppINFO_MeshName = ppINFO_MeshName;

            newgm.transform.position = Last_POS;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Selection.activeGameObject = newgm;
                UnityEditor.EditorUtility.DisplayDialog("Successfully combined. Notice please...", "If your mesh has been successfully combined, please notice that the prefab of the 'old' mesh in Assets Folder is no more valid for the new one. " +
                    "If you want to store the new mesh, you have to save your mesh prefab again.", "OK");
            }
#endif

            DestroyImmediate(gameObject);
        }
        /// <summary>
        /// Public function: Combine all meshes that the current mesh has as a parent [this will not create a new object]
        /// </summary>
        public void MeshEditor_QuickCombineMesh()
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            MeshEditor_ClearVerticeEditor();

            transform.parent = null;

            Vector3 Last_POS = transform.position;
            transform.position = Vector3.zero;

            MeshFilter[] meshes_ = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combiners_ = new CombineInstance[meshes_.Length];

            long counter_ = 0;
            while (counter_ < meshes_.Length)
            {
                combiners_[counter_].mesh = meshes_[counter_].sharedMesh;
                combiners_[counter_].transform = meshes_[counter_].transform.localToWorldMatrix;
                if (meshes_[counter_].gameObject != this.gameObject)
                    DestroyImmediate(meshes_[counter_].gameObject);

                counter_++;
            }

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combiners_);

            GetComponent<MeshFilter>().sharedMesh = newMesh;
            GetComponent<MeshFilter>().sharedMesh.name = ppINFO_MeshName;
            GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
            GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
            SelectedModification = SelectedModification_.None;

            transform.position = Last_POS;

            MD_INTERNAL_TECH_GetMeshInfo();
        }

        /// <summary>
        /// Public function: Create new reference - this will create a totally new object so your components will be lost
        /// </summary>
        public void MeshEditor_CreateNewReference()
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            MeshEditor_ClearVerticeEditor();

            GameObject newgm = new GameObject();
            newgm.AddComponent<MeshFilter>();
            newgm.AddComponent<MeshRenderer>();
            newgm.name = name;

            Material[] Materials = GetComponent<Renderer>().sharedMaterials;

            Vector3 Last_POS = transform.position;

            transform.position = Vector3.zero;

            CombineInstance[] combine = new CombineInstance[1];
            combine[0].mesh = GetComponent<MeshFilter>().sharedMesh;
            combine[0].transform = GetComponent<MeshFilter>().transform.localToWorldMatrix;

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combine);

            newgm.GetComponent<MeshFilter>().sharedMesh = newMesh;
            newgm.AddComponent<MD_MeshProEditor>()._AlreadyAwake = true;
            newgm.GetComponent<MD_MeshProEditor>().ppINFO_MeshName = ppINFO_MeshName;
            newgm.GetComponent<MeshFilter>().sharedMesh.name = ppINFO_MeshName;

            if (Materials.Length > 0)
                newgm.GetComponent<Renderer>().sharedMaterials = Materials;
            newgm.transform.position = Last_POS;
            newgm.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
            newgm.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Selection.activeGameObject = newgm;

                UnityEditor.EditorUtility.DisplayDialog("Notice please...", "If you change the reference of your mesh, please notice that the prefab of the 'old' mesh in Assets Folder is no more valid for the new one. " +
                    "If you want to store the new mesh, you have to save your mesh prefab again.", "OK");
            }
#endif
            newgm.GetComponent<MD_MeshProEditor>().meshF = newgm.GetComponent<MeshFilter>();
            DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Public function: Restore mesh from the original mesh
        /// </summary>
        public void MeshEditor_RestoreMeshToOriginal()
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying && !UnityEditor.EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to restore original mesh data?", "Restore", "Cancel"))
                return;
#endif

            MeshEditor_ClearVerticeEditor();

            meshF.sharedMesh.vertices = originalVertices;
            meshF.sharedMesh.triangles = originalTriangles;
            meshF.sharedMesh.normals = originalNormals;
            meshF.sharedMesh.uv = originalUVS;
            meshF.sharedMesh.RecalculateBounds();
            meshF.sharedMesh.RecalculateNormals();

            MD_INTERNAL_TECH_GetMeshInfo();

            SelectedModification = SelectedModification_.None;
        }

        /// <summary>
        /// Public function: Compile mesh to the Mesh Filter [if it's skinned mesh]
        /// </summary>
        public void MeshEditor_CompileToMeshFilter()
        {
            if (GetComponent<SkinnedMeshRenderer>() && GetComponent<SkinnedMeshRenderer>().sharedMesh != null)
            {
                GameObject newgm = new GameObject();
                newgm.AddComponent<MeshFilter>();
                newgm.AddComponent<MeshRenderer>();
                newgm.name = name + "_CompiledMesh";

                Material[] mater = null;

                if (GetComponent<SkinnedMeshRenderer>().sharedMaterials.Length > 0)
                    mater = GetComponent<Renderer>().sharedMaterials;

                Vector3 Last_POS = transform.root.transform.position;
                Vector3 Last_SCA = transform.localScale;
                Quaternion Last_ROT = transform.rotation;

                transform.position = Vector3.zero;

                Mesh newMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;

                newgm.GetComponent<MeshFilter>().sharedMesh = newMesh;
                newgm.GetComponent<MeshFilter>().sharedMesh.name = ppINFO_MeshName;
                if (mater.Length != 0)
                    newgm.GetComponent<Renderer>().sharedMaterials = mater;

                newgm.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
                newgm.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
                newgm.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Selection.activeGameObject = newgm;
                    UnityEditor.EditorUtility.DisplayDialog("Successfully Compiled!", "Your skinned mesh renderer has been successfully compiled to the Mesh Filter.", "OK");
                }
#endif

                newgm.AddComponent<MD_MeshProEditor>()._BornAsSkinnedMesh = true;

                newgm.transform.position = Last_POS;
                newgm.transform.rotation = Last_ROT;
                newgm.transform.localScale = Last_SCA;

                DestroyImmediate(gameObject);
            }
        }


        private Mesh sourceMesh;
        private Mesh workingMesh;
        /// <summary>
        /// Public function: Smooth mesh
        /// </summary>
        public void MeshEditor_SmoothMesh(float Intensity = 0.5f)
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            MeshEditor_ClearVerticeEditor();

#if UNITY_EDITOR
            if (this.meshF.sharedMesh.vertices.Length > VerticesLimit && !Application.isPlaying)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Mesh has more than " + VerticesLimit.ToString() + " vertices", "Your selected mesh has more than " + VerticesLimit.ToString() + " vertices [" + GetComponent<MeshFilter>().sharedMesh.vertices.Length.ToString() + "]. This could slow the editor performance. Would you like to continue?", "Yes, process", "No, cancel"))
                { DeselectObjectAfterVerticeLimitAgreement = true; }
                else
                    return;
            }
#endif

            sourceMesh = new Mesh();
            sourceMesh = meshF.sharedMesh;
            workingMesh = MD_REQUIRER_SmoothMesh(sourceMesh);
            meshF.mesh = workingMesh;

            for (int i = 0; i < 1; i++)
                workingMesh.vertices = MD_SmoothFunct.HC_Filterer(sourceMesh.vertices, workingMesh.vertices, workingMesh.triangles, 0.0f, Intensity);

            Mesh m = new Mesh();

            m.name = ppINFO_MeshName;
            m.vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
            m.triangles = GetComponent<MeshFilter>().sharedMesh.triangles;
            m.uv = GetComponent<MeshFilter>().sharedMesh.uv;
            m.normals = GetComponent<MeshFilter>().sharedMesh.normals;
            m.RecalculateBounds();
            m.RecalculateNormals();

            verts.Clear();
            points.Clear();

            m = workingMesh;

            GetComponent<MeshFilter>().sharedMesh = m;

            MD_INTERNAL_TECH_GetMeshInfo();

#if UNITY_EDITOR
            if (DeselectObjectAfterVerticeLimitAgreement)
                UnityEditor.Selection.activeObject = null;
            DeselectObjectAfterVerticeLimitAgreement = false;
#endif
        }
        private static Mesh MD_REQUIRER_SmoothMesh(Mesh mesh)
        {
            Mesh clone = new Mesh();

            clone.vertices = mesh.vertices;
            clone.normals = mesh.normals;
            clone.tangents = mesh.tangents;
            clone.triangles = mesh.triangles;

            clone.uv = mesh.uv;
            clone.uv2 = mesh.uv2;
            clone.uv2 = mesh.uv2;

            clone.bindposes = mesh.bindposes;
            clone.boneWeights = mesh.boneWeights;
            clone.bounds = mesh.bounds;

            clone.colors = mesh.colors;
            clone.name = mesh.name;
            return clone;
        }
        /// <summary>
        /// Public function: subdivide mesh
        /// </summary>
        /// <param name="Level"></param>
        public void MeshEditor_SubdivideMesh(int Level)
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            MeshEditor_ClearVerticeEditor();

#if UNITY_EDITOR
            if (this.meshF.sharedMesh.vertices.Length > VerticesLimit && !Application.isPlaying)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Mesh has more than " + VerticesLimit.ToString() + " vertices", "Your selected mesh has more than " + VerticesLimit.ToString() + " vertices [" + GetComponent<MeshFilter>().sharedMesh.vertices.Length.ToString() + "]. This could slow the editor performance. Would you like to continue?", "Yes, process", "No, cancel"))
                { DeselectObjectAfterVerticeLimitAgreement = true; }
                else
                    return;
            }
#endif

            sourceMesh = new Mesh();
            sourceMesh = meshF.sharedMesh;
            MD_SmoothDivisions.Subdivide(sourceMesh, Level);
            meshF.sharedMesh = sourceMesh;

            Mesh m = new Mesh();

            m.name = ppINFO_MeshName;
            m.vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
            m.triangles = GetComponent<MeshFilter>().sharedMesh.triangles;
            m.uv = GetComponent<MeshFilter>().sharedMesh.uv;
            m.normals = GetComponent<MeshFilter>().sharedMesh.normals;
            m.RecalculateBounds();
            m.RecalculateNormals();

            verts.Clear();
            points.Clear();

            m = sourceMesh;

            GetComponent<MeshFilter>().sharedMesh = m;

            MD_INTERNAL_TECH_GetMeshInfo();

#if UNITY_EDITOR
            if (DeselectObjectAfterVerticeLimitAgreement)
                UnityEditor.Selection.activeObject = null;
            DeselectObjectAfterVerticeLimitAgreement = false;
#endif

        }

        #endregion


        #region STATIC - Methods

        public static void MeshEditor_STATIC_CreateNewReference(GameObject EntryObject)
        {
            if (!EntryObject.GetComponent<MeshFilter>() || (EntryObject.GetComponent<MeshFilter>() && EntryObject.GetComponent<MeshFilter>().sharedMesh == null))
            {
                MD_MeshProEditor m = new MD_MeshProEditor();
                MD_Debug.Debug(m, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                return;
            }

            Mesh newMesh = new Mesh();
            newMesh.name = EntryObject.name;
            newMesh.vertices = EntryObject.GetComponent<MeshFilter>().sharedMesh.vertices;
            newMesh.triangles = EntryObject.GetComponent<MeshFilter>().sharedMesh.triangles;
            newMesh.normals = EntryObject.GetComponent<MeshFilter>().sharedMesh.normals;
            newMesh.uv = EntryObject.GetComponent<MeshFilter>().sharedMesh.uv;
            newMesh.MarkDynamic();
            EntryObject.GetComponent<MeshFilter>().sharedMesh.name = EntryObject.name;

            EntryObject.GetComponent<MeshFilter>().sharedMesh = newMesh;
            EntryObject.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
            EntryObject.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
        }

        public static void MeshEditor_STATIC_QuickCombineMesh(MeshFilter target)
        {
            target.transform.parent = null;

            Vector3 Last_POS = target.transform.position;
            target.transform.position = Vector3.zero;

            MeshFilter[] meshes_ = target.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combiners_ = new CombineInstance[meshes_.Length];

            long counter_ = 0;
            while (counter_ < meshes_.Length)
            {
                combiners_[counter_].mesh = meshes_[counter_].sharedMesh;
                combiners_[counter_].transform = meshes_[counter_].transform.localToWorldMatrix;
                if (meshes_[counter_].gameObject != target.gameObject)
                    DestroyImmediate(meshes_[counter_].gameObject);

                counter_++;
            }

            target.transform.localScale = Vector3.one;

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combiners_);

            target.sharedMesh = newMesh;
            target.sharedMesh.name = target.name;
            target.sharedMesh.RecalculateBounds();
            target.sharedMesh.RecalculateNormals();
            target.transform.position = Last_POS;
        }

        #endregion
    }
}