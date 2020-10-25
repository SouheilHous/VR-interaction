using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Mesh Noise")]
    public class MDM_MeshNoise : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Mesh Noise = Component for objects with Mesh Renderer
        //---Perlin Noise generator on the current mesh
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public enum NoiseType { OverallNoise, VerticalNoise }
        public NoiseType ppNoiseType = NoiseType.OverallNoise;

        public float ppMeshNoiseAmount = 1;
        public float ppMeshNoiseSpeed = 0.5f;
        public float ppMeshNoiseIntensity = 0.5f;

        public bool ppCreateNewReference = true;

        private Perlin ppNoise;

        private Vector3[] originalVertices;
        private Vector3[] storedVertices;

        private MeshFilter meshF;


        void Awake()
        {
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
            originalVertices = meshF.sharedMesh.vertices;
            storedVertices = originalVertices;

            ppNoise = new Perlin();
        }

        private void NoiseDoubleCheck()
        {
            meshF = GetComponent<MeshFilter>();
            meshF.sharedMesh.MarkDynamic();
            originalVertices = new Vector3[0];
            originalVertices = meshF.sharedMesh.vertices;
            storedVertices = originalVertices;
            ppNoise = new Perlin();
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;
            if (ppNoiseType == NoiseType.VerticalNoise)
                MeshNoise_UpdateVerticalNoise();
            else if (ppNoiseType == NoiseType.OverallNoise)
                MeshNoise_UpdateOverallNoise();
        }

        //---Update Vertical Noise
        public void MeshNoise_UpdateVerticalNoise()
        {
            if (!Application.isPlaying)
                NoiseDoubleCheck();
            //--Getting vertices
            Vector3[] v = storedVertices;

            //--Generating Noise
            for (int i = 0; i < v.Length; i++)
            {
                float pX = (v[i].x * ppMeshNoiseAmount) + (Time.timeSinceLevelLoad * ppMeshNoiseSpeed);
                float pZ = (v[i].z * ppMeshNoiseAmount) + (Time.timeSinceLevelLoad * ppMeshNoiseSpeed);

                v[i].y = (Mathf.PerlinNoise(pX, pZ) - 0.5f) * ppMeshNoiseIntensity;
            }

            GetComponent<MeshFilter>().sharedMesh.vertices = v;
            GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
            GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
        }
        //---Update Overall Noise
        public void MeshNoise_UpdateOverallNoise()
        {
            if (!Application.isPlaying)
                NoiseDoubleCheck();
            //--Getting vertices
            var vertices = new Vector3[originalVertices.Length];

            float timex = Time.time * ppMeshNoiseSpeed + 0.1365143f;
            float timey = Time.time * ppMeshNoiseSpeed + 1.21688f;
            float timez = Time.time * ppMeshNoiseSpeed + 2.5564f;

            //--Generating Noise
            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = originalVertices[i];
                vertex.x += ppNoise.Noise(timex + vertex.x, timex + vertex.y, timex + vertex.z) * ppMeshNoiseIntensity;
                vertex.y += ppNoise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * ppMeshNoiseIntensity;
                vertex.z += ppNoise.Noise(timez + vertex.x, timez + vertex.y, timez + vertex.z) * ppMeshNoiseIntensity;
                vertices[i] = vertex;
            }

            meshF.sharedMesh.vertices = vertices;

            meshF.sharedMesh.RecalculateNormals();
            meshF.sharedMesh.RecalculateBounds();
        }
    }
}
