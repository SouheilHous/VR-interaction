namespace xDocEditorBase.Focus {

	/// <summary>
	/// This Trigger works with a callback, which gets the 
	/// triggers control name as string argument back
	/// </summary>
	class Trigger : Trigger<string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="xDocEditor.Focus.Trigger"/> class.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="callback">Callback.</param>
		public Trigger(
			string trigger,
			Callback<string> callback
		)
			: base(
				trigger,
				callback
			)
		{
		}

		/// <summary>
		/// Implementation of the requested abstract function.
		/// 
		/// Gets the callback return value from the class implementing this abstract class.
		/// 
		/// This funtion will be called when the callback function needs to be called
		/// </summary>
		/// <returns>The callback return value.</returns>
		/// <param name="controlName">Control name which is checked against the trigger event.</param>
		protected override void CalcCallbackReturnValues(
			string controlName
		)
		{
			controlArg1 = controlName;
		}


	}

	/// <summary>
	/// This Trigger works with a callback, which gets the
	/// string following the triggers control name as string argument back
	/// 
	/// I.e. the controls name is stripped of first.
	/// </summary>
	class TriggerString : Trigger<string>
	{
		readonly int triggerLen;

		/// <summary>
		/// Initializes a new instance of the <see cref="xDocEditor.Focus.TriggerString"/> class.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="callback">Callback.</param>
		public TriggerString(
			string trigger,
			Callback<string> callback
		)
			: base(
				trigger,
				callback
			)
		{
			triggerLen = trigger.Length;
		}

		/// <summary>
		/// Implementation of the requested abstract function.
		/// 
		/// Gets the callback return value from the class implementing this abstract class.
		/// 
		/// This funtion will be called when the callback function needs to be called
		/// </summary>
		/// <returns>The callback return value.</returns>
		/// <param name="controlName">Control name which is checked against the trigger event.</param>
		protected override void CalcCallbackReturnValues(
			string controlName
		)
		{
			// parse the remaining part to int
			controlArg1 = controlName.Substring(triggerLen);
		}


	}

	/// <summary>
	/// This Trigger works with a callback, which gets the
	/// number following the triggers control name as int argument back
	/// 
	/// I.e. the controls name is stripped of first.
	/// </summary>
	class TriggerInt : Trigger<int>
	{
		readonly int triggerLen;

		/// <summary>
		/// Initializes a new instance of the <see cref="xDocEditor.Focus.TriggerInt"/> class.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="callback">Callback.</param>
		public TriggerInt(
			string trigger,
			Callback<int> callback
		)
			: base(
				trigger,
				callback
			)
		{
			triggerLen = trigger.Length;
		}

#if OLD_IMPLEMENTATION
	/// <summary>
	/// Implementation of the requested abstract function.
	/// 
	/// Gets the callback return value from the class implementing this abstract class.
	/// 
	/// This funtion will be called when the callback function needs to be called
	/// </summary>
	/// <returns>The callback return value.</returns>
	/// <param name="controlName">Control name which is checked against the trigger event.</param>
	protected override void CalcCallbackReturnValues(
		string controlName
	)
	{
		// parse the remaining part to int
		controlArg1 = int.Parse(controlName.Substring(triggerLen));
	}
#endif

		/// <summary>
		/// Implementation of the requested abstract function.
		/// 
		/// Gets the callback return value from the class implementing this abstract class.
		/// 
		/// This funtion will be called when the callback function needs to be called
		/// 
		/// This implementation has the advantage that it works with postfixes (e.g. in Datefields or DataFields)
		/// as well.
		/// </summary>
		/// <returns>The callback return value.</returns>
		/// <param name="controlName">Control name which is checked against the trigger event.</param>
		protected override void CalcCallbackReturnValues(
			string controlName
		)
		{
			string[] args = controlName.Substring(triggerLen).Split('_');
			controlArg1 = int.Parse(args[0]);
		}

	}

	/// <summary>
	/// This Trigger works with a callback, which gets the
	/// 2 numbers following the triggers control name as int arguments back
	/// The 2 ints are separated by a '_' char.
	/// 
	/// I.e. the controls name is stripped of its base name first.
	/// </summary>
	class TriggerIntInt : Trigger<int, int>
	{
		readonly int triggerLen;

		/// <summary>
		/// Initializes a new instance of the <see cref="xDocEditor.Focus.TriggerIntInt"/> class.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="callback">Callback.</param>
		public TriggerIntInt(
			string trigger,
			Callback<int, int> callback
		)
			: base(
				trigger,
				callback
			)
		{
			triggerLen = trigger.Length;
		}

		/// <summary>
		/// Implementation of the requested abstract function.
		/// 
		/// Gets the callback return value from the class implementing this abstract class.
		/// 
		/// This funtion will be called when the callback function needs to be called
		/// </summary>
		/// <returns>The callback return value.</returns>
		/// <param name="controlName">Control name which is checked against the trigger event.</param>
		protected override void CalcCallbackReturnValues(
			string controlName
		)
		{
			string[] args = controlName.Substring(triggerLen).Split('_');
			controlArg1 = int.Parse(args[0]);
			controlArg2 = int.Parse(args[1]);
		}

	}

}