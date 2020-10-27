using UnityEditor;
using UnityEngine;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class AnnotationTypeStyleSceneEditor : AnnotationTypeStyleBaseEditor
	{
		readonly SerializedProperty sceneViewMinDistance;
		readonly SerializedProperty sceneViewMaxDistance;
		readonly SerializedProperty showInSceneView;
		readonly SerializedProperty raycastToGameobjectsInObjectsList;
		readonly SerializedProperty raycastColor;

		override protected ATStyleEditorType GetEditorType ()
		{
			return ATStyleEditorType.SCENE;
		}

		public AnnotationTypeStyleSceneEditor (
			SerializedProperty serializedProperty
		)
			: base (
				serializedProperty
			)
		{
			sceneViewMinDistance = serializedProperty.FindPropertyRelative ("sceneViewMinDistance");
			sceneViewMaxDistance = serializedProperty.FindPropertyRelative ("sceneViewMaxDistance");
			showInSceneView = serializedProperty.FindPropertyRelative ("showInSceneView");
			raycastToGameobjectsInObjectsList = serializedProperty.FindPropertyRelative ("raycastToGameobjectsInObjectsList");
			raycastColor = serializedProperty.FindPropertyRelative ("raycastColor");
		}

		override protected void DrawOptions (
			XoxGUIRect currentRect
		)
		{
			using ( var indent = new EditorStateSaver.Indent () ) {
				
				currentRect.SetToLineHeight (1);

				EditorGUI.LabelField (currentRect.rect, "Scene View Options", EditorStyles.boldLabel);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, 
					sceneViewMinDistance,
					new GUIContent ("Min. View Distance")
				);			
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, 
					sceneViewMaxDistance,
					new GUIContent ("Max. View Distance")
				);
				currentRect.MoveDown ();

				EditorGUI.LabelField (currentRect.rect, "Text");
				currentRect.MoveDown ();

				indent.Incr ();
				EditorGUI.PropertyField (currentRect.rect, showInSceneView);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, styleXEditor.spFixedWidth);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, styleXEditor.spFixedHeight);
				currentRect.MoveDown ();

				indent.Decr ();
				EditorGUI.LabelField (currentRect.rect, "Object Reference List");
				currentRect.MoveDown ();

				indent.Incr ();
				EditorGUI.PropertyField (currentRect.rect, raycastToGameobjectsInObjectsList,
					new GUIContent ("Raycast to GO's")
				);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, raycastColor);
			}

		}

		static public float GetHeight ()
		{
			return XoxGUIRect.GetHeightOfLines (10);
		}

	}
}