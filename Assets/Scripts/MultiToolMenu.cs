using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MultiTool3D;
using TMPro;
using UnityEditor.Experimental.EditorVR.UI;

public class MultiToolMenu : MonoBehaviour
{
    public MultiTool3D.MultiTool3D multiTool;
    public GameObject goKeyboard;

    public ToggleGroup toggleGroupUnit;
    public ToggleGroup toggleGroupTool;
    public ToggleGroup toggleGroupEnabled;
    public Toggle toggleEnabledYes;
    public Toggle toggleEnabledNo;
    public Toggle toggleModeBox;
    public Toggle toggleModeSphere;
    public Toggle toggleModePlane;

    public GameObject goToolRuler;
    public GameObject goToolAngle;
    public GameObject goToolVolume;

    public GameObject goToolVolumeBox;
    public GameObject goToolVolumeSphere;
    public GameObject goToolVolumePlane;


    public TextMeshProUGUI[] tmpRulerFrom;
    public TextMeshProUGUI[] tmpRulerTo;
    public TextMeshProUGUI[] tmpAngleBase;
    public TextMeshProUGUI[] tmpAnglePoint1;
    public TextMeshProUGUI[] tmpAnglePoint2;
    public TextMeshProUGUI[] tmpVolumeBoxCenter;
    public TextMeshProUGUI[] tmpVolumeBoxSize;
    public TextMeshProUGUI tmpVolumeSphereRadius;
    public TextMeshProUGUI[] tmpVolumePlanePoint1;
    public TextMeshProUGUI[] tmpVolumePlanePoint2;
    public TextMeshProUGUI[] tmpVolumePlanePoint3;
    public TextMeshProUGUI[] tmpVolumePlanePoint4;
    
    private List<Toggle> valueToggles;
    private int selectedIndex;
    private string typingValue;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        valueToggles = new List<Toggle>();
        valueToggles.Add(tmpRulerFrom[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpRulerFrom[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpRulerFrom[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpRulerTo[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpRulerTo[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpRulerTo[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAngleBase[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAngleBase[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAngleBase[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAnglePoint1[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAnglePoint1[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAnglePoint1[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAnglePoint2[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAnglePoint2[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpAnglePoint2[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumeBoxCenter[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumeBoxCenter[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumeBoxCenter[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumeBoxSize[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumeBoxSize[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumeBoxSize[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumeSphereRadius.transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint1[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint1[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint1[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint2[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint2[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint2[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint3[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint3[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint3[2].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint4[0].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint4[1].transform.parent.GetComponent<Toggle>());
        valueToggles.Add(tmpVolumePlanePoint4[2].transform.parent.GetComponent<Toggle>());

        selectedIndex = -1;
        goKeyboard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool bIsEnabled = false;

        //Enabled Toggle
        foreach (Toggle t in toggleGroupEnabled.ActiveToggles())
        {
            bIsEnabled = (t.name == "Yes");
        }

        //Unit
        foreach (Toggle t in toggleGroupUnit.ActiveToggles())
        {
            multiTool.MEASURE = (MultiTool3D.MultiTool3D.MEASUREMENT)(t.transform.GetSiblingIndex());
        }

        //Tool
        foreach (Toggle t in toggleGroupTool.ActiveToggles())
        {
            switch (t.name)
            {
                case "Ruler":
                    multiTool.rulerToggle = bIsEnabled;
                    goToolRuler.SetActive(true);
                    goToolAngle.SetActive(false);
                    goToolVolume.SetActive(false);

                    tmpRulerFrom[0].text = FormatFloat(multiTool.ruler.from.x);
                    tmpRulerFrom[1].text = FormatFloat(multiTool.ruler.from.y);
                    tmpRulerFrom[2].text = FormatFloat(multiTool.ruler.from.z);
                    tmpRulerTo[0].text = FormatFloat(multiTool.ruler.to.x);
                    tmpRulerTo[1].text = FormatFloat(multiTool.ruler.to.y);
                    tmpRulerTo[2].text = FormatFloat(multiTool.ruler.to.z);
                    break;

                case "Angle":
                    multiTool.angleToggle = bIsEnabled;
                    goToolRuler.SetActive(false);
                    goToolAngle.SetActive(true);
                    goToolVolume.SetActive(false);

                    tmpAngleBase[0].text = FormatFloat(multiTool.angle.from.x);
                    tmpAngleBase[1].text = FormatFloat(multiTool.angle.from.y);
                    tmpAngleBase[2].text = FormatFloat(multiTool.angle.from.z);
                    tmpAnglePoint1[0].text = FormatFloat(multiTool.angle.p1.x);
                    tmpAnglePoint1[1].text = FormatFloat(multiTool.angle.p1.y);
                    tmpAnglePoint1[2].text = FormatFloat(multiTool.angle.p1.z);
                    tmpAnglePoint2[0].text = FormatFloat(multiTool.angle.p2.x);
                    tmpAnglePoint2[1].text = FormatFloat(multiTool.angle.p2.y);
                    tmpAnglePoint2[2].text = FormatFloat(multiTool.angle.p2.z);

                    break;

                case "Volume":
                    multiTool.volumeToggle = bIsEnabled;
                    goToolRuler.SetActive(false);
                    goToolAngle.SetActive(false);
                    goToolVolume.SetActive(true);

                    if (toggleModeBox.isOn)
                    {
                        multiTool.VMODE = MultiTool3D.MultiTool3D.VOLUME_MODE.Box;

                        goToolVolumeBox.SetActive(true);
                        goToolVolumeSphere.SetActive(false);
                        goToolVolumePlane.SetActive(false);

                        tmpVolumeBoxCenter[0].text = FormatFloat(multiTool.volume.bounds.center.x);
                        tmpVolumeBoxCenter[1].text = FormatFloat(multiTool.volume.bounds.center.y);
                        tmpVolumeBoxCenter[2].text = FormatFloat(multiTool.volume.bounds.center.z);
                        tmpVolumeBoxSize[0].text = FormatFloat(multiTool.volume.bounds.size.x);
                        tmpVolumeBoxSize[1].text = FormatFloat(multiTool.volume.bounds.size.y);
                        tmpVolumeBoxSize[2].text = FormatFloat(multiTool.volume.bounds.size.z);
                    }
                    else if (toggleModeSphere.isOn)
                    {
                        multiTool.VMODE = MultiTool3D.MultiTool3D.VOLUME_MODE.Sphere;

                        goToolVolumeBox.SetActive(false);
                        goToolVolumeSphere.SetActive(true);
                        goToolVolumePlane.SetActive(false);

                        tmpVolumeSphereRadius.text = FormatFloat(multiTool.volume.radius);
                    }
                    else
                    {
                        multiTool.VMODE = MultiTool3D.MultiTool3D.VOLUME_MODE.Plane;

                        goToolVolumeBox.SetActive(false);
                        goToolVolumeSphere.SetActive(false);
                        goToolVolumePlane.SetActive(true);

                        tmpVolumePlanePoint1[0].text = FormatFloat(multiTool.volume.p1.x);
                        tmpVolumePlanePoint1[1].text = FormatFloat(multiTool.volume.p1.y);
                        tmpVolumePlanePoint1[2].text = FormatFloat(multiTool.volume.p1.z);
                        tmpVolumePlanePoint2[0].text = FormatFloat(multiTool.volume.p2.x);
                        tmpVolumePlanePoint2[1].text = FormatFloat(multiTool.volume.p2.y);
                        tmpVolumePlanePoint2[2].text = FormatFloat(multiTool.volume.p2.z);
                        tmpVolumePlanePoint3[0].text = FormatFloat(multiTool.volume.p3.x);
                        tmpVolumePlanePoint3[1].text = FormatFloat(multiTool.volume.p3.y);
                        tmpVolumePlanePoint3[2].text = FormatFloat(multiTool.volume.p3.z);
                        tmpVolumePlanePoint4[0].text = FormatFloat(multiTool.volume.p4.x);
                        tmpVolumePlanePoint4[1].text = FormatFloat(multiTool.volume.p4.y);
                        tmpVolumePlanePoint4[2].text = FormatFloat(multiTool.volume.p4.z);
                    }
                    break;
            }

            UpdateTypedValueUI();

            if (!goKeyboard.activeSelf && selectedIndex >= 0)
            {
                SubmitTypedValue();
                valueToggles[selectedIndex].isOn = false;
                selectedIndex = -1;
            }
        }
    }

    public void SelectTool(int index)
    {
        bool bIsEnabled = false;
        MultiTool3D.MultiTool3D.VOLUME_MODE volumeMode = MultiTool3D.MultiTool3D.VOLUME_MODE.Box;

        switch (index)
        {
            case 0:
                bIsEnabled = multiTool.rulerToggle;
                break;

            case 1:
                bIsEnabled = multiTool.angleToggle;
                break;

            case 2:
                bIsEnabled = multiTool.volumeToggle;

                volumeMode = multiTool.VMODE;         

                break;
        }

        toggleGroupEnabled.SetAllTogglesOff();
        toggleEnabledYes.isOn = bIsEnabled;
        toggleEnabledNo.isOn = !bIsEnabled;
        toggleModeBox.isOn = volumeMode == MultiTool3D.MultiTool3D.VOLUME_MODE.Box;
        toggleModeSphere.isOn = volumeMode == MultiTool3D.MultiTool3D.VOLUME_MODE.Sphere;
        toggleModePlane.isOn = volumeMode == MultiTool3D.MultiTool3D.VOLUME_MODE.Plane;
    }

    private string FormatFloat(float f)
    {
        return f.ToString("F2");
    }


    public void SelectTMP(int index)
    {
        if (index <= 0) return;

        if (selectedIndex == index) return;

        if (!valueToggles[index].isOn)
            return;

        if (selectedIndex != index && selectedIndex >= 0)
        {
            SubmitTypedValue();
            valueToggles[selectedIndex].isOn = false;
        }

        selectedIndex = index;
        goKeyboard.SetActive(true);
        //valueToggles[selectedIndex].isOn = true;

        switch (selectedIndex)
        {
            case 0:
                typingValue = tmpRulerFrom[0].text;
                break;
            case 1:
                typingValue = tmpRulerFrom[1].text;
                break;
            case 2:
                typingValue = tmpRulerFrom[2].text;
                break;
            case 3:
                typingValue = tmpRulerTo[0].text;
                break;
            case 4:
                typingValue = tmpRulerTo[1].text;
                break;
            case 5:
                typingValue = tmpRulerTo[2].text;
                break;
            case 6:
                typingValue = tmpAngleBase[0].text;
                break;
            case 7:
                typingValue = tmpAngleBase[1].text;
                break;
            case 8:
                typingValue = tmpAngleBase[2].text;
                break;
            case 9:
                typingValue = tmpAnglePoint1[0].text;
                break;
            case 10:
                typingValue = tmpAnglePoint1[1].text;
                break;
            case 11:
                typingValue = tmpAnglePoint1[2].text;
                break;
            case 12:
                typingValue = tmpAnglePoint2[0].text;
                break;
            case 13:
                typingValue = tmpAnglePoint2[1].text;
                break;
            case 14:
                typingValue = tmpAnglePoint2[2].text;
                break;
            case 15:
                typingValue = tmpVolumeBoxCenter[0].text;
                break;
            case 16:
                typingValue = tmpVolumeBoxCenter[1].text;
                break;
            case 17:
                typingValue = tmpVolumeBoxCenter[2].text;
                break;
            case 18:
                typingValue = tmpVolumeBoxSize[0].text;
                break;
            case 19:
                typingValue = tmpVolumeBoxSize[1].text;
                break;
            case 20:
                typingValue = tmpVolumeBoxSize[2].text;
                break;
            case 21:
                typingValue = tmpVolumeSphereRadius.text;
                break;
            case 22:
                typingValue = tmpVolumePlanePoint1[0].text;
                break;
            case 23:
                typingValue = tmpVolumePlanePoint1[1].text;
                break;
            case 24:
                typingValue = tmpVolumePlanePoint1[2].text;
                break;
            case 25:
                typingValue = tmpVolumePlanePoint2[0].text;
                break;
            case 26:
                typingValue = tmpVolumePlanePoint2[1].text;
                break;
            case 27:
                typingValue = tmpVolumePlanePoint2[2].text;
                break;
            case 28:
                typingValue = tmpVolumePlanePoint3[0].text;
                break;
            case 29:
                typingValue = tmpVolumePlanePoint3[1].text;
                break;
            case 30:
                typingValue = tmpVolumePlanePoint3[2].text;
                break;
            case 31:
                typingValue = tmpVolumePlanePoint4[0].text;
                break;
            case 32:
                typingValue = tmpVolumePlanePoint4[1].text;
                break;
            case 33:
                typingValue = tmpVolumePlanePoint4[2].text;
                break;
        }
    }

    public void TypeValue(string value)
    {
        typingValue += value;
        UpdateTypedValueUI();
    }

    public void TypeDelete()
    {
        typingValue = typingValue.Substring(0, Mathf.Max(typingValue.Length - 1, 0));
        UpdateTypedValueUI();
    }

    private void UpdateTypedValueUI()
    {
        switch (selectedIndex)
        {
            case 0:
                tmpRulerFrom[0].text = typingValue;
                break;
            case 1:
                tmpRulerFrom[1].text = typingValue;
                break;
            case 2:
                tmpRulerFrom[2].text = typingValue;
                break;
            case 3:
                tmpRulerTo[0].text  = typingValue;
                break;
            case 4:
                tmpRulerTo[1].text  = typingValue;
                break;
            case 5:
                tmpRulerTo[2].text  = typingValue;
                break;
            case 6:
                tmpAngleBase[0].text  = typingValue;
                break;
            case 7:
                tmpAngleBase[1].text  = typingValue;
                break;
            case 8:
                tmpAngleBase[2].text  = typingValue;
                break;
            case 9:
                tmpAnglePoint1[0].text  = typingValue;
                break;
            case 10:
                tmpAnglePoint1[1].text  = typingValue;
                break;
            case 11:
                tmpAnglePoint1[2].text  = typingValue;
                break;
            case 12:
                tmpAnglePoint2[0].text  = typingValue;
                break;
            case 13:
                tmpAnglePoint2[1].text  = typingValue;
                break;
            case 14:
                tmpAnglePoint2[2].text  = typingValue;
                break;
            case 15:
                tmpVolumeBoxCenter[0].text  = typingValue;
                break;
            case 16:
                tmpVolumeBoxCenter[1].text  = typingValue;
                break;
            case 17:
                tmpVolumeBoxCenter[2].text  = typingValue;
                break;
            case 18:
                tmpVolumeBoxSize[0].text  = typingValue;
                break;
            case 19:
                tmpVolumeBoxSize[1].text  = typingValue;
                break;
            case 20:
                tmpVolumeBoxSize[2].text  = typingValue;
                break;
            case 21:
                tmpVolumeSphereRadius.text  = typingValue;
                break;
            case 22:
                tmpVolumePlanePoint1[0].text  = typingValue;
                break;
            case 23:
                tmpVolumePlanePoint1[1].text  = typingValue;
                break;
            case 24:
                tmpVolumePlanePoint1[2].text  = typingValue;
                break;
            case 25:
                tmpVolumePlanePoint2[0].text  = typingValue;
                break;
            case 26:
                tmpVolumePlanePoint2[1].text  = typingValue;
                break;
            case 27:
                tmpVolumePlanePoint2[2].text  = typingValue;
                break;
            case 28:
                tmpVolumePlanePoint3[0].text  = typingValue;
                break;
            case 29:
                tmpVolumePlanePoint3[1].text  = typingValue;
                break;
            case 30:
                tmpVolumePlanePoint3[2].text  = typingValue;
                break;
            case 31:
                tmpVolumePlanePoint4[0].text  = typingValue;
                break;
            case 32:
                tmpVolumePlanePoint4[1].text  = typingValue;
                break;
            case 33:
                tmpVolumePlanePoint4[2].text  = typingValue;
                break;
        }
    }

    public void SubmitTypedValue()
    {
        float value = 0f;

        if (float.TryParse(typingValue, out value))
        {
            switch (selectedIndex)
            {
                case 0:
                    multiTool.tRulerFrom.position = new Vector3(value, multiTool.ruler.from.y, multiTool.ruler.from.z);
                    break;
                case 1:
                    multiTool.tRulerFrom.position = new Vector3(multiTool.ruler.from.x, value, multiTool.ruler.from.z);
                    break;
                case 2:
                    multiTool.tRulerFrom.position = new Vector3(multiTool.ruler.from.x, multiTool.ruler.from.y, value);
                    break;
                case 3:
                    multiTool.tRulerTo.position = new Vector3(value, multiTool.ruler.to.y, multiTool.ruler.to.z);
                    break;
                case 4:
                    multiTool.tRulerTo.position = new Vector3(multiTool.ruler.to.x, value, multiTool.ruler.to.z);
                    break;
                case 5:
                    multiTool.tRulerTo.position = new Vector3(multiTool.ruler.to.x, multiTool.ruler.to.y, value);
                    break;
                case 6:
                    multiTool.tAngleFrom.position = new Vector3(value, multiTool.angle.from.y, multiTool.angle.from.z);
                    break;
                case 7:
                    multiTool.tAngleFrom.position = new Vector3(multiTool.angle.from.x, value, multiTool.angle.from.z);
                    break;
                case 8:
                    multiTool.tAngleFrom.position = new Vector3(multiTool.angle.from.x, multiTool.angle.from.y, value);
                    break;
                case 9:
                    multiTool.tAnglePoint1.position = new Vector3(value, multiTool.angle.p1.y, multiTool.angle.p1.z);
                    break;
                case 10:
                    multiTool.tAnglePoint1.position = new Vector3(multiTool.angle.p1.x, value, multiTool.angle.p1.z);
                    break;
                case 11:
                    multiTool.tAnglePoint1.position = new Vector3(multiTool.angle.p1.x, multiTool.angle.p1.y, value);
                    break;
                case 12:
                    multiTool.tAnglePoint2.position = new Vector3(value, multiTool.angle.p1.y, multiTool.angle.p1.z);
                    break;
                case 13:
                    multiTool.tAnglePoint2.position = new Vector3(multiTool.angle.p2.x, value, multiTool.angle.p2.z);
                    break;
                case 14:
                    multiTool.tAnglePoint2.position = new Vector3(multiTool.angle.p2.x, multiTool.angle.p2.y, value);
                    break;
                case 15:
                    multiTool.tVolumeCube.position = new Vector3(value, multiTool.volume.bounds.center.y, multiTool.volume.bounds.center.z);
                    break;
                case 16:
                    multiTool.tVolumeCube.position = new Vector3(multiTool.volume.bounds.center.x, value, multiTool.volume.bounds.center.z);
                    break;
                case 17:
                    multiTool.tVolumeCube.position = new Vector3(multiTool.volume.bounds.center.x, multiTool.volume.bounds.center.y, value);
                    break;
                case 18:
                    multiTool.tVolumeCube.localScale = new Vector3(value, multiTool.volume.bounds.size.y, multiTool.volume.bounds.size.z);
                    break;
                case 19:
                    multiTool.tVolumeCube.localScale = new Vector3(multiTool.volume.bounds.size.x, value, multiTool.volume.bounds.size.z);
                    break;
                case 20:
                    multiTool.tVolumeCube.localScale = new Vector3(multiTool.volume.bounds.size.x, multiTool.volume.bounds.size.y, value);
                    break;
                case 21:
                    multiTool.tVolumeSphereRadius.localScale = Vector3.one * value;
                    break;
                case 22:
                    multiTool.tVolumePlanePoint1.position = new Vector3(value, multiTool.volume.p1.y, multiTool.volume.p1.z);
                    break;
                case 23:
                    multiTool.tVolumePlanePoint1.position = new Vector3(multiTool.volume.p1.x, value, multiTool.volume.p1.z);
                    break;
                case 24:
                    multiTool.tVolumePlanePoint1.position = new Vector3(multiTool.volume.p1.x, multiTool.volume.p1.y, value);
                    break;
                case 25:
                    multiTool.tVolumePlanePoint2.position = new Vector3(value, multiTool.volume.p2.y, multiTool.volume.p2.z);
                    break;
                case 26:
                    multiTool.tVolumePlanePoint2.position = new Vector3(multiTool.volume.p2.x, value, multiTool.volume.p2.z);
                    break;
                case 27:
                    multiTool.tVolumePlanePoint2.position = new Vector3(multiTool.volume.p2.x, multiTool.volume.p2.y, value);
                    break;
                case 28:
                    multiTool.tVolumePlanePoint3.position = new Vector3(value, multiTool.volume.p3.y, multiTool.volume.p3.z);
                    break;
                case 29:
                    multiTool.tVolumePlanePoint3.position = new Vector3(multiTool.volume.p3.x, value, multiTool.volume.p3.z);
                    break;
                case 30:
                    multiTool.tVolumePlanePoint3.position = new Vector3(multiTool.volume.p3.x, multiTool.volume.p3.y, value);
                    break;
                case 31:
                    multiTool.tVolumePlanePoint4.position = new Vector3(value, multiTool.volume.p4.y, multiTool.volume.p4.z);
                    break;
                case 32:
                    multiTool.tVolumePlanePoint4.position = new Vector3(multiTool.volume.p4.x, value, multiTool.volume.p4.z);
                    break;
                case 33:
                    multiTool.tVolumePlanePoint4.position = new Vector3(multiTool.volume.p4.x, multiTool.volume.p4.y, value);
                    break;
            }
        }
    }
}
