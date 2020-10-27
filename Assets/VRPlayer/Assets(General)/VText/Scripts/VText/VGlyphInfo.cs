/*
 * $Id: VGlyphInfo.cs 217 2015-04-28 13:11:20Z dirk $
 * 
 * Virtence VFont package
 * Copyright 2014 .. 2015 by Virtence GmbH
 * http://www.virtence.com
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Virtence {
    namespace VText {
        /// <summary>
        /// VGlyph info
        /// 
        /// contains Mesh and aditional info of specified glyph.
        /// </summary>
        public class VGlyphInfo {
            /// <summary>
            /// The size of the glyph.
            /// </summary>
            public Vector2 size;
            /// <summary>
            /// The distance to next glyph to advance to.
            /// Note: most of the fonts have zero advance.y!
            /// So if you layout vertical we use spacing instead.
            /// </summary>
            public Vector2 advance;
            public Vector2 horizontalBearing;
            public Vector2 verticalBearing;

            protected char _id;
            protected Mesh _mesh;
            private System.IntPtr _fh;
            private int _numContours;
            private Vector3[][] _contours;

            [StructLayout(LayoutKind.Sequential)]
            public struct GlyphInfo {
                public float sizeX;
                public float sizeY;
                public float advanceX;
                public float advanceY;
                public float horizBearingX;
                public float horizBearingY;
                public float vertBearingX;
                public float vertBearingY;
                public int numContours;
            };

            protected class BaseAttributes {
                public BaseAttributes(Vector3 v, Vector3 bv, Vector3 n, float dist) {
                    _v = new Vector3(v.x, v.y, v.z);
                    _bv = new Vector3(bv.x, bv.y, bv.z);
                    _n = new Vector3(n.x, n.y, n.z);
                    _uv = new Vector2(0.0f, dist);
                }

                public Vector3 _v;
                public Vector3 _bv;
                public Vector3 _n;
                public Vector2 _uv;
            }

            #if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_XBOX360)
			[DllImport ("__Internal")]
			private static extern bool GetGlyphInfo([Out] System.IntPtr gi, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport ("__Internal")]
			private static extern int GetGlyphVertices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport ("__Internal")]
			private static extern int GetGlyphTriangleIndices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);
			[DllImport ("__Internal")]
			private static extern void ClearGlyphData([In] System.IntPtr fontHandle, [In] uint id);
			[DllImport ("__Internal")]
			private static extern int GetGlyphContour(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id, [In] int index, ref bool odd, ref bool reverse);

#else
            [DllImport("VText")]
            private static extern bool GetGlyphInfo([Out] System.IntPtr gi, [In] System.IntPtr fontHandle, [In] uint id);

            [DllImport("VText")]
            private static extern int GetGlyphVertices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);

            [DllImport("VText")]
            private static extern int GetGlyphTriangleIndices(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id);

            [DllImport("VText")]
            private static extern void ClearGlyphData([In] System.IntPtr fontHandle, [In] uint id);

            [DllImport("VText")]
            private static extern int GetGlyphContour(ref System.IntPtr buffer, [In] System.IntPtr fontHandle, [In] uint id, [In] int index, ref bool odd, ref bool reverse);
            #endif
            private VGlyphInfo() {
            }

            ~VGlyphInfo( ) {
                _mesh = null;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VGlyphInfo"/> class.
            /// </summary>
            /// <param name="fontHandle">Font handle.</param>
            /// <param name="id">Glyph ddentifier.</param>
            public VGlyphInfo(System.IntPtr fontHandle, char id) {
                _id = id;
                _fh = fontHandle;
                if (System.IntPtr.Zero != _fh) {
                    // Debug.Log("fonthandle " + _fh);
                    GlyphInfo gi = new GlyphInfo();
                    int rawsize = Marshal.SizeOf(gi);
                    // Debug.Log("c: " + _id + " sz: " + rawsize);
                    System.IntPtr ip = Marshal.AllocHGlobal(rawsize);
                    if (GetGlyphInfo(ip, _fh, _id)) {
                        gi = (GlyphInfo)Marshal.PtrToStructure(ip, typeof(GlyphInfo));
                        // Debug.Log(" size " + gi.sizeX + " " + gi.sizeY);
                        size = new Vector2(gi.sizeX, gi.sizeY);
                        // Debug.Log(" advance " + gi.advanceX + " " + gi.advanceY);
                        advance = new Vector2(gi.advanceX, gi.advanceY);
                        horizontalBearing = new Vector2(gi.horizBearingX, gi.horizBearingY);
                        verticalBearing = new Vector2(gi.vertBearingX, gi.vertBearingY);
                        // Debug.Log(gi.numContours + " contours \'" + id + "\'");
                        _numContours = gi.numContours;
                    }
                    Marshal.FreeHGlobal(ip);
                }
            }

            protected int[] sideIndices = null;
            protected int[] bevelIndices = null;

            protected Vector3 zVector = new Vector3(0.0f, 0.0f, 1.0f);

            protected bool fetchNext(ref Vector3 v, Vector3[] contour, int startIndex) {
                Vector3 act = contour[startIndex];
                for (int k = startIndex + 1; k < contour.Length; k++) {
                    if (!act.Equals(contour[k])) {
                        // Debug.Log(startIndex + " -> " + k + " " + contour[k].ToString("F5"));
                        v = contour[k];
                        return true;
                    } else {
                        // Debug.Log(startIndex + " " + k + " " + contour[k].ToString("F5"));
                    }
                }
                // Debug.Log(startIndex + " restart zero");
                // if no successor restart from beginning
                for (int k = 0; k < startIndex; k++) {
                    if (!act.Equals(contour[k])) {
                        // Debug.Log(startIndex + " R-> " + " " + contour[k].ToString("F5"));
                        v = contour[k];
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Creates the sides and or bevel mesh of glyph mesh.
            /// 
            /// in submesh.
            /// </summary>
            /// <param name="p">VTextParameter</param>
            protected void CreateSides(VTextParameter p) {
                float bevel = p.Bevel;
                sideIndices = null;
                bevelIndices = null;
                if (null != _contours) {
                    // side contours
                    List< List<BaseAttributes> > aa = new List< List<BaseAttributes> >();
                    float crease = Mathf.Cos(p.Crease * Mathf.PI / 180.0f);
                    float maxContourLength = 0.0f;

                    Vector3 prev;
                    Vector3 act;
                    Vector3 next;
                    Vector3 first;

                    Vector3 d1;
                    Vector3 d2;

                    Vector3 n1;
                    Vector3 n2;
                    Vector3 nd1;
                    Vector3 nd2;
                    Vector3 bv1 = new Vector3(); // act bevel vertex
                    Vector3 bv2 = new Vector3(); // next bevel vertex
                    Vector3 nnv = new Vector3();

                    Vector3 ndn;
                    Vector3 nnn;
                    Vector3 n;
                    Vector3 an;

                    for (int k = 0; k < _contours.Length; k++) {
                        if (null != _contours[k]) {
                            // Debug.Log("---- _contours[" + k + "] " +_contours[k].Length + " -----------");
                            if (_contours[k].Length > 1) {                                
                                int numContourVertices = _contours[k].Length;
                                if (numContourVertices > 2) {
                                    List<BaseAttributes> attribs = new List<BaseAttributes>();
                                    prev = zVector;
                                    act = _contours[k][0];
                                    first = _contours[k][0];
                                    for (int j = numContourVertices - 1; j > 0; j--) {
                                        if (!act.Equals(_contours[k][j])) {
                                            prev = _contours[k][j];
                                            // Debug.Log("--- found prev at [" + j + "] " +_contours[k].Length + " ----");
                                            break;
                                        }
                                    }
                                    // Debug.Log("contours[" + k + "] prev " + prev);
                                    d1 = (prev - act);
                                    nd1 = Vector3.Normalize(d1);
                                    n1 = Vector3.Normalize(Vector3.Cross(nd1, zVector));

                                    float uvv = 0.0f;
                                    float dot = 0.0f;

                                    bool flat = true;
                                    bool prevSmooth = false;
                                    for (int j = 1; j < numContourVertices; j++) {
                                        next = _contours[k][j];
                                        if (next.Equals(act)) {
                                            // Debug.Log(k + " " + j + " ***** SKIP " + act.ToString("F5") + " **** " + next.ToString("F5"));
                                        } else {
                                            // Debug.Log(j + "] act " + (act.ToString("F5")) + " next " + (next.ToString("F5")));
                                            d2 = (act - next);
                                            nd2 = Vector3.Normalize(d2);
                                            n2 = Vector3.Normalize(Vector3.Cross(nd2, zVector));
                                            // Debug.Log(j + " n2 " + n2);
                                            dot = Vector3.Dot(n1, n2);
                                            // average normal at vertex j-1
                                            n = Vector3.Normalize(n1 + n2);
                                            // act beveled vertex
                                            bv1.x = act.x + n.x * bevel;
                                            bv1.y = act.y + n.y * bevel;
                                            bv1.z = bevel;

                                            // next beveled vertex
                                            bv2.x = next.x + n2.x * bevel;
                                            bv2.y = next.y + n2.y * bevel;
                                            bv2.z = bevel;
                                            // Debug.Log(j + " act " + act + " bv " + bv1 + " / " + bv2 +  " n " + n2);
                                            // Debug.Log("dot " + dot + " n " + n1 + " / " + n2 +  " crease " + crease);
                                            flat = (dot > crease) ? false : true;
                                            if (flat) {
                                                if (prevSmooth) {
                                                    // Debug.Log("S[" + k + "][" + j + "] " + dot + " act " + act + " uvv: " + uvv + " l " + attribs.Count );
                                                    attribs.Add(new BaseAttributes(act, bv1, n1, uvv));
                                                    prevSmooth = false;
                                                }
                                                // use face normal
                                                attribs.Add(new BaseAttributes(act, bv1, n2, uvv));

                                                uvv += d2.magnitude;
                                                if (bevel > 0.0f) {
                                                    if (fetchNext(ref nnv, _contours[k], j)) {
                                                        // Debug.Log(act + " next " + next + " nnv " + nnv);
                                                        ndn = Vector3.Normalize(next - nnv);
                                                        nnn = Vector3.Normalize(Vector3.Cross(ndn, zVector));
                                                        an = Vector3.Normalize(n2 + nnn);
                                                        bv2.x = next.x + an.x * bevel;
                                                        bv2.y = next.y + an.y * bevel;
                                                        bv2.z = bevel;
                                                        // Debug.Log(j + " next " + next.ToString("F5") + " bv " + bv2.ToString("F5") + " n2 " + n2.ToString("F5") +  " nn " + nn.ToString("F5") + " an " + an.ToString("F5"));
                                                    } else {
                                                        Debug.LogWarning("fetch next failed");
                                                    }
                                                    attribs.Add(new BaseAttributes(next, bv2, n2, uvv));
                                                } else {
                                                    attribs.Add(new BaseAttributes(next, next, n2, uvv));
                                                }
                                                d1 = d2;
                                                nd1 = nd2;
                                                n1 = n2;
                                            } else {
                                                // smooth
                                                attribs.Add(new BaseAttributes(act, bv1, n, uvv));

                                                uvv += d2.magnitude;
//												if (bevel > 0.0f) {
													attribs.Add(new BaseAttributes(next, bv2, n2, uvv));
//												}

                                                prevSmooth = true;
                                                if (j < numContourVertices - 1) {
                                                    d1 = d2;
                                                    nd1 = nd2;
                                                    n1 = n2;
                                                }
                                            }
                                        }
                                        act = next;
                                    }
                                    // close last edge
                                    {
                                        int ridx = numContourVertices - 1;
                                        act = _contours[k][ridx];
                                        while (act.Equals(first)) {
                                            ridx--;
                                            act = _contours[k][ridx];
                                            //Debug.Log(k + " Last act **** " + act.ToString("F5") + " **** " + first.ToString("F5"));
                                        }
                                    }
                                    for (int j = 0; j < numContourVertices; j++) {
                                        next = _contours[k][j];
                                        if (act.Equals(next)) {
                                            // skip if p[n-1] == p[0]
                                            //Debug.Log(k + " " + j + " Last == ***** " + act.ToString("F5") + " **** " + next.ToString("F5"));
                                        } else {
                                            // Debug.Log(k + " " + j + " Last != ***** " + act.ToString("F5") + " **** " + next.ToString("F5"));
                                            d2 = (act - next);
                                            nd2 = Vector3.Normalize(d2);
                                            n2 = Vector3.Normalize(Vector3.Cross(nd2, zVector));

                                            dot = Vector3.Dot(n1, n2);
                                            // average normal at vertex j-1
                                            n = Vector3.Normalize(n1 + n2);
                                            // act beveled vertex
                                            bv1.x = act.x + n.x * bevel;
                                            bv1.y = act.y + n.y * bevel;
                                            bv1.z = bevel;

                                            // next beveled vertex
                                            bv2.x = next.x;
                                            bv2.y = next.y;
                                            bv2.z = bevel;

                                            if (bevel > 0.0f) {
                                                /*
                                                Debug.Log(k + " " + j + " prev " + (prev.ToString("F5")) + " act " + (act.ToString("F5")) +
                                                          " next " + (next.ToString("F5")) + " nnv " + (nnv.ToString("F5")));
                                                          */
                                                if (fetchNext(ref nnv, _contours[k], j)) {
                                                    /*
                                                    Debug.Log(k + " " + j + " prev " + (prev.ToString("F5")) + " act " + (act.ToString("F5")) +
                                                              " next " + (next.ToString("F5")) + " nnv " + (nnv.ToString("F5")) + " first " + (first.ToString("F5")));
                                                              */
                                                    ndn = Vector3.Normalize(next - nnv);
                                                    nnn = Vector3.Normalize(Vector3.Cross(ndn, zVector));
                                                    an = Vector3.Normalize(n2 + nnn);
                                                    bv2.x = next.x + an.x * bevel;
                                                    bv2.y = next.y + an.y * bevel;
                                                    // Debug.Log(j + " bv2 " + bv2 + " n1 " + n1 + " n2 " + n2 +  " nnn " + nnn + " an " + an);
                                                } else {
                                                    Debug.LogWarning("fetch next failed");
                                                }
                                            }

                                            flat = (dot > crease) ? false : true;
                                            // Debug.Log("dot " + dot + " n " + n1 + " / " + n2 +  " crease " + crease);
                                            if (flat) {
                                                // use face normal
                                                if (prevSmooth) {
                                                    // Debug.Log("PSL[" + k + "][" + j + "] " + dot + " act " + act + " uvv: " + uvv + " l " + attribs.Count );
                                                    attribs.Add(new BaseAttributes(act, bv1, n1, uvv));
                                                } else {
                                                    n = Vector3.Normalize(n1 + n2);
                                                    // act beveled vertex
                                                    bv1.x = act.x + n.x * bevel;
                                                    bv1.y = act.y + n.y * bevel;
                                                    bv1.z = bevel;
                                                    /*
                                                    Debug.Log(k + " " + j + " prev " + (prev.ToString("F5")) + " act " + (act.ToString("F5")) +
                                                              " next " + (next.ToString("F5")) + " bv1 " + (bv1.ToString("F5")) + " n1 " + (n1.ToString("F5")) + " n2 " + (n2.ToString("F5")));
                                                              */
                                                    attribs.Add(new BaseAttributes(act, bv1, n2, uvv));
                                                }
                                                // Debug.Log("FL[" + k + "][" + j + "] act " + act + " next " + next + " first " + first );
                                                // attribs.Add(new BaseAttributes(act, bv2, n2, uvv));
                                                uvv += d2.magnitude;
                                                if (next.Equals(first)) {
                                                    // Debug.Log("NEF[" + k + "][" + j + "] " + dot + " act " + act + " uvv: " + uvv + " l " + attribs.Count);
                                                    attribs.Add(new BaseAttributes(next, bv2, n2, uvv));
                                                }
                                            } else {                                                
                                                uvv += d2.magnitude;
                                                attribs.Add(new BaseAttributes(next, bv2, n, uvv));
                                            }
                                            // Debug.Log(k + " " + j + " Last *** " + act + " *** " + next);
                                            // we reached last valid edge
                                            break;
                                        }
                                        act = next;
                                    }
                                    if (uvv > maxContourLength) {
                                        maxContourLength = uvv;
                                    }
                                    aa.Add(attribs);
                                }
                            }
                        }
                    }
                    if (aa.Count > 0) {
                        int totalVertices = _mesh.vertices.Length;
                        int numSideIndices = 0;

                        int numSideVerticesFactor = (p.Depth > 0.0f) ? 2 : 0;
                        numSideVerticesFactor += (bevel > 0.0f) ? (p.Backface ? 4 : 2) : 0;

                        Vector3 vact;
                        for (int j = 0; j < aa.Count; j++) {
                            List<BaseAttributes> al = aa[j];
                            totalVertices += al.Count * numSideVerticesFactor;
                            if (al.Count > 0) {
                                prev = al[0]._bv;
                                // count required edges
                                int numEdges = 0;
                                for (int i = 1; i < al.Count; i++) {
                                    vact = al[i]._bv;
                                    if (!vact.Equals(prev)) {
                                        numEdges++;
                                    }
                                    prev = vact;
                                }
                                // Debug.Log("numEdges: " + numEdges + " vs " + (al.Count-1));
                                // we require numEdges*6 indices for each edge(two triangles)
                                numSideIndices += numEdges * 6;
                            }
                        }
                        // Debug.Log("Expand \'" + _id + "\' " + aa.Count + " (" + _mesh.vertices.Length + ") " + totalVertices);
                        Vector3[] nv = new Vector3[totalVertices];
                        Vector3[] nn = new Vector3[totalVertices];
                        Vector2[] nt = new Vector2[totalVertices];
                        Vector4[] tt = new Vector4[totalVertices]; // tangents

                        int k = 0;
                        // first copy already tesselated attributes
                        Vector3[] mv = _mesh.vertices;
                        Vector3[] mn = _mesh.normals;
                        Vector2[] muv = _mesh.uv;
                        for (; k < mv.Length; k++) {
                            nv[k] = mv[k];
                            nn[k] = mn[k];
                            nt[k] = muv[k];
                        }
                        Vector4[] mt = _mesh.tangents;
                        if (null != mt) {
                            // Debug.Log("copy " + mt.Length + " tangents " + mv.Length);
                            for (int tk = 0; tk < mt.Length; tk++) {
                                tt[tk] = mt[tk];
                            }
                        }
                        int aiv = mv.Length;
                        Vector3 hcp;
                        Vector3 znAxis = new Vector3(0f, 0f, -1f);
                        Vector4 ht = new Vector4(0f, 0f, 1f, 1f);

                        float b2 = bevel * bevel;
                        float uBevelSize = (bevel > 0f) ? Mathf.Sqrt(b2 + b2) : 0.0f;
                        float uTotalSize = uBevelSize + p.Depth;
                        const float sideU = 0.1f;
                        float buvw = sideU * uBevelSize / uTotalSize;
                        float duvw = sideU - (p.Backface ? 2.0f * buvw : buvw);
                        bool conformant = true;
                        if (p.Depth > 0.0f) {
                            // fill side vertices
                            float uOffset = 0.5f;
                            float z2 = bevel + p.Depth;
                            for (int j = 0; j < aa.Count; j++) {
                                List<BaseAttributes> al = aa[j];
                                if (al.Count > 0) {
                                    BaseAttributes ba;
                                    for (int i = 0; i < al.Count; i++) {
                                        ba = al[i];
                                        /*
										hcp = Vector3.Cross(ba._n, znAxis);
										ht.x = hcp.x;
										ht.y = hcp.y;
										ht.z = hcp.z;
										*/
                                        nv[k] = ba._bv;
                                        nn[k] = ba._n;
                                        nt[k] = conformant ? new Vector2(uOffset + buvw, ba._uv.y / maxContourLength) : ba._uv;
                                        tt[k] = ht;
                                        k++;
                                        nv[k] = new Vector3(ba._bv.x, ba._bv.y, z2);
                                        nn[k] = ba._n;
                                        nt[k] = conformant ? new Vector2(uOffset + buvw + duvw, ba._uv.y / maxContourLength) : new Vector2(p.Depth, ba._uv.y);
                                        tt[k] = ht;
                                        k++;
                                    }
                                }
                                uOffset += sideU;
                            }
                            // fill side indices
                            sideIndices = new int[numSideIndices];
                            int ai = 0;
                            for (int j = 0; j < aa.Count; j++) {
                                List<BaseAttributes> al = aa[j];
                                if (al.Count > 0) {
                                    prev = al[0]._bv;
                                    int edge = 0;
                                    for (int i = 1; i < al.Count; i++) {
                                        if (al[i]._bv.Equals(prev)) {
                                            aiv += 2;
                                        } else {
                                            sideIndices[ai] = aiv;
                                            sideIndices[ai++] = aiv;
                                            sideIndices[ai++] = aiv + 1;
                                            sideIndices[ai++] = aiv + 2;
										
                                            sideIndices[ai++] = aiv + 2;
                                            sideIndices[ai++] = aiv + 1;
                                            sideIndices[ai++] = aiv + 3;
                                            aiv += 2;
                                            prev = al[i]._bv;
                                            edge++;
                                            // Debug.Log("side edge: " + edge + " max " + (aiv+3) + " alc " + nv.Length);
                                        }
                                    }
                                    aiv += 2;
                                }
                            }
                        }

                        if (bevel > 0.0f) {
                            // Debug.Log("Bevel " + bevel);
                            // fill bevel vertex attributes
                            float uOffset = 0.5f;
                            for (int j = 0; j < aa.Count; j++) {
                                List<BaseAttributes> al = aa[j];
                                if (al.Count > 0) {
                                    BaseAttributes ba;
                                    for (int i = 0; i < al.Count; i++) {
                                        ba = al[i];
                                        /*
										hcp = Vector3.Cross(ba._n, znAxis);
										ht.x = hcp.x;
										ht.y = hcp.y;
										ht.z = hcp.z;
										*/
                                        nv[k] = ba._v;
                                        nn[k] = new Vector3(0f, 0f, -1f);
                                        nt[k] = conformant ? new Vector2(uOffset, ba._uv.y / maxContourLength) : ba._uv;
                                        tt[k] = ht;
                                        k++;
                                        nv[k] = new Vector3(ba._bv.x, ba._bv.y, bevel);
                                        nn[k] = ba._n;
                                        nt[k] = conformant ? new Vector2(uOffset + buvw, ba._uv.y / maxContourLength) : ba._uv;
                                        tt[k] = ht;
                                        k++;
                                    }
                                }
                                uOffset += sideU;
                            }
                            if (p.Backface) {
                                // backface bevel vertex attributes
                                uOffset = 0.5f;
                                float z1 = bevel + p.Depth;
                                float z2 = bevel * 2f + p.Depth;
                                for (int j = 0; j < aa.Count; j++) {
                                    List<BaseAttributes> al = aa[j];
                                    if (al.Count > 0) {
                                        BaseAttributes ba;
                                        for (int i = 0; i < al.Count; i++) {
                                            ba = al[i];
                                            hcp = Vector3.Cross(ba._n, znAxis);
                                            ht.x = hcp.x;
                                            ht.y = hcp.y;
                                            ht.z = hcp.z;
											
                                            nv[k] = new Vector3(ba._bv.x, ba._bv.y, z1);
                                            nn[k] = ba._n;
                                            nt[k] = conformant ? new Vector2(uOffset + buvw + duvw, ba._uv.y / maxContourLength) : ba._uv;
                                            tt[k] = ht;
                                            k++;
                                            nv[k] = new Vector3(ba._v.x, ba._v.y, z2);
                                            nn[k] = new Vector3(0f, 0f, 1f);
                                            nt[k] = conformant ? new Vector2(uOffset + buvw + duvw + buvw, ba._uv.y / maxContourLength) : ba._uv;
                                            tt[k] = ht;
                                            k++;
                                        }
                                    }
                                    uOffset += sideU;
                                }
                            }

                            // fill bevel indices
                            bevelIndices = new int[p.Backface ? numSideIndices * 2 : numSideIndices];
                            int ai = 0;
                            for (int j = 0; j < aa.Count; j++) {
                                List<BaseAttributes> al = aa[j];
                                if (al.Count > 0) {
                                    prev = al[0]._bv;
                                    int edge = 0;
                                    for (int i = 1; i < al.Count; i++) {
                                        if (al[i]._bv.Equals(prev)) {
                                            aiv += 2;
                                        } else {
                                            bevelIndices[ai] = aiv;
                                            bevelIndices[ai++] = aiv;
                                            bevelIndices[ai++] = aiv + 1;
                                            bevelIndices[ai++] = aiv + 2;
											
                                            bevelIndices[ai++] = aiv + 2;
                                            bevelIndices[ai++] = aiv + 1;
                                            bevelIndices[ai++] = aiv + 3;
                                            aiv += 2;
                                            prev = al[i]._bv;
                                            edge++;
                                            // Debug.Log("bevel edge: " + edge + " max " + (aiv+3) + " alc " + nv.Length);
                                        }
                                    }
                                    aiv += 2;
                                }
                            }
                            if (p.Backface) {
                                for (int j = 0; j < aa.Count; j++) {
                                    List<BaseAttributes> al = aa[j];
                                    if (al.Count > 0) {
                                        prev = al[0]._bv;
                                        int edge = 0;
                                        for (int i = 1; i < al.Count; i++) {
                                            if (al[i]._bv.Equals(prev)) {
                                                aiv += 2;
                                            } else {
                                                bevelIndices[ai] = aiv;
                                                bevelIndices[ai++] = aiv;
                                                bevelIndices[ai++] = aiv + 1;
                                                bevelIndices[ai++] = aiv + 2;
												
                                                bevelIndices[ai++] = aiv + 2;
                                                bevelIndices[ai++] = aiv + 1;
                                                bevelIndices[ai++] = aiv + 3;
                                                aiv += 2;
                                                prev = al[i]._bv;
                                                edge++;
                                                // Debug.Log("bevel edge: " + edge + " max " + (aiv+3) + " alc " + nv.Length);
                                            }
                                        }
                                        aiv += 2;
                                    }
                                }
                            }
                        }
                        // Debug.Log(nv.Length + " " + nt.Length);
                        _mesh.vertices = nv;
                        _mesh.uv = nt;
                        _mesh.normals = nn;
                        if (p.GenerateTangents) {
                            _mesh.tangents = tt;
                        } else {
                            _mesh.tangents = null;
                        }
                    }
                } else {
                    Debug.LogWarning("no contours defined");
                }
            }

            /// <summary>
            /// realize the mesh.
            /// </summary>
            /// <returns>The mesh.</returns>
            /// <param name="vtext">Vtext.</param>
            public Mesh GetMesh(Vector2 shift, Vector2 size, VTextInterface vtext, float ascender) {
                //Debug.Log("c: " + _id);
                if (null == _mesh) {
                    _mesh = new Mesh();

                    _mesh.name = "c_" + _id;
                    int subMeshCount = 1;
                    if ((vtext.parameter.Depth - Mathf.Epsilon) > 0.0f) {
                        subMeshCount++;
                    }
                    if ((vtext.parameter.Bevel - Mathf.Epsilon) > 0.0f) {
                        subMeshCount++;
                    }
                    _mesh.subMeshCount = subMeshCount;

                    if (System.IntPtr.Zero != _fh) {
                        System.IntPtr buffer = System.IntPtr.Zero;
                        int vsize = GetGlyphVertices(ref buffer, _fh, _id);
                        //Debug.Log(vsize + " **** glyph vertices **** " + _id);
                        if (vsize > 0) {
                            float[] res = new float[vsize * 2];
                            // fetch xy float array
                            Marshal.Copy(buffer, res, 0, vsize * 2);

                            System.IntPtr ibuffer = System.IntPtr.Zero;
                            // fetch indices
                            int isize = GetGlyphTriangleIndices(ref ibuffer, _fh, _id);
                            // Debug.Log(isize + " **** glyph indices **** " + _id);
                            if (isize > 0) {
                                int tisize = vtext.parameter.Backface ? isize * 2 : isize;
                                int tvsize = vtext.parameter.Backface ? vsize * 2 : vsize;

                                int[] ri = new int[isize];
                                Marshal.Copy(ibuffer, ri, 0, isize);

                                int[] indices = new int[tisize];
                                if (vtext.parameter.Backface) {
                                    for (int k = 0; k < isize - 2; k += 3) {
                                        // front face change ccw to cw
                                        // back face ccw is cw
                                        int idx = ri[k + 2];
                                        indices[k + 0] = idx; // front
                                        indices[isize + k + 2] = idx + vsize; // back

                                        idx = ri[k + 1];
                                        indices[k + 1] = idx;
                                        indices[isize + k + 1] = idx + vsize; // back

                                        idx = ri[k + 0];
                                        indices[k + 2] = idx;
                                        indices[isize + k + 0] = idx + vsize; // back

                                    }
                                } else {
                                    // only front faces
                                    // change ccw to cw
                                    for (int k = 0; k < isize - 2; k += 3) {
                                        int idx = ri[k + 2];
                                        indices[k] = idx;
										
                                        idx = ri[k + 1];
                                        indices[k + 1] = idx;
										
                                        idx = ri[k + 0];
                                        indices[k + 2] = idx;
                                        // Debug.Log(k + "] " + indices[k] + " " + indices[k+1] + " " + indices[k+2]);
                                    }
                                }
                                Vector3[] tvertices = new Vector3[tvsize];
                                Vector2[] tuv = new Vector2[tvsize];
                                Vector3[] tnormals = new Vector3[tvsize];
                                Vector4[] ttangents = new Vector4[tvsize];

                                Vector3 faceNormal = new Vector3(0f, 0f, -1f);
                                Vector4 faceTangent = new Vector4(1f, 0f, 0f, 1f);
                                // copy front vertex attributes
                                for (int k = 0; k < vsize; k++) {
                                    float x = res[k * 2];
                                    float y = res[k * 2 + 1];
                                    
                                    tvertices[k] = new Vector3(x, y, 0f);
                                    // Debug.Log(k + "] " + x + " " + y + " : " + tvertices[k].ToString("0.00000000"));
                                    tuv[k] = new Vector2(0.5f * (x + shift.x) / size.x, 0.5f * (y + shift.y) / size.y);
                                    tnormals[k] = faceNormal;
                                    ttangents[k] = faceTangent;
                                }
                                if (vtext.parameter.Backface) {
                                    // create backface vertex attributes
                                    faceNormal = new Vector3(0f, 0f, 1f);
                                    for (int k = 0; k < vsize; k++) {
                                        float x = res[k * 2];
                                        float y = res[k * 2 + 1];
                                        int offset = vsize + k;
                                        tvertices[offset] = new Vector3(x, y, vtext.parameter.Depth + vtext.parameter.Bevel * 2f);
                                        tuv[offset] = new Vector2(0.5f * (x + shift.x) / size.x, 0.5f + 0.5f * (y + shift.y) / size.y);
                                        tnormals[offset] = faceNormal;
                                        ttangents[offset] = faceTangent;
                                    }
                                }
                                _mesh.vertices = tvertices;
                                _mesh.uv = tuv;
                                _mesh.normals = tnormals;
                                if (vtext.parameter.GenerateTangents) {
                                    _mesh.tangents = ttangents;
                                } else {
                                    _mesh.tangents = null;
                                }
                                if (_numContours > 0) {
                                    if (vtext.Is3D()) {
                                        // Debug.Log(_numContours + " contours -> depth " + vtext.parameter.Depth + " bevel  " + vtext.parameter.Bevel);
                                        bool odd = false;
                                        bool reverse = false;

                                        _contours = new Vector3[_numContours][];
                                        for (int j = 0; j < _numContours; j++) {
                                            System.IntPtr cbuf = System.IntPtr.Zero;
                                            int csize = GetGlyphContour(ref cbuf, _fh, _id, j, ref odd, ref reverse);
                                            // Debug.Log(_numContours + " contour[" + j + "] " + csize);
                                            if (csize > 0) {
                                                // Debug.Log(csize + " gc[" + j + "] " + (reverse ? "reverse" : "fwd") + " winding " + (odd ? "even_odd" : "nonzero"));
                                                _contours[j] = new Vector3[csize];
                                                float[] cvec = new float[csize * 3];
                                                Marshal.Copy(cbuf, cvec, 0, csize * 3);
                                                if (reverse) {
                                                    for (int z = 0; z < csize; z++) {
                                                        int offset = z * 3;
                                                        // Debug.Log(cvec[offset] + " " + cvec[offset+1] + " " + cvec[offset+2]);
                                                        _contours[j][csize - z - 1] = new Vector3(cvec[offset], cvec[offset + 1], cvec[offset + 2]);
                                                    }
                                                } else {
                                                    for (int z = 0; z < csize; z++) {
                                                        int offset = z * 3;
                                                        // Debug.Log(cvec[offset] + " " + cvec[offset+1] + " " + cvec[offset+2]);
                                                        _contours[j][z] = new Vector3(cvec[offset], cvec[offset + 1], cvec[offset + 2]);
                                                    }
                                                }
                                            }
                                        }
                                        CreateSides(vtext.parameter);
                                        _mesh.SetTriangles(indices, 0);
                                        if (null != sideIndices) {
                                            _mesh.SetIndices(sideIndices, MeshTopology.Triangles, 1);
                                        }
                                        if (null != bevelIndices) {
                                            // Debug.Log("bevel ind");
                                            _mesh.SetIndices(bevelIndices, MeshTopology.Triangles, 2);
                                        }
                                    } else {
                                        _mesh.SetTriangles(indices, 0);
                                    }
                                    _mesh.RecalculateBounds();
                                }
                                ClearGlyphData(_fh, _id);
                                return _mesh;
                            }
                        }
                        ClearGlyphData(_fh, _id);
                        // Debug.Log("no glyph " + size);
                    } else {
                        // Debug.Log("ZERO fonthandle " + _fh);
                    }
                }
                return _mesh;
            }
        }
    }
}