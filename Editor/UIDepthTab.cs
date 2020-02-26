using System;
using System.Collections.Generic;
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace ngui.ex
{
    public class UIDepthTab : EditorTab {
        private GameObject root;
        private UIWidget[] widgets = new UIWidget[0];

        public UIDepthTab(TabbedEditorWindow window) : base("Depth", window) {}

        public override void OnEnable() {}

        public override void OnDisable() { }

        public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
        public override void OnChangeScene(string sceneName) {
            Reset();
        }
        public override void OnFocus(bool focus) { }
        public override void OnSelected(bool sel) { }

        public override void OnHeaderGUI() {
            Validate();
            EditorGUILayout.BeginHorizontal();
            if (EditorGUILayoutUtil.ObjectField<GameObject>("Root", ref root, true)) {
                Refresh();
            }
            GUI.enabled = root != null;
            if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(false))) {
                Refresh();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            UIPanel[] panels = root != null? root.GetComponentsInChildren<UIPanel>(true): new UIPanel[0];
            EditorGUILayout.BeginHorizontal ();
            if (EditorGUILayoutUtil.PopupNullable ("Panel", ref panelSel, panels, ObjToString.DefaultToString)) {
                Refresh();
                if (panelSel != null) {
                    EditorGUIUtility.PingObject(panelSel);
                    Selection.activeGameObject = panelSel.gameObject;
                }
            }
            EditorGUILayoutUtil.Toggle("Show Active Only", ref showActiveOnly);
            EditorGUILayout.EndHorizontal ();
            INGUIAtlas[] atlases = GetAtlases (widgets);
            INGUIFont[] fonts = GetFonts (widgets);
            if (EditorGUILayoutUtil.PopupNullable("Select Atlas", ref atlasSel, atlases, ObjToString.DefaultToString)) {
                if (atlasSel != null) {
                    fontSel = null;
                }
            }
            if (EditorGUILayoutUtil.PopupNullable("Select Font", ref fontSel, fonts, ObjToString.DefaultToString)) {
                if (fontSel != null) {
                    atlasSel = null;
                }
            }
            EditorGUILayout.BeginHorizontal ();
            int index = Array.FindIndex(widgets, w => w.gameObject == Selection.activeGameObject);
            GUI.enabled = index >= 0;
            if (GUI.enabled)
            {
                if (GUILayout.Button("+1 over selection")) {
                    for (int i=0; i<=index; ++i) {
                        widgets[i].depth = widgets[i].depth+1;
                        EditorUtil.SetDirty(widgets[i]);
                    }
                }
                if (GUILayout.Button("-1 under selection")) {
                    for (int i=index; i<widgets.Length; ++i) {
                        widgets[i].depth = widgets[i].depth-1;
                        EditorUtil.SetDirty(widgets[i]);
                    }
                }
                // TODOM key handling
                var e = Event.current;
                if (e.type == EventType.KeyUp)
                {
                    if (e.keyCode == KeyCode.UpArrow) {
                        index--;
                        if (index >= 0)
                        {
                            Select(widgets[index]);
                        }
                    } else if (e.keyCode == KeyCode.DownArrow) {
                        index++;
                        if (index < widgets.Length)
                        {
                            Select(widgets[index]);
                        }
                    }
                }
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal ();
        }

        private void Validate() {
            foreach (UIWidget w in widgets) {
                if (w == null || w.gameObject == null) {
                    Refresh();
                    break;
                }
            }
        }

        private void Reset() {
            widgets = new UIWidget[0];
            atlasSel = null;
            fontSel = null;
        }

        private Color BG_COLOR1 = Color.yellow;
        private Color TXT_COLOR1 = Color.white;
        private Color BG_COLOR2 = Color.green;
        private Color TXT_COLOR2 = Color.white;
        private Color SEL_BG_COLOR = Color.red;
        private Color SEL_TXT_COLOR = Color.white;
        private Color INACTIVE_TXT_COLOR = Color.gray;

        private bool showActiveOnly;
        private INGUIAtlas atlasSel;
        private INGUIFont fontSel;
        private UIPanel panelSel;
        public override void OnInspectorGUI()
        {
            object current = null;

            Color contentColor = GUI.contentColor;
            Color bgColor = GUI.backgroundColor;
            Color c1 = BG_COLOR1;
            Color c2 = BG_COLOR1;
            foreach (UIWidget w in widgets) {
                if (w.GetType() == typeof(UIWidget) || w.gameObject == null) {
                    continue;
                }
                UISprite s = w as UISprite;
                UILabel l = w as UILabel;
                UITexture t = w as UITexture;
                int depth = w.depth;

                #pragma warning disable 0253
                bool toggle = false;
                if (s != null) {
                    if (s.atlas ==null)
                    {
                        continue;
                    }
                    if (s.atlas != current) {
                        current = s.atlas;
                        toggle = true;
                    }
                } else if (l != null) {
                    if (l.bitmapFont != current) {
                        current = l.bitmapFont;
                        toggle = true;
                    }
                } else if (t != null) {
                    if (t.mainTexture != current) {
                        current = t.mainTexture;
                        toggle = true;
                    }
                } else {
                    toggle = true;
                    current = null;
                }
                #pragma warning restore 0253
                if (toggle) {
                    c2 = c2 != BG_COLOR1? BG_COLOR1: BG_COLOR2;
                }
                c1 = c2;

                if (atlasSel != null) {
                    c1 = s!=null&& s.atlas==atlasSel? SEL_BG_COLOR: BG_COLOR1;
                } else if (fontSel != null) {
                    c1 = l!=null&& l.bitmapFont==fontSel? SEL_BG_COLOR: BG_COLOR1;
                }
                if (w.gameObject == Selection.activeGameObject) {
                    c1 = SEL_BG_COLOR;
                }
                GUI.backgroundColor = c1;
                if (c1 == BG_COLOR1)
                {
                    GUI.contentColor = TXT_COLOR1;
                } else if (c1 == BG_COLOR2)
                {
                    GUI.contentColor = TXT_COLOR2;
                } else if (c1 == SEL_BG_COLOR)
                {
                    GUI.contentColor = SEL_TXT_COLOR;
                }

                EditorGUILayout.BeginHorizontal();
                string widgetName = w.name;
                if (s != null) {
                    if (s.atlas != null) {
                        widgetName = string.Format("[ATLAS {0}] {2} ({1})", s.atlas.ToString(), s.spriteName, w.name);
                    }
                } else if (l != null) {
                    if (l.trueTypeFont != null) {
                        widgetName = string.Format("[TTF {0}] {1}", l.trueTypeFont.name, w.name);
                    } else if (l.bitmapFont != null) {
						widgetName = string.Format("[BMF {0}] {1}", l.bitmapFont.ToString(), w.name);
                    } else {
                        widgetName = string.Format("FNT {0}", w.name);
                    }
                } else if (t != null) {
                    if (t.mainTexture != null) {
                        widgetName = string.Format("[TEX {0}] {1}", t.mainTexture.name, t.name);
                    } else {
                        widgetName = string.Format("TEX {0}", t.name);
                    }
                }
                bool active = w.gameObject.activeInHierarchy;
                if (active || !showActiveOnly) {
                    Color fgColor = Color.white;
                    if (!active) {
                        fgColor = INACTIVE_TXT_COLOR;
                    }
                    GUI.contentColor = fgColor;
                    if (GUILayout.Button(widgetName, EditorStyles.objectField)) {
                        Select(w);
                    }
                    if (EditorGUILayoutUtil.IntField(null, ref depth, GUILayout.Width(40))) {
                        w.depth = depth;
                        EditorUtil.SetDirty(w);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            GUI.backgroundColor = bgColor;
            GUI.contentColor = contentColor;
        }

        private void Select(UIWidget w)
        {
            EditorGUIUtility.PingObject(w.gameObject);
            if (Selection.activeGameObject == w.gameObject) {
                Selection.activeGameObject = null;
            } else {
                Selection.activeGameObject = w.gameObject;
            }
        }

        private INGUIAtlas[] GetAtlases (UIWidget[] widgets)
        {
            HashSet<INGUIAtlas> set = new HashSet<INGUIAtlas>();
            foreach (UIWidget w in widgets) {
                UISprite s = w as UISprite;
                if (s != null && s.atlas != null) {
                    set.Add(s.atlas);
                }
            }
            return new List<INGUIAtlas> (set).ToArray ();
        }

        private INGUIFont[] GetFonts (UIWidget[] widgets)
        {
            HashSet<INGUIFont> set = new HashSet<INGUIFont>();
            foreach (UIWidget w in widgets) {
                UILabel l = w as UILabel;
                if (l != null && l.bitmapFont != null) {
                    set.Add(l.bitmapFont);
                }
            }
            return new List<INGUIFont> (set).ToArray ();
        }

        public override void OnFooterGUI() {}

        private void Refresh() {
            if (root != null) {
                widgets = root.GetComponentsInChildren<UIWidget>(true);
                if (panelSel != null) {
                    List<UIWidget> list = new List<UIWidget>();
                    foreach (UIWidget w in widgets) {
                        if (w.GetComponentInParent<UIPanel>() == panelSel) {
                            list.Add(w);
                        }
                    }
                    widgets = list.ToArray();
                }
                Array.Sort (widgets, (w1, w2) => {
                    if (w1 == null) {
                        return 1;
                    } else if (w2 == null) {
                        return -1;
                    }
                    if (w1.depth == w2.depth) {
                        string n1 = GetAtlasName(w1);
                        string n2 = GetAtlasName(w2);
                        if (n1 == null) {
                            if (n2 != null) {
                                return 1;
                            } else {
                                return 0;
                            }
                        } else if (n2==null) {
                            return -1;
                        }
                        return n1.CompareTo(n2);
                    }
                    return w2.depth-w1.depth;
                });
            } else {
                Reset();
            }
        }

        private string GetAtlasName (UIWidget w)
        {
            UISprite s = w as UISprite;
            if (s != null && s.atlas != null) {
                return s.atlas.name();
            }
            UILabel l = w as UILabel;
            if (l != null && l.bitmapFont != null) {
                return l.bitmapFont.ToString();
            }
            if (l != null && l.trueTypeFont != null) {
                return l.trueTypeFont.name;
            }
            return null;
        }
    }
}
