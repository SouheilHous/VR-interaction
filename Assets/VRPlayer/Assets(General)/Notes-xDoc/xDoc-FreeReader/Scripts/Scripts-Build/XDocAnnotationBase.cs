//  <copyright file="XDocAnnotationBase.cs" company="xox interactive">
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

namespace XDocBuild
{
    using UnityEngine;

    /// <summary>
    /// The XDocAnnotationBase Class provides _ALL_ the functionality for
    /// the XDocAnnotation Class - which is the main class to annotate GameObjects.
    /// However in builds all the annotation functionality is not needed anymore
    /// And thus this abstract class takes over duty, which has all functionality
    /// removed and deletes the annotation from the gameObject as soon as it is
    /// activated.
    /// The correct abstract base class for XDocAnnotation is selected by
    /// a pre-compiler #if UNITY_EDITOR statement.
    /// Moreover: By having all functionality in the base class, we can reprogramm it
    /// and distribute a new library. As the instance class is an editor script and
    /// unchanged, it will take the new functionality from the new lib, but all 
    /// gameObjects will keep their reference to the instanced class.
    /// </summary>
    public abstract class XDocAnnotationBase : MonoBehaviour
    {
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// As result the annotation is deleted from the gameObject.
        /// </summary>
        private void OnEnable()
        {
            Destroy(this);
        }
    }
}
