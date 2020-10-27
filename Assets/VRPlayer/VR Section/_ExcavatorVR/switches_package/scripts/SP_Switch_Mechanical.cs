using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Switch_Mechanical : MonoBehaviour {
	
	private bool status = true;
	public GameObject pin; //the pin that will be flipped
	public AudioSource audioData; //sound

	//gets called when the player begins pressing the switch
	void grab(Quaternion controllerRotation){//flip the switch
		if(status) status = false;
		else status = true;
		if (pin != null) //flip rotation 
			pin.transform.eulerAngles *= -1;
		//play sound
		if(audioData) audioData.Play ();
	}

	//gets called when the player holds the switch
	void hold(Quaternion controllerRotation){
		//not used here
	}

	//gets called when the player lets go of the switch
	void letGo(Quaternion controllerRotation){
		//not used here
	}

}
