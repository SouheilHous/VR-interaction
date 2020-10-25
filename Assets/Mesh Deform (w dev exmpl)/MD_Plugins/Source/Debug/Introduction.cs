using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Introduction : MonoBehaviour {

    //------------------------------------------------------
    //------------------------------------------------------
    //------------------------------------------------------
    //--------------------INTRODUCTION SCRIPT---------------
    //------------------------------------------------------
    //------------------------------------------------------
    //------------------------------------------------------
    //------------------------------------------------------

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


#if UNITY_EDITOR
    void Awake()
    {
        GenerateScenesToBuild();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public static void GenerateScenesToBuild()
	{
        try
        {
            UnityEditor.EditorBuildSettings.scenes = new UnityEditor.EditorBuildSettingsScene[0];
            string[] tempPaths = Directory.GetFiles(Application.dataPath + "/MD_FullPackage/Examples/Scenes/","*.unity");
            List<UnityEditor.EditorBuildSettingsScene> sceneAr = new List<UnityEditor.EditorBuildSettingsScene>();

            for (int i = 0; i < tempPaths.Length; i++)
            {
                string path = tempPaths[i].Substring(Application.dataPath.Length - "Assets".Length);
                path = path.Replace('\\', '/');

                sceneAr.Add(new UnityEditor.EditorBuildSettingsScene(path, true));
            }
           UnityEditor.EditorBuildSettings.scenes = sceneAr.ToArray();
        }
        catch(IOException e)
        {
            Debug.Log("UNABLE TO LOAD EXAMPLE SCENES! Exception: "+e.Message);
        }

	}
    public static string GetScenePath(string sceneName)
    {
        try
        {
            if (File.Exists(Application.dataPath + "/MD_FullPackage/Examples/Scenes/" + sceneName + ".unity"))
                return Application.dataPath + "/MD_FullPackage/Examples/Scenes/" + sceneName + ".unity";
            else
                return "";
        }
        catch (IOException e)
        {
            Debug.Log("UNABLE TO LOAD EXAMPLE SCENES! Go to Examples/Scenes/Scene_INTRO. Exception: " + e.Message);
        }
        return "";
    }
#endif


    public void _LoadLevel(string LVL_Name)
	{
        if(SceneManager.sceneCountInBuildSettings>1)
            SceneManager.LoadScene(LVL_Name);
        else
            Debug.Log("Can't load level. Please press stop and then press play again to refresh Build Settings.");
    }

    public void OpenUrl1()
    {
        Application.OpenURL("https://matejvanco.com");
    }
    public void OpenUrl2()
    {
        Application.OpenURL("https://matejvanco.com/unity-plugins/");
    }
    public void OpenDocumentation()
    {
        Application.OpenURL("https://docs.google.com/presentation/d/13Utk_hVY304c7QoQPSVG7nHXV5W5RjXzgZIhsKFvUDE/edit?usp=sharing");
    }
}