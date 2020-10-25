using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorXRUltimateRadialMenu : MonoBehaviour
{
    public UltimateRadialMenu radialMenu;
    public GameObject goHandMenu;
    public GameObject goCamera;

    [Header("Initialization")]
    public GameObject prefabRadialMenuCamera;
    public Transform canvasParent;

    public static EditorXRUltimateRadialMenu instance;

    private bool bIsActive;
    private bool bIsActivePrev;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        bIsActive = false;

        //Spawn Radial Menu
        GameObject g = GameObject.Instantiate(prefabRadialMenuCamera, radialMenu.transform);
        g.transform.localPosition = Vector3.back * 5f;
        goCamera = g.GetComponentInChildren<Camera>(true).gameObject;        
    }

    public void Update()
    {
        bIsActivePrev = bIsActive;

        goHandMenu.transform.localScale = (bIsActive ? Vector3.one : Vector3.zero);

        if (bIsActive)
        {
            radialMenu?.EnableRadialMenu();
            UltimateRadialMenuInputHandler.instance.tQuad.gameObject.SetActive(true);
            goCamera.SetActive(true);
        } else
        {
            radialMenu?.DisableRadialMenu();
            UltimateRadialMenuInputHandler.instance.tQuad.gameObject.SetActive(false);
            goCamera.SetActive(false);
        }

        bIsActive = false;        
    }

    public void NotifyActive()
    {
        bIsActive = true;
    }

    public bool IsActive()
    {
        return bIsActivePrev;
    }
}
