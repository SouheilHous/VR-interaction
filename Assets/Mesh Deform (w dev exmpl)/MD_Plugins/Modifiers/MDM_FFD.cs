using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Threading;

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/FFD")]
    [ExecuteInEditMode]
    public class MDM_FFD : MonoBehaviour
    {

        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): FFD (Free Form Deformation) = Component for objects with Mesh Filter
        //---Deform mesh by the specific weight values. In an early stage with just 4 weight nodes only.
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool ppUpdateEveryFrame = true;

        public Vector3[] ppGeneratedPoints;
        public List<Vector3> ppGeneratedPointsOrigins = new List<Vector3>();

        public MeshFilter meshF;

        public enum FFDType_ { OnePointed, TwoPointed, ThreePointed, FourPointed};
        public FFDType_ FFDType = FFDType_.OnePointed;

        public Transform ppWeightNode0;
        public Transform ppWeightNode1;
        public Transform ppWeightNode2;
        public Transform ppWeightNode3;

        private Vector3 node0;
        private Vector3 node1;
        private Vector3 node2;
        private Vector3 node3;

        private Vector3 node0StartPos;
        private Vector3 node1StartPos;
        private Vector3 node2StartPos;
        private Vector3 node3StartPos;

        [Range(0.0f,1.0f)]
        public float ppWeight = 0.5f;
        public float ppWeightMultiplier = 1.0f;
        public float ppWeightDensity = 3.0f;
        [Range(0f, 1f)]
        public float ppWeightEffectorA = 0.5f;
        [Range(0f, 1f)]
        public float ppWeightEffectorB = 0.5f;
        [Range(0f, 1f)]
        public float ppWeightEffectorC = 0.5f;

        public bool ppCreateNewReference = true;
        public bool ppMultithreadingSupported = false;
        private Thread Multithread;
        private ManualResetEvent Multithread_ManualEvent = new ManualResetEvent(true);
        private bool threadDone = false;
        [Range(8, 25)]
        public int ppMultithreadingProcessDelay = 16;

        private void Awake()
        {
            if (ppCreateNewReference)
                MD_MeshProEditor.MeshEditor_STATIC_CreateNewReference(this.gameObject);

            if (!MD_MeshProEditor.MD_INTERNAL_TECH_CheckModifiers(this.gameObject, this.GetType().Name))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    EditorUtility.DisplayDialog("Error", "The modifier cannot be applied to this object, because the object already contains modifiers. Please, remove exists modifier to access to the selected modifier...", "OK");
#endif
                DestroyImmediate(this);
                return;
            }

            if (!GetComponent<MeshFilter>())
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    EditorUtility.DisplayDialog("Error", "The object doesn't contain Mesh Filter which is very required component...", "OK");
                DestroyImmediate(this);
#endif
                return;
            }
            if (meshF == null)
            {
                meshF = GetComponent<MeshFilter>();
                meshF.sharedMesh.MarkDynamic();
                if (meshF.sharedMesh.vertices.Length > MD_MeshProEditor.VerticesLimit)
                    ppMultithreadingSupported = true;
            }
        }

        private void Start()
        {
            if (ppMultithreadingSupported && Application.isPlaying)
            {
                Multithread = new Thread(ffdThreadWork);
                Multithread_ManualEvent = new ManualResetEvent(true);
                Multithread.Start();
            }
        }

        private void OnDrawGizmos()
        {
            if (!ppUpdateEveryFrame)
                return;

            Gizmos.color = Color.green;

            switch(FFDType)
            {
                case FFDType_.OnePointed:
                    if (ppWeightNode0)
                    {
                        Gizmos.DrawWireSphere(node0StartPos, ppWeightNode0.localScale.magnitude / 2);
                        Gizmos.DrawLine(node0StartPos, ppWeightNode0.position);
                    }
                    break;

                case FFDType_.TwoPointed:
                    if (ppWeightNode0)
                    {
                        Gizmos.DrawWireSphere(node0StartPos, ppWeightNode0.localScale.magnitude / 2);
                        Gizmos.DrawLine(node0StartPos, ppWeightNode0.position);
                    }
                    if (ppWeightNode1)
                    {
                        Gizmos.DrawWireSphere(node1StartPos, ppWeightNode1.localScale.magnitude / 2);
                        Gizmos.DrawLine(node1StartPos, ppWeightNode1.position);
                    }
                    break;

                case FFDType_.ThreePointed:
                    if (ppWeightNode0)
                    {
                        Gizmos.DrawWireSphere(node0StartPos, ppWeightNode0.localScale.magnitude / 2);
                        Gizmos.DrawLine(node0StartPos, ppWeightNode0.position);
                    }
                    if (ppWeightNode1)
                    {
                        Gizmos.DrawWireSphere(node1StartPos, ppWeightNode1.localScale.magnitude / 2);
                        Gizmos.DrawLine(node1StartPos, ppWeightNode1.position);
                    }
                    if (ppWeightNode2)
                    {
                        Gizmos.DrawWireSphere(node2StartPos, ppWeightNode2.localScale.magnitude / 2);
                        Gizmos.DrawLine(node2StartPos, ppWeightNode2.position);
                    }
                    break;

                case FFDType_.FourPointed:
                    if (ppWeightNode0)
                    {
                        Gizmos.DrawWireSphere(node0StartPos, ppWeightNode0.localScale.magnitude / 2);
                        Gizmos.DrawLine(node0StartPos, ppWeightNode0.position);
                    }
                    if (ppWeightNode1)
                    {
                        Gizmos.DrawWireSphere(node1StartPos, ppWeightNode1.localScale.magnitude / 2);
                        Gizmos.DrawLine(node1StartPos, ppWeightNode1.position);
                    }
                    if (ppWeightNode2)
                    {
                        Gizmos.DrawWireSphere(node2StartPos, ppWeightNode2.localScale.magnitude / 2);
                        Gizmos.DrawLine(node2StartPos, ppWeightNode2.position);
                    }
                    if (ppWeightNode3)
                    {
                        Gizmos.DrawWireSphere(node3StartPos, ppWeightNode3.localScale.magnitude / 2);
                        Gizmos.DrawLine(node3StartPos, ppWeightNode3.position);
                    }
                    break;
            }
        }

        void Update()
        {
            if (!ppUpdateEveryFrame)
                return;
            if (!meshF)
                return;
            if (ppGeneratedPoints == null)
                return;
            if (ppGeneratedPoints.Length != meshF.sharedMesh.vertices.Length)
                return;
            if (ppGeneratedPointsOrigins == null)
                return;
            if (ppGeneratedPointsOrigins.Count != meshF.sharedMesh.vertices.Length)
                return;

            if (ppMultithreadingSupported)
            {
                if (ppWeightNode0)
                    node0 = ppWeightNode0.position;
                if (ppWeightNode1)
                    node1 = ppWeightNode1.position;
                if (ppWeightNode2)
                    node2 = ppWeightNode2.position;
                if (ppWeightNode3)
                    node3 = ppWeightNode3.position;
                if (ppGeneratedPoints.Length == 0)
                    return;
                if(threadDone)
                    FFD_UpdateMesh();
                Multithread_ManualEvent.Set();
                return;
            }

            switch(FFDType)
            {
                case FFDType_.OnePointed:
                    if (ppWeightNode0)
                    {
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = VecInterpolation(ppGeneratedPointsOrigins[i], ppWeightNode0.position, (1 / Mathf.Pow(Vector3.Distance(ppGeneratedPointsOrigins[i], ppWeightNode0.position), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - ppWeightNode0.position).normalized.magnitude);
                    }
                    break;

                case FFDType_.TwoPointed:
                    if (ppWeightNode0 && ppWeightNode1)
                    {
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = InterpolationOfTwoPointedFFD(ppGeneratedPointsOrigins[i], ppWeightNode0.position, ppWeightNode1.position);
                    }
                    break;

                case FFDType_.ThreePointed:
                    if (ppWeightNode0 && ppWeightNode1 && ppWeightNode2)
                    {
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = InterpolationOfThreePointedFFD(ppGeneratedPointsOrigins[i], ppWeightNode0.position, ppWeightNode1.position, ppWeightNode2.position);
                    }
                    break;

                case FFDType_.FourPointed:
                    if (ppWeightNode0 && ppWeightNode1 && ppWeightNode2 && ppWeightNode3)
                    {
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = InterpolationOfFourPointedFFD(ppGeneratedPointsOrigins[i], ppWeightNode0.position, ppWeightNode1.position, ppWeightNode2.position, ppWeightNode3.position);
                    }
                    break;
            }

            FFD_UpdateMesh();
        }

        /// <summary>
        /// Update current mesh state (in case if Update Every Frame is disabled)
        /// </summary>
        public void FFD_UpdateMesh()
        {
            if (!meshF)
                return;
            if (ppGeneratedPoints == null)
                return;
            if (ppGeneratedPoints.Length != meshF.sharedMesh.vertices.Length)
                return;

            Vector3[] verts = meshF.sharedMesh.vertices;
            for (int i = 0; i < ppGeneratedPoints.Length; i++)
                verts[i] = transform.InverseTransformPoint(ppGeneratedPoints[i]);
            meshF.sharedMesh.vertices = verts;
            meshF.sharedMesh.RecalculateBounds();
            meshF.sharedMesh.RecalculateNormals();
            meshF.sharedMesh.RecalculateTangents();
        }

        /// <summary>
        /// Apply & register FFD weights
        /// </summary>
        public void FFD_ApplyWeights()
        {
            ppGeneratedPoints = meshF.sharedMesh.vertices;
            if(ppWeightNode0)
                node0StartPos = ppWeightNode0.position;
            if (ppWeightNode1)
                node1StartPos = ppWeightNode1.position;
            if (ppWeightNode2)
                node2StartPos = ppWeightNode2.position;
            if (ppWeightNode3)
                node3StartPos = ppWeightNode3.position;

            ppGeneratedPointsOrigins.Clear();
            for (int i = 0; i < ppGeneratedPoints.Length; i++)
                ppGeneratedPointsOrigins.Add(transform.TransformPoint(ppGeneratedPoints[i]));
        }


        private Vector3 InterpolationOfFourPointedFFD(Vector3 p, Vector3 n0, Vector3 n1, Vector3 n2, Vector3 n3)
        {
            return VecInterpolation(
                VecInterpolation(
                    VecInterpolation(
                        VecInterpolation(p, n0, (1 / Mathf.Pow(Vector3.Distance(p, n0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - n0).magnitude),
                        VecInterpolation(p, n1, (1 / Mathf.Pow(Vector3.Distance(p, n1), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node1StartPos - n1).magnitude),
                        ppWeightEffectorA),
                    VecInterpolation(
                        VecInterpolation(p, n0, (1 / Mathf.Pow(Vector3.Distance(p, n0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - n0).magnitude),
                        VecInterpolation(p, n2, (1 / Mathf.Pow(Vector3.Distance(p, n2), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node2StartPos - n2).magnitude),
                        ppWeightEffectorA), 
                    ppWeightEffectorB),
                 VecInterpolation(
                    VecInterpolation(
                        VecInterpolation(p, n0, (1 / Mathf.Pow(Vector3.Distance(p, n0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - n0).magnitude),
                        VecInterpolation(p, n3, (1 / Mathf.Pow(Vector3.Distance(p, n3), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node3StartPos - n3).magnitude),
                        ppWeightEffectorA),
                    VecInterpolation(
                        VecInterpolation(p, n0, (1 / Mathf.Pow(Vector3.Distance(p, n0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - n0).magnitude),
                        VecInterpolation(p, n3, (1 / Mathf.Pow(Vector3.Distance(p, n3), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node3StartPos - n3).magnitude),
                        ppWeightEffectorA),
                    ppWeightEffectorB), 
                ppWeightEffectorC);
        }

        private Vector3 InterpolationOfThreePointedFFD(Vector3 p, Vector3 n0, Vector3 n1, Vector3 n2)
        {
            return VecInterpolation(VecInterpolation(
                VecInterpolation(p, n0, (1 / Mathf.Pow(Vector3.Distance(p, n0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - n0).magnitude),
                VecInterpolation(p, n1, (1 / Mathf.Pow(Vector3.Distance(p, n1), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node1StartPos - n1).magnitude), 
                ppWeightEffectorA), 
                VecInterpolation(
                VecInterpolation(p, n0, (1 / Mathf.Pow(Vector3.Distance(p, n0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - n0).magnitude),
                VecInterpolation(p, n2, (1 / Mathf.Pow(Vector3.Distance(p, n2), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node2StartPos - n2).magnitude),
                ppWeightEffectorA), ppWeightEffectorB);
        }

        private Vector3 InterpolationOfTwoPointedFFD(Vector3 p, Vector3 n0, Vector3 n1)
        {
            return VecInterpolation(
                VecInterpolation(p, n0, (1 / Mathf.Pow(Vector3.Distance(p, n0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - n0).magnitude),
                VecInterpolation(p, n1, (1 / Mathf.Pow(Vector3.Distance(p, n1), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node1StartPos - n1).magnitude),
                ppWeightEffectorA);
        }

        private Vector3 VecInterpolation(Vector3 A, Vector3 B, float t)
        {
            Vector3 final = t * (B - A) + A;
            return final;
        }


        private void ffdThreadWork()
        {
            while (true)
            {
                threadDone = false;
                Multithread_ManualEvent.WaitOne();
                if (ppGeneratedPointsOrigins == null)
                    continue;
                if (ppGeneratedPointsOrigins.Count == 0)
                    continue;

                switch (FFDType)
                {
                    case FFDType_.OnePointed:
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = VecInterpolation(ppGeneratedPointsOrigins[i], node0, (1 / Mathf.Pow(Vector3.Distance(ppGeneratedPointsOrigins[i], node0), ppWeightDensity)) * ppWeight * ppWeightMultiplier * (node0StartPos - node0).magnitude);
                        break;

                    case FFDType_.TwoPointed:
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = InterpolationOfTwoPointedFFD(ppGeneratedPointsOrigins[i], node0, node1);
                        break;

                    case FFDType_.ThreePointed:
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = InterpolationOfThreePointedFFD(ppGeneratedPointsOrigins[i], node0, node1, node2);
                        break;

                    case FFDType_.FourPointed:
                        for (int i = 0; i < ppGeneratedPoints.Length; i++)
                            ppGeneratedPoints[i] = InterpolationOfFourPointedFFD(ppGeneratedPointsOrigins[i], node0, node1, node2, node3);
                        break;
                }
                threadDone = true;
                Thread.Sleep(ppMultithreadingProcessDelay);
                Multithread_ManualEvent.Reset();
            }
        }

        private void OnApplicationQuit()
        {
            if (Multithread != null && Multithread.IsAlive)
                Multithread.Abort();
        }
        private void OnDestroy()
        {
            if (Multithread != null && Multithread.IsAlive)
                Multithread.Abort();
        }
        private void OnDisable()
        {
            if (Multithread != null && Multithread.IsAlive)
                Multithread.Abort();
        }
    }
}

#if UNITY_EDITOR
namespace MD_Plugin
{
    [CustomEditor(typeof(MDM_FFD))]
    public class MDM_FFDEditor : Editor
    {
        private MDM_FFD targ;
        private void OnEnable()
        {
            targ = (MDM_FFD)target;
        }

        public override void OnInspectorGUI()
        {
            s();
            DrawProperty("ppUpdateEveryFrame", "Update Every Frame", "If enabled, the script will refresh its logic every frame (even in Update)");
            s(10);

            bv();
            DrawProperty("FFDType", "FFD Type");
            DrawProperty("ppWeightNode0", "Weight Node 0");
            if (targ.FFDType == MDM_FFD.FFDType_.TwoPointed)
                DrawProperty("ppWeightNode1", "Weight Node 1");
            else if(targ.FFDType == MDM_FFD.FFDType_.ThreePointed)
            {
                DrawProperty("ppWeightNode1", "Weight Node 1");
                DrawProperty("ppWeightNode2", "Weight Node 2");
            }
            else if (targ.FFDType == MDM_FFD.FFDType_.FourPointed)
            {
                DrawProperty("ppWeightNode1", "Weight Node 1");
                DrawProperty("ppWeightNode2", "Weight Node 2");
                DrawProperty("ppWeightNode3", "Weight Node 3");
            }
            ev();
            s(10);

            l("FFD Parameters");
            bv();
            DrawProperty("ppWeight", "Weight","Total weight for FFD");
            DrawProperty("ppWeightMultiplier", "Weight Multiplier", "Additional weight multiplier to FFD");
            DrawProperty("ppWeightDensity", "Weight Density");
            if(targ.FFDType == MDM_FFD.FFDType_.TwoPointed)
                DrawProperty("ppWeightEffectorA", "Weight Effector", "Effector value between two weight nodes");
            else if(targ.FFDType == MDM_FFD.FFDType_.ThreePointed)
            {
                DrawProperty("ppWeightEffectorA", "Weight Effector A", "Effector value between two weight nodes");
                DrawProperty("ppWeightEffectorB", "Weight Effector B", "Effector value between three weight nodes");
            }
            else if (targ.FFDType == MDM_FFD.FFDType_.FourPointed)
            {
                DrawProperty("ppWeightEffectorA", "Weight Effector A", "Effector value between two weight nodes");
                DrawProperty("ppWeightEffectorB", "Weight Effector B", "Effector value between three weight nodes");
                DrawProperty("ppWeightEffectorC", "Weight Effector C", "Effector value between Four weight nodes");
            }
            ev();
            s(5);

            l("Advanced");
            bv();
            if (targ.meshF.sharedMesh.vertices.Length > MD_MeshProEditor.VerticesLimit)
                EditorGUILayout.HelpBox("The multithreading support is recommended as the mesh has more than " + MD_MeshProEditor.VerticesLimit.ToString() + " vertices",MessageType.Warning);
            DrawProperty("ppMultithreadingSupported", "Multithreading Supported");
            if (targ.ppMultithreadingSupported)
                DrawProperty("ppMultithreadingProcessDelay", "Multithreading Process Delay","The bigger value is, the smoother result you get");
            ev();
            s(10);
            bv();
            if (b("Refresh FFD Weights"))
                targ.FFD_ApplyWeights();
            ev();
            s(7);

            if(serializedObject != null)
                serializedObject.Update();

            Color c = Color.black;
            ColorUtility.TryParseHtmlString("#f2d0d0", out c);
            GUI.color = c;
            GUILayout.Space(5);
            if (GUILayout.Button("Back To Mesh Editor"))
            {
                GameObject gm = targ.gameObject;
                DestroyImmediate(targ);
                gm.AddComponent<MD_MeshProEditor>();
            }
        }

        bool b(string txt)
        {
            return GUILayout.Button(txt);
        }

        private void l(string label)
        {
            GUILayout.Label(label);
        }
        private void bh()
        {
            GUILayout.BeginHorizontal("Box");
        }
        private void eh()
        {
            GUILayout.EndHorizontal();
        }
        private void bv()
        {
            GUILayout.BeginVertical("Box");
        }
        private void ev()
        {
            GUILayout.EndVertical();
        }

        private void s(float sp = 5)
        {
            GUILayout.Space(sp);
        }
        private void DrawProperty(string PropertyName, string Text = "", string ToolTip = "", bool includeChilds = false, Texture img = null)
        {
            try
            {
                if (string.IsNullOrEmpty(Text))
                    Text = PropertyName;
                if (img == null)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(PropertyName), new GUIContent(Text, ToolTip), includeChilds, null);
                else
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(PropertyName), new GUIContent(Text, img), includeChilds, null);
            }
            catch { Debug.Log("Error with " + PropertyName); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif