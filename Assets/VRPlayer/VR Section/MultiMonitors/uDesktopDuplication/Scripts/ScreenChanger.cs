using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uDesktopDuplication
{

    public class ScreenChanger : MonoBehaviour
    {
        public int id;

        private Texture _texture;
        private void Awake()
        {
            _texture = GetComponent<Texture>();
            _texture.monitorId = id;
        }
    }
}