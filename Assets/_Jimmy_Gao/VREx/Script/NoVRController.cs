using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoVRController : MonoBehaviour {
    float sensitivity = 0.1f;
    Vector3 lastMouse;
    // Use this for initialization

    public GameObject Controller;
    void Start () {
        lastMouse = Input.mousePosition;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 mouseDelta = Input.mousePosition - lastMouse;
        lastMouse = Input.mousePosition;
        this.transform.localEulerAngles += new Vector3(-mouseDelta.y, mouseDelta.x, 0) * sensitivity;
        Ray myRay = new Ray(Controller.transform.position, Controller.transform.forward);

        VRExInputModule.CustomControllerButtonDown = Input.GetMouseButton(0);
    }
}
