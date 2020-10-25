using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAdministration : MonoBehaviour {

    public bool Visibility;

    public bool Locked = false;

	void Update ()
    {
        Cursor.visible = Visibility;

        if (Locked)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
