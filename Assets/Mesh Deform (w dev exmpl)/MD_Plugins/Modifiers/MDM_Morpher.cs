using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Morpher")]
    [ExecuteInEditMode]
    public class MDM_Morpher : MonoBehaviour
    {

        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Morpher = Component for objects with Mesh Filter
        //---Blend mesh between list of stored & captured vertices
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool ppDynamicMesh = true;

        public bool ppCreateNewReference = true;

        public bool ppMultithreadingSupported = false;
        private Thread Multithread;

        public List<Vector3> originalVertices = new List<Vector3>();
        public Vector3[] threadedVertices;

        public MeshFilter meshF;

        public bool ppInterpolation = false;
        public float ppInterpolationSpeed = 0.5f;

        [Range(0, 1)]
        public float ppBlendValue = 0;

        [SerializeField]
        public Mesh[] ppTargetMorphMeshes = new Mesh[0];
        public int ppIndexOfTargetMesh = 0;

        public bool ppRestartVertState = true;

        [System.Serializable]
        public class registeredMorphs
        {
            public List<Vector3> Vertices = new List<Vector3>();
            public List<int> Indexes = new List<int>();
        }
        public List<registeredMorphs> RegisteredMorphs = new List<registeredMorphs>();

        public bool ModeSwitched = false;

        void Awake()
        {
            if (ppCreateNewReference)
                MD_MeshProEditor.MeshEditor_STATIC_CreateNewReference(this.gameObject);

            if (!MD_MeshProEditor.MD_INTERNAL_TECH_CheckModifiers(this.gameObject, this.GetType().Name))
            {
#if UNITY_EDITOR
                if(!Application.isPlaying)
                UnityEditor.EditorUtility.DisplayDialog("Error", "The modifier cannot be applied to this object, because the object already contains modifiers. Please, remove exists modifier to access to the selected modifier...", "OK");
#endif
                DestroyImmediate(this);
                return;
            }

            if (!GetComponent<MeshFilter>())
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayDialog("Error", "The object doesn't contain Mesh Filter which is very required component...", "OK");
                DestroyImmediate(this);
#endif
                return;
            }
            if (meshF == null)
            {
                meshF = GetComponent<MeshFilter>();
                meshF.sharedMesh.MarkDynamic();
                originalVertices.AddRange(meshF.sharedMesh.vertices);
            }
        }

        private void Start()
        {
            if (ppMultithreadingSupported && Application.isPlaying)
            {
                threadedVertices = meshF.mesh.vertices;
                Multithread = new Thread(UpdateMorphMesh);
                Multithread.Start();
            }
        }


        /// <summary>
        /// Change current target morph mesh index
        /// </summary>
        public void Morpher_ChangeMeshIndex(int entry)
        {
            if (!meshF)
                return;
            if(ppRestartVertState)
                meshF.sharedMesh.vertices = originalVertices.ToArray();
            ppIndexOfTargetMesh = entry;
        }

        /// <summary>
        /// Set current blend value
        /// </summary>
        public void Morpher_SetBlendValue(Slider entry)
        {
            ppBlendValue = entry.value;
        }
        /// <summary>
        /// Set current blend value
        /// </summary>
        public void Morpher_SetBlendValue(float entry)
        {
            ppBlendValue = entry;
        }
        /// <summary>
        /// Set current blend value
        /// </summary>
        public void Morpher_SetBlendValue(int entry)
        {
            ppBlendValue = entry;
        }

        /// <summary>
        /// Refresh target meshes - target meshes must be registered
        /// </summary>
        public void Morpher_RefreshTargetMeshes()
        {
            RegisteredMorphs.Clear();
            if (!meshF)
                return;
            foreach (Mesh m in ppTargetMorphMeshes)
            {
                if (originalVertices.Count != m.vertices.Length)
                {
                    MD_Debug.Debug(this, "target mesh must have the same vertex count & mesh identity as its origin", MD_Debug.DebugType.Error);
                    return;
                }

                registeredMorphs regM = new registeredMorphs();
                for (int i = 0;i<m.vertices.Length;i++)
                {
                    if (ppMultithreadingSupported)
                    {
                        regM.Vertices.Add(m.vertices[i]);
                        regM.Indexes.Add(i);
                    }
                    else if (!Equals(m.vertices[i], originalVertices[i]))
                    {
                        regM.Vertices.Add(m.vertices[i]);
                        regM.Indexes.Add(i);
                    }
                }
                RegisteredMorphs.Add(regM);
            }
        }

        private Vector3 VertFunct(Vector3 A, Vector3 B, float dist)
        {
            Vector3 final = dist * (B - A) + A;
            return final;
        }

        void Update()
        {
            if (!meshF)
                return;
            if (ppMultithreadingSupported)
            {
                if (threadedVertices.Length == 0)
                    return;
                meshF.sharedMesh.vertices = threadedVertices;
                return;
            }

            if (RegisteredMorphs.Count > 0 && (ppIndexOfTargetMesh >= 0 && ppIndexOfTargetMesh < RegisteredMorphs.Count))
            {
                if (RegisteredMorphs[ppIndexOfTargetMesh] == null)
                    return;
                Vector3[] Vertices = meshF.sharedMesh.vertices;
                for (int i = 0; i < RegisteredMorphs[ppIndexOfTargetMesh].Vertices.Count; i++)
                {
                    if (!ppInterpolation)
                        Vertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]] = VertFunct(originalVertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]], RegisteredMorphs[ppIndexOfTargetMesh].Vertices[i], ppBlendValue);
                    else
                        Vertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]] = Vector3.Lerp(Vertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]], VertFunct(originalVertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]], RegisteredMorphs[ppIndexOfTargetMesh].Vertices[i], ppBlendValue), ppInterpolationSpeed * Time.deltaTime);
                }
                meshF.sharedMesh.vertices = Vertices;
            }
        }

        private void UpdateMorphMesh()
        {
            while (true)
            {
                if (RegisteredMorphs.Count > 0 && (ppIndexOfTargetMesh >= 0 && ppIndexOfTargetMesh < RegisteredMorphs.Count))
                {
                    if (RegisteredMorphs[ppIndexOfTargetMesh] == null)
                        return;
                    for (int i = 0; i < RegisteredMorphs[ppIndexOfTargetMesh].Vertices.Count; i++)
                    {
                        if (!ppInterpolation)
                            threadedVertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]] = VertFunct(originalVertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]], RegisteredMorphs[ppIndexOfTargetMesh].Vertices[i], ppBlendValue);
                        else
                            threadedVertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]] = Vector3.Lerp(threadedVertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]], VertFunct(originalVertices[RegisteredMorphs[ppIndexOfTargetMesh].Indexes[i]], RegisteredMorphs[ppIndexOfTargetMesh].Vertices[i], ppBlendValue), ppInterpolationSpeed);
                    }
                }

                Thread.Sleep(1);
            }
        }

        private void OnApplicationQuit()
        {
            if (Multithread != null && Multithread.IsAlive)
                Multithread.Abort();
        }
        private void OnDestroy()
        {
            if (Multithread != null && Multithread.IsAlive)
                Multithread.Abort();
        }
    }
}