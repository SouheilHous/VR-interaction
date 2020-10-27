using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;


namespace xDocEditorBase.AnnotationModule {

	public class TextAreaMenuAnnotationType : Inspectorbar.ButtonBase
	{
		readonly XDocAnnotationEditorBase aData;

		public TextAreaMenuAnnotationType(
			XDocAnnotationEditorBase aData
		)
			: base(
				"Annotation",
				AssetManager.settings.styleToolbarPopup.style
			)
		{
			this.aData = aData;
		}

		public void UpdateTitle()
		{
			if (aData.currentAnnotationTypeIsUnique)
				SetNewTitle(aData.annotationType.name);
			else
				SetNewTitle("<mixed values>");
		}

		protected override void ButtonAction()
		{
			ShowTypesMenu(currentPosition);
		}

		void ShowTypesMenu(
			Rect position
		)
		{
			var typesMenu = new GenericMenu();

//		for (int i = 0; i < AssetManager.prefs.annotationTypes.Count; i++) {
//			AnnotationType2 runner = AssetManager.prefs.annotationTypes[i];
//			typesMenu.AddItem(new GUIContent(runner.annotationTypeName), 
//				aData.currentAnnotationTypeId == runner.id &&
//				aData.currentAnnotationTypeIdIsUnique,
//				SetType, runner.id);
//		}
			List<XDocAnnotationTypeBase> atList = AssetManager.annotationTypesAsset.annotationTypesList;
			for (int i = 0; i < atList.Count; i++) {
				XDocAnnotationTypeBase runner = atList[i];
				typesMenu.AddItem(new GUIContent(runner.name), 
					aData.annotationType == runner &&
					aData.currentAnnotationTypeIsUnique,
					SetType, i);
			}
			typesMenu.DropDown(position);
			GUIUtility.ExitGUI();
		}

		void SetType(
			object id
		)
		{
			List<XDocAnnotationTypeBase> atList = AssetManager.annotationTypesAsset.annotationTypesList;

			aData.annotation.SetAnnotationType(atList[(int)id]);
			aData.serializedObject.Update();
			EditorApplication.RepaintHierarchyWindow();
			SceneView.RepaintAll();
		}

	}
}
