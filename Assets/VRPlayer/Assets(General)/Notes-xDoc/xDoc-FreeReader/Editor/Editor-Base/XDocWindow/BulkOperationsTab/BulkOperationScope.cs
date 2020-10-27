using UnityEditor;
using UnityEngine;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class BulkOperationScope
	{
		Scope previousScope;
		public Scope scope;

		static float height = 18f;

		public BulkOperationScope ()
		{

		}

		public BulkOperationScope (
			Scope scope
		)
		{
			this.scope = scope;
		}

		public enum Scope
		{
			AllScenes,
			CurrentlyLoadedScenes,
			Selection
		}

		static readonly string[] scopeNames = {
			"All Scenes",
			"Currently Loaded Scenes",
			"Selection"
		};

		public bool scopeHasChanged {
			get { 
				return previousScope != scope; 
			}
		}

		public void DrawScopeSelection ()
		{
		
//		if (Event.current.type == EventType.Layout) {
			previousScope = scope;
//		}
			scope = (Scope) GUILayout.Toolbar ((int) scope, scopeNames, GUI.skin.button);
			if ( Event.current.type == EventType.Repaint ) {
				height = GUILayoutUtility.GetLastRect ().height;
			}
		}

		public void DrawScopeSelection (
			Rect rect
		)
		{
			previousScope = scope;
			scope = (Scope) GUI.Toolbar (rect, (int) scope, scopeNames);
			height = rect.height;
		}

		public static float scopeSelectionHeight {
//		get { return EditorGUIUtility.singleLineHeight; }
			get { return height; }
		}

		public bool confirmBulkOperation {
			get {
				if ( isFullScope ) {
					return EditorUtility.DisplayDialog ("WARNING! Bulk Operation in All Scenes", 
						"The scope of operation is 'All Scenes'. " +
						"Therefore the currently open scenes will be save-closed and " +
						"each existing scene in the project folder will be loaded one after the other " +
						"and the requested operations will be executed on each of them. " +
						"Make sure your to have your current state of work under version control! \n\n" +
						"You wont be able to undo! \n" +
						"Do you want to continue?", "Yes", "No");
				} 
				return true;
			}
		}

		public bool isFullScope {
			get { return scope == Scope.AllScenes; }
		}

		public bool isSelection {
			get { return scope == Scope.Selection; }
		}

		public string selectionInformation {
			get { 
				return	"Total selected object: " + Selection.objects.Length + "\n"
				+ "Total gameObjects: " + Selection.gameObjects.Length;
			}
		}
	}
}

