using UnityEditor;
using UnityEngine;
using xDocEditorBase.UI;


namespace xDocEditorBase.Extensions
{

	/// <summary>
	/// PropertyEditor emulates the behaviour of the Editor class, which works
	/// for SerializedObjects, for SerializedProperty's.
	/// The constructor should be used to set up the child properties and Draw 
	/// routines shall be provided for the representation on the UI.
	/// </summary>
	public abstract class PropertyEditor
	{
		public SerializedObject serializedObject;
		public SerializedProperty baseProperty;

		Vector2 scrollPosition;

		protected PropertyEditor (
			SerializedProperty baseProperty
		)
		{
			this.baseProperty = baseProperty;
			serializedObject = baseProperty.serializedObject;
		}

		/// <summary>
		/// A call to the Update usually should not be needed, as this is only a property editor
		/// and it can be assumed that already some routine directly in control of the 
		/// serializedObject has called the Update already. Nevertheless be sure about the call
		/// order of routines.
		/// </summary>
		public void Update ()
		{
			serializedObject.Update ();
		}

		public void ApplyModifiedProperties ()
		{
			serializedObject.ApplyModifiedProperties ();
		}

		public virtual void Draw ()
		{
			Debug.LogError ("Please implement this method before using it!");
		}

		public virtual void Draw (
			Rect rect
		)
		{
			Debug.LogError ("Please implement this method before using it!");
		}
	}

}
