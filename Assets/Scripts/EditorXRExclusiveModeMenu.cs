using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorXRExclusiveModeMenu : MonoBehaviour
{
    public GameObject goHandMenuContainer;

    public static EditorXRExclusiveModeMenu instance;

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
