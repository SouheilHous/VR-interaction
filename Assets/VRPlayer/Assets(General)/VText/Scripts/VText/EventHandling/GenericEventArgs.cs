// ----------------------------------------------------------------------
// File: 		GenericEventArgs
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using System;

/// <summary>
/// allow generic event arguments
/// </summary>
public class GenericEventArgs<T> : EventArgs, IGenericEventArgs<T>
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS

	#endregion // CONSTANTS


	#region FIELDS
    private T _value;                               // the value of this event arg
	#endregion // FIELDS


    #region PROPERTIES
    public T Value { 
        get { return _value; } 
    }
        
    #endregion // PROPERTIES


	#region CONSTRUCTORS

    public GenericEventArgs(T value)
    {
        _value = value;
    }
	#endregion // CONSTRUCTORS


	#region METHODS

	#endregion // METHODS


	#region EVENT HANDLERS

	#endregion // EVENT HANDLERS
}
