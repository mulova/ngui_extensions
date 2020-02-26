using mulova.build;
using UnityEngine;

namespace ngui.ex
{
    public class PopupBaseBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(PopupBase);

        protected override void Verify(Component comp)
        {
            PopupBase p = comp as PopupBase;
            p.window.ui.SetActive(false);
        }
        
        protected override void Preprocess(Component comp)
        {
        }
        
        protected override void Postprocess(Component c)
        {
        }
    }

}

