using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Button : MonoBehaviour {
	//for memory
	public bool status = false;
	private float lastStatusChange = 0.0f;

	public float delay = 1f; //the time in seconds it takes, for the button to go up again
	public float pushInDistance = 0.1f; //the distance that the button will be pushed in
	public GameObject receiver; //the gameObject which might have a tune function attached 
	public AudioSource audioData; //the sound
	public string message = ""; //the signal to send to the receiver when button is pressed 

	// Update is called once per frame
	void Update () {
		if (status) {
			if (Time.time - lastStatusChange > delay) { //when delay is passed reset button
				transform.localPosition += Vector3.up * pushInDistance * transform.localScale.y;
				status = false;
			}
		}
	}

	//gets called when the player begins pressing the switch
	void grab(Quaternion controllerRotation){
		if (!status) {//when not pressed
			status = true;
			transform.localPosition += Vector3.down * pushInDistance * transform.localScale.y;
			//play sound
			if(audioData) audioData.Play ();
			//remember the time the button was pressed
			lastStatusChange = Time.time;
			//send to the receiver
			if (receiver) {
				receiver.SendMessage ("press", message);
			}
		}

	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(Vector3.down * pushInDistance));
        
    }

    //gets called when the player holds the button
    void hold(Quaternion controllerRotation){
		//not used here
	}

	//gets called when the player lets go of the button
	void letGo(Quaternion controllerRotation){
		//not used here
	}
}
