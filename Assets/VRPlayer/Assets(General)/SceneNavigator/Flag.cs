using System;
using UnityEngine;

namespace SceneNavigator
{

    [Serializable]
    public class Flag
    {
        
        public string id;
        public string name;
        public string descr;
        public byte[] previewTexture;
        public Vector3 tp;
        public Vector3 tpos;
        public Quaternion tr;
        public bool to;
        public string tags;
        public float size;
        public string scene;
        public string sceneName;

    }

}