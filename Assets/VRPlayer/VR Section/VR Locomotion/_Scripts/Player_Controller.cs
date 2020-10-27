using JimmyGao;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Player_Controller : MonoBehaviour
{
    public GameObject BrushMenu;
    //public GameObject BrushCanvas;

    public SteamVR_Action_Boolean Toggle_VR_Brush;
    public GameObject VR_Input;
    public bool isBrowserOn = false;

    private bool toggleBrush = false;
    private int pressCount = 1;
    private int pressCount2 = 0;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShowMenu", 3f);
    }

    public void ShowMenu()
    {
        //BrushMenu.GetComponent<BrushManager>().HideBrushMenu();
    }

    // Update is called once per frame
    void Update()
    {
        //if (SteamVR_Actions._default.Toggle_ActionSet.GetStateDown(SteamVR_Input_Sources.Any) 
        //    || SteamVR_Actions.ui.Toggle_ActionSet.GetStateDown(SteamVR_Input_Sources.Any))
        //{
        //    if(pressCount2 < 2)
        //    {
        //        pressCount2++;
        //    }
        //    else
        //    {
        //        pressCount2 = 0;
        //        Toggle_ActionSet();
        //    }
            
        //}
        // print("browser ON ? = " + isBrowserOn);
        if (SteamVR_Actions._default.VR_Brush_Btn.GetStateDown(SteamVR_Input_Sources.Any) && !isBrowserOn)
        {
            print("Checking count");
            if (pressCount >= 2)
            {

                print("Clear press" + pressCount);
                toggleBrush = !toggleBrush;
                pressCount = 1;
            }
            else
            {
                print("press" + pressCount);
                pressCount++;
                
            }
        }
        //print("Brush On = " + toggleBrush);

        if (toggleBrush && !BrushMenu.GetComponent<BrushManager>().BrushMenu.activeSelf) //enable vr brush
        {
            ShowBrushMenu();

            //VR_Input.SetActive(false);
        }
        else if (!toggleBrush && BrushMenu.GetComponent<BrushManager>().BrushMenu.activeSelf) // disable vr brush
        {
            HideBrushMenu();


        }
    }

    private void ShowBrushMenu()
    {
        print("alpha bravo charlie show");
        BrushMenu.GetComponent<BrushManager>().BrushMenu.SetActive(true);
        BrushMenu.GetComponent<BrushManager>().BrushMenu2.SetActive(true);
        BrushMenu.GetComponent<BrushManager>().BrushMenu3.SetActive(true);
    }

    private void HideBrushMenu()
    {
        print("alpha bravo charlie hide");
        BrushMenu.GetComponent<BrushManager>().BrushMenu.SetActive(false);
        BrushMenu.GetComponent<BrushManager>().BrushMenu2.SetActive(false);
        BrushMenu.GetComponent<BrushManager>().BrushMenu3.SetActive(false);
    }

    private void Toggle_ActionSet()
    {
        isBrowserOn = !isBrowserOn;
        if (isBrowserOn)
        {
            SteamVR_Actions._default.Deactivate();
        }
        else
        {
            SteamVR_Actions._default.Activate();
        }
    }

    public void ToggleVRInput()
    {
        if(!isBrowserOn)
        {
            //print("on vr browser input");
            //isBrowserOn = true;
            BrushMenu.SetActive(false);
            
        }
        else
        {
            //print("off vr browser input");
            //isBrowserOn = false;
            BrushMenu.SetActive(true);
            
        }
    }

    
}
