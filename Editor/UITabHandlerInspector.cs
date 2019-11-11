using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System.Collections.Generic;

namespace ngui.ex
{
    [CustomEditor(typeof(UITabHandler))]
    public class UITabHandlerInspector : Editor
    {
        private UITabHandlerInspectorImpl inspector;
        
        void OnEnable() {
            inspector = new UITabHandlerInspectorImpl(target as UITabHandler);
        }
        
        public override void OnInspectorGUI() {
            inspector.OnInspectorGUI();
        }
    }
    
}