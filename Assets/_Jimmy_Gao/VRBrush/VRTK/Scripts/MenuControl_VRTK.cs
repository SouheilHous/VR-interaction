#if VRTK_DEFINE_SDK_STEAMVR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl_VRTK : MonoBehaviour {

	public Text sizetext; // 笔刷挡位显示
	int size; // 笔刷增大或缩小的挡位(共有6个挡位，默认为1)
	List<GameObject> golist = new List<GameObject> ();

	// Use this for initialization
	void Start ()
	{
		size = 1;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	/// <summary>
	/// 后退一步操作
	/// </summary>
	public void BtnUndoClicked ()
	{
		print ("BtnUndoClicked！！！");
		golist = BrushManager_VRTK.Instance.GoList;
		if (golist.Count > 0) {
			GameObject.Destroy (golist [golist.Count - 1]);
			golist.Remove (golist [golist.Count - 1]);
		}
	}

	/// <summary>
	/// 清空
	/// </summary>
	public void BtnNewClicked ()
	{
		print ("BtnNewClicked！！！");
		golist = BrushManager_VRTK.Instance.GoList;
		if (golist.Count > 0) {
			foreach (GameObject item in golist) {
				GameObject.Destroy (item);
			}
			golist.Clear ();
		}
	}

	/// <summary>
	/// 加载
	/// </summary>
	public void BtnLoadClicked ()
	{
		print ("BtnLoadClicked!!!");
		#region 加载前先清空
		golist = BrushManager_VRTK.Instance.GoList;
		if (golist.Count > 0) {
			foreach (GameObject item in golist) {
				GameObject.Destroy (item);
			}
			golist.Clear ();
		}
		#endregion

		BrushManager_VRTK.Instance.LoadBrushContent ();
	}

	/// <summary>
	/// 保存（json格式保存）
	/// </summary>
	public void BtnSaveClicked ()
	{
		print ("BtnSaveClicked!!!");
		BrushManager_VRTK.Instance.SaveBrushContent ();
	}

	/// <summary>
	/// 更改画笔颜色
	/// </summary>
	/// <param name="sender">Sender.</param>
	public void BtnChangeColorCliked (GameObject sender)
	{
		// print (sender.name + "；" + sender.GetComponent<Image> ().color);
		BrushManager_VRTK.Instance.SetColor (sender.GetComponent<Image> ().color);
	}

	/// <summary>
	/// 增大画笔
	/// </summary>
	public void BtnPlusBrushSize ()
	{
		//		RefreshControl ();
		size++;
		if (size > 6) {
			size = 6;
		} else if (size > 1 && size < 7) {
			BrushManager_VRTK.Instance.PlusBrushSize (size);
		}
		sizetext.text = size.ToString ();
	}

	/// <summary>
	/// 减小画笔
	/// </summary>
	public void BtnMinusBrushSize ()
	{
		//		RefreshControl ();
		size--;
		if (size < 1) {
			size = 1;
		}
		sizetext.text = size.ToString ();
		BrushManager_VRTK.Instance.MinusBrushSize (size);
	}


	void RefreshControl ()
	{
		sizetext = BrushManager_VRTK.Instance.BrushMenu.transform.GetChild (0).Find ("Content/operation/Text").GetComponent<Text> ();
	}
}
#endif
