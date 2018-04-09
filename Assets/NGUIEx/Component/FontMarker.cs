using System;
using UnityEngine;

using System.Collections.Generic;
using commons;

namespace ngui.ex
{
    public class FontMarker : MonoBehaviour
    {
        public UILabel label;
        public string fontName;
        
        void Start()
        {
            if (label != null && label.bitmapFont == null && fontName.IsNotEmpty())
            {
                FontLoader.ApplyFont(this);
            }
        }
    }
}
