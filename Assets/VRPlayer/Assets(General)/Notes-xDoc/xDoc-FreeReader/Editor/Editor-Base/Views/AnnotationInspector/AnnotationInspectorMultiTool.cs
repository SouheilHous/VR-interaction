using UnityEngine;
using xDocBase.AssetManagement;
using xDocEditorBase.Inspectorbar;


namespace xDocEditorBase.AnnotationModule {

	public class AnnotationInspectorMultiTool
	{
		readonly XDocAnnotationEditorBase aData;
		readonly AnnotationInspectorToolbar titlebar;
		readonly AnnotationInspectorTitleLabel titleLabel;
		public readonly InspectorbarSlider sizer;
		public readonly InspectorbarSlider sizerCD;
		public readonly InspectorbarSlider sizerSceneViewWidth;
		public readonly InspectorbarSlider sizerSceneViewHeight;

		// _tidyup --- sizer stuff is not needed anymore - keep it for now, for documentation purposes - it does not really harm
		public AnnotationInspectorMultiTool(
			XDocAnnotationEditorBase aData
		)
		{
			this.aData = aData;
			titleLabel = new AnnotationInspectorTitleLabel(aData);
			titlebar = new AnnotationInspectorToolbar(aData);
			sizer = new InspectorbarSlider(
				aData,
				"Max.Height Text",
				aData.controlIdSizer,
				25,
				1000,
				aData.spMaxHeight,
				AnnotationInspectorGUIState.sizer
			);
			sizerCD = new InspectorbarSlider(
				aData,
				"Max.Height Data",
				aData.controlIdSizerCustomData,
				25,
				1000,
				aData.spMaxHeightCD,
				AnnotationInspectorGUIState.sizerCD
			);
			sizerSceneViewWidth = new InspectorbarSlider(
				aData,
				"Max.Width SV",
				aData.controlIdSizerSceneViewWidth,
				0,
				1000,
				aData.spMaxWidthSceneView,
				AnnotationInspectorGUIState.sizerSceneViewWidth
			);
			sizerSceneViewHeight = new InspectorbarSlider(
				aData,
				"Max.Height SV",
				aData.controlIdSizerSceneViewHeight,
				0,
				1000,
				aData.spMaxHeightSceneView,
				AnnotationInspectorGUIState.sizerSceneViewHeight
			);

			aData.annotationInspectorMultiTool = this;

			ResetOpenRequests();
		}

		public void Draw()
		{
			if (!AssetManager.canWrite) {
				titleLabel.Draw();
				return;
			}

			switch (aData.guiStateManager.state) {
			case AnnotationInspectorGUIState.view:
				titleLabel.Draw();
				if (aData.annotationFocusState.hasFocus) {
					aData.guiStateManager.RequestState(AnnotationInspectorGUIState.editTransition);
				}
				break;
			case AnnotationInspectorGUIState.editTransition:
				ResetOpenRequests();
				aData.annotationInspectorTextArea.focusGetter.GetFocus();
				titlebar.Draw();
				aData.guiStateManager.RequestState(AnnotationInspectorGUIState.edit);
				break;
			case AnnotationInspectorGUIState.edit:
				titlebar.Draw();
				DrawHiddenControls();
				LostFocusCheck();
				ProcessOpenRequests();
				break;
			case AnnotationInspectorGUIState.sizer:
				sizer.Draw();
				LostFocusCheck();
				break;
			case AnnotationInspectorGUIState.sizerCD:
				sizerCD.Draw();
				LostFocusCheck();
				break;
			case AnnotationInspectorGUIState.sizerSceneViewWidth:
				sizerSceneViewWidth.Draw();
				LostFocusCheck();
				break;
			case AnnotationInspectorGUIState.sizerSceneViewHeight:
				sizerSceneViewHeight.Draw();
				LostFocusCheck();
				break;
			}
		}

		void LostFocusCheck()
		{
			if (aData.annotationFocusState.hasFocus) {
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
			
			aData.guiStateManager.RequestState(AnnotationInspectorGUIState.view);
			aData.annotationInspectorTextArea.focusGetter.LoseFocus();
		}


#region Modal Elements

		void DrawHiddenControls()
		{
			sizer.DrawHidden();
			sizerCD.DrawHidden();
			sizerSceneViewWidth.DrawHidden();
			sizerSceneViewHeight.DrawHidden();
		}

		void ProcessOpenRequests()
		{
			sizer.ProcessOpenRequest();
			sizerCD.ProcessOpenRequest();
			sizerSceneViewWidth.ProcessOpenRequest();
			sizerSceneViewHeight.ProcessOpenRequest();
		}

		public void ResetOpenRequests()
		{
			sizer.ResetRequest();
			sizerCD.ResetRequest();
			sizerSceneViewWidth.ResetRequest();
			sizerSceneViewHeight.ResetRequest();
		}

#endregion



	}
}

