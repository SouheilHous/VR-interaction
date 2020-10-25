using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class MultiTool3DFloatInput : MonoBehaviour
{

    /*protected Keyboard m_Keyboard;
    public Func<KeyboardUI> spawnKeyboard { private get; set; }*/

    public void OnClick()
    {
        OpenKeyboard();

    }
    /// <summary>
    /// Open a keyboard for this input field
    /// </summary>
    public virtual void OpenKeyboard()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OpenKeyboard();
        }
    }
}
