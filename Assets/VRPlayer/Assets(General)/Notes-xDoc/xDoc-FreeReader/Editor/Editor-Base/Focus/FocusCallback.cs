namespace xDocEditorBase.Focus {

	/// <summary>
	/// Generic callback function for the FocusEvent.Trigger. This delegate will be 
	/// added to the FocusManager to be invoked (with controlArg as parameter) when
	/// a certain focus event happens.
	/// </summary>
	/// <param name="controlArg">
	/// This parameter is the "return value" from the FocusManager to the subscribers
	/// of the FocusManager, for whome the focus manager is screening the focus events. 
	/// 
	/// The argument is (usually) either the name of the control who has been 
	/// checked for the focus events or will be calculated from it.
	/// 
	/// ControlNames will be used to encode parameters for callbacks:
	///   prefix
	///   prefix + arg
	///   prefix + arg + "_" + arg
	/// sub-control elements will get an ID appended (separated by a "_"), e.g.
	///   prefix + arg + "_" + arg + "_" + "subID"
	///
	/// A Callback without any parameters is not designed, bc. it is assumed that at least
	/// the control name (without encoded parameters) has to be handed over via the callback.
	/// </param>
	public delegate void Callback<T>(T controlArg);

	/// <summary>
	/// Generic callback function for the FocusEvent.Trigger. This delegate will be 
	/// added to the FocusManager to be invoked (with controlArg as parameter) when
	/// a certain focus event happens.
	/// </summary>
	/// <param name="controlArg1">
	/// This parameter is the "return value" from the FocusManager to the subscribers
	/// of the FocusManager, for whome the focus manager is screening the focus events. 
	/// 
	/// The argument is (usually) either the name of the control who has been 
	/// checked for the focus events or will be calculated from it.
	/// 
	/// ControlNames will be used to encode parameters for callbacks:<BR>
	///   prefix<BR>
	///   prefix + arg<BR>
	///   prefix + arg + "_" + arg<BR>
	/// sub-control elements will get an ID appended (separated by a "_"), e.g.<BR>
	///   prefix + arg + "_" + arg + "_" + "subID"<BR>
	///
	/// A Callback without any parameters is not designed, bc. it is assumed that at least
	/// the control name (without encoded parameters) has to be handed over via the callback.
	/// </param>
	/// <param name="controlArg2">
	/// This is the 2nd encoded argument in the controls name.
	/// </param>
	public delegate void Callback<M,N>(M controlArg1,N controlArg2);

}