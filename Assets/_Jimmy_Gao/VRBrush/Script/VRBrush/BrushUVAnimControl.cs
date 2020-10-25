using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JimmyGao;
using System.IO;

public class BrushUVAnimControl : MonoBehaviour
{
    public static BrushUVAnimControl Instance;

    private void Awake()
    {
        Instance = this;
    }

    #region 获取Material文件夹下的所有材质
    /*private string filePath = "Assets/_Jimmy_Gao/VRBrush/Material";
    private FileInfo[] files;
    private void GetAllMaterial()
    {
        if (Directory.Exists(filePath))
        {
            DirectoryInfo info = new DirectoryInfo(filePath);
            files = info.GetFiles("*", SearchOption.AllDirectories);
            print("共有材质：" + files.Length);
            
            for (int i = 0; i < files.Length; i++)
            {
                //if (files[i].Name.EndsWith(".mat"))
                //{
                //    continue;
                //}
                if (files[i].Name == "BrushMat2.mat")
                {
                    print("Name："+files[i].Name());
                }
            }
        }
    }
    */
    #endregion

    //UI上显示的值
    public float uvAnimX = 0;
    public float uvAnimY = 0;


    //实际的材质的offset值
    public float offsetX = 0;
    public float offsetY = 0;

    private void FixedUpdate()
    {
        offsetX++;
        offsetY++;

        gameObject.GetComponent<LineRenderer>().material.SetTextureOffset("_MainTex", new Vector2(offsetX * 0.1f, offsetY * 0.1f));
        
        if (offsetX * 0.1f > uvAnimX)
        {
            offsetX = 0;
        }
        if (offsetY * 0.1f > uvAnimY)
        {
            offsetY = 0;
        }
    }
}