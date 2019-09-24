using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Reflection;
using System.Text.Ex;
using System.Text.RegularExpressions;
using convinity;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ngui.ex
{
    class AssetRefTab : SearchTab<Object>
	{
		private Object refObj;
		private MemberInfoRegistry registry = new MemberInfoRegistry(MemberInfoRegistryEx.ObjectRefFilter);
		
		public AssetRefTab(TabbedEditorWindow window) : base("AssetRef", window) {
			SetExclude (@"Legionz/Font/
Legionz/Res/Texture/blank.png
NGUI/Resources/Shaders/
.anim$
Assets/Legionz/Sprite/.*\.prefab$");
		}

		private void SetExclude(string e) {
			exclude = e;
			excludes.Clear();
			if (!e.IsEmpty()) {
				foreach (string s in e.Split(new char[] { '\n' })) {
					if (!s.IsEmpty()) {
						excludes.Add(new Regex(s));
					}
				}
			}
		}

		private string filter;
		private string exclude;
		private List<Regex> excludes = new List<Regex>();
		private bool excludeTexLoader = true;
		public override void OnHeaderGUI(List<Object> found)
		{
			EditorGUILayoutUtil.TextArea ("Exclude", ref exclude);
			EditorGUILayoutUtil.Toggle("Exclude TexLoader", ref excludeTexLoader);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayoutUtil.TextField(null, ref filter);
			if (GUILayout.Button("Search")) {
				SetExclude(exclude);
				Search();
			}
			EditorGUILayout.EndHorizontal();
		}

		public override void OnFooterGUI(List<Object> found)
		{
		}

		protected override List<Object> SearchResource()
		{
			List<Object> list = new List<Object>();
			foreach (var root in roots)
			{
				if (AssetDatabase.GetAssetPath(root).IsEmpty()) {
					IEnumerable<Transform> roots = null;
					if (root != null) {
						roots = new Transform[] {(root as GameObject).transform};
					} else {
						roots = EditorUtil.GetSceneRoots().ConvertAll(o=>o.transform);
					}
					foreach (Transform r in roots) {
						foreach (Component c in r.GetComponentsInChildren<Component>(true)) {
							if (c != null) {
								if (HasAssetRef(c, 0)) {
									list.Add(c);
								}
							}
						}
					}
				} else {
					foreach (Object o in SearchAssets(typeof(GameObject), FileType.Prefab)) {
						foreach (Component c in (o as GameObject).GetComponentsInChildren<Component>(true)) {
							if (c != null) {
								if (HasAssetRef(c, 0)) {
									list.Add(c);
								}
							}
						}
					}
					foreach (Object o in SearchAssets(typeof(Material), FileType.Material)) {
						if (HasAssetRef(o, 0)) {
							list.Add(o);
						}
					}
				}
			}
			return list;
		}

		private const int MAX_DEPTH = 0;
		private bool HasAssetRef(Object o, int depth)
		{
			IEnumerable<FieldInfo> fields = GetObjectFields(o.GetType());
			foreach (FieldInfo f in fields) {
				Object val = f.GetValue(o) as Object;
				// ignore if UITexture has TexLoader
				if (excludeTexLoader && val is Texture && o is UITexture 
				    && (o as UITexture).GetComponent<TexLoader>() != null
				    && (o as UITexture).GetComponent<TexLoader>().Target == o) {
					continue;
				}
				if (val != null) {
					string assetPath = AssetDatabase.GetAssetPath(val);
					if (!assetPath.IsEmpty() && (filter.IsEmpty() || assetPath.Contains(filter))) {
						foreach (Regex regex in excludes) {
							if (regex.IsMatch(assetPath)) {
								return false;
							}
						}
						return true;
					} else if (depth < MAX_DEPTH) {
						Component child = val as Component;
						if (child != null) {
							if (HasAssetRef(child, depth+1)) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private Dictionary<Type, List<FieldInfo>> fieldStore = new Dictionary<Type, List<FieldInfo>>();
		private IEnumerable<FieldInfo> GetObjectFields(Type type) {
			List<FieldInfo> fields = fieldStore.Get (type);
			if (fields == null) {
				fields = new List<FieldInfo>();
				fieldStore[type] = fields;
				foreach (FieldInfo f in registry.GetFields(type)) {
					if (typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType)) {
						fields.Add(f);
					}
				}
			}
			return fields;
		}

		protected override void OnInspectorGUI(List<Object> found)
		{
			GUI.enabled = true;
            throw new Exception("reimplement");
			//EditorGUILayoutUtil.ObjectFieldReorderList(found);
		}
		
		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnFocus(bool focus) {}
		public override void OnSelected(bool sel) {}
	}
}

class AllocateInfo {
	public Object obj;
	public FieldInfo field;

	public PropertyInfo property;

	public AllocateInfo(Object o, FieldInfo f) {
		this.obj = o;
		this.field = f;
	}

	public AllocateInfo(Object o, PropertyInfo p) {
		this.obj = o;
		this.property = p;
	}

	public void SetValue(Object val) {
		if (field != null) {
			field.SetValue(obj, val);
		} else if (property != null) {
			property.SetValue(obj, val, null);
		}
		EditorUtil.SetDirty(obj);
	}
}