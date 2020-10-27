// _FREQ - implement better remove tags (at the moment it will remove any <xyz> string, regardless of if it is a valid rich text format

using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using xDocBase;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.CustomData;
using xDocEditorBase.Focus;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationModule {

	// This is the base class to AnnotationInspector

	// MultiObject editing causes a lot of problems esp. in the custom data and change-AnnotationType area
	// The problem with the annotationType seems to be related to the fact that serialized properties arent
	// used in the editor, instead the target is directly accessed to call the SetAnnotation-function.
	// Probably it would already help just to iterate over the targets-array. But no need at the moment
	// the standard regions are easy to implement and the code is already implemented for these
	//[CanEditMultipleObjects]
	//	[CustomEditor(typeof(Annotation))]
	public abstract class XDocAnnotationEditorBase : Editor
	{

#region Serialized Data

		public	SerializedProperty spAnnotationType;

		public	SerializedProperty spComment;
		public	SerializedProperty spCommentStripped;

		public	SerializedProperty spWordwarp;
		public	SerializedProperty spRichText;
		public	SerializedProperty spMaxHeight;
		public	SerializedProperty spMaxHeightCD;

		public	SerializedProperty spObjectReferenceList;
		public	SerializedProperty spDataFieldList;

		public	SerializedProperty spWordwarpSceneView;
		public	SerializedProperty spRichTextSceneView;
		public	SerializedProperty spMaxHeightSceneView;
		public	SerializedProperty spMaxWidthSceneView;

		public bool currentAnnotationTypeIsUnique {
			get { return !spAnnotationType.hasMultipleDifferentValues; }
		}

#endregion


#region Object References

		/// <summary>
		/// The annotation - the object inspected. 
		/// Use this reference primarily for accessing data - not writing.
		/// Consider using serialized properties instead. If this is not possible 
		/// you need to implement the undo mechanism manually
		/// </summary>
		public XDocAnnotationBase annotation;

		/// <summary>
		/// The annotation type of the annotation inspected. 
		/// Use this reference primarily for accessing data - not writing.
		/// Consider using serialized properties instead. If this is not possible 
		/// you need to implement the undo mechanism manually
		/// </summary>
		public XDocAnnotationTypeBase annotationType {
			get {
				return annotation.annotationTypeNotNull;
			}
		}

#endregion


#region Main

		//		protected override void OnEnable()
		//		{
		//			base.OnEnable();
		protected  void OnEnable()
		{

			spComment = serializedObject.FindProperty("comment");
			spCommentStripped = serializedObject.FindProperty("commentStripped");
			spWordwarp = serializedObject.FindProperty("wordwarp");
			spRichText = serializedObject.FindProperty("richText");

			spAnnotationType = serializedObject.FindProperty("annotationType");

			spMaxHeight = serializedObject.FindProperty("maxHeight");
			spMaxHeightCD = serializedObject.FindProperty("maxHeightCD");
			spObjectReferenceList = serializedObject.FindProperty("objectReferenceList");
			spDataFieldList = serializedObject.FindProperty("dataFieldList");

			spWordwarpSceneView = serializedObject.FindProperty("wordwarpSceneView");
			spRichTextSceneView = serializedObject.FindProperty("richTextSceneView");
			spMaxWidthSceneView = serializedObject.FindProperty("maxWidthSceneView");
			spMaxHeightSceneView = serializedObject.FindProperty("maxHeightSceneView");

			annotation = (XDocAnnotationBase)target;

			InitGUI();

			// _UNDONE --- playmode hack
			// EditorApplication.playmodeStateChanged += PlayModeChangeHandler;
			// Debug.Log("REGISTER: " + propComment.stringValue);
		}

		//		protected override void OnDisable()
		//		{
		protected void OnDisable()
		{
			// even though empty, we need this declarion here
			// otherwise unity throws errors,.. 

			// now it is no longer empty
			// _UNDONE --- playmode hack
			//EditorApplication.playmodeStateChanged -= PlayModeChangeHandler;
			//Debug.Log("UN----ER: " + propComment.stringValue);
		}

#endregion



#region GUI Control

		public AnnotationInspectorMultiTool annotationInspectorMultiTool;
		public AnnotationInspectorTextArea annotationInspectorTextArea;
		public AnnotationInspectorCustomData annotationInspectorCustomData;

		public FocusState annotationFocusState;
		public FocusManager focusManager;
		public AnnotationInspectorGUIStateManager guiStateManager;
		public XoxEditorGUILayout.WidthTester widthTester;

		const string controlNameCommentPrefix = "XG";
		public string controlGroupBaseId;

		public string controlIdTextArea {
			get { return FocusId.GetId(controlGroupBaseId, "TA"); }
		}

		public string controlIdSizer {
			get { return FocusId.GetId(controlGroupBaseId, "SZ"); }
		}

		public string controlIdSizerCustomData {
			get { return FocusId.GetId(controlGroupBaseId, "CZ"); }
		}

		public string controlIdSizerSceneViewWidth {
			get { return FocusId.GetId(controlGroupBaseId, "SW"); }
		}

		public string controlIdSizerSceneViewHeight {
			get { return FocusId.GetId(controlGroupBaseId, "SH"); }
		}

		public string controlIdColorChooser {
			get { return FocusId.GetId(controlGroupBaseId, "CC"); }
		}

		void InitGUI()
		{

			controlGroupBaseId = controlNameCommentPrefix + target.GetInstanceID();
			annotationFocusState = new FocusState(controlGroupBaseId);

			guiStateManager = new AnnotationInspectorGUIStateManager(this);
			focusManager = new FocusManager();
			widthTester = new XoxEditorGUILayout.WidthTester(this);
			annotationInspectorMultiTool = new AnnotationInspectorMultiTool(this);
			annotationInspectorTextArea = new AnnotationInspectorTextArea(this);
			annotationInspectorCustomData = new AnnotationInspectorCustomData(this);
		}

#endregion


#region Draw Inspector Area

		public override bool UseDefaultMargins()
		{
			return true;
		}

		public override void OnInspectorGUI()
		{

			if (!AssetManager.isFunctional) {
				// most probably asset resources couldnt be loaded due to missing / lost script assignments
				EditorGUILayout.HelpBox(AssetManager.errMessageCantLoadAssetResources, MessageType.Error);
				return;
			}

			StartDrawCycle(); 

			annotationInspectorMultiTool.Draw();
			annotationInspectorTextArea.Draw();
			annotationInspectorCustomData.Draw();

			FinalizeDrawCycle();
		}

		public void StartDrawCycle()
		{
			serializedObject.Update();

			focusManager.ForceKeyPressPreCheck();
		}

		public void FinalizeDrawCycle()
		{
			// focus manager update always at the end of the draw cycle
			annotationFocusState.Update();
			focusManager.Update();
			guiStateManager.Update();
			widthTester.Update(annotationType.text.wideView);

			if (AssetManager.canWrite) {
				serializedObject.ApplyModifiedProperties();
			}
		}

#endregion


#region Draw Single Line Comment Entry

		public void DrawSingleLineCommentEntry(
			Rect position
		)
		{
			serializedObject.Update();

			using (new EditorGUI.PropertyScope(position, GUIContent.none, spComment))
			using (new XoxEditorGUI.ChangeCheck(ApplySingleLineCommentEntry))
			using (var bg = new StateSaver.BgColor(annotationType.hierarchyText.styleX.bgColor)) {
				var tmpVar = EditorGUI.TextArea(
					             position,
					             spComment.stringValue,
					             annotationType.hierarchyText.styleX.style);
				if (AssetManager.canWrite) {
					spComment.stringValue = tmpVar;
				}
				// for the tooltip
				bg.Set(Color.clear);
				GUI.Box(position, new GUIContent("", annotation.commentStripped));
			}
		}

		void ApplySingleLineCommentEntry()
		{
			if (!AssetManager.canWrite) {
				return;
			}
			spCommentStripped.stringValue = Regex.Replace(spComment.stringValue, @"<[^>]*>", string.Empty);
			serializedObject.ApplyModifiedProperties();
		}

#endregion


	}
}