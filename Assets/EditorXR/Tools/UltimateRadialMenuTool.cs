using UnityEditor.Experimental.EditorVR.Proxies;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine;
using UnityEngine.InputNew;

namespace UnityEditor.Experimental.EditorVR.Tools
{
    [MainMenuItem("UltimateRadialMenu", "Create", "Enter UltimateRadialMenu mode")]
    [SpatialMenuItem("UltimateRadialMenu", "Tools", "Enter UltimateRadialMenu mode")]
    sealed class UltimateRadialMenuTool : MonoBehaviour, ITool, IStandardActionMap, IConnectInterfaces, IInstantiateMenuUI,
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

        public void Start()
        {

        }

        public void ProcessInput(ActionMapInput input, ConsumeControlDelegate consumeControl)
        {
            GameObject.FindGameObjectWithTag("EDITORXR UltimateRadialMenu").SendMessage("NotifyActive");
        }
    }
}
