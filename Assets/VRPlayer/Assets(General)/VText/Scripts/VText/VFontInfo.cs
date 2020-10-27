/*
 * $Id: VFontInfo.cs 207 2015-04-21 12:56:50Z dirk $
 *
 * Virtence VFont package
 * Copyright 2014 .. 2016 by Virtence GmbH
 * http://www.virtence.com
 *
 */

using UnityEngine;
using System.Linq;

#if UNITY_5
using UnityEngine.Rendering;
#endif

using System.Collections;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;

namespace Virtence.VText {
    /// <summary>
    /// Font bounds.
    ///
    /// Returned from GetFontBounds.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FontBounds {
        /// <summary>
        /// The minimum x.
        /// </summary>
        public float minX;
        /// <summary>
        /// The max x.
        /// </summary>
        public float maxX;
        /// <summary>
        /// The minimum y.
        /// </summary>
        public float minY;
        /// <summary>
        /// The max y.
        /// </summary>
        public float maxY;
    }

    /// <summary>
    /// Virtence font info.
    ///
    /// holds all font related information.
    /// </summary>
    public class VFontInfo {
        private Hashtable m_glyphs = null;
        private System.IntPtr m_fontHandle;
        private FontBounds m_bounds;
        protected string m_fontName;

        /// <summary>
        /// Gets the glyph minimum x.
        /// </summary>
        /// <value>The glyph minimum x.</value>
        public float glyphMinX {
            get {
                return m_bounds.minX;
            }
        }

        /// <summary>
        /// Gets the glyph max x.
        /// </summary>
        /// <value>The glyph max x.</value>
        public float glyphMaxX {
            get {
                return m_bounds.maxX;
            }
        }

        /// <summary>
        /// Gets the glyph minimum y.
        /// </summary>
        /// <value>The glyph minimum y.</value>
        public float glyphMinY {
            get {
                return m_bounds.minY;
            }
        }

        /// <summary>
        /// Gets the glyph max y.
        /// </summary>
        /// <value>The glyph max y.</value>
        public float glyphMaxY {
            get {
                return m_bounds.maxY;
            }
        }

        #region dll

        #if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_XBOX360)
            [DllImport ("__Internal")]
			private static extern System.IntPtr OpenFont([In] System.IntPtr fontFilename);

			[DllImport ("__Internal")]
			private static extern void CloseFont([In] System.IntPtr fontHandle);

			[DllImport ("__Internal")]
			private static extern float GetAscender([In] System.IntPtr fontHandle);

			[DllImport ("__Internal")]
			private static extern float GetDescender([In] System.IntPtr fontHandle);

			[DllImport ("__Internal")]
			private static extern bool GetFontBounds([Out] System.IntPtr b, [In] System.IntPtr fontHandle);

			[DllImport ("__Internal")]
			private static extern bool GetKerning2(ref float kx, ref float ky, [In] System.IntPtr fontHandle, uint first, uint second);

            [DllImport("__Internal")]
            private static extern void SetQuality([In] System.IntPtr fontHandle, int percent);

#else
        [DllImport("VText")]
        private static extern System.IntPtr OpenFont([In] System.IntPtr fontFilename);

        [DllImport("VText")]
        private static extern void CloseFont([In] System.IntPtr fontHandle);

        [DllImport("VText")]
        private static extern float GetAscender([In] System.IntPtr fontHandle);

        [DllImport("VText")]
        private static extern float GetDescender([In] System.IntPtr fontHandle);

        [DllImport("VText")]
        private static extern bool GetFontBounds([Out] System.IntPtr b, [In] System.IntPtr fontHandle);

        [DllImport("VText")]
        private static extern bool GetKerning2(ref float kx, ref float ky, [In] System.IntPtr fontHandle, uint first, uint second);

        [DllImport("VText")]
        private static extern void SetQuality([In] System.IntPtr fontHandle, int percent);
        #endif
        #endregion

        private void SafeOpenFont(string fn) {
            string pPath = System.IO.Path.Combine(Application.persistentDataPath, fn);
            string filePath = System.IO.Path.Combine(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"), fn);
            // Debug.Log("Fontinfo Stream " + filePath);
            // Debug.Log("Persistent Data " + pPath);
            if (filePath.Contains("://")) {
                if (File.Exists(pPath)) {
                    Debug.Log("Persistent Data exists " + pPath);
                    m_fontHandle = OpenFont(Marshal.StringToHGlobalAnsi(pPath));
                    Debug.Log("*** VFontInfo " + m_fontHandle + " ***");
                } else {
                    // Android copy from apk to Persistent
                    WWW w = new WWW(filePath);
                    while (!w.isDone) {
                        new WaitForSeconds(0.5f);
                    }
                    Debug.Log("read done");
                    File.WriteAllBytes(pPath, w.bytes);
                    Debug.Log(w.bytes.Length + " copy done " + pPath);
                }
                m_fontHandle = OpenFont(Marshal.StringToHGlobalAnsi(pPath));
            } else {
                m_fontHandle = OpenFont(Marshal.StringToHGlobalAnsi(filePath));
            }
            SetQuality(m_fontHandle, 19);
            // Debug.Log("*** VFontInfo " + m_fontHandle + " ***");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Virtence.VText.VFontInfo"/> class.
        /// </summary>
        /// <param name="fontname">Fontname.</param>
        public VFontInfo(string fontname) {
            // Debug.Log("*** VFontInfo " + fontname + " ***");
            try {
                m_glyphs = null;
                m_glyphs = new Hashtable();

                m_fontName = fontname;
                SafeOpenFont(m_fontName);
                if (System.IntPtr.Zero != m_fontHandle) {
                    int rawsize = Marshal.SizeOf(m_bounds);
                    System.IntPtr ip = Marshal.AllocHGlobal(rawsize);
                    if (GetFontBounds(ip, m_fontHandle)) {
                        // Debug.Log(ip);
                        m_bounds = (FontBounds)Marshal.PtrToStructure(ip, typeof(FontBounds));
                        // Debug.Log("minX " + m_bounds.minX.ToString("F5") + " maxX " + m_bounds.maxX.ToString("F5"));
                        // Debug.Log("minY " + m_bounds.minY.ToString("F5") + " maxY " + m_bounds.maxY.ToString("F5"));
                    } else {
                        Debug.LogWarning("Get bounds failed");
                    }
                    Marshal.FreeHGlobal(ip);
                }
            } catch (System.Exception e) {
                Debug.Log("Fontinfo failed " + e.ToString());
            }
        }

        ~VFontInfo() {
            Shutdown();
        }

        public void Shutdown() {
            if (System.IntPtr.Zero != m_fontHandle) {
                // Debug.Log("close font " + m_fontHandle);
                CloseFont(m_fontHandle);
                m_fontHandle = System.IntPtr.Zero;
            }
            m_glyphs = null;
        }

        public void SetQuality(int percent) {
            if (System.IntPtr.Zero != m_fontHandle) {                
                SetQuality(m_fontHandle, percent);
            }
        }

        /// <summary>
        /// Fetchs the glyph info.
        /// </summary>
        /// <returns>The glyph info.</returns>
        /// <param name="glyphs">Glyphs.</param>
        /// <param name="c">C.</param>
        protected VGlyphInfo FetchGlyphInfo(Hashtable glyphs, char c) {
            if (((int)c) > 16) {
                if (!glyphs.ContainsKey(c)) {
                    VGlyphInfo gi = new VGlyphInfo(m_fontHandle, c);
                    glyphs.Add(c, gi);
                }
                if (glyphs.ContainsKey(c)) {
                    VGlyphInfo gi = (VGlyphInfo)glyphs[c];
                    return gi;
                }
            }
            return null;
        }

        protected Vector2 Kerning(char a, char b) {
            Vector2 res = new Vector2(0f, 0f);
            if (System.IntPtr.Zero != m_fontHandle) {
                float kx = 0f;
                float ky = 0f;
                if (GetKerning2(ref kx, ref ky, m_fontHandle, (uint)a, (uint)b)) {
                    res.x = kx;
                    res.y = ky;
                }
            }
            return res;
        }

        /// <summary>
        /// Lines the size.
        /// </summary>
        /// <returns>The size.</returns>
        /// <param name="vtext">Vtext.</param>
        /// <param name="str">String.</param>
        public Vector2 LineSize(VTextInterface vtext, string str) {
            float xmax = 0.0f;
            float ymax = 0.0f;
            float lw = 0.0f;
            float lh = 0.0f;
            if (vtext.layout.Horizontal) {
                Vector2 kern = new Vector2(0f, 0f);
                char prev = '\0';
                for (int i = 0; i < str.Length; i++) {
                    VGlyphInfo gi = FetchGlyphInfo(m_glyphs, str[i]);
                    if (null != gi) {
                        if ('\0' != prev) {
                            kern = Kerning(prev, str[i]);
                        }

                        if (i < str.Length - 1) {
							lw += gi.advance.x + kern.x + vtext.layout.GlyphSpacing;
                        } else {
                            lw += gi.size.x + kern.x;
                        }
                        lh = gi.advance.y + kern.y;
                        if (ymax < lh) {
                            ymax = lh;
                        }
                        prev = str[i];
                    }
                }
                // Debug.Log(" width " + lw);
                if (lw > xmax) {
                    xmax = lw;
                }
            } else { // vertical layout
                for (int i = 0; i < str.Length; i++) {
                    VGlyphInfo gi = FetchGlyphInfo(m_glyphs, str[i]);
                    if (null != gi) {
                        lh += gi.advance.y;
                    }
                }
                ymax = lh;
                lw = GetAscender(m_fontHandle);
                // Debug.Log(" Ascender width " + lw);
                xmax = lw;
            }
            return new Vector2(xmax * vtext.layout.Size, ymax * vtext.layout.Size);
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <returns>The bounds.</returns>
        /// <param name="vtext">Vtext.</param>
        /// <param name="str">String.</param>
        public Bounds GetBounds(VTextInterface vtext, string str) {
            Bounds r = new Bounds();
            float ascender = GetAscender(m_fontHandle);
            // float descender = GetDescender(m_fontHandle);
            // Debug.Log(ascender + " descender " + descender);
            string[] sa = str.Split('\n');
            Vector2[] linesizes = new Vector2[sa.Length];
            float maxWidth = 0.0f;
            float maxHeight = 0.0f;
            for (int k = 0; k < sa.Length; k++) {
                linesizes[k] = LineSize(vtext, sa[k]);
                if (vtext.layout.Horizontal) {
                    if (linesizes[k].x > maxWidth) {
                        maxWidth = linesizes[k].x;
                    }
                    maxHeight += linesizes[k].y;
                } else {
                    maxWidth += linesizes[k].x;
                    if (linesizes[k].y > maxHeight) {
                        maxHeight = linesizes[k].y;
                    }
                }
            }
            float xcenter = 0f;
            float ycenter = 0f;

            if (vtext.layout.Horizontal) {
                switch (vtext.layout.Major) {
                case VTextLayout.align.Base:
                    xcenter = (maxWidth * 0.5f);
                    break;
                case VTextLayout.align.Center:
                    xcenter = 0f;
                    break;
                case VTextLayout.align.End:
                    xcenter = maxWidth;
                    break;
                case VTextLayout.align.Start:
                case VTextLayout.align.Block:
                    xcenter = (maxWidth * 0.5f);
                    break;
                }
                switch (vtext.layout.Minor) {
                case VTextLayout.align.Base:
                    ycenter = (maxHeight - ascender) * 0.5f;
                    break;
                case VTextLayout.align.Center:
                    ycenter = 0f;
                    break;
                case VTextLayout.align.End:
                    ycenter = (maxHeight - ascender);
                    break;
                case VTextLayout.align.Start:
                case VTextLayout.align.Block:
                    ycenter = -ascender;
                    break;
                }
            } else { // vertical
                switch (vtext.layout.Major) {
                case VTextLayout.align.Base:
                    ycenter = maxHeight * 0.5f;
                    break;
                case VTextLayout.align.Center:
                    ycenter = 0.0f;
                    break;
                case VTextLayout.align.End:
                    ycenter = -maxHeight * 0.5f;
                    break;
                case VTextLayout.align.Start:
                case VTextLayout.align.Block:
                    ycenter = maxHeight * 0.5f;
                    break;
                }
                switch (vtext.layout.Minor) {
                case VTextLayout.align.Base:
                    xcenter = -maxWidth * 0.5f;
                    break;
                case VTextLayout.align.Center:
                    xcenter = 0.0f;
                    break;
                case VTextLayout.align.End:
                    xcenter = -maxWidth * 0.5f;
                    break;
                case VTextLayout.align.Start:
                case VTextLayout.align.Block:
                    xcenter = 0.0f;
                    break;
                }
            }

            r.center = new Vector3(xcenter, ycenter, vtext.parameter.Depth * 0.5f);
            r.extents = new Vector3(maxWidth, maxHeight, vtext.parameter.Depth);
            return r;
        }

        /// <summary>
        /// Layout the text
        ///
        /// based on settings in VTextInterface
        /// </summary>
        /// <param name="vtext">Vtext.</param>
        /// <param name="str">String.</param>
        public void LayoutText3D(VTextInterface vtext, string str) {
            CreateText(vtext, str, true);
        }

        /// <summary>
        /// Creates the text
        ///
        /// based on settings in VTextInterface
        /// </summary>
        /// <param name="vtext">Vtext.</param>
        /// <param name="str">String.</param>
        public void CreateText3D(VTextInterface vtext, string str) {
            CreateText(vtext, str, false);
        }

        /// <summary>
        /// Creates the text
        ///
        /// based on settings in VTextInterface
        /// </summary>
        /// <param name="vtext">Vtext.</param>
        /// <param name="str">String.</param>
        protected void CreateText(VTextInterface vtext, string str, bool layoutOnly) {
            if (this == null)
                return;
            if (vtext == null)
                return;
            if (System.IntPtr.Zero == m_fontHandle) {
                return;
            }

            if (null == m_glyphs) {
                m_glyphs = new Hashtable();
            }
            // Debug.Log("Font info create text " + str);
            float xoffset = 0.0f;
            float yoffset = 0.0f;
            float ascender = GetAscender(m_fontHandle);
            // float descender = GetDescender(m_fontHandle);
            // Debug.Log(ascender + " descender " + descender);
            float xSpacing = ascender * vtext.layout.Spacing;
            float ySpacing = 0.0f;
            // Debug.Log("*** yoff " + yoffset + " " + ySpacing);

            string[] sa = str.Split('\n');
            //Debug.Log(sa.Length + " lines in text " + str);
            Vector2 glyphShift = new Vector2(-glyphMinX, -glyphMinY);
            Vector2 maxGlyphSize = new Vector2(glyphMaxX - glyphMinX, glyphMaxY - glyphMinY);
            // calculate linesizes and total
            Vector2[] linesizes = new Vector2[sa.Length];
            float maxWidth = 0.0f;
            float maxHeight = 0.0f;

            Vector2 kern = new Vector2(0f, 0f);
            Vector3 locpos = new Vector3(0f, 0f, 0f);
            Vector3 locScale = new Vector3(vtext.layout.Size, vtext.layout.Size, vtext.layout.Size);
            // calc bounds
            for (int k = 0; k < sa.Length; k++) {
                linesizes[k] = LineSize(vtext, sa[k]);
                if (sa[k].Length > 0) {
                    float h = linesizes[k].y * vtext.layout.Spacing;
                    if (h > ySpacing) {
                        ySpacing = h;
                        // Debug.Log(k + " *** ysize " + linesizes[k].y + " " + ySpacing);
                    }
                }
                if (vtext.layout.Horizontal) {
                    if (linesizes[k].x > maxWidth) {
                        maxWidth = linesizes[k].x;
                    }
                    maxHeight += linesizes[k].y;
                } else {
                    maxWidth += linesizes[k].x;
                    if (linesizes[k].y > maxHeight) {
                        maxHeight = linesizes[k].y;
                    }
                }
            }
            float startX = 0.0f;
            // Debug.Log("text size " + maxWidth + " " + maxHeight);
            // now create/layout meshes line by line
            int childIndex = 0;
            for (int k = 0; k < sa.Length; k++) {
                if (vtext.layout.Horizontal) {
                    switch (vtext.layout.Major) {
                    case VTextLayout.align.Base:
                        xoffset = 0.0f;
                        break;
                    case VTextLayout.align.Center:
                        xoffset = (-linesizes[k].x * 0.5f);
                        startX = -maxWidth * 0.5f;
                        break;
                    case VTextLayout.align.End:
                        xoffset = -linesizes[k].x;
                        startX = -maxWidth;
                        break;
                    case VTextLayout.align.Start:
                    case VTextLayout.align.Block:
                        xoffset = 0.0f;
                        break;
                    }
                    switch (vtext.layout.Minor) {
                    case VTextLayout.align.Base:
                        yoffset = 0.0f;
                        break;
                    case VTextLayout.align.Center:
                        yoffset = (maxHeight * 0.5f - ascender);
                        break;
                    case VTextLayout.align.End:
                        yoffset = (maxHeight - ascender);
                        break;
                    case VTextLayout.align.Start:
                    case VTextLayout.align.Block:
                        yoffset = -ascender * vtext.layout.Size;
                        break;
                    }
                    yoffset -= k * ySpacing;
                    // Debug.Log(k +  " yoff " + yoffset + " " + ySpacing);
                } else { // vertical
                    switch (vtext.layout.Major) {
                    case VTextLayout.align.Base:
                        yoffset = 0.0f;
                        break;
                    case VTextLayout.align.Center:
                        yoffset = linesizes[k].y * 0.5f;
                        break;
                    case VTextLayout.align.End:
                        Debug.Log(string.Format("Vertical layout end: lineSize: {0} ascender: {1}", linesizes[k].y, ascender));
                        yoffset = linesizes[k].y - ascender;
                        break;
                    case VTextLayout.align.Start:
                    case VTextLayout.align.Block:
                        yoffset = 0.0f;
                        break;
                    }
                    switch (vtext.layout.Minor) {
                    case VTextLayout.align.Base:
                        xoffset = 0.0f;
                        break;
                    case VTextLayout.align.Center:
                        xoffset = -maxWidth * 0.5f;
                        break;
                    case VTextLayout.align.End:
                        xoffset = -maxWidth;
                        break;
                    case VTextLayout.align.Start:
                    case VTextLayout.align.Block:
                        xoffset = 0.0f;
                        break;
                    }
                    xoffset += k * xSpacing;
                    // Debug.Log(k +  " xoff " + xoffset + " " + xSpacing);
                }

                char c;
                char prev = '\0';
                float adjustX = 0.0f;

                for (int j = 0; j < sa[k].Length; j++) {
                    c = sa[k][j];
                    if (c >= ' ') {
                        if (m_glyphs.ContainsKey(c)) {
                            VGlyphInfo gi = (VGlyphInfo)m_glyphs[c];
                            if (!vtext.layout.Horizontal) {
                                // adjust x to center of glyph
                                adjustX = (((xSpacing - gi.size.x) * 0.5f) - gi.horizontalBearing.x) * vtext.layout.Size;
                                // Debug.Log(gi.horizontalBearing + " adjust " + adjustX + " sz " + gi.size.x);
                            }
                            if ('\0' != prev) {
                                if (vtext.layout.Horizontal) {
                                    // only horizontal kerning adjust!
                                    kern = Kerning(prev, c);
                                }
                            }
                            if (c > ' ') {
                                // no GameObject for space
                                string gname = ("G" + k + "_" + j + "_" + c);
                                GameObject go = null;
                                if (layoutOnly) {
                                    go = vtext.transform.GetChild(childIndex).gameObject;
                                    if (go.name != gname) {
                                        Debug.LogWarning(go.name + " != " + gname);
                                    }
                                    childIndex++;
                                } else {
                                    go = new GameObject(gname);
                                }
                                locpos.x = xoffset + adjustX + kern.x;
                                float normX = (maxWidth > 0f) ? (locpos.x - startX) / maxWidth : 0f;
                                //Debug.Log("normX: " + normX + " startX " + startX + " locX " + locpos.x + " maxw " + maxWidth);
                                float y0 = vtext.layout.CurveXY.Evaluate(normX);
                                locpos.y = yoffset + kern.y + y0;
                                float z0 = vtext.layout.CurveXZ.Evaluate(normX);
                                locpos.z = z0;
                                Quaternion orient = new Quaternion(0f, 0f, 0f, 1f);
                                const float txDelta = 0.01f;
                                float x1 = normX + txDelta;
                                if (maxWidth > 0.0f) {
                                    if (vtext.layout.OrientationXY) {
                                        float y1 = vtext.layout.CurveXY.Evaluate(x1);
                                        float dY = (y1 - y0) / maxWidth;
                                        float rotZ = Mathf.Atan2(dY, txDelta);
                                        orient *= Quaternion.AngleAxis(Mathf.Rad2Deg * rotZ, new Vector3(0f, 0f, 1f));
                                    }
                                    if (vtext.layout.OrientationXZ) {
                                        float z1 = vtext.layout.CurveXZ.Evaluate(x1);
                                        float dZ = (z1 - z0) / maxWidth;
                                        float rotY = Mathf.Atan2(dZ, txDelta);
                                        orient *= Quaternion.AngleAxis(Mathf.Rad2Deg * rotY, new Vector3(0f, -1f, 0f));
                                    }
                                    if (vtext.layout.OrientationCircular) {
                                        normX *= maxWidth;
                                        normX += gi.advance.x * vtext.layout.Size * 0.5f;
                                        normX = normX / maxWidth;

                                        float angle = Mathf.Lerp(-vtext.layout.StartRadius, -vtext.layout.EndRadius, normX);
                                        Quaternion rot = Quaternion.AngleAxis(angle, new Vector3(0f, 1f, 0f));

                                        locpos.x = 0.0f;
                                        float fac = vtext.layout.CircleRadius + vtext.parameter.Depth * vtext.layout.Size;
                                        if (vtext.layout.AnimateRadius) {
                                            fac *= vtext.layout.CurveRadius.Evaluate(normX);
                                        }
                                        locpos += (rot * new Vector3(0.0f, 0.0f, -1.0f)) * fac;
                                        locpos += (rot * new Vector3(-gi.advance.x * vtext.layout.Size * 0.5f, 0.0f, 0.0f));
                                        orient *= rot;
                                    }
                                }
                                if (!layoutOnly) {
                                    go.transform.parent = vtext.transform;
                                }
                                go.transform.localPosition = locpos;
                                go.transform.localRotation = orient;
                                go.transform.localScale = locScale;

                                go.layer = vtext.gameObject.layer;
                                go.isStatic = vtext.gameObject.isStatic;

                                if (layoutOnly) {
                                    MeshRenderer mr = go.GetComponent<MeshRenderer>();
                                    mr.shadowCastingMode = vtext.parameter.ShadowCastMode;
                                    mr.receiveShadows = vtext.parameter.ReceiveShadows;
                                    mr.useLightProbes = vtext.parameter.UseLightProbes;
                                } else {
                                    Mesh mesh = gi.GetMesh(glyphShift, maxGlyphSize, vtext, GetAscender(m_fontHandle));

                                    if (null != mesh) {
                                        go.AddComponent(typeof(MeshFilter));
                                        go.AddComponent(typeof(MeshRenderer));
                                        MeshFilter mf = go.GetComponent<MeshFilter>();
                                        mf.sharedMesh = mesh;
                                        MeshRenderer mr = go.GetComponent<MeshRenderer>();

                                        System.Array.Copy(vtext.materials, vtext.usedMaterials, mesh.subMeshCount);
                                        if ((vtext.parameter.Depth - Mathf.Epsilon) < 0.0f) {
                                            vtext.usedMaterials[1] = null;
                                        }
                                        if ((vtext.parameter.Bevel - Mathf.Epsilon) < 0.0f) {
                                            vtext.usedMaterials[2] = null;
                                        }

                                        mr.sharedMaterials = vtext.usedMaterials.Take(mesh.subMeshCount).ToArray();    // get only as many materials as we have submeshes
                                        mr.shadowCastingMode = vtext.parameter.ShadowCastMode;
                                        mr.receiveShadows = vtext.parameter.ReceiveShadows;
                                        mr.useLightProbes = vtext.parameter.UseLightProbes;
                                    } else {
                                        Debug.LogWarning(gname + " no mesh");
                                    }
                                }
                            }
                            prev = c;
                            float spaceOffset = 0f;
                            if (vtext.layout.Horizontal) {
                                if (vtext.layout.Major == VTextLayout.align.Block) {
                                    if (c == ' ') {
                                        int spaceCount = sa[k].Count(char.IsWhiteSpace);
                                        if (linesizes[k].x < maxWidth) {
                                            spaceOffset = (maxWidth - linesizes[k].x) / spaceCount;
                                        } 
                                    }
                                }
                                xoffset += gi.advance.x * vtext.layout.Size + vtext.layout.GlyphSpacing + spaceOffset;
                            } else {
                                if (vtext.layout.Major == VTextLayout.align.Block) {
                                    if (c == ' ') {
                                        int spaceCount = sa[k].Count(char.IsWhiteSpace);
                                        if (linesizes[k].y < maxHeight) {
                                            spaceOffset = (maxHeight - linesizes[k].y) / spaceCount;
                                        } 
                                    }
                                }

                                yoffset -= gi.advance.y * vtext.layout.Size + vtext.layout.GlyphSpacing + spaceOffset;
                                adjustX = 0.0f;
                            } 
                        } else {
                            Debug.LogWarning("\"" + c + "\" not in glyphs");
                        }
                    }
                }
            }
        }
    }
}
