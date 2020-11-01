using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MovementController : MonoBehaviour
{
    public Rigidbody rg;

    public WheelCollider[] WColForward;
    public WheelCollider[] WColBack;

    public Transform[] wheelsF;
    public Transform[] wheelsB;

    public float wheelOffset = 0.1f;
    public float wheelRadius = 0.13f;

    public float maxSteer = 30;
    public float maxAccel = 25;
    public float maxBrake = 50;
    public float braking_on_the_go = 980f;
    public Transform COM;

    public class WheelData
    {
        public Transform wheelTransform;
        public WheelCollider col;
        public Vector3 wheelStartPos;
        public float rotation = 0.0f;
    }

    protected WheelData[] wheels;
    private float startPitch = 1f;
    public float minPitch = 0f;
    public float maxPitch = 0f;
    public AudioSource audioSourceCar;
    private float starSCar;

    [SerializeField]
    private SteamVR_Action_Single _throttle = SteamVR_Input.GetAction<SteamVR_Action_Single>("buggy", "throttle");
    [SerializeField]
    private SteeringWheel _steeringWheel = null;
    [SerializeField]
    private LinearMapping _steeringValue = null;
    [SerializeField]
    private Transform _player = null;
    [SerializeField]
    private Transform _playerGrabpose = null;

    private bool _isGrabbed = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSourceCar.pitch = startPitch;
        audioSourceCar.Play();
        rg.centerOfMass = COM.localPosition;
        wheels = new WheelData[WColForward.Length + WColBack.Length];
        for (int i = 0; i < WColForward.Length; i++)
        {
            wheels[i] = SetupWheels(wheelsF[i], WColForward[i]);
        }

        for (int i = 0; i < WColBack.Length; i++)
        {
            wheels[i + WColForward.Length] = SetupWheels(wheelsB[i], WColBack[i]);
        }

    }

    private void OnEnable()
    {
        _steeringWheel.Grab.AddListener(OnGrabbed);
        _steeringWheel.ReleaseHand.AddListener(OnReleased);
    }


    private void OnDisable()
    {
        _steeringWheel.Grab.RemoveListener(OnGrabbed);
        _steeringWheel.ReleaseHand.RemoveListener(OnReleased);
    }


    private void OnGrabbed()
    {
        _isGrabbed = true;
        if (_player != null)
            _playerGrabpose.position = _player.position;
    }

    private void OnReleased()
    {
        _isGrabbed = false;
        // if(_player != null)
        // _player.parent = null;
        Stop();
        _player.eulerAngles = Vector3.zero;
    }

    private WheelData SetupWheels(Transform wheel, WheelCollider col)
    {
        WheelData result = new WheelData();

        result.wheelTransform = wheel;
        result.col = col;
        result.wheelStartPos = wheel.transform.localPosition;

        return result;

    }
    void Update()
    {
        SoundCar();
      
    }

    float _acceleration = 0f;
    float _steering = 0f;
    void FixedUpdate()
    {
        if (_isGrabbed)
        {
            if (_steeringWheel.leftHand != null)
                _acceleration = -_throttle.GetAxis(_steeringWheel.leftHand.handType);
            if (_steeringWheel.rightHand != null)
                _acceleration = _throttle.GetAxis(_steeringWheel.rightHand.handType);

            _steering = Map(_steeringWheel.angle, -_steeringWheel.clamp, _steeringWheel.clamp, 1, -1);
            _player.position = _playerGrabpose.position;
            _player.rotation = _playerGrabpose.rotation;

        }
       
        CarMove();
        UpdateWheels();
    }

    public void Stop()
    {
        _acceleration = 0.0f;
    }

    private void UpdateWheels()
    {
        float delta = Time.fixedDeltaTime;


        foreach (WheelData w in wheels)
        {
            WheelHit hit;

            Vector3 lp = w.wheelTransform.localPosition;
            if (w.col.GetGroundHit(out hit))
            {
                lp.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius;
            }
            else
            {

                lp.y = w.wheelStartPos.y - wheelOffset;
            }
            w.wheelTransform.localPosition = lp;


            w.rotation = Mathf.Repeat(w.rotation + delta * w.col.rpm * 360.0f / 60.0f, 360.0f);
            w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 90.0f);
        }

    }

    private void CarMove()
    {

        foreach (WheelCollider col in WColForward)
        {
            col.steerAngle = _steering * maxSteer;
        }

        if (_acceleration == 0)
        {
            foreach (WheelCollider col in WColBack)
            {
                col.brakeTorque = braking_on_the_go;
            }

        }
        else
        {

            foreach (WheelCollider col in WColBack)
            {
                col.brakeTorque = 0;
                col.motorTorque = _acceleration * maxAccel;
            }

        }
        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     foreach (WheelCollider col in WColBack)
        //     {
        //         col.brakeTorque = maxBrake;

        //     }
        // }
        // else if (Input.GetKeyUp(KeyCode.LeftShift))
        // {
        //     foreach (WheelCollider col in WColBack)
        //     {
        //         col.brakeTorque = 0;
        //         col.motorTorque = _acceleration * maxAccel;
        //     }
        // }
        starSCar = _acceleration;
    }
    private void SoundCar()
    {
        if (starSCar > 0)
        {
            audioSourceCar.pitch = Mathf.Lerp(audioSourceCar.pitch, maxPitch, Time.deltaTime * 0.8f);
        }
        else if (starSCar < 0)
        {
            audioSourceCar.pitch = Mathf.Lerp(audioSourceCar.pitch, maxPitch, Time.deltaTime * 0.8f);
        }
        else if (starSCar == 0)
        {
            audioSourceCar.pitch = Mathf.Lerp(audioSourceCar.pitch, minPitch, Time.deltaTime * 1.5f);
        }
    }

    private static float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
