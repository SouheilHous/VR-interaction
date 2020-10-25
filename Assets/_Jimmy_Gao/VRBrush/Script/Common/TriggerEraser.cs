using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JimmyGao;

public class TriggerEraser : MonoBehaviour
{
    private List<GameObject> EraserList = new  List<GameObject>();
    private int eraserCount;

    #region 给EraserList赋值
    public int count = 0;
    private void EraserListAssign()
    {
        if (count == 0)
        {
            eraserCount = EraserList.Count;
            count++;
        }
        if (BrushManager.Instance.isErase == false)
        {
            count = 0;
        }
    }
    #endregion

    private void Update()
    {
        RepealEraserObj();
    }

    #region 撤销擦除的物体
    /// <summary>
    /// 撤销擦除的物体
    /// </summary>
    private void RepealEraserObj()
    {
        if (BrushManager.Instance.isErase == true)
        {
            if (BrushManager.Instance.RedoButton.GetStateDown(BrushManager.Instance.BrushHand.handType))
            {
                EraserListAssign();
                if (EraserList.Count > 0)
                {
                    eraserCount--;
                    if (eraserCount < 0)
                    {
                        eraserCount = 0;
                    }

                    GameObject obj = EraserList[eraserCount];
                    obj.SetActive(true);
                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        Transform child = obj.transform.GetChild(i);
                        if (child.name.Contains("TempObj") == true)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                    EraserList.Remove(obj);
                }
            }
        }
    }
    #endregion

    #region 触发事件
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 2)
        {
            if (other.transform.childCount == 0)
            {
                GameObject HighlightObj = Instantiate(TriggerSelect.Instance.helperCubeTemp);
                HighlightObj.transform.SetParent(other.transform);
                HighlightObj.transform.localPosition = other.gameObject.GetComponent<BoxCollider>().center;
                HighlightObj.transform.localScale = other.gameObject.GetComponent<LineRenderer>().bounds.size;

                HighlightObj.name = "TempObj";
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 2)
        {
            if (BrushManager.Instance.grabPinchAction.GetStateDown(BrushManager.Instance.BrushHand.handType))
            {
                //EraserList.Add(other.gameObject);
                //other.gameObject.SetActive(false);
                Destroy(other.gameObject);
                print("擦除物体");
                print("列表中物体个数为：" + EraserList.Count);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 2)
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
