using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Flying : MonoBehaviour
{

    public float FlySpeed;
    public float MinFlySpeed, MaxFlySpeed;
    public float XflySpd;
    public float longPressTimer;
    public Transform Character;
    public float m_Snstivity = 0.1f;
    public float m_MaxMoveSpeed = 1.0f;
    public int PressTime = 1;

    public SteamVR_Action_Boolean MovePlayer = null;
    public SteamVR_Action_Vector2 RotateAround = null;

    private float mov_Speed = 0.0f;
    private CharacterController c_Controller;
    private Transform m_Cam_Rig;
    private Transform m_HeadTransform;
    public Transform head;
    

    public SteamVR_Behaviour_Pose left;
    public SteamVR_Behaviour_Pose right;
    public SteamVR_Behaviour_Pose clap;

    public float RotationSpeed = 50f;
    public float UpMoveSpd = 1f;
    public float BigJumpForce = 1f;
    public float BigJumpSmoothness = 1f;
    public float GroundMovSpd = 1f;


    public SteamVR_Input_Sources Hand_R;
    public SteamVR_Input_Sources Hand_L;

   
   



    private bool isFlying = false;

    private bool rightOn = false;
    private bool leftOn = false;
    private int gripsPressed;
    private bool BigJump;
    private bool InverseBigJump;
    private float JumpPos;

    //orientation 
    private Vector3 orientationEuler;
    private Quaternion orientation;
    private Vector3 movement;

    private void Awake()
    {
        c_Controller = Character.GetComponent<CharacterController>();
    }

    private void Start()
    {
        m_Cam_Rig = transform;
        m_HeadTransform = head;
        gripsPressed = 0;

        
    }


    private void HandleHead()
    {
        //old values/ set values
        //print("Cam rig = " + m_Cam_Rig);
        //print("head = " + m_HeadTransform);
        Vector3 oldPos = m_Cam_Rig.position;
        Quaternion oldRot = m_Cam_Rig.rotation;

        //rotation
        Character.transform.eulerAngles = new Vector3(0.0f, m_Cam_Rig.rotation.eulerAngles.y, 0.0f);

        //restore
        m_Cam_Rig.position = oldPos;
        m_Cam_Rig.rotation = oldRot;



    }

    private void CalculateMovement()
    {
        // geting movement orientation
        orientationEuler = new Vector3(0.0f, head.eulerAngles.y, 0.0f);
        orientation = Quaternion.Euler(orientationEuler);
        movement = Vector3.zero;

        //if not moving
        //if (MovePlayer.GetStateUp(SteamVR_Input_Sources.Any))
        //{
        //    mov_Speed = 0.0f;
        //}

        ////if button pressed
        //if (MovePlayer.state)
        //{

        //    //add, clamp
        //    mov_Speed += (RotateAround.axis.y * m_Snstivity);
        //    mov_Speed = Mathf.Clamp(mov_Speed, -m_MaxMoveSpeed, m_MaxMoveSpeed);
        //    print("Forward Move");
        //    ////orientation
        //    movement += orientation * (mov_Speed * Vector3.forward) * Time.deltaTime;
        //    //print("orientation = " + orientation);
        //    //c_Controller.transform.rotation = orientation;

        //}
        //movement += orientation * (Vector3.forward);
        //Apply
        //c_Controller.Move(movement);
    }

    private void HandleHeight()
    {
        //get head in local state
        float headHeight = Mathf.Clamp(m_HeadTransform.localPosition.y, 1, 2);
        c_Controller.height = headHeight;

        //cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = c_Controller.height / 2;
        newCenter.y += c_Controller.skinWidth;

        //move capsule in local space
        newCenter.x = m_HeadTransform.localEulerAngles.x;
        newCenter.z = m_HeadTransform.localEulerAngles.z;

        //rotate 
        newCenter = Quaternion.Euler(0, -Character.eulerAngles.y, 0) * newCenter;
        //Apply

        c_Controller.center = newCenter;
    }


    void OnGUI()
    {
        if (isFlying)
        {
            GUI.Label(new Rect(10, 10, 300, 80), "Fly Speed: " + FlySpeed);
        }
    }

    void Update()
    {
        //if(!GetComponent<Player_Controller>().isBrowserOn)
        //{
            if (!isFlying)
            {
                HandleHead();
                HandleHeight();
                CalculateMovement();
                //print("Not Flying");

                if (SteamVR_Actions._default.M_Forward.state)
                {
                    //c_Controller.transform.position += (transform.forward * GroundMovSpd * Time.deltaTime);
                    movement += orientation * (GroundMovSpd * Vector3.forward) * Time.deltaTime;
                    c_Controller.Move(movement);
                }
                else if (SteamVR_Actions._default.M_Backward.state)
                {
                    movement += orientation * (-GroundMovSpd * Vector3.forward) * Time.deltaTime;
                    c_Controller.Move(movement);
                }
                else if (SteamVR_Actions._default.M_Left.state)
                {
                    // print("left movement");
                    movement += orientation * (GroundMovSpd * Vector3.left) * Time.deltaTime;
                    c_Controller.Move(movement);
                    //c_Controller.transform.position += ((-transform.right) * GroundMovSpd * Time.deltaTime);
                }
                else if (SteamVR_Actions._default.M_Right.state)
                {
                    movement += orientation * (-GroundMovSpd * Vector3.left) * Time.deltaTime;
                    c_Controller.Move(movement);
                    //c_Controller.transform.position += (transform.right * GroundMovSpd * Time.deltaTime);
                }

            }


            if (SteamVR_Actions._default.ToggleFlying.state == true)
            {
                //print("Toggle Flight 1");
                isFlying = !isFlying;
                //print("Toggle Flight 2");
                //print("Fly Pressed = " + isFlying);


            }



            if (isFlying)
            {

                Vector3 leftDir = left.transform.position - head.position;
                Vector3 rightDir = right.transform.position - head.position;

                Vector3 dir = leftDir + rightDir;

                c_Controller.transform.position += (head.forward * FlySpeed * 0.2f * Time.deltaTime);



                //tweaking speed

                if (SteamVR_Actions._default.IncreaseFlyingSpd.stateDown && FlySpeed < MaxFlySpeed)
                {
                    FlySpeed += XflySpd;
                    print("inc fly Spd");
                }
                else if (SteamVR_Actions._default.DecreaseFlyingSPd.stateDown && FlySpeed > MinFlySpeed)
                {
                    FlySpeed -= XflySpd;
                    print("dec fly Spd");

                }
            }


            //Rotating Around Upward Axis
            if (!BigJump && !InverseBigJump)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) || SteamVR_Actions._default.MoveAround.axis.x > 0.0f)
                {
                    print("right rot");
                    GetComponent<CharacterController>().transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || SteamVR_Actions._default.MoveAround.axis.x < 0.0f)
                {
                    GetComponent<CharacterController>().transform.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
                    print("left rot");
                }

                //upward movement
                if (SteamVR_Actions._default.Move_Y_Axis.axis.y > 0.0f)
                {
                    print("trying to go up");
                    //float acc = SteamVR_Actions._default.Move_Y_Axis.
                    transform.position = new Vector3(transform.position.x, transform.position.y + (UpMoveSpd * Time.deltaTime), transform.position.z);
                }
                else if (SteamVR_Actions._default.Move_Y_Axis.axis.y < 0.0f)
                {
                    print("trying to go down");
                    transform.position = new Vector3(transform.position.x, transform.position.y - (UpMoveSpd * Time.deltaTime), transform.position.z);
                }
            }

            if (SteamVR_Actions._default.BigJump.state)
            {
                JumpPos = transform.position.y + (UpMoveSpd * BigJumpForce * Time.deltaTime);
                print("up jump pos = " + JumpPos);
                InverseBigJump = false;
                BigJump = true;
            }
            else if (SteamVR_Actions._default.InverseBigJump.state)
            {
                JumpPos = transform.position.y + (UpMoveSpd * -BigJumpForce * Time.deltaTime);
                print("down jump pos = " + JumpPos);
                BigJump = false;
                InverseBigJump = true;


            }


            //up
            if (BigJump && transform.position.y < JumpPos)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + (UpMoveSpd * BigJumpSmoothness * Time.deltaTime), transform.position.z);
                print("going up fast");
            }
            else
            {
                BigJump = false;
                JumpPos = 0f;
            }
            // down
            if (InverseBigJump && transform.position.y > JumpPos)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (UpMoveSpd * BigJumpSmoothness * Time.deltaTime), transform.position.z);
                print("going down fast");
            }
            else
            {
                InverseBigJump = false;
                JumpPos = 0f;
            }
        //}
    }


}
