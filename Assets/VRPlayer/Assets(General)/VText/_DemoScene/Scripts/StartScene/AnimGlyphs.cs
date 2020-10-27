// ----------------------------------------------------------------------
// File:        VTextEditorLayout
// Organisation:    Virtence GmbH
// Department:      Simulation Development
// Copyright:       © 2014 Virtence GmbH. All rights reserved
// Author:          Silvio Lange (dirk.schulz@virtence.com)
// ----------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace Virtence.VText.Demo {
	/// <summary>
	/// Simple Animation for glyphs.
	/// Just a sinus wave on y-axis...
	/// </summary>
	public class AnimGlyphs : MonoBehaviour {
	    #region Publics
	    public float Amplitude = 0.02f; //the amplitude defines the moving range on y-axis
	    public float FrequencyFactor = 1.0f; //the frequency factor defines the sinus strech on x-axis
	    #endregion //Publics

	    #region METHODS
	    void Update () {
	        for(int k=0; k < this.transform.childCount; k++) {
	            Transform t = this.transform.GetChild(k);
	            float dist = (t.localPosition.x + FrequencyFactor*Mathf.PI * Time.time);
	            t.localPosition = new Vector3(t.localPosition.x,
	                t.localPosition.y + (Mathf.Sin(dist)*Amplitude),
	                t.localPosition.z);
	        }
	    }
	    #endregion //Methods
	}
}