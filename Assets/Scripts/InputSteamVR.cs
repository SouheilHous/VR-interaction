using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputSteamVR : MonoBehaviour
{
    public static InputSteamVR instance;

    public string actionSet;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        bool bIsVRBrushActive = EditorXRVRBrushManager.instance != null && EditorXRVRBrushManager.instance.IsActive();
        bool bIsGrabPressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetState(SteamVR_Input_Sources.LeftHand) || SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetState(SteamVR_Input_Sources.RightHand);
        bool bIsUsingScalingTool = ScalingTool.instance.IsScaling();
        bool bIsUltimateRadialMenuActive = (EditorXRUltimateRadialMenu.instance != null && EditorXRUltimateRadialMenu.instance.IsActive());

        actionSet = "Normal";

        //Grab Grip (moving around)
        /*if (bIsVRBrushActive)
        {
            gameObject.SendMessage("UpdateSteamVRInput_LeftGrip_Idle");
            gameObject.SendMessage("UpdateSteamVRInput_RightGrip_Idle");

            actionSet = "VR Brush";
        }
        else*/
        {
            gameObject.SendMessage("UpdateSteamVRInput_LeftGrip", SteamVR_Input.GetAction<SteamVR_Action_Single>("Grip").GetAxis(SteamVR_Input_Sources.LeftHand));
            gameObject.SendMessage("UpdateSteamVRInput_RightGrip", SteamVR_Input.GetAction<SteamVR_Action_Single>("Grip").GetAxis(SteamVR_Input_Sources.RightHand));
        }

        //Triggers
        if (
            (bIsVRBrushActive && !bIsGrabPressed)
            //|| (!bIsVRBrushActive && bIsUsingScalingTool)
            )
        {
            gameObject.SendMessage("UpdateSteamVRInput_LeftTrigger_Idle");
            gameObject.SendMessage("UpdateSteamVRInput_RightTrigger_Idle");
        } 
        else if (ScalingTool.instance != null && !bIsVRBrushActive /*&& !ScalingTool.instance.IsHoveringOverTransform()*/)
        {
            gameObject.SendMessage("UpdateSteamVRInput_LeftTrigger", 
                Mathf.Clamp01(/*(SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetState(SteamVR_Input_Sources.LeftHand) ? 1f : 0f)
                +*/ SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(SteamVR_Input_Sources.LeftHand)
                ));
            gameObject.SendMessage("UpdateSteamVRInput_RightTrigger", 
                Mathf.Clamp01((SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetState(SteamVR_Input_Sources.RightHand) ? 1f : 0f)
                + SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(SteamVR_Input_Sources.RightHand)
                ));
        }
        else
        {
            gameObject.SendMessage("UpdateSteamVRInput_LeftTrigger", SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(SteamVR_Input_Sources.LeftHand));
            gameObject.SendMessage("UpdateSteamVRInput_RightTrigger", SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(SteamVR_Input_Sources.RightHand));
        }

        //Thumbsticks
        if (bIsVRBrushActive)
        {
            gameObject.SendMessage("UpdateSteamVRInput_LeftStick_Idle");
            gameObject.SendMessage("UpdateSteamVRInput_RightStick_Idle");
        } else
        {
            gameObject.SendMessage("UpdateSteamVRInput_LeftStick", SteamVR_Input.GetAction<SteamVR_Action_Vector2>("ThumbstickPosition").GetAxis(SteamVR_Input_Sources.LeftHand));
            gameObject.SendMessage("UpdateSteamVRInput_RightStick", SteamVR_Input.GetAction<SteamVR_Action_Vector2>("ThumbstickPosition").GetAxis(SteamVR_Input_Sources.RightHand));
        }

        gameObject.SendMessage("UpdateSteamVRInput_LeftAction1Down", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action1").GetStateDown(SteamVR_Input_Sources.LeftHand));
        gameObject.SendMessage("UpdateSteamVRInput_LeftAction1Up", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action1").GetStateUp(SteamVR_Input_Sources.LeftHand));
        gameObject.SendMessage("UpdateSteamVRInput_RightAction1Down", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action1").GetStateDown(SteamVR_Input_Sources.RightHand));
        gameObject.SendMessage("UpdateSteamVRInput_RightAction1Up", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action1").GetStateUp(SteamVR_Input_Sources.RightHand));
        gameObject.SendMessage("UpdateSteamVRInput_LeftAction2Down", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action2").GetStateDown(SteamVR_Input_Sources.LeftHand));
        gameObject.SendMessage("UpdateSteamVRInput_LeftAction2Up", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action2").GetStateUp(SteamVR_Input_Sources.LeftHand));
        gameObject.SendMessage("UpdateSteamVRInput_RightAction2Down", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action2").GetStateDown(SteamVR_Input_Sources.RightHand));
        gameObject.SendMessage("UpdateSteamVRInput_RightAction2Up", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action2").GetStateUp(SteamVR_Input_Sources.RightHand));
        gameObject.SendMessage("UpdateSteamVRInput_LeftStickButtonDown", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ThumbstickButton").GetStateDown(SteamVR_Input_Sources.LeftHand));
        gameObject.SendMessage("UpdateSteamVRInput_LeftStickButtonUp", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ThumbstickButton").GetStateUp(SteamVR_Input_Sources.LeftHand));
        gameObject.SendMessage("UpdateSteamVRInput_RightStickButtonDown", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ThumbstickButton").GetStateDown(SteamVR_Input_Sources.RightHand));
        gameObject.SendMessage("UpdateSteamVRInput_RightStickButtonUp", SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ThumbstickButton").GetStateUp(SteamVR_Input_Sources.RightHand));
    }

    public string GetActionSet()
    {
        return actionSet;
    }
}
