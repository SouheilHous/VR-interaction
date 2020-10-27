using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocBase.AnnotationTypeModule {

	[System.Serializable]
	public class AnnotationTypeStyleScene : AnnotationTypeStyleBase
	{
		public bool showInSceneView;
		public float sceneViewMinDistance;
		public float sceneViewMaxDistance;
		public bool raycastToGameobjectsInObjectsList;
		public Color raycastColor;

		public AnnotationTypeStyleScene(
			XDocAnnotationTypeBase parent
		)
			: base(
				"Scene",
				parent
			)
		{
		}

		override protected void InitStyleX()
		{
			styleX.style.padding = new RectOffset(3, 3, 1, 1);
			styleX.style.normal.background = AssetManager.settings.defaultBGSceneView;
			styleX.bgColor = new Color(1f, 1f, 0.6f, 1f);
			styleX.style.fixedWidth = 150;
			styleX.style.fixedHeight = 30;
		}

		override protected void InitStyleOptions()
		{
			showInSceneView = false;
			sceneViewMinDistance = 20;
			sceneViewMaxDistance = 100;
			raycastToGameobjectsInObjectsList = false;
			raycastColor = Color.white;
		}

	}
}

