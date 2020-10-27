using UnityEngine;


namespace xDocBase.UI {

	[System.Serializable]
	public class GUIStyleX
	{
		public string name;
		public Color	bgColor;
		public GUIStyle style;

		/// <summary>
		/// Resets to default values.
		/// Somehow unity does not allow this class to to be initialized in the constructor.
		/// You get a error message: 
		///   GetBackgroundInternal can only be called from the main thread.
		///   Constructors and field initializers will be executed from the loading thread when loading a scene.
		///   Don't use this function in the constructor or field initializers, instead move initialization code to the Awake or Start function.
		/// So we let thos who create a new object call Reset afterwards,..
		/// It must be done this way bc. this is a serialized class imho. The problems arise when trying to set
		/// style.normal.textColor.
		/// Still we are not allowed to initialize during the laodup phase, thus the AssetManager e.g. may create a 
		/// GUIStyleX, but not ResetToDefaultValues. But anyway the AssetManager will get the values/objects
		/// from somewhere else.
		/// </summary>
		public void Initialize()
		{
			name = "GUIStyleX";
			bgColor = Color.white;
			style = new GUIStyle();
			style.normal.textColor = Color.black;
			style.normal.background = null;
			style.border = new RectOffset();
			style.fontSize = 0;
			style.fontStyle = FontStyle.Normal;
			style.clipping = TextClipping.Clip;
			style.border = new RectOffset(8, 8, 8, 8);
		}
	}
}
