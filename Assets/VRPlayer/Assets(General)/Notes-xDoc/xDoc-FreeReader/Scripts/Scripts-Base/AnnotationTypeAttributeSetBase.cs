namespace xDocBase.AnnotationTypeModule {

	/// <summary>
	/// For better managability the setup of an annotationType is devided into separate
	/// "Settings" (data, icon style, title style, etc.).
	/// 
	/// This is the base class for them, which defines that each will have a name and
	/// an Init function.
	/// </summary>
	[System.Serializable]
	public abstract class AnnotationTypeAttributeSetBase
	{
		public string name;
		public XDocAnnotationTypeBase parent;

		protected AnnotationTypeAttributeSetBase(
			string name,
			XDocAnnotationTypeBase parent
		)
		{
			this.parent = parent;
			this.name = name;
		}

		public abstract void Init();
	}
}
