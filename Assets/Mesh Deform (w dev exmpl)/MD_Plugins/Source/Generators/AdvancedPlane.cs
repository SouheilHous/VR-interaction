using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Generators/Advanced/Advanced Plane Generator")]
    public class AdvancedPlane : MonoBehaviour
    {

        //-----Public variables
        [Range(1, 40)]
        public int ppPlaneSizeAngle = 5;
        [Range(1, 125)]
        public int ppPlaneSize = 5;
        public Vector3 ppPlaneOffset;
        public bool ppDynamicMesh = false;

        public bool ppEnableAngle = false;
        [Range(-1f, 1f)]
        public float ppAngle = 0;
        [Range(1, 10f)]
        public float ppAngleDensity = 1;

        public bool ppEnableLandscapeFitter = false;
        [Range(0.1f, 2)]
        public float ppTranslationSpeed = 1;

        //----Internal variables
        Vector3[] vertx;
        int[] trisx;
        Vector2[] uvs;
        MeshFilter meshF;
        Vector3 GizmoPosition1;
        Vector3 GizmoPosition2;

        private void Awake()
        {
            if (!MD_MeshProEditor.MD_INTERNAL_TECH_CheckModifiers(this.gameObject,this.GetType().Name))
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

        private void OnDrawGizmosSelected()
        {
            if (ppEnableAngle)
            {
                Gizmos.DrawWireSphere(GizmoPosition1, this.transform.localScale.y / 2);
                Gizmos.DrawWireSphere(GizmoPosition2, this.transform.localScale.y / 2);
            }
        }

        void Update()
        {
            if (meshF == null)
            {
                if (!GetComponent<MeshFilter>())
                    gameObject.AddComponent<MeshFilter>();
                meshF = GetComponent<MeshFilter>();
                return;
            }

            if(Application.isPlaying && ppDynamicMesh)
                ModifyMesh();
            else if (!Application.isPlaying)
                ModifyMesh();
        }

        void ModifyMesh()
        {
            if (ppEnableAngle)
                MeshGenerator_Angle();
            else
                MeshGenerator_NoAngle();
        }


        void MeshGenerator_NoAngle()
        {
            float OffsetVertex = 0.5f;
            ppPlaneOffset.y = 0;
            ppAngle = 0;
            ppAngleDensity = 1;
            ppEnableLandscapeFitter = false;

            Mesh m = new Mesh();

            vertx = new Vector3[ppPlaneSize * ppPlaneSize * 4];
            trisx = new int[ppPlaneSize * ppPlaneSize * 6];
            uvs = new Vector2[ppPlaneSize * ppPlaneSize * 4];

            int v = 0;
            int t = 0;

            for (int x = 0; x < ppPlaneSize; x++)
            {
                for (int y = 0; y < ppPlaneSize; y++)
                {
                    Vector3 cellOffset = new Vector3(x, 0, y);

                    vertx[v] = new Vector3(-OffsetVertex, 0, -OffsetVertex) + cellOffset + ppPlaneOffset;
                    vertx[v + 1] = new Vector3(-OffsetVertex, 0, OffsetVertex) + cellOffset + ppPlaneOffset;
                    vertx[v + 2] = new Vector3(OffsetVertex, 0, -OffsetVertex) + cellOffset + ppPlaneOffset;
                    vertx[v + 3] = new Vector3(OffsetVertex, 0, OffsetVertex) + cellOffset + ppPlaneOffset;

                    trisx[t] = v;
                    trisx[t + 1] = v + 1;
                    trisx[t + 2] = v + 2;
                    trisx[t + 3] = v + 2;
                    trisx[t + 4] = v + 1;
                    trisx[t + 5] = v + 3;

                    uvs[v] = new Vector2(vertx[v].x, vertx[v].y);
                    uvs[v + 1] = new Vector2(vertx[v].x, vertx[v].y - 1);
                    uvs[v + 2] = new Vector2(vertx[v].x - 1, vertx[v].y);
                    uvs[v + 3] = new Vector2(vertx[v].x - 1, vertx[v].y - 1);

                    v += 4;
                    t += 6;
                }
            }

            m.vertices = vertx;
            m.triangles = trisx;
            m.uv = uvs;
            m.RecalculateNormals();
            m.RecalculateBounds();
            m.RecalculateTangents();

            meshF.sharedMesh = m;
        }

        void MeshGenerator_Angle()
        {
            float OffsetVertex = 0.5f;
            ppPlaneOffset.y = 0;

            if (!ppEnableAngle)
            {
                ppAngle = 0;
                ppAngleDensity = 1;
                ppEnableLandscapeFitter = false;
            }

            Mesh m = new Mesh();

            vertx = new Vector3[ppPlaneSizeAngle * 4];
            trisx = new int[ppPlaneSizeAngle * 6];
            uvs = new Vector2[ppPlaneSizeAngle * 4];

            int v = 0;
            int t = 0;
            float a = 0;
            float aoff = 0;

            for (int x = 0; x < ppPlaneSizeAngle; x++)
            {
                int vy1Off = 0;
                int vy2Off = 0;
                if (v - 1 > 0 && v - 2 > 0)
                {
                    vy1Off = 2;
                    vy2Off = 1;
                }
                Vector3 cellOffset = new Vector3(x, 0, 0);

                vertx[v] = new Vector3(-OffsetVertex, vertx[v - vy2Off].y, -OffsetVertex) + cellOffset + ppPlaneOffset;
                vertx[v + 1] = new Vector3(-OffsetVertex, vertx[v - vy1Off].y, OffsetVertex) + cellOffset + ppPlaneOffset;
                vertx[v + 2] = new Vector3(OffsetVertex, a, -OffsetVertex) + cellOffset + ppPlaneOffset;
                vertx[v + 3] = new Vector3(OffsetVertex, a, OffsetVertex) + cellOffset + ppPlaneOffset;

                trisx[t] = v;
                trisx[t + 1] = v + 1;
                trisx[t + 2] = v + 2;
                trisx[t + 3] = v + 2;
                trisx[t + 4] = v + 1;
                trisx[t + 5] = v + 3;

                uvs[v] = new Vector2(vertx[v].x, vertx[v].y);
                uvs[v + 1] = new Vector2(vertx[v].x, vertx[v].y - 1);
                uvs[v + 2] = new Vector2(vertx[v].x - 1, vertx[v].y);
                uvs[v + 3] = new Vector2(vertx[v].x - 1, vertx[v].y - 1);

                v += 4;
                t += 6;

                aoff += ppAngle;
                a += ppAngle + a / ppAngleDensity;
            }
            Vector3 point1 = transform.TransformPoint(vertx[vertx.Length - 1]);
            Vector3 point2 = transform.TransformPoint(vertx[vertx.Length - 2]);
            GizmoPosition1 = point1;
            GizmoPosition2 = point2;

            m.vertices = vertx;
            m.triangles = trisx;
            m.uv = uvs;
            m.RecalculateNormals();
            m.RecalculateBounds();
            m.RecalculateTangents();

            meshF.sharedMesh = m;

            if (ppEnableLandscapeFitter)
                ModifyMesh_FitToLandscape();
        }

        private void ModifyMesh_FitToLandscape()
        {
            Vector3 point1 = transform.TransformPoint(vertx[vertx.Length - 1]);

            Ray r = new Ray(new Vector3(point1.x,transform.position.y, point1.z), Vector3.down);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(r, out hit))
            {
                if (hit.collider)
                {
                    if (point1.y > hit.point.y + 2)
                        ppAngle -= ppTranslationSpeed * Time.deltaTime;
                    else if (point1.y < hit.point.y)
                        ppAngle += ppTranslationSpeed * Time.deltaTime;
                }
            }
        }
    }
}