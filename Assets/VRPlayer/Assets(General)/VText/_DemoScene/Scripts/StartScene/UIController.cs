// ----------------------------------------------------------------------
// File: 		UIController
// Organisation: 	Virtence GmbH
// Department:   	Simulation Development
// Copyright:    	© 2014 Virtence GmbH. All rights reserved
// Author:       	Silvio Lange (silvio.lange@virtence.com)
// ----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Virtence.VText.Demo {
	/// <summary>
	/// control the main ui in the start scene
	/// </summary>
	public class UIController : MonoBehaviour 
	{	

		#region EXPOSED 
	    [Tooltip("The wrapper for the VTextInterface")]
	    public VtextHandler VTextController;                    // the wrapper for the VTextInterface

	    //[Tooltip("The dropdown which holds the available fonts")]
	    //public Dropdown FontDropdown;                           // the dropdown which holds the available fonts

	    [Header("Common")]
	    [Tooltip("The label which shows the current font name")]
	    public Text FontNameLabel;                              // the label which shows the current font name

	    [Tooltip("The slider which shows the bevel value")]
	    public Slider SizeSlider;                               // the slider which shows the size value

	    [Tooltip("The slider which shows the bevel value")]
	    public Slider DepthSlider;                              // the slider which shows the depth value

	    [Tooltip("The slider which shows the bevel value")]
	    public Slider BevelSlider;                              // the slider which shows the bevel value

	    [Header("Justify")]
	    [Tooltip("The toggle button for align the text to the left")]
	    public UnityEngine.UI.Toggle MajorModeLeftToggle;                      // the toggle button for align the text to the left

	    [Tooltip("The toggle button for align the text to the center")]
	    public UnityEngine.UI.Toggle MajorModeCenterToggle;                    // the toggle button for align the text to the center

	    [Tooltip("The toggle button for align the text to the right")]
	    public UnityEngine.UI.Toggle MajorModeRightToggle;                     // the toggle button for align the text to the right

	    [Tooltip("The toggle button for align the text in block style")]
	    public UnityEngine.UI.Toggle MajorModeBlockToggle;                     // the toggle button for align the text to the right

	    [Header("Lightprobes")]
	    [Tooltip("The toggle button for using lightprobes")]
	    public UnityEngine.UI.Toggle UseLightProbesToggle;                     // the toggle button for align the text to the right

	    #endregion // EXPOSED


		#region CONSTANTS

		#endregion // CONSTANTS


		#region FIELDS
	    private int _currentFontIndex;      // the index of the current used font
		#endregion // FIELDS


		#region PROPERTIES

		#endregion // PROPERTIES


		#region METHODS
		
		// initialize
		void Awake() 
		{
	        VTextController.FontNameChanged  += OnFontNameChanged;;
	        VTextController.SizeValueChanged += OnSizeChanged;
	        VTextController.DepthValueChanged += OnDepthChanged;
	        VTextController.BevelValueChanged += OnBevelChanged;
	        VTextController.MajorValueChanged += OnMajorLayoutChanged;
		}

	    /// <summary>
	    /// initialize
	    /// </summary>
	    void Start() {
	        //FontDropdown.ClearOptions();
	        //FontDropdown.AddOptions(VTextInterface.GetAvailableFonts());
	    }

	    /// <summary>
	    /// Selects the next font.
	    /// </summary>
	    public void SelectNextFont() {
	        SetFontByIndex(_currentFontIndex + 1);

	    }

	    /// <summary>
	    /// Selects the previous font.
	    /// </summary>
	    public void SelectPreviousFont() {
	        SetFontByIndex(_currentFontIndex - 1);
	    }

	    /// <summary>
	    /// set the depth of the VText
	    /// </summary>
	    /// <param name="value">Value.</param>
	    public void SetSize(float value) {        
	        VTextController.SetSize(Mathf.Clamp01(value));
	    }
		
	    /// <summary>
	    /// set the depth of the VText
	    /// </summary>
	    /// <param name="value">Value.</param>
	    public void SetDepth(float value) {
	        VTextController.SetDepth(Mathf.Clamp01(value));
	    }

	    /// <summary>
	    /// set the depth of the VText
	    /// </summary>
	    /// <param name="value">Value.</param>
	    public void SetBevel(float value) {
	        VTextController.SetBevel(Mathf.Clamp01(value));
	    }

	    /// <summary>
	    /// Sets the major layout to Start
	    /// </summary>
	    public void SetMajorLayoutLeft(bool enabled) {
	        if (enabled) {
	            VTextController.SetAlignment(VTextLayout.align.Start);
	        }
	    } 

	    /// <summary>
	    /// Sets the major layout to Center
	    /// </summary>
	    public void SetMajorLayoutCenter(bool enabled) {
	        if (enabled) {
	            VTextController.SetAlignment(VTextLayout.align.Center);
	        }
	    } 

	    /// <summary>
	    /// Sets the major layout to End
	    /// </summary>
	    public void SetMajorLayoutRight(bool enabled) {
	        if (enabled) {
	            VTextController.SetAlignment(VTextLayout.align.End);
	        }
	    } 

	    /// <summary>
	    /// Sets the major layout to block
	    /// </summary>
	    public void SetMajorLayoutBlock(bool enabled) {
	        if (enabled) {
	            VTextController.SetAlignment(VTextLayout.align.Block);
	        }
	    } 

	    /// <summary>
	    /// set the font by specifying an index (the index int the AvailableFonts)
	    /// </summary>
	    /// <param name="index">Index.</param>
	    private void SetFontByIndex(int index) {
	        List<string> availableFonts = VTextInterface.GetAvailableFonts();

	        _currentFontIndex = index;

	        if (index < 0) {
	            _currentFontIndex = availableFonts.Count - 1;
	        }

	        if (index >= availableFonts.Count) {
	            _currentFontIndex = 0;
	        }

	        VTextController.SetFont(availableFonts[_currentFontIndex]);
	    }

	    /// <summary>
	    /// enable or disable the usage of lightprobes
	    /// </summary>
	    /// <returns><c>true</c>, if light probes was enabled, <c>false</c> otherwise.</returns>
	    /// <param name="enable">If set to <c>true</c> enable.</param>
	    public void EnableLightProbes(bool enable) {
	        VTextController.SetLightProbes(enable);
	    }
		#endregion // METHODS

		#region EVENTHANDLERS
	    /// <summary>
	    /// this is called if the current used fontname changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnFontNameChanged(object sender, GenericEventArgs<string> e)
	    {

	        _currentFontIndex = VTextInterface.GetAvailableFonts().IndexOf(e.Value);
	        FontNameLabel.text = string.Format("{0} ({1}/{2})", e.Value, _currentFontIndex + 1, VTextInterface.GetAvailableFonts().Count);
	    }

	    /// <summary>
	    /// this is called if the size value of the vtext object changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnSizeChanged(object sender, GenericEventArgs<float> e)
	    {
	        SizeSlider.value = e.Value;
	    }

	    /// <summary>
	    /// this is called if the depth value of the vtext object changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnDepthChanged(object sender, GenericEventArgs<float> e)
	    {
	        DepthSlider.value = e.Value;

	        BevelSlider.interactable = (e.Value > Mathf.Epsilon);
	    }

	    /// <summary>
	    /// this is called if the bevel value of the vtext object changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnBevelChanged(object sender, GenericEventArgs<float> e)
	    {
	        BevelSlider.value = e.Value;
	    }

	    /// <summary>
	    /// this is called if the major layout mode changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnMajorLayoutChanged(object sender, GenericEventArgs<VTextLayout.align> e)
	    {
	        switch (e.Value) {
	        case VTextLayout.align.Start:
	        case VTextLayout.align.Base:
	            MajorModeLeftToggle.isOn = true;
	            break;
	        case VTextLayout.align.Center:
	            MajorModeCenterToggle.isOn = true;
	            break;
	        case VTextLayout.align.End:
	            MajorModeRightToggle.isOn = true;
	            break;
	        case VTextLayout.align.Block:
	            MajorModeBlockToggle.isOn = true;
	            break;
	        };
	    }

		#endregion // EVENTHANDLERS
	}
}