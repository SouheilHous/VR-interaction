using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MD_Plugin
{
    public class MD_Smooth_MeshHelpers : MonoBehaviour
    {
        //---MeshHelpers - mesh smooth
        //---No explanation - advanced math operations

        public static List<Vector3> findAdjacentNeighbors(Vector3[] v, int[] t, Vector3 vertex)
        {
            List<Vector3> Vertex = new List<Vector3>();
            List<int> FaceCreator = new List<int>();
            int FaceLength = 0;

            for (int i = 0; i < v.Length; i++)
                if (Mathf.Approximately(vertex.x, v[i].x) &&
                    Mathf.Approximately(vertex.y, v[i].y) &&
                    Mathf.Approximately(vertex.z, v[i].z))
                {
                    int v1 = 0;
                    int v2 = 0;
                    bool marker = false;

                    for (int k = 0; k < t.Length; k = k + 3)
                        if (FaceCreator.Contains(k) == false)
                        {
                            v1 = 0;
                            v2 = 0;
                            marker = false;

                            if (i == t[k])
                            {
                                v1 = t[k + 1];
                                v2 = t[k + 2];
                                marker = true;
                            }

                            if (i == t[k + 1])
                            {
                                v1 = t[k];
                                v2 = t[k + 2];
                                marker = true;
                            }

                            if (i == t[k + 2])
                            {
                                v1 = t[k];
                                v2 = t[k + 1];
                                marker = true;
                            }

                            FaceLength++;
                            if (marker)
                            {
                                FaceCreator.Add(k);

                                if (VertexExist(Vertex, v[v1]) == false)
                                {
                                    Vertex.Add(v[v1]);
                                }
                                if (VertexExist(Vertex, v[v2]) == false)
                                {
                                    Vertex.Add(v[v2]);
                                }
                                marker = false;
                            }
                        }
                }

            return Vertex;
        }

        public static List<int> AdjIndexes_Near(Vector3[] v, int[] t, Vector3 vertex)
        {
            List<int> AdjIndex = new List<int>();
            List<Vector3> AdjVertex = new List<Vector3>();
            List<int> AdjFace = new List<int>();
            int FaceLength = 0;

            for (int i = 0; i < v.Length; i++)
                if (Mathf.Approximately(vertex.x, v[i].x) &&
                    Mathf.Approximately(vertex.y, v[i].y) &&
                    Mathf.Approximately(vertex.z, v[i].z))
                {
                    int v1 = 0;
                    int v2 = 0;
                    bool marker = false;

                    for (int k = 0; k < t.Length; k = k + 3)
                        if (AdjFace.Contains(k) == false)
                        {
                            v1 = 0;
                            v2 = 0;
                            marker = false;

                            if (i == t[k])
                            {
                                v1 = t[k + 1];
                                v2 = t[k + 2];
                                marker = true;
                            }

                            if (i == t[k + 1])
                            {
                                v1 = t[k];
                                v2 = t[k + 2];
                                marker = true;
                            }

                            if (i == t[k + 2])
                            {
                                v1 = t[k];
                                v2 = t[k + 1];
                                marker = true;
                            }

                            FaceLength++;
                            if (marker)
                            {
                                AdjFace.Add(k);

                                if (VertexExist(AdjVertex, v[v1]) == false)
                                {
                                    AdjVertex.Add(v[v1]);
                                    AdjIndex.Add(v1);
                                }

                                if (VertexExist(AdjVertex, v[v2]) == false)
                                {
                                    AdjVertex.Add(v[v2]);
                                    AdjIndex.Add(v2);
                                }
                                marker = false;
                            }
                        }
                }

            return AdjIndex;
        }
        static bool VertexExist(List<Vector3> AdjVertex, Vector3 v)
        {
            bool marker = false;
            foreach (Vector3 vec in AdjVertex)
                if (Mathf.Approximately(vec.x, v.x) && Mathf.Approximately(vec.y, v.y) && Mathf.Approximately(vec.z, v.z))
                {
                    marker = true;
                    break;
                }


            return marker;
        }
    }
}