using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.Extensions;
using xDocEditorBase.Focus;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	/// <summary>
	/// X document annotation type editor base.
	/// 
	/// This functionality needs to have the following attribute in
	/// principle, but it is designed to be an abstract class and
	/// as such cant have this attribute.
	///	[CustomEditor(typeof(XDocAnnotationTypeBase))]
	/// 
	/// The child class will get this attribute.
	/// </summary>
	public abstract class XDocAnnotationTypeEditorBase : Editor
	{
		#region Main

		// --------------------------------------------------------------------------
		// --- the target object and parent list manager
		// --------------------------------------------------------------------------
		XDocAnnotationTypeBase annotationType;
		XDocAnnotationTypesListEditorBase parent;

		// --------------------------------------------------------------------------
		// --- serialized properties in the serialized object of the target object
		// --------------------------------------------------------------------------
		public SerializedProperty annotationTypeName;

		/// <summary>
		/// The de-facto constructor.
		/// </summary>
		void OnEnable ()
		{  
			if ( target == null ) {
				// apparently nothing is selected, we can just jump out.
				// but before we rectify possibly non-sense references.
				annotationType = null;
				parent = null;
				annotationTypeName = null;
				return;
			}
			
			annotationType = target as XDocAnnotationTypeBase;

			annotationTypeName = serializedObject.FindProperty ("m_Name");
			
			InitArrayOfSettingEditors ();
			CreateFocusManager ();
		}

		/// <summary>
		/// The parent is needed to query a unique name and the parent - a annotationTypesList - is
		/// managing the list of annotationTypes and thus is able to ensure a new unique name.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public void SetParent (
			XDocAnnotationTypesListEditorBase parent
		)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Catches the lost focus event and initiates data valuation before the values are
		/// saved.
		/// </summary>
		public void OnLostFocus ()
		{
			FocusUtility.LoseFocus ();
			// some possible commit may be needed, when data editor is active

			// 1) AnnotationTypeName
			CommitChanges ();

			// 2) data field names and date fields default value
			// The commit checks will be delegated to the data field editor
			if ( currentMenuIndex == (int) XDocAnnotationTypeBase.SettingsIndex.Data ) {
				dataEditor.CommitChanges ();
			}			
		}

		/// <summary>
		/// Use this function so the object can do some final tidy ups before being destroyed.
		/// (-> other objects may be depended on this objects existance,  i.e. the values it
		/// blocks)
		/// </summary>
		public void DestroyImmediate ()
		{
			OnLostFocus ();
			DestroyImmediate (this);
		}

		#endregion


		#region Settings

		PropertyEditor[] settingsEditor;

		public AnnotationTypeDataEditor dataEditor {
			get { return settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Data] as AnnotationTypeDataEditor; }
			private set { 
				settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Data] = value;
			}
		}

		public AnnotationTypeStyleIconEditor iconEditor {
			get { return settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Icon] as AnnotationTypeStyleIconEditor; }
			private set {
				settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Icon] = value;
			}
		}

		public AnnotationTypeStyleTitleEditor titleEditor {
			get { return settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Title] as AnnotationTypeStyleTitleEditor; }
			private set {
				settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Title] = value;
			}
		}

		public AnnotationTypeStyleTextEditor textEditor {
			get { return settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Text] as AnnotationTypeStyleTextEditor; }
			private set {
				settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Text] = value;
			}
		}

		public AnnotationTypeStyleSceneEditor sceneEditor {
			get { return settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Scene] as AnnotationTypeStyleSceneEditor; }
			private set { 
				settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Scene] = value; 
			}
		}

		public AnnotationTypeStyleHierarchyTextEditor hierarchyTextEditor {
			get { return settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Hierarchy] as AnnotationTypeStyleHierarchyTextEditor; }
			private set { 
				settingsEditor[(int) XDocAnnotationTypeBase.SettingsIndex.Hierarchy] = value;
			}
		}

		void InitArrayOfSettingEditors ()
		{
			if ( annotationType == null ) {
				return;
			}

			// generally needed
			int numMenuItems = XDocAnnotationTypeBase.SettingsCount;

			// Create the Editors
			settingsEditor = new PropertyEditor[numMenuItems];
			dataEditor = new AnnotationTypeDataEditor (serializedObject.FindProperty ("data"), this);
			iconEditor = new AnnotationTypeStyleIconEditor (serializedObject.FindProperty ("icon"));
			titleEditor = new AnnotationTypeStyleTitleEditor (serializedObject.FindProperty ("title"));
			textEditor = new AnnotationTypeStyleTextEditor (serializedObject.FindProperty ("text"));
			sceneEditor = new AnnotationTypeStyleSceneEditor (serializedObject.FindProperty ("scene"));
			hierarchyTextEditor = new AnnotationTypeStyleHierarchyTextEditor (serializedObject.FindProperty ("hierarchyText"));

			// -> sync helper
			List<GUIStyleXEditor> GUIStyleXEditorList = new List<GUIStyleXEditor> ();
			GUIStyleXEditorList.Add (iconEditor.styleXEditor);
			GUIStyleXEditorList.Add (titleEditor.styleXEditor);
			GUIStyleXEditorList.Add (textEditor.styleXEditor);
			GUIStyleXEditorList.Add (sceneEditor.styleXEditor);
			GUIStyleXEditorList.Add (hierarchyTextEditor.styleXEditor);

			// -> sync helper part 2
			List<string> GUIStyleXNameList = new List<string> ();
			GUIStyleXNameList.Add (XDocAnnotationTypeBase.SettingsNames[(int) XDocAnnotationTypeBase.SettingsIndex.Icon]);
			GUIStyleXNameList.Add (XDocAnnotationTypeBase.SettingsNames[(int) XDocAnnotationTypeBase.SettingsIndex.Title]);
			GUIStyleXNameList.Add (XDocAnnotationTypeBase.SettingsNames[(int) XDocAnnotationTypeBase.SettingsIndex.Text]);
			GUIStyleXNameList.Add (XDocAnnotationTypeBase.SettingsNames[(int) XDocAnnotationTypeBase.SettingsIndex.Scene]);
			GUIStyleXNameList.Add (XDocAnnotationTypeBase.SettingsNames[(int) XDocAnnotationTypeBase.SettingsIndex.Hierarchy]);

			// -> sync helper finalize
			foreach ( var GUIStyleXEditor in GUIStyleXEditorList ) {
				GUIStyleXEditor.SetSyncList (GUIStyleXEditorList, GUIStyleXNameList);
			}
		}

		#endregion


		#region Draw

		static int currentMenuIndex = 0;

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector ();
		}

		public void Draw (
			Rect rect
		)
		{
			serializedObject.Update ();
			focusManager.ForceKeyPressPreCheck ();

			// --------------------------------------------------------------------------
			// --- calculate rects
			// --------------------------------------------------------------------------

			// masterRect is the original Rect passed for drawing, minus a margin.
			var masterRect = XoxGUI.RemoveMargins (
				                 rect,
				                 AssetManager.settings.styleEditorWindowXContent.style);

			// This array of 2 Rects has the header Rect and the contents Rect.
			// The header Rect shows the name of the selected AnnotationType and
			//   a menubar to select the AnnotationType subset of configuration options:
			//   Data, IconStyle, TitleStyle, TextStyle, SceneStyle and HierarchyStyle.
			// The content Rect has the GUI elements for the corresponding config subset.
			var headerContentRects = XoxGUI.SplitVertically (
				                         masterRect,
				                         AssetManager.settings.styleEditorWindowXContentSub.style,
				                         NameAndMenuGUI.GetHeight (annotationTypeName));

			// --------------------------------------------------------------------------
			// --- DRAW: top/header and selected settings edior
			// --------------------------------------------------------------------------
			using ( new XoxEditorGUI.ChangeCheck<bool> (serializedObject.ApplyModifiedProperties) ) {
				DrawTop (headerContentRects[0]);
			}
			using ( new XoxEditorGUI.ChangeCheck<bool> (serializedObject.ApplyModifiedProperties) ) {
				settingsEditor[currentMenuIndex].Draw (headerContentRects[1]);
			}

			focusManager.ForceUpdate ();
		}

		/// <summary>
		/// Draws the Name Entry and a Tabs menu bar.
		/// </summary>
		/// <param name="rect">Rect.</param>
		void DrawTop (
			Rect rect
		)
		{
			using ( new XoxEditorGUI.ChangeCheck<bool> (serializedObject.ApplyModifiedProperties) )
			using ( var cs = new NameAndMenuGUI (
				                 rect, 
				                 annotationTypeName, 
				                 XDocAnnotationTypeBase.SettingsNames,
				                 currentMenuIndex) ) {
				// Check if the user has changed to another tab. In this case it isnt needed
				// to keep the focus. Loosing the focus has the benefit that certain validation
				// mechanisms trigger for user input.
				if ( cs.menuIndex != currentMenuIndex ) {
					FocusUtility.LoseFocus ();
					currentMenuIndex = cs.menuIndex;
				}
			}
		}

		class NameAndMenuGUI : XoxEditorGUI.ContentScopeNoScroll
		{
			public NameAndMenuGUI (
				Rect aRect,
				SerializedProperty aNameProperty,
				string[] aMenuNames,
				int aMenuIndex
			)
				: base (
					aRect
				)
			{
				// Draw the name property
				currentRect.AddVSpace (3);
				currentRect.SetToPropertyHeight (aNameProperty);
				FocusId.SetNextId (annotationTypeNameControlId);
				EditorGUI.PropertyField (currentRect.rect, aNameProperty, new GUIContent ("Name"));

				// DRAW the tab menu
				currentRect.MoveDown ();
				currentRect.AddVSpace (3);
				currentRect.SetToButtonHeight ();
				menuIndex = GUI.Toolbar (currentRect.rect, aMenuIndex, aMenuNames);
			}

			public int menuIndex {
				get;
				private set;
			}

			public static float GetHeight (
				SerializedProperty aNameProperty
			)
			{
				return XoxGUIRect.GetHeightOfVSpace (3) +
				XoxGUIRect.GetHeightOfProperty (aNameProperty) +
				XoxGUIRect.GetHeightOfMoveDownSpace () +
				XoxGUIRect.GetHeightOfVSpace (3) +
				XoxGUIRect.GetHeightOfButton () +
				GetHeightOfVPadding ();
			}
		}

		#endregion


		#region FocusManager

		public const string annotationTypeNameControlId = "XoxATN";
		public FocusManager	focusManager;

		void CreateFocusManager ()
		{
			focusManager = new FocusManager ();
	
			// Datafield Name
			focusManager.AddOnLostFocusTriggerCallback (
				CorrectDataFieldName,
				annotationTypeNameControlId);
			focusManager.AddOnKeyPressReturnTriggerCallback (
				CorrectDataFieldName,
				annotationTypeNameControlId);
		}

		/// <summary>
		/// Corrects the name of the data field.
		/// 
		/// We dont need the argument parameter, but as callback it is needed in the 
		/// functions signature.
		/// </summary>
		/// <param name="controlId">Control identifier.</param>
		public void CorrectDataFieldName (
			string controlId
		)
		{
			parent.EnsureUniquenessOfAnnotationTypeNamesCurrent ();
		}

		public void CommitChanges ()
		{
			FocusUtility.LoseFocus ();
			CorrectDataFieldName (null);
		}

		#endregion

	}
}

