using UnityEditor;
using UnityEngine;


namespace xDocBase.AnnotationTypeModule {

	[System.Serializable]
	public class AnnotationTypeStyleTitle : AnnotationTypeStyleBase
	{
		public bool showTitle;
		public TitleFormatType titleFormat;
		public char titleFix;

		public AnnotationTypeStyleTitle(
			XDocAnnotationTypeBase parent
		)
			: base(
				"Title",
				parent
			)
		{
		}

		override protected void InitStyleX()
		{
			styleX.style.normal.textColor = EditorStyles.label.normal.textColor;
			styleX.style.fontStyle = FontStyle.BoldAndItalic;
			styleX.style.padding = new RectOffset(5, 5, 2, 2);
		}

		override protected void InitStyleOptions()
		{
			showTitle = true;
			titleFormat = TitleFormatType.Clean;
			titleFix = '-';
		}

		[System.Serializable]
		public enum TitleFormatType
		{
			Clean,
			UseTitleFix
		}

	}
}
