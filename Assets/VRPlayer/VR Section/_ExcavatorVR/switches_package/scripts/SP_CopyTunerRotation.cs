using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_CopyTunerRotation : MonoBehaviour {

	public Transform source; //the transform whose rotation is to be copied
	public float speed = 1.0f; 

	void Update () {
		if (source) {
			transform.localRotation = Quaternion.Lerp (transform.localRotation, source.transform.localRotation, speed * Time.deltaTime);
		}
	}
}
