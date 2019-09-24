using System.Collections.Generic;
using System.Collections.Generic.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace ngui.ex
{
    public class AtlasSearcherTab : EditorTab
    {
        private GameObject sceneRoot;
        private UIAtlas atlas;
        
        public AtlasSearcherTab(TabbedEditorWindow window): base("AtalsSearch", window) {
        }
        
        public override void OnEnable() { }
        public override void OnDisable() { }
        public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
        public override void OnChangeScene(string sceneName) {}
        public override void OnFocus(bool focus) { }
        public override void OnSelected(bool sel) { }
        
        private string filter = "";
        private bool expand;
        public override void OnHeaderGUI() {
            EditorGUILayoutUtil.ObjectField<GameObject>("Scene Root", ref sceneRoot, true);
            GUI.enabled = sceneRoot != null;
            ComponentSelector.Draw<UIAtlas>("Select", NGUISettings.atlas, OnSelectAtlas, true);
            EditorGUILayoutUtil.TextField("Filter", ref filter);
            if (EditorGUILayoutUtil.Toggle("Expand", ref expand)) {
                foreach (string s in spriteMap.Keys) {
                    foldMap[s] = expand;
                }
            }
            GUI.enabled = true;
            NGUIEditorTools.DrawSeparator();
        }
        
        private MultiMap<string, UISprite> spriteMap = new MultiMap<string, UISprite>();
        private void OnSelectAtlas (Object obj)
        {
            atlas = NGUISettings.atlas = obj as UIAtlas;
            
            UISprite[] sprites = sceneRoot.GetComponentsInChildren<UISprite>(true);
            spriteMap = new MultiMap<string, UISprite>();
            foreach (UISprite s in sprites) {
                if (s.atlas == atlas) {
                    spriteMap.Add(s.spriteName, s);
                }
            }
            Repaint();
        }
        
        private Dictionary<string, bool> foldMap = new Dictionary<string, bool>();
        public override void OnInspectorGUI() {
            List<string> keys = new List<string>(spriteMap.Keys);
            keys.Sort();
            foreach (string s in keys) {
                //          GUI.DrawTextureWithTexCoords
                if (string.IsNullOrEmpty(filter) || s.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0) {
                    bool fold = foldMap.Get(s, false);
                    fold = EditorGUILayout.Foldout(fold, string.Format("{0} ({1})", s, spriteMap[s].Count), EditorStyles.foldoutPreDrop);
                    foldMap[s] = fold;
                    if (fold) {
                        EditorGUI.indentLevel += 2;
                        foreach (UISprite sprite in spriteMap[s]) {
                            EditorGUILayout.ObjectField(sprite, typeof(UISprite), true);
                        }
                        EditorGUI.indentLevel -= 2;
                    }
                }
            }
        }
        
        public override void OnFooterGUI() {
        }
    }
}
