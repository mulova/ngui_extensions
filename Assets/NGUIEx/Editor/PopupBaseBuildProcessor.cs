using mulova.preprocess;
using UnityEngine;

namespace ngui.ex
{
    public class PopupBaseBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(PopupBase);

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
    }

}

