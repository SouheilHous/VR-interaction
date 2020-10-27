using UnityEngine;


namespace xDocEditorBase.Focus {

	public class FocusState
	{
		bool _hasFocus = false;
		readonly string focusBaseId;

		public FocusState(
			string focusBaseId
		)
		{
			this.focusBaseId = focusBaseId;
		}

		public bool hasFocus {
			get { 
				return _hasFocus; 
			}
		}

		/// <summary>
		/// !!! 
		/// !!! Put this call always at the end of the draw cycle. 
		/// !!! So there is no change betwwen the Layout and Repaint run.
		/// !!! (Only relevant, if you interactively change ui layouts)
		/// !!! 
		/// 
		/// Changes are only made when it is the Repaint run
		/// 
		/// Update this instance and check if focus CHANGE events accured and 
		/// callback have to be called. Let them be called in this case.
		/// </summary>
		public void Update()
		{
			if (Event.current == null)
				return;
			if (Event.current.type == EventType.Repaint) {
				_hasFocus = GUI.GetNameOfFocusedControl().StartsWith(focusBaseId);
			} 
		}

	}
}
