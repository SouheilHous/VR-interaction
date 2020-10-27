using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    delegate void ChangeLights();
    ChangeLights change;

    private float _myLightIntensity;

    public GameObject[] Lights;
    public Slider sliderObj;

    public float LightIntensity
    {
        get { return _myLightIntensity; }
        set
        {
            _myLightIntensity = value;
            ChangeIntensity();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _myLightIntensity = 1f;
        LightIntensity = 1f;
        GetLights();
    }

    private void GetLights()
    {
        Lights = GameObject.FindGameObjectsWithTag("SmallLight");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeIntensity()
    {
        print("Changing----");
        if(Lights.Length > 0)
        {
            foreach (GameObject l in Lights)
            {
                l.GetComponent<Light>().intensity = _myLightIntensity;
            }
        }
    }
}
