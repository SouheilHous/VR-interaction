using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_AdjustBrightness : MonoBehaviour {

	//Adjusts the brightness of the light component attached to the script.
	public Light light;
	//the gameObject this script is attached to needs to be the receiver of a tuner
	//when the tuner is "tuned" it is going to call this function
	void tune(float value){
		//if a light exists, change it to the value of the tune function parameter
		if(light) light.intensity = value;
	}
}
