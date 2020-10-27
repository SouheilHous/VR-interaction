using UnityEditor;
using UnityEngine;
using xDocBase.AnnotationTypeModule;
using xDocBase.UI;


namespace xDocEditorBase.AnnotationModule {


	public class AnnotationInspectorTitleLabel
	{
		readonly XDocAnnotationEditorBase aData;

		public AnnotationInspectorTitleLabel(
			XDocAnnotationEditorBase aData
		)
		{
			this.aData = aData;
		}

		public void Draw()
		{
			if (!aData.annotationType.title.showTitle)
				return;

			Rect titleRect;
			Rect imageRect;
			GUIStyle titleStyle;
			string	titleTypeName;

			using (new StateSaver.BgColor()) {
				if (aData.spAnnotationType.hasMultipleDifferentValues) {
					titleTypeName = "<mixed values>";
					titleStyle = GUI.skin.label;
				} else {
					titleTypeName = aData.annotationType.name;
					titleStyle = aData.annotationType.title.styleX.style;
					GUI.backgroundColor = aData.annotationType.title.styleX.bgColor;
				}
				if (titleTypeName.Length == 0)
					titleTypeName = "<Unnamed>";
				if (aData.annotationType.title.titleFormat == AnnotationTypeStyleTitle.TitleFormatType.UseTitleFix) {
					char c = aData.annotationType.title.titleFix;
					titleTypeName =
					new string(c, 3)
					+ " " + titleTypeName + " "
					+ new string(c, (int)EditorGUIUtility.currentViewWidth);
				}

				float h = titleStyle.CalcHeight(new GUIContent(titleTypeName), EditorGUIUtility.currentViewWidth);

				titleRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(h));
				titleRect.xMin = aData.widthTester.xMin;

				if (!aData.spAnnotationType.hasMultipleDifferentValues)
				if (aData.annotationType.icon.icon != null) {
					imageRect = new Rect(titleRect);
					imageRect.width = h;
					titleRect.xMin += h;
					using (new StateSaver.BgColor(aData.annotationType.icon.styleX.bgColor)) {
						EditorGUI.LabelField(
							imageRect,
							new GUIContent(aData.annotationType.icon.icon),
							aData.annotationType.icon.styleX.style);
					}
				}
				EditorGUI.LabelField(titleRect, titleTypeName, titleStyle);
			}
		}

	}
}

