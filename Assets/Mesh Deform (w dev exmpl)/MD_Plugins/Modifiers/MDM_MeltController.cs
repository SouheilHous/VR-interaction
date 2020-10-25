using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Melt Controller")]
    public class MDM_MeltController : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Melt Controller = Component for objects with Mesh Renderer & Melt Shader
        //---Control the Melt shader outside the script
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool ppSelfHeightValue = true;
        public Transform ppTargetHeightValue;

        public bool ppMeltBySurfaceRaycast = true;
        public Vector3 ppRaycastOriginOffset = new Vector3(0, 0, 0);
        public Vector3 ppRaycastDirection = new Vector3(0, -1, 0);
        public float ppRaycastDistance = Mathf.Infinity;
        public LayerMask ppAllowedLayerMasks = -1;

        public bool ppEnableLinearInterpolationBlend = false;
        public float ppLinearInterpolationSpeed = 0.5f;

        public bool ppShowEditorGraphic = true;

        private Material ppSelfMaterial;

        private Transform ppRealTarget;

        private void OnDrawGizmosSelected()
        {
            if (!ppShowEditorGraphic)
                return;
            if (!ppMeltBySurfaceRaycast)
                return;
            if (!ppRealTarget)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(ppRealTarget.position + ppRaycastOriginOffset, ppRealTarget.localScale.magnitude/6);
            Gizmos.DrawLine(ppRealTarget.position + ppRaycastOriginOffset, ppRealTarget.position + ppRaycastOriginOffset + ppRaycastDirection * ppRaycastDistance);
        }

        float targetValue;
        float targetLerpValue;
        float starttime;
        private void Start()
        {
            starttime = Time.time;
        }
        void Update()
        {
            if (ppSelfHeightValue)
            {
                if (Application.isPlaying)
                    ppSelfMaterial = GetComponent<Renderer>().material;
                else
                    ppSelfMaterial = GetComponent<Renderer>().sharedMaterial;
                ppRealTarget = this.transform;
            }
            else if (ppTargetHeightValue)
            {
                if (Application.isPlaying)
                    ppSelfMaterial = GetComponent<Renderer>().material;
                else
                    ppSelfMaterial = GetComponent<Renderer>().sharedMaterial;
                ppRealTarget = ppTargetHeightValue;
            }
            else
                Debug.LogError("Missing 'Target Height Value' object");

            if (!ppRealTarget)
                return;

            if (!ppMeltBySurfaceRaycast)
            {
                if (!ppEnableLinearInterpolationBlend)
                    ppSelfMaterial.SetFloat("_M_Zone", ppRealTarget.position.y);
                else
                    targetValue = ppRealTarget.position.y;
            }
            else
            {
                Ray r = new Ray(ppRealTarget.transform.position + ppRaycastOriginOffset, ppRaycastDirection);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(r, out hit, ppRaycastDistance, ppAllowedLayerMasks))
                {
                    if (hit.collider)
                    {
                        if (!ppEnableLinearInterpolationBlend)
                            ppSelfMaterial.SetFloat("_M_Zone", hit.point.y);
                        else
                            targetValue = hit.point.y;
                    }
                    else
                    {
                        targetValue = ppRealTarget.position.y - 1000;

                        if (!ppEnableLinearInterpolationBlend)
                            ppSelfMaterial.SetFloat("_M_Zone", targetValue);
                    }
                }
                else
                {
                    targetValue = ppRealTarget.position.y - 1000;

                    if (!ppEnableLinearInterpolationBlend)
                        ppSelfMaterial.SetFloat("_M_Zone", targetValue);
                }
            }

            if (ppEnableLinearInterpolationBlend)
            {
                targetLerpValue = Mathf.Lerp(targetLerpValue, targetValue, Time.deltaTime * ppLinearInterpolationSpeed);
                ppSelfMaterial.SetFloat("_M_Zone", targetLerpValue);
            }
        }
    }
}
