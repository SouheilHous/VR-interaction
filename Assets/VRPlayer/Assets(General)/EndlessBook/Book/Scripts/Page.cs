namespace echo17.EndlessBook
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Class to control a page object that animates a turn
    /// </summary>
    public class Page : MonoBehaviour
    {
        /// <summary>
        /// Array of clip lengths by animation
        /// </summary>
        private float[] animationClipLengths;

        /// <summary>
        /// Page material names
        /// </summary>
        private const string PageFrontMaterialName = "PageFront";
        private const string PageBackMaterialName = "PageBack";

        /// <summary>
        /// Page controller hashes for faster updates
        /// </summary>
        private int AnimationSpeedHash = Animator.StringToHash("AnimationSpeed");
        private int AnimationDirectionForwardHash = Animator.StringToHash(TurnDirectionEnum.TurnForward.ToString());
        private int AnimationDirectionBackwardHash = Animator.StringToHash(TurnDirectionEnum.TurnBackward.ToString());

        /// <summary>
        /// The controller of the page
        /// </summary>
        private Animator controller;

        /// <summary>
        /// The renderer of the page
        /// </summary>
        private Renderer pageRenderer;

        /// <summary>
        /// The material index of the front page material
        /// </summary>
        private int pageFrontMaterialIndex;

        /// <summary>
        ///  The material index of the back page material
        /// </summary>
        private int pageBackMaterialIndex;

        /// <summary>
        /// Possible directions the page can turn
        /// </summary>
        public enum TurnDirectionEnum
        {
            TurnForward,
            TurnBackward
        }

        /// <summary>
        /// The handler to call when the page turn has completed
        /// </summary>
        public Action<Page> pageTurnCompleted;

        /// <summary>
        /// The index of the page. This is used by the book to
        /// recycle these pages and reuse them in sequence
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The front and back materials
        /// </summary>
        public Material PageFrontMaterial { get { return pageRenderer.sharedMaterials[pageFrontMaterialIndex]; } }
        public Material PageBackMaterial { get { return pageRenderer.sharedMaterials[pageBackMaterialIndex]; } }

        private void Awake()
        {
            // cache some components

            controller = GetComponent<Animator>();
            pageRenderer = GetComponentInChildren<Renderer>();

            // cache the animation clip lengths
            if (controller != null)
            {
                animationClipLengths = new float[System.Enum.GetNames(typeof(TurnDirectionEnum)).Length];

                var ac = controller.runtimeAnimatorController;
                for (var i = 0; i < ac.animationClips.Length; i++)
                {
                    var index = (int)(TurnDirectionEnum)System.Enum.Parse(typeof(TurnDirectionEnum), ac.animationClips[i].name);
                    animationClipLengths[index] = ac.animationClips[i].length;
                }
            }

            // index the materials used

            var materials = pageRenderer.sharedMaterials;
            for (var i = 0; i < materials.Length; i++)
            {
                if (materials[i].name == PageFrontMaterialName)
                {
                    pageFrontMaterialIndex = i;
                }
                if (materials[i].name == PageBackMaterialName)
                {
                    pageBackMaterialIndex = i;
                }
            }
        }

        /// <summary>
        /// Starts the page turn animation
        /// </summary>
        /// <param name="direction">The direction to turn</param>
        /// <param name="time">The time to play the animation</param>
        /// <param name="frontMaterial">The front of the page material</param>
        /// <param name="backMaterial">The back of the page material</param>
        public void Turn(TurnDirectionEnum direction, float time, Material frontMaterial, Material backMaterial)
        {
            // don't turn the page if the time is invalid
            if (time <= 0) return;

            // turn on the page
            gameObject.SetActive(true);

            // update the materials
            var materials = pageRenderer.sharedMaterials;
            materials[pageFrontMaterialIndex] = frontMaterial;
            materials[pageBackMaterialIndex] = backMaterial;
            pageRenderer.sharedMaterials = materials;

            // set the page turn animation speed: clip length / desired length = speed
            controller.SetFloat(AnimationSpeedHash, animationClipLengths[(int)direction] / time);

            // set the triggers to control which animation is playing
            controller.SetTrigger(direction == TurnDirectionEnum.TurnForward ? AnimationDirectionForwardHash : AnimationDirectionBackwardHash);
        }

        /// <summary>
        /// Called when the page animation has completed
        /// </summary>
        public void PageAnimationCompleted()
        {
            // turn off the page
            gameObject.SetActive(false);

            // call the completion handler if necessary
            if (pageTurnCompleted != null) pageTurnCompleted(this);
        }
    }
}