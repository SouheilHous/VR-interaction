using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRExInputModule : StandaloneInputModule
{
    static VRExInputModule instance
        ;
    static bool disableOtherInputModulesOnStart = true;
    public static VRExInputModule Instance
    {
        get
        {
            if (instance == null)
                instance = EnableInputModule<VRExInputModule>();

            return instance;
        }
        private set { instance = value; }
    }

    GameObject currentDragging;
    GameObject currentPointedAt;
    float dragThreshold = 10.0f;
    bool pressedDown = false;
    bool pressedLastFrame = false;

    Ray customControllerRay;
    protected override void Awake()
    {
        if (!Application.isPlaying) return;

        Instance = this;
        base.Awake();
    }
    // Use this for initialization
    protected override void Start()
    {
        if (!Application.isPlaying) return;

        base.Start();
    }
    public static Ray CustomControllerRay
    {
        get { return Instance.customControllerRay; }
        set { Instance.customControllerRay = value; }
    }
    // Update is called once per frame
    void Update () {
		
	}

    public override void Process()
    {
        ProcessCustomRayController();
    }

    protected virtual void ProcessCustomRayController()
    {

        var mouseData = GetMousePointerEventData(0);
        PointerEventData eventData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData;


        // send update events if there is a selected object - this is important for InputField to receive keyboard events
        SendUpdateEventToSelectedObject();

        // see if there is a UI element that is currently being pointed at
        PointerEventData ControllerData = eventData;

        currentPointedAt = ControllerData.pointerCurrentRaycast.gameObject;

        ProcessDownRelease(ControllerData, (pressedDown && !pressedLastFrame), (!pressedDown && pressedLastFrame));

        //Process move and drag if trigger is pressed
        ProcessMove(ControllerData);
        if (pressedDown)
        {
            ProcessDrag(ControllerData);

            if (!Mathf.Approximately(ControllerData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(ControllerData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, ControllerData, ExecuteEvents.scrollHandler);
            }
        }



        //save button state for this frame
        pressedLastFrame = pressedDown;
    }

    public static bool CustomControllerButtonDown
    {
        get { return Instance.pressedDown; }
        set { Instance.pressedDown = value; }
    }
    /// <summary>
    /// Sends trigger down / trigger released events to gameobjects under the pointer.
    /// </summary>
    protected virtual void ProcessDownRelease(PointerEventData eventData, bool down, bool released)
    {
        var currentOverGo = eventData.pointerCurrentRaycast.gameObject;

        // PointerDown notification
        if (down)
        {
            eventData.eligibleForClick = true;
            eventData.delta = Vector2.zero;
            eventData.dragging = false;
            eventData.useDragThreshold = true;
            eventData.pressPosition = eventData.position;
            eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, eventData);

            if (eventData.pointerEnter != currentOverGo)
            {
                // send a pointer enter to the touched element if it isn't the one to select...
                HandlePointerExitAndEnter(eventData, currentOverGo);
                eventData.pointerEnter = currentOverGo;
            }

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);


            float time = Time.unscaledTime;

            if (newPressed == eventData.lastPress)
            {
                var diffTime = time - eventData.clickTime;
                if (diffTime < 0.3f)
                    ++eventData.clickCount;
                else
                    eventData.clickCount = 1;

                eventData.clickTime = time;
            }
            else
            {
                eventData.clickCount = 1;
            }

            eventData.pointerPress = newPressed;
            eventData.rawPointerPress = currentOverGo;

            eventData.clickTime = time;

            // Save the drag handler as well
            eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (eventData.pointerDrag != null)
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
        }

        // PointerUp notification
        if (released)
        {
            ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
            {
                ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
                //Debug.Log("click");
            }
            else if (eventData.pointerDrag != null && eventData.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
                //Debug.Log("drop");
            }

            eventData.eligibleForClick = false;
            eventData.pointerPress = null;
            eventData.rawPointerPress = null;

            if (eventData.pointerDrag != null && eventData.dragging)
            {
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
                //Debug.Log("end drag");
            }

            eventData.dragging = false;
            eventData.pointerDrag = null;

            // send exit events as we need to simulate this on touch up on touch device
            ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerExitHandler);
            eventData.pointerEnter = null;
        }


    }
    static T EnableInputModule<T>() where T : BaseInputModule
    {
        bool moduleMissing = true;
        EventSystem eventGO = GameObject.FindObjectOfType<EventSystem>();

        if (eventGO == null)
        {
            Debug.LogError("Your EventSystem component is missing from the scene! Unity Canvas will not track interactions without it.");
            return null as T;
        }

        foreach (BaseInputModule module in eventGO.GetComponents<BaseInputModule>())
        {
            if (module is T)
            {
                moduleMissing = false;
                module.enabled = true;
            }
            else if (disableOtherInputModulesOnStart)
                module.enabled = false;
        }

        if (moduleMissing)
            eventGO.gameObject.AddComponent<T>();

        return eventGO.GetComponent<T>();
    }
}
