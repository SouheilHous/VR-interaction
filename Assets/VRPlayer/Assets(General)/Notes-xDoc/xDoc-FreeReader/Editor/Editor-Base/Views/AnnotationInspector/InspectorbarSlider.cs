using UnityEditor;
using UnityEngine;
using xDocEditorBase.AnnotationModule;
using xDocEditorBase.Focus;
using xDocEditorBase.UI;


namespace xDocEditorBase.Inspectorbar {

	public class InspectorbarSlider : InspectorbarModalElement
	{
		readonly XDocAnnotationEditorBase aData;
		readonly FocusGetter focusGetter;
		readonly FocusState focusState;
		readonly SerializedProperty sProp;
		readonly float maxValue;
		readonly float minValue;
		readonly string controlId;
		readonly string label;

		Rect sliderRect;

		public InspectorbarSlider(
			XDocAnnotationEditorBase aData,
			string label,
			string controlId,
			float minValue,
			float maxValue,
			SerializedProperty sProp,
			AnnotationInspectorGUIState state
		)
			: base(
				aData,
				state
			)
		{
			this.aData = aData;
			this.label = label;
			this.sProp = sProp;
			this.maxValue = maxValue;
			this.minValue = minValue;
			this.controlId = controlId;
			focusGetter = new FocusGetter(aData, controlId);
			focusState = new FocusState(controlId);
		}

		public override void GetFocus()
		{
			focusGetter.GetFocus();
			focusState.Update();
		}

		public override void Draw()
		{
			// --- slider
			const float closeButtonWidth = 60f;
			const float spacing = 2f;
			sliderRect = Utility.GetInspectorbarRect();
			sliderRect.width -= closeButtonWidth + spacing * 2;

			if (sProp.hasMultipleDifferentValues)
				EditorGUI.showMixedValue = true;
			float currentMaxHeight = sProp.floatValue;
			FocusId.SetNextId(controlId);
			float newVal = EditorGUI.Slider(sliderRect, label, currentMaxHeight, minValue, maxValue);

			// reset for the next run
			EditorGUI.showMixedValue = false;
			const float ePSILON = 1f;
			if (Mathf.Abs(newVal - currentMaxHeight) > ePSILON) {
				sProp.floatValue = newVal;
				aData.serializedObject.ApplyModifiedProperties();
			}

			// --- Close Button
			sliderRect.x += sliderRect.width + spacing * 2;
			sliderRect.width = closeButtonWidth - spacing;
			if (GUI.Button(sliderRect, "Close", EditorStyles.toolbarButton)) {
				RequestClose();
			}
			if (!focusState.hasFocus) {
				RequestClose();
			}

			focusGetter.Update();
			focusState.Update();
		}

		public override void DrawHidden()
		{
			FocusId.SetNextId(controlId);
			EditorGUI.Slider(XoxEditorGUI.hiddenRect, 0, 0, 1);
			GUI.Button(XoxEditorGUI.hiddenRect, GUIContent.none, XoxEditorGUI.hideStyle); 
		}

	}
}

