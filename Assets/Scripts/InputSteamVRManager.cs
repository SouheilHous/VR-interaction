using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using TMPro;

[ExecuteInEditMode]
public class InputSteamVRManager : MonoBehaviour
{
    public Transform tCanvasLeft;
    public Transform tCanvasRight;
    public TextMeshProUGUI tmpActionSetLeft;
    public TextMeshProUGUI tmpActionSetRight;


    [Header("Left Images")]
    public Image imgGrabGripL;
    public Image imgGrabPinchL;
    public Image imgInteractUIL;
    public Image imgURMSelectL;
    public Image imgEditorXRGripL;
    public Image imgEditorXRTriggerL;
    public Image imgEditorXRThumbstickL;
    public Image imgEditorXRAction1L;
    public Image imgEditorXRAction2L;
    public Image imgVRBrushMenuL;
    public Image imgVRBrushSwitchL;
    public Image imgVRBrushScaleUpL;
    public Image imgVRBrushScaleDownL;
    public Image imgVRBrushUndoL;
    public Image imgVRBrushRedoL;

    [Header("Right Images")]
    public Image imgGrabGripR;
    public Image imgGrabPinchR;
    public Image imgInteractUIR;
    public Image imgURMSelectR;
    public Image imgEditorXRGripR;
    public Image imgEditorXRTriggerR;
    public Image imgEditorXRThumbstickR;
    public Image imgEditorXRAction1R;
    public Image imgEditorXRAction2R;
    public Image imgVRBrushMenuR;
    public Image imgVRBrushSwitchR;
    public Image imgVRBrushScaleUpR;
    public Image imgVRBrushScaleDownR;
    public Image imgVRBrushUndoR;
    public Image imgVRBrushRedoR;


    [Header("Colors")]
    public Color colorEnabled;
    public Color colorEnabledMax;
    public Color colorDisabled;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameObject go = null;

        if (Application.isPlaying)
        {
            if (go == null)
                go = GameObject.Find("ViveProxy(Clone)");

            if (go == null)
                go = GameObject.Find("OVRProxy(Clone)");
        } else
        {
            if (go == null)
                go = GameObject.Find("ViveProxy");

            if (go == null)
                go = GameObject.Find("OVRProxy");
        }

        if (go !=null && go.GetComponent<InputSteamVR>() == null)
        {
            go.AddComponent<InputSteamVR>();
        }


        if (InputSteamVR.instance != null)
        {
            tCanvasLeft.position = InputSteamVR.instance.transform.GetChild(0).position - Vector3.right * 0.24f;
            tCanvasLeft.rotation = InputSteamVR.instance.transform.GetChild(0).rotation;
            tCanvasLeft.LookAt(Camera.main.transform);
            tCanvasRight.position = InputSteamVR.instance.transform.GetChild(1).position + Vector3.right * 0.24f;
            tCanvasRight.rotation = InputSteamVR.instance.transform.GetChild(1).rotation;
            tCanvasRight.LookAt(Camera.main.transform);

            SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
            SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;

            imgGrabGripL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgGrabPinchL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgInteractUIL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgURMSelectL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("UltimateRadialMenuSelect").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgEditorXRGripL.color = (SteamVR_Input.GetAction<SteamVR_Action_Single>("Grip").GetAxis(leftHand) > 0f ? Color.Lerp(colorEnabled, colorEnabledMax, SteamVR_Input.GetAction<SteamVR_Action_Single>("Grip").GetAxis(leftHand)) : colorDisabled);
            imgEditorXRTriggerL.color = (SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(leftHand) > 0f ? Color.Lerp(colorEnabled, colorEnabledMax, SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(leftHand)) : colorDisabled);
            imgEditorXRThumbstickL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ThumbstickButton").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgEditorXRAction1L.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action1").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgEditorXRAction2L.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action2").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgVRBrushMenuL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("MenuButton").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgVRBrushSwitchL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SwitchMenu").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgVRBrushScaleUpL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ScaleUp").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgVRBrushScaleDownL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ScaleDown").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgVRBrushUndoL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("UndoButton").GetState(leftHand) ? colorEnabled : colorDisabled);
            imgVRBrushRedoL.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RedoButton").GetState(leftHand) ? colorEnabled : colorDisabled);


            imgGrabGripR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgGrabPinchR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgInteractUIR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgURMSelectR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("UltimateRadialMenuSelect").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgEditorXRGripR.color = (SteamVR_Input.GetAction<SteamVR_Action_Single>("Grip").GetAxis(rightHand) > 0f ? Color.Lerp(colorEnabled, colorEnabledMax, SteamVR_Input.GetAction<SteamVR_Action_Single>("Grip").GetAxis(rightHand)) : colorDisabled);
            imgEditorXRTriggerR.color = (SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(rightHand) > 0f ? Color.Lerp(colorEnabled, colorEnabledMax, SteamVR_Input.GetAction<SteamVR_Action_Single>("Trigger").GetAxis(rightHand)) : colorDisabled);
            imgEditorXRThumbstickR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ThumbstickButton").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgEditorXRAction1R.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action1").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgEditorXRAction2R.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Action2").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgVRBrushMenuR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("MenuButton").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgVRBrushSwitchR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SwitchMenu").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgVRBrushScaleUpR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ScaleUp").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgVRBrushScaleDownR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ScaleDown").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgVRBrushUndoR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("UndoButton").GetState(rightHand) ? colorEnabled : colorDisabled);
            imgVRBrushRedoR.color = (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RedoButton").GetState(rightHand) ? colorEnabled : colorDisabled);


            tmpActionSetLeft.text = "Action set:  <b>" + InputSteamVR.instance.GetActionSet() + "</b>";
            tmpActionSetRight.text = "Action set:  <b>" + InputSteamVR.instance.GetActionSet() + "</b>";
        }

    }
}
