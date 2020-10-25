/* UltimateRadialMenuPointer.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu( "Ultimate Radial Menu/Pointer" )]
public class UltimateRadialMenuPointer : MonoBehaviour
{
	public UltimateRadialMenu ultimateRadialMenu;

	// POINTER POSITIONING //
	public RectTransform radialMenuPointer;
	public float pointerSize = 0.25f, targetingSpeed = 5.0f;
	public enum SnappingOption
	{
		Instant,
		SnapToButton,
		Free
	}
	public SnappingOption snappingOption = SnappingOption.SnapToButton;
	Quaternion targetRotation;
	public float rotationOffset = 90;

	// VISUAL OPTIONS //
	public bool alwaysOn = false;
	public bool colorChange = false, changeOverTime = false;
	public Image colorChangeImage;
	public float fadeInDuration = 0.25f, fadeOutDuration = 0.5f;
	public Color normalColor = Color.white, activeColor = Color.white;
	public bool spriteSwap = false;
	public Image spriteSwapImage;
	public Sprite normalSprite, activeSprite;
	bool radialMenuFocused = false;


	void Awake ()
	{
		// If the game is not running, then return.
		if( !Application.isPlaying )
			return;

		// If the radial menu is null...
		if( ultimateRadialMenu == null )
		{
			// Attempt to find it in the parent gameobject.
			ultimateRadialMenu = GetComponentInParent<UltimateRadialMenu>();

			// If the menu is still null, then send an error to the console and disable this component.
			if( ultimateRadialMenu == null )
			{
				Debug.LogError( "Ultimate Radial Menu Pointer\nThere is not a Ultimate Radial Menu assigned to this pointer. This component was not able to find a Ultimate Radial Menu in any parent objects either. Disabling this component to avoid errors." );
				enabled = false;
			}
		}
	}

	void Start ()
	{
		// If the game is not running, then return.
		if( !Application.isPlaying )
			return;

		// Subscribe to the needed events of the radial menu.
		ultimateRadialMenu.OnUpdateSizeAndPlacement += UpdateSizeAndPlacement;
		ultimateRadialMenu.OnRadialMenuButtonFound += OnRadialMenuItemFound;
		ultimateRadialMenu.OnRadialMenuLostFocus += OnRadialMenuLostFocus;

		// If the user wants to change colors, then apply the normal color here.
		if( colorChange & colorChangeImage != null )
			colorChangeImage.color = normalColor;

		// If the user want to swap sprites then apply that here.
		if( spriteSwap && spriteSwapImage != null )
			spriteSwapImage.sprite = normalSprite;
	}

	/// <summary>
	/// This function will be called when the radial menu loses focus.
	/// </summary>
	void OnRadialMenuLostFocus ()
	{
		// If the button pointer is current on and the user doesn't want it to always be on...
		if( radialMenuFocused && !alwaysOn )
		{
			// If the user wants to change the color, but not over time, then apply the normal color.
			if( colorChange && colorChangeImage != null && !changeOverTime )
				colorChangeImage.color = normalColor;

			// If the user wants to swap sprites, then apply the normal sprite.
			if( spriteSwap && spriteSwapImage != null )
				spriteSwapImage.sprite = normalSprite;
		}

		// Set radial menu focused to false so that the other functions know it is not longer in focus.
		radialMenuFocused = false;
	}

	/// <summary>
	/// This function will be called when the radial menu selects a new button.
	/// </summary>
	/// <param name="index">The index of the new button found.</param>
	void OnRadialMenuItemFound ( int index )
	{
		// If the radial pointer is assigned...
		if( radialMenuPointer != null )
		{
			// Store the target rotation to being the angle of the radial button at the index.
			targetRotation = Quaternion.Euler( 0, 0, ultimateRadialMenu.UltimateRadialButtonList[ index ].angle - rotationOffset );

			// If the radial menu has not been focused yet, or the user wants to apply the rotation instantly, then simply apply the rotation.
			if( !radialMenuFocused || snappingOption == SnappingOption.Instant )
				radialMenuPointer.rotation = targetRotation;
		}

		// If the pointer is not currently on and the user doesn't want it to always be on...
		if( !radialMenuFocused && !alwaysOn )
		{
			// If the user wants to change color...
			if( colorChange && colorChangeImage != null )
			{
				// If the user wants to change color over time then start the coroutine.
				if( changeOverTime )
					StartCoroutine( UpdateColor() );
				// Else just apply the active color.
				else
					colorChangeImage.color = activeColor;
			}

			// If the user wants to swap sprites, then apply the active sprite.
			if( spriteSwap && spriteSwapImage != null )
				spriteSwapImage.sprite = activeSprite;
		}

		// Set radialMenuFocused as true so that other functions know that the pointer has already been focused.
		radialMenuFocused = true;
	}

	void Update ()
	{
		// If the game is not running, then simply updated the positioning and return.
		if( !Application.isPlaying )
		{
			UpdateSizeAndPlacement();
			return;
		}

		// If the user wants to apply the rotation instantly( which has already been done in the OnRadialMenuItemFound() function ) or the pointer transform is null, then return.
		if( snappingOption == SnappingOption.Instant || radialMenuPointer == null )
			return;

		// If the snapping option is set to free, then transition the rotation to the current angle of the input.
		if( snappingOption == SnappingOption.Free )
			radialMenuPointer.rotation = Quaternion.Slerp( radialMenuPointer.rotation, Quaternion.Euler( 0, 0, ultimateRadialMenu.GetCurrentInputAngle - rotationOffset ), Time.unscaledDeltaTime * targetingSpeed );
		// Else transition the rotation to the target rotation of the currently selected button.
		else
			radialMenuPointer.rotation = Quaternion.Slerp( radialMenuPointer.rotation, targetRotation, Time.unscaledDeltaTime * targetingSpeed );
	}

	/// <summary>
	/// This function will be called with the radial menu updates it's size and positioning.
	/// </summary>
	void UpdateSizeAndPlacement ()
	{
		// If the ultimate radial menu is left unassigned, then return to avoid errors.
		if( ultimateRadialMenu == null )
			return;

		// If the radial pointer is null, then try to get the component on this object.
		if( radialMenuPointer == null )
			radialMenuPointer = GetComponent<RectTransform>();

		// If the radial pointer is now assigned...
		if( radialMenuPointer != null )
		{
			// Store a new pointer size based off of the radial menu's size multiplied by the pointerSize option set by the user.
			float _pointerSize = ultimateRadialMenu.GetComponent<RectTransform>().sizeDelta.x * pointerSize;

			// Apply the size position to the pointer transform.
			radialMenuPointer.sizeDelta = new Vector2( _pointerSize, _pointerSize );
			radialMenuPointer.position = ultimateRadialMenu.BasePosition;

			// If the game is not running, then apply the rotation to look at the first button plus the rotation offset that the user has set.
			if( !Application.isPlaying &&  ultimateRadialMenu.UltimateRadialButtonList.Count > 0 )
				radialMenuPointer.rotation = Quaternion.Euler( 0, 0, ultimateRadialMenu.UltimateRadialButtonList[ 0 ].angle - rotationOffset );
		}
	}

	/// <summary>
	/// Changes the color of the image over time.
	/// </summary>
	IEnumerator UpdateColor ()
	{
		// Set the radial menu focused as true so that this coroutine will at least run the first frame.
		radialMenuFocused = true;

		// Store a temporary float for the speed of the fade in transition.
		float fadeInSpeed = 1.0f / fadeInDuration;
		for( float t = 0.0f; t < 1.0f && radialMenuFocused; t += Time.unscaledDeltaTime * fadeInSpeed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( fadeInSpeed ) )
				break;

			// Transition the color from normal to active by t.
			colorChangeImage.color = Color.Lerp( normalColor, activeColor, t );
			yield return null;
		}

		// If the pointer is still focused then apply the final color.
		if( radialMenuFocused )
			colorChangeImage.color = activeColor;

		// While the radial menu is focused, wait here.
		while( radialMenuFocused )
			yield return null;

		// Configure the fade out speed.
		float fadeOutSpeed = 1.0f / fadeOutDuration;
		for( float t = 0.0f; t < 1.0f && !radialMenuFocused; t += Time.unscaledDeltaTime * fadeOutSpeed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( fadeOutDuration ) )
				break;

			// Transition the color from active to normal by t.
			colorChangeImage.color = Color.Lerp( activeColor, normalColor, t );
			yield return null;
		}

		// If the radial menu is still not focused then apply the normal.
		if( !radialMenuFocused )
			colorChangeImage.color = normalColor;
	}
}