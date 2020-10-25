using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorXRPlayer : MonoBehaviour
{
    private GameObject goCameraRig;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (goCameraRig == null)
        {
            goCameraRig = GameObject.Find("VRCameraRig");
        }

        transform.position = goCameraRig.transform.position;
        transform.rotation = goCameraRig.transform.rotation;

    }
}
