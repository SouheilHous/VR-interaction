using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;


namespace xDocEditorBase.AnnotationModule {

	public class TextAreaMenuSize : Inspectorbar.ButtonBase
	{
		public static readonly int[] sizes = {
			8,
			9,
			10,
			12,
			14,
			16,
			18,
			20,
			22,
			24,
			26,
			28,
			36,
			48,
			72
		};
		
		readonly XDocAnnotationEditorBase aData;

		public TextAreaMenuSize(
			XDocAnnotationEditorBase aData
		)
			: base(
				"<Size>",
				AssetManager.settings.styleToolbarDropDown.style
			)
		{
			this.aData = aData;
		}

		protected override void ButtonAction()
		{
			ShowSizesMenu(currentPosition);
		}

		void ShowSizesMenu(
			Rect position
		)
		{
			var sizesMenu = new GenericMenu();

			foreach ( var sizeItem in sizes )
				sizesMenu.AddItem(new GUIContent(sizeItem.ToString()), false, SetSize, sizeItem);
			sizesMenu.DropDown(position);
		}

		void SetSize(
			object sizeItem
		)
		{
			aData.annotationInspectorTextArea.AddTags("size", "=" + sizeItem);
		}
	}
}

