namespace xDocEditorBase.UI
{
	using UnityEngine;
	using UnityEditor;
	using xDocBase.UI;

	public class XoxGUIRect
	{
		public Rect rect;
		GUIStyleX styleX;
		readonly static float buttonHeight;
		readonly static float minWidth;

		static XoxGUIRect ()
		{
			buttonHeight = GUI.skin.button.CalcHeight (new GUIContent ("A"), 20);
			minWidth = EditorGUIUtility.fieldWidth + EditorGUIUtility.labelWidth;
		}

		public static void SetSaveWidth (
			ref Rect aRect,
			float aWidth
		)
		{
			if ( aWidth < minWidth ) {
				aRect.width = minWidth;
			} else {
				aRect.width = aWidth;
			}
		}

		public static void SetSaveWidth (
			ref Rect aRect,
			float aWidth,
			float aMinWidth
		)
		{
			if ( aWidth < aMinWidth ) {
				aRect.width = aMinWidth;
			} else {
				aRect.width = aWidth;
			}
		}

		public XoxGUIRect (
			Rect rect,
			GUIStyleX styleX
		)
		{
			this.rect = new Rect (rect);
			this.styleX = styleX;

			this.rect.x = rect.x;
			this.rect.y = rect.y;
			this.rect.width = rect.width;
			this.rect.height = rect.height;

			this.rect.xMin += this.styleX.style.padding.left;
			this.rect.xMax -= this.styleX.style.padding.right;
			this.rect.yMin += this.styleX.style.padding.top;
			this.rect.yMax -= this.styleX.style.padding.bottom;
		}

		#region SetHeight

		public void SetToButtonHeight ()
		{
			rect.height = GetHeightOfButton ();
		}

		public void SetToLineHeight (
			int aNumberOfLines
		)
		{
			rect.height = GetHeightOfLines (aNumberOfLines);
		}

		public void SetToPropertyHeight (
			SerializedProperty property
		)
		{
			rect.height = GetHeightOfProperty (property);
		}

		public void SetToHeight (
			float aHeight
		)
		{
			rect.height = aHeight;
		}

		#endregion

		#region Move

		public void MoveDown ()
		{
			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		public void AddVSpace (
			int aNumber
		)
		{
			rect.y += GetHeightOfVSpace (aNumber); 
		}

		#endregion

		#region GetHeightOf

		public static float GetHeightOfButton ()
		{
			return buttonHeight;
		}

		public static float GetHeightOfLines (
			int aNum
		)
		{
			return (aNum - 1) *
			(EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) +
			EditorGUIUtility.singleLineHeight;
		}

		public static float GetHeightOfProperty (
			SerializedProperty property
		)
		{
			return EditorGUI.GetPropertyHeight (property);
		}

		public static float GetHeightOfMoveDownSpace ()
		{
			return EditorGUIUtility.standardVerticalSpacing;
		}

		public static float GetHeightOfVSpace (
			int aNum
		)
		{
			return aNum * EditorGUIUtility.standardVerticalSpacing;
		}

		#endregion
	}
}
