using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Tuner : MonoBehaviour {


	private Quaternion lastRotation; //used to determine the relative rotation
	public GameObject receiver; //the gameObject which might have a tune function attached 
	public bool detented = false; //detents exist? (clicking sound when turning a tuner)
	public float detentStep = 1f; //the minimum relative rotation for a click to happen
	public AudioSource audioData; //sound to play when turning
	public float value = 0.0f; //the total turning amount mutiplied by rate
	public float rate = 0.1f; //the rate at which the value increases when turning

	//at the start of turning this function is called for proper relative motion
	void grab(Quaternion rotation){
		lastRotation = rotation;
	}

	//gets called when the player holds the tuner
	void hold(Quaternion rotation){
		Quaternion delta = Quaternion.Inverse(lastRotation) * rotation;
		float relRot = delta.eulerAngles.z;
		if (!detented) {
			//this "if" is necessary, because when you turn back, the rotation will start at 360 and not be negative
			if (relRot < 180) {
				value += relRot * rate;
				transform.Rotate (Vector3.down * relRot);
			}else {
				value += (relRot - 360) * rate;
				transform.Rotate (Vector3.down * (relRot - 360));
			}
			//play sound
			if (audioData)
				audioData.Play ();
			//remember last rotation
			lastRotation = rotation;
			//send value to receiver
			if(receiver) receiver.SendMessage ("tune", value);
		}else if (relRot > detentStep) { //check wether the detent is passed
			if (relRot < 180) {
				//turn and increment value
				transform.Rotate (Vector3.down * relRot);
				value += relRot * rate;
				//play sound
				if (audioData)
					audioData.Play ();
				//remember last rotation
				lastRotation = rotation;
				//send value to receiver
				if(receiver) receiver.SendMessage ("tune", value);
			} else if (360 - relRot > detentStep) {
				//turn and increment value
				transform.Rotate (Vector3.down * (relRot - 360));
				value += (relRot - 360) * rate;
				//play sound
				if (audioData)
					audioData.Play ();
				//remember last rotation
				lastRotation = rotation;
				//send value to receiver
				if(receiver) receiver.SendMessage ("tune", value);
			}
		}
	}

	//gets called when the player lets go of the tuner
	void letGo(Quaternion controllerRotation){

	}


}
