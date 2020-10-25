using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class EditorXRHierarchyMenu : MonoBehaviour
{
    public GameObject goHandMenu;
    public GameObject goHandMenuContainer;
    public Image imgLocked;
    public Image imgUnlocked;
    public Image imgMove;

    public static EditorXRHierarchyMenu instance;

    private bool bIsActive;
    private bool bIsActivePrev;

    private bool bIsLocked;
    private Vector3 lockedPosition;
    private Quaternion lockedRotation;

    private bool bIsMoving;
    private Vector3 movePosition;

    private Transform tOriginalParent;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        bIsActive = false;

        tOriginalParent = goHandMenu.transform.parent;
    }

    public void Update()
    {
        bIsActivePrev = bIsActive;

        bool bIsActiveNow = bIsActive || bIsLocked;

        goHandMenuContainer.SetActive(bIsActiveNow);
        goHandMenu.transform.localScale = (bIsActiveNow ? Vector3.one : Vector3.zero);
        
        bIsActive = false;
    }

    public void LateUpdate()
    {
        if (bIsLocked)
        {
            bIsMoving &= SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI").GetState(SteamVR_Input_Sources.RightHand);

            if (bIsMoving)
            {
                lockedPosition = Player.instance.rightHand.transform.TransformPoint(movePosition);
                lockedRotation = Quaternion.LookRotation(-(Player.instance.transform.position - lockedPosition), Vector3.up);
            }

            if (goHandMenu.transform.parent != transform)
            {
                goHandMenu.transform.SetParent(transform);
            }
            goHandMenu.transform.position = lockedPosition;
            goHandMenu.transform.rotation = lockedRotation;
        }
        else
        {
            if (goHandMenu.transform.parent != tOriginalParent)
            {
                goHandMenu.transform.SetParent(tOriginalParent);
            }
            goHandMenu.transform.localPosition = new Vector3(0f, 0.02f, 0.1f);
            goHandMenu.transform.localRotation = Quaternion.identity;
        }

        imgLocked.enabled = bIsLocked;
        imgUnlocked.enabled = !bIsLocked;
        imgMove.transform.parent.gameObject.SetActive(bIsLocked);
    }

    public void NotifyActive()
    {
        bIsActive = true;

        if (!bIsActivePrev && !bIsLocked)
            ToggleLock();
    }

    public bool IsActive()
    {
        return bIsActivePrev;
    }

    public void ToggleLock()
    {
        bIsLocked = !bIsLocked;
        if (bIsLocked)
        {
            lockedPosition = Player.instance.transform.InverseTransformPoint(goHandMenu.transform.position);
            lockedRotation = goHandMenu.transform.rotation;
        }
    }

    public void StartMove()
    {
        bIsMoving = true;
        movePosition = Player.instance.rightHand.transform.InverseTransformPoint(goHandMenu.transform.position);
    }

    public bool IsLocked()
    {
        return bIsLocked;
    }
}
