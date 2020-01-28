using mulova.comunity;
using mulova.preprocess;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using System.Text.Ex;

namespace ngui.ex
{
    public class AtlasLoaderBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(AtlasLoader);

        protected override void Verify(Component comp)
        {
            AtlasLoader l = comp as AtlasLoader;
            foreach (AtlasLoader.AtlasPair a in l.atlases)
            {
                if (a.dst.replacement != null)
                {
                    if (AssetBundlePath.inst.IsCdnAsset(a.dst.replacement.gameObject))
                    {
                        a.dst.replacement = null;
                        EditorUtil.SetDirty(a.dst);
                    } else
                    {
                        log.Log("The reference of {0} is not CDN asset".Format(AssetDatabase.GetAssetPath(a.dst.replacement.gameObject)));
                    }
                }
            }
        }
        
        protected override void Preprocess(Component comp)
        {
        }
        
        protected override void Postprocess(Component c)
        {
        }
    }

}

