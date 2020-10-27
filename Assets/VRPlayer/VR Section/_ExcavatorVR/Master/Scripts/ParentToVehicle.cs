using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(TeleportPoint))]
public class ParentToVehicle : MonoBehaviour
{
    private TeleportPoint _teleportPoint;
    [SerializeField]
    private Transform childObject = null;
    [SerializeField]
    private Transform parentObject = null;

    [SerializeField]
    private Transform targetTransform = null;

    [SerializeField]
    private ExcavatorCollision collisionManager = null;

    [SerializeField]
    private bool enterTrigger = false;
    [SerializeField]
    private bool exitTrigger = false;

    [SerializeField]
    private bool movePlayer = false;

    [SerializeField]
    private GameObject player;


    public SteamVR_Input_Sources input_Sources;

    private void Awake()
    {
        _teleportPoint = GetComponent<TeleportPoint>();
    }

    void Start()
    {
        Teleport.Player.AddListener(OnTeleported);
    }

    private void OnTeleported(TeleportMarkerBase marker)
    {
        TeleportPoint point = marker as TeleportPoint;
        if (point != null && point == _teleportPoint)
        {
            childObject.parent = parentObject;
        }
        else
            childObject.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (enterTrigger && other.tag == "Player")
        {
            if (movePlayer && SteamVR_Actions._default.InteractUI.GetStateDown(input_Sources) == true)
            {
                collisionManager.DisableColliders();
                other.transform.position = targetTransform.position;
                other.transform.parent = parentObject;
                ResetPosition(targetTransform);
            }
        }
    }

    //Playmaker
    public void MovePlayerInside()
    {
        if (player != null)
        {
            collisionManager.DisableColliders();
            player.transform.position = targetTransform.position;
            player.transform.parent = parentObject;
            ResetPosition(targetTransform);
        }
    }

    IEnumerator Move(GameObject target, Vector3 source, Vector3 targetPosition, float overTime)
    {
        float startTime = Time.time;
        while(Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(source, targetPosition, (Time.time - startTime)/overTime);
            yield return null;
        }
        target.transform.position = targetPosition;
    }

    private void ResetPosition(Transform desiredHeadPos)
    {

        float offsetAngle = Camera.main.transform.rotation.eulerAngles.y;

        //now rotate CameraRig in opposite direction to compensate
        // Player.instance.transform.Rotate(0f, -offsetAngle, 0f);

        //My player position was inverted after the offset angle so I tiwst 180 degrees
        //Player.instance.transform.Rotate(0f, 180, 0f);

        //{now position}
        //calculate postional offset between CameraRig and Camera
        Vector3 offsetPos = Camera.main.transform.position - Player.instance.transform.position;
        //reposition CameraRig to desired position minus offset

        Vector3 finalPos = (desiredHeadPos.position - offsetPos);

        Vector3 targetPosition = new Vector3(finalPos.x, Player.instance.transform.position.y, finalPos.z);

        StartCoroutine(Move(Player.instance.gameObject, Player.instance.transform.position,targetPosition,10));

        Debug.Log("Player recentered!");

    }

    private void OnTriggerExit(Collider other)
    {
        if (exitTrigger && other.tag == "Player")
        {
        }
    }
}
