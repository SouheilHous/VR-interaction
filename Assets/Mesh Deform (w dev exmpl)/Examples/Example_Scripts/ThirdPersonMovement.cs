using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour {

    //----------CHARACTER MOVMENET------------

    public Transform mainCamera;

    public float speed = 6.0F;
    private Vector3 moveDirection;

    public AnimationClip WalkingAnim;
    public AnimationClip IdleAnim;

    Animation anim;

    private void Start()
    {
        moveDirection = transform.position;
        anim = transform.GetChild(0).GetComponent<Animation>();
    }
    void Update()
    {
        Rigidbody controller = GetComponent<Rigidbody>();

        moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed * Time.deltaTime;

        if (Input.GetAxis("Vertical") < 0)
            return;

        if (moveDirection == Vector3.zero)
            anim.CrossFade(IdleAnim.name);
        else
        {
            anim.CrossFade(WalkingAnim.name);
            Quaternion quat = Quaternion.identity;
            quat = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mainCamera.forward), 0.05f);
            quat.x = transform.rotation.x;
            quat.z = transform.rotation.z;
            transform.rotation = quat;
        }

        controller.position+= moveDirection;
    }
}
