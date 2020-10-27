using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SteeringController : MonoBehaviour
{

    [SerializeField]
    private SteeringWheel _steeringWheel = null;
    [SerializeField]
    private MovementController _movementController = null;


    private bool isGrabbed = false;


    private float zAngle = 0;


    private SteamVR_Action_Boolean _grip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGrip");

    private void Update()
    {
        if (_movementController != null && _grip.state && isGrabbed)
        {
            // zAngle = this.transform.localRotation.eulerAngles.z;
            // _movementController.Move(zAngle);
        }
    }

    public void Grab()
    {
        isGrabbed = true;
    }

    public void Release()
    {
        isGrabbed = false;
        _movementController.Stop();
    }
}

