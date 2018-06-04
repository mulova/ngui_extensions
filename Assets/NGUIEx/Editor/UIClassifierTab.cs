using System.Collections.Generic;
using commons;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using comunity;

namespace ngui.ex {
    public class UIClassifierTab : EditorTab {
		
		private Vector2 scroll;
		private GameObject root;
		private UIWidgetClassifier classifier = new UIWidgetClassifier(null);
		private List<GameObject> panels = new List<GameObject>();
		
		private Dictionary<UIWidgetClassifier.WidgetType, MultiMap<string, DupEntry>> dups = new Dictionary<UIWidgetClassifier.WidgetType, MultiMap<string, DupEntry>>();
		private Vector2 rightPanelScroll;
		
		public UIClassifierTab(TabbedEditorWindow window) : base("Classifier", window) {}
		
		public override void OnEnable() {}
		
		public override void OnDisable() { }

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnFocus(bool focus) { }
		public override void OnSelected(bool sel) { }
		
		public override void OnHeaderGUI() {}
		
		public override void OnInspectorGUI()
		{
			GUI.enabled = true;
			EditorGUILayout.BeginHorizontal();
			if (EditorGUIUtil.ObjectField<GameObject>("Root", ref root, true)) {
				Classify(ref root);
			}
			if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(false)) || root == null) {
				Classify(ref root);
			}
			GUI.enabled = root != null;
			if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false))) {
				Rename();
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			scroll = EditorGUILayout.BeginScrollView(scroll);
			VisualizeGroup();
			EditorGUIUtil.DrawSeparator();
			VisualizeDups();
			EditorGUILayout.EndScrollView();
		}
		
		public override void OnFooterGUI() {}
		
		private void VisualizeDups() {
			if (dups.Count == 0) {
				return;
			}
			EditorGUILayout.LabelField("Duplicates", EditorStyles.boldLabel);
			EditorGUI.indentLevel += 2;
            foreach (UIWidgetClassifier.WidgetType widgetType in EnumUtil.Values<UIWidgetClassifier.WidgetType>()) {
				MultiMap<string, DupEntry> map = null;
				if (dups.TryGetValue(widgetType, out map) && map.Count > 0) {
					EditorGUILayout.LabelField(widgetType.ToString(), EditorStyles.boldLabel);
					EditorGUI.indentLevel += 2;
					foreach (KeyValuePair<string, List<DupEntry>> slot in map) {
						ObjWrapperReorderList drawer = new ObjWrapperReorderList(null, slot.Value);
						drawer.Draw();
						EditorGUILayout.Space();
					}
					EditorGUI.indentLevel -= 2;
				}
			}
			EditorGUI.indentLevel -= 2;
			
		}
		
		private void VisualizeGroup() {
            foreach (UIWidgetClassifier.WidgetType widgetType in EnumUtil.Values<UIWidgetClassifier.WidgetType>()) {
				List<GameObject> list = classifier[widgetType];
				if (list.Count == 0) continue;
				EditorGUILayout.LabelField(widgetType.ToString(), EditorStyles.boldLabel);
				EditorGUIUtil.ObjectFieldReorderList(list);
			}
			
			EditorGUILayout.LabelField("Panel", EditorStyles.boldLabel);
            EditorGUIUtil.ObjectFieldReorderList(panels);
			EditorGUI.indentLevel -= 2;
		}
		
		private void Classify(ref GameObject root) {
			/*  root가 없을 경우 하나를 선택한다. */
			if (Camera.main == null || Camera.main.transform.parent == null) {
				return;
			}
			UIRoot uiRoot = Camera.main.transform.parent.GetComponent<UIRoot>();
			if (uiRoot == null) {
				return;
			}
			root = uiRoot.gameObject;
			
			classifier.SetRoot(root);
			panels.Clear();
			foreach (Object o in Object.FindObjectsOfType(typeof(UIPanel))) {
				panels.Add(((UIPanel)o).gameObject);
			}
			
			dups.Clear();
            foreach (UIWidgetClassifier.WidgetType widgetType in EnumUtil.Values<UIWidgetClassifier.WidgetType>()) {
				MultiMap<string, DupEntry> slot = new MultiMap<string, DupEntry>();
				dups[widgetType] = slot;
				foreach (GameObject o in classifier.GetWidgets(widgetType)) {
					if (widgetType == UIWidgetClassifier.WidgetType.Label) {
						UILabel labelScript = o.GetComponentInChildren<UILabel>();
						if (labelScript != null) {
							string name = labelScript.text;
							slot.Add(o.name, new DupEntry(o, name));
						}
					} else if (widgetType == UIWidgetClassifier.WidgetType.Button) {
						UILabel labelScript = o.GetComponentInChildren<UILabel>();
						if (labelScript != null) {
							string name = labelScript.text;
							slot.Add(o.name, new DupEntry(o, name));
						}
					} else if (widgetType == UIWidgetClassifier.WidgetType.Input) {
						UILabel labelScript = o.GetComponentInChildren<UILabel>();
						if (labelScript != null) {
							string name = labelScript.text;
							slot.Add(o.name, new DupEntry(o, name));
						}
					} else if (widgetType == UIWidgetClassifier.WidgetType.Slider) {
						slot.Add(o.name, new DupEntry(o, o.name));
					} else if (widgetType == UIWidgetClassifier.WidgetType.ProgressBar) {
						slot.Add(o.name, new DupEntry(o, o.name));
					}
				}
			}
			
			RemoveSingle();
		}
		
		private void RemoveSingle() {
            foreach (UIWidgetClassifier.WidgetType widgetType in EnumUtil.Values<UIWidgetClassifier.WidgetType>()) {
				MultiMap<string, DupEntry> slot = dups[widgetType];
				List<string> names = new List<string>();
				foreach (KeyValuePair<string, List<DupEntry>> entry in slot) {
					if (entry.Value.Count == 1) {
						names.Add(entry.Key);
					}
				}
				foreach (string s in names) {
					slot.Remove(s);
				}
			}
		}
		
		private void Rename() {
            foreach (UIWidgetClassifier.WidgetType widgetType in EnumUtil.Values<UIWidgetClassifier.WidgetType>()) {
				MultiMap<string, DupEntry> map = null;
				if (dups.TryGetValue(widgetType, out map)) {
					foreach (KeyValuePair<string, List<DupEntry>> slot in map) {
						foreach (DupEntry e in slot.Value) {
							if (e.name != e.obj.name) {
								e.obj.name = e.name;
							}
						}
					}
				}
			}
		}
		
		class DupEntry : ObjWrapper {
			public GameObject obj;
			public string name;
			
			public DupEntry(GameObject o, string name) {
				this.obj = o;
				name = name.Replace("\r\n", " ");
				name = name.Replace("\n", " ");
				name = name.Replace("\t", " ");
				this.name = name;
			}
			
			public Object Obj { get { return obj;} }
			public string Name { get { return name;} }
		}
	}
}
