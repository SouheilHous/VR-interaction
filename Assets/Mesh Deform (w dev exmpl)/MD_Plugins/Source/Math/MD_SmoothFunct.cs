using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MD_Plugin
{
    public class MD_SmoothFunct : MonoBehaviour
    {

        //---Mesh Smooth
        //---No explanation - advanced math operations

        public static Vector3[] Filter_SmoothFunct(Vector3[] sv, int[] t)
        {
            Vector3[] wv = new Vector3[sv.Length];
            List<Vector3> AdjVertices = new List<Vector3>();

            float dx = 0.0f;
            float dy = 0.0f;
            float dz = 0.0f;

            for (int vi = 0; vi < sv.Length; vi++)
            {
                AdjVertices = MD_Smooth_MeshHelpers.findAdjacentNeighbors(sv, t, sv[vi]);

                if (AdjVertices.Count != 0)
                {
                    dx = 0.0f;
                    dy = 0.0f;
                    dz = 0.0f;

                    for (int j = 0; j < AdjVertices.Count; j++)
                    {
                        dx += AdjVertices[j].x;
                        dy += AdjVertices[j].y;
                        dz += AdjVertices[j].z;
                    }

                    wv[vi].x = dx / AdjVertices.Count;
                    wv[vi].y = dy / AdjVertices.Count;
                    wv[vi].z = dz / AdjVertices.Count;
                }
            }

            return wv;
        }

        public static Vector3[] HC_Filterer(Vector3[] sv, Vector3[] pv, int[] t, float alpha, float beta)
        {
            Vector3[] wv = new Vector3[sv.Length];
            Vector3[] bv = new Vector3[sv.Length];



            wv = Filter_SmoothFunct(sv, t);

            for (int i = 0; i < wv.Length; i++)
            {
                bv[i].x = wv[i].x - (alpha * sv[i].x + (1 - alpha) * sv[i].x);
                bv[i].y = wv[i].y - (alpha * sv[i].y + (1 - alpha) * sv[i].y);
                bv[i].z = wv[i].z - (alpha * sv[i].z + (1 - alpha) * sv[i].z);
            }

            List<int> AdjIndex = new List<int>();

            float dx = 0.0f;
            float dy = 0.0f;
            float dz = 0.0f;

            for (int j = 0; j < bv.Length; j++)
            {
                AdjIndex.Clear();

                AdjIndex = MD_Smooth_MeshHelpers.AdjIndexes_Near(sv, t, sv[j]);

                dx = 0.0f;
                dy = 0.0f;
                dz = 0.0f;

                for (int k = 0; k < AdjIndex.Count; k++)
                {
                    dx += bv[AdjIndex[k]].x;
                    dy += bv[AdjIndex[k]].y;
                    dz += bv[AdjIndex[k]].z;

                }

                wv[j].x -= beta * bv[j].x + ((1 - beta) / AdjIndex.Count) * dx;
                wv[j].y -= beta * bv[j].y + ((1 - beta) / AdjIndex.Count) * dy;
                wv[j].z -= beta * bv[j].z + ((1 - beta) / AdjIndex.Count) * dz;
            }

            return wv;
        }
    }
}