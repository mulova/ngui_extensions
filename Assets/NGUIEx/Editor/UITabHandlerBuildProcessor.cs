
using UnityEngine;
using UnityEditor;
using mulova.build;
using mulova.commons;
using mulova.comunity;
using UnityEngine.Ex;


namespace ngui.ex
{

    /// <summary>
    /// Clear texture in UITexture
    /// Add TexSetter if texture is from CDN
    /// </summary>
    public class UITabHandlerBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp)
        {
        }
        
        protected override void PreprocessComponent(Component comp)
        {
            UITabHandler h = comp as UITabHandler;
            if (h.tabs.IsNotEmpty())
            {
                foreach (var t in h.tabs)
                {
                    t.uiRoot.SetActiveEx(false);
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
                return typeof(UITabHandler);
            }
        }
    }
    
}