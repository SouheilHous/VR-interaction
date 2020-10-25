using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BrushStyle", menuName = "BrushStyle")]
public class BrushStyle : ScriptableObject {
    //public LineRenderer LineStyle;
    public Material Material;
	public float StartWidth=0.005f;
	public float EndWidth=0.005f;
	public Color StartColor=Color.blue;
	public Color EndColor=Color.white;

	public  bool OverrideMaterialColor=false;

	public LineTextureMode TextureMode=LineTextureMode.Stretch;

}
