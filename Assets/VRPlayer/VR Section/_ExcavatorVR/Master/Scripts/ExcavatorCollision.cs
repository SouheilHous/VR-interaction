using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ExcavatorCollision : MonoBehaviour
{
    public bool isFlying = false;
    private bool isInsideVehicle = false;
    [SerializeField]
    private List<GameObject> colliders;

    [SerializeField]
    private Transform targetTransform = null;

    [SerializeField]
    private GameObject player = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Actions._default.ToggleFlying.state == true)
        {
            isFlying = !isFlying;
        }
    }

    public void DisableColliders()
    {
        isInsideVehicle = true;
        foreach (GameObject collider in colliders)
        {
            collider.SetActive(false);
        }

    }

    public void EnableColliders()
    {
        isInsideVehicle = false;
        foreach (GameObject collider in colliders)
        {
            collider.SetActive(true);
        }
        player.transform.parent = null;
        player.transform.rotation  = Quaternion.Euler(0f, player.transform.rotation.eulerAngles.y, 0f);

    }

    public bool IsFlying()
    {
        return isFlying;
    }

    //Playmaker
    public void MovePlayerInside()
    {
        Debug.Log("Move player Inside");
        if (player != null)
        {
            DisableColliders();
            player.transform.parent = this.transform;
            ResetPosition(targetTransform);
        }
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
        Vector3 offsetPos = Camera.main.transform.position - player.transform.position;
        //reposition CameraRig to desired position minus offset

        Vector3 finalPos = (desiredHeadPos.position - offsetPos);

        Vector3 targetPosition = new Vector3(finalPos.x, player.transform.position.y, finalPos.z);

        StartCoroutine(Move(player, player.transform.position,targetPosition,1));
    }

    IEnumerator Move(GameObject target, Vector3 source, Vector3 targetPosition, float overTime)
    {
        float startTime = Time.time;
        while(Time.time < startTime + overTime)
        {
            target.transform.position = Vector3.Lerp(source, targetPosition, (Time.time - startTime)/overTime);
            Debug.Log("Player Moving");
            yield return null;
        }
        target.transform.position = targetPosition;
         Debug.Log("Player recentered!");

    }
}
