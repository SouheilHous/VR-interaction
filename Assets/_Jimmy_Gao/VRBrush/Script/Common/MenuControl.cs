using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JimmyGao;
using System;
using LitJson;
using System.IO;
using System.Text;
//using UnityEditor;
using DG.Tweening;

public class MenuControl : MonoBehaviour
{
    public static MenuControl Instance;

    public Text sizetext; // 笔刷/UVAnim的挡位
	public int size; // 笔刷增大或缩小的挡位(共有6个挡位，默认为1)
    public List<GameObject> golist = new List<GameObject>();
    public List<GameObject> activeList = new List<GameObject>();
    public List<GameObject> concealList = new List<GameObject>();
    private bool isUndoRedo = false;
    
    public bool isShow;//当前ContextMenu是否显示

    private int currentUI;//正前方的UI，MainMenu为1

    public Material mt;

    public float UVAnimX = 0;
    public float UVAnimY = 0;

    //public GameObject selectObj;

    public Button btnundo_redo;


    

    private Vector3 angle_90 = new Vector3(0, 0, 90f);
    private Vector3 angle_180 = new Vector3(0, 0, 180f);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        size = 1;
        currentUI = 1;
        
        isShow = false;

        
    }

    /// <summary>
    /// 被隐藏的line的数量
    /// </summary>
    private int hideCount = 0;
    #region 给golist赋值
    public int count = 0;
    private int currentCount;
    private void GolistAssign()
    {
        if (count == 0)
        {
            golist = BrushManager.Instance.GoList;
            currentCount = golist.Count;
            print("赋值了");
            count = 1;
        }

        if (isUndoRedo == false)
        {
            count = 0;
        }
    }
    #endregion
    
    private void Update()
    {
        #region 左手柄摇杆左右控制笔刷或线条Size的增大或缩小
        if (BrushManager.Instance.ScaleUp.GetStateDown(BrushManager.Instance.SwitcherHand.handType))
        {
            print("按下PlusSizeButton");
            if (BrushManager.Instance.BrushMode == 3)
            {
                BtnPlusLine();
            }
            else
            {
                BtnPlusBrushSize();
            }
        }

        if (BrushManager.Instance.ScaleDown.GetStateDown(BrushManager.Instance.SwitcherHand.handType))
        {
            print("按下MinusSizeButton");
            if (BrushManager.Instance.BrushMode == 3)
            {
                BtnMinusLine();
            }
            else
            {
                BtnMinusBrushSize();
            }
        }
        #endregion
        
        #region 右手柄摇杆左右做撤销或重做操作
        if (isUndoRedo == true)
        {
            if (BrushManager.Instance.UndoButton.GetStateDown(BrushManager.Instance.BrushHand.handType))
            {
                BtnUndoOperation();
            }
            else if (BrushManager.Instance.RedoButton.GetStateDown(BrushManager.Instance.BrushHand.handType))
            {
                BtnRedoOperation();
            }
        }
        #endregion
        
        #region 管理ContextMenu
        if (BrushManager.Instance.BrushMode == 3)
        {
            if (isShow == false)
            {
                if (BrushManager.Instance.MenuButton.GetStateDown(BrushManager.Instance.BrushHand.handType))
                {
                    BrushManager.Instance.ContextMenu.SetActive(true);
                    isShow = true;
                    //设置ContextMenu的位置
                    BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0f, 0f, 270f), 0.75f);
                    currentUI = 4;
                    print("打开菜单");
                }
            }
            else
            {
                if (BrushManager.Instance.MenuButton.GetStateDown(BrushManager.Instance.BrushHand.handType))
                {
                    BrushManager.Instance.ContextMenu.SetActive(false);
                    isShow = false;
                    //设置ContextMenu的位置
                    BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.75f);
                    currentUI = 1;
                    print("关闭菜单");
                }
            }
        }
        #endregion

        /*切换菜单*/
        ToSwitchTheMenu();
    }

    #region 撤销操作
    /// <summary>
    /// 撤销操作
    /// </summary>
    public void BtnUndoOperation()
    {
        GolistAssign();
        if (golist.Count > 0)
        {
            currentCount--;
            if (currentCount < 0)
            {
                currentCount = 0;
            }
            if (currentCount >= 0 && currentCount < golist.Count)
            {
                if (golist[currentCount].activeSelf == true)
                {
                    golist[currentCount].SetActive(false);
                }
                else
                {
                    for (int i = currentCount; i < golist.Count && i >= 0; i--)
                    {
                        if (golist[i].gameObject != null && golist[i].activeSelf == true)
                        {
                            currentCount = i;
                            break;
                        }
                    }
                    golist[currentCount].SetActive(false);
                }
            }
            print("currentCount：" + currentCount);
        }
    }
    #endregion
    #region 重做操作
    /// <summary>
    /// 重做操作
    /// </summary>
    public void BtnRedoOperation()
    {
        if (golist.Count > 0)
        {
            for (int i = 0; i < golist.Count; i++)
            {
                if (golist[i].gameObject != null && golist[i].activeSelf == false)
                {
                    hideCount++;
                }
            }

            if (hideCount == golist.Count)
            {
                currentCount = 0;
                hideCount = 0;
            }

            if (currentCount > 0)
            {
                if (currentCount == golist.Count)
                {
                    currentCount = golist.Count - 1;
                }
                golist[currentCount].SetActive(true);
                print("currentCount：" + currentCount);
                currentCount++;
            }
            else
            {
                golist[currentCount].SetActive(true);
                print("currentCount：" + currentCount);
                currentCount++;
            }
        }
    }
    #endregion

    #region 更改画笔/被选中线条的颜色
    public string colorIndexStr;
    /// <summary>
    /// 更改画笔/被选中线条的颜色
    /// </summary>
    /// <param name="sender">Sender.</param>
    public void BtnChangeColorClicked(GameObject sender)
    {
        //更换画笔的颜色
        if (BrushManager.Instance.BrushMode == 0 || BrushManager.Instance.BrushMode == 1)
        {
            BrushManager.Instance.SetColor(sender.GetComponent<Image>().color);

            for (int i = 0; i < sender.transform.parent.childCount; i++)
            {
                if (sender.transform.parent.GetChild(i).name == sender.name)
                {
                    colorIndexStr = i.ToString();
                }
            }
        }
        //更换选中线条的颜色
        else if (BrushManager.Instance.BrushMode == 3)
        {
            SetSelectLineColor(sender.GetComponent<Image>().color, sender);
        }
    }
    #endregion

    #region 撤销或重做操作
    /// <summary>
    /// 撤销或重做操作
    /// </summary>
    public void BtnUndoOrRedoClicked ()
	{
        var currButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (isUndoRedo == false)
        {
            isUndoRedo = true;
            print("撤销操作");
            currButton.GetComponent<ToggleButtonControl>().DoCheck(true);
        }
        else if(isUndoRedo == true)
        {
            isUndoRedo = false;
            print("重做操作");
            currButton.GetComponent<ToggleButtonControl>().DoCheck(false);
        }
	}
    #endregion
    #region 清空
    /// <summary>
    /// 清空
    /// </summary>
    public void BtnNewClicked()
    {
        golist = BrushManager.Instance.GoList;
        if (golist.Count > 0)
        {
            //清空line列表
            foreach (GameObject item in golist)
            {
                Destroy(item);
            }
            golist.Clear();
            //清空高亮物体列表
            foreach (var item in TriggerSelect.Instance.HighlightList)
            {
                Destroy(item);
            }
            TriggerSelect.Instance.HighlightList.Clear();
            //关闭ContextMenu
            isShow = false;
            BrushManager.Instance.ContextMenu.SetActive(false);

            isUndoRedo = false;
            BrushMenuControl.Instance.UncheckAll();
        }
        BrushManager.Instance.nameCount = 0;

        //重置UVAnim的尺寸
        UVAnimX = 0;
        UVAnimY = 0;
        GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation");
        sizetext.text = "0"; 
        GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation (1)");
        sizetext.text = "0";
        //重置笔刷尺寸
        size = 1;
        BrushManager.Instance.MinusBrushSize(size);
        RefreshControl(BrushManager.Instance.BrushMenu);
        sizetext.text = "1";
        //清空粒子
        for (int i = 0; i < BrushManager.Instance.ParticleContent.transform.childCount; i++)
        {
            Destroy(BrushManager.Instance.ParticleContent.transform.GetChild(i).gameObject);
        }
        BrushManager.Instance.particleStyleIndexList.Clear();
    }
    #endregion\

    #region 保存（json格式保存）
    /// <summary>
    /// 保存（json格式保存）
    /// </summary>
    public void BtnSaveClicked()
    {
        print("BtnSaveClicked!!!");
        BrushManager.Instance.SaveBrushContent();
        //Arichive();
    }
    #endregion
    #region 加载
    /// <summary>
    /// 加载
    /// </summary>
    public void BtnLoadClicked()
    {
        print("BtnLoadClicked!!!");
        #region 加载前先清空
        golist = BrushManager.Instance.GoList;
        if (golist.Count > 0)
        {
            foreach (GameObject item in golist)
            {
                Destroy(item);
            }
            golist.Clear();
        }
        #endregion

        BrushManager.Instance.LoadBrushContent();
        //LoadArichive();
    }
    #endregion
    #region 增大画笔
    /// <summary>
    /// 增大画笔
    /// </summary>
    public void BtnPlusBrushSize()
    {
        RefreshControl(BrushManager.Instance.BrushMenu);
        size++;
        if (size > 6)
        {
            size = 6;
        }
        else if (size > 1 && size < 7)
        {
            BrushManager.Instance.PlusBrushSize(size);
        }
        sizetext.text = size.ToString();
    }
    #endregion
    #region 减小画笔
    /// <summary>
    /// 减小画笔
    /// </summary>
    public void BtnMinusBrushSize()
    {
        RefreshControl(BrushManager.Instance.BrushMenu);
        size--;
        if (size < 1)
        {
            size = 1;
        }
        sizetext.text = size.ToString();
        BrushManager.Instance.MinusBrushSize(size);
    }
    #endregion
    #region 增大被选中的线条
    /// <summary>
    /// 增大被选中的线条
    /// </summary>
    public void BtnPlusLine()
    {
        if (TriggerSelect.Instance.HighlightList.Count > 0)
        {
            RefreshControl(BrushManager.Instance.ContextMenu);
            size++;
            if (size > 6)
            {
                size = 6;
            }
            else if (size > 1 && size < 7)
            {
                PlusLineSize();
            }
            sizetext.text = size.ToString();
        }
    }
    #endregion
    #region 缩小被选中的线条
    /// <summary>
    /// 缩小被选中的线条
    /// </summary>
    public void BtnMinusLine()
    {
        if (TriggerSelect.Instance.HighlightList.Count > 0)
        {
            RefreshControl(BrushManager.Instance.ContextMenu);
            size--;
            if (size < 1)
            {
                size = 1;
            }
            sizetext.text = size.ToString();
            MinusLineSize(size);
        }
    }
    #endregion
    #region 增加线条的size
    public void PlusLineSize()
    {
        foreach (GameObject highlightObj in TriggerSelect.Instance.HighlightList)
        {
            LineRenderer lr = highlightObj.transform.parent.gameObject.GetComponent<LineRenderer>();
            lr.widthMultiplier *= 1.5f;
        }
    }
    #endregion
    #region 缩小线条的size
    public void MinusLineSize(int size)
    {
        foreach (GameObject highlightObj in TriggerSelect.Instance.HighlightList)
        {
            LineRenderer lr = highlightObj.transform.parent.GetComponent<LineRenderer>();
            lr.widthMultiplier /= 1.5f;
            if (size == 1)
            {
                lr.widthMultiplier = 0.025f;
            }
        }
    }
    #endregion

    #region 获取材质
    /*public void GetMaterial()
    {
        if(TriggerSelect.Instance.HighlightList.Count > 0)
        {
            if (TriggerSelect.Instance.HighlightList.Count == 1)
            {
                //mt = selectObj.GetComponent<LineRenderer>().material;
            }
            if (TriggerSelect.Instance.HighlightList.Count > 1)
            {
                List<GameObject> list = TriggerSelect.Instance.HighlightList;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    mt = list[i].gameObject.GetComponent<LineRenderer>().material;
                    Vector2 offset = mt.GetTextureOffset("_MainTex");
                }
            }
        }
    }*/
    #endregion

    #region 增加UVAnimX的Size
    public void BtnPlusUVAnimX()
    {
        if (BrushManager.Instance.BrushMode == 0 || BrushManager.Instance.BrushMode == 1)
        {
            GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation");
            UVAnimX += 0.1f;
            if (UVAnimX > 1)
            {
                UVAnimX = 1;
            }
            sizetext.text = Convert.ToInt32(UVAnimX * 10).ToString();
        }

        //if (BrushManager.Instance.BrushMode == 3 && TriggerSelect.Instance.HighlightList.Count > 0)
        //{
        //    GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation");
        //    UVAnimX += 0.1f;
        //    if (UVAnimX > 1)
        //    {
        //        UVAnimX = 1;
        //    }
        //    else if(UVAnimX > 0 && UVAnimX < 1)
        //    {
        //        //mt = selectObj.GetComponent<LineRenderer>().material;
        //        BrushUVAnimControl.Instance.uvAnimX = UVAnimX;
        //    }
        //
        //    sizetext.text = Convert.ToInt32(UVAnimX * 10).ToString();
        //    print("当前offset值X：" + UVAnimX);
        //    TriggerSelect.Instance.x = UVAnimX;
        //}
    }
    #endregion
    #region 减小UVAnimX的Size
    public void BtnMinusUVAnimX()
    {
        if (BrushManager.Instance.BrushMode == 0 || BrushManager.Instance.BrushMode == 1)
        {
            GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation");
            UVAnimX -= 0.1f;
            if (UVAnimX < 0)
            {
                UVAnimX = 0;
            }
            sizetext.text = Convert.ToInt32(UVAnimX * 10).ToString();
        }

        //if (BrushManager.Instance.BrushMode == 3 && TriggerSelect.Instance.HighlightList.Count > 0)
        //{
        //    GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation");
        //    UVAnimX -= 0.1f;
        //    if (UVAnimX < 0)
        //    {
        //        UVAnimX = 0;
        //    }
        //    //mt = selectObj.GetComponent<LineRenderer>().material;
        //    BrushUVAnimControl.Instance.uvAnimX = UVAnimX;
        //
        //    sizetext.text = Convert.ToInt32(UVAnimX * 10).ToString();
        //    print("当前offset值X：" + UVAnimX);
        //    TriggerSelect.Instance.x = UVAnimX;
        //}
    }
    #endregion
    #region 增加UVAnimY的Size
    public void BtnPlusUVAnimY()
    {
        if(BrushManager.Instance.BrushMode == 0|| BrushManager.Instance.BrushMode == 1)
        {
            GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation (1)");
            UVAnimY += 0.1f;
            if (UVAnimY > 1)
            {
                UVAnimY = 1;
            }
            sizetext.text = Convert.ToInt32(UVAnimY * 10).ToString();
        }

        //if (BrushManager.Instance.BrushMode == 3 && TriggerSelect.Instance.HighlightList.Count > 0)
        //{
        //    GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation (1)");
        //    UVAnimY += 0.1f;
        //    if (UVAnimY > 1)
        //    {
        //        UVAnimY = 1;
        //    }
        //    else if (UVAnimY > 0 && UVAnimY < 1)
        //    {
        //        //mt = selectObj.GetComponent<LineRenderer>().material;
        //        BrushUVAnimControl.Instance.uvAnimY = UVAnimY;
        //    }
        //
        //    sizetext.text = Convert.ToInt32(UVAnimY * 10).ToString();
        //    print("当前offset值Y：" + UVAnimY);
        //    TriggerSelect.Instance.y = UVAnimY;
        //}
    }
    #endregion
    #region 减小UVAnimY的Size
    public void BtnMinusUVAnimY()
    {
        if(BrushManager.Instance.BrushMode == 0 || BrushManager.Instance.BrushMode == 1)
        {
            GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation (1)");
            UVAnimY -= 0.1f;
            if (UVAnimY < 0)
            {
                UVAnimY = 0;
            }
            sizetext.text = Convert.ToInt32(UVAnimY * 10).ToString();
        }


        //if (BrushManager.Instance.BrushMode == 3 && TriggerSelect.Instance.HighlightList.Count > 0)
        //{
        //    GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation (1)");
        //    UVAnimY -= 0.1f;
        //    if (UVAnimY < 0)
        //    {
        //        UVAnimY = 0;
        //    }
        //    //mt = selectObj.GetComponent<LineRenderer>().material;
        //    BrushUVAnimControl.Instance.uvAnimY = UVAnimY;
        //
        //    sizetext.text = Convert.ToInt32(UVAnimY * 10).ToString();
        //    print("当前offset值Y：" + UVAnimY);
        //    TriggerSelect.Instance.y = UVAnimY;
        //}
    }
    #endregion
    
    #region 设置选中线条的颜色
    /// <summary>
    /// 设置选中线条的颜色
    /// </summary>
    /// <param name="contextMenuColor"></param>
    public void SetSelectLineColor(Color contextMenuColor,GameObject sender)
    {
        if (TriggerSelect.Instance.HighlightList.Count > 0)
        {
            for (int i = 0; i < TriggerSelect.Instance.HighlightList.Count; i++)
            {
                LineRenderer lr = TriggerSelect.Instance.HighlightList[i].transform.parent.GetComponent<LineRenderer>();

                lr.startColor = lr.endColor = contextMenuColor;
                lr.material.SetColor("_Color", contextMenuColor);
                lr.material.SetColor("_TintColor", contextMenuColor);
                BrushManager.Instance.BrushObj.GetComponent<MeshRenderer>().material.color = contextMenuColor;

                for (int j = 0; j < sender.transform.parent.childCount; j++)
                {
                    if (sender.transform.parent.GetChild(j).name == sender.name)
                    {
                        lr.GetComponent<DrawingControl>().ColorID = j.ToString();
                    }
                }
            }
        }
    }
    #endregion

    #region 菜单显示
    /// <summary>
    /// 菜单显示
    /// </summary>
    public void ShowBrushMenu()
    {
        SetMenuPosition(BrushManager.Instance.BrushMenu);
        BrushManager.Instance.BrushMenu.SetActive(true);
    }
    #endregion
    #region 设置主菜单位置
    /// <summary>
    /// 设置主菜单位置
    /// </summary>
    /// <param name="menu"></param>
    public void SetMenuPosition(GameObject menu)
    {
        //			Vector3 menupos = SwitcherHand.transform.position;
        //			menu.transform.position = menupos;
        //			Quaternion rot = Quaternion.Euler (new Vector3 (25f, SwitcherHand.transform.rotation.eulerAngles.y, 0));
        //			menu.transform.rotation = rot;
        GameObject parentUI = menu.transform.parent.gameObject;
        parentUI.transform.parent = BrushManager.Instance.SwitcherHand.transform;
        parentUI.transform.localRotation = Quaternion.Euler(new Vector3(93.17f, BrushManager.Instance.SwitcherHand.transform.rotation.eulerAngles.y, 0));
        parentUI.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        //			menu.transform.localPosition = new Vector3 (0.2f, 0, 0);
    }
    #endregion
    #region 切换菜单
    /// <summary>
    /// 切换菜单
    /// </summary>
    public void ToSwitchTheMenu()
    {
        if (BrushManager.Instance.SwitchMenu.GetStateDown(BrushManager.Instance.SwitcherHand.handType))
        {
            if (isShow == true)
            {
                if (currentUI == 4)//当前ContextMenu
                {
                    currentUI = 1;
                }
                else if (currentUI == 1)
                {
                    currentUI = 2;
                }
                else if (currentUI == 2)
                {
                    currentUI = 3;
                }
                else if (currentUI == 3)
                {
                    currentUI = 4;
                }
                print("currentUI:" + currentUI);
                switch (currentUI)
                {
                    case 1://To BrushMenuMain
                        BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0, 0, 0f), 0.75f);
                        break;
                    case 2://To BrushMenu2
                        BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0, 0, 90f), 0.75f);
                        break;
                    case 3://To BrushMenu3
                        BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0, 0, 180f), 0.75f);
                        break;
                    case 4://To ContextMenu
                        BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0, 0, 270f), 0.75f);
                        break;
                    default:
                        break;
                }
                print("切换菜单！");
                //1：-180,90,90
                //2：-270,90,90
                //3：0,90,90
                //4：-90,90,90
            }
            else
            {
                //1：-270,90,90
                //2：0,90,90
                //3：-90,90,90
                if (currentUI == 1)//当前BrushMenuMain为1
                {
                    currentUI = 2;
                }
                else if (currentUI == 2)
                {
                    currentUI = 3;
                }
                else
                {
                    currentUI = 1;
                }
                switch (currentUI)
                {
                    case 1://To BrushMenuMain
                        BrushManager.Instance.UIMenu.transform.DOLocalRotate(
                            BrushManager.Instance.UIMenu.transform.localRotation.eulerAngles +
                            new Vector3(0, 0, 180f), 0.75f);
                        //BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0, 0, 360f), 0.5f);
                        break;
                    case 2://To BrushMenu2
                        //BrushManager.Instance.UIMenu.transform.DOLocalRotate(
                        //    BrushManager.Instance.UIMenu.transform.localRotation.eulerAngles +
                        //    new Vector3(0, 0, 90f), 1f);
                        BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0, 0, 90f), 0.75f);
                        break;
                    case 3://To BrushMenu3
                        //BrushManager.Instance.UIMenu.transform.DOLocalRotate(
                        //    BrushManager.Instance.UIMenu.transform.localRotation.eulerAngles +
                        //    new Vector3(0, 0, 90f), 1f);
                        BrushManager.Instance.UIMenu.transform.DOLocalRotate(new Vector3(0, 0, 180f), 0.75f);
                        break;
                    default:
                        break;
                }
                print("切换菜单！");
            }
        }
    }
    #endregion

    #region 获取画板的TextSize
    public void RefreshControl(GameObject menu)
    {
        sizetext = menu.transform.GetChild(0).Find("Content/operation/Text").GetComponent<Text>();
    }
    #endregion
    #region 获取UVAnim的TextSize
    public void GetUVAnimTextSize(GameObject menu,string str)
    {
        sizetext = menu.transform.GetChild(0).Find(str + "/operation (1)/Text").GetComponent<Text>();
    }
    #endregion

    #region 改变画笔风格
    public int brushStyleIndex;
    public void ChangeBrushStyle(int i)
    {
        if (BrushManager.Instance.BrushMode == 0 || BrushManager.Instance.BrushMode == 1 || BrushManager.Instance.BrushMode == 5)
        {
            BrushManager.Instance.SetBrushStyle(i);
            brushStyleIndex = i;
        }
    }
    #endregion
    #region 改变粒子风格
    public void ChangeParticleStyle(int i)
    {
        if (BrushManager.Instance.BrushMode != 3)
        {
            BrushManager.Instance.SetParticleStyle(i);
        }
    }
    #endregion
}