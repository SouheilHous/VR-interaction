using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class JoystickDrive : MonoBehaviour
{
    public LinearMapping verticalLinearMapping;
    public LinearMapping horizontalLinearMapping;
    public Vector3 clampAngles = Vector3.zero;

    private bool grabbed;
    private Hand hand;
    private GrabTypes grabbedWithType;

    private Quaternion _delta;

    private float XPercentage;
    private float ZPercentage;

    private void Start()
    {
        grabbed = false;
    }

    private void HandHoverUpdate(Hand hand)
    {
        if (hand == this.hand || !grabbed)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

            if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None)
            {
                grabbedWithType = startingGrabType;

                grabbed = true;
                this.hand = hand;

                var lookAt = Quaternion.LookRotation(hand.hoverSphereTransform.position - transform.position);

                _delta = Quaternion.Inverse(lookAt) * transform.rotation;
            }

            else if (grabbedWithType != GrabTypes.None && isGrabEnding)
            {
                grabbed = false;
                grabbedWithType = GrabTypes.None;
                this.hand = null;

            }
        }

    }

    private Vector3 _rot;
    private void Update()
    {
        if (grabbed)
        {
            // _rot = (Quaternion.LookRotation(hand.hoverSphereTransform.position - transform.position) * _delta).eulerAngles;
            // transform.Rotate(_rot.x, 0f, _rot.y);
            transform.rotation = Quaternion.LookRotation(hand.hoverSphereTransform.position - transform.position) * _delta;
            // _rot = (Quaternion.LookRotation(hand.hoverSphereTransform.position - transform.position) * _delta).eulerAngles;
            // _rot.y = 0f;
            // transform.eulerAngles = _rot;

            var angleX = transform.localRotation.eulerAngles.x;
            if (angleX > 180)
                angleX -= 360;
            var angleZ = transform.localRotation.eulerAngles.z;
            if (angleZ > 180)
                angleZ -= 360;

            XPercentage = Mathf.Clamp(angleX / 90f, -1f, 1f);
            ZPercentage = Mathf.Clamp(angleZ / 90f, -1f, 1f);
            UpdateLinearMapping();
        }
    }

    private void UpdateLinearMapping()
    {
        verticalLinearMapping.value = Map(XPercentage, -1f, 1f, 0f, 1f);
        horizontalLinearMapping.value = Map(ZPercentage, -1f, 1f, 0f, 1f);
    }

    private static float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    private static Quaternion ClampRotation(Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);
        return q;
    }
}
