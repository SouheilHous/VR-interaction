using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRController : MonoBehaviour
{
    [SerializeField]
    private Valve.VR.InteractionSystem.Interactable _leftTurner = null;
    [SerializeField]
    private Valve.VR.InteractionSystem.Interactable _rightTurner = null;
    [SerializeField]
    private Valve.VR.InteractionSystem.Interactable _moveUp = null;
    [SerializeField]
    private Valve.VR.InteractionSystem.Interactable _moveDown = null;

    [SerializeField]
    private RearArron _excavator = null;

    private SteamVR_Action_Boolean _grip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "GrabGrip");

    private void Update()
    {
        if (_leftTurner.isHovering && _grip.state)
        {
            _excavator.Arrow1up();
        }
        if (_rightTurner.isHovering && _grip.state)
        {
            _excavator.Arrow1dowen();
        }

        if (_moveUp.isHovering && _grip.state)
        {
            _excavator.Arrow2up();
        }
        if (_moveDown.isHovering && _grip.state)
        {
            _excavator.Arrow2dowen();
        }
    }
}
