using UnityEngine;
using System.Collections;

public class FrontLift : MonoBehaviour {

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
	public RearArron soundG;

	void Start()
	{
		
	}
	void Update()
	{
		if (Input.GetKey (KeyAB_L) && Input.GetKey (KeyFOR_L)) {
			FrontliftupL ();
			soundG.audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_L)) {
			soundG.audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_L) && Input.GetKey (KeyBAK_L)) {
			FrontliftdowenL ();
			soundG.audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_L)) {
			soundG.audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_R) && Input.GetKey (KeyFOR_R)) {
			FrontliftupR ();
			soundG.audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyFOR_R)) {
			soundG.audioF.pitch = 1f;
		}
		if (Input.GetKey (KeyAB_R) && Input.GetKey (KeyBAK_R)) {
			FrontliftdowenR ();
			soundG.audioF.pitch = 1.14f;
		} else if (Input.GetKeyUp (KeyBAK_R)) {
			soundG.audioF.pitch = 1f;
		}
	}
	public void FrontliftupL()
	{
		targetA_L.transform.localPosition = Vector3.MoveTowards(targetA_L.transform.localPosition, forwardPos_L, speed  * Time.deltaTime);
	}
	public void FrontliftdowenL()
	{
		targetA_L.transform.localPosition = Vector3.MoveTowards (targetA_L.transform.localPosition, rearPos_L, speed * Time.deltaTime);
	}
	public void FrontliftupR()
	{
		targetB_R.transform.localPosition = Vector3.MoveTowards(targetB_R.transform.localPosition, forwardPos_R, speed  * Time.deltaTime);
	}
	public void FrontliftdowenR()
	{
		targetB_R.transform.localPosition = Vector3.MoveTowards (targetB_R.transform.localPosition, rearPos_R, speed * Time.deltaTime);
	}

}
