using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

public class MD_VerticeSelectorTool : EditorWindow {


    public MDM_MeshFit Sender;
    public List<GameObject> SelectedVertices = new List<GameObject>();

    void OnGUI()
    {
        if(GUILayout.Button("Assign selected vertices [" + Selection.gameObjects.Length+"]"))
        {
            SelectedVertices.Clear();
            if (Selection.activeGameObject != null && Selection.gameObjects.Length > 1)
            {
                foreach (GameObject gm in Selection.gameObjects)
                {
                    if (gm.transform.root == Sender.transform)
                        SelectedVertices.Add(gm);
                }

                Sender.ppMODIF_MeshFitter_SelectedVertexes = SelectedVertices.ToArray();
                Sender.MD_INTERNAL_RefreshVerticesActivation();
                Selection.activeObject = (Object)Sender.gameObject;

                this.Close();
            }
        }
    }
}
