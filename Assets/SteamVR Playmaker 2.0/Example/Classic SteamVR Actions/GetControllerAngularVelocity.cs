using UnityEngine;
using Valve.VR;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SteamVR")]
    [Tooltip("Get the controller Angular Velocity.")]
    public class GetControllerAngularVelocity : FsmStateAction
    {
        public SteamVR_Input_Sources device;
        [Tooltip("Select Velocity type.")]
        public setAngularVelocityType velocityType;

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

        public enum setAngularVelocityType
        {
            getAngularVelocity,
            getAngularLastStateVelocity,
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
            DoGetAngularVelocity();

        }
        void DoGetAngularVelocity()
        {
            switch (velocityType)
            {
                case setAngularVelocityType.getAngularVelocity:
                    velocity = poseAction.GetAngularVelocity(device);
                    break;
                case setAngularVelocityType.getAngularLastStateVelocity:
                    velocity = poseAction.GetLastAngularVelocity(device);
                    break;
            }


            vector.Value = velocity;
            x.Value = velocity.x;
            y.Value = velocity.y;
            z.Value = velocity.z;
        }
    }
}