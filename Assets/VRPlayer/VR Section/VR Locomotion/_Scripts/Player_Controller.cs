using JimmyGao;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Player_Controller : MonoBehaviour
{
    [Range(1,2)]
    public int isDefaultorPlatformer ;

   
    void Start()
    {
    }

    public void ShowMenu()
    {
        //BrushMenu.GetComponent<BrushManager>().HideBrushMenu();
    }

    // Update is called once per frame
    void Update()
    {
         if(SteamVR_Actions.platformer.Toggle_ActionSet.GetStateDown(SteamVR_Input_Sources.Any) )
            {
            Toggle_ActionSetDefault();
            
        }
        if (SteamVR_Actions._default.Toggle_ActionSet.GetStateDown(SteamVR_Input_Sources.Any))
        {

            Toggle_ActionSetPlatformer();
        }


    }

  
    private void Toggle_ActionSetPlatformer()
    {
        isDefaultorPlatformer = 2;
            SteamVR_Actions._default.Deactivate();
        SteamVR_Actions.platformer.Activate();

    }
    private void Toggle_ActionSetDefault()
    {
        isDefaultorPlatformer = 1;
        SteamVR_Actions._default.Activate();
        SteamVR_Actions.platformer.Deactivate();

    }





}
