/* UltimateRadialMenuScreenSizeUpdater.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UltimateRadialMenuScreenSizeUpdater : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange ()
	{
		StartCoroutine( "YieldPositioning" );
	}

	IEnumerator YieldPositioning ()
	{
		yield return new WaitForEndOfFrame();

		UltimateRadialMenu[] allRadialMenus = FindObjectsOfType( typeof( UltimateRadialMenu ) ) as UltimateRadialMenu[];

		for( int i = 0; i < allRadialMenus.Length; i++ )
			allRadialMenus[ i ].UpdateSizeAndPlacement();
	}
}