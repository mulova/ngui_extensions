using mulova.preprocess;
using UnityEngine;

namespace ngui.ex
{
    public class UITableLayoutBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(UITableLayout);

        protected override void VerifyComponent(Component comp)
        {
        }
        
        protected override void PreprocessComponent(Component comp)
        {
            UITableLayout layout = comp as UITableLayout;
            Deactivate(layout.rowPrefab);
            Deactivate(layout.columnPrefab);
            Deactivate(layout.defaultPrefab);
        }
        
        protected override void PreprocessOver(Component c)
        {
        }

        static void Deactivate(params UITableCell[] objs)
        {
            if (objs != null)
            {
                foreach (var o in objs)
                {
                    if (o != null)
                    {
                        o.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
