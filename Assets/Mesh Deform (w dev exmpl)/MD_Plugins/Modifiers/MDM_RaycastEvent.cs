using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Raycast Event")]
    public class MDM_RaycastEvent : MonoBehaviour
    {

        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Raycast Event = Component for all objects
        //---Raycast function with Event input
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public bool ppUpdateRayPerFrame = true;

        public float ppDistanceRay;
        public bool ppPointRay = true;
        public float ppSphericalRadius = 0.2f;

        public bool ppSavePerformanceByRigidbody = false;
        public Rigidbody ppTargetRigidbody;
        public float ppTargetVelocitySpeed = 0.01f;

        public bool ppRaycastWithSpecificTag = false;
        public string ppRaycastTag = "";

        public UnityEvent ppEventAfterRaycast;
        public UnityEvent ppEventAfterRaycastExit;

        public RaycastHit[] hits;

        private void OnDrawGizmosSelected()
        {
            if (ppPointRay == false)
                Gizmos.DrawWireSphere(transform.position + transform.forward * ppDistanceRay, ppSphericalRadius);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * ppDistanceRay);
        }

        private void Update()
        {
            if (ppUpdateRayPerFrame)
                RayEvent_UpdateRaycastState();
        }

        /// <summary>
        /// Get Raycast state
        /// </summary>
        public bool RayEvent_IsRaycasting()
        {
            return RaycastingState;
        }

        private bool RaycastingState = false;
        /// <summary>
        /// Update current Raycast
        /// </summary>
        public void RayEvent_UpdateRaycastState()
        {
            if (ppSavePerformanceByRigidbody)
            {
                if (ppTargetRigidbody != null && ppTargetRigidbody.velocity.magnitude <= ppTargetVelocitySpeed)
                    return;
            }
            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(transform.position, transform.forward);
            if (!Physics.Raycast(ray, out hit, ppDistanceRay))
            {
                if (RaycastingState && ppEventAfterRaycastExit!=null)
                    ppEventAfterRaycastExit.Invoke();
                RaycastingState = false;
                return;
            }
            else if (ppRaycastWithSpecificTag && hit.collider.tag != ppRaycastTag)
            {
                if (RaycastingState && ppEventAfterRaycastExit != null)
                    ppEventAfterRaycastExit.Invoke();
                RaycastingState = false;
                return;
            }

            RaycastingState = true;

            hits = new RaycastHit[0];
            if (ppPointRay)
                hits = Physics.RaycastAll(ray, ppDistanceRay);
            else
                hits = Physics.SphereCastAll(ray, ppSphericalRadius, ppDistanceRay);

            if (hits.Length > 0 && ppEventAfterRaycast != null)
                ppEventAfterRaycast.Invoke();
        }
    }
}