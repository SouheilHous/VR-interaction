using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MD_Plugin
{
    [ExecuteInEditMode]
    [AddComponentMenu(MD_Debug.ORGANISATION + "/MD Plugin/Modifiers/Landscape Tracking GPU")]
    public class MDM_LandscapeTracking : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Modifier): Landscape Tracking = Component for objects with Mesh Renderer
        //---Interactive tracking mesh surface for specific shader & material [requires Easy Mesh Tracker shader]
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public Camera LT_virtualTrackCamera;
        public RenderTexture LT_TrackerSource;

        public bool NotSet = true;

        public float LT_ViewSize = 5;
        public float LT_VirtCameraHeight = 0.2f;

        void Update()
        {
            if (!LT_virtualTrackCamera)
                return;

            LT_virtualTrackCamera.transform.localPosition = Vector3.zero + Vector3.up * LT_VirtCameraHeight;
            LT_virtualTrackCamera.transform.localRotation = Quaternion.LookRotation(Vector3.down);
            LT_virtualTrackCamera.transform.localScale = Vector3.one;

            LT_virtualTrackCamera.orthographicSize = LT_ViewSize;

            if (LT_TrackerSource != null && LT_virtualTrackCamera.targetTexture == null)
                LT_virtualTrackCamera.targetTexture = LT_TrackerSource;
        }
    }
}
