using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocEditorBase.AnnotationModule {

	public class TextAreaMenuStyle : Inspectorbar.ButtonBase
	{
		public static readonly string[] styles = {
			"bold",
			"italic"
		};

		readonly XDocAnnotationEditorBase aData;

		public TextAreaMenuStyle(
			XDocAnnotationEditorBase aData
		)
			: base(
				"<Style>",
				AssetManager.settings.styleToolbarDropDown.style
			)
		{
			this.aData = aData;
		}

		protected override void ButtonAction()
		{
			ShowStylesMenu(currentPosition);
		}

		void ShowStylesMenu(
			Rect position
		)
		{
			var styleMenu = new GenericMenu();

			foreach ( var item in styles ) {
				styleMenu.AddItem(new GUIContent(item), false, SetStyle, item);
			}
			styleMenu.DropDown(position);
		}

		void SetStyle(
			object styleItem
		)
		{
			string st = styleItem.ToString();
			if (st == styles[0])
				aData.annotationInspectorTextArea.AddTags("b");
			else if (st == styles[1])
				aData.annotationInspectorTextArea.AddTags("i");

		}

	}
}

