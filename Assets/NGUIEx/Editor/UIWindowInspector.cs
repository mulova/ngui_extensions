using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System.Collections.Generic;
using comunity;

namespace ngui.ex
{
    [CustomEditor(typeof(UIWindow))]
    public class UIWindowInspector : Editor
    {
        private UIWindowInspectorImpl inspector;
        
        void OnEnable() {
            inspector = new UIWindowInspectorImpl(target as UIWindow);
            UIWindow win = target as UIWindow;
            PopupBase popup = win.GetComponent<PopupBase>();
            if (popup != null) {
                UITabHandler tab = popup.GetComponentInChildrenEx<UITabHandler>();
                if (tab != null) {
                    popup.tabs = tab;
                    CompatibilityEditor.SetDirty(popup);
                }
            }
        }
        
        public override void OnInspectorGUI() {
            inspector.OnInspectorGUI();
        }
    }
}
