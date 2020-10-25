/* UltimateRadialMenu.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent( typeof( CanvasGroup ) )]
[AddComponentMenu( "Ultimate Radial Menu/Ultimate Radial Menu" )]
public class UltimateRadialMenu : MonoBehaviour
{
	/* ----- > RADIAL MENU POSITIONING < ----- */
	public int menuButtonCount = 4;
	/// <summary>
	/// Returns the angle of each button, determined by the menu button count.
	/// </summary>
	public float GetAnglePerButton
	{
		get
		{
			return 360f / menuButtonCount;
		}
	}
	/// <summary>
	/// Returns the current input angle so that other scripts can access the current input of this radial menu.
	/// </summary>
	public float GetCurrentInputAngle
	{
		get;
		private set;
	}
	public enum ScalingAxis
	{
		Width, Height
	}
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public float menuSize = 5.0f;
	public float horizontalPosition = 50.0f, verticalPosition = 50.0f;
	public float menuButtonSize = 0.25f;
	public float radialMenuButtonRadius = 1.0f;
	public float angleOffset;
	RectTransform baseTransform;
	/// <summary>
	/// Returns the position of the base transform.
	/// </summary>
	public Vector2 BasePosition
	{
		get
		{
			if( baseTransform == null )
				return Vector2.zero;

			return baseTransform.position;
		}
	}
	public bool followOrbitalRotation = true;
	public float minRange = 0.25f, maxRange = 1.5f;
	/// <summary>
	/// Returns the calculated minimum range of the radial menu.
	/// </summary>
	public float CalculatedMinRange
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the calculated maximum range of the radial menu.
	/// </summary>
	public float CalculatedMaxRange
	{
		get;
		private set;
	}
	public bool infiniteMaxRange = false;
	public float buttonInputAngle = 0.0f;

	/* ----- > RADIAL MENU SETTINGS < ----- */
	// COLOR CHANGE //
	public bool colorChange = true;
	public Color normalColor = Color.white, highlightedColor = Color.white;
	public Color pressedColor = Color.white, disabledColor = Color.white;
	// SCALE TRANSFORM //
	public bool scaleTransform = false;
	public Vector2 normalScale = Vector2.one;
	public float highlightedScaleModifier = 1.1f, pressedScaleModifier = 1.05f;
	public float positionModifier = 0.0f;
	// SPRITE SWAP //
	public bool spriteSwap = false;
	public Sprite normalSprite, highlightedSprite;
	public Sprite pressedSprite, disabledSprite;
	// RADIAL MENU TOGGLE //
	public enum RadialMenuToggle
	{
		FadeAlpha,
		Scale
	}
	public RadialMenuToggle radialMenuToggle = RadialMenuToggle.FadeAlpha;
	public float toggleInDuration = 0.25f, toggleOutDuration = 0.25f;
	bool transitioning = false;
	CanvasGroup canvasGroup;
	// CENTER TEXT //
	public bool displayButtonName = false;
	public Text nameText;
	public float nameTextRatioX = 1.0f, nameTextRatioY = 1.0f, nameTextSize = 0.25f;
	public float nameTextHorizontalPosition = 50.0f, nameTextVerticalPosition = 50.0f;
	public bool displayButtonDescription = false;
	public Text descriptionText;
	public float descriptionTextRatioX = 1.0f, descriptionTextRatioY = 1.0f, descriptionTextSize = 0.25f;
	public float descriptionTextHorizontalPosition = 50.0f, descriptionTextVerticalPosition = 50.0f;
	// DISABLE SETTINGS //
	public bool disableIcon = false, disableText = false;

	/* ----- > RADIAL BUTTON SETTINGS < ----- */
	// ICON SETTINGS //
	public float iconSize = 0.25f, iconRotation = 0.0f;
	public float iconHorizontalPosition = 50.0f, iconVerticalPosition = 50.0f;
	public bool iconLocalRotation = false;
	public bool iconColorChange = false;
	public Color iconNormalColor = Color.white, iconHighlightedColor = Color.white;
	public Color iconPressedColor = Color.white, iconDisabledColor = Color.white;
	public bool iconScaleTransform = false;
	public Vector2 iconNormalScale = Vector2.one;
	public float iconHighlightedScaleModifier = 1.1f, iconPressedScaleModifier = 1.05f;

	// TEXT SETTINGS //
	public float textAreaRatioX = 1.0f, textAreaRatioY = 0.25f, textSize = 0.25f;
	public float textHorizontalPosition = 50.0f, textVerticalPosition = 50.0f;
	public bool displayNameOnButton = true;
	public bool textColorChange = false;
	public Color textNormalColor = Color.white, textHighlightedColor = Color.white;
	public Color textPressedColor = Color.white, textDisabledColor = Color.white;
	public enum TextPositioningOption
	{
		Global,
		Local,
		RelativeToIcon
	}
	public TextPositioningOption textPositioningOption = TextPositioningOption.Global;
	public bool textLocalRotation = true;
	// RADIAL MENU BUTTON //
	[Serializable]
	public class UltimateRadialButton
	{
		// BASIC VARIABLES //
		public UltimateRadialMenu radialMenu;
		public RectTransform buttonTransform;
		public Image radialImage;
		public bool buttonDisabled = false;
		public string name;
		public string description;
		public int buttonIndex = -1;

		// INPUT VARIABLES //
		public float minAngle, maxAngle, angle;

		// ICON SETTINGS //
		public bool useIcon = false, useText = false;
		public RectTransform iconTransform;
		public Image icon;
		public bool useIconUnique = false;
		public float iconSize = 0.0f;
		public float iconHorizontalPosition = 0.0f, iconVerticalPosition = 0.0f;
		public float iconRotation = 0.0f;

		// TEXT SETTINGS //
		public Text text;

		// TRANSFORM INTERACTION SETTINGS //
		public Vector3 normalPosition, activePosition;

		// BASIC CALLBACK INFORMATION //
		public string key;
		public int id;
		
		// CALLBACKS //
		public event Action OnRadialButtonInteract;
		public event Action<int> OnRadialButtonInteractWithId;
		public event Action<string> OnRadialButtonInteractWithKey;
		public UnityEvent unityEvent;

		/// <summary>
		/// Returns true if the angle is within the range of this button.
		/// </summary>
		/// <param name="angle">The current input angle.</param>
		public bool IsInAngle ( float angle )
		{
			// If this button is disabled then return false.
			if( buttonDisabled )
				return false;

			// If the minimum calculated angle for this button is less than zero...
			if( minAngle < 0 )
			{
				// If the angle is greater than the max angle, then this means that the angle may be within range of the minimum angle, so...
				if( angle > maxAngle )
				{
					// Modify the calculated min and max angles to being positive values.
					float newMinAngle = minAngle + 360;
					float newMaxAngle = maxAngle + 360;

					// Now check the angle again and see if it is within the new modified angles.
					if( angle >= newMinAngle && angle <= newMaxAngle )
						return true;
				}
			}

			// If the calculated max angle is greater than 360 degrees...
			if( maxAngle > 360 )
			{
				// If the angle is less than the min angle, then this could mean that the angle is within the range of this button so...
				if( angle < minAngle )
				{
					// Set the minimum angle as zero since the max angle is greater than 360, so it needs to check between 0 - max angle.
					float newMinAngle = 0;

					// Modify the max angle by decreasing it by 360.
					float newMaxAngle = maxAngle - 360;

					// Now check the angle against the new modified angles.
					if( angle >= newMinAngle && angle <= newMaxAngle )
						return true;
				}
			}
			
			// If the angle is absolutely 0.0...
			if( angle == 0.0f )
			{
				// If the min angle is less than zero, and max angle is greater than zero, the input is in range, so return true.
				if( minAngle < 0 && maxAngle > 0 )
					return true;

				// If the min angle is less than 360, but the max angle is greater than 360 then the input is in range, so return true.
				if( minAngle < 360 && maxAngle > 360 )
					return true;
			}

			// If the angle is greater than the min angle and the angle is also less than the max angle, the input is in range, so return true.
			if( angle >= minAngle && angle <= maxAngle )
				return true;

			// If none of the above checks have returned true, then return false since the input is not within range.
			return false;
		}

		/// <summary>
		/// Invokes the functionality for when the input enters the button.
		/// </summary>
		public void OnEnter ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;

			// If the user wants to change the color of the radial button, then apply the highlighted color.
			if( radialMenu.colorChange )
				radialImage.color = radialMenu.highlightedColor;

			// If the user want to swap sprites when the radial button is highlighted...
			if( radialMenu.spriteSwap )
			{
				// If the highlighted sprite is assigned, then apply that sprite to the image.
				if( radialMenu.highlightedSprite != null )
					radialImage.sprite = radialMenu.highlightedSprite;
				// Else the highlighted sprite is null, so apply the normal sprite to the image.
				else
					radialImage.sprite = radialMenu.normalSprite;
			}

			// If the user wants to scale the transform when the radial button is being hovered over...
			if( radialMenu.scaleTransform )
			{
				// Modify the scale and position of the radial button.
				buttonTransform.localScale = radialMenu.normalScale * radialMenu.highlightedScaleModifier;
				buttonTransform.localPosition = activePosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( useIcon && icon != null )
			{
				// If the user wants to change color, then apply the highlighted color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconHighlightedColor;

				// If the user wants to scale the transform of the icon...
				if( radialMenu.iconScaleTransform )
				{
					// If the transform is unassigned, then assign it.
					if( iconTransform == null )
						iconTransform = icon.rectTransform;

					// Apply the highlighted scale.
					iconTransform.localScale = Vector3.one * radialMenu.iconHighlightedScaleModifier;
				}
			}

			// If the user wants to use text and it is assigned...
			if( useText && text != null )
			{
				// If the user wants to change color, then apply the highlighted color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textHighlightedColor;
			}
			
			// Inform any subscribers that this button has been entered.
			if( radialMenu.OnRadialButtonEnter != null )
				radialMenu.OnRadialButtonEnter( buttonIndex );
		}

		/// <summary>
		/// Invokes the functionality for when the input exits the button.
		/// </summary>
		public void OnExit ()
		{
			// If this button is disabled, then return...
			if( buttonDisabled )
				return;

			// If the user wants to change the color of the image, then apply the normal color.
			if( radialMenu.colorChange )
				radialImage.color = radialMenu.normalColor;

			// If the user wants to swap sprites, then apply the normal sprite to the image.
			if( radialMenu.spriteSwap )
				radialImage.sprite = radialMenu.normalSprite;

			// If the user wants to scale the transform, then apply the normal scale and position.
			if( radialMenu.scaleTransform )
			{
				radialImage.GetComponent<RectTransform>().localScale = radialMenu.normalScale;
				radialImage.GetComponent<RectTransform>().localPosition = normalPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( useIcon && icon != null )
			{
				// If the user wants to change color, then apply the normal color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconNormalColor;

				// If the user wants to scale the transform of the icon...
				if( radialMenu.iconScaleTransform )
				{
					// If the transform is unassigned, then assign it.
					if( iconTransform == null )
						iconTransform = icon.rectTransform;

					// Apply the basic scale.
					iconTransform.localScale = Vector3.one;
				}
			}

			// If the user wants to use text and it is assigned...
			if( useText && text != null )
			{
				// If the user wants to change color, then apply the normal color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textNormalColor;
			}

			// Inform and subscribers that this button have been exited.
			if( radialMenu.OnRadialButtonExit != null )
				radialMenu.OnRadialButtonExit( buttonIndex );
		}

		/// <summary>
		/// Invokes the functionality for when the input interacts with the button.
		/// </summary>
		public void OnInteract ()
		{
			// If this button is disabled, then return.
			if( buttonDisabled )
				return;

			// If the user has assigned a unity event to call, then call it.
			if( unityEvent != null )
				unityEvent.Invoke();

			// If the user has subscribed to the default callback then invoke it.
			if( OnRadialButtonInteract != null )
				OnRadialButtonInteract();

			// If the user has subscribed to the ID callback then invoke it with the assigned integer ID.
			if( OnRadialButtonInteractWithId != null )
				OnRadialButtonInteractWithId( id );

			// If the user has subscribed to the string key callback then invoke it with the assigned key.
			if( OnRadialButtonInteractWithKey != null )
				OnRadialButtonInteractWithKey( key );

			// Inform any subscribers that this button has been interacted with.
			if( radialMenu.OnRadialButtonInteract != null )
				radialMenu.OnRadialButtonInteract( buttonIndex );
		}

		/// <summary>
		/// Invokes the functionality for when button is disabled.
		/// </summary>
		public void DisableButton ()
		{
			// If the radial image is null, then return.
			if( radialImage == null )
				return;

			// Set the disable button to true so that nothing will be calculated on this button.
			buttonDisabled = true;

			// If the user wants to use a disabled sprite...
			if( radialMenu.spriteSwap && radialMenu.disabledSprite != null )
			{
				// Assign the color and sprite to the image.
				radialImage.color = radialMenu.disabledColor;
				radialImage.sprite = radialMenu.disabledSprite;
			}
			// Else if the use wants to just change the color, then apply the color.
			else if( radialMenu.colorChange )
				radialImage.color = radialMenu.disabledColor;
			// Else, just disable image component.
			else
				radialImage.enabled = false;

			// If the user is scaling the transform, then reset the scale and position.
			if( radialMenu.scaleTransform )
			{
				radialImage.GetComponent<RectTransform>().localScale = radialMenu.normalScale;
				radialImage.GetComponent<RectTransform>().localPosition = normalPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( useIcon && icon != null )
			{
				// If the user wants to change color, then apply the disabled color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconDisabledColor;

				// If the user wants to scale the transform of the icon...
				if( radialMenu.iconScaleTransform )
				{
					// If the transform is unassigned, then assign it.
					if( iconTransform == null )
						iconTransform = icon.rectTransform;

					// Apply the basic scale.
					iconTransform.localScale = Vector3.one;
				}
			}

			// If the user wants to use text and it is assigned...
			if( useText && text != null )
			{
				// If the user wants to change color, then apply the disabled color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textDisabledColor;
			}

			// If the user wants to disable the icon when disabled, then disable it here.
			if( radialMenu.disableIcon && useIcon && icon != null )
				icon.enabled = false;

			// If the user wants to disable the text when disabled, then disable it here.
			if( radialMenu.disableText && useText && text != null )
				text.enabled = false;
		}

		/// <summary>
		/// Invokes the functionality for when button is enabled.
		/// </summary>
		public void EnableButton ()
		{
			// If the radial image is null, then return.
			if( radialImage == null )
				return;

			// Set the disable button to false so that calculations can continue on this button.
			buttonDisabled = false;

			// Enable the radial image component.
			radialImage.enabled = true;

			// If the game object is disabled, then enable it.
			if( !radialImage.gameObject.activeInHierarchy )
				radialImage.gameObject.SetActive( true );

			// If the user wants to use a disabled sprite...
			if( radialMenu.spriteSwap && radialMenu.normalSprite != null )
			{
				// Assign the color and sprite to the image.
				radialImage.color = radialMenu.normalColor;
				radialImage.sprite = radialMenu.normalSprite;
			}
			// Else if the use wants to just change the color, then apply the color.
			else if( radialMenu.colorChange )
				radialImage.color = radialMenu.normalColor;

			// If the user is scaling the transform, then reset the scale and position.
			if( radialMenu.scaleTransform )
			{
				radialImage.GetComponent<RectTransform>().localScale = radialMenu.normalScale;
				radialImage.GetComponent<RectTransform>().localPosition = normalPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( useIcon && icon != null )
			{
				// If the user wants to change color, then apply the normal color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconNormalColor;

				// If the user wants to scale the transform of the icon...
				if( radialMenu.iconScaleTransform )
				{
					// If the transform is unassigned, then assign it.
					if( iconTransform == null )
						iconTransform = icon.rectTransform;

					// Apply the basic scale.
					iconTransform.localScale = Vector3.one;
				}
			}

			// If the user wants to use text and it is assigned...
			if( useText && text != null )
			{
				// If the user wants to change color, then apply the normal color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textNormalColor;
			}

			// If the user disabled the icon, then enable it here.
			if( useIcon && icon != null )
				icon.enabled = true;

			// If the user disabled the text, then enable it here.
			if( useText && text != null )
				text.enabled = true;
		}

		/// <summary>
		/// Invokes the functionality for when the input is down on the button.
		/// </summary>
		public void OnInputDown ()
		{
			// If the user wants to change the color, then apply the pressed color to the image.
			if( radialMenu.colorChange )
				radialImage.color = radialMenu.pressedColor;

			// If the user wants to swap sprites and the pressed sprite isn't null, then apply the sprite.
			if( radialMenu.spriteSwap && radialMenu.pressedSprite != null )
				radialImage.sprite = radialMenu.pressedSprite;

			// If the user wants to scale the transform...
			if( radialMenu.scaleTransform )
			{
				// Then apply the scale modifier and position.
				radialImage.GetComponent<RectTransform>().localScale = radialMenu.normalScale * radialMenu.pressedScaleModifier;
				radialImage.GetComponent<RectTransform>().localPosition = activePosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( useIcon && icon != null )
			{
				// If the user wants to change color, then apply the pressed color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconPressedColor;

				// If the user wants to scale the transform of the icon...
				if( radialMenu.iconScaleTransform )
				{
					// If the transform is unassigned, then assign it.
					if( iconTransform == null )
						iconTransform = icon.rectTransform;

					// Apply the basic scale.
					iconTransform.localScale = Vector3.one * radialMenu.iconPressedScaleModifier;
				}
			}

			// If the user wants to use text and it is assigned...
			if( useText && text != null )
			{
				// If the user wants to change color, then apply the pressed color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textPressedColor;
			}
		}

		/// <summary>
		/// Invokes the functionality for when the input is release on the button.
		/// </summary>
		public void OnInputUp ()
		{
			// If this button is still the currently selected button, then return to the OnEnter() function.
			if( buttonIndex == radialMenu.CurrentButtonIndex )
				OnEnter();
			// Else call the OnExit() function because the input has left this button.
			else
				OnExit();
		}

		/// <summary>
		/// Resets the radial button to be ready for new button information.
		/// </summary>
		public void ResetRadialButtonInformation ()
		{
			key = "";
			id = -1;

			OnRadialButtonInteract = null;
			OnRadialButtonInteractWithId = null;
			OnRadialButtonInteractWithKey = null;
		}
	}
	public List<UltimateRadialButton> UltimateRadialButtonList = new List<UltimateRadialButton>();
	
	// ----- < SCRIPT REFERENCE > ----- //
	static Dictionary<string, UltimateRadialMenu> UltimateRadialMenus = new Dictionary<string, UltimateRadialMenu>();
	public string radialMenuName = string.Empty;

	// ----- < ACTION SUBSCRIPTIONS > ----- //
	/// <summary>
	/// This callback will be called when a radial button has been found. The returned integer will be the index of the radial button.
	/// </summary>
	public event Action<int> OnRadialMenuButtonFound;
	/// <summary>
	/// This callback will be called when a radial button has been entered.
	/// </summary>
	public event Action<int> OnRadialButtonEnter;
	/// <summary>
	/// This callback will be called when a radial button has been exited.
	/// </summary>
	public event Action<int> OnRadialButtonExit;
	/// <summary>
	/// This callback will be called when a radial button has been interacted with.
	/// </summary>
	public event Action<int> OnRadialButtonInteract;
	/// <summary>
	/// This callback will be called when the radial menu has lost focus.
	/// </summary>
	public event Action OnRadialMenuLostFocus;
	/// <summary>
	/// This callback will be called when the radial menu has been enabled.
	/// </summary>
	public event Action OnRadialMenuEnabled;
	/// <summary>
	/// This callback will be called when the radial menu has been disabled.
	/// </summary>
	public event Action OnRadialMenuDisabled;
	/// <summary>
	/// This callback will be called when the radial menu's positioning has been updated.
	/// </summary>
	public event Action OnUpdateSizeAndPlacement;
	/// <summary>
	/// This callback will be called whenever a radial button is added or subtracted from the Radial Menu. This is useful for swapping sprites and positioning for a new count.
	/// </summary>
	public event Action<int> OnRadialMenuButtonCountModified;

	// ----- < CALCULATIONS > ----- //
	[Serializable]
	public class RadialButtonPrefab
	{
		public GameObject prefab;
		public UltimateRadialButton radialButton;
		public int siblingIndex;
	}
	public RadialButtonPrefab radialButtonPrefab = new RadialButtonPrefab();
	/// <summary>
	/// Returns the currently selected button index.
	/// </summary>
	public int CurrentButtonIndex
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current state of the radial menu.
	/// </summary>
	public bool RadialMenuActive
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current state of the input on this radial menu.
	/// </summary>
	public bool InputInRange
	{
		get;
		private set;
	}
	bool inputInRangeLastFrame = false;
	int buttonIndexOnInputDown = -1;

	
	void Awake ()
	{
		// If the application is not playing, then return.
		if( Application.isPlaying == false )
			return;

		// If the name is assigned...
		if( radialMenuName != string.Empty )
		{
			// Check to see if the dictionary already contains this name, and if so, remove the current one.
			if( UltimateRadialMenus.ContainsKey( radialMenuName ) )
				UltimateRadialMenus.Remove( radialMenuName );

			// Register this UltimateRadialMenu into the dictionary.
			UltimateRadialMenus.Add( radialMenuName, GetComponent<UltimateRadialMenu>() );
		}

		// Instantiate a new copy of the first button in the list and set the prefab information.
		GameObject newRadialButton = Instantiate( UltimateRadialButtonList[ 0 ].buttonTransform.gameObject, Vector3.zero, Quaternion.identity );
		radialButtonPrefab = new RadialButtonPrefab()
		{
			prefab = newRadialButton,
			radialButton = UltimateRadialButtonList[ 0 ],
			siblingIndex = UltimateRadialButtonList[ 0 ].buttonTransform.GetSiblingIndex()
		};
		newRadialButton.name = "Radial Button Prefab";
		newRadialButton.transform.SetParent( transform );
		newRadialButton.transform.SetSiblingIndex( radialButtonPrefab.siblingIndex );
		newRadialButton.SetActive( false );

		// Assign the canvas group component.
		canvasGroup = GetComponent<CanvasGroup>();

		// Update the size and placement of the Ultimate Radial Menu.
		UpdateSizeAndPlacement();

		// Reset the menu.
		ResetRadialMenu();

		// Disable the radial menu immediately.
		DisableRadialMenuImmediate();
	}

	void Start ()
	{
		// If the game is running, then return.
		if( !Application.isPlaying )
			return;

		// Store the current parent object.
		Transform parent = transform.parent;
		while( parent != null )
		{
			// If the parent has a Canvas component...
			if( parent.transform.GetComponent<Canvas>() )
			{
				// If there is no Updater script attached, then attach an Updater script.
				if( !parent.transform.GetComponent<Canvas>().GetComponent<UltimateRadialMenuScreenSizeUpdater>() )
					parent.transform.GetComponent<Canvas>().gameObject.AddComponent( typeof( UltimateRadialMenuScreenSizeUpdater ) );

				// Break the loop.
				break;
			}

			// Since there was no Canvas component on the parent, get the new parent.
			parent = parent.transform.parent;
		}

		if( !FindObjectOfType<EventSystem>().gameObject.GetComponent<UltimateRadialMenuInputManager>() )
			FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();
	}

#if UNITY_EDITOR
	// NOTE: This update function will not run inside of a built game because it is surrounded by the #if UNITY_EDITOR. This update function only runs inside the editor of Unity.
	void Update ()
	{
		if( Application.isPlaying == false )
			UpdateSizeAndPlacement();
	}
	#endif

	/// <summary>
	/// Processes the input and calculates the information for use within the Ultimate Radial Menu.
	/// </summary>
	/// <param name="input">The current input values.</param>
	/// <param name="distance">The distance of the input from the center of the radial menu.</param>
	/// <param name="inputDown">EDIT</param>
	/// <param name="inputUp">EDIT</param>
	public void ProcessInput ( Vector2 input, float distance, bool inputDown, bool inputUp )
	{
		// If the radial menu is inactive then return.
		if( !RadialMenuActive )
			return;
		
		// Set the InputInRange bool to false by default.
		InputInRange = false;
		
		// Calculate the angle of the input and convert it to degrees.
		float angle = Mathf.Atan2( input.y, input.x ) * Mathf.Rad2Deg;

		// If the angle is negative, then add 360 to make it positive.
		if( angle < 0 )
			angle += 360;

		// Store the current angle so that other scripts can get it.
		GetCurrentInputAngle = angle;

		// Loop through all of the radial menu buttons...
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			// If the distance of the input exceeds the boundaries of the radial menu...
			if( distance < CalculatedMinRange || distance > CalculatedMaxRange )
			{
				// Reset the radial menu.
				ResetRadialMenu();

				// If the input was in range the last frame, then notify the subscriptions that the radial menu has lost focus.
				if( inputInRangeLastFrame == true && OnRadialMenuLostFocus != null )
					OnRadialMenuLostFocus();

				// Break the loop.
				break;
			}

			// If the angle is within the range of the current radial menu button...
			if( UltimateRadialButtonList[ i ].IsInAngle( angle ) )
			{
				// Set the InputInRange to true.
				InputInRange = true;

				// If the current button index is not this index...
				if( CurrentButtonIndex != i )
				{
					// If the current index is greater than -1, then exit the current button.
					if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count )
					{
						buttonIndexOnInputDown = -1;
						UltimateRadialButtonList[ CurrentButtonIndex ].OnExit();
					}

					// Assign the current button index to this index.
					CurrentButtonIndex = i;

					// Call the OnEnter function on the current button.
					UltimateRadialButtonList[ i ].OnEnter();

					// Notify any subscribers that the radial menu has found a button.
					if( OnRadialMenuButtonFound != null )
						OnRadialMenuButtonFound( i );
				}

				// If the user wants to display the name of the button and the text is assigned, then apply the name.
				if( displayButtonName == true && nameText != null )
					nameText.text = UltimateRadialButtonList[ i ].name;

				// If the user wants to display the description of the button and the text is assigned, then display the description.
				if( displayButtonDescription == true && descriptionText != null )
					descriptionText.text = UltimateRadialButtonList[ i ].description;

				// Break the loop.
				break;
			}

			// If this loop has reached the end of the button list and no buttons have been found worthy...
			if( i == UltimateRadialButtonList.Count - 1 )
			{
				// Reset the radial menu.
				ResetRadialMenu();

				// Inform any subscribers that the radial menu has lost focus.
				if( OnRadialMenuLostFocus != null )
					OnRadialMenuLostFocus();
			}
		}

		// If the last frame caught input, but now the radial button list is zero...
		if( inputInRangeLastFrame && UltimateRadialButtonList.Count == 0 )
		{
			// Inform any subscribers that the radial menu has lost focus.
			if( OnRadialMenuLostFocus != null )
				OnRadialMenuLostFocus();

			ResetRadialMenu();
		}

		// If the input is down on this frame, the input is in range, and the current index is within range...
		if( inputDown && InputInRange && CurrentButtonIndex >= 0 )
		{
			// Call the OnInputDown() function on the current button.
			UltimateRadialButtonList[ CurrentButtonIndex ].OnInputDown();

			// If the invoke action is set to being when the button is down...
			if( UltimateRadialMenuInputManager.Instance.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonDown )
			{
				// Call the OnInteract() function on the current button.
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInteract();

				// If the input manager wants to disable the radial menu when interacted with, then disable the menu.
				if( UltimateRadialMenuInputManager.Instance.disableOnInteract )
					DisableRadialMenu();
			}

			// Set the button index to the buttonIndexOnInputDown so that the button up can be calculated.
			buttonIndexOnInputDown = CurrentButtonIndex;
		}

		// If the input is up on this frame, the input is in range, and the current index is within range...
		if( inputUp && InputInRange && CurrentButtonIndex >= 0 )
		{
			// Call the OnInputUp() function on the current button.
			UltimateRadialButtonList[ CurrentButtonIndex ].OnInputUp();

			// If the invoke action is set to being when the button has been clicked, and the current button index is the same as when the buttonIndexOnInputDown was calculated...
			if( UltimateRadialMenuInputManager.Instance.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonClick && CurrentButtonIndex == buttonIndexOnInputDown )
			{
				// Call the OnInteract() function on the current button.
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInteract();

				// If the input manager wants to disable the radial menu when interacted with, then disable the menu.
				if( UltimateRadialMenuInputManager.Instance.disableOnInteract )
					DisableRadialMenu();
			}

			// Reset the buttonIndexOnInputDown.
			buttonIndexOnInputDown = -1;
		}

		// Store the InputInRange value for the next calculation.
		inputInRangeLastFrame = InputInRange;
	}

	/// <summary>
	/// Resets the Ultimate Radial Menu Buttons and all the enabled options to their default state.
	/// </summary>
	void ResetRadialMenu ()
	{
		// If the current index greater than or equal to zero, then reset the current button.
		if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count )
			UltimateRadialButtonList[ CurrentButtonIndex ].OnExit();

		// Set the current button index to -1, resetting it.
		CurrentButtonIndex = -1;
		buttonIndexOnInputDown = -1;

		// If the user is wanting to display the current selection on the radial menu, reset the text.
		if( displayButtonName == true && nameText != null )
			nameText.text = "";

		// If the user is wanting to display the description of the selected button, then reset the text here.
		if( displayButtonDescription && descriptionText != null )
			descriptionText.text = "";
	}

	/// <summary>
	/// Fades the canvas group component over time.
	/// </summary>
	IEnumerator FadeRadialMenu ()
	{
		// Set transitioning to true so that other functions can know this is running.
		transitioning = true;

		// Calculate the speed of the fade.
		float speed = 1.0f / toggleInDuration;

		// Store the starting alpha value so that the transition will be smooth.
		float startingAlpha = canvasGroup.alpha;
		for( float t = 0.0f; t < 1.0f && RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the alpha by the current alpha to 1.
			canvasGroup.alpha = Mathf.Lerp( startingAlpha, 1.0f, t );
			yield return null;
		}

		// If the radial menu is still active, apply the final alpha value.
		if( RadialMenuActive )
			canvasGroup.alpha = 1.0f;

		// Hold here while the radial menu is active.
		while( RadialMenuActive )
			yield return null;

		// Store the current alpha value so that the transition will be smooth.
		speed = 1.0f / toggleOutDuration;
		startingAlpha = canvasGroup.alpha;
		for( float t = 0.0f; t < 1.0f && !RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the alpha from the current to 0.
			canvasGroup.alpha = Mathf.Lerp( startingAlpha, 0.0f, t );
			yield return null;
		}

		// If the radial menu is not active still, then apply 0 as the final alpha.
		if( !RadialMenuActive )
			canvasGroup.alpha = 0.0f;

		// Set transitioning to false.
		transitioning = false;
	}

	/// <summary>
	/// Scales the transform of the radial menu over time.
	/// </summary>
	IEnumerator ScaleRadialMenu ()
	{
		// Set transitioning to true so that other functions can know this is running.
		transitioning = true;

		// Calculate the speed of the fade.
		float speed = 1.0f / toggleInDuration;

		// Store the starting scale so that the transition will be smooth.
		Vector3 startingScale = baseTransform.localScale;
		for( float t = 0.0f; t < 1.0f && RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the scale from the starting scale to a Vector3.one.
			baseTransform.localScale = Vector3.Lerp( startingScale, Vector3.one, t );
			yield return null;
		}

		// If the radial menu is still active, apply the final scale. 
		if( RadialMenuActive )
			baseTransform.localScale = Vector3.one;

		// Loop here while the radial menu is active.
		while( RadialMenuActive )
			yield return null;

		speed = 1.0f / toggleOutDuration;
		// Store the current scale so that the transition will be smooth.
		startingScale = baseTransform.localScale;
		for( float t = 0.0f; t < 1.0f && !RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the scale from the current to 0.
			baseTransform.localScale = Vector3.Lerp( startingScale, Vector3.zero, t );
			yield return null;
		}

		// If the radial menu is still inactive, apply the final scale. 
		if( !RadialMenuActive )
			baseTransform.localScale = Vector3.zero;

		// Set transitioning to false so other functions know.
		transitioning = false;
	}

	/// <summary>
	/// Returns the ratio of the targeted sprite.
	/// </summary>
	/// <param name="sprite">The sprite to calculate the ratio of.</param>
	Vector2 GetImageAspectRatio ( Sprite sprite )
	{
		Vector2 ratio = Vector2.one;
		
		// Store the raw values of the sprites ratio so that a smaller value can be configured.
		Vector2 rawRatio = new Vector2( sprite.rect.width, sprite.rect.height );

		// Temporary float to store the largest side of the sprite.
		float maxValue = rawRatio.x > rawRatio.y ? rawRatio.x : rawRatio.y;

		// Now configure the ratio based on the above information.
		ratio.x = rawRatio.x / maxValue;
		ratio.y = rawRatio.y / maxValue;

		return ratio;
	}

	/// <summary>
	/// Creates a new GameObject with all the needed components and updates all of the information based off of the new radial button information.
	/// </summary>
	/// <param name="buttonIndex">The index of where to insert the new button.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the new radial button.</param>
	UltimateRadialButton CreateRadialButtonAtIndex ( int buttonIndex, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If for some reason the prefab is null...
		if( radialButtonPrefab.prefab == null || radialButtonPrefab.radialButton == null )
		{
			// Inform the user that there is no prefab and return to avoid errors.
			Debug.LogError( "Ultimate Radial Menu\nThere is no prefab to create new radial buttons. Please make sure that your radial menu has at least one element before trying to add a new element." );
			return null;
		}

		// If the button index is less than zero, then make is zero to avoid errors.
		if( buttonIndex < 0 )
			buttonIndex = 0;

		// If the button index is higher than the button list count, then assign it to the end of the list.
		if( buttonIndex > UltimateRadialButtonList.Count )
			buttonIndex = UltimateRadialButtonList.Count;

		// Create a new radial button game object in the scene.
		GameObject newRadialButton = Instantiate( radialButtonPrefab.prefab, Vector3.zero, Quaternion.identity );

		// Set the parent transform of the new radial button.
		newRadialButton.transform.SetParent( baseTransform );

		// Set the sibling index to the prefab's sibling index, which will essentially put it at the end of the radial menu game objects.
		newRadialButton.transform.SetSiblingIndex( radialButtonPrefab.siblingIndex );

		// Set the new game objects state to active so that it can be visible in the scene.
		newRadialButton.SetActive( true );

		// Assign the radial button name so that it is not a clone.
		newRadialButton.name = "Radial Menu Button";

		// Create a new radial menu button and assign the needed information.
		UltimateRadialButton newRadialMenuButton = new UltimateRadialButton()
		{
			radialMenu = this,
			radialImage = newRadialButton.GetComponent<Image>(),
			buttonTransform = newRadialButton.GetComponent<RectTransform>(),
			buttonDisabled = radialButtonPrefab.radialButton.buttonDisabled,
			key = newRadialButtonInfo.key,
			id = newRadialButtonInfo.id,
			name = newRadialButtonInfo.name,
			description = newRadialButtonInfo.description,
			useIcon = radialButtonPrefab.radialButton.useIcon,
			useText = radialButtonPrefab.radialButton.useText,
		};

		// Set the radial image to enabled. This is because sometimes when creating a new gameobject from a disabled gameobject it's components are disabled for some reason.
		newRadialMenuButton.radialImage.enabled = true;

		// If the prefab was using an icon...
		if( newRadialMenuButton.useIcon )
		{
			// Store all of the images that are a child of the radial button game object.
			Image[] images = newRadialButton.GetComponentsInChildren<Image>();

			// Loop through them all and find the one that is not the image on this gameobject.
			for( int i = 0; i < images.Length; i++ )
			{
				if( images[ i ].gameObject != newRadialButton )
					newRadialMenuButton.icon = images[ i ];
			}

			// If the icon was found successfully...
			if( newRadialMenuButton.icon != null )
			{
				// Then enable the image component.
				newRadialMenuButton.icon.enabled = true;

				newRadialMenuButton.iconTransform = newRadialMenuButton.icon.rectTransform;

				// If the new information has an icon to apply, then do that here.
				if( newRadialButtonInfo.icon != null )
					newRadialMenuButton.icon.sprite = newRadialButtonInfo.icon;
			}
		}

		// If the prefab was using text...
		if( newRadialMenuButton.useText )
		{
			// Assign the text component that is a child of the radial button game object.
			newRadialMenuButton.text = newRadialButton.GetComponentInChildren<Text>();

			// If the text was successfully found then assign the radial info text.
			if( newRadialMenuButton.text != null && displayNameOnButton )
				newRadialMenuButton.text.text = newRadialButtonInfo.name;
		}
		
		// Increase the amount if menu buttons.
		menuButtonCount++;

		// Configure the new angle offset.
		angleOffset = GetAnglePerButton / 2;

		// Insert the new radial button into the list.
		UltimateRadialButtonList.Insert( buttonIndex, newRadialMenuButton );

		// Update the size and placement of the radial buttons since there is a new radial button element.
		UpdateSizeAndPlacement();

		// Notify any subscribers that the button count was modified.
		if( OnRadialMenuButtonCountModified != null )
			OnRadialMenuButtonCountModified( menuButtonCount );

		return newRadialMenuButton;
	}

	/// <summary>
	/// Updates the radial button with information and callback.
	/// </summary>
	/// <param name="buttonIndex">The index of the targeted radial button.</param>
	/// <param name="radialButtonInfo">The new information to apply to the radial button.</param>
	void UpdateRadialButtonInformation ( int buttonIndex, UltimateRadialButtonInfo radialButtonInfo )
	{
		// Assign the basic variables to the new information provided.
		UltimateRadialButtonList[ buttonIndex ].key = radialButtonInfo.key;
		UltimateRadialButtonList[ buttonIndex ].id = radialButtonInfo.id;
		if( radialButtonInfo.name != string.Empty )
			UltimateRadialButtonList[ buttonIndex ].name = radialButtonInfo.name;
		if( radialButtonInfo.description != string.Empty )
			UltimateRadialButtonList[ buttonIndex ].description = radialButtonInfo.description;
		UltimateRadialButtonList[ buttonIndex ].useIcon = radialButtonPrefab.radialButton.useIcon;
		UltimateRadialButtonList[ buttonIndex ].useText = radialButtonPrefab.radialButton.useText;

		// Set the radial image to enabled. This is because sometimes when creating a new gameobject from a disabled gameobject it's components are disabled for some reason.
		UltimateRadialButtonList[ buttonIndex ].radialImage.enabled = true;

		// If the button is using an icon...
		if( UltimateRadialButtonList[ buttonIndex ].useIcon )
		{
			// If the icon is assigned...
			if( UltimateRadialButtonList[ buttonIndex ].icon != null )
			{
				// Then enable the image component.
				UltimateRadialButtonList[ buttonIndex ].icon.enabled = true;

				// If the new information has an icon to apply, then do that here.
				if( radialButtonInfo.icon != null )
					UltimateRadialButtonList[ buttonIndex ].icon.sprite = radialButtonInfo.icon;
			}
		}

		// If the button is using text...
		if( UltimateRadialButtonList[ buttonIndex ].useText )
		{
			// If the text is assigned then assign the radial info text.
			if( UltimateRadialButtonList[ buttonIndex ].text != null && displayNameOnButton )
				UltimateRadialButtonList[ buttonIndex ].text.text = radialButtonInfo.name;
		}
	}

	/// <summary>
	/// Returns the index of the button with the targeted name.
	/// </summary>
	/// <param name="buttonName">The name of the targeted button.</param>
	int GetRadialButtonIndexByName ( string buttonName )
	{
		// Loop through the radial button list...
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			// If the current radial button's name is the same as the button name passed to this function, then return this index.
			if( UltimateRadialButtonList[ i ].name == buttonName )
				return i;
		}

		// Else there was no button with the targeted name, so inform the user and return -1.
		Debug.LogWarning( "Ultimate Radial Menu\nNo radial button was found with the name: " + buttonName );
		return -1;
	}

	// -------------------------------------------------- < PUBLIC FUNCTIONS FOR THE USER > -------------------------------------------------- //
	/// <summary>
	/// Updates the Size and Placement of the radial menu according to the user's options.
	/// </summary>
	public void UpdateSizeAndPlacement ()
	{
		// Set the current reference size for scaling.
		float referenceSize = 1024;// scalingAxis == ScalingAxis.Height ? Screen.height : Screen.width;

		// Configure the target size for the graphic.
		float textureSize = referenceSize * ( menuSize / 10 );

		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( baseTransform == null )
			baseTransform = GetComponent<RectTransform>();

		// Store the starting scale so that it can be returned to. This is necessary if the user wants to show text but is also using the scale transform option for toggling the radial menu.
		Vector3 startingScale = baseTransform.localScale;
		baseTransform.localScale = Vector3.one;

		// If the pivot is not 0.5, then set the pivot.
		if( baseTransform.pivot != Vector2.one / 2 )
			baseTransform.pivot = Vector2.one / 2;

		// First, fix the positioning to be a value between 0.0f and 1.0f.
		Vector2 fixedPositioning = new Vector2( horizontalPosition, verticalPosition ) / 100;

		// Then configure position spacers according to the screen's dimensions, the fixed spacing and texture size.
		//float xPosition = Screen.width * fixedPositioning.x - ( textureSize * fixedPositioning.x ) + ( ( textureSize * baseTransform.pivot.x ) );
		//float yPosition = Screen.height * fixedPositioning.y - ( textureSize * fixedPositioning.y ) + ( ( textureSize * baseTransform.pivot.y ) );
		float xPosition = 0f;
		float yPosition = 0f;

		// Apply the texture size to the baseTransform.
		baseTransform.sizeDelta = new Vector2( textureSize, textureSize );

		// Apply the positioning to the baseTransform.
		baseTransform.position = new Vector2( xPosition, yPosition );

		// Calculate the minimum range.
		CalculatedMinRange = ( baseTransform.sizeDelta.x / 2 ) * minRange;

		// If the user wants to have an infinite max range then apply that, otherwise calculate the max range by the baseTransform's size delta.
		if( infiniteMaxRange )
			CalculatedMaxRange = Mathf.Infinity;
		else
			CalculatedMaxRange = ( baseTransform.sizeDelta.x / 2 ) * maxRange;

		// -------------------------- < CENTER TEXT POSITIONING > -------------------------- //
		// If the user wants to display the radial button name, and the text is assigned...
		if( displayButtonName && nameText != null )
		{
			// Configure text position and size.
			Vector2 textPosition = ( Vector2 )baseTransform.position - ( baseTransform.sizeDelta / 2 );
			textPosition.x += baseTransform.sizeDelta.x * ( nameTextHorizontalPosition / 100 );
			textPosition.y += baseTransform.sizeDelta.y * ( nameTextVerticalPosition / 100 );
			nameText.rectTransform.sizeDelta = new Vector2( baseTransform.sizeDelta.x * nameTextSize, baseTransform.sizeDelta.x * nameTextSize ) * new Vector2( nameTextRatioX, nameTextRatioY );
			nameText.rectTransform.position = textPosition;
			nameText.rectTransform.rotation = Quaternion.identity;
		}

		// If the user wants to display the description of the button...
		if( displayButtonDescription && descriptionText != null )
		{
			// Configure text position and size.
			Vector2 textPosition = ( Vector2 )baseTransform.position - ( baseTransform.sizeDelta / 2 );
			textPosition.x += baseTransform.sizeDelta.x * ( descriptionTextHorizontalPosition / 100 );
			textPosition.y += baseTransform.sizeDelta.y * ( descriptionTextVerticalPosition / 100 );
			descriptionText.rectTransform.sizeDelta = new Vector2( baseTransform.sizeDelta.x * descriptionTextSize, baseTransform.sizeDelta.x * descriptionTextSize ) * new Vector2( descriptionTextRatioX, descriptionTextRatioY );
			descriptionText.rectTransform.position = textPosition;
			descriptionText.rectTransform.rotation = Quaternion.identity;
		}

		// ------------------------- < RADIAL BUTTON POSITIONING > ------------------------- //
		// Configure the angle per button.
		float angle = GetAnglePerButton;

		// Convert the angle into radians. Here we are applying the angle as negative since we want the buttons to go clockwise.
		float angleInRadians = -angle * Mathf.Deg2Rad;

		// Configure how much to offset the rotation of the button by.
		float rotationOffset = angleOffset - ( GetAnglePerButton / 2 );

		// Store the buttons size.
		Vector2 buttonImageSize = ( baseTransform.sizeDelta * menuButtonSize );

		// If the normal sprite is assigned then multiply the button size by the ratio of the sprite.
		if( normalSprite != null )
			buttonImageSize *= GetImageAspectRatio( normalSprite );

		// Configure the button radius. ( half of the base size * by the radius ) minus ( half of the button's Y size ).
		float buttonRadius = ( ( baseTransform.sizeDelta.x / 2 ) * radialMenuButtonRadius ) - ( buttonImageSize.y / 2 );

		// Loop through all of the radial buttons.
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			UltimateRadialButtonList[ i ].buttonIndex = i;

			// If the radial image is null then try to find it.
			if( UltimateRadialButtonList[ i ].radialImage == null )
				UltimateRadialButtonList[ i ].radialImage = UltimateRadialButtonList[ i ].buttonTransform.GetComponent<Image>();

			// Apply the size to the button transform.
			UltimateRadialButtonList[ i ].buttonTransform.sizeDelta = buttonImageSize;

			// Configure a new position for the button.
			Vector3 normalPosition = Vector3.zero;

			// This code may seem like a bunch of voodoo magic, ( but that's what math is, am I right!? ) but essentially it is just adding together all of the angle options that were set and multiplying it by the button radius.
			normalPosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + ( 90 * Mathf.Deg2Rad ) + ( angleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * buttonRadius );
			normalPosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + ( 90 * Mathf.Deg2Rad ) + ( angleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * buttonRadius );

			// Apply the new position to the transform, as well as a default scale of one.
			UltimateRadialButtonList[ i ].buttonTransform.localPosition = normalPosition;
			UltimateRadialButtonList[ i ].buttonTransform.localScale = Vector3.one;

			// If the user wants to scale the transform when interacted with...
			if( scaleTransform )
			{
				// Store the new position as the normal position to return to.
				UltimateRadialButtonList[ i ].normalPosition = normalPosition;

				// Configure the active position for the button.
				Vector3 activePosition = Vector3.zero;

				// Again, voodoo math, but the difference is that it is multiplying the radius modifier by the position modifier that the user has set.
				activePosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + ( 90 * Mathf.Deg2Rad ) + ( angleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * positionModifier ) ) );
				activePosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + ( 90 * Mathf.Deg2Rad ) + ( angleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * positionModifier ) ) );

				// Store the active position as the active position of the radial button.
				UltimateRadialButtonList[ i ].activePosition = activePosition;
			}

			// Store the angle of the radial button. This starts at 90 degrees to start the menu straight up. After that just add/subtract the angle information.
			UltimateRadialButtonList[ i ].angle = 90 + angleOffset + ( -angle * i ) - ( angle / 2 );

			// Store the min and max angles in relation to the angle just calculated above.
			UltimateRadialButtonList[ i ].minAngle = UltimateRadialButtonList[ i ].angle - ( ( angle / 2 ) * buttonInputAngle );
			UltimateRadialButtonList[ i ].maxAngle = UltimateRadialButtonList[ i ].angle + ( ( angle / 2 ) * buttonInputAngle );

			// Configure the new rotation to apply to the button.
			Vector3 newRotation = Vector3.zero;

			// If the user wants to follow the orbital rotation of the menu, then calculate the rotation plus the rotation offset.
			if( followOrbitalRotation )
				newRotation = new Vector3( 0, 0, ( -angle * i ) + rotationOffset );

			// Apply the rotation to the button transform.
			UltimateRadialButtonList[ i ].buttonTransform.rotation = Quaternion.Euler( newRotation );

			// -------------------------- < ICON POSITIONING > -------------------------- //
			if( UltimateRadialButtonList[ i ].useIcon && UltimateRadialButtonList[ i ].icon != null )
			{
				// Store the positioning information so that it can modified if need be.
				float horizontalPos = iconHorizontalPosition;
				float verticalPos = iconVerticalPosition;
				float sizeMod = iconSize;
				float rotationMod = iconRotation;

				// If the user wants to use this icon with unique positioning...
				if( UltimateRadialButtonList[ i ].useIconUnique )
				{
					// Modify the positioning information with the unique information.
					horizontalPos = UltimateRadialButtonList[ i ].iconHorizontalPosition;
					verticalPos = UltimateRadialButtonList[ i ].iconVerticalPosition;
					sizeMod = UltimateRadialButtonList[ i ].iconSize;
					rotationMod = UltimateRadialButtonList[ i ].iconRotation;
				}

				// Configure the position for the icon.
				Vector2 iconPosition = Vector3.zero;
				iconPosition.x += ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x * ( horizontalPos / 100 ) ) - ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x / 2 );
				iconPosition.y += ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y * ( verticalPos / 100 ) ) - ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y / 2 );

				// Apply the size and position to the icon.
				UltimateRadialButtonList[ i ].icon.rectTransform.sizeDelta = new Vector2( baseTransform.sizeDelta.x * sizeMod, baseTransform.sizeDelta.x * sizeMod ) * ( UltimateRadialButtonList[ i ].icon.sprite == null ? Vector2.one : GetImageAspectRatio( UltimateRadialButtonList[ i ].icon.sprite ) );
				UltimateRadialButtonList[ i ].icon.rectTransform.localPosition = iconPosition;

				// If the user wants to use local rotation then increase the rotation mod by the current button's rotation.
				if( iconLocalRotation )
				{
					// Store the image rotation.
					float imageRotation = UltimateRadialButtonList[ i ].radialImage.rectTransform.rotation.eulerAngles.z;

					// If the rotation is less than zero then add 360 to get a positive number.
					if( imageRotation < 0 )
						imageRotation += 360;

					// Store a temporary rotation modifier as the buttons rotation.
					rotationMod = UltimateRadialButtonList[ i ].buttonTransform.rotation.eulerAngles.z + ( UltimateRadialButtonList[ i ].useIconUnique ? UltimateRadialButtonList[ i ].iconRotation : iconRotation );

					// If the rotation is more than 90 degrees and less than 270, then increase the rotation by 180 to flip the icon.
					if( imageRotation > 90 && imageRotation < 270 )
						rotationMod += 180;
				}

				// Apply the rotation.
				UltimateRadialButtonList[ i ].icon.rectTransform.rotation = Quaternion.Euler( new Vector3( 0, 0, rotationMod ) );
			}

			// -------------------------- < TEXT POSITIONING > -------------------------- //
			if( UltimateRadialButtonList[ i ].text != null )//&& !RadialMenuButtonList[ i ].disableTextPositioning )
			{
				// Store the text position as the button transform by default.
				Vector2 textPosition = UltimateRadialButtonList[ i ].buttonTransform.position;

				// If the user wants to position the text relative to the icon then set the icon's position as the start position.
				if( textPositioningOption == TextPositioningOption.RelativeToIcon && UltimateRadialButtonList[ i ].icon != null )
					textPosition = UltimateRadialButtonList[ i ].icon.rectTransform.position;
				// Else if the user is wanting to position the text local to the button then set the position to zero.
				else if( textPositioningOption == TextPositioningOption.Local )
					textPosition = Vector2.zero;

				// Since the user might want to increase the are at which they can position the text, this Vector2 will be a larger area to position.
				Vector2 modifiedRefSizeForText = new Vector2( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x, UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y ) * 1.25f;
				textPosition.x += ( modifiedRefSizeForText.x * ( textHorizontalPosition / 100 ) ) - ( modifiedRefSizeForText.x / 2 );
				textPosition.y += ( modifiedRefSizeForText.y * ( textVerticalPosition / 100 ) ) - ( modifiedRefSizeForText.y / 2 );

				// Apply the size to the text transform.
				UltimateRadialButtonList[ i ].text.rectTransform.sizeDelta = new Vector2( baseTransform.sizeDelta.x * textSize, baseTransform.sizeDelta.x * textSize ) * new Vector2( textAreaRatioX, textAreaRatioY );

				// If the user wants to position the text in local position to the button...
				if( textPositioningOption == TextPositioningOption.Local )
				{
					// Store the image rotation.
					float imageRotation = UltimateRadialButtonList[ i ].radialImage.rectTransform.rotation.eulerAngles.z;

					// If the rotation is less than zero then add 360 to get a positive number.
					if( imageRotation < 0 )
						imageRotation += 360;

					// Store a temporary rotation modifier as the buttons rotation.
					float rotationMod = UltimateRadialButtonList[ i ].buttonTransform.rotation.eulerAngles.z;

					// Apply the local position to the text.
					UltimateRadialButtonList[ i ].text.rectTransform.localPosition = textPosition;

					// If the rotation is more than 90 degrees and less than 270, then increase the rotation by 180 to flip the text so it is readable.
					if( imageRotation > 90 && imageRotation < 270 )
						rotationMod += 180;

					// Apply the rotation to the text's transform.
					if( textLocalRotation )
						UltimateRadialButtonList[ i ].text.rectTransform.rotation = Quaternion.Euler( new Vector3( 0, 0, rotationMod ) );
					else
						UltimateRadialButtonList[ i ].text.rectTransform.rotation = Quaternion.identity;
				}
				else
				{
					// Since the user doesn't want to use local rotation, just apply the calculated text position and zero rotation.
					UltimateRadialButtonList[ i ].text.rectTransform.position = textPosition;
					UltimateRadialButtonList[ i ].text.rectTransform.rotation = Quaternion.identity;
				}
			}
		}

		// Reset the current button index since the menu has likely shifted in some way. This will allow the ProcessInput function to recalculate where the buttons are.
		CurrentButtonIndex = -1;

		// Apply the starting scale.
		baseTransform.localScale = startingScale;

		// Inform any subscribers that the Update Size and Placement function has run.
		if( OnUpdateSizeAndPlacement != null )
			OnUpdateSizeAndPlacement();
	}

	/// <summary>
	/// Updates the radial menu's position to the new position on the screen.
	/// </summary>
	/// <param name="screenPosition">The new position on the screen.</param>
	public void SetPosition ( Vector3 screenPosition )
	{
		if( baseTransform != null )
			baseTransform.position = screenPosition;
	}

	/// <summary>
	/// Enables the Ultimate Radial Menu so that it can be interacted with.
	/// </summary>
	public void EnableRadialMenu ()
	{
		// If the radial menu is already active, then return.
		if( RadialMenuActive )
			return;

		// Set the state to active.
		RadialMenuActive = true;

		// Depending on the options set by the user, start the correct coroutine.
		switch( radialMenuToggle )
		{
			default:
			case RadialMenuToggle.FadeAlpha:
			{
				StartCoroutine( FadeRadialMenu() );
			}
			break;
			case RadialMenuToggle.Scale:
			{
				StartCoroutine( ScaleRadialMenu() );
			}
			break;
		}

		// Notify any subscribers that the radial menu is now enabled.
		if( OnRadialMenuEnabled != null )
			OnRadialMenuEnabled();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu so that it can not be interacted with.
	/// </summary>
	public void DisableRadialMenu ()
	{
		// If the radial menu is already disabled, then return.
		if( !RadialMenuActive )
			return;

		// Set the state to inactive.
		RadialMenuActive = false;

		// Reset the radial menu so that it is ready for the next time it is enabled.
		ResetRadialMenu();

		// If the transitioning coroutine is not currently running...
		if( !transitioning )
		{
			// Start the correct coroutine according to the users options.
			switch( radialMenuToggle )
			{
				default:
				case RadialMenuToggle.FadeAlpha:
				{
					StartCoroutine( FadeRadialMenu() );
				}
				break;
				case RadialMenuToggle.Scale:
				{
					StartCoroutine( ScaleRadialMenu() );
				}
				break;
			}
		}

		// Set the input in range to false, just so that if the input manager was disabled, then other scripts can know that the input is no longer in range.
		InputInRange = false;

		// Notify any subscribers that the radial menu is now disabled.
		if( OnRadialMenuDisabled != null )
			OnRadialMenuDisabled();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu immediately so that it can not be interacted with.
	/// </summary>
	public void DisableRadialMenuImmediate ()
	{
		// Set the state to inactive.
		RadialMenuActive = false;

		// Reset the radial menu so that it is ready.
		ResetRadialMenu();

		// According to the users options, apply the disabled state of the radial menu.
		switch( radialMenuToggle )
		{
			default:
			case RadialMenuToggle.FadeAlpha:
			{
				canvasGroup.alpha = 0.0f;
			}
			break;
			case RadialMenuToggle.Scale:
			{
				baseTransform.localScale = Vector3.zero;
			}
			break;
		}

		// Notify any subscribers that the radial menu has been disabled.
		if( OnRadialMenuDisabled != null )
			OnRadialMenuDisabled();
	}

	/// <summary>
	/// Updates an existing radial button with new information.
	/// </summary>
	/// <param name="buttonName">The name of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public void UpdateRadialButton ( string buttonName, Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Store a temporary integer for the button index.
		int buttonIndex = GetRadialButtonIndexByName( buttonName );

		// If the index is less than zero then there is no button with this name, so return.
		if( buttonIndex < 0 )
			return;

		// Call the method to update the current buttons information.
		UpdateRadialButtonInformation( buttonIndex, newRadialButtonInfo );

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = UltimateRadialButtonList[ buttonIndex ];

		// Subscribe the Button Callback action to the radial button's callback.
		UltimateRadialButtonList[ buttonIndex ].OnRadialButtonInteract += ButtonCallback;
	}

	/// <summary>
	/// Updates an existing radial button with new information.
	/// </summary>
	/// <param name="buttonName">The name of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public void UpdateRadialButton ( string buttonName, Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Store a temporary integer for the button index.
		int buttonIndex = GetRadialButtonIndexByName( buttonName );

		// If the index is less than zero then there is no button with this name, so return.
		if( buttonIndex < 0 )
			return;

		// Call the method to update the current buttons information.
		UpdateRadialButtonInformation( buttonIndex, newRadialButtonInfo );

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = UltimateRadialButtonList[ buttonIndex ];

		// Subscribe the Button Callback action to the radial button's callback.
		UltimateRadialButtonList[ buttonIndex ].OnRadialButtonInteractWithId += ButtonCallback;
	}

	/// <summary>
	/// Updates an existing radial button with new information.
	/// </summary>
	/// <param name="buttonName">The name of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public void UpdateRadialButton ( string buttonName, Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Store a temporary integer for the button index.
		int buttonIndex = GetRadialButtonIndexByName( buttonName );

		// If the index is less than zero then there is no button with this name, so return.
		if( buttonIndex < 0 )
			return;

		// Call the method to update the current buttons information.
		UpdateRadialButtonInformation( buttonIndex, newRadialButtonInfo );

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = UltimateRadialButtonList[ buttonIndex ];

		// Subscribe the Button Callback action to the radial button's callback.
		UltimateRadialButtonList[ buttonIndex ].OnRadialButtonInteractWithKey += ButtonCallback;
	}

	/// <summary>
	/// Updates an existing radial button with new information.
	/// </summary>
	/// <param name="buttonIndex">The index of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public void UpdateRadialButton ( int buttonIndex, Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If the button doesn't exist at the index then return.
		if( !ConfirmRadialButtonIndex( buttonIndex ) )
			return;

		// Call the method to update the current buttons information.
		UpdateRadialButtonInformation( buttonIndex, newRadialButtonInfo );

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = UltimateRadialButtonList[ buttonIndex ];

		// Subscribe the Button Callback action to the radial button's callback.
		UltimateRadialButtonList[ buttonIndex ].OnRadialButtonInteract += ButtonCallback;
	}

	/// <summary>
	/// Updates an existing radial button with new information.
	/// </summary>
	/// <param name="buttonIndex">The index of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public void UpdateRadialButton ( int buttonIndex, Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If the button doesn't exist at the index then return.
		if( !ConfirmRadialButtonIndex( buttonIndex ) )
			return;

		// Call the method to update the current buttons information.
		UpdateRadialButtonInformation( buttonIndex, newRadialButtonInfo );

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = UltimateRadialButtonList[ buttonIndex ];

		// Subscribe the Button Callback action to the radial button's callback.
		UltimateRadialButtonList[ buttonIndex ].OnRadialButtonInteractWithId += ButtonCallback;
	}

	/// <summary>
	/// Updates an existing radial button with new information. This function should only be used on existing buttons.
	/// </summary>
	/// <param name="buttonIndex">The index of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public void UpdateRadialButton ( int buttonIndex, Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If the button doesn't exist at the index then return.
		if( !ConfirmRadialButtonIndex( buttonIndex ) )
			return;

		// Call the method to update the current buttons information.
		UpdateRadialButtonInformation( buttonIndex, newRadialButtonInfo );

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = UltimateRadialButtonList[ buttonIndex ];

		// Subscribe the Button Callback action to the radial button's callback.
		UltimateRadialButtonList[ buttonIndex ].OnRadialButtonInteractWithKey += ButtonCallback;
	}

	/// <summary>
	/// Creates a new radial menu button at the end of the list with the new information.
	/// </summary>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public void AddRadialButton ( Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Make a new radial menu button at the end of the list.
		UltimateRadialButton rmb = CreateRadialButtonAtIndex( 1000, newRadialButtonInfo );

		// If is it null then return.
		if( rmb == null )
			return;

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		rmb.OnRadialButtonInteract += ButtonCallback;

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = rmb;
	}

	/// <summary>
	/// Creates a new radial menu button at the end of the list with the new information.
	/// </summary>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public void AddRadialButton ( Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Make a new radial menu button at the end of the list.
		UltimateRadialButton rmb = CreateRadialButtonAtIndex( 1000, newRadialButtonInfo );

		// If is it null then return.
		if( rmb == null )
			return;

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		rmb.OnRadialButtonInteractWithId += ButtonCallback;

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = rmb;
	}

	/// <summary>
	/// Creates a new radial menu button at the end of the list with the new information.
	/// </summary>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public void AddRadialButton ( Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Make a new radial menu button at the end of the list.
		UltimateRadialButton rmb = CreateRadialButtonAtIndex( 1000, newRadialButtonInfo );

		// If is it null then return.
		if( rmb == null )
			return;

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		rmb.OnRadialButtonInteractWithKey += ButtonCallback;

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = rmb;
	}

	/// <summary>
	/// Creates a new radial menu button at the targeted index with the new information.
	/// </summary>
	/// <param name="buttonIndex">The index to insert a new radial button with the new radial button information.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public void InsertRadialButton ( int buttonIndex, Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Make a new radial menu button at the button index.
		UltimateRadialButton rmb = CreateRadialButtonAtIndex( buttonIndex, newRadialButtonInfo );

		// If is it null then return.
		if( rmb == null )
			return;

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		rmb.OnRadialButtonInteract += ButtonCallback;
		
		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = rmb;
	}

	/// <summary>
	/// Creates a new radial menu button at the targeted index with the new information.
	/// </summary>
	/// <param name="buttonIndex">The index to insert a new radial button with the new radial button information.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public void InsertRadialButton ( int buttonIndex, Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Make a new radial menu button at the button index.
		UltimateRadialButton rmb = CreateRadialButtonAtIndex( buttonIndex, newRadialButtonInfo );

		// If is it null then return.
		if( rmb == null )
			return;

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		rmb.OnRadialButtonInteractWithId += ButtonCallback;

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = rmb;
	}

	/// <summary>
	/// Creates a new radial menu button at the targeted index with the new information.
	/// </summary>
	/// <param name="buttonIndex">The index to insert a new radial button with the new radial button information.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public void InsertRadialButton ( int buttonIndex, Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// Make a new radial menu button at the button index.
		UltimateRadialButton rmb = CreateRadialButtonAtIndex( buttonIndex, newRadialButtonInfo );

		// If is it null then return.
		if( rmb == null )
			return;

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		rmb.OnRadialButtonInteractWithKey += ButtonCallback;

		// Assign the radialButton variable as this new radial button that was created.
		newRadialButtonInfo.radialButton = rmb;
	}

	/// <summary>
	/// Removes the radial button at the targeted index.
	/// </summary>
	/// <param name="buttonIndex">The index to remove the radial button at.</param>
	public void RemoveRadialButton ( int buttonIndex )
	{
		// If the count is currently 1, then just clear the list and return.
		if( UltimateRadialButtonList.Count == 1 )
		{
			ClearRadialButtons();
			return;
		}

		// If the button index is greater than the count, then just set it to the max index.
		if( buttonIndex > UltimateRadialButtonList.Count )
			buttonIndex = UltimateRadialButtonList.Count - 1;

		// Reduce the menu button count.
		menuButtonCount--;

		// Recalculate the angle offset.
		angleOffset = GetAnglePerButton / 2;

		// Destroy the radial button gameobject.
		Destroy( UltimateRadialButtonList[ buttonIndex ].buttonTransform.gameObject );

		// Remove the button at the targeted index.
		UltimateRadialButtonList.RemoveAt( buttonIndex );

		// Update the size and placement since the button count has been modified.
		UpdateSizeAndPlacement();

		// Inform any subscribers that the count has been modified.
		if( OnRadialMenuButtonCountModified != null )
			OnRadialMenuButtonCountModified( menuButtonCount );
	}

	/// <summary>
	/// Clears all of the radial menu buttons from the radial menu.
	/// </summary>
	public void ClearRadialButtons ()
	{
		// Loop through the radial button list and destroy the button gameobject.
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
			Destroy( UltimateRadialButtonList[ i ].buttonTransform.gameObject );

		// Clear the list.
		UltimateRadialButtonList.Clear();

		// Set the menu button count to zero to reset it.
		menuButtonCount = 0;

		// Inform any subscribers that the count has been modified.
		if( OnRadialMenuButtonCountModified != null )
			OnRadialMenuButtonCountModified( menuButtonCount );
	}
	// ------------------------------------------------ < END PUBLIC FUNCTIONS FOR THE USER > ------------------------------------------------ //

	/// <summary>
	/// Confirms the existence of the radial button and the targeted index.
	/// </summary>
	/// <param name="index">The index of the button to check.</param>
	bool ConfirmRadialButtonIndex ( int index )
	{
		// If the index is greater than the list count, then inform the use and return false.
		if( index > UltimateRadialButtonList.Count || index < 0 )
		{
			Debug.LogWarning( "Ultimate Radial Menu - The index is out of range for this radial menu." );
			return false;
		}
		return true;
	}

	// ----------------------------------------------------- < PUBLIC STATIC FUNCTIONS > ----------------------------------------------------- //
	/// <summary>
	/// Enables the targeted Ultimate Radial Menu so that it can be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void EnableRadialMenu ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].EnableRadialMenu();
	}

	/// <summary>
	/// Disables the targeted Ultimate Radial Menu so that it can not be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void DisableRadialMenu ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].DisableRadialMenu();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu immediately so that it can not be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void DisableRadialMenuImmediate ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].DisableRadialMenuImmediate();
	}

	/// <summary>
	/// Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonName">The name of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public static void UpdateRadialButton ( string radialMenuName, string buttonName, Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].UpdateRadialButton( buttonName, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.
	/// </summary>
	/// <param name="radialMenuName"></param>
	/// <param name="buttonName">The name of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public static void UpdateRadialButton ( string radialMenuName, string buttonName, Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].UpdateRadialButton( buttonName, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.
	/// </summary>
	/// <param name="radialMenuName"></param>
	/// <param name="buttonName">The name of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public static void UpdateRadialButton ( string radialMenuName, string buttonName, Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].UpdateRadialButton( buttonName, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonIndex">The index of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public static void UpdateRadialButton ( string radialMenuName, int buttonIndex, Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].UpdateRadialButton( buttonIndex, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.
	/// </summary>
	/// <param name="radialMenuName"></param>
	/// <param name="buttonIndex">The index of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public static void UpdateRadialButton ( string radialMenuName, int buttonIndex, Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].UpdateRadialButton( buttonIndex, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Updates an existing radial button with new information on the targeted radial menu. This function should only be used on existing buttons.
	/// </summary>
	/// <param name="radialMenuName"></param>
	/// <param name="buttonIndex">The index of the targeted radial button.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The information to apply to the radial button.</param>
	public static void UpdateRadialButton ( string radialMenuName, int buttonIndex, Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].UpdateRadialButton( buttonIndex, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Creates a new radial menu button at the end of the list with the new information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public static void AddRadialButton ( string radialMenuName, Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].AddRadialButton( ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Creates a new radial menu button at the end of the list with the new information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public static void AddRadialButton ( string radialMenuName, Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].AddRadialButton( ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Creates a new radial menu button at the end of the list with the new information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public static void AddRadialButton ( string radialMenuName, Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].AddRadialButton( ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Creates a new radial menu button at the targeted index with the new information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonIndex">The index to insert a new radial button with the new radial button information.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public static void InsertRadialButton ( string radialMenuName, int buttonIndex, Action ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;
		
		UltimateRadialMenus[ radialMenuName ].InsertRadialButton( buttonIndex, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Creates a new radial menu button at the targeted index with the new information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonIndex">The index to insert a new radial button with the new radial button information.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public static void InsertRadialButton ( string radialMenuName, int buttonIndex, Action<int> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].InsertRadialButton( buttonIndex, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Creates a new radial menu button at the targeted index with the new information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonIndex">The index to insert a new radial button with the new radial button information.</param>
	/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
	/// <param name="newRadialButtonInfo">The new radial button information to apply to the new button.</param>
	public static void InsertRadialButton ( string radialMenuName, int buttonIndex, Action<string> ButtonCallback, UltimateRadialButtonInfo newRadialButtonInfo )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].InsertRadialButton( buttonIndex, ButtonCallback, newRadialButtonInfo );
	}

	/// <summary>
	/// Removes the radial button at the targeted index.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonIndex">The index to remove the radial button at.</param>
	public static void RemoveRadialButton ( string radialMenuName, int buttonIndex )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RemoveRadialButton( buttonIndex );
	}

	/// <summary>
	/// Clears all of the radial menu buttons from the radial menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void ClearRadialButtons ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ClearRadialButtons();
	}

	/// <summary>
	/// Updates the targeted radial menu's position to the new position on the screen.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="screenPoint">The new position on the screen.</param>
	public static void RadialMenuToScreenPoint ( string radialMenuName, Vector3 screenPoint )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].SetPosition( screenPoint );
	}

	/// <summary>
	/// Returns the radial menu that has been registered with the targeted radial menu name.
	/// </summary>
	/// <param name="radialMenuName">The string name that the radial menu has been registered with.</param>
	public static UltimateRadialMenu GetUltimateRadialMenu ( string radialMenuName )
	{
		// If the radial menu does not exist then return null.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return null;

		return UltimateRadialMenus[ radialMenuName ];
	}
	// --------------------------------------------------- < END PUBLIC STATIC FUNCTIONS > --------------------------------------------------- //

	/// <summary>
	/// Confirms the existence of the radial menu that has been registered with the targeted name.
	/// </summary>
	/// <param name="radialMenuName">The string name that the radial menu has been registered with.</param>
	static bool ConfirmUltimateRadialMenu ( string radialMenuName )
	{
		// If the static radial menu dictionary does not contain the targeted radial menu key, then inform the user and return false.
		if( !UltimateRadialMenus.ContainsKey( radialMenuName ) )
		{
			Debug.LogWarning( "Ultimate Radial Menu - There is no Ultimate Radial Menu registered with the name: " + radialMenuName + " in the scene." );
			return false;
		}
		return true;
	}
}

[Serializable]
public class UltimateRadialButtonInfo
{
	public UltimateRadialMenu.UltimateRadialButton radialButton;
	public string key;
	public int id;
	
	public string name;
	public string description;
	public Sprite icon;

	/// <summary>
	/// Applies a new string to the radial button's text component.
	/// </summary>
	/// <param name="newText">The new string to apply to the radial button.</param>
	public void UpdateText ( string newText )
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// If the text component is null, then notify the user and return.
		if( radialButton.text == null )
		{
			Debug.LogWarning( "Ultimate Radial Button\nThe radial button's text component is not assigned. Please make sure that the radial button is using text and has a text component assigned." );
			return;
		}
		
		// Assign the new text to the text component.
		radialButton.text.text = newText;
	}

	/// <summary>
	/// Assigns a new sprite to the radial button's icon image.
	/// </summary>
	/// <param name="newIcon">The new sprite to assign as the icon for the radial button.</param>
	public void UpdateIcon ( Sprite newIcon )
	{
		// Assign the new icon.
		icon = newIcon;

		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// If the radial button icon is not assigned, then notify the user and return.
		if( radialButton.icon == null )
		{
			Debug.LogWarning( "Ultimate Radial Button\nThe radial button's icon image component is not assigned. Please make sure that the radial button is using an icon and has a image component assigned." );
			return;
		}
		
		// Apply the new icon to the radial button icon.
		radialButton.icon.sprite = newIcon;
	}

	/// <summary>
	/// Updates the radial button with a new name.
	/// </summary>
	/// <param name="newName">The new string to apply to the radial button's name.</param>
	public void UpdateName ( string newName )
	{
		// Assign the new name.
		name = newName;

		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Apply the name to the radial button.
		radialButton.name = name;

		// If the radial button is set to display the name and the text component is assigned, then apply the name.
		if( radialButton.radialMenu.displayNameOnButton && radialButton.text != null )
			radialButton.text.text = name;
	}

	/// <summary>
	/// Updates the radial button with a new description.
	/// </summary>
	/// <param name="newDescription">The new string to apply to the radial button's description.</param>
	public void UpdateDescription ( string newDescription )
	{
		// Assign the new description.
		description = newDescription;

		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Apply the description to the radial button.
		radialButton.description = description;
	}

	/// <summary>
	/// Enables the radial menu button.
	/// </summary>
	public void EnableButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Call the EnableButton() function all the radial button.
		radialButton.EnableButton();
	}

	/// <summary>
	/// Disables the radial menu button.
	/// </summary>
	public void DisableButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Call the DisableButton() function all the radial button.
		radialButton.DisableButton();
	}

	/// <summary>
	/// Deletes the radial menu button.
	/// </summary>
	public void RemoveRadialButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Remove the radial button from the menu.
		radialButton.radialMenu.RemoveRadialButton( radialButton.buttonIndex );
		radialButton = null;
	}

	/// <summary>
	/// Returns the existence of this information on a radial menu.
	/// </summary>
	public bool ExistsOnRadialMenu ()
	{
		// If the radial menu is assigned, then return true that this information is attached.
		if( radialButton != null && radialButton.radialMenu != null )
			return true;

		// Else, return false.
		return false;
	}

	/// <summary>
	/// Removes this information from the current radial button.
	/// </summary>
	public void RemoveInfoFromRadialButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		radialButton.ResetRadialButtonInformation();
		radialButton = null;
	}

	/// <summary>
	/// Returns the index that the radial menu button is assigned.
	/// </summary>
	public int GetButtonIndex
	{
		get
		{
			// If there is a radial button error, then just return 0.
			if( RadialButtonError )
				return 0;

			// Return the radial button's index.
			return radialButton.buttonIndex;
		}
	}

	/// <summary>
	/// Returns true if the radial button is not assigned and displays an error.
	/// </summary>
	bool RadialButtonError
	{
		get
		{
			// If the radial button is null...
			if( radialButton == null || radialButton.radialMenu == null )
			{
				// Inform the user that there is no radial button and return true for there being an error.
				Debug.LogWarning( "Ultimate Radial Button\nNo Radial Menu Button component has been assigned to this Ultimate Radial Button. Have you initialized a new Radial Menu Button using the AddRadialMenuButtonAtIndex function?" );
				return true;
			}
			return false;
		}
	}
}