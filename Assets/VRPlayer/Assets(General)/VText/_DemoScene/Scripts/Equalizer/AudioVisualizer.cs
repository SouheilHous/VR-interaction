// ----------------------------------------------------------------------
// File: 		AudioVisualizer
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------

using UnityEngine;  
using System.Collections; 

namespace Virtence.VText.Demo {
	/// <summary>
	/// animate the letters to the rythm of the music 
	/// </summary>
	public class AudioVisualizer : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("the text which should work as an equalizer")]
	    public VTextInterface DancingText;                  // the text which should work as an equalizer

	    [Tooltip("the scale of the sample to get a better amplitude")]
	    public float Amplitude = 50.0f;                     // the scale factor of the sample value

	    [Tooltip("the lerp duration to get from one y-position to the one provided by the audio source")]
	    public float LerpDuration = 0.2f;                   // the lerp duration to get from one y-position to the one provided by the audio source

	    [Tooltip("the max amplitude to visualize for each letter")]
	    public float MaxAmplitude = 1.0f;                  // the max amplitude to visualize for each letter

	    [Tooltip("minimum strenght of amplitude to react")]
	    public float AmplitudeToReact = 0.5f;               // minimum strenght of amplitude to react

		#endregion // EXPOSED

	   


		#region CONSTANTS
	    private const string EQUALIZER_ANIMATION = "Animate";   // the name of the coroutine to animate VText
		#endregion // CONSTANTS


		#region FIELDS
	    private AudioSource _audioSource;                   // the AudioSource object so the music can be played        
	    private float[] _samples = new float[64];           // this stores the audio samples
		#endregion // FIELDS


		#region PROPERTIES

		#endregion // PROPERTIES


		#region METHODS
	    void Start()  
	    {   
	        _audioSource = GetComponent<AudioSource>(); 

	        // remove all keys from the xy-curve of vtext
	        for (int i = 0; i < DancingText.layout.CurveXY.length; i++) {
	            DancingText.layout.CurveXY.RemoveKey(i);    
	        }

	        for (int i = 0; i < _samples.Length; i++) {
	            float x = (float)i / (float) (_samples.Length - 1);
	            float x2 = (_samples.Length - (float)i )/ (float) (_samples.Length - 1);;
	            Keyframe key = new Keyframe(x, 0);
	            DancingText.layout.CurveXY.AddKey(key);
	        }

	        StartCoroutine(EQUALIZER_ANIMATION);
	    }  

	    // get the spectrum data
	    void Update ()  
	    {          
	        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);  



	    }  

	    /// <summary>
	    /// Animate the VText xy-curve smoothly
	    /// </summary>
	    IEnumerator Animate() {
	        while (true) {
	            float t = 0;

	            while (t < 1) {
	                t += Time.deltaTime / LerpDuration;
	                for (int i = 0; i < _samples.Length; i++) {   
	                    Keyframe key = DancingText.layout.CurveXY[i];
	                    key.value = Mathf.Lerp(key.value, (float)System.Math.Round(_samples[i], 2) * Amplitude, t);
	                    if (key.value > AmplitudeToReact)
	                        key.value = MaxAmplitude;
	                
	                    DancingText.layout.CurveXY.MoveKey(i, key);
	                }  
	                DancingText.Rebuild();
	                yield return null;
	            }
	        }
	    }

		#endregion // METHODS

		#region EVENTHANDLERS

		#endregion // EVENTHANDLERS
	}
}