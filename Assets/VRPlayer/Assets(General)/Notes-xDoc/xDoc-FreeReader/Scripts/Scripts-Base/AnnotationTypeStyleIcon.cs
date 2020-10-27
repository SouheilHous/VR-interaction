using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocBase.AnnotationTypeModule {

	[System.Serializable]
	public class AnnotationTypeStyleIcon : AnnotationTypeStyleBase
	{
		public Texture icon;
		public bool showIconInHierarchyView;
		public bool highPriorityInHierarchyView;

		public AnnotationTypeStyleIcon(
			XDocAnnotationTypeBase parent
		)
			: base(
				"Icon",
				parent
			)
		{
		}

		override protected void InitStyleX()
		{
			styleX.bgColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			styleX.style.normal.background = AssetManager.settings.defaultBGIcon;
			styleX.style.padding = new RectOffset(0, 0, 0, 0);
			styleX.style.border = new RectOffset(0, 0, 0, 0);
		}

		override protected void InitStyleOptions()
		{
			icon = null;
			showIconInHierarchyView = false;
			highPriorityInHierarchyView = false;		
		}
	}
}
