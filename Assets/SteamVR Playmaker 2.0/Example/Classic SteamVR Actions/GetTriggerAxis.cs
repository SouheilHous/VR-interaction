using UnityEngine;
using System;
using Valve.VR;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("GEts the float based on the trigger press.")]
    public class GetTriggerAxis : FsmStateAction
    {
        public SteamVR_Input_Sources device;

        [Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
        public FsmFloat multiplier = 1;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a float variable.")]
        public FsmFloat store;

        private SteamVR_Action_Single singleAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");

        public override void Reset()
        {
            multiplier = 1;
            store = null;
        }

        public override void OnUpdate()
        {

            var axisValue = singleAction.GetAxis(device);
            // if variable set to none, assume multiplier is 1
            if (!multiplier.IsNone)
            {
                axisValue *= multiplier.Value;
            }

            store.Value = axisValue;


        }
    }
}
