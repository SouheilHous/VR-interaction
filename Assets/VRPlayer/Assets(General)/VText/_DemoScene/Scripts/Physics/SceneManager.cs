// ----------------------------------------------------------------------
// File: 		SceneManager
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Virtence.VText.Demo {
	/// <summary>
	/// control the texts in the physics scene
	/// </summary>
	public class SceneManager : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("The vtexts which should be resetted")]
	    public List<VTextInterface> VTexts;               // the vtext interfaces which should be resetted
		#endregion // EXPOSED


		#region CONSTANTS

		#endregion // CONSTANTS


		#region FIELDS

		#endregion // FIELDS


		#region PROPERTIES

		#endregion // PROPERTIES


		#region METHODS
		
		// initialize
		void Start() 
		{
		}

	    /// <summary>
	    /// reset all defined vtext interfaces
	    /// </summary>
	    public void Reset() {
	        foreach (VTextInterface vi in VTexts) {
	            vi.Rebuild();
	        }
	    }
		
		#endregion // METHODS

		#region EVENTHANDLERS

		#endregion // EVENTHANDLERS

		
	}
}
