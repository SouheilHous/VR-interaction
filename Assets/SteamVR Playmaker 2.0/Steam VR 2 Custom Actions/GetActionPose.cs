using UnityEngine;
using Valve.VR;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR 2.0")]
    [Tooltip("Gets all information from the action pose, including velocity, position and rotation.")]
    public class GetActionPose : FsmStateAction
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

        //public SteamVR_Action_Pose poseAction;
        public FsmString poseAction;

        //[ObjectType(typeof(SteamVR_Action_Pose))]
        //public FsmObject poseActtion;

        public FsmEnum trackedObj;

        [ActionSection("Get Velocity")]
        [Tooltip("Select Velocity type.")]
        public setVelocityType velocityType;

        [Tooltip("Get the Vector3 Velocity on any device.")]
        [UIHint(UIHint.Variable)]
        [Title("Store Vector3 Velocity")]
        public FsmVector3 storeVelocity;

        [ActionSection("Get Angular Velocity")]
        [Tooltip("Select Velocity type.")]
        public setaVelocityType angularVelocityType;

        [Tooltip("Get the Vector3 Angular Velocity on any device.")]
        [UIHint(UIHint.Variable)]
        [Title("Store Vector3 Angular")]
        public FsmVector3 storeAngularVelocity;

        private Vector3 velocity;

        public enum setaVelocityType
        {
            getAngularVelocity,
            getLastStateAngularVelocity,
        };


        public enum setVelocityType
        {
            getVelocity,
            getLastStateVelocity,
        };

        [ActionSection("Get Position")]
        [Tooltip("Select the type of position, either local or last local.")]
        public setPositionType positionType;

        public enum setPositionType
        {
            getLocalPosition,
            getLastLocalPosition,
        };

        [Tooltip("Set to True if the button is pressed.")]
        [UIHint(UIHint.Variable)]
        [Title("Store Vector3 Position")]
        public FsmVector3 storePosition;

        [ActionSection("Get Rotation")]
        [Tooltip("Select current state or last state between trigger types.")]
        public setRotationType rotationType;



        public enum setRotationType
        {
            getLocalRotation,
            getLastLocalRotation,
        };

        [Tooltip("Set to True if the button is pressed.")]
        [UIHint(UIHint.Variable)]
        [Title("Store Quaternion Rotation")]
        public FsmQuaternion storeRotation;


        public override void Reset()
        {
            storeVelocity = null;
            storeAngularVelocity = null;
            storePosition = null;
            storeRotation = null;
        }

        public override void OnEnter()
        {
            if (poseAction == null)
            {
                Debug.LogError("Missing Pose Action : " + Owner.name);
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

            DoGetVelocity();
            DoGetAngularVelocity();

            switch (positionType)
            {
                case setPositionType.getLocalPosition:
                    storePosition.Value = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetLocalPosition(devices);
                    break;
                case setPositionType.getLastLocalPosition:
                    storePosition.Value = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetLastLocalPosition(devices);
                    break;

            }
            switch (rotationType)
            {

                case setRotationType.getLocalRotation:
                    storeRotation.Value = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetLocalRotation(devices);
                    break;
                case setRotationType.getLastLocalRotation:
                    storeRotation.Value = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetLastLocalRotation(devices);
                    break;
            }
        }
        void DoGetVelocity()
        {
            switch (velocityType)
            {
                case setVelocityType.getVelocity:
                    velocity = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetVelocity(devices);
                    break;
                case setVelocityType.getLastStateVelocity:
                    velocity = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetLastVelocity(devices);
                    break;
            }

            storeVelocity.Value = velocity;
        }
        void DoGetAngularVelocity()
        {
            switch (angularVelocityType)
            {
                case setaVelocityType.getAngularVelocity:
                    velocity = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetVelocity(devices);
                    break;
                case setaVelocityType.getLastStateAngularVelocity:
                    velocity = SteamVR_Input.GetAction<SteamVR_Action_Pose>(poseAction.Value).GetLastVelocity(devices);
                    break;
            }

            storeAngularVelocity = velocity;
        }

    }
}