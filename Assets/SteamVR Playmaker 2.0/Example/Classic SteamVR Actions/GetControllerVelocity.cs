using UnityEngine;
using Valve.VR;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("Get the controller Velocity.")]
    public class GetControllerVelocity : FsmStateAction
    {
        public SteamVR_Input_Sources device;
        [Tooltip("Select Velocity type.")]
        public setVelocityType velocityType;

        private SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;

        [UIHint(UIHint.Variable)]
        public FsmFloat x;

        [UIHint(UIHint.Variable)]
        public FsmFloat y;

        [UIHint(UIHint.Variable)]
        public FsmFloat z;

        private Vector3 velocity;

        public enum setVelocityType
        {
            getVelocity,
            getLastStateVelocity,
        };

        public override void Reset()
        {
            vector = null;
            x = null;
            y = null;
            z = null;
        }

        public override void OnUpdate()
        {
            DoGetVelocity();

        }
        void DoGetVelocity()
        {
            switch (velocityType)
            {
                case setVelocityType.getVelocity:
                    velocity = poseAction.GetVelocity(device);
                    break;
                case setVelocityType.getLastStateVelocity:
                    velocity = poseAction.GetLastVelocity(device);
                    break;
            }


            vector.Value = velocity;
            x.Value = velocity.x;
            y.Value = velocity.y;
            z.Value = velocity.z;
        }
    }
}