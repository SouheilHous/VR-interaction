using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Valve.VR;
using System;
using JimmyGao;
using System.Linq;
using BayatGames.SaveGamePro.Extensions;
//using NUnit.Framework;

public class EditorXRVRBrushManager : MonoBehaviour
{
    public static EditorXRVRBrushManager instance;

    public GameObject goVRBrushManager;
    public GameObject leftHand;
    public GameObject rightHand;
    public BoxCollider prefabSelectionCube;
    public GameObject controllerLaserPointer;

    private GameObject handUI;
    private GameObject pen;
    private GameObject intersectionModule;
    private List<GameObject> selectedRenderers;
    private List<Vector3> selectedOffset;

    //For selection-moving handling
    private List<Quaternion> selectedLineRendererOriginalRotation;
    private bool isMovingSelection;
    private Vector3 rightHandPrevPosition;
    private Quaternion rightHandPrevRotation;
    private Quaternion selectedCubePrevRotation;


    private bool bIsActive;

    private SteamVR_Action_Boolean actionGrabGrip;
    private SteamVR_Action_Boolean actionTrigger;

    private Transform notifyScalingTransform;

    private List<BoxCollider> selectionCubes;

    private void Awake()
    {
        bIsActive = false;
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        bIsActive = false;

        selectedRenderers = new List<GameObject>();
        selectionCubes = new List<BoxCollider>();
        selectedOffset = new List<Vector3>();
        selectedLineRendererOriginalRotation = new List<Quaternion>();

        actionGrabGrip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
        actionTrigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    }

    public void Update()
    {
        if (handUI == null)
        {
            handUI = GameObject.Find("LeftHandUI");
        }
        if (pen == null)
        {
            pen = GameObject.Find("Pen");
        }
        /*if (controllerLaserPointer == null)
        {
            controllerLaserPointer = GameObject.Find("ControllerLaserPointer");
        }*/
        if (intersectionModule == null)
        {
            intersectionModule = GameObject.Find("IntersectionModule(Clone)");
        }

        if (bIsActive)
        {
            if (pen != null)
            {
                pen.transform.parent = rightHand.transform;
                pen.transform.localPosition = new Vector3(0f, pen.transform.localScale.y + 0.005f, 0.05f);
                //pen.transform.localPosition = Vector3.forward * 0.05f+Vector3.up*0.015f;
                pen.transform.localEulerAngles = Vector3.zero;
            }

            if (controllerLaserPointer != null)
            {
                controllerLaserPointer.transform.parent = rightHand.transform;
                controllerLaserPointer.transform.localPosition = Vector3.zero;
                controllerLaserPointer.transform.localEulerAngles = Vector3.zero;

                //Raycast for ScalingTool
                RaycastHit hitInfo;
                if ((Physics.Raycast(new Ray(controllerLaserPointer.transform.position+controllerLaserPointer.transform.forward, controllerLaserPointer.transform.forward), out hitInfo, 1000f, ~0, QueryTriggerInteraction.Collide )
                    && hitInfo.transform.GetComponent<LineRenderer>() != null)) 
                {
                    GameObject.FindGameObjectWithTag("ScalingToolManager").SendMessage("NotifyIntersectionTransformVRBrush", hitInfo.transform);
                    GameObject.FindGameObjectWithTag("ScalingToolManager").SendMessage("NotifyIntersectionTransformDistanceVRBrush", hitInfo.distance);
                    if (hitInfo.transform != null && hitInfo.transform.GetComponent<Renderer>() != null)
                        GameObject.FindGameObjectWithTag("ScalingToolManager").SendMessage("NotifyIntersectionTransformOffsetVRBrush", hitInfo.transform.GetComponent<Renderer>().bounds.center);
                } 
                else if (notifyScalingTransform != null)
                {
                    GameObject.FindGameObjectWithTag("ScalingToolManager").SendMessage("NotifyIntersectionTransformVRBrush", notifyScalingTransform);
                    if (hitInfo.transform != null && hitInfo.transform.GetComponent<Renderer>() != null)
                        GameObject.FindGameObjectWithTag("ScalingToolManager").SendMessage("NotifyIntersectionTransformOffsetVRBrush", notifyScalingTransform.GetComponent<Renderer>().bounds.center);
                    notifyScalingTransform = null;
                }                
                else
                {
                    GameObject.FindGameObjectWithTag("ScalingToolManager").SendMessage("NotifyIntersectionTransformNoneVRBrush");
                }
            }

            //selectionCube.gameObject.SetActive(false);

            if (selectedRenderers.Count > 0)
            {
                //selectionCube.gameObject.SetActive(true);

                bool bPressedGrabGripRightHand = actionGrabGrip.GetStateDown(SteamVR_Input_Sources.RightHand);
                bool bPressedGrabGripLeftHand = actionGrabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand);
                bool bPressedTriggerRightHand = actionTrigger.GetStateDown(SteamVR_Input_Sources.RightHand);
                bool bPressedTriggerLeftHand = actionTrigger.GetStateDown(SteamVR_Input_Sources.LeftHand);
                bool bReleasedGrabGripRightHand = !actionGrabGrip.GetState(SteamVR_Input_Sources.RightHand);
                bool bReleasedGrabGripLeftHand = !actionGrabGrip.GetState(SteamVR_Input_Sources.LeftHand);
                BoxCollider selectionCube = selectionCubes.FirstOrDefault(x => x.bounds.Contains(rightHand.transform.position));
                bool bHandIsInsideSelectionCube = selectionCube != null;

                //Moving
                if (bPressedGrabGripRightHand && bHandIsInsideSelectionCube)
                {
                    isMovingSelection = true;
                    rightHandPrevPosition = rightHand.transform.position;
                    rightHandPrevRotation = rightHand.transform.rotation;
                    if (selectionCubes.Count == 1)
                        selectedCubePrevRotation = selectionCubes[0].transform.rotation;
                }

                if (isMovingSelection)
                {
                    if (!bReleasedGrabGripRightHand)
                    {
                        Vector3 posOffset = rightHand.transform.position - selectionCube.transform.position;

                        foreach (BoxCollider b in selectionCubes)
                        {
                            b.transform.position = posOffset + b.transform.position;
                            b.transform.rotation = rightHand.transform.rotation * Quaternion.Inverse(rightHandPrevRotation) * selectedCubePrevRotation;
                        }
                    } else
                    {
                        isMovingSelection = false;
                    }
                }

                for (int i = 0; i < selectedRenderers.Count; i++)
                {
                    GameObject l = selectedRenderers[i];
                    l.transform.position = selectionCubes[i].transform.position - selectionCubes[i].transform.rotation * selectedOffset[i];
                    l.transform.rotation = selectionCubes[i].transform.rotation * selectedLineRendererOriginalRotation[i];
                }


                //Check if should duplicate
                if ((bPressedTriggerLeftHand && !bReleasedGrabGripRightHand) 
                    || (bPressedTriggerRightHand && !bReleasedGrabGripLeftHand)) {
                    foreach (GameObject l in selectedRenderers)
                    {
                        GameObject go = GameObject.Instantiate(l.gameObject);
                        go.transform.SetParent(l.transform.parent);
                    }
                }

                //Check if should scale
                if (!bReleasedGrabGripRightHand && !bReleasedGrabGripLeftHand && selectedRenderers.Count == 1)
                {
                    notifyScalingTransform = selectedRenderers[0].transform;
                }

                //Check if should delete
                if (SteamVR_Input.GetAction<SteamVR_Action_Vector2>("ThumbstickPosition").GetAxis(SteamVR_Input_Sources.LeftHand).y < -0.9f)
                {
                    foreach (GameObject l in selectedRenderers)
                    {
                        //Destroy(l);
                        l.SetActive(false);
                    }
                    ClearAllRenderers();
                }


                if (Selection.activeGameObject != null || (bPressedGrabGripRightHand && !bHandIsInsideSelectionCube))
                {
                    ClearAllRenderers();
                }
            }
            else
            {
                ClearAllSelectionCubes();
            }

        } else
        {
            if (pen != null)
            {
                pen.transform.parent = transform;
            }

            if (controllerLaserPointer != null)
            {
                controllerLaserPointer.transform.parent = transform;
            }

            ClearAllRenderers();
            ClearAllSelectionCubes();
        }



        if (goVRBrushManager != null && Application.isPlaying) 
            goVRBrushManager.GetComponentInChildren<JimmyGao.BrushManager>().gameObject.SetActive(bIsActive);        

        if (handUI != null && Application.isPlaying) 
            handUI.SetActive(bIsActive);

        if (pen != null) 
            pen.SetActive(bIsActive);

        if (controllerLaserPointer != null) 
            controllerLaserPointer.SetActive(bIsActive);

        /*if (intersectionModule != null) 
            intersectionModule.SetActive(!bIsActive);*/

        bIsActive = false;        
    }

    public void NotifyActive()
    {
        bIsActive = true;
    }

    public void SelectLine(LineRenderer lineRenderer)
    {
        if (BrushManager.Instance.BrushMode != 3)
        {
            ClearAllRenderers();
            ClearAllSelectionCubes();
        }

        if (lineRenderer != null)
        {
            if (!selectedRenderers.Contains(lineRenderer.gameObject))
            {
                selectedRenderers.Add(lineRenderer.gameObject);

                if (BrushManager.Instance.BrushMode == 2) //Erase Mode
                {
                    foreach (GameObject l in selectedRenderers)
                    {
                        //Destroy(l.gameObject);
                        l.SetActive(false);
                    }
                    ClearAllRenderers();
                    ClearAllSelectionCubes();

                    Selection.activeObject = null;
                }
                else if (BrushManager.Instance.BrushMode == 4) //Duplicate Mode
                {
                    foreach (GameObject l in selectedRenderers)
                    {
                        GameObject go = GameObject.Instantiate(l.gameObject);
                        go.transform.SetParent(l.transform.parent);
                        go.transform.position += Vector3.up * 0.1f;
                    }
                    ClearAllRenderers();
                    ClearAllSelectionCubes();

                    Selection.activeObject = null;
                }
                else //Select Mode
                {
                    // New Selection Cube
                    BoxCollider selectionCube = GameObject.Instantiate(prefabSelectionCube).GetComponent<BoxCollider>();

                    selectionCube.transform.localScale = lineRenderer.bounds.size;
                    selectionCube.transform.position = lineRenderer.bounds.center;
                    selectionCube.transform.rotation = Quaternion.identity;
                    selectionCube.gameObject.SetActive(true);
                    selectionCubes.Add(selectionCube);

                    selectedOffset.Add(lineRenderer.bounds.center - lineRenderer.transform.position);

                    selectedLineRendererOriginalRotation.Add(lineRenderer.transform.rotation);

                    Selection.activeObject = null;
                }
            }
            else
            {
                int index = selectedRenderers.IndexOf(lineRenderer.gameObject);
                selectedRenderers.RemoveAt(index);
                selectedOffset.RemoveAt(index);
                selectedLineRendererOriginalRotation.RemoveAt(index);
                Destroy(selectionCubes[index].gameObject);
                selectionCubes.RemoveAt(index);
            }
        }
        else
        {
            ClearAllSelectionCubes();
            ClearAllRenderers();
        }
    }

    public void SelectParticle(ParticleSystem particleSystem)
    {
        if (BrushManager.Instance.BrushMode != 5)
        {
            ClearAllRenderers();
            ClearAllSelectionCubes();
        }

        if (particleSystem != null)
        {
            if (!selectedRenderers.Contains(particleSystem.gameObject))
            {
                selectedRenderers.Add(particleSystem.gameObject);

                if (BrushManager.Instance.BrushMode == 2) //Erase Mode
                {
                    foreach (GameObject l in selectedRenderers)
                    {
                        l.SetActive(false);
                    }
                    ClearAllRenderers();
                    ClearAllSelectionCubes();

                    Selection.activeObject = null;
                }
                else if (BrushManager.Instance.BrushMode == 4) //Duplicate Mode
                {
                    foreach (GameObject l in selectedRenderers)
                    {
                        GameObject go = GameObject.Instantiate(l.gameObject);
                        go.transform.SetParent(l.transform.parent);
                        go.transform.position += Vector3.up * 0.1f;
                    }
                    ClearAllRenderers();
                    ClearAllSelectionCubes();

                    Selection.activeObject = null;
                }
                else //Select Mode
                {
                    // New Selection Cube
                    BoxCollider selectionCube = GameObject.Instantiate(prefabSelectionCube).GetComponent<BoxCollider>();

                    selectionCube.transform.localScale = particleSystem.GetComponent<BoxCollider>().size * particleSystem.transform.localScale.x;
                    selectionCube.transform.position = particleSystem.transform.position;
                    selectionCube.transform.rotation = Quaternion.identity;
                    selectionCube.gameObject.SetActive(true);
                    selectionCubes.Add(selectionCube);

                    selectedOffset.Add(Vector3.zero);

                    selectedLineRendererOriginalRotation.Add(particleSystem.transform.rotation);

                    Selection.activeObject = null;
                }
            }
            else
            {
                int index = selectedRenderers.IndexOf(particleSystem.gameObject);
                selectedRenderers.RemoveAt(index);
                selectedOffset.RemoveAt(index);
                selectedLineRendererOriginalRotation.RemoveAt(index);
                Destroy(selectionCubes[index].gameObject);
                selectionCubes.RemoveAt(index);
            }
        }
        else
        {
            ClearAllSelectionCubes();
            ClearAllRenderers();
        }
    }

    private void ClearAllSelectionCubes()
    {
        foreach (BoxCollider b in selectionCubes)
        {
            Destroy(b.gameObject);
        }
        selectionCubes.Clear();
    }

    private void ClearAllRenderers()
    {
        selectedRenderers.Clear();
        selectedOffset.Clear();
        selectedLineRendererOriginalRotation.Clear();
    }

    public bool IsActive()
    {
        return bIsActive;
    }
}
