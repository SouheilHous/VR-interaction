/*
Select event for system selection.
*/
using UnityEngine;
using Valve.VR;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR 2.0")]
    [Tooltip("Activates all actions in an action set.")]
    public class ActivateActionSet : FsmStateAction
    {
        public FsmString actionSet;
        public FsmBool activate = true;

        //public SteamVR_Input_Sources forSources;
        private SteamVR_Input_Sources devices;
        public playsources forSources;

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


        public FsmBool disableAllOtherActionSets = false;

        public override void Reset()
        {
            activate = true;
            disableAllOtherActionSets = false;
        }

        public override void OnEnter()
        {
            switch (forSources)
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
        
            if (actionSet == null)
            {
                Debug.LogError("Missing Action Set : " + Owner.name);
                Finish();
            }

            if (activate.Value)
                SteamVR_Input.GetActionSet(actionSet.Value).Activate(devices, 0, disableAllOtherActionSets.Value);
            else
            {
                SteamVR_Input.GetActionSet(actionSet.Value).Deactivate(devices);
            }
        }

        
    }
}