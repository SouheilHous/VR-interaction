//  <copyright file="Annotation.cs" company="xox interactive">
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

using UnityEngine;

#if UNITY_EDITOR
using xDocBase;

#else
using XDocBuild;
#endif


namespace xDoc
{

	#if UNITY_EDITOR
	[HelpURL ("http://xoxinteractive.com/questions/category/xdoc/")]
	[AddComponentMenu (Annotation.menuPath, Annotation.menuPosition)]
	#endif
	public class Annotation : XDocAnnotationBase
	{
		#if UNITY_EDITOR
		public const int menuPosition = -999;
		public const string menuPath = "xDoc Annotation";
		#endif
	}
}
