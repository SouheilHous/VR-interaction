using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Virtence.VText.Demo {
	/// <summary>
	///handle vtext changes in the start scene.
	/// </summary>
	public class VtextHandler : MonoBehaviour {
	    #region EVENTS
	    public event EventHandler<GenericEventArgs<string>> FontNameChanged;                        // this will be raised if the used fontname changes
	    public event EventHandler<GenericEventArgs<float>> SizeValueChanged;                        // this will be raised if the size value of the VTextInterface changes
	    public event EventHandler<GenericEventArgs<float>> DepthValueChanged;                       // this will be raised if the size value of the VTextInterface changes
	    public event EventHandler<GenericEventArgs<float>> BevelValueChanged;                       // this will be raised if the bevel value of the VTextInterface changes
	    public event EventHandler<GenericEventArgs<VTextLayout.align>> MajorValueChanged;           // this will be raised if the major alignment value of the VTextInterface changes
	    public event EventHandler<GenericEventArgs<bool>> UseLightProbesChanged;                    // this will be raised if the usage of lightprobes of the VTextInterface changes
	    #endregion 

	    public VTextInterface[] vti_time = null;
	    public VTextInterface vti_textOptions = null;
	    public VTextInterface vti_textured = null;
			
	    //heading
	    private int old_headingValue;

	    // size
	    private float _minSize = 0.45f;
	    private float _maxSize = 1.0f;

	    //depth
	    private float _minDepth = 0.0f;
	    private float _maxDepth = 3.0f;

	    //bevel
	    private float _minBevel = 0.0f;
	    private float _maxBevel = 0.1f;


	    /// <summary>
	    /// Awake this instance.
	    /// </summary>
	    void Start() { 
	        if (Loom.Current == null)
	            Loom.Initialize();

	        vti_textOptions.layout.SizeChanged += OnSizeChanged;
	        vti_textOptions.layout.MajorChanged += OnMajorLayoutChanged;
	        vti_textOptions.parameter.BevelChanged += OnBevelChanged;
	        vti_textOptions.parameter.DepthChanged += OnDepthChanged;
	        vti_textOptions.parameter.UseLightProbesChanged += OnUseLightProbesChanged;
	        vti_textOptions.parameter.FontNameChanged += OnFontNameChanged;

	        foreach (VTextInterface vi in vti_time) {
	            vi.parameter.UseLightProbes = true;
	        }
	        vti_textOptions.parameter.UseLightProbes = true;
	        vti_textured.parameter.UseLightProbes = true;
			
	        // init alignment
	        old_headingValue = (int) VTextLayout.align.Center;
	        SetAlignment(VTextLayout.align.Center);

	        SetSize(0.4f);                                      // init size
	        SetDepth(0.1f);                                     // init depth
	        SetBevel(0.6f);                                     // init bevel

	        SetFont(vti_textOptions.parameter.Fontname);        // init font type
	        if (FontNameChanged != null) {
	            FontNameChanged.Invoke(this, new GenericEventArgs<string>(vti_textOptions.parameter.Fontname));
	        }
	    }

	    /// <summary>
	    /// Enable or disable light probes both VText odbjects in scene
	    /// </summary>
	    /// <value>The handle lightprobes.</value>
	    void MessageLightprobes(bool lp) {
	        if (vti_time != null) {
	            foreach(VTextInterface vi in vti_time){
	                vi.parameter.UseLightProbes = lp;
	            }
	            vti_textOptions.parameter.UseLightProbes = lp;
	            vti_textured.parameter.UseLightProbes = lp;
	        }
	    }

	    /// <summary>
	    /// enable or disable the usage of lightprobes for the vtext objects
	    /// </summary>
	    /// <param name="enableLightProbes">If set to <c>true</c> enable light probes.</param>
	    public void SetLightProbes(bool enableLightProbes) {
	        foreach (VTextInterface vi in vti_time) {
	            vi.parameter.UseLightProbes = enableLightProbes;
	        }
	        vti_textOptions.parameter.UseLightProbes = enableLightProbes;
	        vti_textured.parameter.UseLightProbes = enableLightProbes;
	    }

	    /// <summary>
	    /// change font of vti_textOptions
	    /// </summary>
	    public void SetFont(string fontName) {
	        vti_textOptions.parameter.Fontname = fontName;
	//        TransformTxt(vti_textOptions.layout.Major);
	    }

	    /// <summary>
	    /// change size of vti_textOptions
	    /// </summary>
	    public void SetSize(float sizeValue) {        
	        if (vti_textOptions != null) {
	            vti_textOptions.layout.Size = _minSize + sizeValue * (_maxSize - _minSize);
	        }
	    }

	    /// <summary>
	    /// change depth of vti_textOptions
	    /// </summary>
	    public void SetDepth(float depthValue) {
	        if (vti_textOptions != null) {
	            vti_textOptions.parameter.Depth = _minDepth + depthValue * (_maxDepth - _minDepth);
	        }
	    }

	    /// <summary>
	    /// change bevel of vti_textOptions
	    /// </summary>
	    public void SetBevel(float bevelValue) {
	        if (vti_textOptions != null) {
	            vti_textOptions.parameter.Bevel = _minBevel + bevelValue * (_maxBevel - _minBevel);
	        }
	    }

	    /// <summary>
	    /// Sets the alignment of the vti_textOptions text
	    /// </summary>
	    /// <param name="alignment">Alignment.</param>
	    public void SetAlignment(VTextLayout.align alignment) {
	        vti_textOptions.layout.Major = alignment;
	        TransformTxt(alignment);
	    }

	    /// <summary>
	    /// change transformation of vti_textOptions in dependence of the current alignment to avoid shifts
	    /// </summary>
	    /// <param name="layout">Layout.</param>
	    void TransformTxt(VTextLayout.align alignment) {
	        float width = vti_textOptions.GetBounds().size.x;
	            
	        switch (alignment) {
	        case VTextLayout.align.Base:
	        case VTextLayout.align.Start:
	        case VTextLayout.align.Block:
	            vti_textOptions.transform.localPosition = new Vector3(-width * 0.25f, vti_textOptions.transform.localPosition.y, vti_textOptions.transform.localPosition.z);
	            break;
	        case VTextLayout.align.Center:
	            vti_textOptions.transform.localPosition = Vector3.zero;
	            break;
	        case VTextLayout.align.End:
	            vti_textOptions.transform.localPosition = new Vector3(width * 0.25f, vti_textOptions.transform.localPosition.y, vti_textOptions.transform.localPosition.z);
	            break;
	        }

	    }

	    #region EVENT HANDLERS
	    /// <summary>
	    /// this is called if the size value of the vtext interface changes 
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnSizeChanged (object sender, GenericEventArgs<float> e)
	    {
	        if (SizeValueChanged != null) {
	            
	            float normalizedValue = (e.Value - _minSize) / (_maxSize - _minSize);
	            SizeValueChanged.Invoke(this, new GenericEventArgs<float>(normalizedValue));
	        }
	    }

	    /// <summary>
	    /// this is called if the depth value of the vtext interface changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnDepthChanged (object sender, GenericEventArgs<float> e)
	    {
	        if (DepthValueChanged != null) {

	            float normalizedValue = (e.Value - _minDepth) / (_maxDepth - _minDepth);
	            DepthValueChanged.Invoke(this, new GenericEventArgs<float>(normalizedValue));
	        }
	    }

	    /// <summary>
	    /// this is called if the bevel value of the vtext interface changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnBevelChanged (object sender, GenericEventArgs<float> e)
	    {
	        if (BevelValueChanged != null) {

	            float normalizedValue = (e.Value - _minBevel) / (_maxBevel - _minBevel);
	            BevelValueChanged.Invoke(this, new GenericEventArgs<float>(normalizedValue));
	        }
	    }

	    /// <summary>
	    /// this is called if the major layout changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnMajorLayoutChanged (object sender, GenericEventArgs<VTextLayout.align> e)
	    {
	        if (MajorValueChanged != null) {
	            MajorValueChanged.Invoke(this, e);
	        }
	    }

	    /// <summary>
	    /// this is called if the usage of lightprobe changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnUseLightProbesChanged (object sender, GenericEventArgs<bool> e)
	    {
	        if (UseLightProbesChanged != null) {
	            UseLightProbesChanged.Invoke(this, e);
	        }
	    }

	    /// <summary>
	    /// this is called if the current used fontname changes
	    /// </summary>
	    /// <param name="sender">Sender.</param>
	    /// <param name="e">E.</param>
	    void OnFontNameChanged (object sender, GenericEventArgs<string> e)
	    {
	        if (FontNameChanged != null) {
	            FontNameChanged.Invoke(this, e);
	        }
	    }
	    #endregion // EVENT HANDLERS
	}
}