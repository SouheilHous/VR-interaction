using xDocBase.Extensions;


namespace xDocBase.AnnotationTypeModule {

	/// <summary>
	/// The AnnotationType defines 
	/// 
	/// 1) the custom data that is being used by the respective annotation and
	/// 
	/// 2) the style of the annotation in 5 different categories: icon, title, text, scene and hierarchy view text
	/// 
	/// All settings are based on the same generic base type. We chose to settings to be SerializedObjects each,
	/// because this enables us to use the EditorClass in the Editor world of unity. They automatically recognize
	/// the right editor to be used. On the other hand side we will have several assets per each annotationType in
	/// the asset file.
	/// 
	/// Alternatively we could have chosen to treat each setting as serialized reference field. This would create only
	/// one SerializedObject per AnnotationType and only 1 asset per AnnotationType in the asset file. On the other hand
	/// side we would then we need to keep track of the class types of the respective PropertyEditors. Even though is
	/// is not a major hassle, we like the first approach better - esp. because the "ScriptableSubAssetObject" takes
	/// care about the separate ScriptableObjects in the asset file.
	/// </summary>
	[System.Serializable]
	public abstract class XDocAnnotationTypeBase : ScriptableSubAssetObject, INamedObject
	{

#region Main

		public const string defaultName = "Annotation";

		public AnnotationTypeData data;
		public AnnotationTypeStyleIcon icon;
		public AnnotationTypeStyleTitle title;
		public AnnotationTypeStyleText text;
		public AnnotationTypeStyleScene scene;
		public AnnotationTypeStyleHierarchyText hierarchyText;

		public void OnEnable()
		{
			// This means the object is already initialized by the serializer
			// We better dont override it.
			if (ScriptableObjectIsInitialized()) {
				return;
			}

			name = defaultName;
			data = new AnnotationTypeData(this);
			icon = new AnnotationTypeStyleIcon(this);
			title = new AnnotationTypeStyleTitle(this);
			text = new AnnotationTypeStyleText(this);
			scene = new AnnotationTypeStyleScene(this);
			hierarchyText = new AnnotationTypeStyleHierarchyText(this);

			data.Init();
			icon.Init();
			title.Init();
			text.Init();
			scene.Init();
			hierarchyText.Init();
			
		}

		bool ScriptableObjectIsInitialized()
		{
			return data != null;
		}

#endregion


#region Settings Objects Helper

		// This region is not to be serialized!

		public enum SettingsIndex
		{
			Data,
			Icon,
			Title,
			Text,
			Scene,
			Hierarchy
		}

		public static int SettingsCount { get { return System.Enum.GetNames(typeof(SettingsIndex)).Length; } }

		public static readonly string[] SettingsNames = {
			"Data",
			"Icon",
			"Title",
			"Text",
			"Scene",
			"Hierarchy"
		};

#endregion


#region INamedObject implementation

		public string Name {
			get { return name; }
			set { name = value; }
		}

#endregion

	}
}
