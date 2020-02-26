using mulova.build;
using mulova.build;
using mulova.unicore;
using UnityEngine;


namespace ngui.ex
{
    public class FontLoaderBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(FontLoader);

        protected override void Verify(Component comp)
        {
        }
        
        protected override void Preprocess(Component comp)
        {
            FontLoader l = comp as FontLoader;
            if  (!l.isActiveAndEnabled)
            {
                return;
            }
            foreach (FontLoader.FontPair f in l.fonts)
            {
                f.dst.replacement = null;
                EditorUtil.SetDirty(f.dst);
            }
        }
        
        protected override void Postprocess(Component c)
        {
        }
    }
}
