using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class HierarchyMenuItem : MonoBehaviour
{
    public GameObject goButtonOpen;
    public Text tmpText;

    private Transform t;

    public void ClickOpen()
    {
        GetComponentInParent<HierarchyMenu>().SelectTransform(t);
    }

    public void ClickSelect()
    {
        Selection.activeGameObject = t.gameObject;
    }

    public void SetTransform(Transform t)
    {
        this.t = t;
        goButtonOpen.SetActive(t.childCount > 0);
        tmpText.text = t.name;
    }
}
