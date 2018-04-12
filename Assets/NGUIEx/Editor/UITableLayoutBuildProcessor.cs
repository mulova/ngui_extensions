using UnityEngine;

using UnityEditor;
using ngui.ex;
using build;

namespace ngui.ex
{
    public class UITableLayoutBuildProcessor : ComponentBuildProcess
    {
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
        
        public override System.Type compType
        {
            get
            {
                return typeof(UITableLayout);
            }
        }
        
        static void Deactivate(params UITableCell[] objs)
        {
            if (objs != null)
            {
                foreach (var o in objs)
                {
                    if (o != null)
                    {
                        o.go.SetActive(false);
                    }
                }
            }
        }
    }
}
