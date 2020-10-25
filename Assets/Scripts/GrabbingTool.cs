using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Valve.VR;

public class GrabbingTool : MonoBehaviour
{
    public Transform rightHand;

    private bool bMoving;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Selection.activeGameObject != null)
        {
            if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetStateDown(SteamVR_Input_Sources.RightHand)
                && Vector3.Distance(Selection.activeGameObject.transform.position, rightHand.transform.position) < 0.1f
                )
            {
                bMoving = true;
            } else if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetStateUp(SteamVR_Input_Sources.RightHand))
            {
                bMoving = false;
            }

            if (bMoving)
            {
                Selection.activeGameObject.transform.position = rightHand.transform.position;
            }
        } else
        {
            bMoving = false;
        }
    }
}
