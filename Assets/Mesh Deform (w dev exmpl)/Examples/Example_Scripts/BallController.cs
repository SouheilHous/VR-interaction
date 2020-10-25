using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {


    public Rigidbody rigid;
    public float speed;
	void Update ()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        dir = Camera.main.transform.TransformDirection(dir);
        rigid.AddTorque(dir * speed);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            rigid.AddForce(Vector3.up * 200);
	}
}
