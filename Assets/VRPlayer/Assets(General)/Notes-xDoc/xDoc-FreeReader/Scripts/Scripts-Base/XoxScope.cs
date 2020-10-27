using System;
using UnityEngine;

namespace xDocBase.UI
{
	public abstract class XoxScope : IDisposable
	{
		private bool m_Disposed;

		protected abstract void CloseScope ();

		~XoxScope ()
		{
			if ( !this.m_Disposed ) {
				// Debug.LogError ("XoxScope was not disposed! You should use the 'using' keyword or manually call Dispose.");
				this.m_Disposed = true;
				this.CloseScope ();
			}
		}

		public virtual void Dispose ()
		{
			if ( !this.m_Disposed ) {
				this.m_Disposed = true;
				this.CloseScope ();
			}
		}
	}
}
