using xDocBase.UI;


namespace xDocBase.AnnotationTypeModule {

	/// <summary>
	/// This class is a special base class for annotationType settings. It contains a
	/// GUIStyleX element. Most annotationType settings will derive from this class,
	/// as annotationType settings usually define styles.
	/// </summary>
	[System.Serializable]
	public abstract class AnnotationTypeStyleBase : AnnotationTypeAttributeSetBase
	{
		public GUIStyleX styleX;

		protected AnnotationTypeStyleBase(
			string name,
			XDocAnnotationTypeBase parent
		)
			: base(
				name,
				parent
			)
		{
		}

		public override void Init()
		{
			styleX = new GUIStyleX();
			styleX.Initialize();
			styleX.name = "GUIStyleX." + name;
			styleX.style.name = "GUIStyle." + name;
			InitStyleX();
			InitStyleOptions();
		}

		abstract protected void InitStyleX();

		abstract protected void InitStyleOptions();

	}
}

