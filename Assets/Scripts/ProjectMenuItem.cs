using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class ProjectMenuItem : MonoBehaviour
{
    public GameObject goButtonOpen;
    public GameObject goPrefabView;
    public Text tmpText;

    private string path;
    private bool isFolder;
    private GameObject prefab;

    public void Update()
    {
    }


    public void ClickOpen()
    {
        if (isFolder)
        {
            GetComponentInParent<ProjectMenu>().SelectFolder(path);
        }
    }

    public void ClickSelect()
    {
        if (!isFolder)
        {
            if (prefab.GetComponent<LODGroup>())
            {
                prefab.GetComponent<LODGroup>().enabled = true;
            }

            string pathName = GetPathName();
            Debug.Log("Loading " + pathName);
            UnityEngine.Object prefabObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pathName);
            //Selection.activeGameObject = t.gameObject;

            UnityEngine.Object instantiatedObject = Instantiate(prefabObject);
            Selection.activeObject = instantiatedObject;            
        } else
        {
            ClickOpen();
        }
    }

    public void UpdatePath()
    {
        SetPath(path, isFolder);
    }

    public void SetPath(string path, bool isFolder)
    {
        this.path = path;
        this.isFolder = isFolder;
        goButtonOpen.SetActive(isFolder);
        tmpText.text = path.Substring(path.LastIndexOf("\\"));

        if (prefab != null && prefab.transform.parent == goPrefabView.transform)
        {
            Destroy(prefab);
        }

        if (!isFolder)
        {
            try
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetPathName());
                prefab = (GameObject)Instantiate(obj);

                prefab.transform.SetParent(goPrefabView.transform);
                if (prefab.GetComponent<LODGroup>())
                {
                    prefab.GetComponent<LODGroup>().enabled = false;
                }

                Renderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>();
                Bounds bounds = renderers[0].bounds;
                foreach (Renderer r in renderers)
                {
                    bounds.Encapsulate(r.bounds);
                }

                float maxSize = Mathf.Max(new float[] { bounds.size.x, bounds.size.y, bounds.size.z });

                prefab.transform.localRotation = Quaternion.identity;
                prefab.transform.localScale = Vector3.one / maxSize;
                prefab.transform.localPosition = Vector3.zero - Vector3.up * Mathf.Clamp(bounds.center.y, -maxSize, maxSize) / maxSize;
            } catch (System.Exception e)
            {
                Debug.LogError(e.Message + e.StackTrace);

            }
        }
        goPrefabView.SetActive(goPrefabView.transform.childCount > 0);
    }

    public string GetPathName()
    {
        return path.Replace(Application.dataPath, "Assets").Replace("\\\\", "/").Replace("\\", "/");
    }
}
