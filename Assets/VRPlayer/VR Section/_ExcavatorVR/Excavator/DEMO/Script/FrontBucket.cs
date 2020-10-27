using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class FrontBucket : MonoBehaviour {

	public Transform Pis1T;
	public Transform Pis2T;
	public KeyCode KeyFOR;
	public KeyCode KeyAB;
	public KeyCode KeyBAK;
	public float speed;
	public float minValue;
	public float maxValue;
	private Vector3 myRotation;
	public Transform target;

	public enum RotAxis  {
		XAxis,
		YAxis,
		ZAxis
	}
	public RotAxis myRotAxis;
	public RearArron soundT;

	void Start() {
		myRotation = target.localEulerAngles;
	}
	void Update()
	{
		if (Input.GetKey (KeyAB) && Input.GetKey (KeyFOR)) {
			Frontbucketfor ();
			soundT.audioF.pitch = 1.14f;

		} else if (Input.GetKeyUp (KeyFOR)) {
			soundT.audioF.pitch = 1f;
		
		}
		if (Input.GetKey (KeyAB) && Input.GetKey (KeyBAK)) {
			Frontbucketrea ();
			soundT.audioF.pitch = 1.14f;

		} else if (Input.GetKeyUp (KeyBAK)) {
			soundT.audioF.pitch = 1f;
		
		}
	
	}	
	void LateUpdate(){

		if (Pis1T != null && Pis2T != null) {
			Pis1T.LookAt (Pis2T.position, Pis1T.up);
			Pis2T.LookAt (Pis1T.position, Pis2T.up);
		}
}
	public void Frontbucketfor()
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
		target.transform.localRotation = Quaternion.Euler(myRotation);
	}
	public void Frontbucketrea()
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
		target.transform.localRotation = Quaternion.Euler(myRotation);
	}

}