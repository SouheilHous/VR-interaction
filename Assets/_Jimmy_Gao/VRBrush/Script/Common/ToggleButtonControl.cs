using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JimmyGao
{
    public class ToggleButtonControl : MonoBehaviour
    {
        public Sprite ChcekedBG;
        public Sprite UnChcekedBG;

        public int brushMode;

        [SerializeField]
        public void DoCheck(bool value)
        {
            _checked = value;
            if (_checked)
            {
                this.GetComponent<Image>().sprite = ChcekedBG;
                
            }
            else
            {
                this.GetComponent<Image>().sprite = UnChcekedBG;

            }
        }


        bool _checked;
        public Color ToggleColor;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}