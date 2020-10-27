//  <copyright file="XDocWindowAnnotationTypesTab.cs" company="xox interactive">
//
//  Copyright (c) 2017 xox interactive. All rights reserved.
//  www: http://xoxinteractive.com
//  email: contact@xoxinteractive.com
//
//  The License terms are defined by the 
//  Unity ASSET STORE TERMS OF SERVICE AND EULA:
//  https://unity3d.com/legal/as_terms
//
//  </copyright>

namespace xDocEditorBase.Windows
{
	using UnityEditor;
	using UnityEngine;
	using xDocBase.AssetManagement;
	using xDocEditorBase.AnnotationTypeModule;
	using xDocEditorBase.UI;

	/// <summary>
	/// This is the class, which instantiates the 
	/// AnnotationType tab in the xDoc window.
	/// 
	/// This is the GUI to the annotationTypesList data.
	/// 
	/// It also creates an editor (GUI representation) for the
	/// annotationTypesList managed by the AssetManager - for
	/// private use.
	/// 
	/// The basic layout is a 2 column layout, where the
	/// available space can be adjusted with a slider in
	/// horizontal direction between the two panels.
	/// 
	/// The left panel shows a list of all AnnotationTypes. 
	/// AnnotationTypes can be selected, added and deleted here.
	/// Selected AnnotationTypes are shown in the right panel
	/// (details of the selected annotationType) and can be edited there.
	/// 
	/// AnnotationType details consists of 6 groups of details: data and 5x style.
	/// Each one is shown in a separate sub-"tab".
	/// </summary>
	public class XDocWindowAnnotationTypesTab : XoxEditorGUI.TwoPanelLayout
	{
		// Every tab needs to provide a name
		public override string name { get { return "Annotation Types"; } }

		// internal property variable
		XDocAnnotationTypesListEditorBase _annotationTypesListEditor;

		// HACK: public
		public XDocAnnotationTypesListEditorBase annotationTypesListEditor {
			get {
				if ( _annotationTypesListEditor == null ) {
					OnEnable ();
				}
				return _annotationTypesListEditor;
			}
		}

		/// <summary>
		/// Constructor: 
		/// Initializes a new instance of the 
		/// <see cref="xDocEditorBase.Windows.XDocWindowAnnotationTypesTab"/> class.
		/// 
		/// There not much to do for the constructor: it just passes the argument to
		/// its parent class.
		/// 
		/// The functionality wise construction is done in OnEnable.
		/// </summary>
		/// <param name="parent">Parent window, in which this tab will be drawn.</param>
		public XDocWindowAnnotationTypesTab (
			XDocWindow parent
		)
			: base (
				parent
			)
		{
		}

		/// <summary>
		/// Is called by the OnEnable event.
		/// 
		/// It creates an Editor (GUI representation) for the AnnotationTypesList
		/// managed by the AssetManager and stores a reference to it locally.
		/// </summary>
		public override void OnEnable ()
		{
			_annotationTypesListEditor = 
			Editor.CreateEditor (AssetManager.annotationTypesAsset) as XDocAnnotationTypesListEditorBase;
		}

		/// <summary>
		/// Implementation of the Left Panel Draw.
		/// 
		/// Draws the list of all AnnotationTypes.
		/// </summary>
		/// <param name="rect">Rect.</param>
		protected override void DrawLeftPanel (
			Rect rect
		)
		{
			try {
				annotationTypesListEditor.DrawList (rect);
			} catch ( System.Exception ex ) {
				EditorGUI.HelpBox (rect, "xDoc Error: Can't draw Annotations Types List; left panel.\n" +
					ex.Message + "\n" + ex.StackTrace, MessageType.Error);
			}
		}

		/// <summary>
		/// Implementation of the Right Panel Draw.
		/// 
		/// Draws the details of the currently selected AnnotationType.
		/// </summary>
		/// <param name="rect">Rect.</param>
		protected override void DrawRightPanel (
			Rect rect
		)
		{
			try {
				annotationTypesListEditor.DrawSelected (rect);
			} catch ( System.Exception ex ) {
				EditorGUI.HelpBox (rect, "xDoc Error: Can't draw Annotations Types List; right panel.\n" +
					ex.Message + "\n" + ex.StackTrace, MessageType.Error);
			}
		}

		/// <summary>
		/// This function is called by the parent window in case of an OnFocusLost 
		/// event. The event is passed on to the annotationTypesListEditor, which 
		/// needs it in case of focus lost during mid-edit -> should have the same 
		/// effect as a save or Return key.
		/// </summary>
		public override void OnLostFocus ()
		{
			annotationTypesListEditor.OnLostFocus ();
		}

	}
}

