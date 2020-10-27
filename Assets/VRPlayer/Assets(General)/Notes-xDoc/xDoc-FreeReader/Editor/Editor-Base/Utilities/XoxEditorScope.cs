namespace xDocEditorBase.UI
{
	using xDocBase.UI;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class XoxEditorScope : XoxScope
	{
		protected XoxEditorScope ()
		{
//			Debug.Log ("Yoo!");
			XoxEditorScopeAccountant.Push (this);
		}

		public override void Dispose ()
		{
//			Debug.Log ("Boooooo!");
			XoxEditorScopeAccountant.Pop ();
			base.Dispose ();
		}

		public void EmergencyDispose ()
		{
			base.Dispose ();
		}
	}
}