using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace JimmyGao
{
    public class VRExUIRaycaster : GraphicRaycaster
    {


        [SerializeField]
        bool showDebug = false;

        bool pointingAtCanvas = false;

        Canvas myCanvas;
        bool overrideEventData = true;
        List<GameObject> objectsUnderPointer = new List<GameObject>();

        Vector3 cyllinderMidPoint;
        GameObject colliderContainer;
        // Use this for initialization

        int cyangle = 1;

        public Vector2 remap;


        // Update is called once per frame
        void Update()
        {

        }

        protected override void Awake()
        {
            base.Awake();
            myCanvas = GetComponent<Canvas>();



            //the canvas needs an event camera set up to process events correctly. Try to use main camera if no one is provided.
            if (myCanvas.worldCamera == null && Camera.main != null)
                myCanvas.worldCamera = Camera.main;
        }
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {


            //check if we have a world camera to process events by
            if (myCanvas.worldCamera == null)
                Debug.LogWarning(" UI Raycaster requires Canvas to have a world camera reference to process events!", myCanvas.gameObject);

            Camera worldCamera = myCanvas.worldCamera;
            Ray ray3D;
            ray3D = VRExInputModule.CustomControllerRay;
            //get a ray to raycast with depending on the control method
            UpdateSelectedObjects(eventData);


            //Create a copy of the eventData to be used by this canvas. This allows
            PointerEventData newEventData = new PointerEventData(EventSystem.current);
            if (!overrideEventData)
            {
                newEventData.pointerEnter = eventData.pointerEnter;
                newEventData.rawPointerPress = eventData.rawPointerPress;
                newEventData.pointerDrag = eventData.pointerDrag;
                newEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
                newEventData.pointerPressRaycast = eventData.pointerPressRaycast;
                newEventData.hovered = new List<GameObject>();
                newEventData.hovered.AddRange(eventData.hovered);
                newEventData.eligibleForClick = eventData.eligibleForClick;
                newEventData.pointerId = eventData.pointerId;
                newEventData.position = eventData.position;
                newEventData.delta = eventData.delta;
                newEventData.pressPosition = eventData.pressPosition;
                newEventData.clickTime = eventData.clickTime;
                newEventData.clickCount = eventData.clickCount;
                newEventData.scrollDelta = eventData.scrollDelta;
                newEventData.useDragThreshold = eventData.useDragThreshold;
                newEventData.dragging = eventData.dragging;
                newEventData.button = eventData.button;
            }
            //int myLayerMask = -1;
            int myLayerMask = 1 << 5;
            Vector2 remappedPosition = eventData.position;
            
            if (!RaycastToCyllinderCanvas(ray3D, out remappedPosition, false, myLayerMask)) return;
            pointingAtCanvas = true;
            PointerEventData eventDataToUse = overrideEventData ? eventData : newEventData;

            // Swap event data pressPosition to our remapped pos if this is the frame of the press
            if (eventDataToUse.pressPosition == eventDataToUse.position)
                eventDataToUse.pressPosition = remappedPosition;

            // Swap event data position to our remapped pos
            eventDataToUse.position = remappedPosition;
            remap = remappedPosition;



            //store objects under pointer so they can quickly retrieved if needed by other scripts
            objectsUnderPointer = eventData.hovered;

            // Use base class raycast method to finish the raycast if we hit anything
            base.Raycast(overrideEventData ? eventData : newEventData, resultAppendList);

        }

        public virtual bool RaycastToCyllinderCanvas(Ray ray3D, out Vector2 o_canvasPos, bool OutputInCanvasSpace = false, int myLayerMask = -1)
        {

            if (showDebug)
            {
                Debug.DrawLine(ray3D.origin, ray3D.GetPoint(1000), Color.red);
            }

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray3D, out hit, float.PositiveInfinity, myLayerMask))
            {
                //find if we hit this canvas - this needs to be uncommented
                if (overrideEventData && hit.collider.gameObject != this.gameObject && (colliderContainer == null || hit.collider.transform.parent != colliderContainer.transform))
                {
                    o_canvasPos = Vector2.zero;
                    return false;
                }

                //direction from the cyllinder center to the hit point
                Vector3 localHitPoint = myCanvas.transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point);
                Vector3 directionFromCyllinderCenter = (localHitPoint - cyllinderMidPoint).normalized;

                //angle between middle of the projected canvas and hit point direction
                float angle = -AngleSigned(directionFromCyllinderCenter.ModifyY(0), cyangle < 0 ? Vector3.back : Vector3.forward, Vector3.up);

                //convert angle to canvas coordinates
                Vector2 canvasSize = myCanvas.GetComponent<RectTransform>().rect.size;

                //map the intersection point to 2d point in canvas space
                Vector2 pointOnCanvas = new Vector3(0, 0, 0);
                pointOnCanvas.x = localHitPoint.x;//angle.Remap(-cyangle / 2.0f, cyangle / 2.0f, -canvasSize.x / 2.0f, canvasSize.x / 2.0f);
                pointOnCanvas.y = localHitPoint.y;


                if (OutputInCanvasSpace)
                    o_canvasPos = pointOnCanvas;
                else //convert the result to screen point in camera. This will be later used by raycaster and world camera to determine what we're pointing at
                    o_canvasPos = myCanvas.worldCamera.WorldToScreenPoint(myCanvas.transform.localToWorldMatrix.MultiplyPoint3x4(pointOnCanvas));

                if (showDebug)
                {
                    Debug.DrawLine(hit.point, hit.point.ModifyY(hit.point.y + 10), Color.green);
                    Debug.DrawLine(hit.point, myCanvas.transform.localToWorldMatrix.MultiplyPoint3x4(cyllinderMidPoint), Color.yellow);
                }

                return true;
            }

            o_canvasPos = Vector2.zero;
            return false;
        }

        protected void UpdateSelectedObjects(PointerEventData eventData)
        {

            //deselect last object if we moved beyond it
            bool selectedStillUnderGaze = false;
            foreach (GameObject go in eventData.hovered)
            {
                if (go == eventData.selectedObject)
                {
                    selectedStillUnderGaze = true;
                    break;
                }
            }
            if (!selectedStillUnderGaze) eventData.selectedObject = null;


            //find new object to select in hovered objects
            foreach (GameObject go in eventData.hovered)
            {
                if (go == null) continue;

                //go through only go that can be selected and are drawn by the canvas
                Graphic gph = go.GetComponent<Graphic>();
#if UNITY_5_1
                        if (go.GetComponent<Selectable>() != null && gph != null && gph.depth != -1)
#else
                if (go.GetComponent<Selectable>() != null && gph != null && gph.depth != -1 && gph.raycastTarget)
#endif
                {
                    if (eventData.selectedObject != go)
                        eventData.selectedObject = go;
                    break;
                }
            }

            //Test for selected object being dragged and initialize dragging, if needed.
            //We do this here to trick unity's StandAloneInputModule into thinking we used a touch or mouse to do it.
            /*
            if (eventData.IsPointerMoving() && eventData.pointerDrag != null
                && !eventData.dragging
                && ShouldStartDrag(eventData.pressPosition, eventData.position, EventSystem.current.pixelDragThreshold, eventData.useDragThreshold))
            {
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
                eventData.dragging = true;
            }*/
        }

        float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }
    }
}