// FREQ ** create/copy from a predefined AnnotationType in a "database"
// FREQ ** do not override existing pref settings file / scriptable object
// _FREQ - google search in all game objects

namespace xDocEditorBase.Windows
{
    using UnityEditor;
    using UnityEngine;
    using xDocBase.AssetManagement;
    using xDocEditorBase.UI;

    /// <summary>
    /// Besides the Annotation Component (attached to GameObjects and Prefabs) the
    /// XDoc window is the main functionality of xDoc: Here one can access the
    /// - Search 
    /// - Bulk Operations
    /// - Configure Annotation Types
    /// - Configure general xDoc settings and
    /// - access the help.
    /// </summary>
    public class XDocWindow : XoxEditorGUI.EditorWindowX
    {
        public const string xDocWindowTitle = "xDoc";

        /// <summary>
        /// Shows / opens the xDoc window.
        /// </summary>
        [MenuItem (AssetManager.menuPathWindow + xDocWindowTitle)]
        public static void ShowWindow ()
        {
            var currentWindow = EditorWindow.GetWindow (typeof(XDocWindow), false, xDocWindowTitle) as XDocWindow;
            currentWindow.ApplyWindowsDecorations ();
        }

        /// <summary>
        /// Applies the windows decorations: Text and Icon on the window tab.
        /// </summary>
        protected void ApplyWindowsDecorations ()
        {
            if (AssetManager.isFunctional) {
                titleContent = new GUIContent (xDocWindowTitle, AssetManager.settings.xDocLogo);
            } else {
                titleContent = new GUIContent (xDocWindowTitle);
            }
        }

        /// <summary>
        /// This method is called by Unity when the xDoc window gets enabled. Within the Unity framework
        /// this is our initialization function / constructor.
        /// </summary>
        new void OnEnable ()
        {

            try {

                // Make sure the window has the correct decorations
                base.OnEnable ();
                ApplyWindowsDecorations ();

                // Make sure the AssetManager is up and running OK
                if (!AssetManager.isFunctional) {
                    // most probably asset resources couldnt be loaded due to missing / lost script assignments
                    return;
                }

                if (AssetManager.canWrite) {
                    // xDoc full License
                    // Add all tabs
                    AddContent (new XDocWindowSearchTab (this));
                    AddContent (new XDocWindowBulkOperationsTab (this));
                    AddContent (new XDocWindowAnnotationTypesTab (this));
                    AddContent (new XDocWindowSettingsTab (this));
                    AddContent (new XDocWindowHelpTab (this));				
                    // Add the Utility Button to add an annotation.
                    SetUtilityButton (new GUIContent ("+"), AddAnnotationToSelectedObject);
                } else {
                    // free reader
                    AddContent (new XDocWindowSearchTab (this));
                    AddContent (new XDocWindowHelpTab (this));
                }

                Repaint ();
            } catch {
                Debug.LogError (
                    "Fatal xDoc Error: xDoc can't open the xDoc window! " +
                    "Re-throwing exception.",
                    this);
                DestroyImmediate (this);
                throw;
            }

        }

        /// <summary>
        /// This method is used by the '+' (Add Annotation) Utility Button 
        /// as callback to call the function, which actually adds an annotation
        /// to the currently selected Object.
        /// </summary>
        static void AddAnnotationToSelectedObject ()
        {
            // old implementation
            // EditorApplication.ExecuteMenuItem("Component/xDoc Annotation");
            AssetManager.createXDocAnnotationCallback ();
        }
    }
}
