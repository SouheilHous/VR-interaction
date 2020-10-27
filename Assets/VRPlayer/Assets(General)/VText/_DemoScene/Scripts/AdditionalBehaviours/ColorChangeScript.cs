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
	/// this randomly changes the color of the mesh
	/// </summary>
	public class ColorChangeScript : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("the duration of showing a random color before choosing the next one")]
	    public float ColorDuration = 0.5f;              // the duration of showing a random color before choosing the next one
		#endregion // EXPOSED


		#region CONSTANTS
	    private const string COLOR_CHANGE_COROUTINE = "ChangeColor";
		#endregion // CONSTANTS


		#region FIELDS
	    private MeshRenderer _meshRenderer;
		#endregion // FIELDS


		#region PROPERTIES

		#endregion // PROPERTIES


		#region METHODS
		
		// initialize
		void Start() 
		{
	        _meshRenderer = GetComponent<MeshRenderer>();

	        if (_meshRenderer != null)
	            StartCoroutine(COLOR_CHANGE_COROUTINE);
		}

	    #region COROUTINES
	    private IEnumerator ChangeColor() {
	        while (true) {
	            Color c = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
	            _meshRenderer.material.color = c;

	            yield return new WaitForSeconds(ColorDuration);
	        }
	    }
	    #endregion // COROUTINES

		#endregion // METHODS

		#region EVENTHANDLERS

		#endregion // EVENTHANDLERS

		
	}
}