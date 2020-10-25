using UnityEngine;
using System.Collections;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Generators/Octahedron Generator")]
    public class OctahedronMesh_Generator : MonoBehaviour 
    {


        //---Octahedron GENERATOR ONLY---
#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/3D Object/MD_Plugin/Octahedron")]
#endif
        /// <summary>
        /// Call 'Generate' to generate target mesh.
        /// </summary>
        public static GameObject Generate() 
        {
            Transform transform = new GameObject("Octahedron").transform;

            //---------------------------------------------------------
            //Classic mesh generator (Static Octahedron)
            //---------------------------------------------------------
            //---------------------------------------------------------
            //-----VERTICES/ VERTEXES or POINTS----------------------
            //---------------------------------------------------------
            //---------------------------------------------------------

            //----Generate vertices to all sides
            Vector3[] Vertices = {
            Vector3.down,
            Vector3.forward,
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.up
        };
            //---------------------------------------------------------
            //---------------------------------------------------------






            //---------------------------------------------------------
            //-----------TRIANGLES (Method 1)-------------------------------------
            //---------------------------------------------------------
            //---------------------------------------------------------
            //---Generate triangles by combinatorics
            int[] Triangles = {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,

            5, 2, 1,
            5, 3, 2,
            5, 4, 3,
            5, 1, 4
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

            Vector2[] UV = new Vector2[Vertices.Length];
            CreateUV(Vertices, UV);

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

            if (!transform.GetComponent<SphereCollider>())
                transform.gameObject.AddComponent<SphereCollider>();

            Shader shad = null;
            shad = Shader.Find("Standard");

            Material mat = new Material(shad);

            transform.GetComponent<Renderer>().material = mat;

            return transform.gameObject;
        }

        public static GameObject Generate(float size)
        {
            Transform transform = new GameObject("Octahedron").transform;

            //---------------------------------------------------------
            //Classic mesh generator (Static Octahedron)
            //---------------------------------------------------------
            //---------------------------------------------------------
            //-----VERTICES/ VERTEXES or POINTS----------------------
            //---------------------------------------------------------
            //---------------------------------------------------------

            //----Generate vertices to all sides
            Vector3[] Vertices = {
            Vector3.down * size,
            Vector3.forward * size,
            Vector3.left * size,
            Vector3.back * size,
            Vector3.right * size,
            Vector3.up * size
        };
            //---------------------------------------------------------
            //---------------------------------------------------------






            //---------------------------------------------------------
            //-----------TRIANGLES (Method 1)-------------------------------------
            //---------------------------------------------------------
            //---------------------------------------------------------
            //---Generate triangles by combinatorics
            int[] Triangles = {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,

            5, 2, 1,
            5, 3, 2,
            5, 4, 3,
            5, 1, 4
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

            Vector2[] UV = new Vector2[Vertices.Length];
            CreateUV(Vertices, UV);

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

            if (!transform.GetComponent<SphereCollider>())
                transform.gameObject.AddComponent<SphereCollider>();

            Shader shad = null;
            shad = Shader.Find("Standard");

            Material mat = new Material(shad);

            transform.GetComponent<Renderer>().material = mat;

            return transform.gameObject;
        }

        private static void CreateUV(Vector3[] vertices, Vector2[] uv)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                Vector2 textureCoordinates;
                textureCoordinates.x = Mathf.Atan2(v.x, v.z) / (-2f * Mathf.PI);
                if (textureCoordinates.x < 0f)
                {
                    textureCoordinates.x += 1f;
                }
                textureCoordinates.y = Mathf.Asin(v.y) / Mathf.PI + 0.5f;
                uv[i] = textureCoordinates;
            }
        }
    }
}