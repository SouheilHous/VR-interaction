using UnityEngine;
using System.Collections.Generic;
using System;

namespace SceneNavigator
{

    [Serializable]
    public class Flags : ScriptableObject, ISerializationCallbackReceiver
    {

        public Dictionary<string, Flag> list = new Dictionary<string, Flag>();
        public List<string> _keys = new List<string>();
        public List<Flag> _values = new List<Flag>();
        
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach(var kvp in list)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            list = new Dictionary<string, Flag>();
            for(int i = 0; i != Math.Min (_keys.Count, _values.Count); i++)
            {
                list.Add(_keys[i], _values[i]);
            }
        }

    }

}