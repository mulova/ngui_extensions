using System.Collections.Generic;


using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using comunity;

namespace ngui.ex {
    public class UIDepthTab : EditorTab {
		private GameObject root;
		private UIWidget[] widgets = new UIWidget[0];
		
		public UIDepthTab(TabbedEditorWindow window) : base("Depth", window) {}
		
		public override void OnEnable() {}
		
		public override void OnDisable() { }

		public override void OnChangePlayMode() {}
		public override void OnChangeScene(string sceneName) {
			Reset();
		}
		public override void OnFocus(bool focus) { }
		public override void OnSelected(bool sel) { }
		
		public override void OnHeaderGUI() {
			Validate();
			EditorGUILayout.BeginHorizontal();
			if (EditorGUIUtil.ObjectField<GameObject>("Root", ref root, true)) {
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
			if (EditorGUIUtil.PopupNullable ("Panel", ref panelSel, panels, ObjToString.DefaultToString)) {
				Refresh();
				if (panelSel != null) {
					EditorGUIUtility.PingObject(panelSel);
					Selection.activeGameObject = panelSel.gameObject;
				}
			}
			EditorGUIUtil.Toggle("Show Active Only", ref showActiveOnly);
			EditorGUILayout.EndHorizontal ();
			UIAtlas[] atlases = GetAtlases (widgets);
			UIFont[] fonts = GetFonts (widgets);
			if (EditorGUIUtil.PopupNullable("Select Atlas", ref atlasSel, atlases, ObjToString.DefaultToString)) {
				if (atlasSel != null) {
					fontSel = null;
				}
			}
			if (EditorGUIUtil.PopupNullable("Select Font", ref fontSel, fonts, ObjToString.DefaultToString)) {
				if (fontSel != null) {
					atlasSel = null;
				}
			}
			EditorGUILayout.BeginHorizontal ();
			int index = Array.FindIndex(widgets, w => w.gameObject == Selection.activeGameObject);
			GUI.enabled = index >= 0;
			if (GUILayout.Button("+1 over selection")) {
				for (int i=0; i<=index; ++i) {
					widgets[i].depth = widgets[i].depth+1;
					CompatibilityEditor.SetDirty(widgets[i]);
				}
			}
			if (GUILayout.Button("-1 under selection")) {
				for (int i=index; i<widgets.Length; ++i) {
					widgets[i].depth = widgets[i].depth-1;
					CompatibilityEditor.SetDirty(widgets[i]);
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

		private Color COLOR1 = Color.yellow;
		private Color COLOR2 = Color.grey;
		private Color COLOR_SEL = Color.red;
		private Color COLOR_RANGE = Color.cyan;

		private bool showActiveOnly;
		private UIAtlas atlasSel;
		private UIFont fontSel;
		private UIPanel panelSel;
		public override void OnInspectorGUI()
		{
			// TODOM key handling
			if (Input.GetKeyUp(KeyCode.UpArrow)) {
			} else if (Input.GetKeyUp(KeyCode.UpArrow)) {
			}
				
			object current = null;

			Color contentColor = GUI.contentColor;
			Color bgColor = GUI.backgroundColor;
			Color c = COLOR1;
			Color toggleColor = COLOR1;
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
					toggleColor = toggleColor != COLOR1? COLOR1: COLOR2;
				}
				c = toggleColor;

				if (atlasSel != null) {
					c = s!=null&& s.atlas==atlasSel? COLOR_SEL: COLOR1;
				} else if (fontSel != null) {
					c = l!=null&& l.bitmapFont==fontSel? COLOR_SEL: COLOR1;
				}
				if (w.gameObject == Selection.activeGameObject) {
					c = COLOR_RANGE;
				}
				GUI.backgroundColor = c;

				EditorGUILayout.BeginHorizontal();
				string widgetName = w.name;
				if (s != null) {
					if (s.atlas != null) {
						widgetName = string.Format("{0}.{1} ({2})", s.atlas.name, s.spriteName, w.name);
					}
				} else if (l != null) {
					if (l.trueTypeFont != null) {
						widgetName = string.Format("{0} ({1})", l.trueTypeFont.name, w.name);
						
					} else if (l.bitmapFont != null && l.bitmapFont.atlas != null) {
						widgetName = string.Format("{0} ({1})", l.bitmapFont.atlas.name, w.name);
					} else {
						widgetName = string.Format("Font ({0})", w.name);
					}
				} else if (t != null) {
					if (t.mainTexture != null) {
						widgetName = string.Format("{0} ({1})", t.mainTexture.name, t.name);
					} else {
						widgetName = string.Format("Texture ({0})", t.name);
					}
				}
				bool active = w.gameObject.activeInHierarchy;
				if (active || !showActiveOnly) {
					Color fgColor = Color.black;
					if (!active) {
						fgColor = Color.gray;
					}
					GUI.contentColor = fgColor;
					if (GUILayout.Button(widgetName, EditorStyles.objectField)) {
						EditorGUIUtility.PingObject(w.gameObject);
						if (Selection.activeGameObject == w.gameObject) {
							Selection.activeGameObject = null;
						} else {
							Selection.activeGameObject = w.gameObject;
						}
					}
					if (EditorGUIUtil.IntField(null, ref depth, GUILayout.Width(40))) {
						w.depth = depth;
						CompatibilityEditor.SetDirty(w);
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			GUI.backgroundColor = bgColor;
			GUI.contentColor = contentColor;
		}

		private UIAtlas[] GetAtlases (UIWidget[] widgets)
		{
			HashSet<UIAtlas> set = new HashSet<UIAtlas>();
			foreach (UIWidget w in widgets) {
				UISprite s = w as UISprite;
				if (s != null && s.atlas != null) {
					set.Add(s.atlas);
				}
			}
			return new List<UIAtlas> (set).ToArray ();
		}

		private UIFont[] GetFonts (UIWidget[] widgets)
		{
			HashSet<UIFont> set = new HashSet<UIFont>();
			foreach (UIWidget w in widgets) {
				UILabel l = w as UILabel;
				if (l != null && l.bitmapFont != null) {
					set.Add(l.bitmapFont);
				}
			}
			return new List<UIFont> (set).ToArray ();
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
				return s.atlas.name;
			}
			UILabel l = w as UILabel;
			if (l != null && l.bitmapFont != null) {
				return l.bitmapFont.name;
			}
			if (l != null && l.trueTypeFont != null) {
				return l.trueTypeFont.name;
			}
			return null;
		}
	}
}
