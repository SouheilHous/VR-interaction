using UnityEngine;


namespace xDocEditorBase.AnnotationModule {

	public enum AnnotationInspectorGUIState
	{
		view,
		edit,
		editTransition,
		sizer,
		sizerCD,
		sizerSceneViewHeight,
		sizerSceneViewWidth
	}

	public class AnnotationInspectorGUIStateManager
	{
		private AnnotationInspectorGUIState _state;
		private AnnotationInspectorGUIState _requestedState;
		private bool _noRequest = true;

		readonly XDocAnnotationEditorBase aData;

		public AnnotationInspectorGUIStateManager(
			XDocAnnotationEditorBase aData
		)
		{
			this.aData = aData;
		
		}

		public AnnotationInspectorGUIState state {
			get { return _state; }
		}

		public bool isInViewMode {
			get { return _state == AnnotationInspectorGUIState.view; }
		}

		public bool isNotEdit {
			get { return _state != AnnotationInspectorGUIState.edit; }
		}

		public void RequestState(
			AnnotationInspectorGUIState requestedState
		)
		{
			_requestedState = requestedState;
			_noRequest = false;
			aData.Repaint();
		}

		public void RequestStateDefault()
		{
			RequestState(AnnotationInspectorGUIState.editTransition);
		}

		public void Update()
		{
			if (_noRequest) {
				return;
			}
			if (Event.current == null) {
				aData.Repaint();
				return;
			}
			if (Event.current.type != EventType.Repaint) {
				aData.Repaint();
				return;
			}
			_noRequest = true;
			_state = _requestedState;
			switch (state) {
			case AnnotationInspectorGUIState.edit:
				aData.annotationInspectorTextArea.focusGetter.GetFocus();
				break;
			case AnnotationInspectorGUIState.view:
				if (aData.annotationFocusState.hasFocus) {
					aData.annotationInspectorTextArea.focusGetter.LoseFocus();
					GUI.FocusControl("");
				}
				break;
			}
			aData.Repaint();
		}

	}
}

