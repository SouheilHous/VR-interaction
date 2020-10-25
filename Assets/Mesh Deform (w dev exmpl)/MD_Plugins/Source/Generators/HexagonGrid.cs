using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Generators/Advanced/Hexagon Grid")]
    public class HexagonGrid : MonoBehaviour
    {

        Vector3[] verts;
        int[] tris;
        Vector2[] uvs;

        [Range(1, 90)]
        public int Count = 1;
        public Vector3 Offset;
        public float CellSize = 1;

        [Range(0.1f, 5)]
        public float OffsetX = 1f;
        [Range(0.1f, 5)]
        public float OffsetZ = 1.35f;

        public float RandomHeightRange = 1;

        public bool PlanarHexagon = true;
        public bool Invert = false;

        public bool ppDynamicMesh = true;
        MeshFilter meshF;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/3D Object/MD_Plugin/Advanced/Hexagon Grid")]
#endif
        public static GameObject InitEditor()
        {
            Transform transform = new GameObject("HexagonGrid").transform;

            transform.gameObject.AddComponent<MeshFilter>();
            transform.gameObject.AddComponent<MeshRenderer>();

            transform.gameObject.AddComponent<HexagonGrid>();

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

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/3D Object/MD_Plugin/Planar Hexagon")]
#endif
        public static GameObject InitEditor_SingleHexa()
        {
            Transform transform = new GameObject("Hexagon").transform;

            transform.gameObject.AddComponent<MeshFilter>();
            transform.gameObject.AddComponent<MeshRenderer>();

            transform.gameObject.AddComponent<HexagonGrid>();
            transform.gameObject.GetComponent<HexagonGrid>().ModifyMesh_Planar();
            transform.gameObject.GetComponent<MeshFilter>().sharedMesh.name = "Hexagon";

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
            DestroyImmediate(transform.gameObject.GetComponent<HexagonGrid>());
            return transform.gameObject;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/3D Object/MD_Plugin/Spatial Hexagon")]
#endif
        public static GameObject InitEditor_SingleHexaSpatial()
        {
            Transform transform = new GameObject("Hexagon").transform;

            transform.gameObject.AddComponent<MeshFilter>();
            transform.gameObject.AddComponent<MeshRenderer>();

            transform.gameObject.AddComponent<HexagonGrid>();
            transform.gameObject.GetComponent<HexagonGrid>().ModifyMesh_Spatial();
            transform.gameObject.GetComponent<MeshFilter>().sharedMesh.name = "Hexagon";
             
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
            DestroyImmediate(transform.gameObject.GetComponent<HexagonGrid>());
            return transform.gameObject;
        }

        private void Awake()
        {
            if (!MD_MeshProEditor.MD_INTERNAL_TECH_CheckModifiers(this.gameObject, this.GetType().Name))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.DisplayDialog("Error", "The modifier cannot be applied to this object, because the object already contains modification. Please, remove modification to access to the selected modification...", "OK");
#endif
                DestroyImmediate(this);
                return;
            }

            if (GetComponent<MeshFilter>())
            {
                meshF = GetComponent<MeshFilter>();
                return;
            }
            else
            {
                gameObject.AddComponent<MeshFilter>();
                gameObject.AddComponent<MeshRenderer>();
                meshF = GetComponent<MeshFilter>();
            }
        }

        private void Update()
        {
            if (!ppDynamicMesh)
                return;

            if (meshF == null)
            {
                if (!GetComponent<MeshFilter>())
                    gameObject.AddComponent<MeshFilter>();
                meshF = GetComponent<MeshFilter>();
                return;
            }

            if (PlanarHexagon)
            {
                if (Application.isPlaying && ppDynamicMesh)
                    ModifyMesh_Planar();
                else if (!Application.isPlaying)
                    ModifyMesh_Planar();
            }
            else
            {
                if (Application.isPlaying && ppDynamicMesh)
                    ModifyMesh_Spatial();
                else if (!Application.isPlaying)
                    ModifyMesh_Spatial();
            }
        }

        public void ModifyMesh_Planar()
        {
            Mesh m = new Mesh();
            m.name = "HexaGrid";
            verts = new Vector3[Count * Count * 7];
            tris = new int[Count * Count * 18];
            uvs = new Vector2[Count * Count * 7];

            int v = 0;
            int t = 0;

            for (int x = 0; x < Count; x++)
            {
                for (int z = 0; z < Count; z++)
                {
                    Vector3 Center = new Vector3(x, 0, z);
                    Center.x = (x + z * 0.5f - z / 2) * (0.5f * 2);
                    Center.x /= OffsetX;
                    Center.z /= OffsetZ;

                    //----Vertices
                    //Mid
                    verts[v] = Center + Offset * CellSize;
                    //Down
                    verts[v + 1] = new Vector3(0, 0, 0.5f) * CellSize + Center + Offset;
                    //L-Down
                    verts[v + 2] = new Vector3(0.5f, 0, 0.25f) * CellSize + Center + Offset;
                    //L-Up
                    verts[v + 3] = new Vector3(0.5f, 0, -0.25f) * CellSize + Center + Offset;
                    //Up
                    verts[v + 4] = new Vector3(0f, 0, -0.5f) * CellSize + Center + Offset;
                    //R-Up
                    verts[v + 5] = new Vector3(-0.5f,0, -0.25f) * CellSize + Center + Offset;
                    //R-Down
                    verts[v + 6] = new Vector3(-0.5f, 0, 0.25f) * CellSize + Center + Offset;


                    //----Triangles
                    tris[t] = v;
                    tris[t + 1] = v + 1;
                    tris[t + 2] = v + 2;

                    tris[t + 3] = v;
                    tris[t + 4] = v + 2;
                    tris[t + 5] = v + 3;

                    tris[t + 6] = v;
                    tris[t + 7] = v + 3;
                    tris[t + 8] = v + 4;

                    tris[t + 9] = v;
                    tris[t + 10] = v + 4;
                    tris[t + 11] = v + 5;

                    tris[t + 12] = v;
                    tris[t + 13] = v + 5;
                    tris[t + 14] = v + 6;

                    tris[t + 15] = v;
                    tris[t + 16] = v + 6;
                    tris[t + 17] = v + 1;


                    //----Uvs
                    //Mid
                    uvs[v] = new Vector2(verts[v].x, verts[v].y);
                    //Down
                    uvs[v + 1] = new Vector2(verts[v].x - 1, verts[v].y);
                    //L-Down
                    uvs[v + 2] = new Vector2(verts[v].x - 1, verts[v].y - 1);
                    //L-Up
                    uvs[v + 3] = new Vector2(verts[v].x, verts[v].y - 1);
                    //Up
                    uvs[v + 4] = new Vector2(verts[v].x + 1, verts[v].y);
                    //R-Up
                    uvs[v + 5] = new Vector2(verts[v].x + 1, verts[v].y + 1);
                    //R-Down
                    uvs[v + 6] = new Vector2(verts[v].x, verts[v].y + 1);

                    v += 7;
                    t += 18;
                }
            }

            m.vertices = verts;
            m.triangles = tris;
            m.uv = uvs;
            m.RecalculateNormals();
            m.RecalculateBounds();
            m.RecalculateTangents();
            meshF.sharedMesh = m;
        }

        public void ModifyMesh_Spatial(float AddHeightRand = 0)
        {
            Mesh m = new Mesh();
            m.name = "HexaGrid";
            verts = new Vector3[Count * Count * 14];
            tris = new int[Count * Count * 72];
            uvs = new Vector2[Count * Count * 14];

            int v = 0;
            int t = 0;

            for (int x = 0; x < Count; x++)
            {
                for (int z = 0; z < Count; z++)
                {
                    Vector3 Center = new Vector3(x, 0, z);
                    Vector3 AddHeight = new Vector3(0, 1, 0);
                    float RandomHeight = Random.Range(0, AddHeightRand);
                    Center.x = (x + z * 0.5f - z / 2) * (0.5f * 2);
                    Center.x /= OffsetX;
                    Center.z /= OffsetZ;
                    //----Vertices

                    //------Bot Part
                    //Mid
                    verts[v] = Center + Offset;
                    //Down
                    verts[v + 1] = new Vector3(0, 0, 0.5f) * CellSize + Center + Offset;
                    //L-Down
                    verts[v + 2] = new Vector3(0.5f, 0, 0.25f) * CellSize + Center + Offset;
                    //L-Up
                    verts[v + 3] = new Vector3(0.5f, 0, -0.25f) * CellSize + Center + Offset;
                    //Up
                    verts[v + 4] = new Vector3(0f, 0, -0.5f) * CellSize + Center + Offset;
                    //R-Up
                    verts[v + 5] = new Vector3(-0.5f, 0, -0.25f) * CellSize + Center + Offset;
                    //R-Down
                    verts[v + 6] = new Vector3(-0.5f, 0, 0.25f) * CellSize + Center + Offset;

                    if(RandomHeight!=0)
                    Center.y = RandomHeight;
                    //------Top Part
                    //Mid
                    verts[v + 7] = Center + Offset + AddHeight;
                    //Down
                    verts[v + 8] = new Vector3(0, 0, 0.5f) * CellSize + Center + Offset + AddHeight;
                    //L-Down
                    verts[v + 9] = new Vector3(0.5f, 0, 0.25f) * CellSize + Center + Offset + AddHeight;
                    //L-Up
                    verts[v + 10] = new Vector3(0.5f, 0, -0.25f) * CellSize + Center + Offset + AddHeight;
                    //Up
                    verts[v + 11] = new Vector3(0f, 0, -0.5f) * CellSize + Center + Offset + AddHeight;
                    //R-Up
                    verts[v + 12] = new Vector3(-0.5f, 0, -0.25f) * CellSize + Center + Offset + AddHeight;
                    //R-Down
                    verts[v + 13] = new Vector3(-0.5f, 0, 0.25f) * CellSize + Center + Offset + AddHeight;


                    //----Triangles
                    #region Tris
                    if (Invert)
                    {
                        //--TOP
                        tris[t] = v;
                        tris[t + 1] = v + 1;
                        tris[t + 2] = v + 2;

                        tris[t + 3] = v;
                        tris[t + 4] = v + 2;
                        tris[t + 5] = v + 3;

                        tris[t + 6] = v;
                        tris[t + 7] = v + 3;
                        tris[t + 8] = v + 4;

                        tris[t + 9] = v;
                        tris[t + 10] = v + 4;
                        tris[t + 11] = v + 5;

                        tris[t + 12] = v;
                        tris[t + 13] = v + 5;
                        tris[t + 14] = v + 6;

                        tris[t + 15] = v;
                        tris[t + 16] = v + 6;
                        tris[t + 17] = v + 1;


                        //--Sides
                        tris[t + 36] = v + 2;
                        tris[t + 37] = v + 1;
                        tris[t + 38] = v + 8;
                        tris[t + 39] = v + 8;
                        tris[t + 40] = v + 9;
                        tris[t + 41] = v + 2;

                        tris[t + 42] = v + 3;
                        tris[t + 43] = v + 2;
                        tris[t + 44] = v + 9;
                        tris[t + 45] = v + 9;
                        tris[t + 46] = v + 10;
                        tris[t + 47] = v + 3;

                        tris[t + 48] = v + 4;
                        tris[t + 49] = v + 3;
                        tris[t + 50] = v + 10;
                        tris[t + 51] = v + 10;
                        tris[t + 52] = v + 11;
                        tris[t + 53] = v + 4;

                        tris[t + 54] = v + 5;
                        tris[t + 55] = v + 4;
                        tris[t + 56] = v + 11;
                        tris[t + 57] = v + 11;
                        tris[t + 58] = v + 12;
                        tris[t + 59] = v + 5;

                        tris[t + 60] = v + 6;
                        tris[t + 61] = v + 5;
                        tris[t + 62] = v + 12;
                        tris[t + 63] = v + 12;
                        tris[t + 64] = v + 13;
                        tris[t + 65] = v + 6;

                        tris[t + 66] = v + 1;
                        tris[t + 67] = v + 6;
                        tris[t + 68] = v + 13;
                        tris[t + 69] = v + 13;
                        tris[t + 70] = v + 8;
                        tris[t + 71] = v + 1;


                        //----BOTTOM
                        tris[t + 18] = v + 7;
                        tris[t + 19] = v + 9;
                        tris[t + 20] = v + 8;

                        tris[t + 21] = v + 7;
                        tris[t + 22] = v + 10;
                        tris[t + 23] = v + 9;

                        tris[t + 24] = v + 7;
                        tris[t + 25] = v + 11;
                        tris[t + 26] = v + 10;

                        tris[t + 27] = v + 7;
                        tris[t + 28] = v + 12;
                        tris[t + 29] = v + 11;

                        tris[t + 30] = v + 7;
                        tris[t + 31] = v + 13;
                        tris[t + 32] = v + 12;

                        tris[t + 33] = v + 7;
                        tris[t + 34] = v + 8;
                        tris[t + 35] = v + 13;
                    }
                    else
                    {
                        //--TOP
                        tris[t] = v;
                        tris[t + 1] = v + 2;
                        tris[t + 2] = v + 1;

                        tris[t + 3] = v;
                        tris[t + 4] = v + 3;
                        tris[t + 5] = v + 2;

                        tris[t + 6] = v;
                        tris[t + 7] = v + 4;
                        tris[t + 8] = v + 3;

                        tris[t + 9] = v;
                        tris[t + 10] = v + 5;
                        tris[t + 11] = v + 4;

                        tris[t + 12] = v;
                        tris[t + 13] = v + 6;
                        tris[t + 14] = v + 5;

                        tris[t + 15] = v;
                        tris[t + 16] = v + 1;
                        tris[t + 17] = v + 6;



                        tris[t + 36] = v + 2;
                        tris[t + 37] = v + 8;
                        tris[t + 38] = v + 1;
                        tris[t + 39] = v + 8;
                        tris[t + 40] = v + 2;
                        tris[t + 41] = v + 9;

                        tris[t + 42] = v + 3;
                        tris[t + 43] = v + 9;
                        tris[t + 44] = v + 2;
                        tris[t + 45] = v + 9;
                        tris[t + 46] = v + 3;
                        tris[t + 47] = v + 10;

                        tris[t + 48] = v + 4;
                        tris[t + 49] = v + 10;
                        tris[t + 50] = v + 3;
                        tris[t + 51] = v + 10;
                        tris[t + 52] = v + 4;
                        tris[t + 53] = v + 11;

                        tris[t + 54] = v + 5;
                        tris[t + 55] = v + 11;
                        tris[t + 56] = v + 4;
                        tris[t + 57] = v + 11;
                        tris[t + 58] = v + 5;
                        tris[t + 59] = v + 12;

                        tris[t + 60] = v + 6;
                        tris[t + 61] = v + 12;
                        tris[t + 62] = v + 5;
                        tris[t + 63] = v + 12;
                        tris[t + 64] = v + 6;
                        tris[t + 65] = v + 13;

                        tris[t + 66] = v + 1;
                        tris[t + 67] = v + 13;
                        tris[t + 68] = v + 6;
                        tris[t + 69] = v + 13;
                        tris[t + 70] = v + 1;
                        tris[t + 71] = v + 8;


                        //----BOTTOM
                        tris[t + 18] = v + 7;
                        tris[t + 19] = v + 8;
                        tris[t + 20] = v + 9;

                        tris[t + 21] = v + 7;
                        tris[t + 22] = v + 9;
                        tris[t + 23] = v + 10;

                        tris[t + 24] = v + 7;
                        tris[t + 25] = v + 10;
                        tris[t + 26] = v + 11;

                        tris[t + 27] = v + 7;
                        tris[t + 28] = v + 11;
                        tris[t + 29] = v + 12;

                        tris[t + 30] = v + 7;
                        tris[t + 31] = v + 12;
                        tris[t + 32] = v + 13;

                        tris[t + 33] = v + 7;
                        tris[t + 34] = v + 13;
                        tris[t + 35] = v + 8;
                    }
                    #endregion


                    //----Uvs
                    //Mid
                    uvs[v] = new Vector2(0, 0);
                    //Down
                    uvs[v + 1] = new Vector2(0, -1);
                    //L-Down
                    uvs[v + 2] = new Vector2(- 1, -1);
                    //L-Up
                    uvs[v + 3] = new Vector2(- 1, 1);
                    //Up
                    uvs[v + 4] = new Vector2(0, 1);
                    //R-Up
                    uvs[v + 5] = new Vector2(1, 1);
                    //R-Down
                    uvs[v + 6] = new Vector2(1, -1);

                    //---Bottom Part
                    //Mid
                    uvs[v + 7] = new Vector2(0, 0);
                    //Down
                    uvs[v + 8] = new Vector2(0, -1);
                    //L-Down
                    uvs[v + 9] = new Vector2(- 1,  - 1);
                    //L-Up
                    uvs[v + 10] = new Vector2(-1,  1);
                    //Up
                    uvs[v + 11] = new Vector2(0, 1);
                    //R-Up
                    uvs[v + 12] = new Vector2( 1,  1);
                    //R-Down
                    uvs[v + 13] = new Vector2(1, - 1);

                    v += 14;
                    t += 72;
                }
            }

            m.vertices = verts;
            m.triangles = tris;
            m.uv = uvs;
            m.RecalculateNormals();
            m.RecalculateBounds();
            m.RecalculateTangents();
            meshF.sharedMesh = m;
        }

        public void hgRandomizeHeight(float Offset)
        {
            ModifyMesh_Spatial(Offset);
        }
    }
}