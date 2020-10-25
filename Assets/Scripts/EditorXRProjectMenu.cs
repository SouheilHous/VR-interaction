using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Valve.VR;

public class EditorXRProjectMenu : MonoBehaviour
{
    public GameObject goHandMenu;
    public GameObject goHandMenuContainer;
    public GameObject goRightHandPoint;

    public static EditorXRProjectMenu instance;

    private bool bIsActive;
    private bool bIsActivePrev;

    private GameObject goDrag;
    private Transform tScaleToOne;
    private ProjectMenuItem projectMenuItem;

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
        

        if (goDrag != null)
        {
            if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI").GetStateUp(SteamVR_Input_Sources.Any))
            {
                EndDrag();
                goDrag = null;
            } else
            {
                goDrag.transform.position = Vector3.Lerp(goDrag.transform.position, goRightHandPoint.transform.position, Time.deltaTime * 10f);
                goDrag.transform.rotation = goRightHandPoint.transform.rotation;
            }
        }

        if (tScaleToOne != null)
        {
            tScaleToOne = null;

            /* Disabling this nice animation
            tScaleToOne.localScale = Vector3.Lerp(tScaleToOne.localScale, Vector3.one, Time.deltaTime * 10f);
            if (tScaleToOne.localScale == Vector3.one)
            {
                tScaleToOne = null;
            }*/
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

    public void StartDragPrefab(GameObject gameObject, ProjectMenuItem projectMenuItem)
    {
        goDrag = gameObject.transform.GetChild(0).gameObject;
        goDrag.transform.SetParent(null);
        this.projectMenuItem = projectMenuItem;
    }

    public bool IsDragging()
    {
        return goDrag != null;
    }

    private void EndDrag()
    {
        goDrag.transform.SetParent(transform);

        if (goDrag.GetComponent<LODGroup>() != null)
        {
            goDrag.GetComponent<LODGroup>().enabled = true;
        }

        //goDrag.transform.localScale = Vector3.one;
        /*if (tScaleToOne != null)
        {
            tScaleToOne.localScale = Vector3.one;
        }*/
        tScaleToOne = goDrag.transform;

        projectMenuItem.UpdatePath();

    }
}
