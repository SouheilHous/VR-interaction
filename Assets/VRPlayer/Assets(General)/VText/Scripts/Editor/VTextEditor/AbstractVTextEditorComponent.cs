// ----------------------------------------------------------------------
// File: 		AbstractVTextEditorComponent
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEditor;

/// <summary>
/// the base of all vtext editor components
/// </summary>
public abstract class AbstractVTextEditorComponent 
{	
	#region EVENTS

	#endregion // EVENTS


	#region CONSTANTS

	#endregion // CONSTANTS


	#region FIELDS

	#endregion // FIELDS


	#region PROPERTIES

	#endregion // PROPERTIES


	#region CONSTRUCTORS

    public AbstractVTextEditorComponent(SerializedObject obj = null, Editor currentEditor = null) 
	{
	}

	#endregion // CONSTRUCTORS


	#region METHODS
    /// <summary>
    /// draw the ui for this component
    /// 
    /// returns true if this aspect of the VText should be updated (mesh, layout, physics, etc)
    /// </summary>
    public abstract bool DrawUI();
	#endregion // METHODS


	#region EVENT HANDLERS

	#endregion // EVENT HANDLERS
}
