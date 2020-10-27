// ----------------------------------------------------------------------
// File: 		TestScript
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace Virtence.VText.Demo {
	/// <summary>
	/// this class rotates the object around its center
	/// </summary>
	public class RotateGlyphs : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("the speed of the rotation")]
	    public float RotationSpeed = 0.5f;              // the speed of the rotation

	    public bool Clockwise;                          // dertermines if the rotation is clockwise or counter clockwise
		#endregion // EXPOSED


		#region CONSTANTS
		#endregion // CONSTANTS


		#region FIELDS
	    private Transform _transform;                   // the cached transform
	    private Renderer _renderer;                     // the cached renderer
		#endregion // FIELDS


		#region PROPERTIES

		#endregion // PROPERTIES


		#region METHODS
		
		// initialize
		void Start() 
		{
	        _transform = transform;
	        _renderer = GetComponent<Renderer>();
		}

	    void Update() {
	        if (GetComponent<Renderer>() != null) {
	            _transform.RotateAround(_renderer.bounds.center, Vector3.up, RotationSpeed * (Clockwise ? 1 : -1));
	        }
	    }
		#endregion // METHODS

		#region EVENTHANDLERS

		#endregion // EVENTHANDLERS
	}
}