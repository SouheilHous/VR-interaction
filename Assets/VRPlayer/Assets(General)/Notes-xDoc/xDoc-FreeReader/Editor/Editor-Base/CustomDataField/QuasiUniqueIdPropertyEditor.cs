namespace xDocEditorBase.CustomData
{
	using UnityEditor;
	using xDocBase.CustomData;
	using xDocEditorBase.Extensions;

	public class QuasiUniqueIdPropertyEditor : PropertyEditor
	{
		public SerializedProperty realtimeSinceStartup;
		public SerializedProperty bigTimeOne;
		public SerializedProperty bigTimeTwo;
		public SerializedProperty randomInt;

		public QuasiUniqueIdPropertyEditor (
			SerializedProperty sPropDataField
		)
			: base (
				sPropDataField
			)
		{
			realtimeSinceStartup = sPropDataField.FindPropertyRelative ("realtimeSinceStartup");
			bigTimeOne = sPropDataField.FindPropertyRelative ("bigTimeOne");
			bigTimeTwo = sPropDataField.FindPropertyRelative ("bigTimeTwo");
			randomInt = sPropDataField.FindPropertyRelative ("randomInt");
		}

		public void InitializeAsNewId ()
		{
			var helper = new QuasiUniqueId ();
			helper.InitializeAsNewId ();
			realtimeSinceStartup.floatValue = helper.realtimeSinceStartup;
			bigTimeOne.intValue = helper.bigTimeOne;
			bigTimeTwo.intValue = helper.bigTimeTwo;
			randomInt.intValue = helper.randomInt;
		}
	}
}
