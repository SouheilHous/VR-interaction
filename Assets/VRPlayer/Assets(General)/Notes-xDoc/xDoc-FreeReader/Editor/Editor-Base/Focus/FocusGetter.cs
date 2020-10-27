using UnityEngine;
using xDocEditorBase.AnnotationModule;


namespace xDocEditorBase.Focus {

	public class FocusGetter
	{
		private bool getFocus;
		private bool loseFocus;
		private readonly string controlId;
		private readonly XDocAnnotationEditorBase aData;

		public FocusGetter(
			XDocAnnotationEditorBase aData,
			string controlId
		)
		{
			this.aData = aData;
			this.controlId = controlId;
			getFocus = false;
			loseFocus = false;
		}

		public void GetFocus()
		{
			getFocus = true;
			Update();
			getFocus = true;
		}

		public void LoseFocus()
		{
			loseFocus = true;
			Update();
		}

		public void Update()
		{
			if (getFocus) {
				if (!GUI.GetNameOfFocusedControl().StartsWith(controlId)) {
					GUI.FocusControl(controlId);
					aData.Repaint();
				}
				if (Event.current == null)
					return;
				if (Event.current.type == EventType.Repaint)
					getFocus = false;
			} else if (loseFocus) {
				GUI.FocusControl("");
				aData.Repaint();
				if (Event.current == null)
					return;
				if (Event.current.type == EventType.Repaint)
					loseFocus = false;
			}
		}
	}
}