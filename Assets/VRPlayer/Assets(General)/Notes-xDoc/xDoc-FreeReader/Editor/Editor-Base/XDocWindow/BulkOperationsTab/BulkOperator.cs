using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using xDocBase;
using xDocBase.AnnotationTypeModule;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule {

	public class BulkOperator : BulkOperationScope
	{
		public delegate bool SingleOperation(XDocAnnotationBase annotation);

		SingleOperation singleOperation;
		XDocAnnotationTypeBase annotationType;
		bool checkType;

		public void Run(
			SingleOperation singleOperation
		)
		{
			checkType = false;
			annotationType = null;
			this.singleOperation = singleOperation;
			RunInternal();
		}

		public void Run(
			SingleOperation singleOperation,
			XDocAnnotationTypeBase annotationType
		)
		{
			checkType = true;
			this.annotationType = annotationType;
			this.singleOperation = singleOperation;
			RunInternal();
		}

		void RunInternal(
	)
		{
			switch (scope) {
			case BulkOperationScope.Scope.AllScenes:
				RunInAllScenes();
				break;
			case BulkOperationScope.Scope.CurrentlyLoadedScenes:
				RunInCurrentlyLoadedScenes();
				break;
			case BulkOperationScope.Scope.Selection:
				RunInSelection();
				break;
			}	
		}

		bool RunSingle(
			XDocAnnotationBase annotation
		)
		{
			if (checkType) {
				if (annotation.annotationType == annotationType) {
					return singleOperation(annotation);
				} else {
					return false;
				}
			} else {
				return singleOperation(annotation);
			}
		}

		void RunInAllScenes()
		{
			// Save all open scenes
			EditorSceneManager.SaveOpenScenes();
			// remember the current scene to load it back
			string currentScenePath = SceneManager.GetActiveScene().path;

			// get all scenes in the project / assets folder
			var sceneGUIDs = AssetDatabase.FindAssets("t:scene");

			// loop over all scenes
			for (int sceneLooper = 0, sceneGUIDsLength = sceneGUIDs.Length; sceneLooper < sceneGUIDsLength; sceneLooper++) {
				var sceneGuid = sceneGUIDs[sceneLooper];
				var preString = "Scene " + (sceneLooper + 1) + "/" + sceneGUIDsLength + ": ";
				var sp = AssetDatabase.GUIDToAssetPath(sceneGuid);
				var loadedScene = EditorSceneManager.OpenScene(sp);

				var arrayOfAllAnnotationsInScene = Resources.FindObjectsOfTypeAll<XDocAnnotationBase>();
				int totalOfAllAnnotationsInScene = arrayOfAllAnnotationsInScene.Length;
				using (var pb = new XoxEditorGUI.ProgressBarIntCancelScan(totalOfAllAnnotationsInScene, "annotations")) {
					for (int i = 0; i < totalOfAllAnnotationsInScene; i++) {
						if (RunSingle(arrayOfAllAnnotationsInScene[i])) {
							return;
						}
						if (pb.SetCurrent(i, preString)) {
							return;
						}
					}
				}
				EditorSceneManager.SaveScene(loadedScene);
			}

			// load back first scene
			EditorSceneManager.OpenScene(currentScenePath);
		}

		void RunInCurrentlyLoadedScenes()
		{
			var arrayOfAllAnnotationsInLoadedScenes = Resources.FindObjectsOfTypeAll<XDocAnnotationBase>();
			int totalOfAllAnnotationsInLoadedScenes = arrayOfAllAnnotationsInLoadedScenes.Length;

			using (var pb = new XoxEditorGUI.ProgressBarIntCancelScan(totalOfAllAnnotationsInLoadedScenes, "annotations")) {
				for (int i = 0; i < totalOfAllAnnotationsInLoadedScenes; i++) {
					if (RunSingle(arrayOfAllAnnotationsInLoadedScenes[i])) {
						return;
					}
					if (pb.SetCurrent(i)) {
						return;
					}
				}
			}
		}

		void RunInSelection()
		{
			var arrayOfGameObjectsInSelection = Selection.gameObjects;
			var totalOfGameObjectsInSelection = arrayOfGameObjectsInSelection.Length;

			using (var pb = new XoxEditorGUI.ProgressBarIntCancelScan(totalOfGameObjectsInSelection, "selection gameObjects")) {
				for (int i = 0; i < totalOfGameObjectsInSelection; i++) {
					foreach ( var annotation in arrayOfGameObjectsInSelection[i].GetComponents<XDocAnnotationBase>() ) {
						if (RunSingle(annotation)) {
							return;
						}
					}
					if (pb.SetCurrent(i)) {
						return;
					}
				}
			}
		}

	}
}

