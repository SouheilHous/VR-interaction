using UnityEditor;
using UnityEngine;
using xDocBase.AnnotationTypeModule;
using xDocBase.AssetManagement;
using xDocBase.Extensions;
using xDocBase.UI;
using xDocEditorBase.Extensions;
using xDocEditorBase.UI;


namespace xDocEditorBase.AnnotationTypeModule
{

	public class AnnotationTypeDataEditor : PropertyEditor
	{
		#region Main

		// --------------------------------------------------------------------------
		// --- the target object - helper
		// --------------------------------------------------------------------------
		public XDocAnnotationTypeBase annotationType;

		// --------------------------------------------------------------------------
		// --- serialized properties in the serialized object of the target object
		// --------------------------------------------------------------------------
		SerializedProperty showDataArea;
		SerializedProperty useObjectReferencesList;
		// The one edited
		SerializedProperty dataFieldList;
		readonly AnnotationTypeDataFieldListPropertyEditor dataFieldListEditor;
		// parent
		public readonly XDocAnnotationTypeEditorBase editor;

		public AnnotationTypeDataEditor (
			SerializedProperty serializedProperty,
			XDocAnnotationTypeEditorBase editor
		)
			: base (
				serializedProperty
			)
		{
			this.editor = editor;
			annotationType = serializedObject.targetObject as XDocAnnotationTypeBase;

			showDataArea = serializedProperty.FindPropertyRelative ("showDataArea");
			useObjectReferencesList = serializedProperty.FindPropertyRelative ("useObjectReferencesList");
			dataFieldList = serializedProperty.FindPropertyRelative ("dataFieldList");
			dataFieldListEditor = new AnnotationTypeDataFieldListPropertyEditor (dataFieldList, this);

		}

		#endregion


		#region Draw

		Vector2 scrollPosition;

		// HACK override removed
		override public void Draw (
			Rect rect
		)
		{
			serializedObject.Update ();

			var ccRects = XoxGUI.SplitVertically (
				              rect,
				              AssetManager.settings.styleEditorWindowXContentSub.style,
				              DataAreaOptionsGUI.GetHeight ()
			              );

			// Draw Area Options
			using ( var cs = new DataAreaOptionsGUI (ccRects[0], this) ) {
			}

			// Draw Data Fields
			ccRects[1].yMax -= AssetManager.settings.styleEditorWindowXContent.style.padding.vertical;
			using ( var cs = new CustomDataFieldsGUI (
				                 ccRects[1], 
				                 scrollPosition, 
				                 AssetManager.settings.customDataBGColor,
				                 this
			                 ) ) {
				scrollPosition = cs.scrollPosition;
			}		
		}

		#endregion

		class CustomDataFieldsGUI : XoxEditorGUI.ContentScope
		{
			static Rect viewRect = new Rect (0, 0, 500, 500);
			static readonly float scrollbarWidth;

			static CustomDataFieldsGUI ()
			{
				GUIStyle st = GUI.skin.GetStyle ("verticalScrollbar");
				scrollbarWidth = st.fixedWidth + st.margin.left + st.margin.right;
			}

			public CustomDataFieldsGUI (
				Rect aRect,
				Vector2 aScrollPosition,
				Color aBGColor,
				AnnotationTypeDataEditor parent
			) :
				base (
					aRect,
					aScrollPosition,
					aBGColor,
					GetAndInitViewRect (aRect, parent)
				)
			{
				currentRect.SetToLineHeight (1);
				EditorGUI.LabelField (currentRect.rect, "Custom Data Fields", EditorStyles.boldLabel);
				currentRect.MoveDown ();

				parent.dataFieldListEditor.Draw (currentRect);
			}

			public static Rect GetAndInitViewRect (
				Rect aPositionRect,
				AnnotationTypeDataEditor aParent
			)
			{
				const float minWidth = 500;
				XoxGUIRect.SetSaveWidth (ref viewRect, aPositionRect.width, minWidth);

				viewRect.height = GetHeightOfVPadding () +
				XoxGUIRect.GetHeightOfLines (1) +
				XoxGUIRect.GetHeightOfMoveDownSpace () +
				aParent.dataFieldListEditor.GetHeight (viewRect.width - scrollbarWidth);

				if ( viewRect.height >= aPositionRect.height ) {
					XoxGUIRect.SetSaveWidth (ref viewRect, viewRect.width - scrollbarWidth, minWidth);
				}

				return viewRect;
			}
		}

		class DataAreaOptionsGUI : XoxEditorGUI.ContentScopeNoScroll
		{
			public DataAreaOptionsGUI (
				Rect aRect,
				AnnotationTypeDataEditor parent
			) : base (
					aRect
				)
			{
				currentRect.SetToLineHeight (1);
				EditorGUI.LabelField (currentRect.rect, "Data Area Options", EditorStyles.boldLabel);
				currentRect.MoveDown ();
				EditorGUI.PropertyField (currentRect.rect, parent.showDataArea);
				currentRect.MoveDown ();
				using ( new EditorStateSaver.Indent (1) ) {
					GUI.enabled = parent.showDataArea.boolValue;
					EditorGUI.PropertyField (currentRect.rect, parent.useObjectReferencesList, new GUIContent ("Use Obj.Ref. List"));
					GUI.enabled = true;
				}
			}

			public static float GetHeight ()
			{
				return XoxGUIRect.GetHeightOfLines (3) + GetHeightOfVPadding ();
			}
		}


		#region Utility functions for the data field list

		/// <summary>
		/// <para>
		/// Returns an unique name for the data field.
		/// </para>
		/// 
		/// The reason for this funtion to be in this class, is that this class has a 
		/// reference to the List of data fields (instead of access to the serialized
		/// array).
		/// </summary>
		/// <returns>The unique data field name.</returns>
		/// <param name="baseName">Base name.</param>
		/// <param name="index">Index.</param>
		public string GetUniqueDataFieldName (
			string baseName,
			int index = -1
		)
		{
			return annotationType.data.dataFieldList.GetUniqueName (baseName, index);
		}

		/// <summary>
		/// Commits the possible changes. Needed when window is closed e.g.
		/// </summary>
		public void CommitChanges ()
		{
			dataFieldListEditor.CommitChanges ();
		}

		#endregion

	}
}
