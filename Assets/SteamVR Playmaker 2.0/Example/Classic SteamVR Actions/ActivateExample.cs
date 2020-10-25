using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ActivateExample : MonoBehaviour
{

    //[SteamVR_DefaultActionSet("platformer")]
    public SteamVR_ActionSet actionSet = SteamVR_Input.GetActionSet("platformer");
    // Use this for initialization
    //public SteamVR_Action_Boolean grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");


    void Start()
    {
        actionSet.Activate();
    }
}

