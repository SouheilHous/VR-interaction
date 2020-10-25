using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DataPackage
{
    public List<DataContent> DataContentList { get; set; }
    
    public string InstanceID { get; set; }
    public string LastUpdateTimestamp { get; set; }
    public string CreateTimestamp { get; set; }
}
