using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using comunity;
using commons;

namespace ngui.ex
{
    public class UITabHandlerInspectorImpl
    {
        private UITabHandler tabHandler;
        private UITabArrInspector tabArrInspector;
        private UITabArrInspector tabPrefabArrInspector;
        private ArrayDrawer<UIButton> tabButtonArrInspector;
        
        public UITabHandlerInspectorImpl(UITabHandler handler)
        {
            this.tabHandler = handler;
            tabArrInspector = new UITabArrInspector(tabHandler, "tabs", true);
            tabPrefabArrInspector = new UITabArrInspector(tabHandler, "tabPrefabs", false);
            tabPrefabArrInspector.selectTab = false;
            tabButtonArrInspector = new ArrayDrawer<UIButton>(tabHandler, "tabButtons");
            bool auto = false;
            if (auto) {
                tabArrInspector.AddItemAddCallback(OnTabChanged);
                tabArrInspector.AddItemChangeCallback(OnTabChanged);
                tabArrInspector.AddItemRemoveCallback(OnTabRemoved);
            }
        }
        
        private UIButton  button;
        
        private void OnTabChanged(Object o, int index) {
            UITab tab = o as UITab;
            if (tab == null) {
                return;
            }
            if (tab.tabButton != null) {
                EventDelegateUtil.AddCallback<UITab>(tab.tabButton.onClick, tabHandler.OnClickTab, tab);
            }
        }
        
        private void OnTabRemoved(Object o, int index) {
            UITab tab = o as UITab;
            if (tab == null || tab.tabButton == null) {
                return;
            }
            EventDelegateUtil.RemoveCallback<UITab>(tab.tabButton.onClick, tabHandler, tabHandler.OnClickTab, tab);
        }
        
        public bool OnInspectorGUI() {
            bool changed = tabArrInspector.OnInspectorGUI();
            changed |= tabPrefabArrInspector.OnInspectorGUI();
            if (tabPrefabArrInspector.Length > 0)
            {
                changed |= tabButtonArrInspector.Draw();
            }
            if (GUILayout.Button("Reallocate")) {
                tabHandler.tabs = new UITab[0];
                foreach (UITab t in tabHandler.GetComponentsInChildren<UITab>(true)) {
                    tabHandler.tabs = tabHandler.tabs.Add(t);
                    OnTabChanged(t, 0);
                }
                CompatibilityEditor.SetDirty(tabHandler);
                //          for (int i=0; i<tabHandler.tabs.Length; ++i) {
                //              UITab t = tabHandler.tabs[i];
                //              OnTabRemoved(t, 0);
                //              OnTabChanged(t, 0);
                //          }
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtil.ObjectField<UIButton>(ref button, true);
            EditorGUILayout.EndHorizontal();
            
            foreach (UITab t in tabHandler.tabs) {
                if (t != null) {
                    if (t.tabButton == null) {
                        EditorGUILayout.HelpBox(t.name + " Tab Button is null", MessageType.Error);
                    }
                    if (t.uiRoot == null) {
                        EditorGUILayout.HelpBox(t.name + " Tab Root is null", MessageType.Error);
                    }
                }
            }
            HashSet<GameObject> activeTabs = new HashSet<GameObject>();
            foreach (UITab t in tabHandler.tabs) {
                if (t != null && t.IsVisible()) {
                    activeTabs.Add(t.uiRoot);
                }
            }
            if (activeTabs.Count > 1) {
                EditorGUILayout.HelpBox("Multiple Tabs are activated", MessageType.Error);
                if (GUILayout.Button("Fix")) {
                    tabHandler.Init(tabHandler);
                    foreach (UITab t in tabHandler.tabs) {
                        if (t != null) {
                            CompatibilityEditor.SetDirty(t.uiRoot);
                        }
                    }
                }
            }
            return changed;
        }
        
        private void SetTabColor(UIButton tabButton, Color inactiveTabColor, Color hoverTabColor, Color pressedTabColor, Color activeTabColor)
        {
            if (tabButton == null)
            {
                return;
            }
            UISprite sprite = tabButton.GetComponent<UISprite>();
            UILabel label = tabButton.GetComponentInChildrenEx<UILabel>();
            if (label != null)
            {
                Undo.RecordObjects(new Object[] {tabButton.gameObject, label.gameObject}, "TabColor");
            } else 
            {
                Undo.RecordObject(tabButton.gameObject, "TabColor");
            }
            
            
            if (sprite != null)
            {
                tabButton.normalSprite = "tab_normal";
                tabButton.hoverSprite = null;
                tabButton.pressedSprite = null;
                tabButton.disabledSprite = "tab_selected";
                sprite.color = Color.white;
            }
            if (label != null)
            {
                UIButtonColor[] colors = tabButton.GetComponents<UIButtonColor>();
                UIButtonColor c = null;
                foreach (var textCol in colors)
                {
                    if (textCol.tweenTarget == label.gameObject)
                    {
                        c = textCol;
                        break;
                    }
                }
                if (c == null)
                {
                    c = tabButton.gameObject.AddComponent<UIButtonColor>();
                }
                // add ButtonColor for Label
                label.effectStyle = UILabel.Effect.Outline;
                label.effectColor = Color.black;
                c.tweenTarget = label.gameObject;
                CompatibilityEditor.SetDirty(label.gameObject);
            }
            foreach (UIButtonColor c in tabButton.GetComponents<UIButtonColor>()) {
                if (c.tweenTarget == tabButton.gameObject)
                {
                    c.tweenTarget.GetComponent<UIWidget>().color = inactiveTabColor;
                    c.hover = hoverTabColor;
                    c.pressed = pressedTabColor;
                    c.disabledColor = activeTabColor;
                }
            }
            CompatibilityEditor.SetDirty(tabButton.gameObject);
        }
        
        private void ExtendTabCollider(UIButton tabButton)
        {
            UIWidget w = tabButton.GetComponent<UIWidget>();
            w.autoResizeBoxCollider = false;
            NGUITools.AddWidgetCollider(tabButton.gameObject);
            BoxCollider2D collider = tabButton.GetComponent<BoxCollider2D>();
            Vector2 offset = collider.offset;
            offset.y += 15;
            Vector2 size = collider.size;
            size.y += 40;
            size.x += 8;
            collider.size = size;
            collider.offset = offset;
            CompatibilityEditor.SetDirty(tabButton.gameObject);
        }
    }
    
    class UITabArrInspector : ObjArrInspector<UITab> {
        private UITabHandler tabHandler;
        public bool selectTab = true;
        public UITabArrInspector(UITabHandler handler, string varName, bool allowSceneObj) : base(handler, varName, allowSceneObj) {
            this.tabHandler = handler;
            this.SetTitle(varName);
            AddItemChangeCallback(OnItemChange);
        }
        
        protected override bool OnInspectorGUI(UITab tab, int i) {
            bool changed = base.OnInspectorGUI(tab, i);
            if (tab != null && selectTab) {
                bool visible = tab.IsVisible();
                if (EditorGUIUtil.Toggle(null, ref visible, GUILayout.Width(30))) {
                    changed = true;
                    for (int j=0; j<Length; ++j) {
                        UITab t = this[j] as UITab;
                        if (tab != t) {
                            t.uiRoot.SetActive(false);
                        }
                        CompatibilityEditor.SetDirty(t.gameObject);
                    }
                    tab.uiRoot.SetActive(true);
                }
            }
            return changed;
        }
        
        private void OnItemChange(Object o, int index)
        {
            UITab tab = o as UITab;
            if (tab.tabButton != null) {
                EventDelegateUtil.AddCallback(tab.tabButton.onClick, tabHandler.OnClickTab, tab);
            }
        }
    }
}