namespace xDocEditorBase.Focus {

	/// <summary>
	/// Generic focus event trigger class. This class adds the callbacks to the base class.
	/// 
	/// Generics is needed for the callback function parameter. Some subscribers to the
	/// FocusManger may want string arguments to be given back, some may want to have int's
	/// or else,..
	/// 
	/// A Callback without any parameters is not designed, bc. it is assumed that at least
	/// the control name (without encoded parameters) has to be handed over via the callback.
	/// </summary>
	abstract class Trigger<T> : TriggerBase
	{
		protected Callback<T> callback;

		protected T controlArg1;

		protected Trigger(
			string trigger,
			Callback<T> callback
		)
			: base(
				trigger
			)
		{
			this.callback = callback;
		}

		/// <summary>
		/// Trigger the specified triggerer.
		/// </summary>
		/// <param name="focusTriggerer">Name of the control to check the trigger against</param>
		public override void TryToTrigger(
			string focusTriggerer
		)
		{
			if (HasTriggered(focusTriggerer)) {
				CalcCallbackReturnValues(focusTriggerer);
				callback(controlArg1);
			}
		}

		/// <summary>
		/// Gets the callback return value from the class implementing this abstract class.
		/// 
		/// This funtion will be called when the callback function needs to be called
		/// </summary>
		/// <returns>The callback return value.</returns>
		/// <param name="controlName">Control name which is checked against the trigger event.</param>
		abstract protected void CalcCallbackReturnValues(
			string controlName
		);

	}


	/// <summary>
	/// Generic focus event trigger class. This class adds the callbacks to the base class.
	/// 
	/// Generics is needed for the callback function parameter. Some subscribers to the
	/// FocusManger may want string arguments to be given back, some may want to have int's
	/// or else,..
	/// 
	/// This class allows 2 callback parameters to be given back
	/// </summary>
	abstract class Trigger<M, N> : TriggerBase
	{
		protected Callback<M,N> callback;

		protected M controlArg1;
		protected N controlArg2;

		protected Trigger(
			string trigger,
			Callback<M,N> callback
		)
			: base(
				trigger
			)
		{
			this.callback = callback;
		}

		/// <summary>
		/// Trigger the specified triggerer.
		/// </summary>
		/// <param name="focusTriggerer">Name of the control to check the trigger against</param>
		public override void TryToTrigger(
			string focusTriggerer
		)
		{
			if (HasTriggered(focusTriggerer)) {
				CalcCallbackReturnValues(focusTriggerer);
				callback(controlArg1, controlArg2);
			}
		}

		/// <summary>
		/// Gets the callback return value from the class implementing this abstract class.
		/// 
		/// This funtion will be called when the callback function needs to be called
		/// </summary>
		/// <returns>The callback return value.</returns>
		/// <param name="controlName">Control name which is checked against the trigger event.</param>
		abstract protected void CalcCallbackReturnValues(
			string controlName
		);

	}



}