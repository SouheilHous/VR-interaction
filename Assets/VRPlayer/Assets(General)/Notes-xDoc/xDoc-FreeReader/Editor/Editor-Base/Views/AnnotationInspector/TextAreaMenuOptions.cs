using UnityEditor;
using UnityEngine;
using xDocBase;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.UI;


namespace xDocEditorBase.AnnotationModule {

	/// <summary>
	/// This is the button in the inspectorbar, which opens the options menu as a window
	/// (Class: TextAreaMenuOptionsWindow)
	/// </summary>
	public class TextAreaMenuOptions : Inspectorbar.ButtonBase
	{
		readonly XDocAnnotationEditorBase aData;
		TextAreaMenuOptionsWindow optionsWindow;

		public TextAreaMenuOptions(
			XDocAnnotationEditorBase aData
		)
			: base(
				"Options",
				AssetManager.settings.styleToolbarDropDown.style
			)
		{
			this.aData = aData;
		}

		protected override void ButtonAction()
		{
			optionsWindow = ScriptableObject.CreateInstance<TextAreaMenuOptionsWindow>();
			optionsWindow.SetDataRef(aData);
			var screenXY = GUIUtility.GUIToScreenPoint(new Vector2(currentPosition.x, currentPosition.y));
			var screenRect = new Rect(screenXY, new Vector2(currentPosition.width, currentPosition.height));
			optionsWindow.ShowAsDropDown(
				screenRect,
				new Vector2(
					TextAreaMenuOptionsWindow.width,
					TextAreaMenuOptionsWindow.height));
		}
	}

	public class TextAreaMenuOptionsWindow : EditorWindow
	{
		const int numSettingLines = 14;
		XDocAnnotationEditorBase aData;
		static Color bgColor;

		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(TextAreaMenuOptionsWindow), true);
		}

		public void SetDataRef(
			XDocAnnotationEditorBase aData
		)
		{
			this.aData = aData;
		}

		public static float width { get { return 225; } }


		public static float height { 
			get { 
				return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numSettingLines
				+ EditorGUIUtility.standardVerticalSpacing;
			}
		}

		void OnEnable()
		{
			minSize = new Vector2(0, 0);
			maxSize = new Vector2(1000, 1000);
			bgColor = new Color(.9f, .9f, .9f);
		}

		void OnGUI()
		{
			// just for a whiter bg of the windows
			using (new StateSaver.GuiColor(bgColor))
				GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), AssetManager.settings.whitePixel, ScaleMode.StretchToFill);

			aData.serializedObject.Update();

			EditorGUIUtility.labelWidth = 100;

			EditorGUILayout.LabelField("Inspector Options", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(aData.spWordwarp);
			EditorGUILayout.PropertyField(aData.spRichText);
			aData.spMaxHeight.floatValue = EditorGUILayout.Slider("Max.Height Text", aData.spMaxHeight.floatValue, 25, 1000);
			aData.spMaxHeightCD.floatValue = EditorGUILayout.Slider("Max.Height Data", aData.spMaxHeightCD.floatValue, 25, 1000);


			EditorGUILayout.GetControlRect();
			EditorGUILayout.LabelField("Scene View Options", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(aData.spWordwarpSceneView, new GUIContent("Wordwarp"));
			EditorGUILayout.PropertyField(aData.spRichTextSceneView, new GUIContent("Richtext"));
			aData.spMaxWidthSceneView.floatValue = EditorGUILayout.Slider(
				"Max.Width",
				aData.spMaxWidthSceneView.floatValue,
				0,
				1000);
			aData.spMaxHeightSceneView.floatValue = EditorGUILayout.Slider(
				"Max.Height",
				aData.spMaxHeightSceneView.floatValue,
				0,
				1000);
			aData.serializedObject.ApplyModifiedProperties();

			EditorGUILayout.GetControlRect();
			EditorGUILayout.LabelField("Game Object", EditorStyles.boldLabel);
			if (GUILayout.Button("Place After", EditorStyles.popup)) {
				GenericMenu menu = new GenericMenu();
				// Analysis disable once ObjectCreationAsStatement
				new PlaceAfterSubMenu(aData, menu, this);
				Rect rr = GUILayoutUtility.GetLastRect();
				rr.height = height;
				menu.DropDown(rr);
			}
		}
	}

	public class PlaceAfterSubMenu
	{
		//	const string subMenuName = "Place After/";
		const string subMenuName = "";
		readonly XDocAnnotationEditorBase aData;
		GenericMenu parentMenu;
		readonly EditorWindow parent;
		Component[] cArray;
		int ownIndex;

		public PlaceAfterSubMenu(
			XDocAnnotationEditorBase aData,
			GenericMenu parentMenu,
			EditorWindow parent
		)
		{
			this.parent = parent;
			this.parentMenu = parentMenu;
			this.aData = aData;

			BuildPlaceComponentAfterMenu();
		}

		void BuildPlaceComponentAfterMenu()
		{
			ExtractComponents();

			for (int i = 0; i < cArray.Length; i++) {
				Component c = cArray[i];
				if (c.GetType() == aData.annotation.GetType()) {
					XDocAnnotationBase a = (XDocAnnotationBase)c;
					XDocAnnotationTypeBase at = a.annotationTypeNotNull;
					AddPlaceAfterItem("Annotation: " + at.Name, i);
				} else {
					AddPlaceAfterItem(c.GetType().Name, i);
				}
			}
		}

		void ExtractComponents()
		{
			cArray = aData.annotation.transform.GetComponents<Component>();
			for (int i = 0; i < cArray.Length; i++) {
				if (cArray[i] == aData.annotation) {
					ownIndex = i;
				}
			}
		}

		void AddPlaceAfterItem(
			string text,
			int pos
		)
		{
			if (pos == ownIndex - 1) {
				parentMenu.AddDisabledItem(new GUIContent(subMenuName + pos + ": " + text));
				return;
			}
			if (pos == ownIndex) {
				parentMenu.AddDisabledItem(new GUIContent(subMenuName + "> " + text));
				return;
			}
			parentMenu.AddItem(new GUIContent(subMenuName + pos + ": " + text), false, MoveIt, pos - ownIndex);
		}

		void MoveIt(
			object oSteps
		)
		{
			parent.Close();
			int steps = (int)oSteps;
			if (steps < 0) {
				steps = -steps - 1;
				for (int i = 0; i < steps; i++) {
					UnityEditorInternal.ComponentUtility.MoveComponentUp(aData.annotation);
				}
			} else {
				for (int i = 0; i < steps; i++) {
					UnityEditorInternal.ComponentUtility.MoveComponentDown(aData.annotation);
				}
			}
		}
	}


}

