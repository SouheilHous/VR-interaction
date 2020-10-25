using UnityEditor.Experimental.EditorVR.Proxies;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine;
using UnityEngine.InputNew;

namespace UnityEditor.Experimental.EditorVR.Tools
{
    [MainMenuItem("Project Visualizer", "Create", "Open Project Visualizer")]
    [SpatialMenuItem("Project Visualizer", "Tools", "Open Project Visualizer")]
    sealed class _ProjectVisualizerTool : MonoBehaviour, ITool, IStandardActionMap, IConnectInterfaces, IInstantiateMenuUI,
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
            GameObject goProjectVisualizer = GameObject.FindGameObjectWithTag("EDITORXR ProjectVisualizer");

            goProjectVisualizer.SendMessage("NotifyActive");

            if (goProjectVisualizer.transform.childCount > 0)
            {
                Transform t = goProjectVisualizer.transform.GetChild(0);
                t.SetParent(null);

                //Remove children from Spatial Hash
                if (t.GetComponent<LODGroup>() != null)
                {
                    t.gameObject.AddComponent<MeshRenderer>().enabled = false;
                    t.gameObject.AddComponent<MeshFilter>().sharedMesh = t.GetComponent<LODGroup>().GetLODs()[0].renderers[0].GetComponent<MeshFilter>().sharedMesh;
                    this.AddToSpatialHash(t.gameObject);

                    foreach (Transform tc in t.GetComponentsInChildren<Transform>())
                    {
                        if (tc != t)
                        {
                            this.RemoveFromSpatialHash(tc.gameObject);
                        }
                    }
                } else
                {
                    this.AddToSpatialHash(t.gameObject);
                }
            }
        }
    }
}
