using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ExcavatorController : MonoBehaviour
{
    public enum RearBucketVerticalSwitch
    {
        Vertical,
        Extend,
        VerticalExtend,
        Bucket
    }
    public enum RearBucketHorizontalSwitch
    {
        Turn,
        Horizontal
    }
    public enum FrontBucketSwitch
    {
        Arm,
        Bucket
    }

    public enum RotationalAxis
    {
        XAxis,
        YAxis,
        ZAxis
    }

    public Transform liftLeft;
    public Transform liftRight;
    public Vector3 forwardPos_L;
    public Vector3 rearPos_L;
    public Vector3 forwardPos_R;
    public Vector3 rearPos_R;


    public Transform bucketHorizontalLinear;
    public Vector3 bucketHorizontalLinearForwardPos;
    public Vector3 bucketHorizontalLinearRearPos;

    public Transform bucketHorizontal;
    public float minValueBucketHorizontal;
    public float maxValueBucketHorizontal;
    private Vector3 _currentBucketHorizontalRot;
    public RotationalAxis bucketHorizontalRotAxis;

    public Transform bucketVertical;
    public float minValueBucketVertical;
    public float maxValueBucketVertical;
    private Vector3 _currentBucketVerticalRot;
    public RotationalAxis bucketVerticalRotAxis;

    public Transform bucketExtend;
    public float bucketExtendMinValue;
    public float bucketExtendMaxValue;
    private Vector3 _currentBucketExtendRot;
    public RotationalAxis bucketExtendRotAxis;

    public Transform bucketTilt;
    public float bucketTiltMinValue;
    public float bucketTiltMaxValue;
    private Vector3 _currentBucketTiltRot;
    public RotationalAxis bucketTiltRotAxis;

    public Transform bucketVerticalExtend;
    public Vector3 bucketVerticalExtendForwardPos;
    public Vector3 bucketVerticalExtendRearPos;

    float speed = 0.25f;

    [SerializeField]
    private LinearMapping _liftLeftMapping = null;
    [SerializeField]
    private LinearMapping _liftRightMapping = null;
    [SerializeField]
    private LinearMapping _bucketHorizontalMapping = null;
    [SerializeField]
    private LinearMapping _bucketVerticalMapping = null;
    [SerializeField]
    private LinearMapping _frontBucketMapping = null;

    public AudioSource audioF;

    public Transform Piston1F;
    public Transform Piston2F;
    public Transform Piston1G;
    public Transform Piston2G;
    public Transform Piston1B;
    public Transform Piston2B;
    public Transform Piston1D;
    public Transform Piston2D;
    public Transform Piston1C;
    public Transform Piston2C;
    public Transform Det1D;
    public Transform Det2D;
    public Transform Det1C;
    public Transform Det2C;


    public Transform Det;
    public Transform arm;
    public Transform plate;
    private Vector3 armRot;
    private Vector3 plateRot;
    private Vector3 DetRot;
    public float minArmAngle;
    public float maxArmAngle;
    private float minDetAngle;
    private float maxDetAngle;
    public float minPlateAngle;
    public float maxPlateAngle;
    public Transform Pis1T;
    public Transform Pis2T;
    public Transform Pis1J;
    public Transform Pis2J;
    public Transform Pis1K;
    public Transform Pis2K;
    public Transform Pis1H;
    public Transform Pis2H;
    public Transform Pis1C;
    public Transform Pis2C;

    public SteamVR_Action_Boolean switchButton = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Teleport");
    private float _switchDelay = 0.5f;
    public Valve.VR.InteractionSystem.Interactable rearBucketVerticalLever = null;
    public Valve.VR.InteractionSystem.Interactable rearBucketHorizontalLever = null;
    public Valve.VR.InteractionSystem.Interactable frontBucketLever = null;

    private RearBucketHorizontalSwitch _currentHorizontalMode = RearBucketHorizontalSwitch.Turn;
    private RearBucketVerticalSwitch _currentVerticalMode = RearBucketVerticalSwitch.Vertical;
    private FrontBucketSwitch _currentFrontMode = FrontBucketSwitch.Arm;

    private void Start()
    {
        audioF = GetComponent<AudioSource>();
    }

    private void OnSwitched(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (rearBucketHorizontalLever.isHovering && rearBucketHorizontalLever.hoveringHand.handType == fromSource && fromAction.state)
            _currentHorizontalMode = GetNextHorizontalMode();
        if (rearBucketVerticalLever.isHovering && rearBucketVerticalLever.hoveringHand.handType == fromSource && fromAction.state)
            _currentVerticalMode = GetNextVerticalMode();
        if (frontBucketLever.isHovering && frontBucketLever.hoveringHand.handType == fromSource && fromAction.state)
            _currentFrontMode = GetNextFrontMode();
    }

    private float _elapsedTime = 0f;
    private void Update2()
    {
        if (rearBucketHorizontalLever.isHovering)
        {
            if (switchButton.GetState(rearBucketHorizontalLever.hoveringHand.handType))
            {
                if (Time.time > _elapsedTime + _switchDelay)
                {
                    _currentHorizontalMode = GetNextHorizontalMode();
                    _elapsedTime = Time.time;
                }
            }
        }
        if (rearBucketVerticalLever.isHovering)
        {
            if (switchButton.GetState(rearBucketVerticalLever.hoveringHand.handType))
            {
                if (Time.time > _elapsedTime + _switchDelay)
                {
                    _currentVerticalMode = GetNextVerticalMode();
                    _elapsedTime = Time.time;
                }
            }
        }
        if (frontBucketLever.isHovering)
        {
            if (switchButton.GetState(frontBucketLever.hoveringHand.handType))
            {
                if (Time.time > _elapsedTime + _switchDelay)
                {
                    _currentFrontMode = GetNextFrontMode();
                    _elapsedTime = Time.time;
                }
            }
        }
    }

    private RearBucketHorizontalSwitch GetNextHorizontalMode()
    {
        int index = (int)_currentHorizontalMode + 1;
        if (index >= Enum.GetNames(typeof(RearBucketHorizontalSwitch)).Length)
            index = 0;
        return (RearBucketHorizontalSwitch)index;
    }

    private RearBucketVerticalSwitch GetNextVerticalMode()
    {
        int index = (int)_currentVerticalMode + 1;
        if (index >= Enum.GetNames(typeof(RearBucketVerticalSwitch)).Length)
            index = 0;
        return (RearBucketVerticalSwitch)index;
    }

    private FrontBucketSwitch GetNextFrontMode()
    {
        int index = (int)_currentFrontMode + 1;
        if (index >= Enum.GetNames(typeof(FrontBucketSwitch)).Length)
            index = 0;
        return (FrontBucketSwitch)index;
    }

    private void FixedUpdate2()
    {
        MoveLiftLeft(_liftLeftMapping.value);
        MoveLiftRight(_liftRightMapping.value);
        MoveBucketHorizontal(_bucketHorizontalMapping.value);
        MoveBucketVertical(_bucketVerticalMapping.value);
        MoveFrontBucket(_frontBucketMapping);
    }

    private void MoveFrontBucket(LinearMapping frontBucketMapping)
    {
        switch (_currentFrontMode)
        {
            case FrontBucketSwitch.Arm:
                armRot.x = Map(frontBucketMapping.value, 0f, 1f, minArmAngle, maxArmAngle);
                plateRot.x = Map(frontBucketMapping.value, 0f, 1f, minPlateAngle, maxPlateAngle);
                DetRot.x = Map(frontBucketMapping.value, 0f, 1f, minDetAngle, maxDetAngle);
                arm.localEulerAngles = armRot;
                plate.localEulerAngles = plateRot;
                Det.localEulerAngles = DetRot;
                break;
            case FrontBucketSwitch.Bucket:
                plateRot.x = Map(frontBucketMapping.value, 0f, 1f, minPlateAngle, maxPlateAngle);
                plate.localEulerAngles = plateRot;
                break;
        }
    }

    public void FrontLoaderBucket(float value)
    {
        plateRot.x = Map(value, 60f, -60f, minPlateAngle, maxPlateAngle);
        plate.localEulerAngles = plateRot;
    }

    public void FrontLoaderArm(float value)
    {
        // Debug.Log(value);
        // var angle = Map(value, 1f, -1f, minArmAngle, maxArmAngle);
        // if (angle > armRot.x)
        //     armRot.x += angle;
        // else
        //     armRot.x -= angle;
        // Debug.Log(String.Format("{0} {1}", armRot.x, angle));
        // plateRot.x = Map(value, 60f, -60f, minPlateAngle, maxPlateAngle);

        armRot.x = Map(value, 60f, -60f, minArmAngle, maxArmAngle);
        DetRot.x = Map(value, 60f, -60f, minDetAngle, maxDetAngle);
        arm.localEulerAngles = armRot;
        // plate.localEulerAngles = plateRot;
        Det.localEulerAngles = DetRot;
    }

    private void LateUpdate()
    {
        if (Piston1F != null && Piston2F != null)
        {
            Piston1F.LookAt(Piston2F.position, Piston1F.up);
            Piston2F.LookAt(Piston1F.position, Piston2F.up);
        }
        if (Piston1G != null && Piston2G != null)
        {
            Piston1G.LookAt(Piston2G.position, Piston1G.up);
            Piston2G.LookAt(Piston1G.position, Piston2G.up);
        }
        if (Piston1B != null && Piston2B != null)
        {
            Piston1B.LookAt(Piston2B.position, Piston1B.up);
            Piston2B.LookAt(Piston1B.position, Piston2B.up);
        }
        if (Piston1C != null && Piston2C != null)
        {
            Piston1C.LookAt(Piston2C.position, Piston1C.up);
            Piston2C.LookAt(Piston1C.position, Piston2C.up);
        }
        if (Piston1D != null && Piston2D != null)
        {
            Piston1D.LookAt(Piston2D.position, Piston1D.up);
            Piston2D.LookAt(Piston1D.position, Piston2D.up);
        }
        if (Det1D != null && Det2D != null)
        {
            Det1D.LookAt(Det2D.position, Det1D.up);
            Det2D.LookAt(Det1D.position, Det2D.up);
        }
        if (Det1C != null && Det2C != null)
        {
            Det1C.LookAt(Det2C.position, Det1C.up);
            Det2C.LookAt(Det1C.position, Det2C.up);
        }



        if (Pis1T != null && Pis2T != null)
        {
            Pis1T.LookAt(Pis2T.position, Pis1T.up);
            Pis2T.LookAt(Pis1T.position, Pis2T.up);
        }
        if (Pis1J != null && Pis2J != null)
        {
            Pis1J.LookAt(Pis2J.position, Pis1J.up);
            Pis2J.LookAt(Pis1J.position, Pis2J.up);
        }
        if (Pis1H != null && Pis2H != null)
        {
            Pis1H.LookAt(Pis2H.position, Pis1H.up);
            Pis2H.LookAt(Pis1H.position, Pis2H.up);
        }
        if (Pis1K != null && Pis2K != null)
        {
            Pis1K.LookAt(Pis2K.position, Pis1K.up);
            Pis2K.LookAt(Pis1K.position, Pis2K.up);
        }
        if (Pis1C != null && Pis2C != null)
        {
            Pis1C.LookAt(Pis2C.position, Pis1C.up);
            Pis2C.LookAt(Pis1C.position, Pis2C.up);
        }
    }

    public void MoveLiftLeft(float value)
    {
        liftLeft.transform.localPosition = Vector3.Lerp(forwardPos_L, rearPos_L, value);
    }

    public void MoveLiftRight(float value)
    {
        liftRight.transform.localPosition = Vector3.Lerp(forwardPos_R, rearPos_R, value);
    }

    private void MoveBucketHorizontal(float value)
    {
        switch (_currentHorizontalMode)
        {
            case RearBucketHorizontalSwitch.Turn:
                switch (bucketHorizontalRotAxis)
                {
                    case RotationalAxis.XAxis:
                        _currentBucketHorizontalRot.x = Map(value, 0, 1, minValueBucketHorizontal, maxValueBucketHorizontal);
                        break;
                    case RotationalAxis.YAxis:
                        _currentBucketHorizontalRot.y = Map(value, 0, 1, minValueBucketHorizontal, maxValueBucketHorizontal);
                        break;
                    case RotationalAxis.ZAxis:
                        _currentBucketHorizontalRot.z = Map(value, 0, 1, minValueBucketHorizontal, maxValueBucketHorizontal);
                        break;
                }
                bucketHorizontal.transform.localRotation = Quaternion.Euler(_currentBucketHorizontalRot);
                break;
            case RearBucketHorizontalSwitch.Horizontal:
                bucketHorizontalLinear.transform.localPosition = Vector3.Lerp(bucketHorizontalLinearForwardPos, bucketHorizontalLinearRearPos, value);
                break;
        }
    }

    private void MoveBucketVertical(float value)
    {
        switch (_currentVerticalMode)
        {
            case RearBucketVerticalSwitch.Vertical:
                switch (bucketVerticalRotAxis)
                {
                    case RotationalAxis.XAxis:
                        _currentBucketVerticalRot.x = Map(value, 0, 1, minValueBucketVertical, maxValueBucketVertical);
                        break;
                    case RotationalAxis.YAxis:
                        _currentBucketVerticalRot.y = Map(value, 0, 1, minValueBucketVertical, maxValueBucketVertical);

                        break;
                    case RotationalAxis.ZAxis:
                        _currentBucketVerticalRot.z = Map(value, 0, 1, minValueBucketVertical, maxValueBucketVertical);
                        break;
                }
                bucketVertical.transform.localRotation = Quaternion.Euler(_currentBucketVerticalRot);
                break;
            case RearBucketVerticalSwitch.Extend:
                switch (bucketExtendRotAxis)
                {
                    case RotationalAxis.XAxis:
                        _currentBucketExtendRot.x = Map(value, 0, 1, bucketExtendMinValue, bucketExtendMaxValue);
                        break;
                    case RotationalAxis.YAxis:
                        _currentBucketExtendRot.y = Map(value, 0, 1, bucketExtendMinValue, bucketExtendMaxValue);

                        break;
                    case RotationalAxis.ZAxis:
                        _currentBucketExtendRot.z = Map(value, 0, 1, bucketExtendMinValue, bucketExtendMaxValue);
                        break;
                }
                bucketExtend.transform.localRotation = Quaternion.Euler(_currentBucketExtendRot);
                break;
            case RearBucketVerticalSwitch.Bucket:
                switch (bucketTiltRotAxis)
                {
                    case RotationalAxis.XAxis:
                        _currentBucketTiltRot.x = Map(value, 0, 1, bucketTiltMinValue, bucketTiltMaxValue);
                        break;
                    case RotationalAxis.YAxis:
                        _currentBucketTiltRot.y = Map(value, 0, 1, bucketTiltMinValue, bucketTiltMaxValue);

                        break;
                    case RotationalAxis.ZAxis:
                        _currentBucketTiltRot.z = Map(value, 0, 1, bucketTiltMinValue, bucketTiltMaxValue);
                        break;
                }
                bucketTilt.transform.localRotation = Quaternion.Euler(_currentBucketTiltRot);
                break;
            case RearBucketVerticalSwitch.VerticalExtend:
                bucketVerticalExtend.transform.localPosition = Vector3.Lerp(bucketVerticalExtendForwardPos, bucketVerticalExtendRearPos, value);
                break;
        }
    }

    public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

}
