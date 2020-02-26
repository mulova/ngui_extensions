using mulova.build;
using UnityEngine;

namespace ngui.ex
{
    public class UITableLayoutBuildProcessor : ComponentBuildProcess
    {
        public override System.Type compType => typeof(UITableLayout);

        protected override void Verify(Component comp)
        {
        }
        
        protected override void Preprocess(Component comp)
        {
            UITableLayout layout = comp as UITableLayout;
            Deactivate(layout.rowPrefab);
            Deactivate(layout.columnPrefab);
            Deactivate(layout.defaultPrefab);
        }
        
        protected override void Postprocess(Component c)
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
