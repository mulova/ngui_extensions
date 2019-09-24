using mulova.build;
using UnityEngine;


namespace ngui.ex
{
    public class FontLoaderBuildProcessor : ComponentBuildProcess
    {
        
        protected override void VerifyComponent(Component comp)
        {
        }
        
        protected override void PreprocessComponent(Component comp)
        {
            FontLoader l = comp as FontLoader;
            if  (!l.isActiveAndEnabled)
            {
                return;
            }
            foreach (FontLoader.FontPair f in l.fonts)
            {
                f.dst.replacement = null;
                BuildScript.SetDirty(f.dst);
            }
        }
        
        protected override void PreprocessOver(Component c)
        {
        }
        
        public override System.Type compType
        {
            get
            {
                return typeof(FontLoader);
            }
        }
    }
}
