/*
Use touch when touching the actual touchpad to trigger an event, and use press
to trigger the touchpad event as a button.
*/
using UnityEngine;
using Valve.VR;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("Sends an Event when a Touchpad button is Pressed.")]
    public class GetTouchpad : FsmStateAction
    {
        private SteamVR_Action_Boolean booleanAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

        public SteamVR_Input_Sources device;

        [Tooltip("Select touch or Press for trigger type.")]
        public setTouchpadType touchpadType;

        public enum setTouchpadType
        {
            getPress,
            getPressDown,
            getPressUp,
            getTouch,
            getTouchDown,
            getTouchUp,
        };

        [Tooltip("Event to send if the button is pressed.")]
        public FsmEvent sendEvent;

        [Tooltip("Set to True if the button is pressed.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;



        public override void Reset()
        {
            sendEvent = null;
            storeResult = null;
        }

        public override void OnUpdate()
        {

                switch (touchpadType)
                {
                    case setTouchpadType.getPress:
                        var padDown = booleanAction.GetState(device);
                        if (padDown)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padDown;
                        break;
                    case setTouchpadType.getPressUp:
                        var padDownUp = booleanAction.GetStateUp(device);

                        if (padDownUp)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padDownUp;
                        break;
                    case setTouchpadType.getPressDown:
                        var padDownDown = booleanAction.GetStateDown(device);

                        if (padDownDown)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padDownDown;
                        break;
                    case setTouchpadType.getTouch:
                        var padTouch = booleanAction.GetLastState(device);

                        if (padTouch)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padTouch;
                        break;
                    case setTouchpadType.getTouchUp:
                        var padTouchUp = booleanAction.GetLastStateUp(device);

                        if (padTouchUp)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padTouchUp;
                        break;
                    case setTouchpadType.getTouchDown:
                        var padTouchDown = booleanAction.GetLastStateDown(device);

                        if (padTouchDown)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padTouchDown;
                        break;
            }
        }
    }
}