using UnityEngine;
using Valve.VR;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("Gets all information from the action pose, including velocity, position and rotation.")]
    public class SetRumble : FsmStateAction
    {
        public SteamVR_Input_Sources device;

        //public FsmFloat secondsFromNow = 0;
        public FsmFloat duration = 0;

        [HasFloatSlider(0, 320)]
        public FsmFloat frequency = .5f;
        [HasFloatSlider(0, 1)]
        public FsmFloat strength = 1;

        private SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

        [Tooltip("Event to send once duration is complete.")]
        public FsmEvent sendEvent;

        public override void Reset()
        {
            //secondsFromNow = 1;
            duration = .1f;
            frequency = 100f;
            strength = .1f;
            sendEvent = null;
        }

        public override void OnEnter()
        {
        StartCoroutine(DoStartCoroutine());


        }


        IEnumerator DoStartCoroutine()
        {
            float startTime = FsmTime.RealtimeSinceStartup;
            while (FsmTime.RealtimeSinceStartup - startTime <= duration.Value)
            {
                hapticAction.Execute(0, duration.Value, frequency.Value, strength.Value, device);
                yield return null;
            }

                Fsm.Event(sendEvent);
        }
    }

}