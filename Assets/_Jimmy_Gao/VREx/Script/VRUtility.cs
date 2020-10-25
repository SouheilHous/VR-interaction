using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace JimmyGao
{
	public class VRUtility  {

		public static Hand CheckTriggerDown(Hand hand1, Hand hand2)
		{

			return null;
		}

		public static Hand CheckMenuButton(Hand hand1, Hand hand2)
		{
///TODO
			// if (hand1 != null) {
			// 	SteamVR_Controller.Device device1 = hand1.controller;

			// 	if (device1!=null && device1.GetPressDown (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu)) {
			// 		return hand1;
			// 	}
			// }


			// if (hand2 != null) {
			// 	SteamVR_Controller.Device device2 = hand2.controller;
			// 	if (device2!=null && device2.GetPressDown (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu)) {
			// 		return hand2;
			// 	}
			// }

			return null;
		}

		//1 up 2 down 3 left 4 right  , 0 no touch , -1 error
		public  static int CheckTouchPad(Hand hand)
		{
			//TODO:
			// if (hand == null)
			// 	return-1;
			// SteamVR_Controller.Device device = hand.controller;


			// if (device == null)
			// 	return -1;
			// if (!device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
			// 	return 0;
			// Vector2 touchposVec = device.GetAxis ();
			// float touchangle = JimmyUtility.VectorAngle(new Vector2(1, 0), touchposVec); 
			// int touchPos = JimmyUtility.AngleToPosition (touchangle);
			// return touchPos;
			return 3;
		}

		/*
		public static GameObject GetAttachedObj(Hand hand)
		{

			foreach (Hand.AttachedObject obj in hand.AttachedObjects) {
				if (obj.attachedObject.gameObject.GetComponent<ContentControl> () != null) {
					return obj.attachedObject.gameObject;
				}
			}

			return null;

		}*/

		public static GameObject GetHovingObj(Hand hand)
		{
			if (hand.hoveringInteractable != null) {
				return hand.hoveringInteractable.gameObject;
			} else
				return null;


		}
	}


}

