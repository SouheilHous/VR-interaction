using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class SP_Controller : MonoBehaviour {
	//the object that the trigger entered
	private GameObject obj;
    public string inputButtonName = "TriggerButton";

    void Start () {

	}

    void Update () {
        if (obj) {
            
			if(Input.GetButtonDown(inputButtonName))
                GrabObject();                

            if (Input.GetButton(inputButtonName))
                HoldObject ();

            if (Input.GetButtonUp(inputButtonName))
                LetGoOfObject ();
		}
	}

    void GrabObject()
    {
        obj.SendMessage("grab", gameObject.transform.rotation);
    }

    void HoldObject()
    {
        obj.SendMessage("hold", gameObject.transform.rotation);

    }

    void LetGoOfObject()
    {
        obj.SendMessage("letGo", gameObject.transform.rotation);
    }

    void OnTriggerStay(Collider other){
		if (other.tag == "SP") { //object is interactable
			obj = other.gameObject;
		}
	}

	void OnTriggerExit(Collider other){
		obj = null;
	}

	
}
