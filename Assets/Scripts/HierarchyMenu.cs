using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HierarchyMenu : MonoBehaviour
{
    public TextMeshProUGUI tmpPath;
    public HierarchyMenuItem[] menuItems;

    private Transform selectedTransform;
    private int childIndexOffset;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void UpdateUI()
    {
        int index = 0;

        //Update path name
        tmpPath.text = "/ ";
        if (selectedTransform != null)
        {
            string pathName = "";
            Transform t = selectedTransform;
            while (t != null)
            {
                pathName = t.name + " / " + pathName;
                t = t.parent;
            }
            tmpPath.text = "/ " + pathName;
     
            
            while (index < menuItems.Length && index + childIndexOffset < selectedTransform.childCount)
            {
                menuItems[index].SetTransform(selectedTransform.GetChild(index + childIndexOffset));
                menuItems[index].gameObject.SetActive(true);
                index++;
            }
        } else
        {
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            while (index < menuItems.Length && index + childIndexOffset < rootGameObjects.Length)
            {
                menuItems[index].SetTransform(rootGameObjects[index + childIndexOffset].transform);
                menuItems[index].gameObject.SetActive(true);
                index++;
            }
        }

        while (index < menuItems.Length)
        {
            menuItems[index].gameObject.SetActive(false);
            index++;
        }
    }

    public void SelectTransform(Transform t)
    {
        selectedTransform = t;
        childIndexOffset = 0;
        UpdateUI();
    }

    public void Back()
    {
        childIndexOffset = Mathf.Max(0, childIndexOffset - 8);
        UpdateUI();
    }

    public void Next()
    {
        if (selectedTransform != null)
        {
            childIndexOffset = Mathf.Min(childIndexOffset + 8, selectedTransform.childCount-1);
        } else
        {
            childIndexOffset = Mathf.Min(childIndexOffset + 8, SceneManager.GetActiveScene().GetRootGameObjects().Length - 1);
        }
        UpdateUI();
    }

    public void Up()
    {
        if (selectedTransform != null)
        {
            selectedTransform = selectedTransform.parent;
            UpdateUI();
        }
    }

    public void ToggleLock()
    {

    }



}
