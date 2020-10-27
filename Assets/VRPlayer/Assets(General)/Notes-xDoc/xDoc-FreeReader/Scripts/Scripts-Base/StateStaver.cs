using UnityEngine;


namespace xDocBase.UI
{

	public abstract class SaverBase<T> : GUI.Scope
	{
		protected T savedValue;

		protected override void CloseScope ()
		{
			Reset ();
		}

		public void Reset ()
		{
			Set (savedValue);
		}

		public abstract void Set (
			T aValue
		);

		public abstract T Get () ;
	}


	public static class StyleSaver
	{

		public abstract class StyleSaverBase<T> : SaverBase<T>
		{
			readonly protected GUIStyle style;

			protected StyleSaverBase (
				GUIStyle style
			)
			{
				this.style = style;
				// Analysis disable once DoNotCallOverridableMethodsInConstructor
				savedValue = Get ();
			}

			protected StyleSaverBase (
				GUIStyle style,
				T aValue
			)
				: this (
					style
				)
			{
				// Analysis disable once DoNotCallOverridableMethodsInConstructor
				Set (aValue);
			}
		}

		public class FixedWidth : StyleSaverBase<float>
		{
			public FixedWidth (
				GUIStyle style
			)
				: base (
					style
				)
			{
			}

			public FixedWidth (
				GUIStyle style,
				float aValue
			)
				: base (
					style,
					aValue
				)
			{
			}

			public override void Set (
				float aValue
			)
			{
				style.fixedWidth = aValue;
			}

			public override float Get ()
			{
				return style.fixedWidth;
			}
		}

		public class FixedHeight : StyleSaverBase<float>
		{
			public FixedHeight (
				GUIStyle style
			)
				: base (
					style
				)
			{
			}

			public FixedHeight (
				GUIStyle style,
				float aValue
			)
				: base (
					style,
					aValue
				)
			{
			}

			public override void Set (
				float aValue
			)
			{
				style.fixedHeight = aValue;
			}

			public override float Get ()
			{
				return style.fixedHeight;
			}
		}

		public class NormalTextColor : StyleSaverBase<Color>
		{
			public NormalTextColor (
				GUIStyle style
			)
				: base (
					style
				)
			{
			}

			public NormalTextColor (
				GUIStyle style,
				Color aValue
			)
				: base (
					style,
					aValue
				)
			{
			}

			public override void Set (
				Color aValue
			)
			{
				style.normal.textColor = aValue;
			}

			public override Color Get ()
			{
				return style.normal.textColor;
			}
		}

		public class FontStyle : StyleSaverBase<UnityEngine.FontStyle>
		{
			public FontStyle (
				GUIStyle style
			)
				: base (
					style
				)
			{
			}

			public FontStyle (
				GUIStyle style,
				UnityEngine.FontStyle aValue
			)
				: base (
					style,
					aValue
				)
			{
			}

			public override void Set (
				UnityEngine.FontStyle aValue
			)
			{
				style.fontStyle = aValue;
			}

			public override UnityEngine.FontStyle Get ()
			{
				return style.fontStyle;
			}
		}
	}

	public static class StateSaver
	{
		public abstract class StateSaverBase<T> : SaverBase<T>
		{
			protected StateSaverBase ()
			{
				// Analysis disable once DoNotCallOverridableMethodsInConstructor
				savedValue = Get ();
			}

			protected StateSaverBase (
				T aValue
			)
				: this ()
			{
				// Analysis disable once DoNotCallOverridableMethodsInConstructor
				Set (aValue);
			}

		}

		public class TextFieldAlignment : StateSaverBase<TextAnchor>
		{
			public TextFieldAlignment ()
			{
			}

			public TextFieldAlignment (
				TextAnchor aValue
			)
				: base (
					aValue
				)
			{
			}

			public override void Set (
				TextAnchor aValue
			)
			{
				GUI.skin.textField.alignment = aValue;
			}

			public override TextAnchor Get ()
			{
				return GUI.skin.textField.alignment;
			}

		}

		public class BgColor : StateSaverBase<Color>
		{
			public BgColor ()
			{
			}

			public BgColor (
				Color newColor
			)
				: base (
					newColor
				)
			{
			}

			public override Color Get ()
			{
				return GUI.backgroundColor;
			}

			public override void Set (
				Color aValue
			)
			{
				GUI.backgroundColor = aValue;
			}

		}

		public class FgColor : StateSaverBase<Color>
		{
			public FgColor ()
			{
			}

			public FgColor (
				Color newColor
			)
				: base (
					newColor
				)
			{
			}

			public override Color Get ()
			{
				return GUI.contentColor;
			}

			public override void Set (
				Color aValue
			)
			{
				GUI.contentColor = aValue;
			}

		}

		public class GuiColor : StateSaverBase<Color>
		{
			public GuiColor ()
			{
			}

			public GuiColor (
				Color newColor
			)
				: base (
					newColor
				)
			{
			}

			public override Color Get ()
			{
				return GUI.color;
			}

			public override void Set (
				Color aValue
			)
			{
				GUI.color = aValue;
			}

		}

		public class GizmoColor : StateSaverBase<Color>
		{
			public GizmoColor ()
			{
			}

			public GizmoColor (
				Color newColor
			)
				: base (
					newColor
				)
			{
			}

			public override Color Get ()
			{
				return Gizmos.color;
			}

			public override void Set (
				Color aValue
			)
			{
				Gizmos.color = aValue;
			}

		}

	}

}