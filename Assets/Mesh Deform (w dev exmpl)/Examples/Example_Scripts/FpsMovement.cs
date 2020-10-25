using UnityEngine;
using System.Collections;

public class FpsMovement : MonoBehaviour 
{


	//----------CHARACTER MOVMENET------------


	public float speed = 6.0F;
	private Vector3 moveDirection;

    public bool fly = false;

    private void Start()
    {
        moveDirection = transform.position;
    }
    void Update() 
	{
		Rigidbody controller = GetComponent<Rigidbody>();

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveDirection.y, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed * Time.deltaTime;

        if (!fly)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                controller.AddForce(Vector3.up * 200);

            controller.position += moveDirection;

        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
                moveDirection.y += speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.C))
                moveDirection.y -= speed * Time.deltaTime;

            transform.position += moveDirection;
        }
    }
}
