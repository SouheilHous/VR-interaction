// ----------------------------------------------------------------------
// File: 		HighlightGlyph
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------

using UnityEngine;

namespace Virtence.VText.Demo {
	/// <summary>
	/// this class highlights the mesh if the mouse is over it
	/// </summary>
	public class HighlightGlyph : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("the color of the glyph if the mouse is over")]
	    public Color HighlightColor = Color.red;    // the color of the glyph if the mouse is over
		#endregion // EXPOSED


		#region CONSTANTS

		#endregion // CONSTANTS


		#region FIELDS
	    private Renderer _renderer;                 // the cached renderer
	    private Color _defaultColor;                // the default color of the glyph
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
		}
		
	    /// <summary>
	    /// this is called if the mouse enters the glyph
	    /// </summary>
	    void OnMouseEnter() {
	        _renderer.material.color = HighlightColor;
	    }

	    /// <summary>
	    /// this is called if the mouse leaves the glyph
	    /// </summary>
	    void OnMouseExit() {
	        _renderer.material.color = _defaultColor;
	    }

		#endregion // METHODS

		#region EVENTHANDLERS

		#endregion // EVENTHANDLERS	
	}
}