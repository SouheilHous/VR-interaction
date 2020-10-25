using UnityEngine;
using System;
using Valve.VR;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR 2.0")]
    [Tooltip("Useful for getting the information on analog controllers or touchpads.")]
    public class GetActionVector2 : FsmStateAction
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

        //public SteamVR_Action_Vector2 vector2Action;
        //[ObjectType(typeof(SteamVR_Action_Vector2))]
        //public FsmObject vector2Action;

        //public SteamVR_Action_Vector2 vector2Action;
        public FsmString vector2Action;

        public setTriggerType vector2Type;

        public enum setTriggerType
        {
            getAxis,
            getAxisDelta,
            getLastAxis,
            getLastAxisDelta,
        }

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a float variable.")]
        public FsmVector2 storeVector2Result;

        private Vector2 result;

        public override void OnEnter()
        {
            if (vector2Action == null)
            {
                Debug.LogError ("Missing Vector 2 Action : " + Owner.name);
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

        public override void Reset()
        {
            storeVector2Result = null;
        }

        public override void OnUpdate()
        {
            //var vector2 = vector2Action.Value as SteamVR_Action_Vector2;

            switch (vector2Type)
            {
                case setTriggerType.getAxis:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(vector2Action.Value).GetAxis(devices);
                    break;
                case setTriggerType.getAxisDelta:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(vector2Action.Value).GetAxisDelta(devices);
                    break;
                case setTriggerType.getLastAxis:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(vector2Action.Value).GetLastAxis(devices);
                    break;
                case setTriggerType.getLastAxisDelta:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(vector2Action.Value).GetLastAxisDelta(devices);
                    break;
            }

            storeVector2Result.Value = result;
        }
    }
}
