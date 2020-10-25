using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace MD_Plugin
{
    [AddComponentMenu(MD_Debug.ORGANISATION+"/MD Plugin/Mesh Paint VR Input")]
    public class MD_MeshPaint_VR_Input : MonoBehaviour
    {
        //-----------------------DESCRIPTION------------------------------------------
        //----------------------------------------------------------------------------
        //---MD (Mesh Deformation Collection): Mesh Paint VR Input = Component for VR Input integration in Mesh Paint
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public MD_MeshPaint TargetMeshPaint;

        public SteamVR_Input_Sources pInput_Device;
        public SteamVR_Action_Boolean pInput_Action;

        void Update()
        {
            TargetMeshPaint.MP_TypeCustom_DRAW = pInput_Action.GetState(pInput_Device);
        }
    }
}