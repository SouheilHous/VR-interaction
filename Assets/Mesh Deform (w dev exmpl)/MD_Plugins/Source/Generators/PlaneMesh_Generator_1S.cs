using UnityEngine;
using System.Collections;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Generators/Plane Generator 1 Side")]
    public class PlaneMesh_Generator_1S : MonoBehaviour {


        //---PLANE GENERATOR - One Sided---

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/3D Object/MD_Plugin/Plane_One Sided")]
        #endif
        /// <summary>
        /// Call 'Generate' to generate target mesh.
        /// </summary>
        public static GameObject Generate() 
        {
            Transform transform = new GameObject("Plane").transform;

            //---------------------------------------------------------
            //Classic mesh generator (Static Plane)
            //---------------------------------------------------------

            //---------------------------------------------------------
            //-----VERTICES/ VERTEXES or POINTS----------------------
            //---------------------------------------------------------
            //---------------------------------------------------------

            Vector3[] Vertices = new Vector3[]
                {
                    //Our mesh has 1 side:

                    //back side
                    new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0),
                    //----First vector is a first point or vertice of our mesh (from null position)
                    //----Second vector is a second point or vertice of our mesh (to right side of null position)
                    //----Third vector is a third point or vertice of our mesh (to up side of null position)
                    //----Fourth vector is a fourth point or vertice of our mesh (to right-up side of null position)

                    //---Every point must be assigned to each speciffic position. For now, we have whole numbers to set vertice position.
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
                    //Our triangle has 2 combinatons:

                    //Triangle is created by logical thinking...
                    //There is used combinatorics
                    //---You can check information image in the assets under Script to better understanding...-----
                    0, 1, 2,
                    //---Every number is assigned to the each vertice (like: first number 0 is assigned to the first vertice 0 (new Vectory3(0,0,0))---
                    2, 1, 3,

                };
            //---------------------------------------------------------
            //---------------------------------------------------------



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
                    new Vector2(1, 1),
                };
            //---------------------------------------------------------
            //---------------------------------------------------------


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












