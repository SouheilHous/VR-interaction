using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DataContent
{
    public string ObjInstanceId { get; set; }

    public string DrawdingTypeData { get; set; }
    public string DrawdingStyleIdData { get; set; }
    public string ColorIDData { get; set; }
    public string ParticleStyleIdData { get; set; }
    public string AnimationXData { get; set; }
    public string AnimationYData { get; set; }

    public string NodeCount { get; set; }
    public string PositionData { get; set; }
    public string RotationData { get; set; }
    public string ScaleData { get; set; }
    public string WidthData { get; set; }
}