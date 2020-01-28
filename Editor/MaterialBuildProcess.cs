using System;
using mulova.commons;
using mulova.preprocess;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ngui.ex
{
    public class MaterialBuildProcess : AssetBuildProcess
    {
        private RegexMgr excludeShader = new RegexMgr();

        public override Type assetType => typeof(Material);
        public override string title => "Missing texture";

        public MaterialBuildProcess()
        {
            excludeShader.SetPattern("ShurikenMagic|Standard");
        }
        
        protected override void Preprocess(string path, Object obj)
        {
        }
        
        protected override void Verify(string path, Object obj)
        {
            // TODOM
            //            Material m = obj as Material;
            //            if (m.mainTexture == null)
            //            {
            //                if (!excludeShader.IsMatch(m.shader.name))
            //                {
            //                    AddErrorFormat("{0}({1})", path, m.name);
            //                }
            //            }
        }

        protected override void Postprocess(string path, Object obj)
        {
        }
    }
}
