using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD_Plugin;

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Mesh Editor Runtime")]
    public class MD_MeshEditorRuntime : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Collection): Mesh Editor Runtime = Component for any object
        //---Edit any mesh at runtime by included functions and variables
        //---Mobile devices are supported!
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        //---Editor Type
        public bool ppAXIS_EDITOR = false;

        public enum _VertexControlMode {MoveVertex, PushVertex, PullVertex, SculptVertex };
        public _VertexControlMode ppVertexControlMode = _VertexControlMode.MoveVertex;

        //---Enable Script for Mobile Devices
        public bool ppMobileSuppored = false;

        //---Move Vertex
        public ParticleSystem ppHoldingEffect;
        public bool ppMakeInterativePoint = false;
        public bool ppAttachToSender = true;

        public bool ppLockAxis_X = false; //----Axes to lock
        public bool ppLockAxis_Y = false;
        public bool ppLockAxis_Z = false;

        //---Push-Pull
        public float ppEffectRate = 0.01f;
        private float currRate = 0;
        public bool ppEnableRigidbodyAfterHit = false;

        //---Sculpt-Vertex
        public enum pp_RuntimeFunctions_Internal { UseRaiseOnly, UseRaiseLowerOnly, UseRaiseLowerRevertOnly };
        public pp_RuntimeFunctions_Internal pp_SculptingRuntimeFunctions;
        public KeyCode ppInput_Sculpt_Raise = KeyCode.Mouse0;
        public KeyCode ppInput_Sculpt_Lower = KeyCode.Mouse1;
        public KeyCode ppInput_Sculpt_Revert = KeyCode.Mouse2;
        public float ppSculpt_Radius = 0.3f;
        public float ppSculpt_Strength = 0.5f;

        //-----GLOBAL
        public float ppVertexSpeed = 20;
        public bool ppRaycastSpecificPoints = false;
        public string ppSpecialTag;

        public KeyCode ppInput = KeyCode.Mouse0;

        public bool ppCursorIsOrigin = true;
        public bool ppLockAndHideCursor = true;

        public bool ppPointRay = true;
        public float ppSphericalRayRadius = 0.5f;

        //--Debug
        public bool ppShowGraphic = false;
        private GameObject pp_Graphic_Sphere;
        private LineRenderer pp_Graphic_Point;
        public float ppGraphic_Size = 0.2f;


        //-----Axis Editor Settings
        public GameObject ppAXIS_AxisObject;

        public MD_MeshProEditor ppAXIS_TargetObject;

        public KeyCode ppAXIS_SelectionInput = KeyCode.Mouse0;
        public KeyCode ppAXIS_AddPointsInput = KeyCode.LeftShift;
        public KeyCode ppAXIS_RemovePointsInput = KeyCode.LeftAlt;

        public bool ppAXIS_LocalSpace = false;
        public float ppAXIS_Speed = 2;
        private Color ppAXIS_StoragePointColor;
        public Color ppAXIS_SelectedPointColor = Color.green;
        public Color ppAXIS_SelectionGridColor = Color.black;

        private bool ppAXIS_Selecting = false;
        private bool ppAXIS_Moving = false;
        public enum ppAXIS_MovingTo_ { X, Y,Z};
        public ppAXIS_MovingTo_ ppAXIS_MovingTo;
        private Vector3 ppAXIS_CursorPosOrigin;
        public List<Transform> ppAXIS_TotalPoints = new List<Transform>();
        public List<Transform> ppAXIS_SelectedPoints = new List<Transform>();
        private GameObject ppAXIS_GroupSelector;
        public List<Transform> ppAXIS_UndoStoredObjects = new List<Transform>();

        void Start () 
        {
            if (ppAXIS_EDITOR)
            {
                if(ppAXIS_TargetObject == null)
                {
                    MD_Debug.Debug(this, "Target object is empty! Script was disabled.", MD_Debug.DebugType.Error);
                    this.enabled = false;
                    return;
                }
                ppAXIS_AxisObject.SetActive(false);
                AXIS_SwitchTarget(ppAXIS_TargetObject);
                return;
            }

            if (ppHoldingEffect != null && CurrentHoldingEffect == null)
                CurrentHoldingEffect = (ParticleSystem)Instantiate(ppHoldingEffect);

            if(ppShowGraphic)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.SetFloat("_Mode", 2);
                Color c = Color.red;
                c.a = 0.2f;
                mat.SetColor("_Color", c);

                pp_Graphic_Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(pp_Graphic_Sphere.GetComponent<Collider>());
                pp_Graphic_Sphere.transform.localScale = new Vector3(ppGraphic_Size, ppGraphic_Size, ppGraphic_Size);
                pp_Graphic_Sphere.GetComponent<Renderer>().material = mat;

                GameObject newgm = new GameObject("LinePoint");
                pp_Graphic_Point = newgm.AddComponent<LineRenderer>();
                pp_Graphic_Point.material = mat;
                pp_Graphic_Point.startWidth = ppGraphic_Size;
                pp_Graphic_Point.endWidth = ppGraphic_Size;
            }
        }

        Camera cam;
        void Update () 
        {
            if (!ppAXIS_EDITOR)
                InternalProcess_NonAxisEditor();
            else
                InternalProcess_AxisEditor();
        }


        private void InternalProcess_NonAxisEditor()
        {
            if (!ppCursorIsOrigin && ppLockAndHideCursor && !ppMobileSuppored)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Ray ray = new Ray();
            RaycastHit hit = new RaycastHit();

            if (ppVertexControlMode == _VertexControlMode.SculptVertex)
                ppMobileSuppored = false;

            if (!ppMobileSuppored)
            {
                if (Camera.main != null)
                    cam = Camera.main;
                else if (GetComponent<Camera>())
                    cam = GetComponent<Camera>();
                else
                {
                    Debug.LogError("Camera component is missing! Please set your camera tag to MainCamera or add Mesh Editor Runtime to camera component.");
                    return;
                }

                if (ppCursorIsOrigin)
                    ray = cam.ScreenPointToRay(Input.mousePosition);
                else
                    ray = new Ray(transform.position, transform.forward);
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (Camera.main != null)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        cam = Camera.main;
                    }
                    else if (GetComponent<Camera>())
                    {
                        ray = GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
                        cam = GetComponent<Camera>();
                    }
                    else
                    {
                        Debug.LogError("Camera component is missing! Please set your camera tag to MainCamera or add Mesh Editor Runtime to camera component.");
                        return;
                    }
                }
            }

            bool @Raycast = false;
            if (ppPointRay)
                @Raycast = Physics.Raycast(ray, out hit);
            else
                @Raycast = Physics.SphereCast(ray, ppSphericalRayRadius, out hit);

            if (ppRaycastSpecificPoints)
            {
                if (@Raycast && hit.collider != null && hit.collider.tag != ppSpecialTag && SelectedPoint == null)
                    return;
            }
            if (ppVertexControlMode == _VertexControlMode.MoveVertex)
                NON_AXIS_MoveVertex(ray, hit, @Raycast);
            else if (ppVertexControlMode == _VertexControlMode.PushVertex)
                NON_AXIS_PushVertex(ray, hit, @Raycast);
            else if (ppVertexControlMode == _VertexControlMode.PullVertex)
                NON_AXIS_PullVertex(ray, hit, @Raycast);
            else if (ppVertexControlMode == _VertexControlMode.SculptVertex)
                NON_AXIS_SculptVertex(ray, hit, @Raycast);

            if (ppShowGraphic)
            {
                if (@Raycast)
                {
                    pp_Graphic_Point.enabled = true;
                    pp_Graphic_Point.SetPosition(0, transform.position);
                    pp_Graphic_Point.SetPosition(1, hit.point);
                }
                else
                    pp_Graphic_Point.enabled = false;

                if (!@Raycast)
                {
                    pp_Graphic_Sphere.SetActive(false);
                    return;
                }
                pp_Graphic_Sphere.SetActive(true);
                pp_Graphic_Sphere.transform.position = hit.point;
            }
        }

        private void InternalProcess_AxisEditor()
        {
            if (Camera.main != null)
                cam = Camera.main;
            else if (GetComponent<Camera>())
                cam = GetComponent<Camera>();
            else
            {
                Debug.LogError("Camera component is missing! Please set your camera tag to MainCamera or add Mesh Editor Runtime to camera component.");
                return;
            }

            //---BEFORE SELECTION
            if (Input.GetKeyDown(ppAXIS_SelectionInput))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                if(Physics.Raycast(ray,out hit))
                {
                    bool hitt = true;
                    switch(hit.collider.name)
                    {
                        case "AXIS_X":
                            ppAXIS_Moving = true;
                            ppAXIS_MovingTo = ppAXIS_MovingTo_.X;
                            return;
                        case "AXIS_Y":
                            ppAXIS_Moving = true;
                            ppAXIS_MovingTo = ppAXIS_MovingTo_.Y;
                            return;
                        case "AXIS_Z":
                            ppAXIS_Moving = true;
                            ppAXIS_MovingTo = ppAXIS_MovingTo_.Z;
                            return;
                        default:
                            hitt = false;
                            break;
                    }
                    if(!hitt)
                    {
                        if(hit.collider.transform.parent != null)
                        {
                            if (InternalAxis_CheckSideFunctions() && ppAXIS_SelectedPoints.Count > 0)
                            {
                                if (Input.GetKey(ppAXIS_AddPointsInput) && hit.collider.transform.parent == ppAXIS_TargetObject.ppVerticesRoot.transform)
                                {
                                    hit.collider.gameObject.GetComponentInChildren<Renderer>().material.color = ppAXIS_SelectedPointColor;
                                    hit.collider.gameObject.transform.parent = ppAXIS_GroupSelector.transform;
                                    ppAXIS_SelectedPoints.Add(hit.collider.gameObject.transform);
                                    ppAXIS_Moving = true;
                                    ppAXIS_AxisObject.SetActive(true);

                                    InternalAxis_RefreshBounds();
                                    return;
                                }
                                else if (Input.GetKey(ppAXIS_RemovePointsInput) && hit.collider.transform.parent == ppAXIS_GroupSelector.transform)
                                {
                                    hit.collider.gameObject.GetComponentInChildren<Renderer>().material.color = ppAXIS_StoragePointColor;
                                    hit.collider.gameObject.transform.parent = ppAXIS_TargetObject.ppVerticesRoot;
                                    ppAXIS_SelectedPoints.Remove(hit.collider.gameObject.transform);
                                    ppAXIS_Moving = true;
                                    ppAXIS_AxisObject.SetActive(true);

                                    InternalAxis_RefreshBounds();
                                    return;
                                }
                            }
                            else if (ppAXIS_SelectedPoints.Count == 0 && hit.collider.transform.parent == ppAXIS_TargetObject.ppVerticesRoot.transform)
                            {
                                ppAXIS_SelectedPoints.Add(hit.collider.gameObject.transform);

                                ppAXIS_Moving = true;
                                ppAXIS_AxisObject.SetActive(true);
                                ppAXIS_GroupSelector.transform.position = hit.collider.transform.position;

                                ppAXIS_StoragePointColor = hit.collider.transform.GetComponentInChildren<Renderer>().material.color;
                                hit.collider.gameObject.GetComponentInChildren<Renderer>().material.color = ppAXIS_SelectedPointColor;

                                InternalAxis_RefreshBounds(hit.collider.gameObject.transform);

                                ppAXIS_UndoStoredObjects.Clear();
                                ppAXIS_UndoStoredObjects.Add(ppAXIS_SelectedPoints[0]);
                                return;
                            }
                        }
                    }
                }

                ppAXIS_Selecting = true;
                ppAXIS_CursorPosOrigin = Input.mousePosition;
                if (!InternalAxis_CheckSideFunctions())
                {
                    if (ppAXIS_SelectedPoints.Count > 0)
                    {
                        ppAXIS_UndoStoredObjects.Clear();
                        foreach (Transform t in ppAXIS_SelectedPoints)
                        {
                            t.GetComponentInChildren<Renderer>().material.color = ppAXIS_StoragePointColor;
                            t.transform.parent = ppAXIS_TargetObject.ppVerticesRoot.transform;
                            ppAXIS_UndoStoredObjects.Add(t);
                        }
                    }
                    ppAXIS_AxisObject.SetActive(false);
                    ppAXIS_SelectedPoints.Clear();
                }
            }

            if(ppAXIS_Moving)
            {
                if (ppAXIS_MovingTo == ppAXIS_MovingTo_.X)
                {
                    float PosFix = 1;
                    if (cam.transform.position.z > ppAXIS_AxisObject.transform.position.z)
                        PosFix *= -1;
                    ppAXIS_GroupSelector.transform.position += ppAXIS_GroupSelector.transform.right * (Input.GetAxis("Mouse X") * PosFix) * ppAXIS_Speed * Time.deltaTime;
                }
                if (ppAXIS_MovingTo == ppAXIS_MovingTo_.Y)
                    ppAXIS_GroupSelector.transform.position += ppAXIS_GroupSelector.transform.up * Input.GetAxis("Mouse Y") * ppAXIS_Speed * Time.deltaTime;
                if (ppAXIS_MovingTo == ppAXIS_MovingTo_.Z)
                {
                    float PosFix = 1;
                    if (cam.transform.position.x < ppAXIS_AxisObject.transform.position.x)
                        PosFix *= -1;
                    ppAXIS_GroupSelector.transform.position += ppAXIS_GroupSelector.transform.forward * (Input.GetAxis("Mouse X")* PosFix) * ppAXIS_Speed * Time.deltaTime;
                }

                ppAXIS_AxisObject.transform.position = ppAXIS_GroupSelector.transform.position;
            }

            //---AFTER SELECTION
            if (Input.GetKeyUp(ppAXIS_SelectionInput))
            {
                if(ppAXIS_Moving)
                {
                    ppAXIS_Moving = false;
                    return;
                }

                if (ppAXIS_TotalPoints.Count == 0)
                    return;

                int c = 0;
                foreach (Transform t in ppAXIS_TotalPoints)
                {
                    if (t == null)
                        continue;
                    if (AxisEditor_Utilities.IsInsideSelection(cam, ppAXIS_CursorPosOrigin, t.gameObject))
                    {
                        if (!Input.GetKey(ppAXIS_RemovePointsInput))
                        {
                            if (c == 0)
                                ppAXIS_StoragePointColor = t.GetComponentInChildren<Renderer>().material.color;
                            ppAXIS_SelectedPoints.Add(t);
                            t.GetComponentInChildren<Renderer>().material.color = ppAXIS_SelectedPointColor;
                        }
                        else
                        {
                            t.GetComponentInChildren<Renderer>().material.color = ppAXIS_StoragePointColor;
                            t.transform.parent = ppAXIS_TargetObject.ppVerticesRoot;
                            ppAXIS_SelectedPoints.Remove(t);
                            continue;
                        }
                        c++;
                    }
                }
                ppAXIS_Selecting = false;
                if (ppAXIS_SelectedPoints.Count>0)
                {
                    ppAXIS_AxisObject.SetActive(true);

                    InternalAxis_RefreshBounds();
                }
                else
                    ppAXIS_AxisObject.SetActive(false);
            }
        }


        #region AXIS Methods

        private bool InternalAxis_CheckSideFunctions()
        {
            return (Input.GetKey(ppAXIS_AddPointsInput) || Input.GetKey(ppAXIS_RemovePointsInput));
        }

        private void InternalAxis_RefreshBounds(Transform center = null)
        {
            if (InternalAxis_CheckSideFunctions())
            {
                foreach (Transform p in ppAXIS_SelectedPoints)
                    p.parent = null;
            }

            Vector3 Center = AxisEditor_Utilities.FindCenterPoint(ppAXIS_SelectedPoints.ToArray());
            ppAXIS_GroupSelector.transform.position = Center;

            if (!ppAXIS_LocalSpace)
                ppAXIS_GroupSelector.transform.rotation = Quaternion.identity;
            else
            {
                if (!center)
                    ppAXIS_GroupSelector.transform.rotation = ppAXIS_TargetObject.ppVerticesRoot.transform.rotation;
                else
                    ppAXIS_GroupSelector.transform.rotation = center.rotation;
            }

            foreach (Transform p in ppAXIS_SelectedPoints)
                p.parent = ppAXIS_GroupSelector.transform;

            ppAXIS_AxisObject.transform.position = ppAXIS_GroupSelector.transform.position;
            ppAXIS_AxisObject.transform.rotation = ppAXIS_GroupSelector.transform.rotation;
        }

        void OnGUI()
        {
            if (!ppAXIS_EDITOR)
                return;

            if (ppAXIS_Selecting)
            {
                var rect = AxisEditor_Utilities.GetScreenRect(ppAXIS_CursorPosOrigin, Input.mousePosition);
                AxisEditor_Utilities.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f), ppAXIS_SelectionGridColor);
                AxisEditor_Utilities.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f), ppAXIS_SelectionGridColor);
            }
        }

        /// <summary>
        /// Axis method - switch editor target
        /// </summary>
        /// <param name="Target"></param>
        public void AXIS_SwitchTarget(MD_MeshProEditor Target)
        {
            ppAXIS_TargetObject = Target;

            ppAXIS_UndoStoredObjects.Clear();
            ppAXIS_TotalPoints.Clear();
            ppAXIS_SelectedPoints.Clear();

            if (!ppAXIS_TargetObject)
            {
                MD_Debug.Debug(this, "Target Object is missing!", MD_Debug.DebugType.Error);
                return;
            }
            if (!ppAXIS_TargetObject.ppVerticesRoot)
            {
                MD_Debug.Debug(this, "Target Objects vertices root is missing!", MD_Debug.DebugType.Error);
                return;
            }
            if (!ppAXIS_GroupSelector)
                ppAXIS_GroupSelector = new GameObject("AxisEditor_GroupSelector");
            foreach (Transform t in ppAXIS_TargetObject.ppVerticesRoot.transform)
                ppAXIS_TotalPoints.Add(t);
        }

        /// <summary>
        /// Axis method - undo selection
        /// </summary>
        public void AXIS_Undo()
        {
            if (ppAXIS_UndoStoredObjects.Count == 0)
                return;

            if (ppAXIS_SelectedPoints.Count > 0)
                foreach (Transform t in ppAXIS_SelectedPoints)
                {
                    t.GetComponentInChildren<Renderer>().material.color = ppAXIS_StoragePointColor;
                    t.transform.parent = ppAXIS_TargetObject.ppVerticesRoot.transform;
                }

            foreach (Transform t in ppAXIS_UndoStoredObjects)
            {
                if (t != null)
                    ppAXIS_SelectedPoints.Add(t);
            }
            ppAXIS_UndoStoredObjects.Clear();

            ppAXIS_Selecting = false;
            ppAXIS_AxisObject.SetActive(true);

            Vector3 Center = AxisEditor_Utilities.FindCenterPoint(ppAXIS_SelectedPoints.ToArray());
            ppAXIS_GroupSelector.transform.position = Center;
            if (ppAXIS_LocalSpace && ppAXIS_SelectedPoints.Count == 1)
                ppAXIS_GroupSelector.transform.rotation = ppAXIS_SelectedPoints[0].rotation;
            else
                ppAXIS_GroupSelector.transform.rotation = Quaternion.identity;
            ppAXIS_StoragePointColor = ppAXIS_SelectedPoints[0].GetComponentInChildren<Renderer>().material.color;
            foreach (Transform p in ppAXIS_SelectedPoints)
            {
                p.parent = ppAXIS_GroupSelector.transform;
                p.GetComponentInChildren<Renderer>().material.color = ppAXIS_SelectedPointColor;
            }

            ppAXIS_AxisObject.transform.position = ppAXIS_GroupSelector.transform.position;
            ppAXIS_AxisObject.transform.rotation = ppAXIS_GroupSelector.transform.rotation;
        }

        private static class AxisEditor_Utilities
        {
            //---Creating Grid Texture
            static Texture2D GridTexture;
            public static Texture2D GridColor(Color tex)
            {
                    if (GridTexture == null)
                    {
                        GridTexture = new Texture2D(1, 1);
                        GridTexture.SetPixel(0, 0, tex);
                        GridTexture.Apply();
                    }
                    return GridTexture;
            }
            //---Drawing Grid Borders
            public static void DrawScreenRectBorder(Rect re, float thic, Color c, Color mainC)
            {
                DrawScreenRect(new Rect(re.xMin, re.yMin, re.width, thic), c, mainC);
                DrawScreenRect(new Rect(re.xMin, re.yMin, thic, re.height), c, mainC);
                DrawScreenRect(new Rect(re.xMax - thic, re.yMin, thic, re.height), c, mainC);
                DrawScreenRect(new Rect(re.xMin, re.yMax - thic, re.width, thic), c, mainC);
            }
            public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
            {
                screenPosition1.y = Screen.height - screenPosition1.y;
                screenPosition2.y = Screen.height - screenPosition2.y;
                var topLeft = Vector3.Min(screenPosition1, screenPosition2);
                var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
                return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
            }
            //---Drawing Screen Rect
            public static void DrawScreenRect(Rect rect, Color color, Color mainCol)
            {
                GUI.color = color;
                GUI.DrawTexture(rect, GridColor(mainCol));
                GUI.color = Color.white;
            }
            //---Generating Bounds
            public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
            {
                Vector3 v1 = camera.ScreenToViewportPoint(screenPosition1);
                Vector3 v2 = camera.ScreenToViewportPoint(screenPosition2);
                Vector3 min = Vector3.Min(v1, v2);
                Vector3 max = Vector3.Max(v1, v2);
                min.z = camera.nearClipPlane;
                max.z = camera.farClipPlane;

                Bounds bounds = new Bounds();
                bounds.SetMinMax(min, max);
                return bounds;
            }
            //---Checking Selection
            public static bool IsInsideSelection(Camera camSender, Vector3 MousePos, GameObject ObjectInsideSelection)
            {
                Camera camera = camSender;
                Bounds viewportBounds = GetViewportBounds(camera, MousePos, Input.mousePosition);
                return viewportBounds.Contains(camera.WorldToViewportPoint(ObjectInsideSelection.transform.position));
            }
            //---Find Center In List
            public static Vector3 FindCenterPoint(Transform[] Senders)
            {
                if (Senders.Length == 0)
                    return Vector3.zero;
                if (Senders.Length == 1)
                    return Senders[0].position;
                Bounds bounds = new Bounds(Senders[0].position, Vector3.zero);
                for (int i = 1; i < Senders.Length; i++)
                    bounds.Encapsulate(Senders[i].position);
                return bounds.center;
            }
        }

        #endregion

        #region NON_AXIS Methods

        /// <summary>
        /// Switch current control mode by index [1-Move,2-Push,3-Pull,4-Sculpt]
        /// </summary>
        public void NON_AXIS_SwitchControlMode(int index)
        {
            if (index == 0)
                ppVertexControlMode = _VertexControlMode.MoveVertex;
            if (index == 1)
                ppVertexControlMode = _VertexControlMode.PushVertex;
            if (index == 2)
                ppVertexControlMode = _VertexControlMode.PullVertex;
            if (index == 3)
                ppVertexControlMode = _VertexControlMode.SculptVertex;
        }


        GameObject SelectedPoint;
        ParticleSystem CurrentHoldingEffect;
        int storedLayer = 0;
        Transform storedparent;
        float HitDistance = 0;

        /// <summary>
        /// Move with vertices by custom raycast
        /// </summary>
        public void NON_AXIS_MoveVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            if (SelectedPoint == null)
            {
                if (@Raycast && Internal_GetControlInput())
                {
                    if (hit.collider != null && hit.collider.GetComponent<MD_MeshProEditor>())
                        return;
                    if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                    {
                        SelectedPoint = hit.transform.gameObject;
                        HitDistance = cam.WorldToScreenPoint(SelectedPoint.transform.position).z;
                        storedLayer = SelectedPoint.layer;
                        storedparent = SelectedPoint.transform.parent;
                        SelectedPoint.transform.parent = null;
                    }
                }
            }

            if (SelectedPoint && Internal_GetControlInput(false))
            {
                if (CurrentHoldingEffect != null)
                {
                    CurrentHoldingEffect.transform.position = SelectedPoint.transform.position;
                    if (!CurrentHoldingEffect.isPlaying)
                        CurrentHoldingEffect.Play();
                }

                if (ppMakeInterativePoint)
                {
                    SelectedPoint.layer = 2;

                    if (Raycast)
                    {
                        SelectedPoint.transform.position = hit.point;
                        return;
                    }
                }
                if(ppAttachToSender && !ppCursorIsOrigin)
                {
                    if (SelectedPoint.transform.parent != this.transform)
                        SelectedPoint.transform.parent = transform;
                    return;
                }
                else if (ppAttachToSender && ppCursorIsOrigin)
                {
                    Vector3 p = Input.mousePosition;
                    p.z = HitDistance;
                    p = cam.ScreenToWorldPoint(p);
                    Vector3 SelPoint_Old = SelectedPoint.transform.position;
                    Vector3 SelPoint_New = p;
                    if (ppLockAxis_X)
                        SelPoint_New.x = SelPoint_Old.x;
                    if (ppLockAxis_Y)
                        SelPoint_New.y = SelPoint_Old.y;
                    if (ppLockAxis_Z)
                        SelPoint_New.z = SelPoint_Old.z;
                        SelectedPoint.transform.position = SelPoint_New;
                    return;
                }

                SelectedPoint.transform.LookAt(transform.position);
                SelectedPoint.transform.position += SelectedPoint.transform.right * Input.GetAxis("Mouse X") * Time.deltaTime * ppVertexSpeed * -1;
                SelectedPoint.transform.position += SelectedPoint.transform.up * Input.GetAxis("Mouse Y") * Time.deltaTime * ppVertexSpeed;
            }
            else if (SelectedPoint && !Internal_GetControlInput(false))
            {
                if (CurrentHoldingEffect != null)
                    CurrentHoldingEffect.Stop();
                SelectedPoint.layer = storedLayer;
                SelectedPoint.transform.parent = storedparent;
                SelectedPoint = null;
            }
        }

        /// <summary>
        /// Push vertices by custom raycast
        /// </summary>
        public void NON_AXIS_PushVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            if (@Raycast && Internal_GetControlInput(false))
            {
                if (hit.collider != null && hit.collider.GetComponent<MD_MeshProEditor>())
                    return;
                if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                {
                    if (currRate > ppEffectRate)
                    {
                        if (ppCursorIsOrigin)
                        {
                            hit.transform.LookAt(transform.position);
                            hit.transform.position += -hit.transform.forward * Time.deltaTime * ppVertexSpeed;
                        }
                        else
                            hit.transform.position += this.transform.forward * Time.deltaTime * ppVertexSpeed;

                        currRate = 0;

                        if (ppEnableRigidbodyAfterHit)
                        {
                            if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                            {
                                hit.transform.position += this.transform.forward * Time.deltaTime * ppVertexSpeed;
                                hit.transform.root.GetComponent<MD_MeshProEditor>().MD_INTERNAL_FUNCT_UpdateMeshState();
                                 hit.transform.root.GetComponent<MD_MeshProEditor>().ppAnimationMode = false;
                                 if (!hit.transform.root.GetComponent<MD_MeshColliderRefresher>())
                                     hit.transform.root.gameObject.AddComponent<MD_MeshColliderRefresher>().Convex_MeshCollider = true;
                                 else
                                     hit.transform.root.gameObject.GetComponent<MD_MeshColliderRefresher>().Convex_MeshCollider = true;
                                RigidTarget = hit.transform.root.gameObject;
                                Invoke("AddRigidbodyToHitObject", 0.05f);
                                return;
                            }
                        }
                    }
                    currRate += Time.deltaTime;
                }
            }
        }
        GameObject RigidTarget;
        private void AddRigidbodyToHitObject()
        {
            if (RigidTarget == null)
                return;

            RigidTarget.transform.root.GetComponent<MD_MeshProEditor>().MeshEditor_ClearVerticeEditor();
            if (!RigidTarget.transform.root.gameObject.GetComponent<Rigidbody>())
                RigidTarget.transform.root.gameObject.AddComponent<Rigidbody>().AddForce(transform.forward * 100);
            else
                RigidTarget.transform.root.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 100);
        }

        /// <summary>
        /// Pull vertices by custom raycast
        /// </summary>
        public void NON_AXIS_PullVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            if (@Raycast && Internal_GetControlInput(false))
            {
                if (hit.collider != null && hit.collider.GetComponent<MD_MeshProEditor>())
                    return;
                if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                {
                    if (currRate > ppEffectRate)
                    {
                        if (ppCursorIsOrigin)
                        {
                            hit.transform.LookAt(transform.position);
                            hit.transform.position += hit.transform.forward * Time.deltaTime * ppVertexSpeed;
                        }
                        else
                            hit.transform.position += -this.transform.forward * Time.deltaTime * ppVertexSpeed;
                        currRate = 0;
                        if (ppEnableRigidbodyAfterHit)
                        {
                            if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                            {
                                hit.transform.position += this.transform.forward * Time.deltaTime * ppVertexSpeed;
                                hit.transform.root.GetComponent<MD_MeshProEditor>().MD_INTERNAL_FUNCT_UpdateMeshState();
                                hit.transform.root.GetComponent<MD_MeshProEditor>().ppAnimationMode = false;
                                if (!hit.transform.root.GetComponent<MD_MeshColliderRefresher>())
                                    hit.transform.root.gameObject.AddComponent<MD_MeshColliderRefresher>().Convex_MeshCollider = true;
                                else
                                    hit.transform.root.gameObject.GetComponent<MD_MeshColliderRefresher>().Convex_MeshCollider = true;
                                hit.transform.root.gameObject.AddComponent<Rigidbody>().AddForce(transform.forward * 100);
                                hit.transform.root.GetComponent<MD_MeshProEditor>().MeshEditor_ClearVerticeEditor();
                                return;
                            }
                        }
                    }
                    currRate += Time.deltaTime;
                }
            }
        }

        private MDM_SculptingPro LastHit;
        private bool UpdatedCollider;
        /// <summary>
        /// Sculpt vertices by custom raycast
        /// </summary>
        private void NON_AXIS_SculptVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            bool allowed = false;
            if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseOnly)
                allowed = Input.GetKey(ppInput_Sculpt_Raise);
            else if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseLowerOnly)
                allowed = Input.GetKey(ppInput_Sculpt_Raise) || Input.GetKey(ppInput_Sculpt_Lower);
            else if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseLowerRevertOnly)
                allowed = Input.GetKey(ppInput_Sculpt_Raise) || Input.GetKey(ppInput_Sculpt_Lower) || Input.GetKey(ppInput_Sculpt_Revert);


            if((Input.GetKeyUp(ppInput_Sculpt_Raise) || Input.GetKeyUp(ppInput_Sculpt_Lower) || Input.GetKeyUp(ppInput_Sculpt_Revert)) && LastHit)
            {
                if (UpdatedCollider)
                    LastHit.SS_Funct_RefreshMeshCollider();
            }



            if (@Raycast)
            {
                if (hit.collider != null && hit.collider.GetComponent<MDM_SculptingPro>())
                {
                    if (!LastHit)
                        LastHit = hit.collider.GetComponent<MDM_SculptingPro>();
                    else if (LastHit != hit.collider.GetComponent<MDM_SculptingPro>())
                    {
                        LastHit.SS_Funct_SetBasics(ppSculpt_Radius, ppSculpt_Strength, false, hit.point, hit.normal);
                        if (UpdatedCollider)
                            LastHit.SS_Funct_RefreshMeshCollider();
                        UpdatedCollider = false;
                        LastHit = null;
                    }
                    hit.collider.GetComponent<MDM_SculptingPro>().SS_Funct_SetBasics(ppSculpt_Radius, ppSculpt_Strength, true, hit.point, hit.normal);
                }
                else if (LastHit)
                {
                    LastHit.SS_Funct_SetBasics(ppSculpt_Radius, ppSculpt_Strength, false, hit.point, hit.normal);
                    if (UpdatedCollider)
                        LastHit.SS_Funct_RefreshMeshCollider();
                    UpdatedCollider = false;
                    LastHit = null;
                }
            }
            else if (LastHit)
            {
                LastHit.SS_Funct_SetBasics(ppSculpt_Radius, ppSculpt_Strength, false, hit.point, hit.normal);
                if (UpdatedCollider)
                    LastHit.SS_Funct_RefreshMeshCollider();
                UpdatedCollider = false;
                LastHit = null;
            }

            if (@Raycast && allowed)
            {
                if (hit.collider != null && hit.collider.GetComponent<MDM_SculptingPro>())
                {
                    MDM_SculptingPro sc = hit.collider.GetComponent<MDM_SculptingPro>();

                    if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseOnly)
                        sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Raise);
                    else if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseLowerOnly)
                    {
                        if (Input.GetKey(ppInput_Sculpt_Raise))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Raise);
                        else if (Input.GetKey(ppInput_Sculpt_Lower))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Lower);
                    }
                    else if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseLowerRevertOnly)
                    {
                        if (Input.GetKey(ppInput_Sculpt_Raise))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Raise);
                        else if (Input.GetKey(ppInput_Sculpt_Lower))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Lower);
                        else if (Input.GetKey(ppInput_Sculpt_Revert))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Revert);
                    }
                    UpdatedCollider = true;
                }
            }
        }

        #region Additional Sculpting Methods

        /// <summary>
        /// Change brush radius by float value
        /// </summary>
        public void SS_Funct_ChangeRadius(float size)
        {
            ppSculpt_Radius = size;
        }
        /// <summary>
        /// Change brush radius by Slider value
        /// </summary>
        /// <param name="size"></param>
        public void SS_Funct_ChangeRadius(UnityEngine.UI.Slider size)
        {
            ppSculpt_Radius = size.value;
        }

        /// <summary>
        /// Change brush strength by float value
        /// </summary>
        public void SS_Funct_ChangeStrength(float strength)
        {
            ppSculpt_Strength = strength;
        }
        /// <summary>
        /// Change brush strength by Slider value
        /// </summary>
        public void SS_Funct_ChangeStrength(UnityEngine.UI.Slider strength)
        {
            ppSculpt_Strength = strength.value;
        }

        #endregion

        #endregion

        private bool Internal_GetControlInput(bool KeyDown = true)
        {
            if(!ppMobileSuppored)
            {
                if (!KeyDown)
                    return Input.GetKey(ppInput);
                else
                    return Input.GetKeyDown(ppInput);
            }
            else return Input.touchCount > 0;
        }
    }
}