using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Valve.VR;

public class ScalingTool : MonoBehaviour
{
    public static ScalingTool instance;

    private bool bScaling;
    private float originalControllerDistance;
    private Vector3 originalGameObjectLocalScale;
    private Vector3 originalGameObjectPosition;

    private Transform currentTransform;
    private float distanceToCurrentTransform;

    private Transform currentTransformVRBrush;
    private Vector3 currentTransformOffsetVRBrush;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        SteamVR_Action_Boolean action = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

        Transform t = currentTransformVRBrush;
        Vector3 offset = currentTransformOffsetVRBrush;

        if (t == null) {
            t = currentTransform;
            if (currentTransform != null)
            {
                offset = currentTransform.position;
            }
            else
            {
                offset = Vector3.zero;
            }
        }

        if (t != null)
        {
            if (action.GetState(SteamVR_Input_Sources.LeftHand) && action.GetState(SteamVR_Input_Sources.RightHand))
            {
                if (!bScaling)
                {
                    originalControllerDistance = Vector3.Distance(SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose").GetLocalPosition(SteamVR_Input_Sources.LeftHand), SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose").GetLocalPosition(SteamVR_Input_Sources.RightHand));
                    originalGameObjectPosition = t.position;
                    originalGameObjectLocalScale = t.localScale;
                }
                bScaling = true;

                float currentControllerDistance = Vector3.Distance(SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose").GetLocalPosition(SteamVR_Input_Sources.LeftHand), SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose").GetLocalPosition(SteamVR_Input_Sources.RightHand));

                Vector3 newScale = originalGameObjectLocalScale * (currentControllerDistance / originalControllerDistance);
                //t.position = originalGameObjectPosition + offset * (newScale.x / originalGameObjectLocalScale.x - 1);
                t.position = (originalGameObjectPosition - offset) * newScale.x / originalGameObjectLocalScale.x  + offset;
                //t.position = offset
                t.localScale = newScale;
            }
            else
            {
                bScaling = false;
            }
        } else
        {
            bScaling = false;
        }        
    }

    public bool IsScaling()
    {
        return bScaling;
    }

    public void NotifyIntersectionTransform(Transform t)
    {
        if (!bScaling)
        {
            currentTransform = t;
        }
    }

    public void NotifyIntersectionTransformVRBrush(Transform t)
    {
        if (!bScaling)
        {
            currentTransformVRBrush = t;
        }
    }

    public void NotifyIntersectionTransformOffsetVRBrush(Vector3 offset)
    {
        if (!bScaling)
        {
            currentTransformOffsetVRBrush = offset;
        }
    }

    public void NotifyIntersectionTransformDistance(float distance)
    {
        if (!bScaling)
        {
            distanceToCurrentTransform = distance;
        }
    }
    public void NotifyIntersectionTransformNone()
    {
        if (!bScaling)
        {
            currentTransform = null;
        }
    }
    public void NotifyIntersectionTransformNoneVRBrush()
    {
        if (!bScaling)
        {
            currentTransformVRBrush = null;
        }
    }

    public bool IsHoveringOverTransform()
    {
        return currentTransform != null;
    }
}
