using UnityEngine;


namespace xDocEditorBase.Focus {

	public static class FocusId
	{
		/// <summary>
		/// Needed to set the sub / child id's
		/// </summary>
		private static string lastControlName;

		/// <summary>
		/// <para>
		/// -> Gets the name of the "current" control. Current means, the last
		/// lingering control name. Lingering means it was set up up with
		/// GUI.SetNextControlName(lastControlName) and waits now for the 
		/// construction of the next GUI element and then to be consumed by it.
		/// </para>
		/// 
		/// <para>
		/// -> This function will primariliy used by components, which can 
		/// then create sub control names, which all start with the same
		/// "prefix".
		/// </para>
		/// 
		/// <para>
		/// -> The value of this funtions has to be retrieved BEFORE any controlId's are 
		/// requested / assigned, so it does not change and stays the parents control id
		/// for sub-id's - for the case you are planning to use sub-id's.
		/// </para>
		/// </summary>
		/// <returns>The current control name.</returns>
		public static string GetCurrentControlName()
		{
			return lastControlName;
		}


#region GetId

		/// <summary>
		/// Returns the name of a control that has to be assigned to the control with
		/// 'GUI.SetNextControlName'. Possible information and data are
		/// coded into the controls name and have to be extracted in the 
		/// CalcCallbackReturnValues when called by the Trigger
		/// </summary>
		/// <returns>The parameter encoded id.</returns>
		/// <param name="controlBaseId">
		/// The parameter base id - without the encoded parameters. 
		/// The variables name so to say.
		/// </param>
		public static string GetId(
			string controlBaseId
		)
		{
			return controlBaseId;
		}

		public static string GetId<T>(
			string controlBaseId,
			T parameter
		)
		{
			return controlBaseId + parameter;
		}

		public static string GetId<M,N>(
			string controlBaseId,
			M parameter1,
			N parameter2
		)
		{
			return controlBaseId + parameter1 + "_" + parameter2;
		}

#endregion


#region SetNextId

		public static string SetNextId(
			string controlBaseId
		)
		{
			lastControlName = GetId(controlBaseId);
			GUI.SetNextControlName(lastControlName);
			return lastControlName;
		}

		public static string SetNextId<T>(
			string controlBaseId,
			T parameter
		)
		{
			lastControlName = GetId(controlBaseId, parameter);
			GUI.SetNextControlName(lastControlName);
			return lastControlName;
		}

		public static string SetNextId<M,N>(
			string controlBaseId,
			M parameter1,
			N parameter2
		)
		{
			lastControlName = GetId(controlBaseId, parameter1, parameter2);
			GUI.SetNextControlName(lastControlName);
			return lastControlName;
		}

#endregion


#region SetSubId

		/// <summary>
		/// <para>Sets the sub identifier.</para>
		/// <para>!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!</para>
		/// <para>!!! The parent ID has to have been set already !!!</para>
		/// <para>!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!</para>
		/// 
		/// <para>The parent ID is set by using one of the SetNextId or SetSubId
		/// methods of this class</para>
		/// 
		/// <para>It is possible to create sub-sub-id's. But there is no mechanism to encode
		/// parameters in sub-id's.</para>
		/// </summary>
		/// <param name = "parentId"></param>
		/// <param name="subId">the sub identifier</param>
		public static string SetSubId(
			string parentId,
			string subId
		)
		{
			// we keep the '_' in the separation string, so that
			// parameter splitting with '_' gives clean parameters 
			// back in the above implementations.

			lastControlName = parentId + "_#" + subId;
			GUI.SetNextControlName(lastControlName);
			return lastControlName;
		}

		/// <summary>
		/// Determines, if the focus change is only within a focus group.
		/// 
		/// A focus group is build by setting by setting successively sub-id's
		/// after the parent-id has been set.
		/// 
		/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!<BR>
		/// !!! This works only on the toplevel parent id  !!!<BR>
		/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!<BR>
		/// 
		/// For nested requests another implementation will be needed. Probably 
		/// something in the lines of the oldimplementation
		/// 
		/// </summary>
		/// <returns>
		/// <c>true</c> 
		/// if it is only a focus change within a control group; otherwise, 
		/// <c>false</c>.</returns>
		/// <param name="lastFocus">Last focus - to be gotten from FocusManager.</param>
		/// <param name="currentFocus">Current focus - to be gotten from FocusManager.</param>
		public static bool IsSubFocusChange(
			string lastFocus,
			string currentFocus
		)
		{
			string[] args1 = lastFocus.Split('#');
			string[] args2 = currentFocus.Split('#');

			if (args1[0].Equals(args2[0]))
				return true;
			return false;
		}

#if oldImplementation
	public static bool IsSubFocusChange(
		string lastFocus,
		string currentFocus
	)
	{
		string[] args1 = lastFocus.Split('_');
		string[] args2 = currentFocus.Split('_');

		if (args1.Length != 3 || args2.Length != 3)
			return false;

		if (args1[0].Equals(args2[0]))
		if (args1[1].Equals(args2[1]))
			return true;
		return false;
	}
#endif

#endregion


	}
}

