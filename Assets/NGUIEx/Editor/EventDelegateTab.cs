using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using commons;
using comunity;


namespace ngui.ex
{
    public class EventDelegateTab : EditorTab {
        
        private SortedMultiMap<string, EventDelegateData> map = new SortedMultiMap<string, EventDelegateData>();
        public EventDelegateTab(TabbedEditorWindow window): base("EventDelegate", window) {
        }
        
        private void Refresh() {
            this.map.RemoveAll();
            foreach (UIButton btn in Resources.FindObjectsOfTypeAll<UIButton>()) {
                AddDelegate(btn, btn.onClick);
            }
            foreach (UIEventTrigger t in Resources.FindObjectsOfTypeAll<UIEventTrigger>()) {
                AddDelegate(t, t.onHoverOver);
                AddDelegate(t, t.onHoverOut);
                AddDelegate(t, t.onPress);
                AddDelegate(t, t.onRelease);
                AddDelegate(t, t.onSelect);
                AddDelegate(t, t.onDeselect);
                AddDelegate(t, t.onClick);
                AddDelegate(t, t.onDoubleClick);
            }
            foreach (UIPlayAnimation a in Resources.FindObjectsOfTypeAll<UIPlayAnimation>()) {
                AddDelegate(a, a.onFinished);
            }
            foreach (UIPlayTween t in Resources.FindObjectsOfTypeAll<UIPlayTween>()) {
                AddDelegate(t, t.onFinished);
            }
            foreach (UIPopupList p in Resources.FindObjectsOfTypeAll<UIPopupList>()) {
                AddDelegate(p, p.onChange);
            }
            foreach (UIProgressBar p in Resources.FindObjectsOfTypeAll<UIProgressBar>()) {
                AddDelegate(p, p.onChange);
            }
            foreach (UIToggle t in Resources.FindObjectsOfTypeAll<UIToggle>()) {
                AddDelegate(t, t.onChange);
            }
            foreach (ActiveAnimation a in Resources.FindObjectsOfTypeAll<ActiveAnimation>()) {
                AddDelegate(a, a.onFinished);
            }
            foreach (UITweener t in Resources.FindObjectsOfTypeAll<UITweener>()) {
                AddDelegate(t, t.onFinished);
            }
            foreach (UIInput i in Resources.FindObjectsOfTypeAll<UIInput>()) {
                AddDelegate(i, i.onChange);
                AddDelegate(i, i.onSubmit);
            }
            foreach (UIWindow p in Resources.FindObjectsOfTypeAll<UIWindow>()) {
                AddDelegate(p, p.onInit);
                AddDelegate(p, p.onShowBegin);
                AddDelegate(p, p.onShowBegun);
                AddDelegate(p, p.onShowEnd);
                AddDelegate(p, p.onHideBegin);
                AddDelegate(p, p.onHideEnd);
            }
        }
        
        private void AddDelegate(MonoBehaviour src, IEnumerable<EventDelegate> list) {
            if (list == null || !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(src.gameObject))) {
                return;
            }
            foreach (EventDelegate d in list) {
                if (d != null) {
                    string key = d.ToString();
                    if (key.IsEmpty()) {
                        key = "[NULL]";
                    }
                    map.Add(key, new EventDelegateData(src, d));
                }
            }
        }
        
        public override void OnFocus(bool focus)
        {
            if (focus) {
                Refresh();  
            }
        }
        
        public override void OnEnable() { 
            Refresh();
        }
        public override void OnDisable() { }
        
        public override void OnChangePlayMode() {}
        public override void OnChangeScene(string sceneName) {}
        public override void OnSelected(bool sel) { }
        
        public override void OnHeaderGUI() {
        }
        
        public override void OnInspectorGUI() {
            string remove = null;
            foreach (KeyValuePair<string, List<EventDelegateData>> p in map) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(p.Key, EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical();
                foreach (EventDelegateData d in p.Value) {
                    MonoBehaviour target = ReflectionUtil.GetFieldValue<MonoBehaviour>(d.callback, "mTarget");
                    if (target != null) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(d.script, d.script.GetType(), true);
                        EditorGUILayout.LabelField("->", GUILayout.Width(30));
                        EditorGUILayout.ObjectField(target, target.GetType(), true);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("x", EditorStyles.toolbarButton)) {
                    remove = p.Key;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (remove != null) {
                map.Remove(remove);
            }
        }
        
        public override void OnFooterGUI() { }
        
        private class EventDelegateData {
            public MonoBehaviour script;
            public EventDelegate callback;
            
            public EventDelegateData(MonoBehaviour script, EventDelegate callback) {
                this.script = script;
                this.callback = callback;
            }
        }
    }
}
