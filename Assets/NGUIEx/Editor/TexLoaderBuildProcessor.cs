using System.Text.Ex;
using mulova.build;
using mulova.preprocess;
using UnityEngine;

namespace ngui.ex
{
    /// <summary>
    /// Clear texture in UITexture
    /// Add TexSetter if texture is from CDN
    /// </summary>
    public class TexLoaderBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(TexLoader);

        protected override void VerifyComponent(Component comp)
        {
        }

        protected override void PreprocessComponent(Component comp)
        {
            TexLoader l = comp as TexLoader;
            if (l.Target != null && l.Target.mainTexture != null)
            {
                ClearTexture(l);
            }
        }

        protected override void PreprocessOver(Component c)
        {
        }


        public static bool ClearTexture(TexLoader l)
        {
            if (l.Target != null && l.Target.mainTexture != null)
            {
                l.editorTexPath = UnityEditor.AssetDatabase.GetAssetPath(l.Target.mainTexture);
                l.Target.mainTexture = null;
                BuildScript.SetDirty(l.Target);
                return true;
            }
            return false;
        }
        
        public static void RestoreTexture(TexLoader l)
        {
            if (l.Target != null && !l.editorTexPath.IsEmpty())
            {
                l.Target.mainTexture = UnityEditor.AssetDatabase.LoadAssetAtPath(l.editorTexPath, typeof(Texture)) as Texture;
                BuildScript.SetDirty(l.gameObject);
            }
        }
    }
}
