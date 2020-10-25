using UnityEngine;
using Valve.VR;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR 2.0")]
    [Tooltip("Gets all information from the action pose, including velocity, position and rotation.")]
    public class GetActionVibration : FsmStateAction
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
        //public SteamVR_Action_Vibration hapticAction;

        //[ObjectType(typeof(SteamVR_Action_Vibration))]
        //public FsmObject hapticAction;

        //public SteamVR_Action_Vibration hapticAction;
        public FsmString hapticAction;

        //public FsmFloat secondsFromNow = 0;
        public FsmFloat duration = 0;

        [HasFloatSlider(0, 320)]
        public FsmFloat frequency = .5f;
        [HasFloatSlider(0, 1)]
        public FsmFloat amplitude = 1;

        [Tooltip("Event to send once duration is complete.")]
        public FsmEvent sendEvent;

        public override void Reset()
        {
            //secondsFromNow = 1;
            duration = .1f;
            frequency = 100f;
            amplitude = .1f;
            sendEvent = null;
        }

        public override void OnEnter()
        {
            if (hapticAction == null)
            { 
                Debug.LogError("Missing haptic Action : " + Owner.name);
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

            StartCoroutine(DoStartCoroutine());


        }


        IEnumerator DoStartCoroutine()
        {

            float startTime = FsmTime.RealtimeSinceStartup;
            while (FsmTime.RealtimeSinceStartup - startTime <= duration.Value)
            {
                SteamVR_Input.GetAction<SteamVR_Action_Vibration>(hapticAction.Value).Execute(0, duration.Value, frequency.Value, amplitude.Value, devices);
                yield return null;
            }

                Fsm.Event(sendEvent);
        }
    }

}