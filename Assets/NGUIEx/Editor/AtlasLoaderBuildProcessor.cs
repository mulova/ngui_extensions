using mulova.build;
using mulova.comunity;
using mulova.preprocess;
using UnityEditor;
using UnityEngine;

namespace ngui.ex
{
    public class AtlasLoaderBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(AtlasLoader);

        protected override void VerifyComponent(Component comp)
        {
            AtlasLoader l = comp as AtlasLoader;
            foreach (AtlasLoader.AtlasPair a in l.atlases)
            {
                if (a.dst.replacement != null)
                {
                    if (AssetBundlePath.inst.IsCdnAsset(a.dst.replacement.gameObject))
                    {
                        a.dst.replacement = null;
                        BuildScript.SetDirty(a.dst);
                    } else
                    {
                        log.LogFormat("The reference of {0} is not CDN asset", AssetDatabase.GetAssetPath(a.dst.replacement.gameObject));
                    }
                }
            }
        }
        
        protected override void PreprocessComponent(Component comp)
        {
        }
        
        protected override void PreprocessOver(Component c)
        {
        }
    }

}

