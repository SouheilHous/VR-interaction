using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.EventSystems;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Sculpting Pro")]
    public class MDM_SculptingPro : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Sculpting Pro = Component for objects with Mesh Renderer
        //---Full sculpting system for mesh renderers in editor or at runtime
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool SS_AtRuntime = false;
        public bool SS_InEditMode = false;
        public bool SS_MobileSupport = false;

        public bool SS_UseBrushProjection = true;
        public GameObject SS_BrushProjection;

        public float SS_BrushSize = 0.5f;
        public float SS_BrushStrength = 0.05f;

        public bool SS_MultithreadingSupported = false;
        [Range(2,20)]
        public int SS_MultithreadingProcessDelay = 10;
        private Thread Multithread;
        ManualResetEvent Multithread_ManualEvent = new ManualResetEvent(true);
        [System.Serializable]
        public class Multithreading_Internal
        {
            public Vector3 mth_WorldPoint;
            public float mth_Radius;
            public float mth_Strength;
            public SS_State_Internal mth_State;
            public Vector3 mth_WorldPos;
            public Quaternion mth_WorldRot;
            public Vector3 mth_WorldScale;
            public Vector3 mth_Direction;
            public SS_NoiseFunctDirections mth_NoiseDirs;
            public SS_SculptingType_ SS_SculptingType;
            public int SS_MultithreadingProcessDelay;

            public void SetParams(Vector3 worldPoint, float Radius, float Strength, Vector3 Dir, SS_State_Internal State, Vector3 RealPos, Vector3 RealScale, Quaternion RealRot, SS_NoiseFunctDirections NoiseDirs, SS_SculptingType_ SS_SculptingType__, int SS_MultithreadingProcessDelay_)
            {
                mth_WorldPoint = worldPoint;
                mth_Radius = Radius / 20;
                mth_Strength = Strength;
                mth_State = State;
                mth_WorldPos = RealPos;
                mth_WorldRot = RealRot;
                mth_WorldScale = RealScale;
                mth_Direction = Dir;
                mth_NoiseDirs = NoiseDirs;
                SS_SculptingType = SS_SculptingType__;
                SS_MultithreadingProcessDelay = SS_MultithreadingProcessDelay_;
            }
        }
        public Multithreading_Internal Multithreading_Manager;

        public enum SS_State_Internal : int { None = 0, Raise = 1, Lower = 2, Revert = 3, Noise = 4 };
        public SS_State_Internal SS_State = SS_State_Internal.None;

        public enum SS_MeshSculptMode_Internal { VerticesDirection, BrushDirection, CustomDirection, CustomDirectionObject, InternalScriptDirection };
        public SS_MeshSculptMode_Internal SS_MeshSculptMode = SS_MeshSculptMode_Internal.BrushDirection;
        public Vector3 SS_CustomDirection;
        public bool SS_EnableHeightLimitations = false;
        public Vector2 SS_HeightLimitations;
        public enum SS_CustomDirObjDirection_Internal { Up, Down, Forward, Back, Right, Left};
        public SS_CustomDirObjDirection_Internal SS_CustomDirObjDirection;
        public GameObject SS_CustomDirectionObject;
        public bool SS_UpdateColliderAfterRelease = true;
        public enum SS_SculptingType_ { Linear, Exponential};
        public SS_SculptingType_ SS_SculptingType = SS_SculptingType_.Exponential;

        //---Runtime Settings
        public bool SS_UseInput = true;
        public bool SS_UseRaiseFunct = true;
        public bool SS_UseLowerFunct = true;
        public bool SS_UseRevertFunct = true;
        public bool SS_UseNoiseFunct = true;
        public enum SS_NoiseFunctDirections { XYZ, XZ, XY, YZ, Z, Y, X};
        public SS_NoiseFunctDirections SS_NoiseFunctionDirections = SS_NoiseFunctDirections.XYZ;
        public KeyCode SS_SculptingRaiseInput = KeyCode.Mouse0;
        public KeyCode SS_SculptingLowerInput = KeyCode.Mouse1;
        public KeyCode SS_SculptingRevertInput = KeyCode.Mouse2;
        public KeyCode SS_SculptingNoiseInput = KeyCode.LeftControl;

        public bool SS_SculptFromCursor = true;

        public Transform SS_SculptOrigin;
        //----------------------

        public MeshFilter mesh;
        public List<Vector3> StoredVertices = new List<Vector3>();
        public Vector3[] ThreatedPoints;

        public bool SSInternal_Set = false;

        public bool SSCreateNewReference = true;

       
        private void Awake()
        {
            Mesh m = Instantiate(mesh.sharedMesh);
            mesh.sharedMesh = m;

            if (SSInternal_Set)
                return;

            if (SSCreateNewReference)
                MD_MeshProEditor.MeshEditor_STATIC_CreateNewReference(this.gameObject);

            mesh = GetComponent<MeshFilter>();
            if (mesh == null || (mesh && mesh.sharedMesh == null))
            {
                MD_Debug.Debug(this, "Sculpting Pro: The object doesn't contain Mesh Filter.", MD_Debug.DebugType.Error);
                this.enabled = false;
                return;
            }
            SS_Funct_RefreshMeshCollider();

#if UNITY_EDITOR
            if (this.mesh.sharedMesh.vertices.Length > MD_MeshProEditor.VerticesLimit)
            {
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Mesh has more than " + MD_MeshProEditor.VerticesLimit.ToString() + " vertices", "Your selected mesh has more than " + MD_MeshProEditor.VerticesLimit.ToString() + " vertices [" + GetComponent<MeshFilter>().sharedMesh.vertices.Length.ToString() + "]. This could slow down the editor performance. The *Multithreading Support* will be enabled...", "Ok");
                    SS_MultithreadingSupported = true;
                }
            }
#endif
            SSInternal_Set = true;
        }

        private void Start()
        {
            if (mesh == null || (mesh && mesh.sharedMesh == null))
                return;

            StoredVertices.Clear();

            if (SS_BrushProjection == null)
            {
                GameObject brushProj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Material m = new Material(Shader.Find("Transparent/Diffuse"));
                m.color = new Color(0, 128, 0, 0.2f);
                brushProj.GetComponent<Renderer>().sharedMaterial = m;
                DestroyImmediate(brushProj.GetComponent<Collider>());
                brushProj.name = "Brush_Object";

                if (SS_MultithreadingSupported)
                {
                    GameObject br = new GameObject("Brush_ObjectRoot");
                    brushProj.transform.parent = br.transform;
                    brushProj.transform.localPosition = Vector3.zero;
                    brushProj.transform.localRotation = Quaternion.identity;
                    brushProj.transform.localScale = Vector3.one * 0.1f;

                    SS_BrushProjection = br;
                    SS_BrushProjection.SetActive(false);
                }
                else
                {
                    brushProj.SetActive(false);
                    SS_BrushProjection = brushProj;
                }
            }

            StoredVertices.AddRange(mesh.sharedMesh.vertices);
            ThreatedPoints = mesh.sharedMesh.vertices;

            if (SS_MultithreadingSupported)
            {
                Multithread = new Thread(SS_Funct_DoSculpting_ThreadWorker);
                Multithread.Start();

                CheckForThread(true, 0.5f);
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
                return;

            if(SS_MultithreadingSupported)
            {
                if (Multithreading_Manager == null)
                {
                    Multithreading_Manager = new Multithreading_Internal();
                    Multithread_ManualEvent = new ManualResetEvent(true);
                }
                Multithreading_Manager.mth_WorldPos = transform.position;
                Multithreading_Manager.mth_WorldRot = transform.rotation;
                Multithreading_Manager.mth_WorldScale = transform.localScale;
            }

            if (!SS_AtRuntime)
                return;

            if (!SS_UseInput)
                return;

            if (SS_BrushProjection && SS_BrushProjection.GetComponent<Collider>())
                DestroyImmediate(SS_BrushProjection.GetComponent<Collider>());

            bool eventSystemOverObject = false;
            if (EventSystem.current != null)
                eventSystemOverObject = EventSystem.current.IsPointerOverGameObject();

            Ray r = new Ray();
            if (!SS_MobileSupport)
            {
                if (SS_SculptFromCursor)
                {
                    if (Camera.main != null)
                        r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    else if (GetComponent<Camera>())
                        r = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    else
                    {
                        Debug.LogError("Camera component is missing! Please set your camera tag to MainCamera or add Mesh Sculpting to camera component.");
                        return;
                    }
                }
                else
                    r = new Ray(SS_SculptOrigin.transform.position, SS_SculptOrigin.transform.forward);
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (Camera.main != null)
                        r = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    else if (GetComponent<Camera>())
                        r = GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
                    else
                    {
                        Debug.LogError("Camera component is missing! Please set your camera tag to MainCamera or add Mesh Sculpting to camera component.");
                        return;
                    }
                }
            }
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(r, out hit) && !eventSystemOverObject)
            {
                if (hit.collider == this.GetComponent<Collider>())
                {
                    SS_BrushProjection.SetActive(true);
                    SS_BrushProjection.transform.position = hit.point;
                    SS_BrushProjection.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                    SS_BrushProjection.transform.localScale = new Vector3(SS_BrushSize, SS_BrushSize, SS_BrushSize);

                    if (SS_BrushProjection && SS_UseBrushProjection && SS_BrushProjection.GetComponent<Renderer>())
                        SS_BrushProjection.GetComponent<Renderer>().enabled = true;
                    else if(!SS_UseBrushProjection)
                    {
                        if (SS_BrushProjection.GetComponent<Renderer>() && SS_BrushProjection.GetComponent<Renderer>().enabled)
                            SS_BrushProjection.GetComponent<Renderer>().enabled = false;
                    }

                    SS_Funct_ManageControls(hit);
                }
                else
                    SS_BrushProjection.SetActive(false);
            }
            else
                SS_BrushProjection.SetActive(false);

            if(SS_UpdateColliderAfterRelease)
            {
                if (SS_MobileSupport)
                {
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                        SS_Funct_RefreshMeshCollider();
                }
                else
                {
                    if (Input.GetKeyUp(SS_SculptingRaiseInput) && SS_UseRaiseFunct)
                        SS_Funct_RefreshMeshCollider();
                    if (Input.GetKeyUp(SS_SculptingLowerInput) && SS_UseLowerFunct)
                        SS_Funct_RefreshMeshCollider();
                    if (Input.GetKeyUp(SS_SculptingRevertInput) && SS_UseRevertFunct)
                        SS_Funct_RefreshMeshCollider();
                    if (Input.GetKeyUp(SS_SculptingRevertInput) && SS_UseNoiseFunct)
                        SS_Funct_RefreshMeshCollider();
                }
            }
        }

        private void SS_Funct_ManageControls(RaycastHit hit)
        {
            if (SS_MobileSupport)
            {
                if (MD_PRIVATE_FUNCT_GetControlInput(KeyCode.Space) && SS_UseRaiseFunct && SS_State == SS_State_Internal.Raise)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Raise);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
                else if (MD_PRIVATE_FUNCT_GetControlInput(KeyCode.Space) && SS_UseLowerFunct && SS_State == SS_State_Internal.Lower)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Lower);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
                else if (MD_PRIVATE_FUNCT_GetControlInput(KeyCode.Space) && SS_UseRevertFunct && SS_State == SS_State_Internal.Revert)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Revert);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
                else if (MD_PRIVATE_FUNCT_GetControlInput(KeyCode.Space) && SS_UseNoiseFunct && SS_State == SS_State_Internal.Noise)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Noise);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
            }
            else
            {
                if (MD_PRIVATE_FUNCT_GetControlInput(SS_SculptingRaiseInput) && SS_UseRaiseFunct)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Raise);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
                else if (MD_PRIVATE_FUNCT_GetControlInput(SS_SculptingLowerInput) && SS_UseLowerFunct)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Lower);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
                else if (MD_PRIVATE_FUNCT_GetControlInput(SS_SculptingRevertInput) && SS_UseRevertFunct)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Revert);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
                else if (MD_PRIVATE_FUNCT_GetControlInput(SS_SculptingNoiseInput) && SS_UseNoiseFunct)
                {
                    SS_Funct_DoSculpting(hit.point, SS_BrushProjection.transform.forward, SS_BrushSize, SS_BrushStrength, SS_State_Internal.Noise);
                    if (!SS_UpdateColliderAfterRelease)
                        SS_Funct_RefreshMeshCollider();
                }
            }
        }
        
        /// <summary>
        /// Restore original mesh
        /// </summary>
        public void SS_Funct_RestoreOriginal()
        {
            if (mesh == null || (mesh && mesh.sharedMesh == null))
            {
                Debug.Log("Sculpting Pro: The object doesn't contain Mesh Filter.");
                return;
            }

            if (StoredVertices.Count == 0)
                return;

            mesh.sharedMesh.vertices = StoredVertices.ToArray();
            if (SS_MultithreadingSupported)
                ThreatedPoints = mesh.sharedMesh.vertices;
            SS_Funct_RefreshMeshCollider();
        }
        /// <summary>
        /// Sculpt current mesh by specific parameters
        /// </summary>
        /// <param name="WorldPoint">World point [example: raycast hit point]</param>
        /// <param name="Radius">Range or radius of the point</param>
        /// <param name="Strength">Strenght & hardness of the sculpting</param>
        /// <param name="State">Sculpting state</param>
        public void SS_Funct_DoSculpting(Vector3 WorldPoint, Vector3 Direction, float Radius, float Strength, SS_State_Internal State)
        {
            mesh.sharedMesh.RecalculateNormals();
            if (SS_MultithreadingSupported) //---Multithreaded sculpting
            {
                if (!threadDone)
                    return;
                if (SS_MeshSculptMode == SS_MeshSculptMode_Internal.CustomDirectionObject)
                {
                    if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Up)
                        Direction = SS_CustomDirectionObject.transform.up;
                    else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Down)
                        Direction = -SS_CustomDirectionObject.transform.up;
                    else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Forward)
                        Direction = SS_CustomDirectionObject.transform.forward;
                    else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Back)
                        Direction = -SS_CustomDirectionObject.transform.forward;
                    else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Right)
                        Direction = SS_CustomDirectionObject.transform.right;
                    else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Left)
                        Direction = -SS_CustomDirectionObject.transform.right;
                }
                Multithreading_Manager.SetParams(transform.InverseTransformPoint(WorldPoint), Radius, Strength, Direction, State, transform.position,transform.localScale,transform.rotation, SS_NoiseFunctionDirections, SS_SculptingType,SS_MultithreadingProcessDelay);
                mesh.sharedMesh.vertices = ThreatedPoints;
                Multithread_ManualEvent.Set();
            }
            else //---Normal sculpting
            {
                List<Vector3> RaisePoints = new List<Vector3>();
                RaisePoints.AddRange(mesh.sharedMesh.vertices);
                int i = 0;
                while (i < mesh.sharedMesh.vertices.Length)
                {
                    if (Vector3.Distance(transform.TransformPoint(RaisePoints[i]), WorldPoint) < Radius)
                    {
                        float str = Strength;
                        if (State == SS_State_Internal.Raise)
                            str *= -1;

                        Vector3 origin = transform.TransformPoint(RaisePoints[i]);
                        if (State == SS_State_Internal.Revert)
                            RaisePoints[i] = Vector3.Lerp(RaisePoints[i], StoredVertices[i], 0.1f);
                        else
                        {
                            Vector3 Dir = Direction;

                            if (SS_MeshSculptMode == SS_MeshSculptMode_Internal.CustomDirection)
                                Dir = SS_CustomDirection;
                            else if (SS_MeshSculptMode == SS_MeshSculptMode_Internal.CustomDirectionObject)
                            {
                                if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Up)
                                    Dir = SS_CustomDirectionObject.transform.up;
                                else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Down)
                                    Dir = -SS_CustomDirectionObject.transform.up;
                                else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Forward)
                                    Dir = SS_CustomDirectionObject.transform.forward;
                                else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Back)
                                    Dir = -SS_CustomDirectionObject.transform.forward;
                                else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Right)
                                    Dir = SS_CustomDirectionObject.transform.right;
                                else if (SS_CustomDirObjDirection == SS_CustomDirObjDirection_Internal.Left)
                                    Dir = -SS_CustomDirectionObject.transform.right;
                            }
                            else if (SS_MeshSculptMode == SS_MeshSculptMode_Internal.VerticesDirection)
                                Dir = transform.TransformDirection(-mesh.sharedMesh.vertices[i]);

                            if (SS_EnableHeightLimitations)
                            {
                                if (origin.y < SS_HeightLimitations.x && State == SS_State_Internal.Lower)
                                    str = 0;
                                if (origin.y > SS_HeightLimitations.y && State == SS_State_Internal.Raise)
                                    str = 0;
                            }

                            if (State == SS_State_Internal.Noise)
                            {
                                float rand_x = Random.Range(-0.01f, 0.01f);
                                float rand_y = Random.Range(-0.01f, 0.01f);
                                float rand_z = Random.Range(-0.01f, 0.01f);
                                switch (SS_NoiseFunctionDirections)
                                {
                                    case SS_NoiseFunctDirections.X:
                                        origin.x += rand_x * str;
                                        break;
                                    case SS_NoiseFunctDirections.Y:
                                        origin.y += rand_y * str;
                                        break;
                                    case SS_NoiseFunctDirections.Z:
                                        origin.z += rand_z * str;
                                        break;

                                    case SS_NoiseFunctDirections.XY:
                                        origin.x += rand_x * str;
                                        origin.y += rand_y * str;
                                        break;
                                    case SS_NoiseFunctDirections.XZ:
                                        origin.x += rand_x * str;
                                        origin.z += rand_z * str;
                                        break;

                                    case SS_NoiseFunctDirections.YZ:
                                        origin.y += rand_y * str;
                                        origin.z += rand_z * str;
                                        break;
                                    case SS_NoiseFunctDirections.XYZ:
                                        origin.x += rand_x * str;
                                        origin.y += rand_y * str;
                                        origin.z += rand_z * str;
                                        break;
                                }
                            }
                            else
                            {
                                str *= 0.05f;
                                if(SS_SculptingType == SS_SculptingType_.Exponential)
                                    str *= (Radius - Vector3.Distance(origin, WorldPoint));
                                origin += Dir * str;
                            }

                            RaisePoints[i] = transform.InverseTransformPoint(origin);
                        }
                    }
                    i++;
                }
                mesh.sharedMesh.vertices = RaisePoints.ToArray();
            }
        }

        bool threadDone = true;
        private void SS_Funct_DoSculpting_ThreadWorker()
        { 
            while (true)
            {
                Multithread_ManualEvent.WaitOne();

                int i = 0;
                threadDone = false;
                while (i < ThreatedPoints.Length)
                {
                    if (Vector3.Distance(ThreatedPoints[i], Multithreading_Manager.mth_WorldPoint) < Multithreading_Manager.mth_Radius)
                    {
                        float str = Multithreading_Manager.mth_Strength;
                        if (Multithreading_Manager.mth_State == SS_State_Internal.Raise)
                            str *= -1;

                        if (Multithreading_Manager.mth_State == SS_State_Internal.Revert)
                            ThreatedPoints[i] = Vector3.Lerp(ThreatedPoints[i], StoredVertices[i], 0.1f);

                        Vector3 origin = TransformPoint(Multithreading_Manager.mth_WorldPos, Multithreading_Manager.mth_WorldRot, Multithreading_Manager.mth_WorldScale, ThreatedPoints[i]);

                        if (Multithreading_Manager.mth_State != SS_State_Internal.Revert)
                        {
                            Vector3 Dir = Multithreading_Manager.mth_Direction;
                            
                            if (SS_MeshSculptMode == SS_MeshSculptMode_Internal.CustomDirection)
                                Dir = SS_CustomDirection;
                            else if (SS_MeshSculptMode == SS_MeshSculptMode_Internal.CustomDirectionObject)
                            {
                               //---sets in previous method
                            }
                             else if (SS_MeshSculptMode == SS_MeshSculptMode_Internal.VerticesDirection)
                                 Dir = TransformDirection (Multithreading_Manager.mth_WorldPos, Multithreading_Manager.mth_WorldRot, Multithreading_Manager.mth_WorldScale, -ThreatedPoints[i]);

                            if (SS_EnableHeightLimitations)
                            {
                                if (origin.y < SS_HeightLimitations.x && Multithreading_Manager.mth_State == SS_State_Internal.Lower)
                                    str = 0;
                                if (origin.y > SS_HeightLimitations.y && Multithreading_Manager.mth_State == SS_State_Internal.Raise)
                                    str = 0;
                            }

                            if (Multithreading_Manager.mth_State == SS_State_Internal.Noise)
                            {
                                float rand_x = (float)GetRandomNumber(-0.01f, 0.01f);
                                float rand_y = (float)GetRandomNumber(-0.01f, 0.01f);
                                float rand_z = (float)GetRandomNumber(-0.01f, 0.01f);
                                switch(Multithreading_Manager.mth_NoiseDirs)
                                {
                                    case SS_NoiseFunctDirections.X:
                                        origin.x += rand_x * str;
                                        break;
                                    case SS_NoiseFunctDirections.Y:
                                        origin.y += rand_y * str;
                                        break;
                                    case SS_NoiseFunctDirections.Z:
                                        origin.z += rand_z * str;
                                        break;

                                    case SS_NoiseFunctDirections.XY:
                                        origin.x += rand_x * str;
                                        origin.y += rand_y * str;
                                        break;
                                    case SS_NoiseFunctDirections.XZ:
                                        origin.x += rand_x * str;
                                        origin.z += rand_z * str;
                                        break;

                                    case SS_NoiseFunctDirections.YZ:
                                        origin.y += rand_y * str;
                                        origin.z += rand_z * str;
                                        break;
                                    case SS_NoiseFunctDirections.XYZ:
                                        origin.x += rand_x * str;
                                        origin.y += rand_y * str;
                                        origin.z += rand_z * str;
                                        break;
                                }
                            }
                            else
                            {
                                str *= 0.05f;
                                if (Multithreading_Manager.SS_SculptingType == SS_SculptingType_.Exponential)
                                    str *= (Multithreading_Manager.mth_Radius - Vector3.Distance(origin, Multithreading_Manager.mth_WorldPoint));
                                origin += Dir * str;
                            }

                            ThreatedPoints[i] = TransformPointInverse(Multithreading_Manager.mth_WorldPos, Multithreading_Manager.mth_WorldRot, Multithreading_Manager.mth_WorldScale, origin);
                        }
                    }
                    i++;
                }
                threadDone = true;
                Thread.Sleep(Multithreading_Manager.SS_MultithreadingProcessDelay);
            }
        }

        static System.Random random = new System.Random();
        public double GetRandomNumber(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        private Vector3 TransformPoint(Vector3 WorldPos, Quaternion WorldRot, Vector3 WorldScale, Vector3 Point)
        {
            var localToWorldMatrix = Matrix4x4.TRS(WorldPos, WorldRot, WorldScale);
            return localToWorldMatrix.MultiplyPoint3x4(Point);
        }
        private Vector3 TransformPointInverse(Vector3 WorldPos, Quaternion WorldRot, Vector3 WorldScale, Vector3 Point)
        {
            var localToWorldMatrix = Matrix4x4.TRS(WorldPos, WorldRot, WorldScale).inverse;
            return localToWorldMatrix.MultiplyPoint3x4(Point);
        }
        private Vector3 TransformDirection(Vector3 WorldPos, Quaternion WorldRot, Vector3 WorldScale, Vector3 Point)
        {
            var localToWorldMatrix = Matrix4x4.TRS(WorldPos, WorldRot, WorldScale);
            return localToWorldMatrix.MultiplyVector(Point);
        }

        public void CheckForThread(bool CreateSculptThread = true, float Delay = 0.1f)
        {
            StartCoroutine(CheckForThreadDelay(CreateSculptThread, Delay));
        }
        IEnumerator CheckForThreadDelay(bool CreateSculptThread, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (CreateSculptThread)
            {
                if (Multithread == null)
                {
                    Multithread = new Thread(SS_Funct_DoSculpting_ThreadWorker);
                    Multithread.Start();
                }
                else
                {
                    Multithread.Abort();
                    Multithread = null;
                    Multithread = new Thread(SS_Funct_DoSculpting_ThreadWorker);
                    Multithread.Start();
                }
            }
            else
            {
                if (Multithread != null)
                {
                    Multithread.Abort();
                    Multithread = null;
                }
            }
            yield return null;
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

        #region Additional Methods

        /// <summary>
        /// Refresh mesh collider at runtime
        /// </summary>
        public void SS_Funct_RefreshMeshCollider()
        {
            MeshCollider mc = GetComponent<MeshCollider>();
            if (!mc)
            {
                try
                {
                    this.gameObject.AddComponent<MeshCollider>();
                }
                catch
                {
                    MD_Debug.Debug(this, "Sculpting Pro: The mesh collider couldn't be added to this object.", MD_Debug.DebugType.Error);
                }
                mc = GetComponent<MeshCollider>();
            }
            
            mc.sharedMesh = mesh.sharedMesh;
            mesh.sharedMesh.RecalculateBounds();
            mesh.sharedMesh.RecalculateNormals();
        }

        /// <summary>
        /// Reset mesh transform and matrix
        /// </summary>
        public void SS_Funct_BakeMesh()
        {
            Vector3[] vertsNew = mesh.sharedMesh.vertices;
            Vector3 lastPos = transform.position;
            Quaternion lastRot = transform.rotation;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            for (int i = 0; i < vertsNew.Length; i++)
                vertsNew[i] = transform.TransformPoint(mesh.sharedMesh.vertices[i]);
            transform.localScale = Vector3.one;
            mesh.sharedMesh.vertices = vertsNew;

            transform.position = lastPos;
            transform.rotation = lastRot;
        }

        //----Public available methods for variables

        /// <summary>
        /// Change brush size by float value
        /// </summary>
        public void SS_Funct_ChangeSize(float size)
        {
            SS_BrushSize = size;
        }
        /// <summary>
        /// Change brush size by Slider value
        /// </summary>
        /// <param name="size"></param>
        public void SS_Funct_ChangeSize(UnityEngine.UI.Slider size)
        {
            SS_BrushSize = size.value;
        }

        /// <summary>
        /// Change brush strength by float value
        /// </summary>
        public void SS_Funct_ChangeStrength(float strength)
        {
            SS_BrushStrength = strength;
        }
        /// <summary>
        /// Change brush strength by Slider value
        /// </summary>
        public void SS_Funct_ChangeStrength(UnityEngine.UI.Slider strength)
        {
            SS_BrushStrength = strength.value;
        }

        /// <summary>
        /// Change brush state by index value [0 = None, 1 = Raise, 2 = Lower, 3 = Revert]
        /// </summary>
        public void SS_Funct_ChangeBrushState(int StateIndex)
        {
            SS_State = (SS_State_Internal)StateIndex;
        }

        /// <summary>
        /// Change basic sculpting values in one method
        /// </summary>
        public void SS_Funct_SetBasics(float Radius, float Strength, bool showBrush, Vector3 BrushPoint, Vector3 BrushDirection)
        {
            SS_BrushSize = Radius;
            SS_BrushStrength = Strength;
            SS_BrushProjection.transform.position = BrushPoint;
            SS_BrushProjection.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, BrushDirection);
            SS_BrushProjection.transform.localScale = new Vector3(SS_BrushSize, SS_BrushSize, SS_BrushSize);
            if (SS_BrushProjection && SS_UseBrushProjection)
                    SS_BrushProjection.SetActive(showBrush);
        }

        #endregion

        private bool MD_PRIVATE_FUNCT_GetControlInput(KeyCode key)
        {
            bool final = false;

            if (!SS_MobileSupport)
            {
                final = Input.GetKey(key);
                if (SS_MultithreadingSupported)
                {
                    if (!final && Multithread != null && Multithread.IsAlive)
                        Multithread_ManualEvent.Reset();
                }
            }
            else
            {
                final = Input.touchCount > 0;
                if (SS_MultithreadingSupported)
                {
                    if (!final && Multithread != null && Multithread.IsAlive)
                        Multithread_ManualEvent.Reset();
                }
            }
            return final;
        }
    }
}