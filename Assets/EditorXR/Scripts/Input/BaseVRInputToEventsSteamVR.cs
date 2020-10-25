using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.EditorVR.Tools;
using UnityEditor.Experimental.EditorVR.Workspaces;
using UnityEngine;
using UnityEngine.InputNew;
using UnityEngine.XR;
using Valve;

namespace UnityEditor.Experimental.EditorVR.Input
{
    abstract class BaseVRInputToEventsSteamVR : BaseInputToEvents
    {
        protected virtual string DeviceName
        {
            get { return "Unknown VR Device"; }
        }

        const uint k_ControllerCount = 2;
        const int k_AxisCount = (int)VRInputDevice.VRControl.Analog9 + 1;
        const float k_DeadZone = 0.05f;

        List<XRNodeState> m_NodeStates = new List<XRNodeState>();

        float[,] m_LastAxisValues = new float[k_ControllerCount, k_AxisCount];
        Vector3[] m_LastPositionValues = new Vector3[k_ControllerCount];
        Quaternion[] m_LastRotationValues = new Quaternion[k_ControllerCount];
        static readonly VRInputDevice.VRControl[] k_Buttons =
        {
            VRInputDevice.VRControl.Action1,
            VRInputDevice.VRControl.Action2,
            VRInputDevice.VRControl.LeftStickButton
        };

        private float steamVRLeftTrigger;
        private float steamVRRightTrigger;
        private float steamVRLeftGrip;
        private float steamVRRightGrip;
        private Vector2 steamVRLeftStick;
        private Vector2 steamVRRightStick;
        private bool steamVRLeftAction1Down;
        private bool steamVRLeftAction1Up;
        private bool steamVRRightAction1Down;
        private bool steamVRRightAction1Up;
        private bool steamVRLeftAction2Down;
        private bool steamVRLeftAction2Up;
        private bool steamVRRightAction2Down;
        private bool steamVRRightAction2Up;
        private bool steamVRLeftStickButtonDown;
        private bool steamVRLeftStickButtonUp;
        private bool steamVRRightStickButtonDown;
        private bool steamVRRightStickButtonUp;

        public void Update()
        {
            var deviceActive = false;
            foreach (var device in UnityEngine.Input.GetJoystickNames())
            {
                if (device.IndexOf(DeviceName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    deviceActive = true;
                    break;
                }
            }

            active = deviceActive;
            if (!active)
                return;

            for (VRInputDevice.Handedness hand = VRInputDevice.Handedness.Left;
                (int)hand <= (int)VRInputDevice.Handedness.Right;
                hand++)
            {
                int deviceIndex = hand == VRInputDevice.Handedness.Left ? 3 : 4;

                // TODO change 3 and 4 based on virtual devices defined in InputDeviceManager (using actual hardware available)
                SendButtonEvents(hand, deviceIndex);
                SendAxisEvents(hand, deviceIndex);
                SendTrackingEvents(hand, deviceIndex);
            }
        }

        bool GetAxis(VRInputDevice.Handedness hand, VRInputDevice.VRControl axis, out float value)
        {
            switch (axis)
            {
                case VRInputDevice.VRControl.Trigger1:
                    if (hand == VRInputDevice.Handedness.Left)
                        value = /*UnityEngine.Input.GetAxis("XRI_Left_Trigger") + */steamVRLeftTrigger;
                    else
                        value = /*UnityEngine.Input.GetAxis("XRI_Right_Trigger") + */steamVRRightTrigger;
                    return true;
                case VRInputDevice.VRControl.Trigger2:
                    if (hand == VRInputDevice.Handedness.Left)
                        value = /*UnityEngine.Input.GetAxis("XRI_Left_Grip") + */steamVRLeftGrip;
                    else
                        value = /*UnityEngine.Input.GetAxis("XRI_Right_Grip") + */steamVRRightGrip;
                    return true;
                case VRInputDevice.VRControl.LeftStickX:
                    if (hand == VRInputDevice.Handedness.Left)
                        value = /*UnityEngine.Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal") + */steamVRLeftStick.x;
                    else
                        value = /*UnityEngine.Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal") + */steamVRRightStick.x;
                    return true;
                case VRInputDevice.VRControl.LeftStickY:
                    if (hand == VRInputDevice.Handedness.Left)
                        value = -1f * (/*UnityEngine.Input.GetAxis("XRI_Left_Primary2DAxis_Vertical")*/ - steamVRLeftStick.y);
                    else
                        value = -1f * (/*UnityEngine.Input.GetAxis("XRI_Right_Primary2DAxis_Vertical")*/ - steamVRRightStick.y);
                    return true;
            }

            value = 0f;
            return false;
        }

        void SendAxisEvents(VRInputDevice.Handedness hand, int deviceIndex)
        {
            for (var axis = 0; axis < k_AxisCount; ++axis)
            {
                float value;
                if (GetAxis(hand, (VRInputDevice.VRControl)axis, out value))
                {
                    if (Mathf.Approximately(m_LastAxisValues[(int)hand, axis], value))
                        continue;

                    if (Mathf.Abs(value) < k_DeadZone)
                        value = 0;

                    var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                    inputEvent.deviceType = typeof(VRInputDevice);
                    inputEvent.deviceIndex = deviceIndex;
                    inputEvent.controlIndex = axis;
                    inputEvent.value = value;

                    m_LastAxisValues[(int)hand, axis] = inputEvent.value;

                    InputSystem.QueueEvent(inputEvent);
                }
            }
        }

        protected virtual string GetButtonAxis(VRInputDevice.Handedness hand, VRInputDevice.VRControl button)
        {
            switch (button)
            {
                case VRInputDevice.VRControl.Action1:
                    if (hand == VRInputDevice.Handedness.Left)
                        return "XRI_Left_PrimaryButton";
                    else
                        return "XRI_Right_PrimaryButton";

                case VRInputDevice.VRControl.Action2:
                    if (hand == VRInputDevice.Handedness.Left)
                        return "XRI_Left_SecondaryButton";
                    else
                        return "XRI_Right_SecondaryButton";

                case VRInputDevice.VRControl.LeftStickButton:
                    if (hand == VRInputDevice.Handedness.Left)
                        return "XRI_Left_Primary2DAxisClick";
                    else
                        return "XRI_Right_Primary2DAxisClick";
            }

            // Not all buttons are currently mapped
            return null;
        }

        void SendButtonEvents(VRInputDevice.Handedness hand, int deviceIndex)
        {
            foreach (VRInputDevice.VRControl button in k_Buttons)
            {
                string axis = GetButtonAxis(hand, button);

                bool isDown = UnityEngine.Input.GetButtonDown(axis);
                bool isUp = UnityEngine.Input.GetButtonUp(axis);

                switch (axis)
                {
                    case "XRI_Left_PrimaryButton":
                        isDown |= steamVRLeftAction1Down;
                        isUp |= steamVRLeftAction1Up;
                        break;

                    case "XRI_Right_PrimaryButton":
                        isDown |= steamVRRightAction1Down;
                        isUp |= steamVRRightAction1Up;
                        break;

                    case "XRI_Left_SecondaryButton":
                        isDown |= steamVRLeftAction2Down;
                        isUp |= steamVRLeftAction2Up;
                        break;

                    case "XRI_Right_SecondaryButton":
                        isDown |= steamVRRightAction2Down;
                        isUp |= steamVRRightAction2Up;
                        break;

                    case "XRI_Left_Primary2DAxisClick":
                        isDown |= steamVRLeftStickButtonDown;
                        isUp |= steamVRLeftStickButtonUp;
                        break;

                    case "XRI_Right_Primary2DAxisClick":
                        isDown |= steamVRRightStickButtonDown;
                        isUp |= steamVRRightStickButtonUp;
                        break;
                }

                if (isDown || isUp)
                {
                    var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                    inputEvent.deviceType = typeof(VRInputDevice);
                    inputEvent.deviceIndex = deviceIndex;
                    inputEvent.controlIndex = (int)button;
                    inputEvent.value = isDown ? 1.0f : 0.0f;

                    InputSystem.QueueEvent(inputEvent);
                }
            }
        }

        void SendTrackingEvents(VRInputDevice.Handedness hand, int deviceIndex)
        {
#pragma warning disable 618
            XRNode node = hand == VRInputDevice.Handedness.Left ? XRNode.LeftHand : XRNode.RightHand;
            var localPosition = InputTracking.GetLocalPosition(node);
            var localRotation = InputTracking.GetLocalRotation(node);
#pragma warning restore 618

            if (localPosition == m_LastPositionValues[(int)hand] && localRotation == m_LastRotationValues[(int)hand])
                return;

            var inputEvent = InputSystem.CreateEvent<VREvent>();
            inputEvent.deviceType = typeof(VRInputDevice);
            inputEvent.deviceIndex = deviceIndex;
            inputEvent.localPosition = localPosition;
            inputEvent.localRotation = localRotation;

            m_LastPositionValues[(int)hand] = inputEvent.localPosition;
            m_LastRotationValues[(int)hand] = inputEvent.localRotation;

            InputSystem.QueueEvent(inputEvent);
        }

        public void UpdateSteamVRInput_LeftTrigger(float value)
        {
            if (value > 0f)
            {
                if (steamVRLeftTrigger == 0f)
                {
                    foreach (TransformTool t in transform.root.GetComponentsInChildren<TransformTool>())
                    {
                        t.DuplicateCurrentlySelectedObjects();
                    }

                    SendMessageUpwards("DuplicateCurrentlySelectedObjects");
                }

                foreach (WorkspaceUI t in transform.root.GetComponentsInChildren<WorkspaceUI>())
                {
                    t.LeftTriggerHeld();
                }
            } else
            {
                foreach (WorkspaceUI t in transform.root.GetComponentsInChildren<WorkspaceUI>())
                {
                    t.LeftTriggerReleased();
                }
            }

            steamVRLeftTrigger = value;
        }
        public void UpdateSteamVRInput_LeftTrigger_Idle()
        {
            foreach (WorkspaceUI t in transform.root.GetComponentsInChildren<WorkspaceUI>())
            {
                t.LeftTriggerReleased();
            }

            steamVRLeftTrigger = 0f;
        }
        public void UpdateSteamVRInput_RightTrigger(float value)
        {
            if (value > 0f)
            {
                foreach (WorkspaceUI t in transform.root.GetComponentsInChildren<WorkspaceUI>())
                {
                    t.RightTriggerHeld();
                }
            } else
            {
                foreach (WorkspaceUI t in transform.root.GetComponentsInChildren<WorkspaceUI>())
                {
                    t.RightTriggerReleased();
                }
            }

            steamVRRightTrigger = value;
        }
        public void UpdateSteamVRInput_RightTrigger_Idle()
        {
            foreach (WorkspaceUI t in transform.root.GetComponentsInChildren<WorkspaceUI>())
            {
                t.RightTriggerReleased();
            }
            steamVRRightTrigger = 0f;
        }
        public void UpdateSteamVRInput_LeftGrip(float value)
        {
            steamVRLeftGrip = value;
        }
        public void UpdateSteamVRInput_LeftGrip_Idle()
        {
            steamVRLeftGrip = 0f;
        }
        public void UpdateSteamVRInput_RightGrip(float value)
        {
            steamVRRightGrip = value;
        }
        public void UpdateSteamVRInput_RightGrip_Idle()
        {
            steamVRRightGrip = 0f;
        }

        public void UpdateSteamVRInput_LeftStick(Vector2 value)
        {
            steamVRLeftStick = value;
        }
        public void UpdateSteamVRInput_LeftStick_Idle()
        {
            steamVRLeftStick = Vector2.zero;
        }
        public void UpdateSteamVRInput_RightStick(Vector2 value)
        {
            steamVRRightStick = value;
        }
        public void UpdateSteamVRInput_RightStick_Idle()
        {
            steamVRRightStick = Vector2.zero;
        }
        public void UpdateSteamVRInput_LeftAction1Down(bool value)
        {
            steamVRLeftAction1Down = value;
        }
        public void UpdateSteamVRInput_LeftAction1Up(bool value)
        {
            steamVRLeftAction1Up = value;
        }
        public void UpdateSteamVRInput_RightAction1Down(bool value)
        {
            steamVRRightAction1Down = value;
        }
        public void UpdateSteamVRInput_RightAction1Up(bool value)
        {
            steamVRRightAction1Up = value;
        }
        public void UpdateSteamVRInput_LeftAction2Down(bool value)
        {
            steamVRLeftAction2Down = value;
        }
        public void UpdateSteamVRInput_LeftAction2Up(bool value)
        {
            steamVRLeftAction2Up = value;
        }
        public void UpdateSteamVRInput_RightAction2Down(bool value)
        {
            steamVRRightAction2Down = value;
        }
        public void UpdateSteamVRInput_RightAction2Up(bool value)
        {
            steamVRRightAction2Up = value;
        }
        public void UpdateSteamVRInput_LeftStickButtonDown(bool value)
        {
            steamVRLeftStickButtonDown = value;
        }
        public void UpdateSteamVRInput_LeftStickButtonUp(bool value)
        {
            steamVRLeftStickButtonUp = value;
        }
        public void UpdateSteamVRInput_RightStickButtonDown(bool value)
        {
            steamVRRightStickButtonDown = value;
        }
        public void UpdateSteamVRInput_RightStickButtonUp(bool value)
        {
            steamVRRightStickButtonUp = value;
        }
    }
}
