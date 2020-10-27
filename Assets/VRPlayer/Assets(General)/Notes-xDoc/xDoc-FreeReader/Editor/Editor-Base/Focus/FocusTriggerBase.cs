namespace xDocEditorBase.Focus {

	/// <summary>
	/// Base class for focus event trigger. This class is abstract and defines that 
	/// focus event triggers need to implement a function 'TryToTrigger' which is
	/// invoked by the FocusManager with the name of the UI control name, which 
	/// need to be checked - usually the one with the current focus, or the one 
	/// which previously had the focus (for onLostFocus events).
	/// 
	/// The Callback function wont be store here, as we want the callback function
	/// to be generic with one or more (return) parameters.
	/// 
	/// This class is not generic, as we need it as "base" class to be managed in
	/// lists e.g. by the FocusManager
	/// </summary>
	abstract class TriggerBase
	{
		/// <summary>
		/// The trigger. A focus event will be triggered when the Trigger function is
		/// invoked with an argument, which starts with the 'trigger' string
		/// </summary>
		protected readonly string	trigger;

		/// <summary>
		/// Initializes a new instance of the <see cref="xDocEditor.Focus.TriggerBase"/> class.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		protected TriggerBase(
			string trigger
		)
		{
			this.trigger = trigger;
		}

		protected bool HasTriggered(
			string triggerer
		)
		{
			return triggerer.StartsWith(trigger);
		}

		/// <summary>
		/// This function has to call the respective callback functions once it
		/// is implemented and the focusTriggerer matches the trigger.
		/// 
		/// This is checked with the HasTriggered function
		/// </summary>
		/// <param name="focusTriggerer">Focus triggerer.</param>
		abstract public void TryToTrigger(
			string focusTriggerer
		);

	}

}