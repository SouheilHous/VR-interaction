using UnityEditor;
using UnityEngine;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class AnnotationTypeStyleIconEditor : AnnotationTypeStyleBaseEditor
	{
		readonly SerializedProperty icon;
		readonly SerializedProperty showIconInHierarchyView;
		readonly SerializedProperty highPriorityInHierarchyView;

		readonly GUIContent iconLabel;
		readonly GUIContent showIconInHierarchyViewLabel;
		readonly GUIContent highPriorityInHierarchyViewLabel;

		override protected ATStyleEditorType GetEditorType ()
		{
			return ATStyleEditorType.ICON;
		}

		public AnnotationTypeStyleIconEditor (
			SerializedProperty serializedProperty
		)
			: base (
				serializedProperty
			)
		{
			icon = serializedProperty.FindPropertyRelative ("icon");
			showIconInHierarchyView = serializedProperty.FindPropertyRelative ("showIconInHierarchyView");
			highPriorityInHierarchyView = serializedProperty.FindPropertyRelative ("highPriorityInHierarchyView");

			iconLabel = new GUIContent ("Icon", "Icons can be shown in the hierarchy view or in the title line.");
			showIconInHierarchyViewLabel = new GUIContent ("Show Icon", "Enable to show the icon in hierarchy view.");
			highPriorityInHierarchyViewLabel = new GUIContent (
				"High Priority",
				"If not high priority, icons will only be shown when no other icons are shown on the same object." +
				" Disable, if you want to avoid clutter in the hierarchy view. Enable for important annotation types.");
		}

		override protected void DrawOptions (
			XoxGUIRect currentRect
		)
		{
			currentRect.SetToLineHeight (1);

			EditorGUI.PropertyField (currentRect.rect, icon, iconLabel);
			currentRect.MoveDown ();
				
			EditorGUI.LabelField (currentRect.rect, "Hierarchy View Icon Options", EditorStyles.boldLabel);
			currentRect.MoveDown ();

			EditorGUI.PropertyField (currentRect.rect, showIconInHierarchyView, showIconInHierarchyViewLabel);
			currentRect.MoveDown ();

			using ( new EditorStateSaver.Indent (1) ) {
				GUI.enabled = showIconInHierarchyView.boolValue;

				EditorGUI.PropertyField (currentRect.rect, highPriorityInHierarchyView, highPriorityInHierarchyViewLabel);

				GUI.enabled = true;
			}
		}

		static public float GetHeight ()
		{
			return XoxGUIRect.GetHeightOfLines (4);
		}

	}
}