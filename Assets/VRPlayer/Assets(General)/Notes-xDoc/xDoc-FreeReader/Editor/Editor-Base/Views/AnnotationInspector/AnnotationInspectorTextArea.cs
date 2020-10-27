using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using xDocBase.AssetManagement;
using xDocBase.UI;
using xDocEditorBase.Focus;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationModule
{

	public class AnnotationInspectorTextArea
	{
		readonly XDocAnnotationEditorBase aData;
		public FocusGetter focusGetter;

		// helpers
		Vector2	scrollPosition;
		TextEditor editor;
		int lastCursorIndex = 0;
		bool requestTextAreaScrollToCursorPosAfterKeyPress;

		// more helpers
		GUIStyle style;
		Color bgColor;
		/// <summary>
		/// View Area of the scroll region
		/// </summary>
		Rect vRect;
		/// <summary>
		/// Content area of the scroll region
		/// </summary>
		Rect cRect;
		/// <summary>
		/// Rect for the text area
		/// </summary>
		Rect tRect;


		#region Initialization

		public AnnotationInspectorTextArea (
			XDocAnnotationEditorBase aData
		)
		{
			this.aData = aData;
			aData.annotationInspectorTextArea = this;
			focusGetter = new FocusGetter (aData, aData.controlIdTextArea);
		}

		#endregion


		#region Draw

		public void Draw ()
		{
			if (!aData.annotationType.text.showTextArea) {
				return;
			}
			style = aData.annotationType.text.styleX.style;
			bgColor = aData.annotationType.text.styleX.bgColor;

			style.wordWrap = aData.annotation.wordwarp;
			style.richText = aData.annotation.richText && aData.guiStateManager.isNotEdit;

			CalcAndAllocateRects ();
			DrawScrollArea ();

			focusGetter.Update ();
		}

		void CalcAndAllocateRects ()
		{
			Vector2 viewSize;
			Vector2 textSize;
			Vector2 contentSize;
			GUIContent content = new GUIContent (aData.annotation.comment);

			// calc availWidth
			viewSize.x = aData.widthTester.width;
			contentSize.x = viewSize.x;
			textSize.x = contentSize.x - style.margin.left - style.margin.right;

			// calculate textarea size
			if (aData.annotation.wordwarp) {
				textSize.y = style.CalcHeight (content, textSize.x);
				contentSize.y = textSize.y + style.margin.top + style.margin.bottom;
				if (contentSize.y >= aData.annotation.maxHeight) {
					// there will be a vertical scrollbar - so recalc
					GUIStyle st = GUI.skin.GetStyle ("verticalScrollbar");
					float scrollbarWidth = GUI.skin.GetStyle ("verticalScrollbar").fixedWidth
					                       + st.margin.left + st.margin.right;
					textSize.x -= scrollbarWidth;
					contentSize.x -= scrollbarWidth;
					textSize.y = style.CalcHeight (content, textSize.x);
					contentSize.y = textSize.y + style.margin.top + style.margin.bottom;
					viewSize.y = aData.annotation.maxHeight;
				} else {
					viewSize.y = contentSize.y;
				}
				//viewSize.y = Mathf.Min(contentSize.y, aData.annotation.maxHeight);
			} else {
				float availableTextWidth = textSize.x;
				textSize = style.CalcSize (content);
				textSize.x = Mathf.Max (textSize.x, availableTextWidth);
				contentSize.x = textSize.x + style.margin.left + style.margin.right;
				contentSize.y = textSize.y + style.margin.top + style.margin.bottom;
				if (contentSize.x < viewSize.x) {
					viewSize.y = Mathf.Min (contentSize.y, aData.annotation.maxHeight);
				} else {
					// we have a horizontal scrollbar
					float scrollbarHeight = GUI.skin.GetStyle ("horizontalScrollbar").fixedHeight;
					viewSize.y = Mathf.Min (contentSize.y + scrollbarHeight, aData.annotation.maxHeight);
				} 
			}

			// === The first request is important, so that unity can follow up the layout
			// for the rest we do the placements / sizings by hand
			vRect = EditorGUILayout.GetControlRect (false, viewSize.y, GUIStyle.none, GUILayout.Height (viewSize.y), 
				GUILayout.MinHeight (viewSize.y),
				GUILayout.MaxHeight (viewSize.y),
				GUILayout.ExpandHeight (false)
			);
			vRect.xMin = aData.widthTester.xMin;
			cRect = new Rect (0, 0, contentSize.x, contentSize.y);
			tRect = new Rect (style.margin.left, style.margin.top, textSize.x, textSize.y);

		}

		void DrawScrollArea ()
		{
			ProcessKeyCommands ();

			using (new XoxEditorGUI.ChangeCheck (TextChanged))
			using (new StateSaver.BgColor ())
			using (var sv = new GUI.ScrollViewScope (vRect, scrollPosition, cRect)) {
				scrollPosition = sv.scrollPosition;

				GUIStyle styleToUse;
				if (aData.spAnnotationType.hasMultipleDifferentValues) {
					styleToUse = GUI.skin.textArea;
				} else {
					GUI.backgroundColor = bgColor;
					styleToUse = style;
				}
				FocusId.SetNextId (aData.controlIdTextArea);
				if (aData.spComment.hasMultipleDifferentValues) {
					string str = GUI.TextArea (tRect, "", styleToUse);
					if (str.Length != 0)
						aData.spComment.stringValue = str;
				} else {
					aData.spComment.stringValue = GUI.TextArea (tRect, aData.spComment.stringValue, styleToUse);
				}
				editor = (TextEditor)GUIUtility.GetStateObject (typeof(TextEditor), GUIUtility.keyboardControl);

				KeepCursorVisibleWhileTextEntry ();
			}
		}

		void ProcessKeyCommands ()
		{
			// This if is needed for the case that there are 2 Annotation scripts attached to the same object
			// at least this is where the unusual behaviour was detected that _all_ inspectors of _all_ 
			// components get _all_ events. So infact, this was a needed restriction anyway.
			if (aData.focusManager.HasFocus (aData.controlIdTextArea)) {
				if (Event.current.type == EventType.ValidateCommand) {
					//Debug.Log(">" + Event.current);
					if (Event.current.commandName.Equals ("Copy")) {
						EditCopy ();
					} else if (Event.current.commandName.Equals ("Cut")) {
						EditCut ();
					} else if (Event.current.commandName.Equals ("Paste")) {
						EditPaste ();
					} else if (Event.current.commandName.Equals ("SelectAll")) {
						EditSelectAll ();
					}
				}
			}
		}

		void KeepCursorVisibleWhileTextEntry ()
		{
			// keep the cursor in the visible scroll area
			// this "if" is needed for the same reasons as the if in ProcessKeyCommands
			if (aData.focusManager.HasFocus (aData.controlIdTextArea)) {
				var cursorRect = new Rect (editor.graphicalCursorPos - new Vector2 (10, 20), new Vector2 (30, 50));
				if (lastCursorIndex != editor.cursorIndex) {
					requestTextAreaScrollToCursorPosAfterKeyPress = true;
					lastCursorIndex = editor.cursorIndex;
				}
				if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp) {
					requestTextAreaScrollToCursorPosAfterKeyPress = true;
				}
				if (Event.current.type == EventType.Repaint) {
					if (requestTextAreaScrollToCursorPosAfterKeyPress) {
						GUI.ScrollTo (cursorRect);
						requestTextAreaScrollToCursorPosAfterKeyPress = false;
					}
				}
			}
		}

		#endregion


		#region TextArea Utility Functions

		public void EditSelectAll ()
		{
			editor.SelectAll ();
			aData.Repaint ();
		}

		public void EditCopy ()
		{
			editor.Copy (); 
		}

		public void EditCut ()
		{
			editor.Copy (); 
			RemoveSelection ();
		}

		void TextChanged ()
		{
			if (!AssetManager.canWrite) {
				return;
			}
			aData.serializedObject.ApplyModifiedProperties ();
			aData.serializedObject.Update ();
			aData.spCommentStripped.stringValue = Regex.Replace (aData.spComment.stringValue, @"<[^>]*>", string.Empty);
			aData.serializedObject.ApplyModifiedProperties ();
			aData.serializedObject.Update ();
		}

		public void EditPaste ()
		{
			using (var ute = new UtilityTextEditor (this)) {
				string replaceString = EditorGUIUtility.systemCopyBuffer;
				ute.ReplaceSelection (replaceString);
			}
		}

		public void RemoveSelection ()
		{
			using (var ute = new UtilityTextEditor (this)) {
				ute.RemoveSelection ();
			}
		}

		public void EditRemoveTags ()
		{
			using (var ute = new UtilityTextEditor (this)) {
				string replaceString = Regex.Replace (editor.SelectedText, @"<[^>]*>", string.Empty);
				ute.ReplaceSelection (replaceString);
			}
		}

		public void AddTags (
			string tag,
			string parameter = ""
		)
		{
			using (var ute = new UtilityTextEditor (this)) {
				string replaceString = "<" + tag + parameter + ">" + editor.SelectedText + "</" + tag + ">";
				ute.ReplaceSelection (replaceString);
			}
		}

		class UtilityTextEditor : IDisposable
		{
			public int selectionLength = 0;

			int startIndex = 0;
			int endIndex = 0;
			string newString;
			readonly AnnotationInspectorTextArea parent;

			public UtilityTextEditor (
				AnnotationInspectorTextArea parent
			)
			{
				this.parent = parent;
				if (AssetManager.canWrite) {
					parent.aData.serializedObject.ApplyModifiedProperties ();
				}
				parent.aData.serializedObject.Update ();
				UpdateEditorIndices ();
				newString = parent.aData.annotation.comment;
			}

			public void RemoveSelection ()
			{
				newString = newString.Remove (startIndex, selectionLength);
			}

			public void InsertAtCursor (
				string text
			)
			{
				newString = newString.Insert (startIndex, text);
			}

			public void ReplaceSelection (
				string replacementText
			)
			{
				RemoveSelection ();
				InsertAtCursor (replacementText);
				selectionLength = replacementText.Length;
			}

			void UpdateEditorIndices ()
			{
				if (parent.editor.cursorIndex < parent.editor.selectIndex) {
					startIndex = parent.editor.cursorIndex;
					endIndex = parent.editor.selectIndex;
				} else {
					startIndex = parent.editor.selectIndex;
					endIndex = parent.editor.cursorIndex;
				}
				selectionLength = endIndex - startIndex;
			}

			public void Dispose ()
			{
				parent.editor.text = newString;
				parent.editor.cursorIndex = startIndex;
				parent.editor.selectIndex = startIndex + selectionLength;

				parent.aData.spComment.stringValue = newString;
				parent.TextChanged ();
			}

		}

		
		#endregion

	
	}
}
