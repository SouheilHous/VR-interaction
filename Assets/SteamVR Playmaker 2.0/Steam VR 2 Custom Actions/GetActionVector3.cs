using UnityEngine;
using System;
using Valve.VR;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR 2.0")]
    [Tooltip("Gets any Vector3s from the controller set in binding.")]
    public class GetActionVector3 : FsmStateAction
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

        //public SteamVR_Action_Vector3 vector3Action;
        public FsmString vector3Action;


        ////public SteamVR_Action_Vector3 vector3Action;
        //[ObjectType(typeof(SteamVR_Action_Vector3))]
        //public FsmObject vector3Action;

        public setTriggerType vector3Type;


        public enum setTriggerType
        {
            getAxis,
            getAxisDelta,
            getLastAxis,
            getLastAxisDelta,
        }

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a float variable.")]
        [Title("Store Vector3 Result")]
        public FsmVector3 storeResult;

        private Vector3 result;

        public override void Reset()
        {
            storeResult = null;
        }

        public override void OnEnter()
        {
            if (vector3Action == null)
            { 
                Debug.LogError("Missing Vector 3 Action : " + Owner.name);
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
            //var vector3 = vector3Action.Value as SteamVR_Action_Vector3;

            switch (vector3Type)
            {
                case setTriggerType.getAxis:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector3>(vector3Action.Value).GetAxis(devices);
                    break;
                case setTriggerType.getAxisDelta:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector3>(vector3Action.Value).GetAxisDelta(devices);
                    break;
                case setTriggerType.getLastAxis:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector3>(vector3Action.Value).GetLastAxis(devices);
                    break;
                case setTriggerType.getLastAxisDelta:
                    result = SteamVR_Input.GetAction<SteamVR_Action_Vector3>(vector3Action.Value).GetLastAxisDelta(devices);
                    break;
            }
            storeResult.Value = result;
        }
    }
}
