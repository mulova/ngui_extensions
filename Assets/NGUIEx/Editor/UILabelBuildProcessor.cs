
using UnityEngine;
using UnityEditor;
using build;
using comunity;
using commons;


namespace ngui.ex
{
    public class UILabelBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp)
        {
        }
        
        protected override void PreprocessComponent(Component comp)
        {
            UILabel label = comp as UILabel;
            UIFont font = label.bitmapFont;
            if (font == null)
            {
                return;
            }
            if (isCdnAsset)
            {
                if (font.replacement != null)
                {
                    // get singleton prefab
                    GameObject singleton = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Reskin/singletons.prefab");
                    FontLoader loader = singleton.GetComponentEx<FontLoader>();
                    AddFont(loader, font);
                    label.bitmapFont = null;
                    BuildScript.SetDirty(label);
                }
                // add font marker to restore
                FontMarker marker = label.GetComponentEx<FontMarker>();
                marker.label = label;
                marker.fontName = font.name;
                BuildScript.SetDirty(marker);
            }
        }
        
        private void AddFont(FontLoader loader, UIFont font)
        {
            foreach (FontLoader.FontPair f in loader.fonts)
            {
                if (f.dst == font)
                {
                    return;
                }
            }
            FontLoader.FontPair pair = new FontLoader.FontPair();
            pair.dst = font;
            pair.asset = new AssetRef();
            pair.asset.SetPath(font.replacement);
            pair.asset.reference = font.replacement.gameObject;
            loader.fonts = loader.fonts.Add(pair);
            BuildScript.SetDirty(loader);
        }
        
        protected override void PreprocessOver(Component c)
        {
        }
        
        public override System.Type compType
        {
            get
            {
                return typeof(UILabel);
            }
        }
    }

}
