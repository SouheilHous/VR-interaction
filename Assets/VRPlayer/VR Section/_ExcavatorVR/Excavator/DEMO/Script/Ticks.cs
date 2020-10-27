using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Ticks : MonoBehaviour {

	public Transform Piston1A;
	public Transform Piston2A;
	public KeyCode KeyFOR;
	public KeyCode KeyAB;
	public KeyCode KeyBAK;
	public float speed;
	public float minValue;
	public float maxValue;
	private Vector3 myRotation;
	public Transform target_Ticks;

	public enum RotAxis  {
		XAxis,
		YAxis,
		ZAxis
	}
	public RotAxis myRotAxis;
	public RearArron soundR;

	void Start() {
		myRotation = target_Ticks.localEulerAngles;
	}
	void Update()
	{
		if (Input.GetKey (KeyAB) && Input.GetKey (KeyFOR)) {
			Ticksup ();
			soundR.audioF.pitch = 1.14f;

		} else if (Input.GetKeyUp (KeyFOR)) {
			soundR.audioF.pitch = 1f;

		}
		if (Input.GetKey (KeyAB) && Input.GetKey (KeyBAK)) {
			Ticksdowen ();
			soundR.audioF.pitch = 1.14f;

		} else if (Input.GetKeyUp (KeyBAK)) {
			soundR.audioF.pitch = 1f;

		}

	}	
	void LateUpdate(){

		if (Piston1A != null && Piston2A != null) {
			Piston1A.LookAt (Piston2A.position, Piston1A.up);
			Piston2A.LookAt (Piston1A.position, Piston2A.up);
		}
	}
	public void Ticksup()
	{
		switch(myRotAxis)  {
		case RotAxis.XAxis:
			myRotation.x = Mathf.Clamp(myRotation.x + speed * Time.deltaTime, minValue, maxValue);
			break;
		case RotAxis.YAxis:
			myRotation.y = Mathf.Clamp(myRotation.y + speed * Time.deltaTime, minValue, maxValue);
			break;
		case RotAxis.ZAxis:
			myRotation.z = Mathf.Clamp(myRotation.z + speed * Time.deltaTime, minValue, maxValue);
			break;
		}
		target_Ticks.transform.localRotation = Quaternion.Euler(myRotation);
	}
	public void Ticksdowen()
	{
		switch(myRotAxis)  {
		case RotAxis.XAxis:
			myRotation.x = Mathf.Clamp(myRotation.x - speed * Time.deltaTime, minValue, maxValue);
			break;
		case RotAxis.YAxis:
			myRotation.y = Mathf.Clamp(myRotation.y - speed * Time.deltaTime, minValue, maxValue);
			break;
		case RotAxis.ZAxis:
			myRotation.z = Mathf.Clamp(myRotation.z - speed * Time.deltaTime, minValue, maxValue);
			break;
		}
		target_Ticks.transform.localRotation = Quaternion.Euler(myRotation);
	}
}
