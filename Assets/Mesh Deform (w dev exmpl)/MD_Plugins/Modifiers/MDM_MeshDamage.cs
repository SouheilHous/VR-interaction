using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Mesh Damage")]
    public class MDM_MeshDamage : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Mesh Damage = Component for objects with Mesh Renderer
        //---Damage impact logically occured through the script
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool ppDynamicMesh = true;
        public Rigidbody ppRigidbody;

        public bool ppAutoForce = true;
        public float ppForceAmount = 0.15f;
        public bool ppAutoGenerateRadius = false;
        public float ppRadius = 0.5f;
        public float ppForceDetection = 1.5f;

        public bool ppContinousDamage = false;

        public bool ppCollisionWithSpecificTag = false;
        public string ppCollisionTag = "";

        public bool ppEnableEvent;
        public UnityEvent ppEvent;

        public bool ppCreateNewReference = true;

        private List<Vector3> originalVertices = new List<Vector3>();
        private List<Vector3> storedVertices = new List<Vector3>();
        private List<Vector3> startingVertices = new List<Vector3>();

        private MeshFilter meshF;

        private void OnDrawGizmosSelected()
        {
            if (!ppAutoGenerateRadius)
                Gizmos.DrawWireSphere(transform.position, ppRadius);
        }

        void Awake()
        {
            if(!ppRigidbody && ppForceDetection > 0)
            {
                if (GetComponent<Rigidbody>())
                    ppRigidbody = gameObject.GetComponent<Rigidbody>();
                else if (transform.gameObject.GetComponent<Rigidbody>())
                    ppRigidbody = transform.gameObject.GetComponent<Rigidbody>();
            }

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
            storedVertices.AddRange(originalVertices);
            startingVertices.AddRange(storedVertices);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!Application.isPlaying)
                return;
            if (!ppDynamicMesh)
                return;
            if (collision.contacts.Length == 0)
                return;
            if (ppRigidbody && ppForceDetection != 0 && ppRigidbody.velocity.magnitude < ppForceDetection)
                return;
            if (ppCollisionWithSpecificTag && ppCollisionTag != collision.gameObject.tag)
                return;

            if (ppAutoForce && collision.gameObject.GetComponent<Rigidbody>())
                ppForceAmount = collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude / 16;
            if (ppAutoGenerateRadius)
                ppRadius = collision.transform.localScale.magnitude/4;
            foreach(ContactPoint cp in collision.contacts)
                MeshDamage_ModifyMesh(cp.point, ppRadius, ppForceAmount);

            if(ppContinousDamage)
                MeshDamage_RefreshVertices();

            if (ppEnableEvent && ppEvent != null)
                ppEvent.Invoke();
        }

        /// <summary>
        /// Modify current mesh by the point, radius, force and vertex direction
        /// </summary>
        /// <param name="AtPoint">Point of modification</param>
        /// <param name="Radius">Density & Radius</param>
        /// <param name="VerticeDirection">Direction of the selected vertices</param>
        public void MeshDamage_ModifyMesh(Vector3 AtPoint, float Radius, float Force)
        {
            for (int i = 0; i < storedVertices.Count; i++)
            {
                float distance = Vector3.Distance(AtPoint, transform.TransformPoint(storedVertices[i]));
                if (distance < Radius)
                {
                    //Vector3 modifVertex = transform.TransformPoint(originalVertices[i]) - transform.TransformDirection(originalVertices[i]) * Force;
                    Vector3 direction = transform.TransformDirection(-meshF.mesh.vertices[i]);
                    Vector3 origin = transform.TransformPoint(storedVertices[i]);
                    origin += direction * Force;
                    storedVertices[i] = transform.InverseTransformPoint(origin);
                }
            }

            meshF.mesh.SetVertices(storedVertices);
            meshF.mesh.RecalculateNormals();
            meshF.mesh.RecalculateBounds();
        }
        /// <summary>
        /// Refresh vertices that have been already interacted...
        /// </summary>
        public void MeshDamage_RefreshVertices()
        {
            storedVertices.Clear();
            originalVertices.Clear();
            storedVertices.AddRange(meshF.mesh.vertices);
            originalVertices.AddRange(storedVertices);
        }
        /// <summary>
        /// Repair deformed mesh
        /// </summary>
        public void MeshDamage_RepairMesh(float Speed = 0.5f)
        {
            for (int i = 0; i < storedVertices.Count; i++)
                storedVertices[i] = Vector3.Lerp(storedVertices[i], startingVertices[i], Speed * Time.deltaTime);
            meshF.mesh.SetVertices(storedVertices);
            meshF.mesh.RecalculateNormals();
            meshF.mesh.RecalculateBounds();
        }

        /// <summary>
        /// Modify current mesh by custom RaycastEvent
        /// </summary>
        public void MeshDamage_ModifyMesh(MDM_RaycastEvent RayEvent)
        {
            if (!Application.isPlaying)
                return;
            if (!ppDynamicMesh)
                return;
            if (RayEvent == null)
                return;
            if (RayEvent.hits.Length > 0 && RayEvent.hits[0].collider.gameObject != this.gameObject)
                return;
            if (ppAutoGenerateRadius)
            {
                if (!RayEvent.ppPointRay)
                    ppRadius = RayEvent.ppSphericalRadius;
                else
                    ppRadius = 0.1f;
            }

            foreach (RaycastHit hit in RayEvent.hits)
                MeshDamage_ModifyMesh(hit.point, ppRadius, ppForceAmount);
        }
    }
}