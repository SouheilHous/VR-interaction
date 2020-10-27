// ----------------------------------------------------------------------
// File: 		ChangeColorWhenHit
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace Virtence.VText.Demo {
	/// <summary>
	/// handle the sun symbol
	/// </summary>
	public class SunController : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("the VText interface for the shout")]
	    public GameObject Shout;             // the VText interface for the shout

	    [Tooltip("the color of the object if it is hit")]
	    public Color HitColor;                      // the color of the object if it is hit

	    [Tooltip("how long (in seconds) the hit color stays until it switches back to its normal color")]
	    public float HitDuration = 1.0f;            // how long (in seconds) the hit color stays until it switches back to its normal color

	    [Tooltip("The duration after the shoult disappears")]
	    public float ShoutDuration = 1.0f;            // the duration after the shoult disappears
		#endregion // EXPOSED


		#region CONSTANTS
	    private const string CHANGE_COLOR_COROUTINE = "ChangeColor";        // the name of the coroutine which changes the objects color
		#endregion // CONSTANTS


		#region FIELDS
	    private Color _defaultColor;                // the default color 
	    private Renderer _renderer;                 // the renderer of this object
		#endregion // FIELDS


		#region PROPERTIES

		#endregion // PROPERTIES


		#region METHODS
		
		// initialize
		void Start() 
		{
	        _renderer = GetComponent<Renderer>();
	        if (_renderer != null) {
	            _defaultColor = _renderer.material.color;
	        }

	        if (Shout != null) {
	            Shout.SetActive(false);
	        }
		}
		
	    /// <summary>
	    /// this is called if another object collides with this
	    /// </summary>
	    /// <param name="other">Other.</param>
	    void OnCollisionEnter(Collision col) {
	        StopCoroutine(CHANGE_COLOR_COROUTINE);
	        StartCoroutine(CHANGE_COLOR_COROUTINE);
	    }

	    #region COROUTINES
	    /// <summary>
	    /// Changes the color of this object to "hit color" and back to its default color afte a while (HitDuration)
	    /// </summary>
	    /// <returns>The color.</returns>
	    private IEnumerator ChangeColor() {
	        if (_renderer != null) {
	            _renderer.material.color = HitColor;
	            Shout.SetActive(true);

	            yield return new WaitForSeconds(HitDuration);
	            _renderer.material.color = _defaultColor;
	            yield return new WaitForSeconds(ShoutDuration);
	            Shout.SetActive(false);
	        }
	    }
	    #endregion // COROUTINES

		#endregion // METHODS

		#region EVENTHANDLERS

		#endregion // EVENTHANDLERS
	}
}
