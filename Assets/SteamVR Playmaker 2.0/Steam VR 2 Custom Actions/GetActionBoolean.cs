using UnityEngine;
using Valve.VR;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR 2.0")]
    [Tooltip("Gets the current state of the boolean. Useful for any type of button.")]
    public class GetActionBoolean : FsmStateAction
    {
        private SteamVR_Input_Sources devices;
        public playsources device;

        public enum playsources
        {
            Any,
            LeftHand,
            RightHand,
            LeftFoot,
            LeftShoulder,
            RightShoulder,
            Waist,
            Chest,
            Head,
            Gamepad,
            Camera,
            Keyboard,
        }

        public FsmString booleanAction;

        [Tooltip("Select current state or last state between boolean types.")]
        public setTriggerType booleanType;

        public enum setTriggerType
        {
            getState,
            getStateDown,
            getStateUp,
            getLastState,
            getLastStateDown,
            getLastStateUp,
        };

        [Tooltip("Event to send if the button is pressed.")]
        public FsmEvent sendEvent;

        [Tooltip("Set to True if the button is pressed.")]
        [UIHint(UIHint.Variable)]
        [Title("Store Bool Result")]
        public FsmBool storeResult;

        public override void Reset()
        {
            sendEvent = null;
            storeResult = null;
        }

        public override void OnEnter()
        {
            if (booleanAction == null)
            {
                Debug.LogError("Missing Boolean Action : " + Owner.name);
                Finish();
            }

            switch (device)
            {
                case playsources.Any: devices = SteamVR_Input_Sources.Any; break;
                case playsources.Camera: devices = SteamVR_Input_Sources.Camera; break;
                case playsources.Chest: devices = SteamVR_Input_Sources.Chest; break;
                case playsources.Gamepad: devices = SteamVR_Input_Sources.Gamepad; break;
                case playsources.Head: devices = SteamVR_Input_Sources.Head; break;
                case playsources.Keyboard: devices = SteamVR_Input_Sources.Keyboard; break;
                case playsources.LeftFoot: devices = SteamVR_Input_Sources.LeftFoot; break;
                case playsources.LeftHand: devices = SteamVR_Input_Sources.LeftHand; break;
                case playsources.LeftShoulder: devices = SteamVR_Input_Sources.LeftShoulder; break;
                case playsources.RightHand: devices = SteamVR_Input_Sources.RightHand; break;
                case playsources.RightShoulder: devices = SteamVR_Input_Sources.RightShoulder; break;
                case playsources.Waist: devices = SteamVR_Input_Sources.Waist; break;
            }
        }

        public override void OnUpdate()
        {
            switch (booleanType)
            {
                case setTriggerType.getState:
                    var buttonDown = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(booleanAction.Value).GetState(devices);

                    if (buttonDown)
                    {
                        Fsm.Event(sendEvent);
                    }
                    
                    storeResult.Value = buttonDown;
                    break;
                case setTriggerType.getStateUp:
                    var buttonDownUp = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(booleanAction.Value).GetStateUp(devices);

                    if (buttonDownUp)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonDownUp;
                    break;
                case setTriggerType.getStateDown:
                    var buttonDownDown = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(booleanAction.Value).GetStateDown(devices);

                    if (buttonDownDown)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonDownDown;
                    break;
                case setTriggerType.getLastState:
                    var buttonTouch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(booleanAction.Value).GetLastState(devices);

                    if (buttonTouch)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonTouch;
                    break;
                case setTriggerType.getLastStateUp:
                    var buttonTouchUp = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(booleanAction.Value).GetLastStateUp(devices);

                    if (buttonTouchUp)
                    {
                        Fsm.Event(sendEvent);
                    }
                    storeResult.Value = buttonTouchUp;
                    break;
                case setTriggerType.getLastStateDown:
                    var buttonTouchDown = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(booleanAction.Value).GetLastStateDown(devices);

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