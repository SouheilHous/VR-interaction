using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocEditorBase.AnnotationModule;


namespace xDocEditorBase.Inspectorbar {

	abstract public class InspectorbarModalElement
	{
		readonly XDocAnnotationEditorBase aData;
		readonly AnnotationInspectorGUIState state;

		bool requestOpen;

		abstract public void GetFocus();

		abstract public void Draw();

		abstract public void DrawHidden();

		protected InspectorbarModalElement(
			XDocAnnotationEditorBase aData,
			AnnotationInspectorGUIState state
		)
		{
			this.aData = aData;
			this.state = state;
		}

		public void ResetRequest()
		{
			requestOpen = false;
		}

		public void RequestOpen()
		{
			aData.guiStateManager.RequestState(state);
			GetFocus();
			requestOpen = true;
		}

		public void RequestClose()
		{
			aData.guiStateManager.RequestStateDefault();
		}

		public void ProcessOpenRequest()
		{
			if (!requestOpen) {
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
			aData.guiStateManager.RequestState(state);
			GetFocus();
		}
	}

	public static class Utility
	{
		public static Rect GetInspectorbarRect()
		{
			return EditorGUILayout.GetControlRect(
				false,
				EditorGUIUtility.singleLineHeight,
				AssetManager.settings.styleToolbarExpanded.style);
		}
	}
}