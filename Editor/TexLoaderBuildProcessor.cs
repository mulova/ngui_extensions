using System.Text.Ex;
using mulova.build;
using mulova.unicore;
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

        protected override void Verify(Component comp)
        {
        }

        protected override void Preprocess(Component comp)
        {
            TexLoader l = comp as TexLoader;
            if (l.Target != null && l.Target.mainTexture != null)
            {
                ClearTexture(l);
            }
        }

        protected override void Postprocess(Component c)
        {
        }


        public static bool ClearTexture(TexLoader l)
        {
            if (l.Target != null && l.Target.mainTexture != null)
            {
                l.editorTexPath = UnityEditor.AssetDatabase.GetAssetPath(l.Target.mainTexture);
                l.Target.mainTexture = null;
                EditorUtil.SetDirty(l.Target);
                return true;
            }
            return false;
        }
        
        public static void RestoreTexture(TexLoader l)
        {
            if (l.Target != null && !l.editorTexPath.IsEmpty())
            {
                l.Target.mainTexture = UnityEditor.AssetDatabase.LoadAssetAtPath(l.editorTexPath, typeof(Texture)) as Texture;
                EditorUtil.SetDirty(l.gameObject);
            }
        }
    }
}
