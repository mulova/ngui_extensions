
using UnityEngine;
using mulova.build;
using mulova.comunity;


namespace ngui.ex
{
    public class UIAtlasBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp)
        {
        }
        
        protected override void PreprocessComponent(Component comp)
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
        
        protected override void PreprocessOver(Component c)
        {
        }
        
        public override System.Type compType
        {
            get
            {
                return typeof(UIAtlas);
            }
        }
    }
}