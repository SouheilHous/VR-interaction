using UnityEngine;
using System.Collections;

public class Cabin : MonoBehaviour {

	public Transform Door_Left;
	public KeyCode keyLeftDoor;
	public float angelFB_LeftDoor = 0f;
	private float smooth = 0.3f;
	private Quaternion tRotationLeft_Door;
	private bool a = true;

	public Transform Door_Right;
	public KeyCode keyRightDoor;
	public float angelFB_RightDoor = 0f;
	private Quaternion tRotationRight_Door;
	private bool b = true;

	public Transform Window_Left;
	public KeyCode keyLeftWindow;
	public float angelFB_LeftWindow = 0f;
	private Quaternion tRotationLeft_Window;
	private bool c = true;

	public Transform Window_Right;
	public KeyCode keyRightWindow;
	public float angelFB_RightWindow = 0f;
	private Quaternion tRotationRight_Window;
	private bool d = true;
	AudioSource sound_Door;
	public AudioClip doorS;

	void Start()
	{
		tRotationLeft_Door = Door_Left.transform.localRotation;
		tRotationRight_Door = Door_Right.transform.localRotation;
		tRotationLeft_Window = Window_Left.transform.localRotation;
		tRotationRight_Window = Window_Right.transform.localRotation;
		sound_Door = GetComponent<AudioSource> ();
	}
	void Update(){

		if (Input.GetKeyDown (keyLeftDoor) && a == true) {
			tRotationLeft_Door *= Quaternion.AngleAxis (angelFB_LeftDoor, Vector3.up);
			a = false;
			sound_Door.PlayOneShot(doorS,0.7f);
		} else if (Input.GetKeyDown (keyLeftDoor) && a == false) {
			tRotationLeft_Door *= Quaternion.AngleAxis (-angelFB_LeftDoor, Vector3.up);
			a = true;
			sound_Door.PlayOneShot(doorS,0.7f);
		}
		if (Input.GetKeyDown (keyRightDoor) && b == true) {
			tRotationRight_Door *= Quaternion.AngleAxis (angelFB_RightDoor, Vector3.up);
			b = false;
			sound_Door.PlayOneShot(doorS,0.7f);
		} else if (Input.GetKeyDown (keyRightDoor) && b == false) {
			tRotationRight_Door *= Quaternion.AngleAxis (-angelFB_RightDoor, Vector3.up);
			b = true;
			sound_Door.PlayOneShot(doorS,0.7f);
		}
		if (Input.GetKeyDown (keyLeftWindow) && c == true) {
			tRotationLeft_Window *= Quaternion.AngleAxis (angelFB_LeftWindow, Vector3.up);
			c = false;
			sound_Door.PlayOneShot(doorS,0.7f);
		} else if (Input.GetKeyDown (keyLeftWindow) && c == false) {
			tRotationLeft_Window *= Quaternion.AngleAxis (-angelFB_LeftWindow, Vector3.up);
			c = true;
			sound_Door.PlayOneShot(doorS,0.7f);
		}
		if (Input.GetKeyDown (keyRightWindow) && d == true) {
			tRotationRight_Window *= Quaternion.AngleAxis (angelFB_RightWindow, Vector3.up);
			d = false;
			sound_Door.PlayOneShot(doorS,0.7f);
		} else if (Input.GetKeyDown (keyRightWindow) && d == false) {
			tRotationRight_Window *= Quaternion.AngleAxis (-angelFB_RightWindow, Vector3.up);
			d = true;
			sound_Door.PlayOneShot(doorS,0.7f);
		}

		Door_Right.transform.localRotation= Quaternion.Lerp (Door_Right.transform.localRotation, tRotationRight_Door , 10 * smooth * Time.deltaTime); 
		Door_Left.transform.localRotation= Quaternion.Lerp (Door_Left.transform.localRotation, tRotationLeft_Door , 10 * smooth * Time.deltaTime); 
		Window_Right.transform.localRotation= Quaternion.Lerp (Window_Right.transform.localRotation, tRotationRight_Window , 10 * smooth * Time.deltaTime); 
		Window_Left.transform.localRotation= Quaternion.Lerp (Window_Left.transform.localRotation, tRotationLeft_Window , 10 * smooth * Time.deltaTime); 

	}

	public void ToggleLeftDoor(){
		if (a == true) {
			tRotationLeft_Door *= Quaternion.AngleAxis (angelFB_LeftDoor, Vector3.up);
			a = false;
			sound_Door.PlayOneShot(doorS,0.7f);
		} else if (a == false) {
			tRotationLeft_Door *= Quaternion.AngleAxis (-angelFB_LeftDoor, Vector3.up);
			a = true;
			sound_Door.PlayOneShot(doorS,0.7f);
		}
	}

	public void ToggleRightDoor(){
		if ( b == true) {
			tRotationRight_Door *= Quaternion.AngleAxis (angelFB_RightDoor, Vector3.up);
			b = false;
			sound_Door.PlayOneShot(doorS,0.7f);
		} else if ( b == false) {
			tRotationRight_Door *= Quaternion.AngleAxis (-angelFB_RightDoor, Vector3.up);
			b = true;
			sound_Door.PlayOneShot(doorS,0.7f);
		}
	}
}