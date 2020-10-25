using JimmyGao;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerSelect : MonoBehaviour
{
    public static TriggerSelect Instance;

    public GameObject helperCubeTemp;//高光效果物体，后期替换
    private GameObject HighlightObj;
    public List<GameObject> HighlightList = new List<GameObject>();
    
    public bool isAdd = false;
    public GameObject uvSignObj;
    private int signCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Bounds penBounds = gameObject.GetComponent<MeshRenderer>().bounds;
        SphereCollider penCollider = gameObject.AddComponent<SphereCollider>();
        Rigidbody penRigigbody = gameObject.AddComponent<Rigidbody>();
        penCollider.isTrigger = true;
        penCollider.center = penBounds.center - gameObject.transform.position;
        penRigigbody.mass = 0;
        penRigigbody.angularDrag = 0;
        penRigigbody.useGravity = false;
        
    }
    public bool isMoveMode = false;
    private Vector3 lastPos;
    private void Update()
    {
        if (HighlightList.Count > 0)
        {
            isMoveMode = true;
        }
        else
        {
            isMoveMode = false;
        }

        #region 拖拽移动
        if (BrushManager.Instance.BrushMode == 3 && isMoveMode == true)
        {
            if (BrushManager.Instance.grabGripAction.GetStateDown(BrushManager.Instance.BrushHand.handType))
            {
                lastPos = BrushManager.Instance.BrushObj.transform.position;
                print("开始拖拽");
            }
            else if (BrushManager.Instance.grabGripAction.GetState(BrushManager.Instance.BrushHand.handType))
            {
                Vector3 currentPos = BrushManager.Instance.BrushObj.transform.position;
                Vector3 distance = currentPos - lastPos;
                for (int i = 0; i < HighlightList.Count; i++)
                {
                    if(HighlightList[i].transform.gameObject!=null)
                    HighlightList[i].transform.parent.position += distance;
                }
                lastPos = currentPos;
                print("拖拽中");
            }
            else if (BrushManager.Instance.grabGripAction.GetStateUp(BrushManager.Instance.BrushHand.handType))
            {
                print("拖拽结束");
            }
        }
        #endregion
    }

    #region 触发事件
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 3)
        {
            if (other.transform.childCount == 0)
            {
                HighlightObj = Instantiate(helperCubeTemp);
                HighlightObj.transform.SetParent(other.transform);
                HighlightObj.transform.localPosition = other.gameObject.GetComponent<BoxCollider>().center;
                HighlightObj.transform.localScale = other.gameObject.GetComponent<LineRenderer>().bounds.size;

                HighlightObj.name = "TempObj";
            }
            else
            {
                for (int i = 0; i < other.transform.childCount; i++)
                {
                    Transform child = other.transform.GetChild(i);
                    if (child.name.Contains("TempObj") == false && child.name.Contains("ListObj") == false)
                    {
                        isAdd = true;
                    }
                    else
                    {
                        isAdd = false;
                    }
                }
                if (isAdd == true)
                {
                    HighlightObj = Instantiate(helperCubeTemp);
                    HighlightObj.transform.SetParent(other.transform);
                    HighlightObj.transform.localPosition = other.gameObject.GetComponent<BoxCollider>().center;
                    HighlightObj.transform.localScale = other.gameObject.GetComponent<LineRenderer>().bounds.size;

                    HighlightObj.name = "TempObj";
                }
            }
        }
    }

    private IEnumerator OnTriggerStay(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 3)
        {
            Transform child = null;
            for (int i = 0; i < other.transform.childCount; i++)
            {
                child = other.transform.GetChild(i);
                
                if (child.name.Contains("TempObj") == true)
                {
                    if (BrushManager.Instance.grabPinchAction.GetStateDown(BrushManager.Instance.BrushHand.handType))
                    {
                        print("选中");
                        child.gameObject.name = "ListObj";
                        HighlightList.Add(child.gameObject);
                        print("highlightList.Count：" + HighlightList.Count);

                        #region 
                        //other.gameObject.AddComponent<BrushUVAnimControl>();
                        //if (signCount == 0)
                        //{
                        //    GameObject uvSign = Instantiate(uvSignObj);
                        //    uvSign.name = "uvSign";
                        //    uvSign.transform.SetParent(other.gameObject.transform);
                        //    print("添加标志物体！");
                        //
                        //    signCount = 1;
                        //}

                        //Vector2 uvOffset = other.gameObject.GetComponent<LineRenderer>().material.GetTextureOffset("_MainTex");
                        //MenuControl.Instance.UVAnimX = uvOffset.x;
                        //MenuControl.Instance.UVAnimY = uvOffset.y;
                        //MenuControl.Instance.GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation");
                        //MenuControl.Instance.sizetext.text = Convert.ToInt32(MenuControl.Instance.UVAnimX * 10).ToString();
                        //MenuControl.Instance.GetUVAnimTextSize(BrushManager.Instance.BrushMenu2, "Animation (1)");
                        //MenuControl.Instance.sizetext.text = Convert.ToInt32(MenuControl.Instance.UVAnimY * 10).ToString();
                        //
                        //MenuControl.Instance.selectObj = other.gameObject;
                        #endregion
                    }
                }
                else if (child.name.Contains("ListObj") == true)
                {
                    yield return new WaitForSeconds(0.1f);
                    if (BrushManager.Instance.grabPinchAction.GetStateDown(BrushManager.Instance.BrushHand.handType))
                    {
                        StartCoroutine(OnTriggerStay(other));
                        HighlightList.Remove(child.gameObject);
                        Destroy(child.gameObject);
                        print("移除了：" + child.gameObject.name);
                    }
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 3)
        {
            for (int i = 0; i < other.transform.childCount; i++)
            {
                Transform child = other.transform.GetChild(i);
                if (child.name.Contains("TempObj") == true)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
    #endregion
}