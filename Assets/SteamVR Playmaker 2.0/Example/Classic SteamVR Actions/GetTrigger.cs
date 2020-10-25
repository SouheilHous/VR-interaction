using UnityEngine;
using Valve.VR;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("Sends an Event when a Trigger Button is pressed.")]
    public class GetTrigger : FsmStateAction
    {
        public SteamVR_Input_Sources device;

        [Tooltip("Select current state or last state between trigger types.")]
        public setTriggerType triggerType;

        private SteamVR_Action_Boolean booleanAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

        public enum setTriggerType
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
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
            switch (triggerType)
            {
                case setTriggerType.getPress:
                    var buttonDown = booleanAction.GetState(device);

                    if (buttonDown)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonDown;
                    break;
                case setTriggerType.getPressUp:
                    var buttonDownUp = booleanAction.GetStateUp(device);

                    if (buttonDownUp)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonDownUp;
                    break;
                case setTriggerType.getPressDown:
                    var buttonDownDown = booleanAction.GetStateDown(device);

                    if (buttonDownDown)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonDownDown;
                    break;
                case setTriggerType.getTouch:
                    var buttonTouch = booleanAction.GetLastState(device);

                    if (buttonTouch)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonTouch;
                    break;
                case setTriggerType.getTouchUp:
                    var buttonTouchUp = booleanAction.GetLastStateUp(device);

                    if (buttonTouchUp)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonTouchUp;
                    break;
                case setTriggerType.getTouchDown:
                    var buttonTouchDown = booleanAction.GetLastStateDown(device);

                    if (buttonTouchDown)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonTouchDown;
                    break;
            }
        }

    }
}