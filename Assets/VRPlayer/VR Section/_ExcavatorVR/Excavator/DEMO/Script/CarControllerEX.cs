using UnityEngine;
using System.Collections;

public class CarControllerEX : MonoBehaviour {

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

	public class WheelData{ 
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

	void Start () {

		audioSourceCar.pitch = startPitch;
		audioSourceCar.Play ();
		rg.centerOfMass = COM.localPosition;
		wheels = new WheelData[WColForward.Length+WColBack.Length]; 
		for (int i = 0; i<WColForward.Length; i++){ 
			wheels[i] = SetupWheels(wheelsF[i],WColForward[i]); 
		}

		for (int i = 0; i<WColBack.Length; i++){ 
			wheels[i+WColForward.Length] = SetupWheels(wheelsB[i],WColBack[i]); 
		}

	}


	private WheelData SetupWheels(Transform wheel, WheelCollider col){ 
		WheelData result = new WheelData(); 

		result.wheelTransform = wheel; 
		result.col = col; 
		result.wheelStartPos = wheel.transform.localPosition; 

		return result; 

	}
	void Update(){
		SoundCar();
	}
	void FixedUpdate () {

		float accel = 0;
		float steer = 0;

		accel = Input.GetAxis("Vertical");
		steer = Input.GetAxis("Horizontal");		
		CarMove(accel,steer);
		UpdateWheels();

	}

	private void UpdateWheels(){ 
		float delta = Time.fixedDeltaTime;


		foreach (WheelData w in wheels){ 
			WheelHit hit;

			Vector3 lp = w.wheelTransform.localPosition;
			if(w.col.GetGroundHit(out hit)){ 
				lp.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius; 
			}else{ 

				lp.y = w.wheelStartPos.y - wheelOffset; 
			}
			w.wheelTransform.localPosition = lp; 


			w.rotation = Mathf.Repeat(w.rotation + delta * w.col.rpm * 360.0f / 60.0f, 360.0f); 
			w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 90.0f); 
		}	

	}

	private void CarMove(float accel,float steer){

		foreach(WheelCollider col in WColForward){
			col.steerAngle = steer*maxSteer;
		}

		if(accel == 0){
			foreach(WheelCollider col in WColBack){
				col.brakeTorque = braking_on_the_go;
			}	

		}else{

			foreach(WheelCollider col in WColBack){
				col.brakeTorque = 0;
				col.motorTorque	= accel*maxAccel;
			}	

		}
		if (Input.GetKey (KeyCode.LeftShift)) {
			foreach (WheelCollider col in WColBack) {
				col.brakeTorque = maxBrake;

			}
		} else if (Input.GetKeyUp (KeyCode.LeftShift)) {
			foreach(WheelCollider col in WColBack){
				col.brakeTorque = 0;
				col.motorTorque	= accel*maxAccel;
			}
		}
		starSCar = accel;
	}
	private void SoundCar(){
		if (starSCar > 0) {
			audioSourceCar.pitch = Mathf.Lerp (audioSourceCar.pitch, maxPitch, Time.deltaTime * 0.8f);
		} else if (starSCar < 0) {
			audioSourceCar.pitch = Mathf.Lerp (audioSourceCar.pitch, maxPitch, Time.deltaTime * 0.8f);
		} else if (starSCar == 0) {
			audioSourceCar.pitch = Mathf.Lerp (audioSourceCar.pitch, minPitch, Time.deltaTime * 1.5f);
		}
	}
}