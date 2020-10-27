using UnityEditor;
using xDocBase.UI;


namespace xDocEditorBase.UI {

	public static class EditorStateSaver
	{

		public class Indent : StateSaver.StateSaverBase<int>
		{
			public Indent()
			{
			}

			public Indent(
				int aValue
			)
				: base(
					aValue
				)
			{
			}

			public void Incr()
			{
				Set(Get() + 1);
			}

			public void Decr()
			{
				Set(Get() - 1);
			}

			public override void Set(
				int aValue
			)
			{
				EditorGUI.indentLevel = aValue;
			}

			public override int Get()
			{
				return EditorGUI.indentLevel;
			}
		}

	}
}
