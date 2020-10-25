using UnityEngine;
using System.Collections;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Generators/Cube Generator")]
    public class CubeMesh_Generator : MonoBehaviour {

        //---CUBE GENERATOR ONLY---
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/3D Object/MD_Plugin/Cube")]
        #endif
        /// <summary>
        /// Call 'Generate' to generate target mesh.
        /// </summary>
        public static GameObject Generate() 
        {
            Transform transform = new GameObject("Cube2").transform;
            //---------------------------------------------------------
            //Classic mesh generator (Static Cube)
            //---------------------------------------------------------

            //---------------------------------------------------------
            //-----VERTICES/ VERTEXES or POINTS----------------------
            //---------------------------------------------------------
            //---------------------------------------------------------

            Vector3[] Vertices = new Vector3[]
                {
                    //Our mesh has 6 sides:

                    //back side
                    new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0),
                    //----First vector is a first point or vertice of our mesh (from null position)
                    //----Second vector is a second point or vertice of our mesh (to right side of null position)
                    //----Third vector is a third point or vertice of our mesh (to up side of null position)
                    //----Fourth vector is a fourth point or vertice of our mesh (to right-up side of null position)

                    //-------Other faces or sides are working on the same principles like this principle-----------------

                    //right side
                    new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1),
                    //forward side
                    new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1),
                    //left side
                    new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 1), new Vector3(0, 1, 0),
                    //up side
                    new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 1), new Vector3(1, 1, 1),
                    //down side
                    new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1),

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
                    //Our triangle has 12 combinatons:

                    //Triangle is created by logical thinking...
                    //There is used combinatorics
                    //---You can check information image in the assets under Script to better understanding...-----
                    0, 3, 1,
                    //---Every number is assigned to the each vertice (like: first number 0 is assigned to the first vertice 0 (new Vectory3(0,0,0))---
                    3, 0, 2,

                    4, 7, 5,
                    7, 4, 6,

                    8, 11, 9,
                    11, 8, 10,

                    12, 15, 13,
                    15, 12, 14,

                    16, 19, 17,
                    19, 16, 18,

                    21, 22, 20,
                    22, 21, 23,
                };
            //---------------------------------------------------------
            //---------------------------------------------------------


            //---------------------------------------------------------
            //-----------TRIANGLES (Method 2)-------------------------------------
            //---------------------------------------------------------
            //---------------------------------------------------------

            //------This method is a too same, like previous. But this method is a much longer, it's just like "excerption" of combinations...----
            /*
            int[] Triangles = new int[36] ;

            Triangles[0] = 0;
            Triangles[1] = 3;
            Triangles[2] = 1;

            Triangles[3] = 3;
            Triangles[4] = 0;
            Triangles[5] = 2;

            Triangles[6] = 4;
            Triangles[7] = 7;
            Triangles[8] = 5;

            Triangles[9] = 7;
            Triangles[10] = 4;
            Triangles[11] = 6;

            Triangles[12] = 8;
            Triangles[13] = 11;
            Triangles[14] = 9;

            Triangles[15] = 11;
            Triangles[16] = 8;
            Triangles[17] = 10;

            Triangles[18] = 12;
            Triangles[19] = 15;
            Triangles[20] = 13;

            Triangles[21] = 15;
            Triangles[22] = 12;
            Triangles[23] = 14;

            Triangles[24] = 16;
            Triangles[25] = 19;
            Triangles[26] = 17;

            Triangles[27] = 19;
            Triangles[28] = 16;
            Triangles[29] = 18;

            Triangles[30] = 21;
            Triangles[31] = 22;
            Triangles[32] = 20;

            Triangles[33] = 22;
            Triangles[34] = 21;
            Triangles[35] = 23;
            */
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

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),

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
                    Vector3.back,

                    //Second Normals are directed to the right side, cause our second face is created from right side and is directed to the right side---- etc...
                    Vector3.right,
                    Vector3.right,
                    Vector3.right,
                    Vector3.right,

                    Vector3.forward,
                    Vector3.forward,
                    Vector3.forward,
                    Vector3.forward,

                    Vector3.left,
                    Vector3.left,
                    Vector3.left,
                    Vector3.left,

                    Vector3.up,
                    Vector3.up,
                    Vector3.up,
                    Vector3.up,

                    Vector3.down,
                    Vector3.down,
                    Vector3.down,
                    Vector3.down,
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

            //-----------------Assign mesh requirements to the Vertices, Triagnles, Normals and UV maps---- (optional- optimize) ----
            myMesh.vertices = Vertices;
            myMesh.triangles = Triangles;
            myMesh.normals = Normals;
            myMesh.uv = UV;
            myMesh.RecalculateNormals();
            myMesh.RecalculateBounds();
            myMesh.RecalculateTangents();
            //---------------------------------------------------------

            //---Assign mesh----
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
    











