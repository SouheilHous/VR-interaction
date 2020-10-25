using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JimmyGao;

public class TriggerDuplicate : MonoBehaviour
{
    private float offset = 0.1f;
    
    #region 触发事件
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 4)
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
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 4)
        {
            if (BrushManager.Instance.grabPinchAction.GetStateUp(BrushManager.Instance.BrushHand.handType))
            {
                print("点击了复制");
                GameObject newLine = Instantiate(other.gameObject);
                newLine.transform.SetParent(BrushManager.Instance.BrushContent.transform);
                //在当前对象的位置上进行偏移
                newLine.transform.position = other.transform.position + new Vector3(0, offset, 0);
                //在golist列表中的最后一个对象的位置上进行偏移
                //newLine.transform.position = BrushManager.Instance.GoList[BrushManager.Instance.GoList.Count - 1].transform.position + new Vector3(0, offset, 0);
                //newLine.transform.rotation = other.transform.rotation;
                BrushManager.Instance.GoList.Add(newLine);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Line" && BrushManager.Instance.BrushMode == 4)
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
