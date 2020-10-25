using UnityEditor.Experimental.EditorVR.Proxies;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine;
using UnityEngine.InputNew;
using System;
using System.Collections.Generic;

namespace UnityEditor.Experimental.EditorVR.Tools
{
    [MainMenuItem("VR Brush", "Create", "Enter VR Brush mode")]
    [SpatialMenuItem("VR Brush", "Tools", "Enter VR Brush mode")]
    sealed class _VRBrushTool : MonoBehaviour, ITool, IStandardActionMap, IConnectInterfaces, IInstantiateMenuUI,
        IUsesRayOrigin, IUsesSpatialHash, IUsesViewerScale, ISelectTool, IIsHoveringOverUI, IIsMainMenuVisible,
        IRayVisibilitySettings, IMenuIcon, IRequestFeedback, IUsesNode/*, IExclusiveMode*/
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
            GameObject goManager = GameObject.FindGameObjectWithTag("VRBrushManager");            
            goManager.SendMessage("NotifyActive");

            if (lstGameObjects == null)
            {
                lstGameObjects = new List<GameObject>();
            }

            foreach (LineRenderer lr in GameObject.FindGameObjectWithTag("VRBrushContent").GetComponentsInChildren<LineRenderer>())
            {
                if (!lstGameObjects.Contains(lr.gameObject))
                {
                    this.AddToSpatialHash(lr.gameObject);
                    lstGameObjects.Add(lr.gameObject);
                }
            }
        }

        
    }
}
