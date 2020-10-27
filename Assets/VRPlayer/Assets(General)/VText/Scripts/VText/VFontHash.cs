//  $Id: VFontHash.cs 172 2015-03-13 14:05:02Z dirk $
//
// Virtence VFont package
// Copyright 2014 by Virtence GmbH
// http://www.virtence.com
//

using UnityEngine;
using System.Collections;

namespace Virtence {
	namespace VText {

		/// <summary>
		/// VFont hashtable singleton
		/// 
		/// holds all used fonts in one Hashtable
		/// </summary>
		public class VFontHash {
			static protected Hashtable fonts = null;

            static VFontHash() {
                Debug.Log("clear font hash");
            }

			static public VFontInfo GetFontInfo(string fontname) {
				if(null == fonts) {
					// Debug.Log("Setup Fonts");
					fonts = new Hashtable();
				}

				if(null != fonts) {
					if(fonts.ContainsKey(fontname)) {
						// Debug.Log("VFontHash have VFont " + fontname + " " + fonts.Count);
					} else {
						Debug.Log("VFontHash " + fonts.Count + " Fonts add " + fontname);
						fonts.Add(fontname, new VFontInfo(fontname));
					}
					return (VFontInfo)fonts[fontname];
				} else {
					Debug.LogError("No fonts hashtable");
				}
				return null;
			}
		}
	}
}