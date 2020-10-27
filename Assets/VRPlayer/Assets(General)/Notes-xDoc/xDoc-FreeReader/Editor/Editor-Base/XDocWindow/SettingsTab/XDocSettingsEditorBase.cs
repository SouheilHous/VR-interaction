using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocEditorBase.Extensions;
using xDocEditorBase.UI;


namespace xDocEditorBase.AssetManagement
{

	// the next statement is useless, as this is a abstract class,... thus outdoced
	// [CustomEditor(typeof(XDocSettingsBase))]
	public abstract class XDocSettingsEditorBase : EditorX
	{
		public SerializedProperty backgroundColorMain;
		public SerializedProperty backgroundColorTitle;
		public SerializedProperty excerptLength;
		public SerializedProperty searchResultsPerPage;
		public SerializedProperty capTotalSearchResults;

		public SerializedProperty backgroundColorFilter;
		public SerializedProperty backgroundColorAnnotation;
		public SerializedProperty backgroundColorPrefab;
		public SerializedProperty backgroundColorEmpty;
		public SerializedProperty selectionItemsPerPage;
		public SerializedProperty capTotalSelectionList;

		Vector2 scrollPosition;

		void OnEnable ()
		{
			backgroundColorMain = serializedObject.FindProperty ("backgroundColorMain");
			backgroundColorTitle = serializedObject.FindProperty ("backgroundColorTitle");
			excerptLength = serializedObject.FindProperty ("excerptLength");
			searchResultsPerPage = serializedObject.FindProperty ("searchResultsPerPage");
			capTotalSearchResults = serializedObject.FindProperty ("capTotalSearchResults");

			backgroundColorFilter = serializedObject.FindProperty ("backgroundColorFilter");
			backgroundColorAnnotation = serializedObject.FindProperty ("backgroundColorAnnotation");
			backgroundColorPrefab = serializedObject.FindProperty ("backgroundColorPrefab");
			backgroundColorEmpty = serializedObject.FindProperty ("backgroundColorEmpty");
			selectionItemsPerPage = serializedObject.FindProperty ("selectionItemsPerPage");
			capTotalSelectionList = serializedObject.FindProperty ("capTotalSelectionList");
		}

		//		override public void Draw(
		//			Rect rect
		//		)
		//		{
		//			serializedObject.Update();
		//
		//			using (new GUILayout.AreaScope(rect)) {
		//				DrawContent();
		//			}
		//		}

		public float GetHeight ()
		{
			return XoxGUIRect.GetHeightOfLines (14);
		}

		public void Draw (
			XoxGUIRect currentRect
		)
		{
			serializedObject.Update ();


			using ( new XoxEditorGUI.ChangeCheck (ApplyModifiedProperties) ) {

				// search tab
				currentRect.SetToPropertyHeight (backgroundColorMain);
				EditorGUI.PropertyField (currentRect.rect, backgroundColorMain, new GUIContent ("koko"));
				currentRect.MoveDown ();

				currentRect.SetToLineHeight (1);

				EditorGUI.PropertyField (currentRect.rect, backgroundColorTitle);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, excerptLength);
				excerptLength.intValue = Mathf.Max (30, excerptLength.intValue);
				currentRect.MoveDown ();
			
				EditorGUI.PropertyField (currentRect.rect, searchResultsPerPage);
				searchResultsPerPage.intValue = Mathf.Clamp (searchResultsPerPage.intValue, 2, 50);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, capTotalSearchResults);
				capTotalSearchResults.intValue = Mathf.Max (10, capTotalSearchResults.intValue);
				currentRect.MoveDown ();


				// Bulk operations
				currentRect.SetToPropertyHeight (backgroundColorMain);
				EditorGUI.PropertyField (currentRect.rect, backgroundColorFilter);
				currentRect.MoveDown ();

				currentRect.SetToLineHeight (1);

				EditorGUI.PropertyField (currentRect.rect, backgroundColorAnnotation);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, backgroundColorPrefab);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, backgroundColorEmpty);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, selectionItemsPerPage);
				selectionItemsPerPage.intValue = Mathf.Clamp (selectionItemsPerPage.intValue, 2, 200);
				currentRect.MoveDown ();

				EditorGUI.PropertyField (currentRect.rect, capTotalSelectionList);
				capTotalSelectionList.intValue = Mathf.Max (10, capTotalSelectionList.intValue);
				currentRect.MoveDown ();
			}	
		}

		//		void DrawContent ()
		//		{
		//			using ( var sv = new EditorGUILayout.ScrollViewScope (scrollPosition) )
		//			using ( new XoxEditorGUI.ChangeCheck (ApplyModifiedProperties) ) {
		//				scrollPosition = sv.scrollPosition;
		//
		//				EditorGUILayout.PropertyField (backgroundColorMain);
		//				EditorGUILayout.PropertyField (backgroundColorTitle);
		//				EditorGUILayout.PropertyField (excerptLength);
		//				excerptLength.intValue = Mathf.Max (30, excerptLength.intValue);
		//				EditorGUILayout.PropertyField (searchResultsPerPage);
		//				searchResultsPerPage.intValue = Mathf.Clamp (searchResultsPerPage.intValue, 2, 50);
		//				EditorGUILayout.PropertyField (capTotalSearchResults);
		//				capTotalSearchResults.intValue = Mathf.Max (10, capTotalSearchResults.intValue);
		//
		//				EditorGUILayout.PropertyField (backgroundColorFilter);
		//				EditorGUILayout.PropertyField (backgroundColorAnnotation);
		//				EditorGUILayout.PropertyField (backgroundColorPrefab);
		//				EditorGUILayout.PropertyField (backgroundColorEmpty);
		//				EditorGUILayout.PropertyField (selectionItemsPerPage);
		//				selectionItemsPerPage.intValue = Mathf.Clamp (selectionItemsPerPage.intValue, 2, 200);
		//				EditorGUILayout.PropertyField (capTotalSelectionList);
		//				capTotalSelectionList.intValue = Mathf.Max (10, capTotalSelectionList.intValue);
		//			}
		//		}
	}
}
