using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {


    //----------MOUSE LOOK------------------
    public bool OnClick = false;
    public bool HideCursor = false;
    public KeyCode OnClickKey = KeyCode.Mouse2;

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, 	MouseY = 2 };
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationX = 0F;
	float rotationY = 0F;
	
	Quaternion originalRotation;
    Quaternion rotToRotate;
	void Update()
	{
        if(HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            newSphere.transform.position = this.transform.position + transform.forward * 0.5f;
            newSphere.AddComponent<Rigidbody>().AddForce(transform.forward * 1000);
            Destroy(newSphere, 10);
        }
        if (axes == RotationAxes.MouseXAndY)
		{
            bool allowed = true;
            if (OnClick)
                allowed = Input.GetKey(OnClickKey);
            // Read the mouse input axis
            if (allowed)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                rotToRotate = originalRotation * xQuaternion * yQuaternion;
            }

            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotToRotate, 5 * Time.deltaTime);
		}
		else if (axes == RotationAxes.MouseX)
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationX = ClampAngle(rotationX, minimumX, maximumX);
			
			Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation = originalRotation * xQuaternion;
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = ClampAngle(rotationY, minimumY, maximumY);
			
			Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
			transform.localRotation = originalRotation * yQuaternion;
		}
	}
	
	void Start()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
	
}
