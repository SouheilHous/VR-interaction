using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEditor.Experimental.EditorVR.Core;

using UnityObject = UnityEngine.Object;
using UnityEngine.XR;
using Valve.VR;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class switchEditor : MonoBehaviour
{
    public UnityObject[] m_DefaultContext;
    public EditingContextManager m_DefaultContextCurrent;
    public bool clicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Actions._default.GrabPinch.stateDown)
        {
            if (clicked != true)
            {
                Debug.Log("Switched");
                if (m_DefaultContextCurrent.m_DefaultContext != m_DefaultContext[0])
                {
                    m_DefaultContextCurrent.m_DefaultContext = m_DefaultContext[0];

                    clicked = true;
                }
                else
                {
                    m_DefaultContextCurrent.m_DefaultContext = m_DefaultContext[1];

                    clicked = true;
                }
            }
           
            
        }
        else
        {
            clicked = false;
        }
    }
}
