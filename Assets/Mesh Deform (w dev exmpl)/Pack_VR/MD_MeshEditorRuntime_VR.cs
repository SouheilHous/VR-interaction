using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Mesh Editor Runtime VR")]
    public class MD_MeshEditorRuntime_VR : MonoBehaviour {

        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Collection): Mesh Editor Runtime VR = Component for VR CONTROLLER object in VR Mode
        //---Edit any mesh in game at runtime in VR by included functions and variables
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public enum _VertexControlMode { MoveVertex, PushVertex, PullVertex, SculptVertex};
        public _VertexControlMode ppVertexControlMode = _VertexControlMode.MoveVertex;

        public SteamVR_Input_Sources pInput_Device;

        public bool ppTargetController = false;
        public Transform TargetController;

        //---Move Vertex
        public ParticleSystem ppHoldingEffect;
        public bool ppMakeInterativePoint = false;

        //---Push-Pull
        public float ppEffectRate = 0.01f;
        private float currRate = 0;
        public bool ppEnableRigidbodyAfterHit = false;

        //---Sculpt-Vertex
        public enum pp_RuntimeFunctions_Internal { UseRaiseOnly, UseRaiseLowerOnly, UseRaiseLowerRevertOnly };
        public pp_RuntimeFunctions_Internal pp_SculptingRuntimeFunctions;

        public SteamVR_Action_Boolean ppInput_Sculpt_Raise;
        public SteamVR_Action_Boolean ppInput_Sculpt_Lower;
        public SteamVR_Action_Boolean ppInput_Sculpt_Revert;

        public float ppSculpt_Radius = 0.3f;
        public float ppSculpt_Strength = 0.5f;
        public bool UseDebugSizeBySculptingRadius = false;

        //-----GLOBAL
        public float ppVertexSpeed = 20;
        public bool ppRaycastSpecificPoints = false;
        public string ppSpecialTag;

        public SteamVR_Action_Boolean pInput_Action;

        public bool ppPointRay = true;
        public float ppSphericalRayRadius = 0.5f;

        //--Debug
        public bool ppShowGraphic = false;
        private GameObject pp_Graphic_Sphere;
        private LineRenderer pp_Graphic_Point;
        public float ppGraphic_Size = 0.2f;

        private void Start()
        {
            if (!ppTargetController)
                TargetController = this.transform;

            if (ppHoldingEffect != null && CurrentHoldingEffect == null)
                CurrentHoldingEffect = Instantiate(ppHoldingEffect);

            if (ppShowGraphic)
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

        private void Update()
        {
            Ray ray;
            RaycastHit hit;
            bool @Raycast;

            ray = new Ray(transform.position, transform.forward);

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
                MeshRuntimeVR_MoveVertex(ray, hit, @Raycast);
            else if (ppVertexControlMode == _VertexControlMode.PushVertex)
                MeshRuntimeVR_PushVertex(ray, hit, @Raycast);
            else if (ppVertexControlMode == _VertexControlMode.PullVertex)
                MeshRuntimeVR_PullVertex(ray, hit, @Raycast);
            else if (ppVertexControlMode == _VertexControlMode.SculptVertex)
                MeshRuntimeVR_SculptVertex(ray, hit, @Raycast);

            if (ppShowGraphic)
            {
                if (@Raycast)
                {
                    pp_Graphic_Point.enabled = true;
                    pp_Graphic_Point.SetPosition(0, transform.position);
                    pp_Graphic_Point.SetPosition(1, hit.point);
                    if(UseDebugSizeBySculptingRadius && ppVertexControlMode == _VertexControlMode.SculptVertex)
                    {
                        pp_Graphic_Sphere.transform.localScale = new Vector3(ppSculpt_Radius, ppSculpt_Radius, ppSculpt_Radius);
                        pp_Graphic_Point.startWidth = ppSculpt_Radius;
                        pp_Graphic_Point.endWidth = ppSculpt_Radius;
                    }
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

        /// <summary>
        /// Switch current control mode by index
        /// </summary>
        public void MeshRuntimeVR_SwitchControlMode(int index)
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

        /// <summary>
        /// Move with vertices by custom raycast
        /// </summary>
        private void MeshRuntimeVR_MoveVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            if (SelectedPoint == null)
            {
                if (@Raycast && GetControls(0))
                {
                    if (hit.collider != null && hit.collider.GetComponent<MD_MeshProEditor>())
                        return;
                    if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                    {
                        SelectedPoint = hit.transform.gameObject;
                        storedLayer = SelectedPoint.layer;
                        storedparent = SelectedPoint.transform.parent;
                    }
                }
            }

            if (SelectedPoint && GetControls(0))
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

                if (SelectedPoint.transform.parent != this.transform)
                    SelectedPoint.transform.parent = transform;
            }
            else if (SelectedPoint && GetControls(1))
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
        private void MeshRuntimeVR_PushVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            if (@Raycast && GetControls(0))
            {
                if (hit.collider != null && hit.collider.GetComponent<MD_MeshProEditor>())
                    return;
                if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                {
                    if (currRate > ppEffectRate)
                    {
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

        /// <summary>
        /// Pull vertices by custom raycast
        /// </summary>
        private void MeshRuntimeVR_PullVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            if (@Raycast && GetControls(0))
            {
                if (hit.collider != null && hit.collider.GetComponent<MD_MeshProEditor>())
                    return;
                if (hit.transform.root.GetComponent<MD_MeshProEditor>())
                {
                    if (currRate > ppEffectRate)
                    {
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
        private void MeshRuntimeVR_SculptVertex(Ray ray, RaycastHit hit, bool @Raycast)
        {
            bool allowed = false;
            if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseOnly)
                allowed = GetControls(2);
            else if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseLowerOnly)
                allowed = GetControls(2) || GetControls(3);
            else if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseLowerRevertOnly)
                allowed = GetControls(2) || GetControls(3) || GetControls(4);

            if ((GetControls(5) || GetControls(6) || GetControls(7)) && LastHit)
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
                        if(GetControls(2))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Raise);
                        else if (GetControls(3))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Lower);
                    }
                    else if (pp_SculptingRuntimeFunctions == pp_RuntimeFunctions_Internal.UseRaiseLowerRevertOnly)
                    {
                        if (GetControls(2))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Raise);
                        else if (GetControls(3))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Lower);
                        else if (GetControls(4))
                            sc.SS_Funct_DoSculpting(hit.point, transform.forward, ppSculpt_Radius, ppSculpt_Strength, MDM_SculptingPro.SS_State_Internal.Revert);
                    }
                    UpdatedCollider = true;
                }
            }
        }

        private bool GetControls(int controlIndex)
        {
            switch(controlIndex)
            {
                case 0:
                    return pInput_Action.GetState(pInput_Device);
                case 1:
                    return pInput_Action.GetStateUp(pInput_Device);

                case 2:
                    return ppInput_Sculpt_Raise.GetState(pInput_Device);
                case 3:
                    return ppInput_Sculpt_Lower.GetState(pInput_Device);
                case 4:
                    return ppInput_Sculpt_Revert.GetState(pInput_Device);
                case 5:
                    return ppInput_Sculpt_Raise.GetStateUp(pInput_Device);
                case 6:
                    return ppInput_Sculpt_Lower.GetStateUp(pInput_Device);
                case 7:
                    return ppInput_Sculpt_Revert.GetStateUp(pInput_Device);

                default:
                    return false;
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
        /// Add value to brush radius by float value
        /// </summary>
        public void SS_Funct_AddRadius(float AddValue)
        {
            ppSculpt_Radius += AddValue;
        }
        /// <summary>
        /// Sbustract value from brush radius by float value
        /// </summary>
        public void SS_Funct_SubstractRadius(float SbusValue)
        {
            ppSculpt_Radius -= SbusValue;
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
        /// <summary>
        /// Add value to brush strength by float value
        /// </summary>
        public void SS_Funct_AddStrength(float AddValue)
        {
            ppSculpt_Strength += AddValue;
        }
        /// <summary>
        /// Sbustract value from brush strength by float value
        /// </summary>
        public void SS_Funct_SubstractStrength(float SbusValue)
        {
            ppSculpt_Strength -= SbusValue;
        }

        #endregion
    }
}