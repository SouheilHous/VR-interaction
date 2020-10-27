// ----------------------------------------------------------------------
// File: 		ComponentExtensions
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------
using UnityEngine;
using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Virtence.VText.Extensions {
	/// <summary>
	/// 
	/// </summary>
	public static class ComponentExtensions 
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
	    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
	    {
	        Type type = comp.GetType();
	        if (type != other.GetType()) return null; // type mis-match
	        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
	        PropertyInfo[] pinfos = type.GetProperties(flags);
	        foreach (var pinfo in pinfos) {
	            if (pinfo.CanWrite) {
	                try {
	                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
	                }
	                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
	            }
	        }
	        FieldInfo[] finfos = type.GetFields(flags);
	        foreach (var finfo in finfos) {
	            finfo.SetValue(comp, finfo.GetValue(other));
	        }
	        return comp as T;
	    }

		#endregion // METHODS


		#region EVENT HANDLERS

		#endregion // EVENT HANDLERS
	}
}
