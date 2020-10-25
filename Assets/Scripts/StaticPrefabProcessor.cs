using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class StaticPrefabProcessor : MonoBehaviour
{
    private static List<string> prefabFiles;
    private static int convertedPrefabsCount;
    private static bool doNotPrompt;

    [MenuItem("Prefabs/Convert all to non-static")]
    public static void ConvertToNonStatic()
    {
        Debug.Log("Starting prefab conversion...");

        try
        {
            //Get all prefab files
            prefabFiles = new List<string>();
            FindAllPrefabFiles(Application.dataPath);
            Debug.Log("Found " + prefabFiles.Count + " prefabs.");

            //Update prefabs
            convertedPrefabsCount = 0;
            doNotPrompt = false;
            foreach (string prefab in prefabFiles)
            {
                ConvertPrefabToNonStatic(prefab);
            }

            //Reimport modified prefabs
            AssetDatabase.Refresh();

        } catch (System.Exception e)
        {
            Debug.LogError("Failed to conver prefabs: "+e.Message);
        }

        Debug.Log("Successfully converted "+convertedPrefabsCount+" prefabs to non-static.");
    }

    private static void FindAllPrefabFiles(string path)
    {
        string[] files = Directory.GetFiles(path);
        string[] folders = Directory.GetDirectories(path);

        foreach (string file in files)
        {
            if (file.EndsWith(".prefab"))
            {
                prefabFiles.Add(file);
            }
        }

        foreach (string folder in folders)
        {
            FindAllPrefabFiles(folder);
        }
    }

    private static void ConvertPrefabToNonStatic(string path)
    {
        try
        {
            string[] prefab = File.ReadAllLines(path);
            bool bModifiedPrefab = false;

            for (int i=0; i<prefab.Length; i++) {
                if (prefab[i].Contains("m_StaticEditorFlags:") && !prefab[i].Contains("m_StaticEditorFlags: 0"))
                {
                    prefab[i] = prefab[i].Substring(0, prefab[i].IndexOf(":"));
                    prefab[i] += ": 0";
                    bModifiedPrefab = true;
                }
            }

            if (bModifiedPrefab) {

                int result = 0;
                if (!doNotPrompt)
                {
                    result = EditorUtility.DisplayDialogComplex("Confirmation required", "Convert prefab " + path + " to non-static?", "Yes", "No", "Yes to all");
                    if (result == 2)
                    {
                        doNotPrompt = true;
                        result = 0;
                    }
                }

                if (result == 0)
                {
                    File.WriteAllLines(path, prefab);
                    convertedPrefabsCount++;
                    Debug.Log("Successfully converted prefab: " + path);
                } else
                {
                    Debug.Log("Did not convert prefab: " + path);
                }
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("Errors when converting prefab: " + path + " " + e.Message);
        }
    }
}
