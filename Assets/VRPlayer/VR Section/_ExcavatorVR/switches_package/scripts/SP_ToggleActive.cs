using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_ToggleActive : MonoBehaviour {
	//Attach the gameObject this script is attached to, as a receiver of a button

	//the target to disable
	public GameObject target;
	//this function disables/activates an object;
	//it could do anything else though, like playing a sound or an animation.
	void press(string message){
		if (target) {
			if (target.activeSelf)
				target.SetActive (false);
			else target.SetActive(true);
		}

	}
}
