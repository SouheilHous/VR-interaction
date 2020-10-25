using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateRadialMenuTest : MonoBehaviour
{
    public UltimateRadialMenu menu;
    public Sprite example;

    // Start is called before the first frame update
    void Start()
    {
        UltimateRadialButtonInfo info = new UltimateRadialButtonInfo();
        info.key = "Example";
        info.description = "Example";
        info.icon = example;


        menu.AddRadialButton(RadialButtonCallback, info);
        menu.EnableRadialMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RadialButtonCallback(string text)
    {
        Debug.Log(text);
    }
}
