using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace JimmyGao
{
    public class BrushMenuControl : MonoBehaviour
    {
        public static BrushMenuControl Instance;
        private void Awake()
        {
            Instance = this;
        }

        public GameObject StraightButton;
		public GameObject EraseModeButton;
		public GameObject SelectButton;
        public GameObject DuplicateButton;
        public GameObject UndoRedoButton;

		public GameObject ContextMenuEraseModeBtn;
        public GameObject ContextMenuDuplicateBtn;

        public Camera vrCamera;
        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;
        private Text currentTimeText;

        private int number;
        private string photoPath;

        private bool bUpdatedThisFrame;
        
        private void Start()
        {
            currentTimeText = GameObject.Find("CurrentTimeText").GetComponent<Text>();

            #region 程序运行，修改所有的图片文件名称并排序
            number = 1;
            photoPath = Application.streamingAssetsPath + "/Photo";

            if (Directory.Exists(photoPath))
            {
                DirectoryInfo info = new DirectoryInfo(Application.streamingAssetsPath);
                FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length - 1; i++)
                    {
                        if (files[i].Extension == ".meta")
                        {
                            files[i].Delete();
                        }
                        else
                        {
                            files[i].MoveTo(photoPath + "/Photo" + number.ToString() + ".png");
                            if (number < files.Length)
                            {
                                number++;
                            }
                        }
                    }
                }
            }
            #endregion
        }
        
        private void Update()
        {
            DisplayTimeStamp();
        }

        private void LateUpdate()
        {
            bUpdatedThisFrame = false;
        }
        #region 设置直线模式
        /// <summary>
        /// 设置直线模式
        /// </summary>
        public void SetStraightLineMode()
        {
            //var currButton=UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            var currButton = StraightButton;

            if (BrushManager.Instance.BrushMode == 1)
            {
				BrushManager.Instance.BrushMode = 0;
				currButton.GetComponent<ToggleButtonControl>().DoCheck(false);
            }
            else
            {
				UncheckAll();
                BrushManager.Instance.BrushMode = 1;
                currButton.GetComponent<ToggleButtonControl>().DoCheck(true);
            }
        }
        #endregion
        #region 设置擦除模式
        /// <summary>
        /// 设置擦除模式
        /// </summary>
        public void SetEraseMode()
        {
            if (bUpdatedThisFrame) return;
            bUpdatedThisFrame = true;

            //var currButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            var currButton = EraseModeButton;
            ToggleButtonControl toggleButtonControl = null;

            if (currButton != null)
                toggleButtonControl = currButton.GetComponent<ToggleButtonControl>();


            if (MenuControl.Instance.isShow == true)
            {
                if (BrushManager.Instance.BrushMode == 2)
                {
                    BrushManager.Instance.BrushMode = 0;
                    BrushManager.Instance.CurrentState = 1;
                    if (toggleButtonControl != null)
                        toggleButtonControl.DoCheck(false);
                    BrushManager.Instance.isErase = false;
                }
                else
                {
                    UncheckAll();
                    BrushManager.Instance.BrushMode = 2;
                    BrushManager.Instance.CurrentState = 0;
                    if (toggleButtonControl != null)
                        toggleButtonControl.DoCheck(true);
                    BrushManager.Instance.isErase = true;
                    print("擦除模式");
                }
            }
            else
            {
                if (BrushManager.Instance.BrushMode == 2)
                {
                    BrushManager.Instance.BrushMode = 0;
                    BrushManager.Instance.CurrentState = 1;
                    if (toggleButtonControl != null)
                        toggleButtonControl.DoCheck(false);
                    BrushManager.Instance.isErase = false;
                }
                else
                {
                    UncheckAll();
                    BrushManager.Instance.BrushMode = 2;
                    BrushManager.Instance.CurrentState = 0;
                    if (toggleButtonControl != null)
                        toggleButtonControl.DoCheck(true);
                    BrushManager.Instance.isErase = true;
                    print("擦除模式");
                }
            }
        }
        #endregion
        #region 设置选择模式
        /// <summary>
        /// 设置选择模式
        /// </summary>
        public void SetSelectMode()
        {
            if (bUpdatedThisFrame) return;
            bUpdatedThisFrame = true;
            //var currButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            var currButton = SelectButton;

            if (BrushManager.Instance.BrushMode == 3)
            {
                BrushManager.Instance.BrushMode = 0;
                BrushManager.Instance.CurrentState = 1;
                BrushManager.Instance.ContextMenu.SetActive(false);
                
                /*foreach (var item in TriggerSelect.Instance.HighlightList)
                {
                    Destroy(item);
                }
                TriggerSelect.Instance.HighlightList.Clear();*/

                currButton.GetComponent<ToggleButtonControl>().DoCheck(false);
            }
            else
            {
                UncheckAll();
                BrushManager.Instance.BrushMode = 3;
                BrushManager.Instance.CurrentState = 0;

                currButton.GetComponent<ToggleButtonControl>().DoCheck(true);
            }
        }
        #endregion
        #region 设置复制模式
        public void SetDuplicateMode()
        {
            if (bUpdatedThisFrame) return;
            bUpdatedThisFrame = true;
            //var currButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            var currButton = DuplicateButton;

            if (MenuControl.Instance.isShow == true)
            {
                if (BrushManager.Instance.BrushMode == 4)
                {
                    BrushManager.Instance.BrushMode = 3;
                    currButton.GetComponent<ToggleButtonControl>().DoCheck(false);
                }
                else
                {
                    UncheckAll();
                    BrushManager.Instance.BrushMode = 4;
                    currButton.GetComponent<ToggleButtonControl>().DoCheck(true);
                    print("Enabled duplicate");
                }
            }
            else
            {
                if (BrushManager.Instance.BrushMode == 4)
                {
                    BrushManager.Instance.BrushMode = 0;
                    currButton.GetComponent<ToggleButtonControl>().DoCheck(false);
                }
                else
                {
                    UncheckAll();
                    BrushManager.Instance.BrushMode = 4;
                    currButton.GetComponent<ToggleButtonControl>().DoCheck(true);
                    print("Enabled duplicate");
                }
            }
        }
        #endregion
        #region 设置粒子模式
        /// <summary>
        /// 设置粒子模式
        /// </summary>
        public void SetParticleSystemMode()
        {
            if (bUpdatedThisFrame) return;
            bUpdatedThisFrame = true;
            var currButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (BrushManager.Instance.BrushMode == 0)
            {
                BrushManager.Instance.BrushMode = 5;
            }
            else
            {
                BrushManager.Instance.BrushMode = 0;
            }
        }
        #endregion

        #region 点击快照按钮
        public int width;
        public int height;
        public void BtnSnapshot()
        {
            ShowOrHide(false);

            CaptureScreen(vrCamera, new Rect(0, 0, width, height));

            ShowOrHide(true);
        }
        #endregion
        #region VR相机截图
        /// <summary>
        /// VR相机截图
        /// </summary>
        /// <param name="vrCamera">VRCamera</param>
        /// <param name="_rect">截屏的区域</param>
        /// <returns></returns>
        string filename;
        byte[] bytes;
        int num = 0;
        public Texture2D CaptureScreen(Camera vrCamera, Rect _rect)
        {
            RenderTexture rt = new RenderTexture((int)_rect.width, (int)_rect.height, 1);
            vrCamera.targetTexture = rt;
            vrCamera.Render();

            //激活rt, 并从中取出像素
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)_rect.width, (int)_rect.height, TextureFormat.RGB24, false);
            //这个时候其实是从RenderTexture.active中读取像素
            screenShot.ReadPixels(_rect, 0, 0);
            screenShot.Apply();
            //重置相关参数，使得camera继续在屏幕上显示  
            vrCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            bytes = screenShot.EncodeToPNG();
            if (!Directory.Exists(photoPath))
            {
                Directory.CreateDirectory(photoPath);
            }

            if (Directory.Exists(photoPath))
            {
                DirectoryInfo info = new DirectoryInfo(photoPath);
                FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);

                if (files.Length == 0)
                {
                    filename = photoPath + "/Photo" + number.ToString() + ".png";
                    File.WriteAllBytes(filename, bytes);
                    print("截图，第" + number.ToString() + "张");
                }
                else
                {
                    string lastIndexStr = files[files.Length - 1].Name.Substring(5, 1);
                    num = Convert.ToInt32(lastIndexStr);
                    num++;
                    filename = photoPath + "/Photo" + num.ToString() + ".png";
                    File.WriteAllBytes(filename, bytes);
                    print("截图，第" + num.ToString() + "张");
                }
            }
            return screenShot;
        }
        #endregion
        #region 显示或隐藏画板和画笔
        private void ShowOrHide(bool state)
        {
            BrushManager.Instance.BrushMenu.SetActive(state);
            BrushManager.Instance.BrushMenu2.SetActive(state);
            BrushManager.Instance.BrushMenu3.SetActive(state);
            BrushManager.Instance.ContextMenu.SetActive(state);
            BrushManager.Instance.BrushHand.gameObject.SetActive(state);
            BrushManager.Instance.SwitcherHand.gameObject.SetActive(state);
        }
        #endregion
        #region 显示时间戳
        /// <summary>
        /// 显示时间戳
        /// </summary>
        private void DisplayTimeStamp()
        {
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;
            year = DateTime.Now.Year;
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;
            second = DateTime.Now.Second;

            currentTimeText.text = string.Format(
                "{0:D2}:{1:D2}:{2:D2}" +
                "  " +
                "{3:D2}/{4:D2}/{5:D4}",
                hour, minute, second,
                month, day, year);
        }
        #endregion

        public void UncheckAll()
		{
			BrushManager.Instance.BrushMode = 0;
			StraightButton.GetComponent<ToggleButtonControl>().DoCheck(false);
			SelectButton.GetComponent<ToggleButtonControl>().DoCheck(false);
			EraseModeButton.GetComponent<ToggleButtonControl>().DoCheck(false);
            DuplicateButton.GetComponent<ToggleButtonControl>().DoCheck(false);
            UndoRedoButton.GetComponent<ToggleButtonControl>().DoCheck(false);
            ContextMenuEraseModeBtn.GetComponent<ToggleButtonControl>().DoCheck(false);
            ContextMenuDuplicateBtn.GetComponent<ToggleButtonControl>().DoCheck(false);
        }
    }
} 