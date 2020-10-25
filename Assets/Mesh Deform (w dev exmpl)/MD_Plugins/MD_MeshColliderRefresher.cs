using UnityEngine;
using System.Collections;

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Mesh Collider Refresher")]
    public class MD_MeshColliderRefresher : MonoBehaviour
    {

        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Collection): Mesh Collider Refresher = Component for objects with MeshCollider
        //---Refresh mesh collider script
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public enum RefreshType_ { Once, PerFrame, Interval, Never};
        public RefreshType_ RefreshType = RefreshType_.PerFrame;

        public float IntervalSeconds = 1f;

        public bool Convex_MeshCollider = false;

        public bool IgnoreRaycast = false;

        public Vector3 ColliderOffset = Vector3.zero;

        void Start () 
        {
            if (RefreshType == RefreshType_.Never)
                return;

            //----Setting up layer----
            if (IgnoreRaycast)
                gameObject.layer = 2;

            //----Checking Mesh Collider and Renderer component----
            if (transform.GetComponent<Renderer>())
            {
                if (transform.GetComponent<MeshCollider>())
                    transform.GetComponent<MeshCollider>().convex = Convex_MeshCollider;
                else
                    transform.gameObject.AddComponent<MeshCollider>().convex = Convex_MeshCollider;

                if (ColliderOffset!= Vector3.zero)
                {
                    Mesh MeshColliderMesh = new Mesh();
                    MeshColliderMesh.vertices = GetComponent<MeshCollider>().sharedMesh.vertices;
                    MeshColliderMesh.triangles = GetComponent<MeshCollider>().sharedMesh.triangles;
                    MeshColliderMesh.normals = GetComponent<MeshCollider>().sharedMesh.normals;
                    Vector3[] verts = MeshColliderMesh.vertices;
                    for (int i = 0; i < verts.Length; i++)
                        verts[i] += ColliderOffset;
                    MeshColliderMesh.vertices = verts;
                    GetComponent<MeshCollider>().sharedMesh = MeshColliderMesh;
                }
            }
            else
                MD_Debug.Debug(this, "the object " + this.name + " doesn't contain any Mesh Renderer Component.", MD_Debug.DebugType.Error);
        }

        float intervalTimer = 0;
        void LateUpdate () 
        {
            if (RefreshType == RefreshType_.PerFrame)
                MeshCollider_UpdateMeshCollider();
            else if(RefreshType == RefreshType_.Interval)
            {
                intervalTimer += Time.deltaTime;
                if (intervalTimer > IntervalSeconds)
                {
                    MeshCollider_UpdateMeshCollider();
                    intervalTimer = 0;
                }
            }
        }


        /// <summary>
        /// Update Mesh Collider of target Mesh
        /// </summary>
        /// <param name="TargetMesh">Target Mesh</param>
        public static void MeshCollider_UpdateMeshCollider(GameObject Target, bool Convex)
        {
            //----Updating MeshCollider at function call----
            if (Target.GetComponent<MeshCollider>())
                Target.GetComponent<MeshCollider>().convex = Convex;
            else
                Target.gameObject.AddComponent<MeshCollider>().convex = Convex;

            Mesh Baked_Mesh = new Mesh();
            MeshCollider MyNewCollider = Target.GetComponent<MeshCollider>();

            if (Target.GetComponent<SkinnedMeshRenderer>())
            {
                SkinnedMeshRenderer myNewSkin = Target.GetComponent<SkinnedMeshRenderer>();
                myNewSkin.BakeMesh(Baked_Mesh);
            }
            else if (Target.GetComponent<MeshFilter>())
            {
                MeshFilter myNewSkin = Target.GetComponent<MeshFilter>();
                Baked_Mesh = myNewSkin.sharedMesh;
            }

            MyNewCollider.sharedMesh = null;
            MyNewCollider.sharedMesh = Baked_Mesh;
        }
      
        /// <summary>
        /// Update Mesh Collider of itself
        /// </summary>
        public void MeshCollider_UpdateMeshCollider()
        {
            //----Updating MeshCollider at function call----
            if (transform.GetComponent<MeshCollider>())
                transform.GetComponent<MeshCollider>().convex = Convex_MeshCollider;
            else
                transform.gameObject.AddComponent<MeshCollider>().convex = Convex_MeshCollider;

            Mesh Baked_Mesh = new Mesh();
            MeshCollider MyNewCollider = GetComponent<MeshCollider>();

            if (GetComponent<SkinnedMeshRenderer>())
            {
                SkinnedMeshRenderer myNewSkin = GetComponent<SkinnedMeshRenderer>();
                myNewSkin.BakeMesh(Baked_Mesh);
            }
            else if (GetComponent<MeshFilter>())
            {
                MeshFilter myNewSkin = GetComponent<MeshFilter>();
                Baked_Mesh = myNewSkin.sharedMesh;
            }

            MyNewCollider.sharedMesh = null;
            MyNewCollider.sharedMesh = Baked_Mesh;

            if (ColliderOffset != Vector3.zero)
            {
                Mesh MeshColliderMesh = new Mesh();
                MeshColliderMesh.vertices = GetComponent<MeshCollider>().sharedMesh.vertices;
                MeshColliderMesh.triangles = GetComponent<MeshCollider>().sharedMesh.triangles;
                MeshColliderMesh.normals = GetComponent<MeshCollider>().sharedMesh.normals;
                Vector3[] verts = MeshColliderMesh.vertices;
                for (int i = 0; i < verts.Length; i++)
                    verts[i] += ColliderOffset;
                MeshColliderMesh.vertices = verts;
                GetComponent<MeshCollider>().sharedMesh = MeshColliderMesh;
            }
        }
    }
}