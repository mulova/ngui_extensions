using UnityEngine;
using System.Collections;

using UnityEditor;
using build;

namespace ngui.ex
{
    public class PopupBaseBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp)
        {
            PopupBase p = comp as PopupBase;
            p.window.ui.SetActive(false);
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
                return typeof(PopupBase);
            }
        }
    }

}

