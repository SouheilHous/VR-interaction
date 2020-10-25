using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MD_Plugin;

public class ModifiersEditorController : Editor
{

    public void ppAddMeshColliderRefresher(MonoBehaviour Sender)
    {
        if (Sender.gameObject.GetComponent<MD_MeshColliderRefresher>())
            return;
        Color c = Color.green;
        ColorUtility.TryParseHtmlString("#49de71", out c);
        GUI.color = c;
        GUILayout.Space(15);
            if (GUILayout.Button("Add Mesh Collider Refresher"))
            {
                GameObject gm = Sender.gameObject;
                gm.AddComponent<MD_MeshColliderRefresher>();
            }
    }
    public void ppBackToMeshEditor(MonoBehaviour Sender)
    {
        Color c = Color.black;
        ColorUtility.TryParseHtmlString("#f2d0d0", out c);
        GUI.color = c;
        GUILayout.Space(5);
        if (GUILayout.Button("Back To Mesh Editor"))
        {
            GameObject gm = Sender.gameObject;
            DestroyImmediate(Sender);
            gm.AddComponent<MD_MeshProEditor>();
        }
    }
}
