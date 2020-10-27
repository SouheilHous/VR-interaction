using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocEditorBase.AnnotationModule {

	public class TextAreaMenuEdit : Inspectorbar.ButtonBase
	{
		public static readonly string[] editCommands = {
			"Select All",
			"SEP",
			"Cut",
			"Copy",
			"Paste",
			"SEP",
			"Remove Tags"
		};

		readonly XDocAnnotationEditorBase aData;

		public TextAreaMenuEdit(
			XDocAnnotationEditorBase aData
		)
			: base(
				"Edit",
				AssetManager.settings.styleToolbarDropDown.style
			)
		{
			this.aData = aData;
		}

		protected override void ButtonAction()
		{
			ShowEditMenu(currentPosition);
		}

		void ShowEditMenu(
			Rect position
		)
		{
			var editMenu = new GenericMenu();
	
			foreach ( var ed in editCommands )
				if (ed == "SEP")
					editMenu.AddSeparator("");
				else
					editMenu.AddItem(new GUIContent(ed), false, CallEditCommand, ed);
			editMenu.DropDown(position);
		}

		void CallEditCommand(
			object cmd
		)
		{
			string c = cmd.ToString();
			if (c == editCommands[0])
				aData.annotationInspectorTextArea.EditSelectAll();
			else if (c == editCommands[2])
				aData.annotationInspectorTextArea.EditCut();
			else if (c == editCommands[3])
				aData.annotationInspectorTextArea.EditCopy();
			else if (c == editCommands[4])
				aData.annotationInspectorTextArea.EditPaste();
			else if (c == editCommands[6])
				aData.annotationInspectorTextArea.EditRemoveTags();
		}
	
	
	}
}

