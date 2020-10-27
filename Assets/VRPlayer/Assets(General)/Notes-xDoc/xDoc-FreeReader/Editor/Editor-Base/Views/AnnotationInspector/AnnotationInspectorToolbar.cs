using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocEditorBase.AnnotationModule {

	public class AnnotationInspectorToolbar
	{
		readonly Inspectorbar.Bumper startBumper;
		readonly Inspectorbar.Filler endFiller;
		readonly TextAreaMenuAnnotationType typeButton;
		readonly TextAreaMenuEdit editButton;
		readonly TextAreaMenuStyle styleButton;
		readonly TextAreaMenuColor colorButton;
		readonly TextAreaMenuSize sizeButton;
		readonly TextAreaMenuOptions optionsButton;

		XDocAnnotationEditorBase aData;

		Rect totalRect;
		float neededWidth;
		float excessWidth;
		float scaleFactor = 1;

		public AnnotationInspectorToolbar(
			XDocAnnotationEditorBase aData
		)
		{ 
			if (!AssetManager.isFunctional) {
				return;
			}

			this.aData = aData;
			startBumper = new Inspectorbar.Bumper(6);
			typeButton = new TextAreaMenuAnnotationType(aData);
			editButton = new TextAreaMenuEdit(aData);
			styleButton = new TextAreaMenuStyle(aData);
			colorButton = new TextAreaMenuColor(aData);
			sizeButton = new TextAreaMenuSize(aData);
			optionsButton = new TextAreaMenuOptions(aData);
			endFiller = new Inspectorbar.Filler();
		}

		public void Draw()
		{
			totalRect = Inspectorbar.Utility.GetInspectorbarRect();
			totalRect.xMin = 0;
			totalRect.xMax = aData.widthTester.xMax;
			CalcRectParameters();
			var currentRect = new Rect(totalRect);
			totalRect.xMax = EditorGUIUtility.currentViewWidth;
			startBumper.Draw(ref currentRect);
			typeButton.UpdateTitle();
			typeButton.Draw(ref currentRect, scaleFactor);
			editButton.Draw(ref currentRect, scaleFactor);
			styleButton.Draw(ref currentRect, scaleFactor);
			colorButton.Draw(ref currentRect, scaleFactor);
			sizeButton.Draw(ref currentRect, scaleFactor);
			optionsButton.Draw(ref currentRect, scaleFactor);
			endFiller.Draw(currentRect, totalRect);
		}

		void CalcRectParameters()
		{
			// in principlye we would neet a more complicated logic with
			// forced Repaints, when last and current "measurement" do not
			// match, but we can keep it this easy here, as aData.widthTester
			// will request already needed Repaints for its own needs and
			// those will match our needs here as well
			// the only thing we make sure here, is that we do not
			// mess up our scale factors - so no updates if not in
			// Repaint run, which are the only ones, who give the right size 
			// values.
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			CalcNeededWidth();
			CalcExcessWidth();
			CalcScaleFactor();
		}

		void CalcScaleFactor()
		{
			scaleFactor = 1;
			if (excessWidth > 0) {
				float delta = typeButton.LimitWidthBy(excessWidth, 70f);
				if (excessWidth - delta > 0) {
					scaleFactor = (totalRect.xMax - startBumper.baseWidth) / (neededWidth - delta - startBumper.baseWidth);
				}
			}
		}

		void CalcExcessWidth()
		{
			excessWidth = neededWidth - totalRect.xMax;
		}

		private void CalcNeededWidth()
		{
			neededWidth = startBumper.baseWidth
			+ typeButton.baseWidth
			+ editButton.baseWidth
			+ styleButton.baseWidth
			+ colorButton.baseWidth
			+ sizeButton.baseWidth
			+ optionsButton.baseWidth;
		}

	}
}
