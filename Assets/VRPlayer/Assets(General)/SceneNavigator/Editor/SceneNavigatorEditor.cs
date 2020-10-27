using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SceneNavigator.Editor
{

    public class SceneNavigatorEditor : EditorWindow
    {

        [MenuItem("Window/Scene Navigator")]
        static void Init()
        {
            SceneNavigatorEditor window = (SceneNavigatorEditor)EditorWindow.GetWindow(typeof(SceneNavigatorEditor));
            window.autoRepaintOnSceneChange = true;
        }
            
        private int customScene;
        private int customSceneTemp;
        private string currentScenePath;
        private string[] scenesArr;
        private string[] scenesPathArr;
        private Dictionary<int, string[]> sceneList = new Dictionary<int, string[]>();

        private Vector2 scrollPosition = Vector2.zero;
        private string tags;
        private string customFlagName;
        private string descrFlag;
        private Flags flags;
        private Flag selectedFlag;
        private Flag lastSelectedFlag;
        private const string dbName = "Flags.asset";
        private static float iconSize = 100f;
        private bool toChange;
        private bool drawGizmo = true;
        private bool sort = true;

        private Texture2D logo;
        private Texture2D border_tex;
        private Texture2D border_top_tex;
        private Texture2D button_add_tex;
        private Texture2D button_remove_tex;
        private Texture2D button_change_tex;
        private Texture2D button_clear_tex;
        private Texture2D button_gizmo_tex;
        private Texture2D button_tag_tex;
        private Texture2D button_sort_tex;
        private Texture2D button_action_tex;
        private Texture2D button_open_tex;

        private IOrderedEnumerable<KeyValuePair<string, Flag>> ioe;

        private string pathToAsset
        {
            get
            {
                MonoScript ms = MonoScript.FromScriptableObject(this);
                return AssetDatabase.GetAssetPath(ms).Replace("Editor/" + ms.name + ".cs", "");
            }
        }

        private void OnEnable()
        {
            
            if(PlayerPrefs.HasKey("FlagsSize")) 
            {
                iconSize = PlayerPrefs.GetFloat("FlagsSize");
            }

            if(PlayerPrefs.HasKey("FlagsSort")) 
            {
                sort = PlayerPrefs.GetInt("FlagsSort") != 0;
            }

            logo = (Texture2D)Resources.Load("Sprites/logo", typeof(Texture2D));
            border_tex = Resources.Load<Texture2D>("Sprites/border");
            border_top_tex = Resources.Load<Texture2D>("Sprites/border-top");
            button_add_tex = Resources.Load<Texture2D>("Sprites/add");
            button_remove_tex = Resources.Load<Texture2D>("Sprites/remove");
            button_change_tex = Resources.Load<Texture2D>("Sprites/change");
            button_clear_tex = Resources.Load<Texture2D>("Sprites/clear");
            button_gizmo_tex = Resources.Load<Texture2D>("Sprites/gizmo");
            button_tag_tex = Resources.Load<Texture2D>("Sprites/tag");
            button_sort_tex = Resources.Load<Texture2D>("Sprites/sort");
            button_action_tex = Resources.Load<Texture2D>("Sprites/action");
            button_open_tex = Resources.Load<Texture2D>("Sprites/open");

            initScenes();

        }

        private void initScenes()
        {
            tryLoadAsset();

            sceneList = new Dictionary<int, string[]>(); // string[name, path]

            customScene = -1;
            customSceneTemp = -1;
            currentScenePath = EditorSceneManager.GetActiveScene().path;

            int cnt = 0;
            foreach(string key in flags.list.Keys)
            {
                Flag flagScript = flags.list[key];

                string[] val = new string[2];
                val[0] = flagScript.sceneName;
                val[1] = flagScript.scene;

                if(!sceneList.ContainsValue(val))
                {
                    sceneList.Add(cnt, val);
                    cnt++;
                }
            }

            scenesArr = new string[sceneList.Count];
            scenesPathArr = new string[sceneList.Count];

            foreach(int key in sceneList.Keys)
            {
                if(currentScenePath == sceneList[key][1]) customScene = customSceneTemp = key;
                scenesArr[key] = sceneList[key][0];
                scenesPathArr[key] = sceneList[key][1];
            }

            ToggleSort(true);
            ToggleGizmos(true);

        }
        
        private void tryLoadAsset(bool beforeSave = false)
        {
            Flags flagsTemp = AssetDatabase.LoadAssetAtPath(Path.Combine(pathToAsset, dbName), typeof(Flags)) as Flags;
            if(flagsTemp == null)
            {
                Restore(true);
            } else if(!beforeSave) {
                flags = flagsTemp;
                AssetDatabase.Refresh();
                Selection.activeObject = flags;
            }
        }

        private bool containTag(string tag)
        {
            if(string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(tags))
            {
                return false;
            }
            string[] tagsArr = tags.Split(" "[0]);
            string[] tagArr = tag.Split(" "[0]);
            foreach(string str in tagsArr)
            {
                if(System.Array.IndexOf(tagArr, str) > -1) 
                {
                    return true;
                }
            }
            return false;
        }

        private void OnGUI()
        {

            if (currentScenePath != EditorSceneManager.GetActiveScene().path) 
            {
                initScenes();
            }

            GUILayout.BeginArea(new Rect(10, 10, position.width - 20, 60));
            GUILayout.BeginHorizontal();
            GUILayout.Label(logo);
            GUILayout.FlexibleSpace();
            GUIStyle nobg_style = new GUIStyle(GUI.skin.button);
            nobg_style.normal.background = null;
            nobg_style.padding = new RectOffset(0, 0, 0, 0);
            if(GUILayout.Button(new GUIContent(button_add_tex, "Add flag"), nobg_style, GUILayout.Width(24), GUILayout.Height(24))) 
            {
                AddFlag();
            }
            if(GUILayout.Button(new GUIContent(button_remove_tex, "Remove flag"), nobg_style, GUILayout.Width(24), GUILayout.Height(24)))
            {
                RemoveFlag();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //-----------------------------------------------------------------

            GUILayout.BeginArea(new Rect(120, 50, position.width - 130, 20));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            customScene = EditorGUILayout.Popup(customScene, scenesArr);
            if(GUILayout.Button(new GUIContent(button_open_tex, "Load scene"), nobg_style, GUILayout.Width(16), GUILayout.Height(16)))
            {
                if(customScene >= 0) 
                {
                    LoadScene(scenesPathArr[customScene]);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //-----------------------------------------------------------------

            GUILayout.BeginArea(new Rect(10, 80, position.width - 20, 40));
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name: ", GUILayout.Width(45));
            customFlagName = EditorGUILayout.TextField("", customFlagName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Note: ", GUILayout.Width(45));
            descrFlag = EditorGUILayout.TextField("", descrFlag);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //-----------------------------------------------------------------

            GUILayout.BeginArea(new Rect(10, 120, position.width - 20, 40));
            GUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(button_change_tex, "Change flag"), nobg_style, GUILayout.Width(24), GUILayout.Height(24))) 
            { 
                toChange = !toChange;
            };
            if(GUILayout.Button(new GUIContent(button_clear_tex, "Remove all flags"), nobg_style, GUILayout.Width(24), GUILayout.Height(24))) 
            {
                ClearAll();
            }
            if(GUILayout.Button(new GUIContent(button_gizmo_tex, "Toggle flags gizmos"), nobg_style, GUILayout.Width(24), GUILayout.Height(24))) 
            {
                ToggleGizmos();
            }
            if(GUILayout.Button(new GUIContent(button_action_tex, "Last selected"), nobg_style, GUILayout.Width(24), GUILayout.Height(24))) 
            {
                GoToFlag(lastSelectedFlag);
            }
            iconSize = GUILayout.HorizontalSlider(iconSize, 50F, 200F);
            if(GUILayout.Button(new GUIContent(button_sort_tex, "Sorting type"), nobg_style, GUILayout.Width(16), GUILayout.Height(16))) 
            {
                ToggleSort();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
            //-----------------------------------------------------------------

            GUILayout.BeginArea(new Rect(10, 155, position.width - 15, position.height - 80));
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent(button_tag_tex), GUILayout.Width(20));
            tags = EditorGUILayout.TextField("", tags);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(position.width - 19), GUILayout.Height(position.height - 180));

            int cnt = 1;
            int max = Mathf.Max(Mathf.FloorToInt((position.width - 25) / (iconSize + 10)), 1);

            if(ioe == null) 
            {
                ToggleSort(true);
            }
            
            foreach (KeyValuePair<string, Flag> item in ioe)
            {
                
                Flag flagScript = item.Value;
                
                if(customScene >= 0 && flagScript.scene != scenesPathArr[customScene])
                {
                    continue;
                }

                if(!string.IsNullOrEmpty(tags) && !containTag(flagScript.tags) && !toChange) 
                {
                    continue;
                }

                if(cnt == 1) 
                {
                    GUILayout.BeginHorizontal();
                }

                GUIStyle style = new GUIStyle(GUI.skin.button);
                Texture2D tex = LoadTextureFromFile(flagScript.previewTexture);
                style.normal.background = tex;

                GUIContent content = new GUIContent("", flagScript.name + "\n " + flagScript.descr + "\n Tags: " + flagScript.tags);

                if(GUILayout.Button(content, style, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
                {

                    if(toChange)
                    {
                        ChangeFlag(flagScript);
                        toChange = false;
                        return;
                    } else {
                        GoToFlag(flagScript);
                        return;
                    }
                    
                };

                GUIStyle style_active = new GUIStyle(GUI.skin.box);
                style_active.fontSize = Mathf.Min((int)iconSize / 12, 12);

                if(selectedFlag != null && flagScript.id == selectedFlag.id)
                {
                    style_active.normal.background = border_tex;
                } else {
                    style_active.normal.background = border_top_tex;
                }

                if(!string.IsNullOrEmpty(flagScript.name))
                {
                    string more = flagScript.name.Length > 16 ? "..." : "";
                    GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent(flagScript.name.Substring(0, Mathf.Min(16, flagScript.name.Length)) + more), style_active);
                }

                if(cnt == max)
                {
                    GUILayout.EndHorizontal();
                    cnt = 1;
                } else {
                    cnt++;
                }

                DestroyImmediate(tex);

            }
            
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }

        private void ToggleSort(bool reset = false)
        {
            if(!reset) 
            {
                sort = !sort;
            }
            if(sort)
            {
                ioe = flags.list.OrderByDescending(e => e.Value.id);
            } else {
                ioe = flags.list.OrderBy(e => e.Value.id);
            }
        }

        private void ToggleGizmos(bool refresh = false)
        {

            string currentScenePath = EditorSceneManager.GetActiveScene().path;

            if(string.IsNullOrEmpty(currentScenePath)) 
            {
                return;
            }

            if(!refresh) 
            {
                drawGizmo = !drawGizmo;
            }

            string name = "SceneNavigator";
            GameObject container = GameObject.Find(name);
            if(drawGizmo)
            {
                if(refresh) 
                {
                    DestroyImmediate(container);
                }
                if(container == null)
                {
                    container = new GameObject(name);
                    container.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable | HideFlags.HideInInspector;
                }
                foreach(string key in flags.list.Keys)
                {
                    if(customScene >= 0 && flags.list[key].scene != scenesPathArr[customScene]) 
                    {
                        continue;
                    }
                    Flag flagScript = flags.list[key];
                    GameObject curr = new GameObject(flagScript.id);
                    curr.transform.parent = container.transform;
                    curr.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector | HideFlags.DontSave;
                    FlagScript fs = curr.AddComponent<FlagScript>();
                    fs.flagData = flagScript;
                }
            } else {
                if(container != null)
                {
                    DestroyImmediate(container);
                }
            }
            SceneView.RepaintAll();
        }

        private void ClearAll()
        {
            if(EditorUtility.DisplayDialog("Remove all flags?", "Remove all flags?", "Yes", "No")) 
            {
                Restore();
            }
        }

        private void RemoveFlag(string id = null)
        {
            if(selectedFlag == null && id == null) 
            {
                return;
            }
            string nid = id != null ? id : selectedFlag.id;
            if(id == null)
            {
                selectedFlag = null;
            }
            flags.list.Remove(nid);
            SaveAsset();
            ToggleGizmos(true);
        }

        private void ChangeFlag(Flag f)
        {
            if(f == null) 
            {
                return;
            }

            f.previewTexture = new byte[0];

            string nn = !string.IsNullOrEmpty(customFlagName) ? customFlagName : f.name;
            customFlagName = "";
            string nd = !string.IsNullOrEmpty(descrFlag) ? descrFlag : f.descr;
            descrFlag = "";
            string nt = !string.IsNullOrEmpty(tags) ? tags : f.tags;

            GUI.FocusControl(null);

            Flag nf = createNewFlag(new string[] { f.id, nn }, nd, nt);
            flags.list[f.id] = nf;
            SaveAsset();
            SelectFlag(flags.list[f.id]);
        }

        private bool LoadScene(string scenePath)
        {
            if(scenePath == currentScenePath || string.IsNullOrEmpty(scenePath))
            {
                return false;
            }
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            FileInfo info = new FileInfo(scenePath);
            if(!info.Exists)
            {
                if(EditorUtility.DisplayDialog("Scene is not exist", "Scene is not exist in " + scenePath + "! Change path to scene?", "Change path", "Close"))
                {
                    string newpath = EditorUtility.OpenFilePanel("Scene is not exist", "Assets/", "unity");
                    if (!string.IsNullOrEmpty(newpath)) {
                        newpath = newpath.Substring(newpath.IndexOf("Assets/"));
                        if(new FileInfo(newpath).Exists && ChangeScenePaths(scenePath, newpath))
                        {
                            scenePath = newpath;
                            EditorSceneManager.OpenScene(scenePath);
                            initScenes();
                            return true;
                        }
                    }
                }
                if(EditorUtility.DisplayDialog("Remove all flags?", "Remove all flags in " + scenePath + "?", "Remove", "Close")) 
                {
                    Restore(false, scenePath);
                }
                return false;
            }
            EditorSceneManager.OpenScene(scenePath);
            initScenes();
            return true;
        }

        private bool ChangeScenePaths(string oldPath, string newPath)
        {
            if(string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath)) 
            {
                return false;
            }
            string newName = new FileInfo(newPath).Name;
            newName = newName.Substring(0, newName.IndexOf(".unity"));
            foreach(string key in flags.list.Keys)
            {
                Flag f = flags.list[key];
                if(f.scene == oldPath)
                {
                    f.scene = newPath;
                    f.sceneName = newName;
                }
            }
            return true;
        }

        private void GoToFlag(Flag flag)
        {

            if(flag == null) 
            {
                return;
            }
            if(flag.scene != currentScenePath)
            {
                if(!LoadScene(flag.scene))
                {
                    if(lastSelectedFlag != null && lastSelectedFlag.id == flag.id)
                    {
                        lastSelectedFlag = null;
                    }
                    return;
                }
            } else if(customScene < 0) {
                initScenes();
            } else {
                customScene = customSceneTemp;
            }
            SelectFlag(flag);
            SceneView svf = SceneView.lastActiveSceneView;
            svf.pivot = flag.tp;
            svf.rotation = flag.tr;
            svf.orthographic = flag.to;
            svf.size = flag.size;
        }

        private void SelectFlag(Flag flag)
        {
            if(flag == null)
            {
                return;
            }
            if(selectedFlag == null)
            {
                lastSelectedFlag = flag;
            } else {
                lastSelectedFlag = selectedFlag;
            }
            selectedFlag = flag;
        }

        private void AddFlag()
        {
            if(string.IsNullOrEmpty(currentScenePath))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                initScenes();
            } else if(scenesArr.Length < 1) {
                initScenes();
            }
            if(string.IsNullOrEmpty(currentScenePath))
            {
                EditorUtility.DisplayDialog("Alert", "Save scene before add flags!", "Close");
                return;
            }
            string[] nn = newName();
            Flag newflag = createNewFlag(nn, descrFlag, tags);
            if(newflag == null)
            {
                return;
            }
            flags.list.Add(nn[0], newflag);
            descrFlag = "";
            SaveAsset();
            GoToFlag(newflag);
            ToggleGizmos(true);
        }

        private Flag createNewFlag(string[] nn, string descr = "", string tags = "")
        {
            SceneView lasc = SceneView.lastActiveSceneView;
            if(lasc == null)
            {
                return null;
            }
            Camera c = lasc.camera;
            Flag flagScript = new Flag();
            flagScript.id = nn[0];
            flagScript.name = nn[1];
            flagScript.descr = descr;
            flagScript.tags = tags;
            flagScript.tp = lasc.pivot;
            flagScript.size = lasc.size;
            flagScript.tpos = c.transform.position;
            flagScript.to = lasc.orthographic;
            flagScript.tr = lasc.rotation;
            flagScript.scene = currentScenePath;
            flagScript.sceneName = EditorSceneManager.GetActiveScene().name;

            RenderTexture rt = new RenderTexture(256, 256, 24);
            c.targetTexture = rt;
            c.Render();
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(256, 256, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
            tex.Apply();
            c.targetTexture = null;
            RenderTexture.active = null;
            flagScript.previewTexture = tex.EncodeToPNG();
            DestroyImmediate(tex);

            return flagScript;
        }

        private string[] newName()
        {
            string[] nn = new string[2];
            GUI.FocusControl(null);
            string newFlagName = "Flag";
            nn[0] = "id" + System.DateTime.Now.Ticks.ToString();
            nn[1] = !string.IsNullOrEmpty(customFlagName) ? customFlagName : newFlagName;
            customFlagName = "";
            return nn;
        }

        private void Restore(bool ifnew = false, string scenePath = null)
        {
            if(ifnew)
            {
                flags = CreateInstance<Flags>();
                EditorUtility.SetDirty(flags);
                AssetDatabase.CreateAsset(flags, Path.Combine(pathToAsset, dbName));
            } else {
                if(scenePath == null)
                {
                    string[] ids = flags.list.Keys.ToArray();
                    foreach(string key in ids)
                    {
                        if(flags.list[key].scene != currentScenePath)
                        {
                            continue;
                        }
                        RemoveFlag(key);
                    }
                } else {
                    List<string> toremove = new List<string>();
                    foreach(string key in flags.list.Keys) 
                    {
                        if(flags.list[key].scene == scenePath)
                        {
                            toremove.Add(key);
                        }
                    }
                    foreach(string key in toremove)
                    {
                        RemoveFlag(key);
                    }
                }
                SaveAsset();
            }
            initScenes();
        }

        private void OnDisable()
        {
            SavePrefs();
            if(customSceneTemp >= 0) 
            {
                customScene = customSceneTemp;
            }
        }

        private void OnDestroy()
        {
            SaveAsset();
            SavePrefs();
        }

        private void SaveAsset()
        {
            tryLoadAsset(true);
            EditorUtility.SetDirty(flags);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SavePrefs()
        {
            PlayerPrefs.SetFloat("FlagsSize", iconSize);
            PlayerPrefs.SetInt("FlagsSort", sort ? 1 : 0);
            PlayerPrefs.Save();
        }

        private Texture2D LoadTextureFromFile(byte[] bytes)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return tex;
        }

    }

}