using UnityEngine;
using System;
using Valve.VR;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR 2.0")]
    [Tooltip("Get any Singles from the controller set in binding. Useful for getting the triggers on controllers.")]
    public class GetActionSingle : FsmStateAction
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

        public FsmString singleAction;
        ////public SteamVR_Action_Single singleAction;
        //[ObjectType(typeof(SteamVR_Action_Single))]
        //public FsmObject singleAction;
        //public SteamVR_Action_Single singleAction;

        public setTriggerType singleType;

        public enum setTriggerType
        {
            getAxis,
            getAxisDelta,
            getLastAxis,
            getLastAxisDelta,
        }

        [Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
        public FsmFloat multiplier = 1;

        private float result;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a float variable.")]
        [Title("Store Float Result")]
        public FsmFloat store;

        public override void Reset()
        {
            multiplier = 1;
            store = null;
        }

        public override void OnEnter()
        {
            if (singleAction == null)
            {
                Debug.LogError("Missing Single Action : " + Owner.name);
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
            //var single = singleAction.Value as SteamVR_Action_Single;

            switch (singleType)
            {
                case setTriggerType.getAxis:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Single>(singleAction.Value).GetAxis(devices);
                    break;
                case setTriggerType.getAxisDelta:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Single>(singleAction.Value).GetAxisDelta(devices);
                    break;
                case setTriggerType.getLastAxis:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Single>(singleAction.Value).GetLastAxis(devices);
                    break;
                case setTriggerType.getLastAxisDelta:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Single>(singleAction.Value).GetLastAxisDelta(devices);
                    break;
            }

            // if variable set to none, assume multiplier is 1
            if (!multiplier.IsNone)
            {
                result *= multiplier.Value;
            }

            store.Value = result;


        }
    }
}
