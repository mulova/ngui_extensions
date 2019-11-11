using mulova.unicore;
using UnityEditor;

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
                UITabHandler tab = popup.GetComponentInChildren<UITabHandler>();
                if (tab != null) {
                    popup.tabs = tab;
                    EditorUtil.SetDirty(popup);
                }
            }
        }
        
        public override void OnInspectorGUI() {
            inspector.OnInspectorGUI();
        }
    }
}
