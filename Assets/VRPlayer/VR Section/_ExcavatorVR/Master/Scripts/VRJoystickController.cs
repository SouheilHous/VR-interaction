using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRJoystickController : MonoBehaviour
{
    enum JoystickMode
    {
        FrontLoaderBucket,
        FrontLoaderArm,
        RearArron1,
        RearArron2,
        RearArron3,
        RearArron4,
        RearLiftL,
        RearLiftR,
        None
    }
    [SerializeField]
    private ExcavatorController _excavatorController = null;
    [SerializeField]
    private RearArron _rearArron = null;

    [SerializeField]
    private JoystickMode xJoystickMode;

    [SerializeField]
    private JoystickMode zJoystickMode;

    private bool isGrabbed = false;


    private float xAngle = 0;
    private float yAngle = 0;


    private SteamVR_Action_Boolean _grip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGrip");

    private void Update()
    {
        if (_excavatorController != null && _grip.state && isGrabbed)
        {
            xAngle = this.transform.localRotation.eulerAngles.x;
            if (xAngle >= 300)
            {
                xAngle = ExcavatorController.Map(xAngle, 300f, 360f, -60, 0);
            }

            yAngle = this.transform.localRotation.eulerAngles.y;
            if (yAngle >= 300)
            {
                yAngle = ExcavatorController.Map(yAngle, 300f, 360f, -60, 0);
            }

            SendCommand(xJoystickMode, xAngle);
            SendCommand(zJoystickMode, yAngle);
        }

    }

    public void Grab(){
        isGrabbed = true;
    }

    public void Release(){
         isGrabbed = false;
    }

    private void SendCommand(JoystickMode mode, float angle)
    {
        //Debug.Log(mode);
        switch (mode)
        {
            case JoystickMode.FrontLoaderArm:
                _excavatorController.FrontLoaderArm(angle);
                break;
            case JoystickMode.FrontLoaderBucket:
                _excavatorController.FrontLoaderBucket(angle);
                break;
            case JoystickMode.RearArron1:
                if (angle > 0)
                    _rearArron.Arrow1dowen();
                else
                    _rearArron.Arrow1up();
                break;
            case JoystickMode.RearArron2:
                if (angle > 0)
                    _rearArron.Arrow2dowen();
                else
                    _rearArron.Arrow2up();
                break;
            case JoystickMode.RearArron3:
                if (angle > 0)
                    _rearArron.Arrow3dowen();
                else
                    _rearArron.Arrow3up();
                break;
            case JoystickMode.RearArron4:
                if (angle > 0)
                    _rearArron.Arrow4up();
                else
                    _rearArron.Arrow4dowen();
                break;
            case JoystickMode.RearLiftL:
                if (angle > 0)
                    _rearArron.RearliftupR();
                else
                    _rearArron.RearliftdowenR();
                break;
            case JoystickMode.RearLiftR:
                if (angle > 0)
                    _rearArron.RearliftupL();
                else
                    _rearArron.RearliftdowenL();
                break;
        }
    }

}

