using UnityEngine;

using UnityEditor;
using ngui.ex;
using build;

namespace ngui.ex
{
    public class UIGridLayoutBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp)
        {
        }
        
        protected override void PreprocessComponent(Component comp)
        {
            UIGridLayout layout = comp as UIGridLayout;
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
                return typeof(UIGridLayout);
            }
        }
        
        static void Deactivate(params GameObject[] objs)
        {
            if (objs != null)
            {
                foreach (GameObject p in objs)
                {
                    if (p != null)
                    {
                        p.SetActive(false);
                    }
                }
            }
        }
    }
}
