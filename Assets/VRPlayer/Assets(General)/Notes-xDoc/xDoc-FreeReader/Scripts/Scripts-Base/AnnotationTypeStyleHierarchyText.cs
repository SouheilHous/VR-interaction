using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocBase.AnnotationTypeModule {

	[System.Serializable]
	public class AnnotationTypeStyleHierarchyText : AnnotationTypeStyleBase
	{
		public bool showTextInHierarchyView;
		public float textWidthInHierarchyView;

		public AnnotationTypeStyleHierarchyText(
			XDocAnnotationTypeBase parent
		)
			: base(
				"Hier.Text",
				parent
			)
		{
		}

		override protected void InitStyleX()
		{
			styleX.bgColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			styleX.style.normal.background = AssetManager.settings.defaultBGHierarchyText;
			styleX.style.padding = new RectOffset(1, 1, 1, 1);
		}

		override protected void InitStyleOptions()
		{
			showTextInHierarchyView = false;
			textWidthInHierarchyView = 60;
		}

	}
}

