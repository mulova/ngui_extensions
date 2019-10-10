using mulova.comunity;
using mulova.preprocess;
using UnityEngine;

namespace ngui.ex
{
    public class UIAtlasBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(UIAtlas);

        protected override void Verify(Component comp)
        {
        }
        
        protected override void Preprocess(Component comp)
        {
            UIAtlas atlas = comp as UIAtlas;
            if (atlas.spriteMaterial == null)
            {
                return;
            }
            if (atlas.spriteMaterial.shader.name == ShaderId.UI)
            {
                TexFormatGroup format = GetOption<TexFormatGroup>();
                if (format == TexFormatGroup.ETC || format == TexFormatGroup.PVRTC || format == TexFormatGroup.AUTO)
                {
                    UIAtlasMenu.SplitChannelETC(atlas);
                }
            }
        }
        
        protected override void Postprocess(Component c)
        {
        }
    }
}