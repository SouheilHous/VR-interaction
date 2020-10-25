using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
namespace MD_Plugin
{
#if UNITY_EDITOR
    public class MD_StartUp : EditorWindow
    {
        public Texture2D Logo;

        public Texture2D Home;
        public Texture2D Web;
        public Texture2D Doc;

        bool donotshowAgain = true;

        public Font f;
        private GUIStyle style;
        [MenuItem("Window/MD_Plugin/StartUp Window")]
        public static void Init()
        {
            MD_StartUp md = (MD_StartUp)EditorWindow.GetWindow(typeof(MD_StartUp));
            md.maxSize = new UnityEngine.Vector2(400, 500);
            md.minSize = new UnityEngine.Vector2(399, 498);
            md.Show();
        }
         
        public static void InitStartup()
        {
            if (PlayerPrefs.GetString("MD_Plugin") == "True")
                return;

            MD_StartUp md = (MD_StartUp)EditorWindow.GetWindow(typeof(MD_StartUp));
            md.maxSize = new UnityEngine.Vector2(400, 500);
            md.minSize = new UnityEngine.Vector2(399, 499);
            md.Show();
        }
         
        private void OnDestroy()
        {
            PlayerPrefs.SetString("MD_Plugin", donotshowAgain.ToString());
        }

        private void OnGUI()
        {
            style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.font = f;
            style.wordWrap = false;

            GUILayout.Label(Logo);
            style.fontSize = 30;
            GUI.Label(new Rect(this.maxSize.x-70, Logo.height/2-15, 30, 30), "V"+MD_Debug.VERSION, style);
            style.fontSize = 13;
            style.wordWrap = true;

            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Welcome user! Welcome to the Mesh Deformation [MD] Full Collection! Check news below...", style);
            GUILayout.EndVertical();

            style.alignment = TextAnchor.UpperLeft;
            GUILayout.Space(5);
            style.fontSize = 12;

           GUILayout.BeginVertical("Box");
            GUILayout.Label("<size=14><color=#ffa84a>MD Package version "+MD_Debug.VERSION+"  ["+ MD_Debug.DATE.Day+"/"+MD_Debug.DATE.Month+"/"+MD_Debug.DATE.Year+ " <size=8>dd/mm/yyyy</size>]</color></size>\n- Added Tunnel Creator system\n- Added FFD & simple deformer modifier\n- Added new example scenes\n- Updated Documentation\n- Updated API\n- Updated Mesh Editor Component\n- Package cleanup[Unity 2018 and +]", style);
            GUILayout.Space(20);
            GUILayout.Label("No idea where to start? Open Documentation to get more!",style);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            style = new GUIStyle(GUI.skin.button);
            style.imagePosition = ImagePosition.ImageAbove;

            GUILayout.BeginHorizontal("Box");
            if(GUILayout.Button(new GUIContent("Take Me Home",Home), style))
            {
                Introduction.GenerateScenesToBuild();
                string scene = Introduction.GetScenePath("Scene_INTRO");
                if (!string.IsNullOrEmpty(scene))
                    EditorSceneManager.OpenScene(scene);
                else
                    Debug.LogError("Scene is not in Build Settings! Required path: ["+ Application.dataPath + "/MD_FullPackage/Examples/Scenes/]");
            }
            if (GUILayout.Button(new GUIContent("Creator's Webpage", Web),style))
            {
                Application.OpenURL("https://matejvanco.com");
            }
            if (GUILayout.Button(new GUIContent("Open Documentation", Doc), style))
            {
                try
                {
                    System.Diagnostics.Process.Start(Application.dataPath + "/MD_FullPackage/ReadMe-Documentation.pdf");
                }
                catch
                {
                    Debug.LogError("Documentation could not be found!");
                }
            }

            GUILayout.EndHorizontal();
        }
    }
#endif
    public class MD_Debug
    {
        public const string ORGANISATION = "Matej Vanco";
        public const short VERSION = 13;
        //--Y/M/D
        public static System.DateTime DATE = new System.DateTime(2020, 3, 12);

        public enum DebugType { Error, Warning, Information };
        public static void Debug(UnityEngine.MonoBehaviour Sender, string Message, DebugType DType = DebugType.Information)
        {
            string senderName = "MD Plugin ";
            if (Sender != null)
                senderName = Sender.GetType().Name;
            if (DType == DebugType.Information)
                UnityEngine.Debug.Log(senderName + ": " + Message);
            else if (DType == DebugType.Warning)
                UnityEngine.Debug.LogWarning(senderName + ": " + Message);
            else if (DType == DebugType.Error)
                UnityEngine.Debug.LogError(senderName + ": " + Message);
        }
    }
}