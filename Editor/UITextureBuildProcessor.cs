
using mulova.build;
using mulova.comunity;
using mulova.build;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{
    public class UITextureBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(UITexture);

        protected override void Verify(Component comp)
        {
            UITexture tex = comp as UITexture;
            TexLoader l = tex.GetComponent<TexLoader>();
            if (tex.mainTexture != null && AssetBundlePath.inst.IsCdnAsset(tex.mainTexture)
                && (l == null || IsTextureMismatch(tex)))
            {
                TexSetter setter = tex.FindComponent<TexSetter>();
                var aref = new AssetRef();
                aref.cdn = true;
                aref.SetPath(tex.mainTexture);
                setter.textures.Clear();
                setter.textures.Add(aref);
                EditorUtil.SetDirty(comp.gameObject);
                EditorUtil.SetDirty(setter);
            }
        }

        // check if applied by prefab and instance has different texture
        private bool IsTextureMismatch(UITexture tex)
        {
            TexSetter s = tex.GetComponent<TexSetter>();
            if (s == null || s.textures.Count == 0)
            {
                return false;
            }
            return s.textures.Count == 1 && s.textures[0].path != EditorAssetUtil.GetAssetRelativePath(tex.mainTexture);
        }
        
        protected override void Preprocess(Component comp)
        {
        }
        
        protected override void Postprocess(Component c)
        {
        }
    }
}