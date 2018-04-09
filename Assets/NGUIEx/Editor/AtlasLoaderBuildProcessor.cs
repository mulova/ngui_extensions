using UnityEngine;
using System.Collections;
using UnityEditor;
using build;
using comunity;

namespace ngui.ex
{
    public class AtlasLoaderBuildProcessor : ComponentBuildProcess
    {
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
                        AddErrorFormat("The reference of {0} is not CDN asset", AssetDatabase.GetAssetPath(a.dst.replacement.gameObject));
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
        
        public override System.Type compType
        {
            get
            {
                return typeof(AtlasLoader);
            }
        }
    }

}

