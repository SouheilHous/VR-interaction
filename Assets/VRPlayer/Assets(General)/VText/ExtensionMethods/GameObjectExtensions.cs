// ----------------------------------------------------------------------
// File: 		GameObjectExtensions
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEngine;
using System.Reflection;

namespace Virtence.VText.Extensions {
	/// <summary>
	/// some extensions to game objects
	/// </summary>
	public static class GameObjectExtensions 
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

		#endregion // CONSTRUCTORS


		#region METHODS
	    /// <summary>
	    /// Adds the component of type and sets the values corresponding to "toAdd"s values)
	    /// </summary>
	    /// <returns>The component.</returns>
	    /// <param name="go">Go.</param>
	    /// <param name="toAdd">To add.</param>
	    /// <typeparam name="T">The 1st type parameter.</typeparam>
	    public static Component AddComponentClone(this GameObject go, Component toAdd)
	    {                
	        Component copy = System.ObjectExtensions.Copy<Component>(toAdd);
	        object clone = null;
	        if (copy != null) {
	            clone = go.AddComponent(copy.GetType());

	            System.Reflection.FieldInfo[] fields = clone.GetType().GetFields();
	            foreach (System.Reflection.FieldInfo field in fields)
	            {
	                field.SetValue(clone, field.GetValue(toAdd));
	            }

	            var props = clone.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
	            foreach (var prop in props)
	            {
	                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
	                prop.SetValue(clone, prop.GetValue(toAdd, null), null);
	            }
	        }

	        return copy;
	    }
		#endregion // METHODS


		#region EVENT HANDLERS

		#endregion // EVENT HANDLERS
	}
}
