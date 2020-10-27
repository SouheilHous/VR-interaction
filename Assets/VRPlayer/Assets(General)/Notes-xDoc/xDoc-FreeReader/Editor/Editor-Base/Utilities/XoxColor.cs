//  <copyright file="XoxColor.cs" company="xox interactive">
// 
//  Copyright (c) 2017 xox interactive. All rights reserved.
//  www: http://xoxinteractive.com
//  email: contact@xoxinteractive.com
//
//  The License terms are defined by the 
//  Unity ASSET STORE TERMS OF SERVICE AND EULA:
//  https://unity3d.com/legal/as_terms
// 
//  </copyright>

namespace xDocEditorBase.Extensions
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This static class extends the Color and Color32 classes: it adds the 
    /// HexString method to the Color and the Color32 class.
    /// </summary>
    public static class XoxColor
    {
        /// <summary>
        /// Returns the RGB or RGBA string of the Color.
        /// </summary>
        /// <returns>The RGB or RGBA string including the # sign and with 2 digits
        /// per base color in HEX.</returns>
        /// <param name="aColor">The color.</param>
        /// <param name="includeAlpha">If set to <c>true</c> include alpha.</param>
        public static string HexString (
            this Color aColor, 
            bool includeAlpha = false
        )
        {
            return ((Color32)aColor).HexString (includeAlpha);
        }

        /// <summary>
        /// Returns the RGB or RGBA string of the Color.
        /// </summary>
        /// <returns>The RGB or RGBA string including the # sign and with 2 digits
        /// per base color in HEX.</returns>
        /// <param name="aColor">The color.</param>
        /// <param name="includeAlpha">If set to <c>true</c> include alpha.</param>
        public static string HexString (
            this Color32 aColor, 
            bool includeAlpha
        )
        {
            String rs = Convert.ToString (aColor.r, 16).ToUpper ();
            String gs = Convert.ToString (aColor.g, 16).ToUpper ();
            String bs = Convert.ToString (aColor.b, 16).ToUpper ();
            String a_s = Convert.ToString (aColor.a, 16).ToUpper ();
            while (rs.Length < 2)
                rs = "0" + rs;
            while (gs.Length < 2)
                gs = "0" + gs;
            while (bs.Length < 2)
                bs = "0" + bs;
            while (a_s.Length < 2)
                a_s = "0" + a_s;
            if (includeAlpha)
                return "#" + rs + gs + bs + a_s;
            return "#" + rs + gs + bs;
        }
    }
}

