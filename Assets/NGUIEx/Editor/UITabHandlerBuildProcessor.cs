using System.Collections.Generic.Ex;
using mulova.preprocess;
using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{

    /// <summary>
    /// Clear texture in UITexture
    /// Add TexSetter if texture is from CDN
    /// </summary>
    public class UITabHandlerBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(UITabHandler);

        protected override void Verify(Component comp)
        {
        }
        
        protected override void Preprocess(Component comp)
        {
            UITabHandler h = comp as UITabHandler;
            if (!h.tabs.IsEmpty())
            {
                foreach (var t in h.tabs)
                {
                    t.uiRoot.SetActiveEx(false);
                }
            }
        }
        
        protected override void Postprocess(Component c)
        {
        }
    }
    
}