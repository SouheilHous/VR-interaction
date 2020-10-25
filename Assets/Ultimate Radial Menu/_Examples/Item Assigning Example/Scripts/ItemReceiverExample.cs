using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemReceiverExample : MonoBehaviour, IDragHandler, IPointerUpHandler
{
	RectTransform myTransform;
	Vector3 originalPosition;

	public UltimateRadialButtonInfo newRadialButtonInfo;

	int itemCount = 0;

	static List<int> usedIndex = new List<int>();

	public Sprite placeholderIcon;


	void Start ()
	{
		// Store this transform.
		myTransform = GetComponent<RectTransform>();

		// Store the starting position so that it can return to it after being released.
		originalPosition = myTransform.localPosition;
	}

	public void OnDrag ( PointerEventData eventData )
	{
		// When the user drags to pointer, move this transform with the pointer.
		myTransform.position = eventData.position;
	}

	public void OnPointerUp ( PointerEventData eventData )
	{
		// When the pointer is released, get the Ultimate Radial Menu's current button index.
		int index = UltimateRadialMenu.GetUltimateRadialMenu( "ItemWheelExample" ).CurrentButtonIndex;

		// If the index is greater than zero ( meaning that the input was on the radial menu ), and this button information does not currently exist on the menu...
		if( index >= 0 && !usedIndex.Contains( index ) && !newRadialButtonInfo.ExistsOnRadialMenu() )
		{
			// Update the radial button at the targeted index with this information, and register the ButtonCallback function as the ButtonCallback parameter.
			UltimateRadialMenu.UpdateRadialButton( "ItemWheelExample", index, UseItem, newRadialButtonInfo );

			// Add this index to the used index list so that other buttons know that this button is taken.
			usedIndex.Add( index );
		}

		// Increase this items count.
		itemCount++;

		// If this button does exist on the radial menu, then update the text of the button.
		if( newRadialButtonInfo.ExistsOnRadialMenu() )
			newRadialButtonInfo.UpdateText( itemCount.ToString() );

		// Update this transform back to the original position.
		myTransform.localPosition = originalPosition;
	}

	void UseItem ()
	{
		// Decrease the item count.
		itemCount--;

		// Update the text of the button.
		newRadialButtonInfo.UpdateText( itemCount.ToString() );

		// If the count is less than 0, then the item is all used up...
		if( itemCount <= 0 )
		{
			// Remove the index from the usedIndex list so that other items know that this button is available.
			usedIndex.Remove( newRadialButtonInfo.GetButtonIndex );

			newRadialButtonInfo.UpdateText( "Text" );
			newRadialButtonInfo.UpdateIcon( placeholderIcon );

			// Remove the button from the list.
			newRadialButtonInfo.RemoveInfoFromRadialButton();
		}
	}
}