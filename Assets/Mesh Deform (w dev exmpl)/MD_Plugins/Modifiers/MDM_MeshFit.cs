using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Mesh Fit")]
    public class MDM_MeshFit : MonoBehaviour
    {

        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Mesh Fit = Component for objects with Mesh Renderer
        //---Modify mesh vertices by surface height
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool ppCreateNewReference = true;
        public bool UpdateManually = true;

        private MeshFilter meshF;

        //---Vertices & Points
        public List<Vector3> verts = new List<Vector3>();
        public List<Transform> points = new List<Transform>();

        public Vector3[] originalVerts;
        public Vector3[] startingVerts;

        public float ppMODIF_MeshFitterOffset = 0.03f;
        public float ppMODIF_MeshFitterSurfaceDetection = 3;
        public enum MeshFitterMode { FitWholeMesh, FitSpecificVertices };
        public MeshFitterMode ppMODIF_MeshFitterType = MeshFitterMode.FitSpecificVertices;
        public bool ppMODIF_MeshFitterContinuousEffect = false;
        public GameObject[] ppMODIF_MeshFitter_SelectedVertexes;

        public bool GotOriginal = false;

        void Awake()
        {
            if (ppCreateNewReference)
                MD_MeshProEditor.MeshEditor_STATIC_CreateNewReference(this.gameObject);

            if (!MD_MeshProEditor.MD_INTERNAL_TECH_CheckModifiers(this.gameObject, this.GetType().Name))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
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

            meshF = GetComponent<MeshFilter>();
            meshF.sharedMesh.MarkDynamic();

            if (!GotOriginal)
            {
                originalVerts = GetComponent<MeshFilter>().sharedMesh.vertices;
                startingVerts = GetComponent<MeshFilter>().sharedMesh.vertices;
                GotOriginal = true;
#if UNITY_EDITOR
                if (!Application.isPlaying && GetComponent<MeshFilter>().sharedMesh.vertices.Length > MD_MeshProEditor.VerticesLimit)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Mesh has more than " + MD_MeshProEditor.VerticesLimit.ToString() + " vertices", "Your selected mesh has more than " + MD_MeshProEditor.VerticesLimit.ToString() + " vertices [" + GetComponent<MeshFilter>().sharedMesh.vertices.Length.ToString() + "]. This could slow the performance.", "OK");
                }
#endif
            }

        }
       
        private void Update()
        {
            if(!UpdateManually)
                MeshFit_UpdateMeshState();
        }

        /// <summary>
        /// Refresh vertices Activation [only for internal use!]
        /// </summary>
        public void MD_INTERNAL_RefreshVerticesActivation()
        {
            if (points.Count == 0 || ppMODIF_MeshFitter_SelectedVertexes.Length == 0)
                return;

            foreach (Transform gm in points)
                gm.gameObject.SetActive(false);
            foreach (GameObject gm in ppMODIF_MeshFitter_SelectedVertexes)
                gm.SetActive(true);
        }

        /// <summary>
        /// Show/ Hide points
        /// </summary>
        public void MeshFit_ShowHidePoints(bool activation)
        {
            if (points.Count == 0)
                return;

            if(ppMODIF_MeshFitter_SelectedVertexes != null && ppMODIF_MeshFitter_SelectedVertexes.Length > 0)
            {
                foreach (GameObject p in ppMODIF_MeshFitter_SelectedVertexes)
                {
                    if (p && p.GetComponent<Renderer>())
                        p.GetComponent<Renderer>().enabled = activation;
                }
                return;
            }
            foreach (Transform p in points)
            {
                if(p.transform.parent.name.Contains(this.name) && p.GetComponent<Renderer>())
                    p.GetComponent<Renderer>().enabled = activation;
            }
        }
        /// <summary>
        /// Generate points on the mesh
        /// </summary>
        public void MeshFit_GeneratePoints()
        {
            if (meshF == null || (meshF && meshF.sharedMesh == null))
            {
                if (!GetComponent<MeshFilter>())
                {
                    MD_Debug.Debug(this, "The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                    return;
                }
                else
                    meshF = GetComponent<MeshFilter>();
            }

            MeshFit_ClearPoints();

            transform.parent = null;

            Vector3 lastPos = transform.position;
            Quaternion lastRot = transform.rotation;

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            GameObject vertRoot = new GameObject("VertRoot_" + this.name);
            vertRoot.transform.position = Vector3.zero;
            vertRoot.transform.rotation = Quaternion.identity;

            verts.Clear();
            points.Clear();

            //---Generating Points & Vertices
            var vertices = meshF.sharedMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                GameObject gm = null;

                Material new_Mat = new Material(Shader.Find("Unlit/Color"));
                new_Mat.color = Color.red;
                gm = OctahedronMesh_Generator.Generate();
                if (gm.GetComponent<Collider>())
                    DestroyImmediate(gm.GetComponent<Collider>());
                gm.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                gm.GetComponent<Renderer>().material = new_Mat;

                gm.transform.parent = vertRoot.transform;

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
                        vertice.transform.parent = vertice2.transform;
                        vertice.gameObject.SetActive(false);
                    }
                }
            }

            //---Renaming Points
            int counter = 1;
            foreach (Transform vertice in vertRoot.transform)
            {
                vertice.gameObject.SetActive(true);
                vertice.name = "P" + counter.ToString();
                counter++;
            }

            vertRoot.transform.parent = this.transform;
            transform.position = lastPos;
            transform.rotation = lastRot;
        }
        /// <summary>
        /// Restore original mesh
        /// </summary>
        public void MeshFit_RestoreOriginal()
        {
            MeshFit_ClearPoints();

            meshF.sharedMesh.vertices = startingVerts;
            originalVerts = startingVerts;

            meshF.sharedMesh.RecalculateNormals();
            meshF.sharedMesh.RecalculateBounds();
        }
        /// <summary>
        /// Clear points on the mesh [if possible]
        /// </summary>
        public void MeshFit_ClearPoints()
        {
            verts.Clear();
            points.Clear();

            if (transform.Find("VertRoot_" + this.name) != null)
                DestroyImmediate(transform.Find("VertRoot_" + this.name).gameObject);
        }
        /// <summary>
        /// Reset mesh matrix transform [Set scale to 1 and keep the shape]
        /// </summary>
        public void MeshFit_BakeMesh()
        {
            MeshFit_ClearPoints();

            Vector3[] vertsNew = meshF.sharedMesh.vertices;
            Vector3 lastPos = transform.position;
            Quaternion lastRot = transform.rotation;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            for (int i = 0; i < vertsNew.Length; i++)
                vertsNew[i] = transform.TransformPoint(meshF.sharedMesh.vertices[i]);
            transform.localScale = Vector3.one;
            meshF.sharedMesh.vertices = vertsNew;

            originalVerts = meshF.sharedMesh.vertices;

            transform.position = lastPos;
            transform.rotation = lastRot;
        }

        /// <summary>
        /// Update mesh state
        /// </summary>
        public void MeshFit_UpdateMeshState()
        {
            if (verts.Count == 0 || points.Count == 0)
                return;
           
            if (ppMODIF_MeshFitterType == MeshFitterMode.FitWholeMesh)
            {
                for (int i = 0; i < verts.Count; i++)
                {
                    points[i].gameObject.layer = 2;
                    gameObject.layer = 2;

                    Ray ray = new Ray(points[i].transform.position + Vector3.up * ppMODIF_MeshFitterSurfaceDetection, Vector3.down);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.collider)
                            points[i].transform.position = new Vector3(hit.point.x, hit.point.y + ppMODIF_MeshFitterOffset, hit.point.z);
                        else if (!ppMODIF_MeshFitterContinuousEffect) points[i].transform.position = transform.TransformPoint(originalVerts[i]);
                    }
                    else if (!ppMODIF_MeshFitterContinuousEffect) points[i].transform.position = transform.TransformPoint(originalVerts[i]);

                    verts[i] = new Vector3(points[i].position.x - (meshF.transform.position.x - Vector3.zero.x), points[i].position.y - (meshF.transform.position.y - Vector3.zero.y), points[i].position.z - (meshF.transform.position.z - Vector3.zero.z));

                    points[i].gameObject.layer = 0;
                    gameObject.layer = 0;
                }
            }
            else if (ppMODIF_MeshFitterType == MeshFitterMode.FitSpecificVertices)
            {
                if (ppMODIF_MeshFitter_SelectedVertexes == null || ppMODIF_MeshFitter_SelectedVertexes.Length == 0)
                    return;

                for (int i = 0; i < verts.Count; i++)
                {
                    points[i].gameObject.layer = 2;
                    gameObject.layer = 2;

                    if (points[i].gameObject.activeInHierarchy)
                    {
                        Ray ray = new Ray(points[i].transform.position + Vector3.up * ppMODIF_MeshFitterSurfaceDetection, Vector3.down);
                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                        {
                            if (hit.collider)
                                points[i].transform.position = new Vector3(hit.point.x, hit.point.y + ppMODIF_MeshFitterOffset, hit.point.z);
                            else if (!ppMODIF_MeshFitterContinuousEffect) points[i].transform.position = transform.TransformPoint(originalVerts[i]);
                        }
                        else if(!ppMODIF_MeshFitterContinuousEffect) points[i].transform.position = transform.TransformPoint(originalVerts[i]);
                    }

                    verts[i] = new Vector3(points[i].position.x - (meshF.transform.position.x - Vector3.zero.x), points[i].position.y - (meshF.transform.position.y - Vector3.zero.y), points[i].position.z - (meshF.transform.position.z - Vector3.zero.z));
                    gameObject.layer = 0;
                }
            }

            meshF.sharedMesh.vertices = verts.ToArray();
            meshF.sharedMesh.RecalculateNormals();
            meshF.sharedMesh.RecalculateBounds();
        }
    }
}