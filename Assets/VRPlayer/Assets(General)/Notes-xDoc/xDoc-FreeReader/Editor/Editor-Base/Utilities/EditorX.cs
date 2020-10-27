using UnityEditor;
using UnityEngine;


namespace xDocEditorBase.Extensions {

	/// <summary>
	/// This is just a normal Editor, but provides for an OnGUI-Method (Draw), which can take 
	/// a Rect as argument. The Draw method has to be called manually.
	/// 
	/// Editors are used in conjunction with SerializedObjects and SerializedProperties
	/// </summary>
	public abstract class EditorX : Editor
	{
		/// <summary>
		/// This function has to be overriden to draw the properties of the 
		/// serializedObject in the provided rect.
		/// 
		/// There is no callback mechanism on this function as it is available for
		/// the OnGUI function from Unity. 
		/// 
		/// !!! This funtion has to be called explicitly.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public virtual void Draw(
			Rect rect
		)
		{
		}

		/// <summary>
		/// Applies the modified properties on the serializedObject. This is just a shortcut function.
		/// Besides it returns just void (in contrast to bool) and can be used for ChangeCheck callbacks
		/// with less hassle.
		/// </summary>
		public void ApplyModifiedProperties()
		{
			serializedObject.ApplyModifiedProperties();
		}

	}
}
