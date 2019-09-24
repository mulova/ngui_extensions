using System.Collections.Generic;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace ngui.ex
{
    public class UIWindowInspectorImpl
    {
        private UIWindow window;
        private SerializedInspector varInspector;
        private AnimClipInspector animClipInspector;
        public const string PANEL_EVENT_ID = "event/panel_event_id";
        
        public UIWindowInspectorImpl(UIWindow window) {
            this.window = window;
            this.varInspector = new SerializedInspector(new SerializedObject(window), "ui", "alpha");
            this.animClipInspector = new AnimClipInspector(window, "showClip", "hideClip");
            if (window.ui == null) {
                Transform uiTrans = window.transform.Find("ui");
                
                if (uiTrans != null) {
                    window.ui = uiTrans.gameObject;
                } else {
                    List<Transform> children = new List<Transform>();
                    foreach (Transform t in window.transform) {
                        children.Add(t);
                    }
                    window.ui = new GameObject("ui");
                    window.ui.transform.SetParent(window.transform, false);
                    foreach (Transform t in children) {
                        t.parent = window.ui.transform;
                    }
                }
                EditorUtil.SetDirty(window);
            }
        }
        
        public void OnInspectorGUI() {
            bool changed = varInspector.OnInspectorGUI();
            changed |= animClipInspector.OnInspectorGUI();
            if (NGUIEditorTools.DrawHeader("Panel Event")) {
                NGUIEditorTools.BeginContents();
                NGUIEditorTools.DrawEvents("OnInit", window, window.onInit);
                NGUIEditorTools.DrawEvents("OnShowBegin", window, window.onShowBegin);
                NGUIEditorTools.DrawEvents("OnShowBegun", window, window.onShowBegun);
                NGUIEditorTools.DrawEvents("OnShowEnd", window, window.onShowEnd);
                NGUIEditorTools.DrawEvents("OnHideBegin", window, window.onHideBegin);
                NGUIEditorTools.DrawEvents("OnHideEnd", window, window.onHideEnd);
                NGUIEditorTools.EndContents();
            }
            if (changed) {
                EditorUtil.SetDirty(window);
            }
        }
    }
}
