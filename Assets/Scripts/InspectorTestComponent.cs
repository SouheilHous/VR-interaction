using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorTestComponent : MonoBehaviour
{
    public enum Number { One, Two, Three, Four }

    public Number enumValue;
    public int intValue;
    public float floatValue;
    public string stringValue;
    public InspectorTestComponent referenceValue;


}
