using mulova.build;
using mulova.commons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ngui.ex
{
    public class MaterialBuildProcess : AssetBuildProcess
    {
        private RegexMgr excludeShader = new RegexMgr();
        
        public MaterialBuildProcess() : base("Missing texture", typeof(Material))
        {
            excludeShader.SetPattern("ShurikenMagic|Standard");
        }
        
        protected override void PreprocessAsset(string path, Object obj)
        {
        }
        
        protected override void VerifyAsset(string path, Object obj)
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
    }
}
