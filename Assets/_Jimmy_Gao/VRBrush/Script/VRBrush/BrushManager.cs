using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.IO;
using System.Text;
using LitJson;
using System.Linq;
using UnityEngine.EventSystems;

namespace JimmyGao
{
    public class BrushManager : MonoBehaviour
    {
        public SteamVR_Action_Boolean grabPinchAction;
        public SteamVR_Action_Boolean grabGripAction;

        public SteamVR_Action_Boolean MenuButton;
        public SteamVR_Action_Boolean ScaleUp;
        public SteamVR_Action_Boolean ScaleDown;
        public SteamVR_Action_Boolean UndoButton;
        public SteamVR_Action_Boolean RedoButton;
        public SteamVR_Action_Boolean SwitchMenu;

        //public SteamVR_TrackedObject TrackedObj;
        public static BrushManager Instance;
        public Hand BrushHand;
        public Hand SwitcherHand;
        public GameObject BrushObj;
        public GameObject BrushMenu;
        public GameObject BrushMenu2;
        public GameObject BrushMenu3;

        public GameObject ContextMenu;
        //public Hand hand2;
        public GameObject InitLineObj;
        public GameObject ParticleInitObj;

        public List<Color> BrushColor;

        public GameObject LaserBeam;

        private LineRenderer currLine;
        private Vector3 lastPos;
        private int segNum = 0;
        // Use this for initialization
        //0 idle  1 draw
        public int CurrentState;
        public int colorIdx;
        public List<GameObject> GoList = new List<GameObject>();
        string txtname = "brushsavejson.txt";

        //0 normal 1 straight line 2 Erase 3 multiselect 4 draw Object
        public int BrushMode = 0;

        //public List<GameObject> SelectedObjs;

        public List<GameObject> DrawHistory;

        public GameObject BrushContent;
        public GameObject ParticleContent;

        public bool isErase = false;

        private bool uvAnimMode = false;//是否处于画UVAnim线模式
        
        private Bounds currLineBound;
        private BoxCollider currLineCollider;
        
        GameObject _particle;

        Transform particleVessel;
        GameObject[] partcleArr = new GameObject[12];
        Transform colorVessel;
        Color[] colorArr = new Color[10];

        private Transform Hand1;
        public GameObject UIMenu;
        private Transform LeftHandUI;

        public void Awake()
        {
            Instance = this;
            
        }

        void Start()
        {
            CurrentState = 0;
            colorIdx = 0;
            SetColor(BrushColor[colorIdx]);
            MenuControl.Instance.ShowBrushMenu();

            ContextMenu.SetActive(false);

            ParticleInitObj.SetActive(false);
            _particle = new GameObject();

            
            //所有的粒子
            particleVessel = GameObject.Find("ParticleVessel").GetComponent<Transform>();
            for (int i = 0; i < particleVessel.childCount; i++)
            {
                partcleArr[i] = particleVessel.GetChild(i).gameObject;
                partcleArr[i].SetActive(false);
            }

            //所有的颜色
            colorVessel = GameObject.Find("ColorVessel").GetComponent<Transform>();
            for (int i = 0; i < colorVessel.childCount; i++)
            {
                colorArr[i] = colorVessel.GetChild(i).GetComponent<Image>().color;
            }

            if (GameObject.Find("Hand1") != null)
            {
                Hand1 = GameObject.Find("Hand1").GetComponent<Transform>();
                UIMenu = new GameObject();
                UIMenu.name = "UIMenu";
                UIMenu.transform.SetParent(Hand1);
                UIMenu.transform.localPosition = new Vector3(0, 0, 0);

                if (GameObject.Find("LeftHandUI") != null)
                {
                    LeftHandUI = GameObject.Find("LeftHandUI").GetComponent<Transform>();
                    LeftHandUI.SetParent(UIMenu.transform);
                    print("设置父物体:" + LeftHandUI.parent.name);
                }
                else
                {
                    print("没有找到");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

            //SteamVR_Controller.Device device = BrushHand.controller;
            //SteamVR_Controller.Device switcherDevice = SwitcherHand.controller;

            // if (device == null || switcherDevice == null)
            //     return;

            //Brush input
            VRLaserRay laserRay = LaserBeam.GetComponent<VRLaserRay>();

            if (laserRay != null && laserRay.PointAt)
            {
                if (grabPinchAction.GetStateDown(BrushHand.handType) && !grabGripAction.GetState(SteamVR_Input_Sources.Any))
                {
                    VRExInputModule.CustomControllerButtonDown = true;
                }
                else if (grabPinchAction.GetStateUp(BrushHand.handType)/* || (grabPinchAction.GetState(BrushHand.handType) && !grabGripAction.GetState(SteamVR_Input_Sources.Any))*/)
                {
                    VRExInputModule.CustomControllerButtonDown = false;
                }
            }
            else if (BrushMode == 0)
            {
                //画线
                DrawLineUpdate();
            }
            else if (BrushMode == 1)
            {
                //画直线
                DrawStraightLineUpdate();
            }
            else if (BrushMode == 2)
            {
                //擦除
            }
            else if (BrushMode == 3)
            {
                //选择
            }
            else if (BrushMode == 4)
            {
                //复制
            }
            else if(BrushMode == 5)
            {
                //粒子
            }

            #region 
            //Switch Input
            // if (switcherDevice.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) && CurrentState == 0)
            // {
            //     Vector2 touchposVec = switcherDevice.GetAxis();
            //     //print (touchposVec);
            //     float touchangle = JimmyUtility.VectorAngle(new Vector2(1, 0), touchposVec);
            //     //print (touchangle);
            //     int touchPos = JimmyUtility.AngleToPosition(touchangle);
            //     print("touched:" + touchPos);
            //     if (touchPos == 3)
            //     {
            //         colorIdx--;
            //         if (colorIdx < 0)
            //             colorIdx = BrushColor.Count - 1;
            //     }
            //     else if (touchPos == 4)
            //     {
            //         colorIdx++;
            //         if (colorIdx > BrushColor.Count - 1)
            //             colorIdx = 0;
            //     }
            //     if (touchPos == 3 || touchPos == 4)
            //     {
            //         SetColor(BrushColor[colorIdx]);
            //     }

            //     if (touchPos == 1)
            //     {
            //         BrushObj.transform.localScale *= 2;
            //         LineRenderer lr = InitLineObj.GetComponent<LineRenderer>();
            //         lr.widthMultiplier *= 2;
            //     }
            //     if (touchPos == 2)
            //     {
            //         BrushObj.transform.localScale /= 2;
            //         LineRenderer lr = InitLineObj.GetComponent<LineRenderer>();
            //         lr.widthMultiplier /= 2;
            //     }

            // }
            #endregion

            #region 判断面板上的UVAnim的size是否有改变
            if (BrushMode == 0 || BrushMode == 1)
            {
                if (MenuControl.Instance.UVAnimX > 0 || MenuControl.Instance.UVAnimY > 0)
                {
                    uvAnimMode = true;
                }
                else
                {
                    uvAnimMode = false;
                }
            }
            #endregion

            //创建粒子特效
            DrawingParticleSystem();

        }
        public int nameCount = 0;

        #region 画线更新
        /// <summary>
        /// DrawLineUpdate
        /// </summary>
        GameObject line;
        public Material lastMaterial;
        public void DrawLineUpdate()
        {
            if (grabPinchAction.GetStateDown(BrushHand.handType) && !grabGripAction.GetState(SteamVR_Input_Sources.Any))
            {
                /*
                GameObject line = new GameObject ();

                currLine = line.AddComponent<LineRenderer> ();

                currLine.startWidth = 0.01f;
                currLine.endWidth = 0.01f;
                */

                line = Instantiate(InitLineObj);
                nameCount++;
                line.name = "line"+nameCount.ToString();

                currLine = line.GetComponent<LineRenderer>();
                lastMaterial = currLine.material;
                
                currLine.transform.parent = BrushContent.transform;
                currLine.gameObject.layer = BrushContent.layer;
                currLine.gameObject.AddComponent<VRBrushLineSelectable>();

                segNum = 1;
                currLine.positionCount = segNum;
                currLine.SetPosition(segNum - 1, BrushObj.transform.position);
                lastPos = BrushObj.transform.position;

                if (uvAnimMode == true)
                {
                    line.gameObject.AddComponent<BrushUVAnimControl>();
                    line.gameObject.GetComponent<BrushUVAnimControl>().uvAnimX = MenuControl.Instance.UVAnimX;
                    line.gameObject.GetComponent<BrushUVAnimControl>().uvAnimY = MenuControl.Instance.UVAnimY;

                    line.gameObject.GetComponent<DrawingControl>().UVAnimX = MenuControl.Instance.UVAnimX;
                    line.gameObject.GetComponent<DrawingControl>().UVAnimY = MenuControl.Instance.UVAnimY;
                }

                CurrentState = 1;

                GoList.Add(line);
                MenuControl.Instance.count = 0;
            }
            else if (grabPinchAction.GetState(BrushHand.handType) && !grabGripAction.GetState(SteamVR_Input_Sources.Any) && CurrentState == 1)
            {
                Vector3 currentPos = BrushObj.transform.position;

                if (Vector3.Distance(lastPos, currentPos) > 0.02)
                {
                    segNum++;
                    //print(segNum);
                    if (currLine != null)
                    {
                        currLine.positionCount = segNum;
                        currLine.SetPosition(segNum - 1, BrushObj.transform.position);
                        lastPos = currentPos;
                    }
                }
            }
            if (grabPinchAction.GetStateUp(BrushHand.handType) || (CurrentState == 1 && grabPinchAction.GetState(BrushHand.handType) && grabGripAction.GetState(SteamVR_Input_Sources.Any)))
            {
                if (line != null)
                {
                    CurrentState = 0;

                    currLineBound = line.GetComponent<Renderer>().bounds;
                    currLineCollider = line.AddComponent<BoxCollider>();
                    currLineCollider.isTrigger = true;
                    currLineCollider.size = currLineBound.size;
                    currLineCollider.center = currLineBound.center - line.transform.position;

                    line.GetComponent<DrawingControl>().DrawingType = "Line";
                    line.GetComponent<DrawingControl>().DrawingStyleID = MenuControl.Instance.brushStyleIndex.ToString();
                    line.GetComponent<DrawingControl>().ColorID = MenuControl.Instance.colorIndexStr.ToString();

                    print("Golist.Count：" + GoList.Count);
                }
            }
        }
        #endregion
        #region 画直线更新
        /// <summary>
        /// DrawStraightLineUpdate
        /// </summary>
        public void DrawStraightLineUpdate()
        {
            Vector3 brushPos = BrushObj.transform.position;
            if (grabPinchAction.GetStateDown(BrushHand.handType) && CurrentState == 0 && !grabGripAction.GetState(SteamVR_Input_Sources.Any))
            {
                GameObject line = Instantiate(InitLineObj);
                currLine = line.GetComponent<LineRenderer>();
                lastMaterial = currLine.material;
                
                currLine.transform.SetParent(BrushContent.transform);
                currLine.gameObject.layer = BrushContent.layer;
                currLine.gameObject.AddComponent<VRBrushLineSelectable>();

                segNum = 2;
                currLine.positionCount = segNum;
                currLine.SetPosition(segNum - 2, brushPos);
                currLine.SetPosition(segNum - 1, brushPos);
                lastPos = brushPos;

                if (uvAnimMode == true)
                {
                    line.gameObject.AddComponent<BrushUVAnimControl>();
                    line.gameObject.GetComponent<BrushUVAnimControl>().uvAnimX = MenuControl.Instance.UVAnimX;
                    line.gameObject.GetComponent<BrushUVAnimControl>().uvAnimY = MenuControl.Instance.UVAnimY;

                    line.gameObject.GetComponent<DrawingControl>().UVAnimX = MenuControl.Instance.UVAnimX;
                    line.gameObject.GetComponent<DrawingControl>().UVAnimY = MenuControl.Instance.UVAnimY;
                }
                line.GetComponent<DrawingControl>().DrawingType = "Line";
                line.GetComponent<DrawingControl>().DrawingStyleID = MenuControl.Instance.brushStyleIndex.ToString();
                line.GetComponent<DrawingControl>().ColorID = MenuControl.Instance.colorIndexStr.ToString();

                CurrentState = 1;
                GoList.Add(line);
                MenuControl.Instance.count = 0;
            }
            if (CurrentState == 1)
            {
                currLine.SetPosition(currLine.positionCount - 1, brushPos);

                if (grabPinchAction.GetStateDown(BrushHand.handType) && !grabGripAction.GetState(SteamVR_Input_Sources.Any))
                {
                    segNum += 2;
                    currLine.positionCount = segNum;
                    currLine.SetPosition(segNum - 2, brushPos);
                    currLine.SetPosition(segNum - 1, brushPos);

                    if (uvAnimMode == true)
                    {
                        line.gameObject.AddComponent<BrushUVAnimControl>();
                        line.gameObject.GetComponent<BrushUVAnimControl>().uvAnimX = MenuControl.Instance.UVAnimX;
                        line.gameObject.GetComponent<BrushUVAnimControl>().uvAnimY = MenuControl.Instance.UVAnimY;
                    }
                }

                if (grabGripAction.GetStateDown(BrushHand.handType))
                {
                    CurrentState = 0;

                    currLineBound = currLine.bounds;
                    currLineCollider = currLine.gameObject.AddComponent<BoxCollider>();
                    currLineCollider.isTrigger = true;
                    currLineCollider.center = currLineBound.center - currLine.gameObject.transform.position;
                    currLineCollider.size = currLineBound.size;

                    

                    print("Golist.Count：" + GoList.Count);
                }
            }
        }
        #endregion

        ParticleStyle currentStyle;

        #region 设置画笔风格
        /// <summary>
        /// 设置画笔风格
        /// </summary>
        /// <param name="styleIdx"></param>
        public void SetBrushStyle(int styleIdx)
        {
            if (BrushMode == 5)
            {
                BrushMode = 0;
            }
            MenuControl.Instance.colorIndexStr = "";

            print("Set Style:" + styleIdx);
            BrushPalette bp = GetComponent<BrushPalette>();
            BrushStyle[] styleList = bp.Palette;

            BrushStyle currStyle = styleList[styleIdx - 1];
            #region 
            /*if(currStyle.LineStyle!=null)
			{
				 LineRenderer lr = InitLineObj.GetComponent<LineRenderer>();
				 lr.startWidth=currStyle.LineStyle.startWidth;
				 lr.endWidth=currStyle.LineStyle.endWidth;
				 lr.startColor=currStyle.LineStyle.startColor;
				 lr.endColor=currStyle.LineStyle.endColor;
				 lr.material=currStyle.LineStyle.material;

			}
			else
			{
				}*/
            #endregion

            LineRenderer lr = InitLineObj.GetComponent<LineRenderer>();
            lr.startWidth = currStyle.StartWidth;
            lr.endWidth = currStyle.EndWidth;
            lr.startColor = currStyle.StartColor;
            lr.endColor = currStyle.EndColor;
            lr.material = currStyle.Material;
            lr.textureMode = currStyle.TextureMode;
            if (currStyle.OverrideMaterialColor)
            {
                lr.material.color = currStyle.StartColor;
                lr.material.SetColor("_Color", currStyle.StartColor);
                lr.material.SetColor("_TintColor", currStyle.StartColor);
            }

            BrushObj.GetComponent<MeshRenderer>().material.color = currStyle.StartColor;
        }
        #endregion
        #region 设置粒子风格
        public int particleStyleIndex;
        public List<int> particleStyleIndexList = new List<int>();
        /// <summary>
        /// 设置粒子风格
        /// </summary>
        /// <param name="styleIdx"></param>
        public void SetParticleStyle(int styleIdx)
        {
            print("Set Particle Style:" + styleIdx);
            ParticlesPalette pp = GetComponent<ParticlesPalette>();
            ParticleStyle[] styleList = pp.Palette;
            currentStyle = styleList[styleIdx - 1];
            particleStyleIndex = styleIdx - 1;

            //BrushObj.GetComponent<MeshRenderer>().material.color = currentStyle.startColor;

            BrushMode = 5;
        }
        #endregion
        #region 画粒子特效
        /// <summary>
        /// 画粒子特效
        /// </summary>
        private void DrawingParticleSystem()
        {
            if (grabPinchAction.GetStateDown(BrushHand.handType) && BrushMode == 5 && EventSystem.current.IsPointerOverGameObject() == false && !grabGripAction.GetState(SteamVR_Input_Sources.Any))
            {
                //_particle = Instantiate(ParticleInitObj);
                //print("创建粒子");
                //ParticleSystem ps = _particle.GetComponent<ParticleSystem>();
                //ParticleSystem.MainModule main = ps.main;
                //main.startColor = currentStyle.startColor;
                //_particle.transform.SetParent(ParticleContent.transform);

                _particle = Instantiate(currentStyle.particle);
                _particle.AddComponent<DrawingControl>();
                //Transform[] childs = _particle.GetComponentsInChildren<Transform>();
                //foreach (var item in childs)
                //{
                //    item.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                //}
                _particle.name = currentStyle.particle.name;
                _particle.transform.SetParent(ParticleContent.transform);

                _particle.transform.position = BrushObj.transform.position;
                _particle.transform.rotation = BrushObj.transform.rotation;

                _particle.GetComponent<DrawingControl>().DrawingType = "Particle";

                _particle.GetComponent<DrawingControl>().ParticleStyleID = particleStyleIndex.ToString();

                _particle.layer = BrushContent.layer;
                _particle.AddComponent<VRBrushParticleSelectable>();

                BoxCollider collider = _particle.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.size = Vector3.one * 5f;
                collider.center = Vector3.zero;

                particleStyleIndexList.Add(particleStyleIndex);

                GoList.Add(_particle);
            }
        }
        #endregion

        #region 设置颜色
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="brushColor"></param>
        public void SetColor(Color brushColor)
        {
            LineRenderer lr = InitLineObj.GetComponent<LineRenderer>();
            lr.startColor = lr.endColor = brushColor;
            lr.material = lastMaterial;
            lr.material.SetColor("_Color", brushColor);
            lr.material.SetColor("_TintColor", brushColor);


            BrushObj.GetComponent<MeshRenderer>().material.color = brushColor;

        }
        #endregion
        #region 颜色索引递增
        /// <summary>
        /// 颜色索引递增
        /// </summary>
        public void PlusColorIdx()
        {
            colorIdx++;
            if (colorIdx > BrushColor.Count - 1)
                colorIdx = 0;
            SetColor(BrushColor[colorIdx]);
        }
        #endregion
        #region 颜色索引递减
        /// <summary>
        /// 颜色索引递减
        /// </summary>
        public void MinusColorIdx()
        {
            colorIdx--;
            if (colorIdx < 0)
                colorIdx = BrushColor.Count - 1;
            SetColor(BrushColor[colorIdx]);
        }
        #endregion

        #region 增加笔刷的size
        /// <summary>
        /// 增加笔刷的size
        /// </summary>
        public void PlusBrushSize(int size)
        {
            BrushObj.transform.localScale *= 1.5f;
            BrushObj.transform.localPosition = new Vector3(0f, BrushObj.transform.localScale.y+0.005f, 0.05f);
            LineRenderer lr = InitLineObj.GetComponent<LineRenderer>();
            lr.widthMultiplier *= 1.5f;
        }
        #endregion
        #region 减小笔刷的size
        /// <summary>
        /// 减小笔刷的size
        /// </summary>
        public void MinusBrushSize(int size)
        {
            BrushObj.transform.localScale /= 1.5f;
            LineRenderer lr = InitLineObj.GetComponent<LineRenderer>();
            lr.widthMultiplier /= 1.5f;
            if (size == 1)
            {
                BrushObj.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
                lr.widthMultiplier = 0.025f;
            }
            BrushObj.transform.localPosition = new Vector3(0f, BrushObj.transform.localScale.y + 0.005f, 0.05f);
        }
        #endregion
        
        #region 保存Brush
        /// <summary>
        /// 保存Brush
        /// </summary>
        public void SaveBrushContent()
        {
            List<DataContent> contentList = new List<DataContent>();
            DataPackage package = new DataPackage();
            string objposition = "";
            string objlineposition = "";
            #region 获取对象信息
            foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.GetComponent<LineRenderer>() != null)
                {
                    DataContent content = new DataContent();
                    LineRenderer objline = obj.GetComponent<LineRenderer>();
                    content.ObjInstanceId = obj.GetInstanceID().ToString();
                    content.NodeCount = objline.positionCount.ToString();
                    // 位置，大小，旋转信息
                    objposition = float.Parse(obj.transform.position.x.ToString("F5")) + ";" + float.Parse(obj.transform.position.y.ToString("F5")) + ";" + float.Parse(obj.transform.position.z.ToString("F5"));
                    for (int i = 0; i < objline.positionCount; i++)
                    {
                        objlineposition += (objline.GetPosition(i).x.ToString("E") + ";" + objline.GetPosition(i).y.ToString("E") + ";" + objline.GetPosition(i).z.ToString("E") + ";");
                    }
                    content.DrawdingTypeData = objline.GetComponent<DrawingControl>().DrawingType;
                    content.DrawdingStyleIdData = objline.GetComponent<DrawingControl>().DrawingStyleID;
                    content.ColorIDData = objline.GetComponent<DrawingControl>().ColorID;
                    content.AnimationXData = objline.GetComponent<DrawingControl>().UVAnimX.ToString();
                    content.AnimationYData = objline.GetComponent<DrawingControl>().UVAnimY.ToString();
                    
                    content.PositionData = objposition + ";" + objlineposition;
                    content.RotationData = float.Parse(obj.transform.rotation.eulerAngles.x.ToString("F5")) + ";" + float.Parse(obj.transform.rotation.eulerAngles.y.ToString("F5")) + ";" + float.Parse(obj.transform.rotation.eulerAngles.z.ToString("F5"));
                    content.ScaleData = float.Parse(obj.transform.localScale.x.ToString("F5")) + ";" + float.Parse(obj.transform.localScale.y.ToString("F5")) + ";" + float.Parse(obj.transform.localScale.z.ToString("F5"));
                    content.WidthData = objline.endWidth.ToString();

                    if (obj.name != "LineInit")
                    {
                        contentList.Add(content);
                    }
                    
                    objlineposition = "";
                }
            }
            for (int i = 0; i < ParticleContent.transform.childCount; i++)
            {
                GameObject obj = ParticleContent.transform.GetChild(i).gameObject;
                DataContent content = new DataContent();
                content.ObjInstanceId = obj.GetInstanceID().ToString();
                objposition =
                    float.Parse(obj.transform.position.x.ToString("F5")) + ";" +
                    float.Parse(obj.transform.position.y.ToString("F5")) + ";" +
                    float.Parse(obj.transform.position.z.ToString("F5"));
                content.PositionData = objposition;
                content.RotationData =
                    float.Parse(obj.transform.rotation.eulerAngles.x.ToString("F5")) + ";" +
                    float.Parse(obj.transform.rotation.eulerAngles.y.ToString("F5")) + ";" +
                    float.Parse(obj.transform.rotation.eulerAngles.z.ToString("F5"));
                content.ScaleData =
                    float.Parse(obj.transform.localScale.x.ToString("F5")) + ";" +
                    float.Parse(obj.transform.localScale.y.ToString("F5")) + ";" +
                    float.Parse(obj.transform.localScale.z.ToString("F5"));

                content.DrawdingTypeData = "Particle";
                content.ParticleStyleIdData = ParticleContent.transform.GetChild(i).GetComponent<DrawingControl>().ParticleStyleID.ToString();

                if (obj.name != "ParticleInit")
                {
                    contentList.Add(content);
                }
            }
            #endregion
            package.InstanceID = System.Guid.NewGuid().ToString();
            package.CreateTimestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            package.LastUpdateTimestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            //contentList.RemoveAt(contentList.Count - 1); // 移除最后一个LineInit,也就是初始LineInit
            package.DataContentList = contentList;
            //将对象序列化为json字符串
            string json_txt = JsonMapper.ToJson(package);
            WriteTxt(json_txt);

        }
        #endregion
        #region 写入存档文件
        /// <summary>
        /// 写入存档文件
        /// </summary>
        /// <param name="json">Json.</param>
        private void WriteTxt(string jsontxt)
        {
            string datapath = GetDataPath() + "/DataJson/";
            string path = datapath + txtname;
            if (!File.Exists(datapath))
            {
                Directory.CreateDirectory(datapath);
            }
            StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
            //开始写入
            sw.Write(jsontxt);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
        }
        #endregion
        #region 获得存档路径
        /// <summary>
        /// 获得存档路径
        /// </summary>
        /// <returns>The data path.</returns>
        private string GetDataPath()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // iPhone路径
                string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
                path = path.Substring(0, path.LastIndexOf('/'));
                return path + "/Documents";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                // 安卓路径
                return Application.persistentDataPath + "/";
            }
            else
            {
                // 其它路径
                return Application.streamingAssetsPath;
            }
        }
        #endregion
        #region 读取存档文件
        /// <summary>
        /// 读取存档文件
        /// </summary>
        private string ReadTxt()
        {
            string datapath = GetDataPath() + "/DataJson/";
            string path = datapath + txtname;
            if (File.Exists(path))
            {
                //读取文件内容
                StreamReader sr = new StreamReader(path);
                string str_read = sr.ReadToEnd();
                sr.Close();
                return str_read;
            }
            else
            {
                return "";
            }
        }
        #endregion
        #region 读取Brush和粒子
        /// <summary>
        /// 读取Brush和粒子
        /// </summary>
        public void LoadBrushContent ()
		{
			string json_txt = ReadTxt ();
			// 反序列化成对象
			DataPackage package = JsonMapper.ToObject<DataPackage> (json_txt);
			List<DataContent> contentList = package.DataContentList;
			if (contentList.Count > 0)
            {
				foreach (DataContent item in contentList)
                {
                    if (item.DrawdingTypeData == "Line")
                    {
                        GameObject go = Instantiate(InitLineObj);
                        go.transform.SetParent(BrushContent.transform);
                    
                        InitialBrush(go, item);
                        GoList.Add(go);
                    }
                    else if (item.DrawdingTypeData == "Particle")
                    {
                        InitialParticle(item);
                    }
                }
			}
        }
        #endregion
        #region 最初的粒子
        private void InitialParticle(DataContent model)
        {
            GameObject obj = Instantiate(partcleArr[int.Parse(model.ParticleStyleIdData)]);
            List<float> poslist = SplitStringAndFormatFloat(model.PositionData.Replace("(", "").Replace(")", "").Replace(",", ";").TrimEnd(';'), false);
            List<float> rotlist = SplitStringAndFormatFloat(model.RotationData, false);
            List<float> scalist = SplitStringAndFormatFloat(model.ScaleData, false);
            Vector3 pos = new Vector3(poslist[0], poslist[1], poslist[2]);
            Quaternion rot = Quaternion.Euler(rotlist[0], rotlist[1], rotlist[2]);
            Vector3 sca = new Vector3(scalist[0], scalist[1], scalist[2]);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.transform.localScale = sca;
            obj.AddComponent<DrawingControl>();
            obj.GetComponent<DrawingControl>().DrawingType = model.DrawdingTypeData;
            obj.GetComponent<DrawingControl>().ParticleStyleID = model.ParticleStyleIdData;
            obj.SetActive(true);
            obj.transform.SetParent(ParticleContent.transform);

            #region 
            /*
            string index = "";
            for (int i = 0; i < ParticleContent.transform.childCount; i++)
            {
                index = ParticleContent.transform.GetChild(i).name.Substring(1, 1);
                if (index == "1")
                {
                    obj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                }
                else if(index == "2"|| index == "3"|| index == "4")
                {
                    obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                else if (index == "5")
                {
                    obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }
                else if (index == "7" || index == "8")
                {
                    obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else if (index == "11")
                {
                    obj.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                }
                else if (index == "11")
                {
                    obj.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
                }
                else if (index == "6" || index == "9" || index == "10")
                {
                    
                }
            }
            */
            #endregion
        }
        #endregion
        #region 最初的画笔
        string colorId = "";
        string styleId = "";
        /// <summary>
        /// 最初的画笔
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        private void InitialBrush (GameObject obj, DataContent model)
		{
			List<float> poslist = SplitStringAndFormatFloat (model.PositionData.Replace("(","").Replace(")","").Replace(",",";").TrimEnd(';'), false);
			List<float> rotlist = SplitStringAndFormatFloat (model.RotationData, false);
			List<float> scalist = SplitStringAndFormatFloat (model.ScaleData, false);

            Vector3 pos = new Vector3 (poslist [0], poslist [1], poslist [2]);
			Quaternion rot = Quaternion.Euler (rotlist [0], rotlist [1], rotlist [2]);
			Vector3 sca = new Vector3 (scalist [0], scalist [1], scalist [2]);
			obj.transform.position = pos;
			obj.transform.rotation = rot;
			obj.transform.localScale = sca;
			obj.GetComponent<LineRenderer> ().positionCount = int.Parse (model.NodeCount);
			Vector3 linpos;
			List<Vector3> linposs = new List<Vector3> ();
			for (int i = 1; i < poslist.Count / 3; i++) {
				linpos = new Vector3 (poslist [3 * i], poslist [3 * i + 1], poslist [3 * i + 2]);
				linposs.Add (linpos);
			}
			obj.GetComponent<LineRenderer> ().SetPositions (linposs.ToArray ());
			obj.GetComponent<LineRenderer> ().startWidth = obj.GetComponent<LineRenderer> ().endWidth = float.Parse (model.WidthData);

            obj.GetComponent<LineRenderer>().material =
                    transform.GetComponent<BrushPalette>().Palette[int.Parse(model.DrawdingStyleIdData) - 1].Material;
            obj.GetComponent<LineRenderer>().material.SetTextureOffset("_MainTex", new Vector2(float.Parse(model.AnimationXData), float.Parse(model.AnimationYData)));
            
            obj.GetComponent<DrawingControl>().DrawingType = "Line";
            obj.GetComponent<DrawingControl>().DrawingStyleID = model.DrawdingStyleIdData;
            obj.GetComponent<DrawingControl>().ColorID = model.ColorIDData;
            
            obj.GetComponent<DrawingControl>().UVAnimX = float.Parse(model.AnimationXData);
            obj.GetComponent<DrawingControl>().UVAnimY = float.Parse(model.AnimationYData);
            //设置UV
            obj.AddComponent<BrushUVAnimControl>();
            obj.GetComponent<BrushUVAnimControl>().uvAnimX = float.Parse(model.AnimationXData);
            obj.GetComponent<BrushUVAnimControl>().uvAnimY = float.Parse(model.AnimationYData);

            #region 设置材质属性
            styleId = model.DrawdingStyleIdData.ToString();
            if (model.ColorIDData.ToString() != "")
            {
                colorId = model.ColorIDData.ToString();
            }
            switch (int.Parse(styleId) - 1)
            {
                case 0:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.RepeatPerSegment;
                    SetTintColor1(model, obj);
                    break;
                case 1:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.RepeatPerSegment;
                    SetTintColor1(model, obj);
                    break;
                case 2:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor1(model, obj);
                    break;
                case 3:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.RepeatPerSegment;
                    SetTintColor1(model, obj);
                    break;
                case 4:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor1(model, obj);
                    break;
                case 5:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 6:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.RepeatPerSegment;
                    SetTintColor2(model, obj);
                    break;
                case 7:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 8:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 9:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 10:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 11:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 12:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 13:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetTintColor2(model, obj);
                    break;
                case 14:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetColor1(model, obj);
                    break;
                case 15:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetColor2(model, obj);
                    break;
                case 16:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetColor2(model, obj);
                    break;
                case 17:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetColor2(model, obj);
                    break;
                case 18:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetColor2(model, obj);
                    break;
                case 19:
                    obj.GetComponent<LineRenderer>().textureMode = LineTextureMode.Stretch;
                    SetColor2(model, obj);
                    break;
                default:
                    break;
            }
            #endregion

            Bounds objBounds = obj.GetComponent<Renderer>().bounds;
            BoxCollider objCollider = obj.AddComponent<BoxCollider>();
            objCollider.isTrigger = true;
            objCollider.size = objBounds.size;
            objCollider.center = objBounds.center - obj.transform.position;
        }
        #endregion
        #region 设置Color
        private void SetTintColor1(DataContent model,GameObject obj)
        {
            if (model.ColorIDData == "")
            {
                obj.GetComponent<LineRenderer>().startColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].StartColor;
                obj.GetComponent<LineRenderer>().endColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].EndColor;
                obj.GetComponent<LineRenderer>().material.SetColor("_TintColor",obj.GetComponent<LineRenderer>().startColor);
            }
            else if (model.ColorIDData != "")
            {
                obj.GetComponent<LineRenderer>().startColor = obj.GetComponent<LineRenderer>().endColor = colorArr[int.Parse(colorId)];
                obj.GetComponent<LineRenderer>().material.SetColor("_TintColor", 
                    colorArr[int.Parse(colorId)]);
            }
        }
        private void SetTintColor2(DataContent model, GameObject obj)
        {
            if (model.ColorIDData == "")
            {
                obj.GetComponent<LineRenderer>().startColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].StartColor;
                obj.GetComponent<LineRenderer>().endColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].EndColor;
                obj.GetComponent<LineRenderer>().material.SetColor("_TintColor",
                    transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].Material.GetColor("_TintColor"));
            }
            else if (model.ColorIDData != "")
            {
                obj.GetComponent<LineRenderer>().startColor = colorArr[int.Parse(colorId)];
                obj.GetComponent<LineRenderer>().endColor = colorArr[int.Parse(colorId)];
                obj.GetComponent<LineRenderer>().material.SetColor("_TintColor", colorArr[int.Parse(colorId)]);
            }
        }
        private void SetColor1(DataContent model, GameObject obj)
        {
            if (model.ColorIDData == "")
            {
                obj.GetComponent<LineRenderer>().startColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].StartColor;
                obj.GetComponent<LineRenderer>().endColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].EndColor;
                obj.GetComponent<LineRenderer>().material.SetColor("_Color",
                    transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].Material.GetColor("_Color"));
            }
            else if (model.ColorIDData != "")
            {
                obj.GetComponent<LineRenderer>().startColor = colorArr[int.Parse(colorId)];
                obj.GetComponent<LineRenderer>().endColor = colorArr[int.Parse(colorId)];
                obj.GetComponent<LineRenderer>().material.SetColor("_Color", colorArr[int.Parse(colorId)]);
            }
        }
        private void SetColor2(DataContent model, GameObject obj)
        {
            if (model.ColorIDData == "")
            {
                obj.GetComponent<LineRenderer>().startColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].StartColor;
                obj.GetComponent<LineRenderer>().endColor = transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].EndColor;
                print(transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].name);
                obj.GetComponent<LineRenderer>().material.SetColor("_Color",
                    transform.GetComponent<BrushPalette>().Palette[int.Parse(styleId) - 1].StartColor);
            }
            else if (model.ColorIDData != "")
            {
                obj.GetComponent<LineRenderer>().startColor = colorArr[int.Parse(colorId)];
                obj.GetComponent<LineRenderer>().endColor = colorArr[int.Parse(colorId)];
                obj.GetComponent<LineRenderer>().material.SetColor("_Color", 
                    colorArr[int.Parse(colorId)]);
            }
        }
        #endregion
        #region 分割字符串并将其转化为float类型
        /// <summary>
        /// 分割字符串并将其转化为float类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="flag">(false:根据 ; 切割)(true：根据 , 切割)</param>
        /// <returns></returns>
        private List<float> SplitStringAndFormatFloat (string str, bool flag)
		{
			List<float> strfloat = new List<float> ();
			string[] strArr = new string[]{ };
			if (flag == false) {
				strArr = str.Split (';');
			} else if (flag == true) {
				strArr = str.Split (',');
			}
			foreach (var item in strArr) {
				strfloat.Add (float.Parse(item));
			}
			return strfloat;
		}
        #endregion
    }
}