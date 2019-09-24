using System.Text.Ex;
using mulova.commons;
using UnityEngine;

namespace ngui.ex
{
    public class FontMarker : MonoBehaviour
    {
        public UILabel label;
        public string fontName;
        
        void Start()
        {
            if (label != null && label.bitmapFont == null && !fontName.IsEmpty())
            {
                FontLoader.ApplyFont(this);
            }
        }
    }
}
