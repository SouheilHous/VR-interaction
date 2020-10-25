/* UltimateRadialMenuInputManager.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Valve.VR;

public class UltimateRadialMenuInputManager : MonoBehaviour
{
	public static UltimateRadialMenuInputManager Instance
	{
		get;
		private set;
	}

	// INTERACT SETTINGS //
	public enum InvokeAction
	{
		OnButtonDown,
		OnButtonClick
	}
	[Header( "Interact Settings" )]
	[Tooltip( "The action required to invoke the radial button." )]
	public InvokeAction invokeAction = InvokeAction.OnButtonDown;
	[Tooltip( "Determines whether or not the Ultimate Radial Menu will receive input when the Ultimate Radial Menu is released and disabled." )]
	public bool onMenuRelease = false;
	[Tooltip( "Determines if the Ultimate Radial Menu should be disabled when the interaction occurs." )]
	public bool disableOnInteract = false;

	// MOUSE SETTINGS //
	[Header( "Mouse and Keyboard Settings" )]
	[Tooltip( "Determines if mouse and keyboard input should be used to send to the Ultimate Radial Menu." )]
	public bool keyboardInput = true;
	[Tooltip( "The mouse button index to use for interacting." )]
	public int mouseButtonIndex = 0;
	[Tooltip( "The input key used for enabling and disabling the Ultimate Radial Menu." )]
	public string enableButtonKeyboard = "Submit";

	// CONTROLLER SETTINGS //
	[Header( "Controller Settings" )]
	[Tooltip( "Determines if controller input should be used to send to the Ultimate Radial Menu." )]
	public bool controllerInput = false;
	[Tooltip( "The input key for the controller horizontal axis." )]
	public string horizontalAxisController = "Horizontal";
	[Tooltip( "The input key for the controller vertical axis." )]
	public string verticalAxisController = "Vertical";
	[Tooltip( "The input key for the controller button interaction." )]
	public string interactButtonController = "Cancel";
	[Tooltip( "The input key used for enabling and disabling the Ultimate Radial Menu." )]
	public string enableButtonController = "Submit";

	// CUSTOM INPUT SETTINGS //
	[Header( "Custom Input Settings" )]
	public bool customInput = false;

	public class UltimateRadialMenuInfomation
	{
		public UltimateRadialMenu radialMenu;
		public bool lastRadialMenuState = false;
	}
	public List<UltimateRadialMenuInfomation> UltimateRadialMenuInformations
	{
		get;
		private set;
	}


	void Awake ()
	{
		// If this input manager is not located on the event system...
		if( !GetComponent<EventSystem>() )
		{
			// If the event system in the scene does not have a input manager, then add one to it.
			if( !FindObjectOfType<EventSystem>().GetComponent<UltimateRadialMenuInputManager>() )
				FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();

			// Destroy this component and return.
			Destroy( this );
			return;
		}

		// Assign the instance as this.
		Instance = this;
	}

	void Start ()
	{
		// Create a new list for the radial menus.
		UltimateRadialMenuInformations = new List<UltimateRadialMenuInfomation>();

		// Find all the radial menus in the scene.
		UltimateRadialMenu[] allRadialMenus = FindObjectsOfType<UltimateRadialMenu>();

		// Loop through all the radial menus and assign the information.
		for( int i = 0; i < allRadialMenus.Length; i++ )
			UltimateRadialMenuInformations.Add( new UltimateRadialMenuInfomation() { radialMenu = allRadialMenus[ i ] } );
	}

	void Update ()
	{
		// Loop through each of the radial menus.
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			// Booleans to check if we want to enable or disable the radial menu this frame.
			bool enableMenu = false;
			bool disableMenu = false;
			bool inputDown = false;
			bool inputUp = false;

			// This is for the current input of the selected Input Type. ( Mouse input for Keyboard controls, and joystick input for controllers )
			Vector2 input = Vector2.zero;

			// This will store the distance from the center of the radial menu to help calculate if the input is within range.
			float distance = 0.0f;

			// If the user wants to use keyboard input then run the MouseAndKeyboardInput function.
			if( keyboardInput )
				MouseAndKeyboardInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user wants to use controller input then run the ControllerInput function.
			if( controllerInput )
				ControllerInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			if( customInput )
				CustomInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If we want to activate the radial menu when we release the menu when hovering over a button...
			if( onMenuRelease )
			{
				// Check the last known radial menu state to see if it was active. If we are going to disable the menu on this frame and the last known state was true, then set interact to true.
				if( UltimateRadialMenuInformations[ i ].lastRadialMenuState == true && disableMenu == true )
					inputDown = inputUp = true;
			}

			// Send all of the calculations to the Ultimate Radial Menu to process.
			UltimateRadialMenuInformations[ i ].radialMenu.ProcessInput( input, distance, inputDown, inputUp );

			// If we want to enable the radial menu on this frame then do that here.
			if( enableMenu )
				UltimateRadialMenuInformations[ i ].radialMenu.EnableRadialMenu();

			// Same this for the disable. Do that here.
			if( disableMenu )
				UltimateRadialMenuInformations[ i ].radialMenu.DisableRadialMenu();

			// Store the last known state for calculations.
			UltimateRadialMenuInformations[ i ].lastRadialMenuState = UltimateRadialMenuInformations[ i ].radialMenu.RadialMenuActive;
		}
	}

	/// <summary>
	/// This function will catch input from the Mouse and Keyboard and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void MouseAndKeyboardInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// Store the mouse position.
		Vector2 mPosition = new Vector2( Input.mousePosition.x, Input.mousePosition.y );

		// By subtracting the mouse position from the radial menu's position we get a relative number. Then we divide by the height of the screen space to give us an easier and more consistent number to work with.
		Vector2 modInput = ( mPosition - UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BasePosition ) / Screen.height;

		// If there is a mouse present...
		if( Input.mousePresent )
		{
			// Apply our new calculated input.
			input = modInput;

			// Configure the distance of the mouse position from the Radial Menu's base position.
			distance = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BasePosition, mPosition );
		}
		
		// Check for the mouse button being pressed down, and if so set activate to true.
		if( Input.GetMouseButtonDown( mouseButtonIndex ) )
			inputDown = true;
		if( Input.GetMouseButtonUp( mouseButtonIndex ) )
			inputUp = true;

		// If the user has a enable key assigned...
		if( enableButtonKeyboard != string.Empty )
		{
			// Check for the Enable and Disable button keys and set the enable or disable booleans accordingly.
			if( Input.GetButtonDown( enableButtonKeyboard ) )
				enableMenu = true;
			else if( Input.GetButtonUp( enableButtonKeyboard ) )
				disableMenu = true;
		}
	}

	/// <summary>
	/// This function will catch input from the Controller and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void ControllerInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// Store the horizontal and vertical axis of the targeted joystick axis.
		Vector2 modInput = new Vector2( Input.GetAxis( horizontalAxisController ), Input.GetAxis( verticalAxisController ) );

        modInput += SteamVR_Input.GetAction<SteamVR_Action_Vector2>("UltimateRadialMenuHover").GetAxis(SteamVR_Input_Sources.Any);
        modInput += UltimateRadialMenuInputHandler.instance.input;

        // Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
        float tempDist = Vector2.Distance( Vector2.zero, modInput );

		// Set the input to what we have calculated.
		if( modInput != Vector2.zero )
			input = modInput;

		// If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
		if( tempDist >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.minRange )
			distance = Mathf.Lerp( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange, 0.5f );

		// If the activation action is set to being the press of a button on the controller...
		if( Input.GetButtonDown( interactButtonController ) )
			inputDown = true;
		else if( Input.GetButtonUp( interactButtonController ) )
			inputUp = true;

        inputDown |= SteamVR_Input.GetAction<SteamVR_Action_Boolean>("UltimateRadialMenuSelect").GetStateDown(SteamVR_Input_Sources.Any);
        inputDown |= SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI").GetStateDown(SteamVR_Input_Sources.Any);
        inputUp |= SteamVR_Input.GetAction<SteamVR_Action_Boolean>("UltimateRadialMenuSelect").GetStateUp(SteamVR_Input_Sources.Any);
        inputUp |= SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI").GetStateUp(SteamVR_Input_Sources.Any);

        // If the user has a enable key assigned...
        if ( enableButtonController != string.Empty )
		{
			// Check for the Enable and Disable button keys and set the enable or disable booleans accordingly.
			if( Input.GetButtonDown( enableButtonController ) )
				enableMenu = true;
			else if( Input.GetButtonUp( enableButtonController ) )
				disableMenu = true;
		}
	}

	/// <summary>
	/// This function is a virtual void to allow for easy custom input logic.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void CustomInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// WARNING! This is not where you want to put your custom logic. See check out our video tutorials for more information.
		// Video Tutorials: https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu 
	}
}