using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocBase.AnnotationTypeModule {

	[System.Serializable]
	public class AnnotationTypeStyleText : AnnotationTypeStyleBase
	{
		public bool showTextArea;
		public bool wideView;

		public AnnotationTypeStyleText(
			XDocAnnotationTypeBase parent
		)
			: base(
				"Text",
				parent
			)
		{
		}

		override protected void InitStyleX()
		{
			styleX.style.padding = new RectOffset(5, 5, 5, 5);
			styleX.style.margin = new RectOffset(2, 2, 0, 0);
			styleX.style.normal.background = AssetManager.settings.defaultBGText;
			styleX.bgColor = new Color(1f, 1f, 0.6f, 1f);
		}

		override protected void InitStyleOptions()
		{
			showTextArea = true;
			wideView = true;

		}

	}
}

