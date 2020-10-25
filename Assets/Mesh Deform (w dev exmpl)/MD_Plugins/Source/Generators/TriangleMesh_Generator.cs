using UnityEngine;
using System.Collections;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Generators/Triangle Generator")]
    public class TriangleMesh_Generator : MonoBehaviour 
    {


        //---TRIANGLE GENERATOR ONLY---

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/3D Object/MD_Plugin/Triangle")]
        #endif
        /// <summary>
        /// Call 'Generate' to generate target mesh.
        /// </summary>
        public static GameObject Generate() 
        {
            Transform transform = new GameObject("Triangle").transform;

            //---------------------------------------------------------
            //Classic mesh generator (Static Triangle)
            //---------------------------------------------------------
            //---------------------------------------------------------
            //-----VERTICES/ VERTEXES or POINTS----------------------
            //---------------------------------------------------------
            //---------------------------------------------------------

            Vector3[] Vertices = new Vector3[]
                {
                    //Our mesh has 5 sides:

                    //back side
                    new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0.5f, 1, 0.5f),
                    //----First vector is a first point or vertice of our mesh (from null position)
                    //----Second vector is a second point or vertice of our mesh (to right side of null position)
                    //----Third vector is a third point or vertice of our mesh (to up side of null position)
                    //----Fourth vector is a fourth point or vertice of our mesh (to right-up side of null position)

                    //-------Other faces or sides are working on the same principles like this principle-----------------

                    //right side
                    new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0.5f, 1, 0.5f),
                    //forward side
                    new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0.5f, 1, 0.5f),
                    //left side
                    new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0.5f, 1, 0.5f),
                    //down side
                    new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1)

                    //---Every point must be assigned to each specific position. For now, we have whole numbers to set vertice position.
                    //---------------------------------------------------------
                    //---------------------------------------------------------

                };
            //---------------------------------------------------------
            //---------------------------------------------------------






            //---------------------------------------------------------
            //-----------TRIANGLES (Method 1)-------------------------------------
            //---------------------------------------------------------
            //---------------------------------------------------------
            int[] Triangles = new int[]
                {
                    //Our triangle has 6 combinatons:

                    //Triangle is created by logical thinking...
                    //There is used combinatorics
                    //---You can check information image in the assets under Script to better understanding...-----
                    0, 2, 1,
                    //---Every number is assigned to the each vertice (like: first number 0 is assigned to the first vertice 0 (new Vectory3(0,0,0))---
                    3, 5, 4,

                    6, 8, 7,
                    9, 11, 10,

                    12, 13, 14,
                    14, 15, 12,
                };
            //---------------------------------------------------------
            //---------------------------------------------------------


            //---------------------------------------------------------
            //-----------TRIANGLES (Method 2)-------------------------------------
            //---------------------------------------------------------
            //---------------------------------------------------------
            //------This method is a too same, like previous. But this method is more like "excerption"...----


            //---------------------------------------------------------
            //-----UV MAPS----------------------------------------
            //---------------------------------------------------------
            //---------------------------------------------------------
            Vector2[] UV = new Vector2[]
                {
                    //-----UV maps are assigned to the each point on each face of the mesh---
                    //----It means, that every ,,new Vector2,, is something like every vertice on each face-----
                    //---------------------------------------------------------
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),

                };
            //---------------------------------------------------------
            //---------------------------------------------------------



            //---------------------------------------------------------
            //---------------NORMALS-------------------------
            //---------------------------------------------------------
            //---------------------------------------------------------
            Vector3[] Normals = new Vector3[]
                {
                    //--Normals are displaying shadows of our generated mesh
                    //--So every normal should have direction from specific vertice to specific direction

                    //First Normals are directed to the backward, cause our first face is created from back and is directed to the back side----
                    Vector3.back,
                    Vector3.back,
                    Vector3.back,

                    //Second Normals are directed to the right side, cause our second face is created from right side and is directed to the right side---- etc...
                    Vector3.right,
                    Vector3.right,
                    Vector3.right,

                    Vector3.forward,
                    Vector3.forward,
                    Vector3.forward,

                    Vector3.left,
                    Vector3.left,
                    Vector3.left,

                    Vector3.down,
                    Vector3.down,
                    Vector3.down,
                    Vector3.down
                };



            //---------------------------------------------------------
            //-----------------SETTING UP MESH FILDER AND MESH RENDERER--
            //---------------------------------------------------------
            //---------------------------------------------------------
            if (!transform.GetComponent<MeshFilter>())
            {
                transform.gameObject.AddComponent<MeshFilter>();
            }
            if (!transform.GetComponent<MeshRenderer>())
            {
                transform.gameObject.AddComponent<MeshRenderer>();
            }
            //---------------------------------------------------------
            //---------------------------------------------------------

            Mesh myMesh = new Mesh();

            //-----------------Assign mesh needs to Vertices, Triagnles, Normals and UV maps---- (optional- optimize) ----
            myMesh.vertices = Vertices;
            myMesh.triangles = Triangles;
            myMesh.normals = Normals;
            myMesh.uv = UV;
            myMesh.RecalculateNormals();
            myMesh.RecalculateBounds();
            myMesh.RecalculateTangents();
            //---------------------------------------------------------

            //---Assign mesh to each needs----

            transform.GetComponent<MeshFilter>().mesh = myMesh;

            myMesh.name = "NewMesh" + Random.Range(1, 999).ToString();
            transform.GetComponent<MeshFilter>().mesh = myMesh;

            if (!transform.GetComponent<MeshCollider>())
                transform.gameObject.AddComponent<MeshCollider>();

         
            if (transform.GetComponent<MeshCollider>())
                try
            {
                transform.GetComponent<MeshCollider>().sharedMesh = myMesh;
                transform.GetComponent<MeshCollider>().convex = true;
            }
            catch (UnityException e)
            {
                Debug.LogError(e);
            }



            Shader shad = null;
            shad = Shader.Find("Standard");

            Material mat = new Material(shad);

            transform.GetComponent<Renderer>().material = mat;


#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Selection.activeGameObject = transform.gameObject;
                transform.position = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position + UnityEditor.SceneView.lastActiveSceneView.camera.transform.forward * 3f;
            }
#endif

            return transform.gameObject;
        }
    }
}