using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Ex;
using mulova.comunity;
using UnityEngine;

namespace ngui.ex
{
    public class FontLoader : MonoBehaviour
    {
        [Serializable]
        public class FontPair
        {
            public UIFont dst;
            public AssetRef asset;
        }
        
        public bool loadOnStart;
        public FontPair[] fonts;
        private static Dictionary<string, UIFont> fontsLoaded = new Dictionary<string, UIFont>();
        
        void Start()
        {
            if (loadOnStart)
            {
                Load(null);
            }
        }
        
        public void Load(Action callback)
        {
            int count = 0;
            for (int i=0; i<fonts.Length; ++i)
            {
                FontPair pair = fonts[i];
                pair.asset.LoadAsset<GameObject>(o=>{
                    UnityEngine.Object.DontDestroyOnLoad(o);
                    UIFont f = o.GetComponent<UIFont>();
                    pair.dst.replacement = f;
                    fontsLoaded[pair.dst.name] = f;
                    count++;
                    if (count == fonts.Length)
                    {
                        callback.Call();
                    }
                });
            }
        }
        
        public static void ApplyFont(FontMarker marker)
        {
            if (marker.label.bitmapFont != null)
            {
                return;
            }
            marker.label.bitmapFont = fontsLoaded.Get(marker.fontName);
        }
    }
}
