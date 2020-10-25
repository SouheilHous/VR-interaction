/*
Use touch type on the grip to have the bool store result stay true when held.
*/
using UnityEngine;
using Valve.VR;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("Sends an Event when a Grip button is Pressed.")]
    public class GetGrip : FsmStateAction
    {
        public SteamVR_Input_Sources device;

        private SteamVR_Action_Boolean booleanAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

        [Tooltip("Select touch or Press for trigger type.")]
        public setGripType gripType;

        public enum setGripType
        {
            getPress,
            getPressDown,
            getPressUp,
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
                switch (gripType)
                {
                    case setGripType.getPress:
                        var padDown = booleanAction.GetState(device);
                    if (padDown)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padDown;
                        break;
                    case setGripType.getPressUp:
                        var padDownUp = booleanAction.GetStateUp(device);

                    if (padDownUp)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padDownUp;
                        break;
                    case setGripType.getPressDown:
                        var padDownDown = booleanAction.GetStateDown(device);

                    if (padDownDown)
                        {
                            Fsm.Event(sendEvent);
                        }
                        storeResult.Value = padDownDown;
                        break;
                }
            }
        }
    }