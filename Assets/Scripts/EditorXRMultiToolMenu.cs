using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorXRMultiToolMenu : MonoBehaviour
{
    public GameObject goHandMenu;
    public GameObject goHandMenuContainer;

    public static EditorXRMultiToolMenu instance;

    private bool bIsActive;
    private bool bIsActivePrev;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        bIsActive = false;
    }

    public void Update()
    {
        bIsActivePrev = bIsActive;

        goHandMenuContainer.SetActive(bIsActive);

        goHandMenu.transform.localScale = (bIsActive ? Vector3.one : Vector3.zero);
        
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
