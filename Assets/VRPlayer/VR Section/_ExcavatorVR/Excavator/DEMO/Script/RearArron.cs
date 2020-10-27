using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class RearArron : MonoBehaviour {

	public Transform targetA_L;
	public Transform targetB_R;
	public float speed = 0.0f;
	public KeyCode KeyFOR_L;
	public KeyCode KeyAB_L;
	public KeyCode KeyBAK_L;
	public Vector3 forwardPos_L;
	public Vector3 rearPos_L;
	public KeyCode KeyFOR_R;
	public KeyCode KeyAB_R;
	public KeyCode KeyBAK_R;
	public Vector3 forwardPos_R;
	public Vector3 rearPos_R;

	public Transform arrow0;
	public KeyCode KeyFOR_Arrow;
	public KeyCode KeyAB_Arrow;
	public KeyCode KeyBAK_Arrow;
	public Vector3 forwardPos_Arrow;
	public Vector3 rearPos_Arrow;
	public float speedRear_Arron = 0f;

	public Transform Arrow1;
	public KeyCode KeyFOR_Arrow1;
	public KeyCode KeyAB_Arrow1;
	public KeyCode KeyBAK_Arrow1;
	public float speedRear_Arrow1;
	public float minValue_Arrow1;
	public float maxValue_Arrow1;
	private Vector3 myRotation_Arrow1;
	public enum RotAxis_Arrow1  {
		XAxis,
		YAxis,
		ZAxis
	}
	public RotAxis_Arrow1 myRotAxis_Arrow1;

	public Transform Arrow2;
	public KeyCode KeyFOR_Arrow2;
	public KeyCode KeyAB_Arrow2;
	public KeyCode KeyBAK_Arrow2;
	public float speedRear_Arrow2;
	public float minValue_Arrow2;
	public float maxValue_Arrow2;
	private Vector3 myRotation_Arrow2;
	public enum RotAxis_Arrow2  {
		XAxis,
		YAxis,
		ZAxis
	}
	public RotAxis_Arrow2 myRotAxis_Arrow2;

	public Transform Arrow3;
	public KeyCode KeyFOR_Arrow3;
	public KeyCode KeyAB_Arrow3;
	public KeyCode KeyBAK_Arrow3;
	public float speedRear_Arrow3;
	public float minValue_Arrow3;
	public float maxValue_Arrow3;
	private Vector3 myRotation_Arrow3;
	public enum RotAxis_Arrow3  {
		XAxis,
		YAxis,
		ZAxis
	}
	public RotAxis_Arrow3 myRotAxis_Arrow3;

	public Transform Arrow4;
	public KeyCode KeyFOR_Arrow4;
	public KeyCode KeyAB_Arrow4;
	public KeyCode KeyBAK_Arrow4;
	public float speedRear_Arrow4;
	public float minValue_Arrow4;
	public float maxValue_Arrow4;
	private Vector3 myRotation_Arrow4;
	public enum RotAxis_Arrow4  {
		XAxis,
		YAxis,
		ZAxis
	}
	public RotAxis_Arrow4 myRotAxis_Arrow4;

	public Transform arrow5;
	public KeyCode KeyFOR_Arrow5;
	public KeyCode KeyAB_Arrow5;
	public KeyCode KeyBAK_Arrow5;
	public Vector3 forwardPos_Arrow5;
	public Vector3 rearPos_Arrow5;
	public float speedRear_Arron5 = 0f;

	public Transform Piston1F;
	public Transform Piston2F;
	public Transform Piston1G;
	public Transform Piston2G;
	public Transform Piston1B;
	public Transform Piston2B;
	public Transform Piston1D;
	public Transform Piston2D;
	public Transform Piston1C;
	public Transform Piston2C;
	public Transform Det1D;
	public Transform Det2D;
	public Transform Det1C;
	public Transform Det2C;

	public AudioSource audioF;


	void Start()
	{
		audioF = GetComponent<AudioSource> ();
		myRotation_Arrow1 = Arrow1.localEulerAngles;
		myRotation_Arrow2 = Arrow2.localEulerAngles;
		myRotation_Arrow3 = Arrow3.localEulerAngles;
		myRotation_Arrow4 = Arrow4.localEulerAngles;
	}


	void Update2()
	{
		if (Input.GetKey (KeyAB_L) && Input.GetKey (KeyFOR_L)) {
			RearliftupL ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_L)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_L) && Input.GetKey (KeyBAK_L)) {
			RearliftdowenL ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_L)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_R) && Input.GetKey (KeyFOR_R)) {
			RearliftupR ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_R)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_R) && Input.GetKey (KeyBAK_R)) {
			RearliftdowenR ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_R)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow) && Input.GetKey (KeyFOR_Arrow)) {
			Arrow0up ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_Arrow)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow) && Input.GetKey (KeyBAK_Arrow)) {
			Arrow0dowen ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_Arrow)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow1) && Input.GetKey (KeyFOR_Arrow1)) {
			Arrow1up ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_Arrow1)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow1) && Input.GetKey (KeyBAK_Arrow1)) {
			Arrow1dowen ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_Arrow1)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow2) && Input.GetKey (KeyFOR_Arrow2)) {
			Arrow2up ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_Arrow2)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow2) && Input.GetKey (KeyBAK_Arrow2)) {
			Arrow2dowen ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_Arrow2)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow3) && Input.GetKey (KeyFOR_Arrow3)) {
			Arrow3up ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_Arrow3)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow3) && Input.GetKey (KeyBAK_Arrow3)) {
			Arrow3dowen ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_Arrow3)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow4) && Input.GetKey (KeyFOR_Arrow4)) {
			Arrow4up ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_Arrow4)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow4) && Input.GetKey (KeyBAK_Arrow4)) {
			Arrow4dowen ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_Arrow4)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow5) && Input.GetKey (KeyFOR_Arrow5)) {
			Arrow5up ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_Arrow5)) {
			audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_Arrow5) && Input.GetKey (KeyBAK_Arrow5)) {
			Arrow5dowen ();
			audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_Arrow5)) {
			audioF.pitch = 1f;
		}
	}
	void LateUpdate(){

		if (Piston1F != null && Piston2F != null) {
			Piston1F.LookAt (Piston2F.position, Piston1F.up);
			Piston2F.LookAt (Piston1F.position, Piston2F.up);
		}
		if (Piston1G != null && Piston2G != null) {
			Piston1G.LookAt (Piston2G.position, Piston1G.up);
			Piston2G.LookAt (Piston1G.position, Piston2G.up);
		}
		if (Piston1B != null && Piston2B != null) {
			Piston1B.LookAt (Piston2B.position, Piston1B.up);
			Piston2B.LookAt (Piston1B.position, Piston2B.up);
		}
		if (Piston1C != null && Piston2C != null) {
			Piston1C.LookAt (Piston2C.position, Piston1C.up);
			Piston2C.LookAt (Piston1C.position, Piston2C.up);
		}
		if (Piston1D != null && Piston2D != null) {
			Piston1D.LookAt (Piston2D.position, Piston1D.up);
			Piston2D.LookAt (Piston1D.position, Piston2D.up);
		}
		if (Det1D != null && Det2D != null) {
			Det1D.LookAt (Det2D.position, Det1D.up);
			Det2D.LookAt (Det1D.position, Det2D.up);
		}
		if (Det1C != null && Det2C != null) {
			Det1C.LookAt (Det2C.position, Det1C.up);
			Det2C.LookAt (Det1C.position, Det2C.up);
		}
	
	}

	
	public void RearliftupL()
	{
		targetA_L.transform.localPosition = Vector3.MoveTowards(targetA_L.transform.localPosition, forwardPos_L, speed  * Time.deltaTime);
	}
	public void RearliftdowenL()
	{
		targetA_L.transform.localPosition = Vector3.MoveTowards (targetA_L.transform.localPosition, rearPos_L, speed * Time.deltaTime);
	}
	public void RearliftupR()
	{
		targetB_R.transform.localPosition = Vector3.MoveTowards(targetB_R.transform.localPosition, forwardPos_R, speed  * Time.deltaTime);
	}
	public void RearliftdowenR()
	{
		targetB_R.transform.localPosition = Vector3.MoveTowards (targetB_R.transform.localPosition, rearPos_R, speed * Time.deltaTime);
	}
	public void Arrow0up()
	{
		arrow0.transform.localPosition = Vector3.MoveTowards(arrow0.transform.localPosition, forwardPos_Arrow, speedRear_Arron  * Time.deltaTime);
	}
	public void Arrow0dowen()
	{
		arrow0.transform.localPosition = Vector3.MoveTowards(arrow0.transform.localPosition, rearPos_Arrow, speedRear_Arron  * Time.deltaTime);
	}
	public void Arrow5up()
	{
		arrow5.transform.localPosition = Vector3.MoveTowards(arrow5.transform.localPosition, forwardPos_Arrow5, speedRear_Arron5  * Time.deltaTime);
	}
	public void Arrow5dowen()
	{
		arrow5.transform.localPosition = Vector3.MoveTowards(arrow5.transform.localPosition, rearPos_Arrow5, speedRear_Arron5  * Time.deltaTime);
	}
	public void Arrow1up()
	{
		switch(myRotAxis_Arrow1)  {
		case RotAxis_Arrow1.XAxis:
			myRotation_Arrow1.x = Mathf.Clamp(myRotation_Arrow1.x + speedRear_Arrow1 * Time.deltaTime, minValue_Arrow1, maxValue_Arrow1);
			break;
		case RotAxis_Arrow1.YAxis:
			myRotation_Arrow1.y = Mathf.Clamp(myRotation_Arrow1.y + speedRear_Arrow1 * Time.deltaTime, minValue_Arrow1, maxValue_Arrow1);
			break;
		case RotAxis_Arrow1.ZAxis:
			myRotation_Arrow1.z = Mathf.Clamp(myRotation_Arrow1.z + speedRear_Arrow1 * Time.deltaTime, minValue_Arrow1, maxValue_Arrow1);
			break;
		}
		Arrow1.transform.localRotation = Quaternion.Euler(myRotation_Arrow1);
	}
	public void Arrow1dowen()
	{
		switch(myRotAxis_Arrow1)  {
		case RotAxis_Arrow1.XAxis:
			myRotation_Arrow1.x = Mathf.Clamp(myRotation_Arrow1.x - speedRear_Arrow1 * Time.deltaTime, minValue_Arrow1, maxValue_Arrow1);
			break;
		case RotAxis_Arrow1.YAxis:
			myRotation_Arrow1.y = Mathf.Clamp(myRotation_Arrow1.y - speedRear_Arrow1 * Time.deltaTime, minValue_Arrow1, maxValue_Arrow1);
			break;
		case RotAxis_Arrow1.ZAxis:
			myRotation_Arrow1.z = Mathf.Clamp(myRotation_Arrow1.z - speedRear_Arrow1 * Time.deltaTime, minValue_Arrow1, maxValue_Arrow1);
			break;
		}
		Arrow1.transform.localRotation = Quaternion.Euler(myRotation_Arrow1);
	}
	public void Arrow2up()
	{
		switch(myRotAxis_Arrow2)  {
		case RotAxis_Arrow2.XAxis:
			myRotation_Arrow2.x = Mathf.Clamp(myRotation_Arrow2.x + speedRear_Arrow2 * Time.deltaTime, minValue_Arrow2, maxValue_Arrow2);
			break;
		case RotAxis_Arrow2.YAxis:
			myRotation_Arrow2.y = Mathf.Clamp(myRotation_Arrow2.y + speedRear_Arrow2 * Time.deltaTime, minValue_Arrow2, maxValue_Arrow2);
			break;
		case RotAxis_Arrow2.ZAxis:
			myRotation_Arrow2.z = Mathf.Clamp(myRotation_Arrow2.z + speedRear_Arrow2 * Time.deltaTime, minValue_Arrow2, maxValue_Arrow2);
			break;
		}
		Arrow2.transform.localRotation = Quaternion.Euler(myRotation_Arrow2);
	}
	public void Arrow2dowen()
	{
		switch(myRotAxis_Arrow2)  {
		case RotAxis_Arrow2.XAxis:
			myRotation_Arrow2.x = Mathf.Clamp(myRotation_Arrow2.x - speedRear_Arrow2 * Time.deltaTime, minValue_Arrow2, maxValue_Arrow2);
			break;
		case RotAxis_Arrow2.YAxis:
			myRotation_Arrow2.y = Mathf.Clamp(myRotation_Arrow2.y - speedRear_Arrow2 * Time.deltaTime, minValue_Arrow2, maxValue_Arrow2);
			break;
		case RotAxis_Arrow2.ZAxis:
			myRotation_Arrow2.z = Mathf.Clamp(myRotation_Arrow2.z - speedRear_Arrow2 * Time.deltaTime, minValue_Arrow2, maxValue_Arrow2);
			break;
		}
		Arrow2.transform.localRotation = Quaternion.Euler(myRotation_Arrow2);
	}
	public void Arrow3up()
	{
		switch(myRotAxis_Arrow3)  {
		case RotAxis_Arrow3.XAxis:
			myRotation_Arrow3.x = Mathf.Clamp(myRotation_Arrow3.x + speedRear_Arrow3 * Time.deltaTime, minValue_Arrow3, maxValue_Arrow3);
			break;
		case RotAxis_Arrow3.YAxis:
			myRotation_Arrow3.y = Mathf.Clamp(myRotation_Arrow3.y + speedRear_Arrow3 * Time.deltaTime, minValue_Arrow3, maxValue_Arrow3);
			break;
		case RotAxis_Arrow3.ZAxis:
			myRotation_Arrow3.z = Mathf.Clamp(myRotation_Arrow3.z + speedRear_Arrow3 * Time.deltaTime, minValue_Arrow3, maxValue_Arrow3);
			break;
		}
		Arrow3.transform.localRotation = Quaternion.Euler(myRotation_Arrow3);
	}
	public void Arrow3dowen()
	{
		switch(myRotAxis_Arrow3)  {
		case RotAxis_Arrow3.XAxis:
			myRotation_Arrow3.x = Mathf.Clamp(myRotation_Arrow3.x - speedRear_Arrow3 * Time.deltaTime, minValue_Arrow3, maxValue_Arrow3);
			break;
		case RotAxis_Arrow3.YAxis:
			myRotation_Arrow3.y = Mathf.Clamp(myRotation_Arrow3.y - speedRear_Arrow3 * Time.deltaTime, minValue_Arrow3, maxValue_Arrow3);
			break;
		case RotAxis_Arrow3.ZAxis:
			myRotation_Arrow3.z = Mathf.Clamp(myRotation_Arrow3.z - speedRear_Arrow3 * Time.deltaTime, minValue_Arrow3, maxValue_Arrow3);
			break;
		}
		Arrow3.transform.localRotation = Quaternion.Euler(myRotation_Arrow3);
	}
	public void Arrow4up()
	{
		switch(myRotAxis_Arrow4)  {
		case RotAxis_Arrow4.XAxis:
			myRotation_Arrow4.x = Mathf.Clamp(myRotation_Arrow4.x + speedRear_Arrow4 * Time.deltaTime, minValue_Arrow4, maxValue_Arrow4);
			break;
		case RotAxis_Arrow4.YAxis:
			myRotation_Arrow4.y = Mathf.Clamp(myRotation_Arrow4.y + speedRear_Arrow4 * Time.deltaTime, minValue_Arrow4, maxValue_Arrow4);
			break;
		case RotAxis_Arrow4.ZAxis:
			myRotation_Arrow4.z = Mathf.Clamp(myRotation_Arrow4.z + speedRear_Arrow4 * Time.deltaTime, minValue_Arrow4, maxValue_Arrow4);
			break;
		}
		Arrow4.transform.localRotation = Quaternion.Euler(myRotation_Arrow4);
	}
	public void Arrow4dowen()
	{
		switch(myRotAxis_Arrow4)  {
		case RotAxis_Arrow4.XAxis:
			myRotation_Arrow4.x = Mathf.Clamp(myRotation_Arrow4.x - speedRear_Arrow4 * Time.deltaTime, minValue_Arrow4, maxValue_Arrow4);
			break;
		case RotAxis_Arrow4.YAxis:
			myRotation_Arrow4.y = Mathf.Clamp(myRotation_Arrow4.y - speedRear_Arrow4 * Time.deltaTime, minValue_Arrow4, maxValue_Arrow4);
			break;
		case RotAxis_Arrow4.ZAxis:
			myRotation_Arrow4.z = Mathf.Clamp(myRotation_Arrow4.z - speedRear_Arrow4 * Time.deltaTime, minValue_Arrow4, maxValue_Arrow4);
			break;
		}
		Arrow4.transform.localRotation = Quaternion.Euler(myRotation_Arrow4);
	}
}
