using UnityEngine;
using System;
using Valve.VR;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("Sends a vector 2 acis depending on press.")]
    public class GetTouchpadAxis : FsmStateAction
    {
        public SteamVR_Input_Sources device;

        private SteamVR_Action_Vector2 vector2Action = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("platformer", "Move");

        [Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
        public FsmFloat multiplier = 1;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Vector2 variable.")]
        public FsmVector2 storeVector;

        public override void Reset()
        {
            multiplier = 1;
            storeVector = null;
        }

        public override void OnUpdate()
        {

            var axisValue = vector2Action.GetAxis(device);

            // if variable set to none, assume multiplier is 1
            if (!multiplier.IsNone)
            {
                axisValue *= multiplier.Value;
            }

            storeVector.Value = axisValue;


        }
    }
}
