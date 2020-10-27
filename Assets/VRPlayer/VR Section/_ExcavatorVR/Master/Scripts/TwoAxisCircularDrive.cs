using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class TwoAxisCircularDrive : MonoBehaviour
{
    // public LinearMapping verticalLinearMapping;
    // public LinearMapping horizontalLinearMapping;

    // private bool grabbed = true;
    // private Hand hand;
    // private GrabTypes grabbedWithType;

    // private Quaternion _delta;

    // [SerializeField]
    // float _maxXAngle = 45;

    // [SerializeField]
    // float _maxZAngle = 45;

    // [SerializeField]
    // [Range(0, 1)]
    // float _vibrationStrenght = .1f;

    // [SerializeField]
    // [Range(0, 1)]
    // float _vibrationInterval = .05f;

    // public float XPercentage;
    // public float ZPercentage;

    // float verticalOutAngle;
    // float horizontalOutAngle;

    // private void Start()
    // {
    //     grabbed = false;
    // }

    // private void HandHoverUpdate(Hand hand)
    // {
    //     if (hand == this.hand || !grabbed)
    //     {
    //         GrabTypes startingGrabType = hand.GetGrabStarting();
    //         bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;
    //         if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None)
    //         {
    //             grabbedWithType = startingGrabType;

    //             grabbed = true;
    //             this.hand = hand;

    //             var lookAt = Quaternion.LookRotation(hand.hoverSphereTransform.position - transform.position);

    //             _delta = Quaternion.Inverse(lookAt) * transform.rotation;
    //             // verticalOutAngle = _delta.eulerAngles.y;
    //             // horizontalOutAngle = _delta.eulerAngles.x;
    //             // UpdateLinearMapping();
    //         }

    //         else if (grabbedWithType != GrabTypes.None && isGrabEnding)
    //         {
    //             grabbed = false;
    //             grabbedWithType = GrabTypes.None;
    //             this.hand = null;
    //         }
    //     }
    // }

    // private void UpdateLinearMapping()
    // {
    //     verticalLinearMapping.value = Map(XPercentage, -1f, 1f, 0f, 1f);
    //     horizontalLinearMapping.value = Map(ZPercentage, -1f, 1f, 0f, 1f);
    // }

    // private void Update()
    // {
    //     verticalOutAngle = transform.localRotation.eulerAngles.x;
    //     if (verticalOutAngle > 180)
    //         verticalOutAngle -= 360;

    //     horizontalOutAngle = transform.localRotation.eulerAngles.z;
    //     if (horizontalOutAngle > 180)
    //         horizontalOutAngle -= 360;

    //     XPercentage = verticalOutAngle / _maxXAngle;
    //     ZPercentage = horizontalOutAngle / _maxZAngle;

    //     if (grabbed)
    //     {
    //         transform.rotation = Quaternion.LookRotation(hand.hoverSphereTransform.position - transform.position) * _delta;
    //         CheckMovement();
    //         UpdateLinearMapping();
    //     }
    // }

    // private float _currentXPercentage;
    // private float _currentZPercentage;
    // private void CheckMovement()
    // {
    //     _currentXPercentage = XPercentage;
    //     _currentZPercentage = ZPercentage;
    //     if (grabbed)
    //     {
    //         if (Mathf.Abs(_currentXPercentage - XPercentage) > _vibrationInterval)
    //             _currentXPercentage = XPercentage;
    //         else if (Mathf.Abs(_currentZPercentage - ZPercentage) > _vibrationInterval)
    //             _currentZPercentage = ZPercentage;
    //     }
    // }

    // private static float Map(float x, float in_min, float in_max, float out_min, float out_max)
    // {
    //     return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    // }

    public Collider childCollider = null;
    public LinearMapping verticalLinearMapping;
    public LinearMapping horizontalLinearMapping;
    public bool hoverLock = false;
    public bool limited = false;
    public Vector2 frozenDistanceMinMaxThreshold = new Vector2(0.1f, 0.2f);
    public UnityEvent onFrozenDistanceThreshold;

    [HeaderAttribute("Limited Rotation Min")]
    public float minAngle = -45.0f;
    public bool freezeOnMin = false;
    public UnityEvent onMinAngle;

    [HeaderAttribute("Limited Rotation Max")]
    public float maxAngle = 45.0f;
    public bool freezeOnMax = false;
    public UnityEvent onMaxAngle;

    public bool forceStart = false;
    public float startAngle = 0.0f;

    public bool rotateGameObject = true;

    public float outAngleX;
    public float outAngleZ;

    private Quaternion start;
    private Vector3 worldPlaneNormalX = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 localPlaneNormalX = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 worldPlaneNormalZ = new Vector3(0.0f, 0.0f, 1.0f);
    private Vector3 localPlaneNormalZ = new Vector3(0.0f, 0.0f, 1.0f);
    private Vector3 lastHandProjectedX;
    private Vector3 lastHandProjectedZ;

    private bool driving = false;

    private float minMaxAngularThresholdX = 1.0f;
    private bool frozenX = false;
    private float frozenAngleX = 0.0f;
    private Vector3 frozenHandWorldPosX = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 frozenSqDistanceMinMaxThresholdX = new Vector2(0.0f, 0.0f);

    
    private float minMaxAngularThresholdZ = 1.0f;
    private bool frozenZ = false;
    private float frozenAngleZ = 0.0f;
    private Vector3 frozenHandWorldPosZ = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 frozenSqDistanceMinMaxThresholdZ = new Vector2(0.0f, 0.0f);

    private Hand handHoverLocked = null;
    private Valve.VR.InteractionSystem.Interactable interactable;

    private void FreezeX(Hand hand)
    {
        frozenX = true;
        frozenAngleX = outAngleX;
        frozenHandWorldPosX = hand.hoverSphereTransform.position;
        frozenSqDistanceMinMaxThresholdX.x = frozenDistanceMinMaxThreshold.x * frozenDistanceMinMaxThreshold.x;
        frozenSqDistanceMinMaxThresholdX.y = frozenDistanceMinMaxThreshold.y * frozenDistanceMinMaxThreshold.y;
    }


    private void UnFreezeX()
    {
        frozenX = false;
        frozenHandWorldPosX.Set(0.0f, 0.0f, 0.0f);
    }

    
    private void FreezeZ(Hand hand)
    {
        frozenZ = true;
        frozenAngleZ = outAngleZ;
        frozenHandWorldPosZ = hand.hoverSphereTransform.position;
        frozenSqDistanceMinMaxThresholdZ.x = frozenDistanceMinMaxThreshold.x * frozenDistanceMinMaxThreshold.x;
        frozenSqDistanceMinMaxThresholdZ.y = frozenDistanceMinMaxThreshold.y * frozenDistanceMinMaxThreshold.y;
    }


    private void UnFreezeZ()
    {
        frozenX = false;
        frozenHandWorldPosX.Set(0.0f, 0.0f, 0.0f);
    }

    private void Awake()
    {
        interactable = this.GetComponent<Valve.VR.InteractionSystem.Interactable>();
    }

    //-------------------------------------------------
    private void Start()
    {
        if (childCollider == null)
        {
            childCollider = GetComponentInChildren<Collider>();
        }
        if (verticalLinearMapping == null)
        {
            verticalLinearMapping = GetComponent<LinearMapping>();
        }
        if (verticalLinearMapping == null)
        {
            verticalLinearMapping = gameObject.AddComponent<LinearMapping>();
        }
        if (horizontalLinearMapping == null)
        {
            horizontalLinearMapping = GetComponent<LinearMapping>();
        }
        if (horizontalLinearMapping == null)
        {
            horizontalLinearMapping = gameObject.AddComponent<LinearMapping>();
        }

        worldPlaneNormalX = new Vector3(1.0f, 0.0f, 0.0f);
        localPlaneNormalX = worldPlaneNormalX;
        worldPlaneNormalZ = new Vector3(0.0f, 0.0f, 1.0f);
        localPlaneNormalZ = worldPlaneNormalZ;

        if (transform.parent)
        {
            worldPlaneNormalX = transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormalX).normalized;
            worldPlaneNormalZ = transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormalZ).normalized;
        }

        if (limited)
        {
            start = Quaternion.identity;
            outAngleX = transform.localEulerAngles[0];
            outAngleZ = transform.localEulerAngles[2];
            if (forceStart)
            {
                outAngleX = Mathf.Clamp(startAngle, minAngle, maxAngle);
                outAngleZ = Mathf.Clamp(startAngle, minAngle, maxAngle);
            }
        }
        else
        {
            start = transform.rotation;
            outAngleX = 0.0f;
            outAngleZ = 0.0f;
        }
        UpdateAll();
    }


    //-------------------------------------------------
    void OnDisable()
    {
        if (handHoverLocked)
        {
            handHoverLocked.HideGrabHint();
            handHoverLocked.HoverUnlock(interactable);
            handHoverLocked = null;
        }
    }


    //-------------------------------------------------
    private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount)
    {
        if (hand != null)
        {
            int nRangeMax = (int)Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
            nCount = Mathf.Clamp(nCount, 1, 10);

            //float hapticDuration = nRangeMax * nCount;

            //hand.TriggerHapticPulse(hapticDuration, nRangeMax, flMagnitude);

            for (ushort i = 0; i < nCount; ++i)
            {
                ushort duration = (ushort)Random.Range(100, nRangeMax);
                hand.TriggerHapticPulse(duration);
                yield return new WaitForSeconds(.01f);
            }
        }
    }


    //-------------------------------------------------
    private void OnHandHoverBegin(Hand hand)
    {
        hand.ShowGrabHint();
    }


    //-------------------------------------------------
    private void OnHandHoverEnd(Hand hand)
    {
        hand.HideGrabHint();

        if (driving && hand)
        {
            //hand.TriggerHapticPulse() //todo: fix
            StartCoroutine(HapticPulses(hand, 1.0f, 10));
        }

        driving = false;
        handHoverLocked = null;
    }

    private GrabTypes grabbedWithType;
    //-------------------------------------------------
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

        if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None)
        {
            grabbedWithType = startingGrabType;
            // Trigger was just pressed
            lastHandProjectedX = ComputeToTransformProjectedX(hand.hoverSphereTransform);
            lastHandProjectedZ = ComputeToTransformProjectedZ(hand.hoverSphereTransform);

            if (hoverLock)
            {
                hand.HoverLock(interactable);
                handHoverLocked = hand;
            }

            driving = true;

            ComputeAngle(hand);
            UpdateAll();

            hand.HideGrabHint();
        }
        else if (grabbedWithType != GrabTypes.None && isGrabEnding)
        {
            // Trigger was just released
            if (hoverLock)
            {
                hand.HoverUnlock(interactable);
                handHoverLocked = null;
            }

            driving = false;
            grabbedWithType = GrabTypes.None;
        }

        if (driving && isGrabEnding == false && hand.hoveringInteractable == this.interactable)
        {
            ComputeAngle(hand);
            UpdateAll();
        }
    }

    private Vector3 ComputeToTransformProjectedX(Transform xForm)
    {
        Vector3 toTransform = (xForm.position - transform.position).normalized;
        Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

        // Need a non-zero distance from the hand to the center of the CircularDrive
        if (toTransform.sqrMagnitude > 0.0f)
        {
            toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormalX).normalized;
        }
        else
        {
            Debug.LogFormat("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString());
            Debug.Assert(false, string.Format("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString()));
        }

        return toTransformProjected;
    }


    private Vector3 ComputeToTransformProjectedZ(Transform xForm)
    {
        Vector3 toTransform = (xForm.position - transform.position).normalized;
        Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

        // Need a non-zero distance from the hand to the center of the CircularDrive
        if (toTransform.sqrMagnitude > 0.0f)
        {
            toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormalZ).normalized;
        }
        else
        {
            Debug.LogFormat("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString());
            Debug.Assert(false, string.Format("<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString()));
        }

        return toTransformProjected;
    }

    private void UpdateLinearMapping()
    {
        if (limited)
        {
            // Map it to a [0, 1] value
            verticalLinearMapping.value = (outAngleX - minAngle) / (maxAngle - minAngle);
            horizontalLinearMapping.value = (outAngleZ - minAngle) / (maxAngle - minAngle);
        }
        else
        {
            // Normalize to [0, 1] based on 360 degree windings
            float flTmp = outAngleX / 360.0f;
            verticalLinearMapping.value = flTmp - Mathf.Floor(flTmp);
            float flTmp1 = outAngleZ / 360.0f;
            horizontalLinearMapping.value = flTmp1 - Mathf.Floor(flTmp1);
        }
    }

    private void UpdateGameObject()
    {
        if (rotateGameObject)
        {
            transform.localRotation = start * Quaternion.AngleAxis(outAngleX, localPlaneNormalX) * Quaternion.AngleAxis(outAngleZ, localPlaneNormalZ);
        }
    }

    //-------------------------------------------------
    // Updates the Debug TextMesh with the linear mapping value and the angle
    //-------------------------------------------------
    private void UpdateAll()
    {
        UpdateLinearMapping();
        UpdateGameObject();
    }

    private void ComputeAngle(Hand hand)
    {
        ComputeAngleX(hand);
        ComputeAngleZ(hand);
    }

    private void ComputeAngleX(Hand hand)
    {
        Vector3 toHandProjected = ComputeToTransformProjectedX(hand.hoverSphereTransform);

        if (!toHandProjected.Equals(lastHandProjectedX))
        {
            float absAngleDelta = Vector3.Angle(lastHandProjectedX, toHandProjected);

            if (absAngleDelta > 0.0f)
            {
                if (frozenX)
                {
                    float frozenSqDist = (hand.hoverSphereTransform.position - frozenHandWorldPosX).sqrMagnitude;
                    if (frozenSqDist > frozenSqDistanceMinMaxThresholdX.x)
                    {
                        outAngleX = frozenAngleX + Random.Range(-1.0f, 1.0f);

                        float magnitude = Util.RemapNumberClamped(frozenSqDist, frozenSqDistanceMinMaxThresholdX.x, frozenSqDistanceMinMaxThresholdX.y, 0.0f, 1.0f);
                        if (magnitude > 0)
                        {
                            StartCoroutine(HapticPulses(hand, magnitude, 10));
                        }
                        else
                        {
                            StartCoroutine(HapticPulses(hand, 0.5f, 10));
                        }

                        if (frozenSqDist >= frozenSqDistanceMinMaxThresholdX.y)
                        {
                            onFrozenDistanceThreshold.Invoke();
                        }
                    }
                }
                else
                {
                    Vector3 cross = Vector3.Cross(lastHandProjectedX, toHandProjected).normalized;
                    float dot = Vector3.Dot(worldPlaneNormalX, cross);

                    float signedAngleDelta = absAngleDelta;

                    if (dot < 0.0f)
                    {
                        signedAngleDelta = -signedAngleDelta;
                    }

                    if (limited)
                    {
                        float angleTmp = Mathf.Clamp(outAngleX + signedAngleDelta, minAngle, maxAngle);

                        if (outAngleX == minAngle)
                        {
                            if (angleTmp > minAngle && absAngleDelta < minMaxAngularThresholdX)
                            {
                                outAngleX = angleTmp;
                                lastHandProjectedX = toHandProjected;
                            }
                        }
                        else if (outAngleX == maxAngle)
                        {
                            if (angleTmp < maxAngle && absAngleDelta < minMaxAngularThresholdX)
                            {
                                outAngleX = angleTmp;
                                lastHandProjectedX = toHandProjected;
                            }
                        }
                        else if (angleTmp == minAngle)
                        {
                            outAngleX = angleTmp;
                            lastHandProjectedX = toHandProjected;
                            onMinAngle.Invoke();
                            if (freezeOnMin)
                            {
                                FreezeX(hand);
                            }
                        }
                        else if (angleTmp == maxAngle)
                        {
                            outAngleX = angleTmp;
                            lastHandProjectedX = toHandProjected;
                            onMaxAngle.Invoke();
                            if (freezeOnMax)
                            {
                                FreezeX(hand);
                            }
                        }
                        else
                        {
                            outAngleX = angleTmp;
                            lastHandProjectedX = toHandProjected;
                        }
                    }
                    else
                    {
                        outAngleX += signedAngleDelta;
                        lastHandProjectedX = toHandProjected;
                    }
                }
            }
        }
    }

    private void ComputeAngleZ(Hand hand)
    {
        Vector3 toHandProjected = ComputeToTransformProjectedX(hand.hoverSphereTransform);

        if (!toHandProjected.Equals(lastHandProjectedZ))
        {
            float absAngleDelta = Vector3.Angle(lastHandProjectedZ, toHandProjected);

            if (absAngleDelta > 0.0f)
            {
                if (frozenZ)
                {
                    float frozenSqDist = (hand.hoverSphereTransform.position - frozenHandWorldPosZ).sqrMagnitude;
                    if (frozenSqDist > frozenSqDistanceMinMaxThresholdZ.x)
                    {
                        outAngleZ = frozenAngleZ + Random.Range(-1.0f, 1.0f);

                        float magnitude = Util.RemapNumberClamped(frozenSqDist, frozenSqDistanceMinMaxThresholdZ.x, frozenSqDistanceMinMaxThresholdZ.y, 0.0f, 1.0f);
                        if (magnitude > 0)
                        {
                            StartCoroutine(HapticPulses(hand, magnitude, 10));
                        }
                        else
                        {
                            StartCoroutine(HapticPulses(hand, 0.5f, 10));
                        }

                        if (frozenSqDist >= frozenSqDistanceMinMaxThresholdZ.y)
                        {
                            onFrozenDistanceThreshold.Invoke();
                        }
                    }
                }
                else
                {
                    Vector3 cross = Vector3.Cross(lastHandProjectedZ, toHandProjected).normalized;
                    float dot = Vector3.Dot(worldPlaneNormalZ, cross);

                    float signedAngleDelta = absAngleDelta;

                    if (dot < 0.0f)
                    {
                        signedAngleDelta = -signedAngleDelta;
                    }

                    if (limited)
                    {
                        float angleTmp = Mathf.Clamp(outAngleZ + signedAngleDelta, minAngle, maxAngle);

                        if (outAngleZ == minAngle)
                        {
                            if (angleTmp > minAngle && absAngleDelta < minMaxAngularThresholdZ)
                            {
                                outAngleZ = angleTmp;
                                lastHandProjectedZ = toHandProjected;
                            }
                        }
                        else if (outAngleZ == maxAngle)
                        {
                            if (angleTmp < maxAngle && absAngleDelta < minMaxAngularThresholdZ)
                            {
                                outAngleZ = angleTmp;
                                lastHandProjectedZ = toHandProjected;
                            }
                        }
                        else if (angleTmp == minAngle)
                        {
                            outAngleZ = angleTmp;
                            lastHandProjectedZ = toHandProjected;
                            onMinAngle.Invoke();
                            if (freezeOnMin)
                            {
                                FreezeZ(hand);
                            }
                        }
                        else if (angleTmp == maxAngle)
                        {
                            outAngleZ = angleTmp;
                            lastHandProjectedZ = toHandProjected;
                            onMaxAngle.Invoke();
                            if (freezeOnMax)
                            {
                                FreezeZ(hand);
                            }
                        }
                        else
                        {
                            outAngleZ = angleTmp;
                            lastHandProjectedZ = toHandProjected;
                        }
                    }
                    else
                    {
                        outAngleZ += signedAngleDelta;
                        lastHandProjectedZ = toHandProjected;
                    }
                }
            }
        }
    }
}
