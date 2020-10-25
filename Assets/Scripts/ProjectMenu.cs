using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ProjectMenu : MonoBehaviour
{
    public TextMeshProUGUI tmpPath;
    public ProjectMenuItem[] menuItems;

    private string rootPath;
    private string path;
    private int childIndexOffset;
    private List<string> items;

    // Start is called before the first frame update
    void Start()
    {
        rootPath = Application.dataPath;
        path = Application.dataPath;
        items = new List<string>();

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
        tmpPath.text = "\\ " + path.Replace(rootPath, "\\");

        items.Clear();
        items.AddRange(Directory.GetDirectories(path));
        int numberOfFolders = items.Count;

        items.AddRange(Directory.GetFiles(path));

        int i = 0;
        while (i < items.Count)
        {
            if (items[i].EndsWith(".meta"))
            {
                items.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        while (index < menuItems.Length && index + childIndexOffset < items.Count)
        {
            menuItems[index].SetPath(items[index + childIndexOffset], index + childIndexOffset < numberOfFolders);
            menuItems[index].gameObject.SetActive(true);
            index++;
        }

        while (index < menuItems.Length)
        {
            menuItems[index].gameObject.SetActive(false);
            index++;
        }
    }

    public void SelectFolder(string path)
    {
        this.path = path;
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
        childIndexOffset = Mathf.Min(childIndexOffset + 8, items.Count-1);
        UpdateUI();
    }

    public void Up()
    {
        if (path != rootPath)
        {
            path = path.Substring(0, path.LastIndexOf("\\"));
            UpdateUI();
        }
    }



}
