using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Interactive Landscape")]
    public class MDM_InteractiveLandscape : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Interactive Landscape = Component for objects with Mesh Renderer
        //---Interactive mesh surface with physically based logic
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool ppDynamicMesh = true;

        public bool ppAllowRigidbodies = true;

        public bool ppMultithreadingSupported = false;
        private Thread Multithread;

        public bool ppEnableCustomInteractionSpeed = false;
        public bool ppCustomInteraction_Continuous = false;
        public float ppInteractionSpeed = 1.5f;

        public Vector3 ppVerticeDirection = new Vector3(0, -1, 0);
        public float ppRadius = 0.8f;
        public bool ppFitToObjectSize = false;
        public float ppForceDetection = 0;

        public bool ppCreateNewReference = true;

        private List<Vector3> originalVertices = new List<Vector3>();
        private List<Vector3> storedVertices = new List<Vector3>();

        private List<Vector3> startingVertices = new List<Vector3>();

        private MeshFilter meshF;

        public bool ppRepair;
        public float ppRepairSpeed = 0.5f;

        public bool ppCollisionWithSpecificTag = false;
        public string ppCollisionTag = "";

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
                if(!Application.isPlaying)
                UnityEditor.EditorUtility.DisplayDialog("Error", "The object doesn't contain Mesh Filter which is very required component...", "OK");
                DestroyImmediate(this);
#endif
                return;
            }
            if (!Application.isPlaying)
                return;
            meshF = GetComponent<MeshFilter>();
            meshF.mesh.MarkDynamic();
            originalVertices.AddRange(meshF.mesh.vertices);
            startingVertices.AddRange(meshF.mesh.vertices);
            storedVertices.AddRange(meshF.mesh.vertices);
        }

        private void Start()
        {
            if (ppMultithreadingSupported && Application.isPlaying)
            {
                Thrd_RealRot = transform.rotation;
                Thrd_RealPos = transform.position;
                Thrd_RealSca = transform.localScale;

                Multithread = new Thread(MD_FUNCT_ModifyMesh_Thread);
                Multithread.Start();
            }
        }

        bool checkForUpdate_InterSpeed, checkForUpdate_Repair = false;
        private void LateUpdate()
        {
            if (!Application.isPlaying)
                return;

            if (ppEnableCustomInteractionSpeed && storedVertices.Count > 0)
            {
                if (checkForUpdate_InterSpeed)
                {
                    int doneAll = 0;
                    if (ppCustomInteraction_Continuous)
                    {
                        for (int i = 0; i < originalVertices.Count; i++)
                        {
                            if (originalVertices[i] == storedVertices[i])
                                doneAll++;
                            originalVertices[i] = Vector3.Lerp(originalVertices[i], storedVertices[i], ppInteractionSpeed * Time.deltaTime);
                        }
                        if (doneAll == originalVertices.Count)
                            checkForUpdate_InterSpeed = false;
                        meshF.mesh.SetVertices(originalVertices);
                    }
                    else
                    {
                        List<Vector3> Verts = new List<Vector3>();
                        Verts.AddRange(meshF.mesh.vertices);
                        for (int i = 0; i < Verts.Count; i++)
                        {
                            if (Verts[i] == storedVertices[i])
                                doneAll++;
                            Verts[i] = Vector3.Lerp(Verts[i], storedVertices[i], ppInteractionSpeed * Time.deltaTime);
                        }
                        if (doneAll == Verts.Count)
                            checkForUpdate_InterSpeed = false;
                        meshF.mesh.SetVertices(Verts);
                    }
                   
                    meshF.mesh.RecalculateNormals();
                }
            }
            if (ppRepair && storedVertices.Count>0)
            {
                if (checkForUpdate_Repair)
                {
                    int doneAll = 0;
                    for (int i = 0; i < storedVertices.Count; i++)
                    {
                        if (originalVertices[i] == storedVertices[i])
                            doneAll++;
                        storedVertices[i] = Vector3.Lerp(storedVertices[i], startingVertices[i], ppRepairSpeed * Time.deltaTime);
                    }
                    if (doneAll == storedVertices.Count)
                        checkForUpdate_Repair = false;
                    meshF.mesh.SetVertices(storedVertices);
                    meshF.mesh.RecalculateNormals();
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!Application.isPlaying)
                return;
            if (!ppAllowRigidbodies)
                return;
            if (!ppDynamicMesh)
                return;
            if (collision.contacts.Length == 0)
                return;
            if (ppForceDetection != 0 && collision.gameObject.GetComponent<Rigidbody>() && collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude < ppForceDetection)
                return;
            if (ppFitToObjectSize)
                ppRadius = collision.transform.localScale.magnitude / 4;
            foreach (ContactPoint cp in collision.contacts)
                InteractiveLandscape_ModifyMesh(cp.point, ppRadius, ppVerticeDirection);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!Application.isPlaying)
                return;
            if (!ppAllowRigidbodies)
                return;
            if (!ppDynamicMesh)
                return;
            if (collision.contacts.Length == 0)
                return;
            if (ppForceDetection != 0 && collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude < ppForceDetection)
                return;

            if (ppFitToObjectSize)
                ppRadius = collision.transform.localScale.magnitude / 4;
            foreach (ContactPoint cp in collision.contacts)
                InteractiveLandscape_ModifyMesh(cp.point, ppRadius, ppVerticeDirection);
        }

        /// <summary>
        /// Modify current mesh by the point, radius, force and vertex direction
        /// </summary>
        /// <param name="AtPoint">Point of modification</param>
        /// <param name="Radius">Density & Radius</param>
        /// <param name="VerticeDirection">Direction of the selected vertices</param>
        public void InteractiveLandscape_ModifyMesh(Vector3 AtPoint, float Radius, Vector3 VerticeDirection)
        {
            if (ppMultithreadingSupported)
            {
                Thrd_AtPoint = AtPoint;
                Thrd_Radius = Radius;
                Thrd_Dir = VerticeDirection;
                Thrd_RealPos = transform.position;
                Thrd_RealRot = transform.rotation;
                Thrd_RealSca = transform.localScale;
            }
            else
            {
                for (int i = 0; i < storedVertices.Count; i++)
                {
                    Vector3 TransformedPoint = transform.TransformPoint(storedVertices[i]);
                    float distance = Vector3.Distance(AtPoint, TransformedPoint);
                    if (distance < Radius)
                    {
                        Vector3 modifVertex = originalVertices[i] + VerticeDirection;
                        storedVertices[i] = modifVertex;
                    }
                }

            }

            if (!ppEnableCustomInteractionSpeed)
                meshF.mesh.SetVertices(storedVertices);
            meshF.mesh.RecalculateNormals();
            checkForUpdate_Repair = true;
            checkForUpdate_InterSpeed = true;
        }

        Vector3 Thrd_AtPoint;
        float Thrd_Radius;
        Vector3 Thrd_Dir;
        Vector3 Thrd_RealPos;
        Vector3 Thrd_RealSca;
        Quaternion Thrd_RealRot;

        private void MD_FUNCT_ModifyMesh_Thread()
        {
            while (true)
            {
                for (int i = 0; i < storedVertices.Count; i++)
                {
                    Vector3 TransformedPoint = TransformPoint(Thrd_RealPos, Thrd_RealRot, Thrd_RealSca, storedVertices[i]);
                    float distance = Vector3.Distance(new Vector3(Thrd_AtPoint.x,0,Thrd_AtPoint.z), new Vector3(TransformedPoint.x,0, TransformedPoint.z));
                    if (distance < Thrd_Radius)
                    {
                        Vector3 modifVertex = originalVertices[i] + Thrd_Dir;
                        storedVertices[i] = modifVertex;
                    }
                }

                Thread.Sleep(1);
            }
        }

        private Vector3 TransformPoint(Vector3 WorldPos, Quaternion WorldRot, Vector3 WorldScale, Vector3 Point)
        {
            var localToWorldMatrix = Matrix4x4.TRS(WorldPos, Thrd_RealRot, Thrd_RealSca);
            return localToWorldMatrix.MultiplyPoint3x4(Point);
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

        /// <summary>
        /// Modify current mesh by custom RaycastEvent
        /// </summary>
        public void InteractiveLandscape_ModifyMesh(MDM_RaycastEvent RayEvent)
        {
            if (!Application.isPlaying)
                return;
            if (!ppDynamicMesh)
                return;
            if (RayEvent==null)
                return;
            if (RayEvent.hits.Length>0 && RayEvent.hits[0].collider.gameObject != this.gameObject)
                return;
            if (ppFitToObjectSize)
            {
                if (!RayEvent.ppPointRay)
                    ppRadius = RayEvent.ppSphericalRadius;
                else
                    ppRadius = 0.1f;
            }
            
            foreach(RaycastHit hit in RayEvent.hits)
                InteractiveLandscape_ModifyMesh(hit.point, ppRadius, ppVerticeDirection);
        }
    }
}