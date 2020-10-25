using UnityEditor.Experimental.EditorVR.Proxies;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine;
using UnityEngine.InputNew;
using System;
using System.Collections.Generic;

namespace UnityEditor.Experimental.EditorVR.Tools
{
    [MainMenuItem("Exclusive Mode", "Create", "Enter Exclusive Mode")]
    [SpatialMenuItem("Exclusive Mode", "Tools", "Enter Exclusive mode")]
    sealed class ExclusiveModeTool : MonoBehaviour, ITool, IStandardActionMap, IConnectInterfaces, IInstantiateMenuUI,
        IUsesRayOrigin, IUsesSpatialHash, IUsesViewerScale, ISelectTool, IIsHoveringOverUI, IIsMainMenuVisible,
        IRayVisibilitySettings, IMenuIcon, IRequestFeedback, IUsesNode, IExclusiveMode
    {
#pragma warning disable 649
        [SerializeField]
        CreatePrimitiveMenu m_MenuPrefab;

        [SerializeField]
        Sprite m_Icon;
#pragma warning restore 649

        const float k_DrawDistance = 0.075f;

        GameObject m_ToolMenu;
        bool m_Freeform;

        GameObject m_CurrentGameObject;

        public Transform rayOrigin { get; set; }
        public Node node { get; set; }

        public Sprite icon { get { return m_Icon; } }

        public ActionMap standardActionMap { private get; set; }

        private bool bIsActive;

        private List<GameObject> lstGameObjects;

        public void Start()
        {
            this.RemoveFromSpatialHash(GameObject.Find("Pen"));
        }

        public void ProcessInput(ActionMapInput input, ConsumeControlDelegate consumeControl)
        {
            GameObject goManager = GameObject.FindGameObjectWithTag("EDITORXR ExclusiveMode");
            goManager.SendMessage("NotifyActive");
        }        
    }
}
