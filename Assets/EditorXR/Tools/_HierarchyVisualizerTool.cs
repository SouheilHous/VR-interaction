using UnityEditor.Experimental.EditorVR.Proxies;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine;
using UnityEngine.InputNew;

namespace UnityEditor.Experimental.EditorVR.Tools
{
    [MainMenuItem("Hierarchy Visualizer", "Create", "Open Hierarchy Visualizer")]
    [SpatialMenuItem("Hierarchy Visualizer", "Tools", "Open Hierarchy Visualizer")]
    sealed class _HierarchyVisualizerTool : MonoBehaviour, ITool, IStandardActionMap, IConnectInterfaces, IInstantiateMenuUI,
        IUsesRayOrigin, IUsesSpatialHash, IUsesViewerScale, ISelectTool, IIsHoveringOverUI, IIsMainMenuVisible,
        IRayVisibilitySettings, IMenuIcon, IRequestFeedback, IUsesNode
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

        public void ProcessInput(ActionMapInput input, ConsumeControlDelegate consumeControl)
        {
            GameObject.FindGameObjectWithTag("EDITORXR HierarchyVisualizer").SendMessage("NotifyActive");
        }
    }
}
