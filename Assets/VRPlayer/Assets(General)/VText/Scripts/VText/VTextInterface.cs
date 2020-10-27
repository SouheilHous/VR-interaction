/*
 * $Id: VTextInterface.cs 172 2015-03-13 14:05:02Z dirk $
 * 
 * Virtence VFont package
 * Copyright 2014 .. 2016 by Virtence GmbH
 * http://www.virtence.com
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Virtence.VText;
using Virtence.VText.Extensions;
using System;
using System.Linq;
using UnityEngine.Rendering;

/// <summary>
/// VText mesh parameter.
/// 
/// change requires rebuild of glyph meshes
/// </summary>

[System.Serializable]
public class VTextParameter {

    #region EVENTS
    public event EventHandler<GenericEventArgs<float>> DepthChanged;                            // this is raised if the depth value changes
    public event EventHandler<GenericEventArgs<float>> BevelChanged;                            // this is raised if the bevel value changes
    public event EventHandler<GenericEventArgs<bool>> NeedTangentsChanged;                      // this is raised if the need tangents value changes
    public event EventHandler<GenericEventArgs<bool>> BackfaceChanged;                          // this is raised if the backface value changes
    public event EventHandler<GenericEventArgs<float>> CreaseChanged;                           // this is raised if the creast value changes
    public event EventHandler<GenericEventArgs<int>> QualityChanged;                            // this is raised if the tesselation quality value changes
    public event EventHandler<GenericEventArgs<string>> FontNameChanged;                        // this is raised if the font name value changes
    public event EventHandler<GenericEventArgs<ShadowCastingMode>> ShadowCastingModeChanged;    // this is raised if the shadow casting mode value changes
    public event EventHandler<GenericEventArgs<bool>> ReceiveShadowsChanged;                    // this is raised if the receive shadows value changes
    public event EventHandler<GenericEventArgs<bool>> UseLightProbesChanged;                    // this is raised if the use lightprobes value changes

    #endregion // EVENTS

    #region FIELDS
    /// <summary>
    /// The depth of the glyphs.
    /// </summary>
    [SerializeField]
    private float m_depth = 0.0f;
    /// <summary>
    /// The bevel frame of the glyphs.
    /// 
    /// range [0..1] where 1 is max factor of 1/10 width of glyph
    /// </summary>
    [SerializeField]
    private float m_bevel = 0.0f;
    /// <summary>
    /// The need tangents property
    /// 
    /// If set, tangents will be generated for Mesh
    /// </summary>
    [SerializeField]
    private bool m_needTangents = false;
    /// <summary>
    /// create backface
    /// 
    /// If set, backface will be generated for Mesh
    /// </summary>
    [SerializeField]
    private bool m_backface = false;
    /// <summary>
    /// crease angle
    /// 
    /// in degree for smoothing sides and bevel.
    /// </summary>
    [SerializeField]
    private float m_crease = 35.0f;
    /// <summary>
    /// quality
    /// 
    /// in percent. range [0..100]
    /// if not in range no change!
    /// </summary>
    [SerializeField]
    private int m_quality = 20;

    /// <summary>
    /// The fontname must specify a font available in StreamingAsset
    /// folder.
    /// Accepted formats are:
    ///  - ttf
    ///  - otf
    ///  - ps (Postscript)
    /// </summary>
    [SerializeField]
    private string m_fontname = string.Empty;

    /// <summary>
    /// The cast shadows property
    /// 
    /// will be passed to children  Mesh Renderer.
    /// </summary>
    [SerializeField]
    private ShadowCastingMode m_shadowCastMode = ShadowCastingMode.On;

    /// <summary>
    /// The receive shadows property
    /// 
    /// will be passed to children Mesh Renderer.
    /// </summary>
    [SerializeField]
    private bool m_receiveShadows = true;

    /// <summary>
    /// The use light probes property
    /// 
    /// will be passed to children Mesh Renderer.
    /// </summary>
    [SerializeField]
    private bool m_useLightProbes = false;

    [HideInInspector]
    private bool m_modified = false;

    #endregion // FIELDS

    #region PROPERTIES
    /// <summary>
    /// The depth of the glyphs.
    /// 
    /// getter setter
    /// </summary>
    public float Depth {
        get {
            return m_depth;
        }
        set {
            float v = (value < 0.0f) ? 0.0f : value;
            if (m_depth != v) {
                m_depth = v;

                if ((m_depth - Mathf.Epsilon) < 0)
                    Bevel = 0;

                if (DepthChanged != null) {
                    DepthChanged.Invoke(this, new GenericEventArgs<float>(m_depth));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// The crease angle to generate sides and bevel
    /// 
    /// getter setter
    /// range [10..45]
    /// </summary>
    public float Crease {
        get {
            return m_crease;
        }
        set {
            float v = Mathf.Clamp(value, 10f, 45f);
            if (m_crease != v) {
                m_crease = v;

                if (CreaseChanged != null) {
                    CreaseChanged.Invoke(this, new GenericEventArgs<float>(m_crease));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// The bevel frame of the glyphs.
    /// 
    /// getter setter
    /// range [0..1] where 1 is max factor of 1/10 width of glyph
    /// </summary>
    public float Bevel {
        get {
            return m_bevel;
        }
        set {
            float bevel = Mathf.Clamp01(value);
            if ((Depth - Mathf.Epsilon) < 0) {
                bevel = 0;
            }

            if (m_bevel != bevel) {
                m_bevel = bevel;
                m_modified = true;

                if (BevelChanged != null) {
                    BevelChanged.Invoke(this, new GenericEventArgs<float>(m_bevel));
                }
            }
        }
    }

    /// <summary>
    /// Quality for tesselation
    /// 
    /// getter setter
    /// in percent range [0..100]
    /// </summary>
    public int Quality {
        get {
            return m_quality;
        }
        set {
            if (m_quality != value) {
                m_quality = value;

                if (QualityChanged != null) {
                    QualityChanged.Invoke(this, new GenericEventArgs<int>(m_quality));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// Flag generate backface
    /// 
    /// getter setter
    /// </summary>
    public bool Backface {
        get {
            return m_backface;
        }
        set {
            if (m_backface != value) {
                m_backface = value;

                if (BackfaceChanged != null) {
                    BackfaceChanged.Invoke(this, new GenericEventArgs<bool>(m_backface));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// Flag generate tangents
    /// 
    /// getter setter
    /// </summary>
    public bool GenerateTangents {
        get {
            return m_needTangents;
        }
        set {
            if (m_needTangents != value) {
                m_needTangents = value;

                if (NeedTangentsChanged != null) {
                    NeedTangentsChanged.Invoke(this, new GenericEventArgs<bool>(m_needTangents));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// Fontname
    /// 
    /// getter setter
    /// </summary>
    public string Fontname {
        get {
            return m_fontname;
        }
        set {
            if (m_fontname != value) {
                m_fontname = value;

                if (FontNameChanged != null) {
                    FontNameChanged.Invoke(this, new GenericEventArgs<string>(m_fontname));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// shadow casting Mode
    /// </summary>
    public ShadowCastingMode ShadowCastMode {
        get {
            return m_shadowCastMode;
        }
        set {
            m_shadowCastMode = value;

            if (ShadowCastingModeChanged != null) {
                ShadowCastingModeChanged.Invoke(this, new GenericEventArgs<ShadowCastingMode>(m_shadowCastMode));
            }

            m_modified = true;
        }
    }

    /// <summary>
    /// flag receive shadows
    /// </summary>
    public bool ReceiveShadows {
        get {
            return m_receiveShadows;
        }
        set {
            m_receiveShadows = value;

            if (ReceiveShadowsChanged != null) {
                ReceiveShadowsChanged.Invoke(this, new GenericEventArgs<bool>(m_receiveShadows));
            }

            m_modified = true;
        }
    }

    /// <summary>
    /// flag use Lightprobes
    /// </summary>
    public bool UseLightProbes {
        get {
            return m_useLightProbes;
        }
        set {
            m_useLightProbes = value;

            if (UseLightProbesChanged != null) {
                UseLightProbesChanged.Invoke(this, new GenericEventArgs<bool>(m_useLightProbes));
            }

            m_modified = true;
        }
    }
    #endregion // PROPERTIES

    #region METHODS
    public bool CheckClearModified() {
        if (m_modified) {
            m_modified = false;
            return true;
        }
        return false;
    }

    #endregion // METHODS

}

[System.Serializable]
public class VTextLayout {
    //! alignment
    public enum align {
        Base,
        Start,
        Center,
        End,
        Block
    };

    #region EVENTS
    public event EventHandler<GenericEventArgs<bool>> IsHorizontalLayoutChanged;        // this is raised if the major alignment value changes
    public event EventHandler<GenericEventArgs<float>> SizeChanged;                     // this is raised if the size value changes
    public event EventHandler<GenericEventArgs<align>> MajorChanged;                    // this is raised if the major alignment value changes
    public event EventHandler<GenericEventArgs<align>> MinorChanged;                    // this is raised if the minor alignment value changes
    public event EventHandler<GenericEventArgs<float>> LineSpacingChanged;              // this is raised if the line spacing value changes
    public event EventHandler<GenericEventArgs<float>> GlyphSpacingChanged;             // this is raised if the glyph spacing value changes
    #endregion // EVENTS

    #region FIELDS 
    [SerializeField]
    private bool m_horizontal = true;

    [SerializeField]
    private align m_major = align.Base;

    [SerializeField]
    private align m_minor = align.Base;

    [SerializeField]
    private float m_size = 1.0f;

    [SerializeField]
    private float  m_spacing = 1.0f;

    [SerializeField]
    private float m_glyphSpacing = 0.0f;

    [SerializeField]
    private AnimationCurve m_curveXZ;

    [SerializeField]
    private AnimationCurve m_curveXY;

    [SerializeField]
    private bool m_orientXZ = false;

    [SerializeField]
    private bool m_orientXY = false;

    [SerializeField]
    private bool m_isCircular = false;

    [SerializeField]
    private float m_startRadius = 0.0f;

    [SerializeField]
    private float m_endRadius = 180.0f;

    [SerializeField]
    private float m_circleRadius = 10.0f;

    [SerializeField]
    private bool m_animateRadius = false;

    [SerializeField]
    private AnimationCurve m_curveRadius;

    [HideInInspector]
    private bool m_modified = false;

    #endregion // FIELDS

    #region PROPERTIES
    /// <summary>
    /// Main layout direction.
    /// If false the text will be layout vertical.
    /// </summary>
    public bool Horizontal {
        get {
            return m_horizontal;
        }
        set {
            if (m_horizontal != value) {
                m_horizontal = value;
                if (IsHorizontalLayoutChanged != null) {
                    IsHorizontalLayoutChanged.Invoke(this, new GenericEventArgs<bool>(m_horizontal));
                }
            }

            m_modified = true;
        }
    }

    /// <summary>
    /// The major aligment.
    /// </summary>
    public align Major {
        get {
            return m_major;
        }
        set {
            if (value != m_major) {
                m_major = value;
                m_modified = true;

                if (MajorChanged != null) {
                    MajorChanged.Invoke(this, new GenericEventArgs<align>(m_major));
                }
            }
        }
    }

    /// <summary>
    /// The minor aligment.
    /// </summary>
    public align Minor {
        get {
            return m_minor;
        }
        set {
            if (value != m_minor) {
                m_minor = value;

                if (MinorChanged != null) {
                    MinorChanged.Invoke(this, new GenericEventArgs<align>(m_minor));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// The font size scale factor.
    /// </summary>
    public float Size {
        get {
            return m_size;
        }
        set {
            if (value != m_size) {
                m_size = value;

                if (SizeChanged != null) {
                    SizeChanged.Invoke(this, new GenericEventArgs<float>(m_size));
                }

                m_modified = true;
            }
        }
    }

    /// <summary>
    /// The line spacing factor.
    /// </summary>
    public float Spacing {
        get {
            return m_spacing;
        }
        set {
            if (value != m_spacing) {
                m_spacing = value;
                if (LineSpacingChanged != null) {
                    LineSpacingChanged.Invoke(this, new GenericEventArgs<float>(m_spacing));
                }
                m_modified = true;
            }
        }
    }

    /// <summary>
    /// The spacing between glyphs
    /// </summary>
    public float GlyphSpacing {
        get {
            return m_glyphSpacing;
        }
        set {
            if (value != m_glyphSpacing) {
                m_glyphSpacing = value;
                if (GlyphSpacingChanged != null) {
                    GlyphSpacingChanged.Invoke(this, new GenericEventArgs<float>(m_glyphSpacing));
                }
                m_modified = true;
            }
        }
    }

    /// <summary>
    /// The XZ Curve
    /// </summary>
    public AnimationCurve CurveXZ {
        get {
            return m_curveXZ;
        }
        set {
            m_modified = true;
            m_curveXZ = value;
        }
    }

    /// <summary>
    /// The XY Curve
    /// </summary>
    public AnimationCurve CurveXY {
        get {
            return m_curveXY;
        }
        set {
            m_modified = true;
            m_curveXY = value;
        }
    }

    /// <summary>
    /// adjust orientation for XZ Curve
    /// </summary>
    public bool OrientationXZ {
        get {
            return m_orientXZ;
        }
        set {
            if (value != m_orientXZ) {
                m_modified = true;
                m_orientXZ = value;
            }
        }
    }

    /// <summary>
    /// adjust orientation for XY Curve
    /// </summary>
    public bool OrientationXY {
        get {
            return m_orientXY;
        }
        set {
            if (value != m_orientXY) {
                m_modified = true;
                m_orientXY = value;
            }
        }
    }

    /// <summary>
    /// bend the text circular
    /// </summary>
    public bool OrientationCircular {
        get {
            return m_isCircular;
        }
        set {
            if (value != m_isCircular) {
                m_modified = true;
                m_isCircular = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the start radius.
    /// </summary>
    /// <value>The start radius.</value>
    public float StartRadius {
        get {
            return m_startRadius;
        }
        set {
            if (value != m_startRadius) {
                m_modified = true;
            }
            m_startRadius = value;
        }
    }

    /// <summary>
    /// Gets or sets the end radius.
    /// </summary>
    /// <value>The end radius.</value>
    public float EndRadius {
        get {
            return m_endRadius;
        }
        set {
            if (value != m_endRadius) {
                m_modified = true;
            }
            m_endRadius = value;
        }
    }

    /// <summary>
    /// Gets or sets the radius of the circle.
    /// </summary>
    /// <value>The circle radius.</value>
    public float CircleRadius {
        get {
            return m_circleRadius;
        }
        set {
            if (value != m_circleRadius) {
                m_modified = true;
            }
            m_circleRadius = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether radius should be determined by the AnimationCurve CurveRadius
    /// </summary>
    /// <value><c>true</c> if animate radius; otherwise, <c>false</c>.</value>
    public bool AnimateRadius {
        get {
            return m_animateRadius;
        }
        set {
            if (value != m_animateRadius) {
                m_modified = true;
            }
            m_animateRadius = value;
        }
    }

    /// <summary>
    /// Gets or sets the CurveRadius Animationcurve.
    /// </summary>
    /// <value>The curve radius.</value>
    public AnimationCurve CurveRadius {
        get {
            return m_curveRadius;
        }
        set {
            if (value != m_curveRadius) {
                m_modified = true;
                m_curveRadius = value;
            }
        }
    }

    #endregion // PROPERTIES

    #region CONSTRUCTORS
    public VTextLayout() {
        m_curveXZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        m_curveXY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        m_curveRadius = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
    }
    #endregion // CONSTRUCTORS

    #region METHODS
    /// <summary>
    /// check if the parameters are modified and reset the modify flag
    /// </summary>
    /// <returns><c>true</c>, if the parameters are modified, <c>false</c> otherwise.</returns>
    public bool CheckClearModified() {
        if (m_modified) {
            m_modified = false;
            return true;
        }
        return false;
    }
    #endregion // METHODS
}

/// <summary>
/// the physics parameters for VText objects
/// </summary>
[System.Serializable]
public class VTextPhysics {
    /// <summary>
    /// Bounding box types
    /// TO BE BACKWARD COMPATIBLE ADD NEW ENTRIES ONLY TO THE END OF THE LIST ... THIS IS SERIALIZED AS AN INTEGER!!!!!
    /// </summary>
    public enum ColliderType {
        None,
        Box,
        Mesh,
    };

    #region VARIABLES

    [HideInInspector]
    private bool _modified = false;                                 // determine if the physics parameters are changed or not

    #region COLLIDER
    [SerializeField]
    private ColliderType _colliderType = ColliderType.None;         // the type of the collider which should be added to each glyph

    [SerializeField]
    private PhysicMaterial _colliderMaterial = null;                // the physics material for the collider

    [SerializeField]
    private bool _colliderIsTrigger = false;                        // determines if this collider is a trigger or not

    [SerializeField]
    private bool _colliderIsConvex = false;                         // determines if this collider is convex or not (used for mesh colliders)

    #endregion // COLLIDER

    #region RIGIDBODY
    [SerializeField]
    private bool _createRigidBody = false;                          // automatically create rigidbodys for each glyph

    [SerializeField]
    private float _rigidbodyMass = 1.0f;                            // the mass of this rigidbody

    [SerializeField]
    private float _rigidbodyDrag = 0.0f;                            // the drag value of this rigidbody

    [SerializeField]
    private float _rigidbodyAngularDrag = 0.05f;                    // the angular drag value of this rigidbody

    [SerializeField]
    private bool _rigidbodyUseGravity = false;                      // use gravity or not for this rigidbody

    [SerializeField]
    private bool _rigidbodyIsKinematic = false;                     // determines if this rigidbody is kinematic or not

    #endregion // RIGIDBODY

    #endregion // VARIABLES

    #region PROPERTIES
    #region COLLIDER
    /// <summary>
    /// the type of collider which is created for each glyph
    /// </summary>
    /// <value>the collider type created for each glyph </value>
    public ColliderType Collider {
        get {
            return _colliderType;
        }
        set {
            if (value != _colliderType) {
                _modified = true;
            }
            _colliderType = value;
        }
    }

    /// <summary>
    /// determines if this collider is a trigger or not
    /// </summary>
    /// <value> true if this collider is setup as a trigger </value>
    public bool ColliderIsTrigger {
        get {
            return _colliderIsTrigger;
        }
        set {
            if (value != _colliderIsTrigger) {
                _modified = true;
            }
            _colliderIsTrigger = value;
        }
    }

    /// <summary>
    /// determines if this collider is a trigger or not
    /// </summary>
    /// <value> true if this collider is setup as a trigger </value>
    public bool ColliderIsConvex {
        get {
            return _colliderIsConvex;
        }
        set {
            if (value != _colliderIsConvex) {
                _modified = true;
            }
            _colliderIsConvex = value;
        }
    }

    /// <summary>
    /// the physics material of the collider
    /// </summary>
    /// <value>the collider type created for each glyph </value>
    public PhysicMaterial ColliderMaterial {
        get {
            return _colliderMaterial;
        }
        set {
            if (value != _colliderMaterial) {
                _modified = true;
            }
            _colliderMaterial = value;
        }
    }
    #endregion // COLLIDER

    #region RIGIDBODY 
    public bool CreateRigidBody {
        get { return _createRigidBody; } 
        set { 
            if (value != _createRigidBody) {
                _modified = true;
            }
            _createRigidBody = value;
        }
    }

    public float RigidbodyMass {
        get { return _rigidbodyMass; } 
        set { 
            if (value != _rigidbodyMass) {
                _modified = true;
            }
            _rigidbodyMass = value;
        }
    }

    public float RigidbodyDrag {
        get { return _rigidbodyDrag; } 
        set { 
            if (value != _rigidbodyDrag) {
                _modified = true;
            }
            _rigidbodyDrag = value;
        }
    }

    public float RigidbodyAngularDrag {
        get { return _rigidbodyAngularDrag; } 
        set { 
            if (value != _rigidbodyAngularDrag) {
                _modified = true;
            }
            _rigidbodyAngularDrag = value;
        }
    }
    public bool RigidbodyUseGravity {
        get { return _rigidbodyUseGravity; } 
        set { 
            if (value != _rigidbodyUseGravity) {
                _modified = true;
            }
            _rigidbodyUseGravity = value;
        }
    }

    public bool RigidbodyIsKinematic {
        get { return _rigidbodyIsKinematic; } 
        set { 
            if (value != _rigidbodyIsKinematic) {
                _modified = true;
            }
            _rigidbodyIsKinematic = value;
        }
    }
    #endregion // RIGIDBODY
    #endregion // PROPERTIES

    #region METHODS
    /// <summary>
    /// check if the parameters are modified and reset the modify flag
    /// </summary>
    /// <returns><c>true</c>, if the parameters are modified, <c>false</c> otherwise.</returns>
    public bool CheckClearModified() {
        if (_modified) {
            _modified = false;
            return true;
        }
        return false;
    }
    #endregion // METHODS
}

/// <summary>
/// Additional components for each glyph
/// </summary>
[System.Serializable]
public class VTextAdditionalComponents {
    #region VARIABLES

    [HideInInspector]
    private bool _modified = false;                                 // determine if the additional components are changed or not

    [SerializeField]
    private GameObject _additionalComponentsObject;                 // a dummy gameobject which holds all components which should be added to each glyph

    #endregion // VARIABLES

    #region PROPERTIES
    public GameObject AdditionalComponentsObject {
        get { return _additionalComponentsObject; }

        set {
            if (value != _additionalComponentsObject) {
                _modified = true;
            }
            _additionalComponentsObject = value;
        }
    }
    #endregion // PROPERTIES

    #region METHODS
    /// <summary>
    /// check if the parameters are modified and reset the modify flag
    /// </summary>
    /// <returns><c>true</c>, if the parameters are modified, <c>false</c> otherwise.</returns>
    public bool CheckClearModified() {
        if (_modified) {
            _modified = false;
            return true;
        }
        return false;
    }
    #endregion // METHODS
}

/// <summary>
/// Virtence polygon text interface
/// </summary>
[ExecuteInEditMode]
public class VTextInterface : MonoBehaviour {
    #region FIELDS
    [SerializeField]
    public VTextParameter parameter;                                // the mesh parameters

    [SerializeField]
    public VTextLayout layout;                                      // the layout parameters

    [SerializeField]
    public VTextPhysics Physics;                                    // the physics parameters

    [SerializeField]
    public VTextAdditionalComponents AdditionalComponents;          // the additional components for each glyp

    /// <summary>
    /// The text to render.
    /// might be overridden by external script for dynamic update.
    /// Line breaks by '\n'
    /// </summary>
    [SerializeField]
    public string RenderText = "Hello world";
    /// <summary>
    /// Check change on update
    /// </summary>
    private string m_oldText;


    /// <summary>
    /// Select your Materials.
    /// The meshes produced will have valid uv.
    /// </summary>
    public Material[] materials = new Material[3];
    /// <summary>
    /// Workaround for the dynamic Batching error.
    /// </summary>
    public Material[] usedMaterials = new Material[3];
    private VFontInfo m_fontInfo = null;
    private List<MonoBehaviour> m_changeListener = null;

    /// <summary>
    /// the list of component-types on the glyphs which should not be overwritten or deleted 
    /// </summary>
    private List<Type> _componentsToKeep;
    #endregion // FIELDS

    #region CONSTRUCTORS
    public VTextInterface() {
        parameter = new VTextParameter();
        layout = new VTextLayout();
        Physics = new VTextPhysics();
        AdditionalComponents = new VTextAdditionalComponents();

        _componentsToKeep = new List<Type>() {
            typeof(Transform),
            typeof(Renderer),
            typeof(MeshFilter),
            typeof(Rigidbody),
            typeof(Collider),
        };

    }
    #endregion // CONSTRUCTORS

    #region DESTRUCTORS
    ~VTextInterface() {
        if (null != m_fontInfo) {
            //Debug.Log("shutdown()");
            m_fontInfo.Shutdown();
            m_fontInfo = null;
        }
    }
    #endregion // DESTRUCTORS

    #region METHODS
	void OnEnable() {
		if (gameObject.activeInHierarchy) {
			m_oldText = "";
			m_fontInfo = null;

			// set default material 
			var mat = new Material(Shader.Find("Standard"));
			if (materials[0] == null)
				materials[0] = mat;
			if (materials[1] == null)
				materials[1] = mat;
			if (materials[2] == null)
				materials[2] = mat;

			layout.CheckClearModified();
			Physics.CheckClearModified();
			AdditionalComponents.CheckClearModified();
		}
	}

    /// <summary>
    /// Gets all available fonts.
    /// </summary>
    /// <returns>The available fonts.</returns>
    public static List<string> GetAvailableFonts() {
        DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"));
        FileInfo[] fiarray = di.GetFiles("*.*");
        List<string> result = new List<string>();

        foreach (FileInfo fi in fiarray) {
            if (!fi.Name.StartsWith(".") &&  (fi.Extension.ToUpper() == ".TTF" || fi.Extension.ToUpper() == ".OTF")) {
                result.Add(fi.Name);
            } 
        }

        return result;
    }

    public bool Is3D() {
        if ((parameter.Depth > 0.0f) ||
            (parameter.Bevel > 0.0f)) {
            return true;
        }
        return false;
    }

    public void RegisterListener(MonoBehaviour go) {
        if (null == m_changeListener) {
            m_changeListener = new List<MonoBehaviour>();
        }
        m_changeListener.Add(go);
    }

    public void UnRegisterListener(MonoBehaviour go) {
        if (null != m_changeListener) {
            if (m_changeListener.Contains(go)) {
                m_changeListener.Remove(go);
            }
        }
    }

    private void clearChildren() {
        // Debug.Log("clearChildren()");
        for (int k = transform.childCount - 1; k >= 0; k--) {
            GameObject go = transform.GetChild(k).gameObject;
            Renderer r = go.GetComponent<Renderer>();
            r.enabled = false;
            MeshFilter mf = go.GetComponent<MeshFilter>();
            mf.sharedMesh = null;
            if (Application.isPlaying) {
                Destroy(r);
                Destroy(mf);
                Destroy(go);
            } else {
                DestroyImmediate(r);
                DestroyImmediate(mf);
                DestroyImmediate(go);
            }
        }
        Resources.UnloadUnusedAssets();
    }

    private void UpdateGlyphs(bool updateGeometry = false) {
        if (this == null)
            return; 
        clearChildren();
        if (parameter.Fontname.Length > 4) {
            if (Is3D()) {
                if (null == m_fontInfo) {
                    m_fontInfo = new VFontInfo(parameter.Fontname);
                }
                if (null == m_fontInfo) {
                    Debug.Log("Null fontinfo");
                } else {                    
                    m_fontInfo.SetQuality(parameter.Quality);
                    m_fontInfo.CreateText3D(this, RenderText);
                }
            } else {
                // use common fontinfo
                if (!updateGeometry) {
                    VFontInfo fi = VFontHash.GetFontInfo(parameter.Fontname);
                    fi.SetQuality(parameter.Quality);
                    fi.CreateText3D(this, RenderText);
                } else {
                    // Debug.Log("Update geo");
                    // VFontInfo fi = VFontHash.GetFontInfo(parameter.Fontname);
                    if (null == m_fontInfo) {
                        m_fontInfo = new VFontInfo(parameter.Fontname);
                    }
                    if (null == m_fontInfo) {
                        Debug.Log("Null fontinfo");
                    } else {                    
                        m_fontInfo.SetQuality(parameter.Quality);
                        m_fontInfo.CreateText3D(this, RenderText);
                    }
                }
            }
            if (null != m_changeListener) {
                foreach (MonoBehaviour mb in m_changeListener) {
                    mb.SendMessage("VTextChanged");
                }
            }
        }
    }

    private void UpdateLayout() {
        if (this == null)
            return;
        // Debug.Log("ug " + parameter.fontname);
        if (parameter.Fontname.Length > 4) {
            if (Is3D()) {
                if (null != m_fontInfo) {
                    m_fontInfo.LayoutText3D(this, RenderText);
                }
            } else {
                // use common fontinfo
                VFontInfo fi = VFontHash.GetFontInfo(parameter.Fontname);
                if (null != fi) {
                    fi.LayoutText3D(this, RenderText);
                }
            }
        }
    }

    /// <summary>
    /// update the physics aspects of the vtext
    /// </summary>
    private void UpdatePhysics() {
        for (int i = 0; i < transform.childCount; i++) {
            Transform t = transform.GetChild(i);

            CreateRigidbody(t);
            CreateCollider(t);
        }
    }

    /// <summary>
    /// update the additional components
    /// </summary>
    private void UpdateAdditionalComponents() {
        if (AdditionalComponents.AdditionalComponentsObject != null) {
            // foreach glyph
            for (int i = 0; i < transform.childCount; i++) {
                Transform t = transform.GetChild(i);

                // remove all existing MonoBehaviours of the glyph
                Component[] components = t.GetComponents<Component>();
                foreach (Component c in components) {
                    if (_componentsToKeep.Any(type => type.IsAssignableFrom(c.GetType()))) {
                        continue;
                    }
                        
                    if (Application.isPlaying)
                        Destroy(c);
                    else
                        DestroyImmediate(c);
                }

                // now add copies of the new behaviours
                Component[] componentsToAdd = AdditionalComponents.AdditionalComponentsObject.GetComponents<Component>();
                foreach (Component c in componentsToAdd) {
                    if (_componentsToKeep.Any(type => type.IsAssignableFrom(c.GetType()))) {
                        continue;
                    }
                    t.gameObject.AddComponentClone(c);
                }
            }
        }
    }

    /// <summary>
    /// create or remove the rigidbody component
    /// </summary>
    /// <param name="t"> the transform to change</param>
    private void CreateRigidbody(Transform t) {
        if (Physics.CreateRigidBody) {
            Rigidbody rigidBody = t.GetComponent<Rigidbody>();
            if (rigidBody == null) {
                rigidBody = t.gameObject.AddComponent<Rigidbody>();
            }
            rigidBody.useGravity = Physics.RigidbodyUseGravity;
            rigidBody.mass = Physics.RigidbodyMass;
            rigidBody.drag = Physics.RigidbodyDrag;
            rigidBody.angularDrag = Physics.RigidbodyAngularDrag;
            rigidBody.isKinematic = Physics.RigidbodyIsKinematic;
        } else {
            #if UNITY_EDITOR
            DestroyImmediate(t.GetComponent<Rigidbody>());
            #else
            Destroy(t.GetComponent<Rigidbody>());
            #endif
        }
    }

    /// <summary>
    /// create or remove the collider on the specified transform
    /// </summary>
    /// <param name="t"> the transform to modify </param>
    private void CreateCollider(Transform t) {
        switch (Physics.Collider) {
        case VTextPhysics.ColliderType.None:
            RemoveCollider(t);
            break;

        case VTextPhysics.ColliderType.Box:
            RemoveCollider(t);
            BoxCollider bc = t.gameObject.AddComponent<BoxCollider>();
            bc.material = Physics.ColliderMaterial;
            bc.isTrigger = Physics.ColliderIsTrigger;
            break;

        case VTextPhysics.ColliderType.Mesh:
            RemoveCollider(t);
            MeshCollider mc = t.gameObject.AddComponent<MeshCollider>();
            mc.material = Physics.ColliderMaterial;
            mc.convex = Physics.ColliderIsConvex;
            if (mc.convex) {
                mc.isTrigger = Physics.ColliderIsTrigger;
            } else {
                mc.isTrigger = false;
            }
            break;
        }
    }

    /// <summary>
    /// Removes the collider from the specified transform.
    /// </summary>
    /// <param name="t">T.</param>
    private void RemoveCollider(Transform t) {
        Collider[] colliders = t.GetComponents<Collider>();
        foreach (Collider c in colliders) {
            #if UNITY_EDITOR
            DestroyImmediate(t.GetComponent<Collider>());
            #else
            Destroy(t.GetComponent<Collider>());
            #endif
        }
    }

    public void Rebuild(bool rebuildMesh = false) {
		if (this == null || !this.gameObject.activeInHierarchy)
            return;
            
        if (Loom.Current == null)
            Loom.Initialize();
                
        if (null != m_fontInfo) {
            m_fontInfo.Shutdown();
            m_fontInfo = null;
        }
        UpdateGlyphs(rebuildMesh);
        UpdatePhysics();
        UpdateAdditionalComponents();
    }

    public Bounds GetBounds() {
        if (this == null)
            return new Bounds();
            
        Bounds r = new Bounds();
        if (null != m_fontInfo) {
            return m_fontInfo.GetBounds(this, RenderText);
        }
        return r;
    }

    /// <summary>
    /// Checks the rebuild.
    /// 
    /// use by Editor only
    /// </summary>
    /// <param name="updateMesh">If set to <c>true</c> update mesh.</param>
    /// <param name="updateLayout">If set to <c>true</c> update layout.</param>
    public void CheckRebuild(bool updateMesh, bool updateLayout, bool updatePhysics, bool updateAdditionalComponents) {
        if (this == null || !this.gameObject.activeInHierarchy)
            return;
            
        parameter.CheckClearModified();
        layout.CheckClearModified();
        Physics.CheckClearModified();
        AdditionalComponents.CheckClearModified();

        if (updateMesh) {
            //Debug.Log("update mesh");
            Rebuild(true);
            UpdatePhysics();
            UpdateAdditionalComponents();
        } else if (updateLayout) {
            // Debug.Log("update layout");
            UpdateLayout();
            UpdatePhysics();
            UpdateAdditionalComponents();
        } else if (updatePhysics) {
            UpdatePhysics();
            UpdateAdditionalComponents();
        } else if (updateAdditionalComponents) {
            UpdateAdditionalComponents();
        }
    }

    void Update() {
        if (this == null)
            return;


        if (parameter.CheckClearModified()) {
            // Debug.Log("param update mesh: " + RenderText);
            layout.CheckClearModified();
            Physics.CheckClearModified();
            AdditionalComponents.CheckClearModified();
            Rebuild();
        } else if (m_oldText != RenderText) {
            // Debug.Log("string update mesh: " + RenderText);
            layout.CheckClearModified();
            Physics.CheckClearModified();
            AdditionalComponents.CheckClearModified();
            UpdateGlyphs();
            UpdatePhysics();
            UpdateAdditionalComponents();
        } else if (layout.CheckClearModified()) {
            // Debug.Log("layout update mesh: " + RenderText);
            UpdateLayout();
            UpdatePhysics();
            UpdateAdditionalComponents();
        } else if (Physics.CheckClearModified()) {
            UpdatePhysics();
            UpdateAdditionalComponents();
        } else if (AdditionalComponents.CheckClearModified()) {
            UpdateAdditionalComponents();
        }
        m_oldText = RenderText;
    }
    #endregion // METHODS
}
