using UnityEngine;
using System.Collections;

public class CameraOrbitEx : MonoBehaviour {

	public Transform target_Car;
	public float distance = 6f;
	private float x = 0f;
	private float y = 0f;
	float xSpeed= 250f;
	float  ySpeed= 120f;
	private float yMinLi= -30f;
	private float yMaxLi= 85f;

	void  LateUpdate (){

		if (target_Car) {

			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			y = ClampAngle(y, yMinLi, yMaxLi);
			Quaternion rotation= Quaternion.Euler(y, x, 0);
			Vector3 position= rotation * new Vector3(0f, 0f, -distance) + target_Car.position;

			transform.rotation = rotation;
			transform.position = position;
		}
		//	if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
		//	GetComponent<Camera> ().fieldOfView--;
		//} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
		//	GetComponent<Camera> ().fieldOfView++;
		//}
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			distance--;
		} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			distance++;
		}
		if (Input.GetKeyDown (KeyCode.RightShift)) {
			xSpeed = 0.0f;
			ySpeed = 0.0f;
		}else if (Input.GetKeyUp(KeyCode.RightShift)){
			xSpeed= 250f;
			ySpeed= 120f;
		}

	}
	static float ClampAngle ( float angle ,   float min ,   float max  ){

		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}
