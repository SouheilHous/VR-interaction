using System.Collections.Generic;
using UnityEngine;


namespace xDocEditorBase.Focus {

	public static class FocusUtility
	{
	
		public static void LoseFocus()
		{
			GUI.FocusControl("");
		}

		public static bool HasFocus(
			string focusId
		)
		{
			var focusedControl = GUI.GetNameOfFocusedControl();
			// The following checks are not needed. Calling routines must make sure to
			// use this routine in the correct setup.
//		if (focusedControl.Equals(string.Empty)) {
//			return false;
//		}
//		if (focusedControl.Equals("")) {
//			return false;
//		}
//		if (focusId == null) {
//			return false;
//		}
			return focusedControl.StartsWith(focusId);
		}
	}

	/// <summary>
	/// The Focus manager deals with "events" all around UI elements and their focuses.
	/// 
	/// E.g. a UI element loses focus or a focused UI element receives a keyboard event.
	/// 
	/// "Subscribers" can outsource their "watchdog" tasks to the focus manager, which
	/// just needs a call to the Update function once every cycle. 
	/// </summary>
	public class FocusManager
	{

		/// <summary>
		/// The last focus.
		/// </summary>
		public string lastFocus = "";
		/// <summary>
		/// The current focus.
		/// </summary>
		public string currentFocus = "";

		readonly List<TriggerBase> onGotFocusTriggers;
		readonly List<TriggerBase> onLostFocusTriggers;
		readonly List<TriggerBase> onKeyPressReturnTriggers;

		public bool IsSubFocusChange()
		{
			return FocusId.IsSubFocusChange(lastFocus, currentFocus);
		}

		public bool HasFocus(
			string focusId
		)
		{
			return currentFocus.StartsWith(focusId);
		}

		public FocusManager()
		{
			onGotFocusTriggers = new List<TriggerBase>();
			onLostFocusTriggers = new List<TriggerBase>();
			onKeyPressReturnTriggers = new List<TriggerBase>();
		}

		/// <summary>
		/// !!! 
		/// !!! Put this call always at the end of the draw cycle. 
		/// !!! So there is no change betwwen the Layout and Repaint run.
		/// !!! (Only relevant, if you interactively change ui layouts)
		/// !!! 
		/// 
		/// Update this instance and check if focus CHANGE events accured and 
		/// callback have to be called. Let them be called in this case.
		/// </summary>
		public void Update()
		{
			// Only during Repaint focus seems to have valid values
			if (Event.current.type == EventType.Repaint) {
				ForceUpdate();
			}

		}

		/// <summary>
		/// !!! 
		/// !!! Put this call always at the end of the draw cycle. 
		/// !!! So there is no change betwwen the Layout and Repaint run.
		/// !!! (Only relevant, if you interactively change ui layouts)
		/// !!! 
		/// 
		/// Runs the update even, if it is not the Repaint run. As a effect,
		/// clicks on buttons, will be interpreted as lost / got focus.
		/// This you may want or not. Try both versions to get the desired
		/// effect.
		/// 
		/// Update this instance and check if focus CHANGE events accured and 
		/// callback have to be called. Let them be called in this case.
		/// </summary>
		public void ForceUpdate()
		{
			currentFocus = GUI.GetNameOfFocusedControl();

			// Actually these are focus changed events
			if (!lastFocus.Equals(currentFocus)) {
				// Focus lost check
				foreach ( TriggerBase item in onLostFocusTriggers ) {
					item.TryToTrigger(lastFocus);
				}
				// Focus gotten check
				foreach ( TriggerBase item in onGotFocusTriggers ) {
					item.TryToTrigger(currentFocus);
				}
			}

//		// Key Pressed
//		if (Event.current != null) {
//			// Sometimes there is no Event.current when update is called
//			if (Event.current.type == EventType.KeyDown) {
//				// if other key events shall be monitored as well, they 
//				// can / should be placed in these bracets  
//				if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
//					foreach ( TriggerBase item in onKeyPressReturnTriggers ) {
//						item.TryToTrigger(currentFocus);
//					}
//				}
//			}
//		}
	
			// keep track of the focusses
			lastFocus = currentFocus;
		}


		/// <summary>
		/// This function exists, bc. Keypresses have to be evaluated at the beginning of a Draw/OnGUI cycle.
		/// 
		/// This does not work without Update called as well, because Update takes care about following
		/// the focuses
		/// </summary>
		public void ForceKeyPressPreCheck()
		{
			// TO DO bad code design - doubled code as above - tidy up may be needed
			// Key Pressed
			if (Event.current != null) {
				// Sometimes there is no Event.current when update is called
				if (Event.current.type == EventType.KeyDown) {
					// if other key events shall be monitored as well, they 
					// can / should be placed in these bracets  
					if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
						foreach ( TriggerBase item in onKeyPressReturnTriggers ) {
							item.TryToTrigger(currentFocus);
						}
					}
				}
			}
		}



#region Add Trigger: LOST FOCUS

		/// <summary>
		/// Adds the on lost focus trigger callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnLostFocusTriggerCallback(
			Callback<string> callback,
			string trigger
		)
		{
			onLostFocusTriggers.Add(new Trigger(trigger, callback));
		}


		/// <summary>
		/// Adds the on lost focus trigger string callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnLostFocusTriggerStringCallback(
			Callback<string> callback,
			string trigger
		)
		{
			onLostFocusTriggers.Add(new TriggerString(trigger, callback));
		}

		/// <summary>
		/// Adds the on lost focus trigger int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnLostFocusTriggerIntCallback(
			Callback<int> callback,
			string trigger
		)
		{
			onLostFocusTriggers.Add(new TriggerInt(trigger, callback));
		}

		/// <summary>
		/// Adds the on lost focus trigger int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnLostFocusTriggerIntIntCallback(
			Callback<int,int> callback,
			string trigger
		)
		{
			onLostFocusTriggers.Add(new TriggerIntInt(trigger, callback));
		}

#endregion


#region Add Trigger: GOT FOCUS


		/// <summary>
		/// Adds the on Got focus trigger callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnGotFocusTriggerCallback(
			Callback<string> callback,
			string trigger
		)
		{
			onGotFocusTriggers.Add(new Trigger(trigger, callback));
		}

		/// <summary>
		/// Adds the on Got focus trigger callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnGotFocusTriggerStringCallback(
			Callback<string> callback,
			string trigger
		)
		{
			onGotFocusTriggers.Add(new TriggerString(trigger, callback));
		}

		/// <summary>
		/// Adds the on lost focus trigger int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnGotFocusTriggerIntCallback(
			Callback<int> callback,
			string trigger
		)
		{
			onGotFocusTriggers.Add(new TriggerInt(trigger, callback));
		}

		/// <summary>
		/// Adds the on lost focus trigger int-int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnGotFocusTriggerIntIntCallback(
			Callback<int,int> callback,
			string trigger
		)
		{
			onGotFocusTriggers.Add(new TriggerIntInt(trigger, callback));
		}

#endregion


#region Add Trigger: KeyPress

		/// <summary>
		/// Adds the on key press return trigger int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnKeyPressReturnTriggerCallback(
			Callback<string> callback,
			string trigger
		)
		{
			onKeyPressReturnTriggers.Add(new Trigger(trigger, callback));
		}

		/// <summary>
		/// Adds the on key press return trigger int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnKeyPressReturnTriggerStringCallback(
			Callback<string> callback,
			string trigger
		)
		{
			onKeyPressReturnTriggers.Add(new TriggerString(trigger, callback));
		}


		/// <summary>
		/// Adds the on key press return trigger int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnKeyPressReturnTriggerIntCallback(
			Callback<int> callback,
			string trigger
		)
		{
			onKeyPressReturnTriggers.Add(new TriggerInt(trigger, callback));
		}

		/// <summary>
		/// Adds the on key press return trigger int callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="trigger">Trigger.</param>
		public void AddOnKeyPressReturnTriggerIntIntCallback(
			Callback<int,int> callback,
			string trigger
		)
		{
			onKeyPressReturnTriggers.Add(new TriggerIntInt(trigger, callback));
		}

#endregion

	}
}

