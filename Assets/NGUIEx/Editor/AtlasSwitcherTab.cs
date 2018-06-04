using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using comunity;


namespace ngui.ex {
    public class AtlasSwitcherTab : EditorTab
	{
		private UIAtlas atlasFrom = null;
		private UIAtlas atlasTo = null;
		
		private GameObject sceneRoot;
		private List<string> duplicateSprites = new List<string>();
		private string spriteName1;
		private string spriteName2;
		
		public AtlasSwitcherTab(TabbedEditorWindow window): base("AtalsSwitcher", window) {
		}
		
		private void CopySpriteProperty(UISpriteData spriteFrom, UISpriteData spriteTo) {
			spriteTo.paddingLeft = spriteFrom.paddingLeft;
			spriteTo.paddingRight = spriteFrom.paddingRight;
			spriteTo.paddingTop = spriteFrom.paddingTop;
			spriteTo.paddingBottom = spriteFrom.paddingBottom;
			
			// XXX_mulova
			//		Vector4 borderFrom = new Vector4(
			//			spriteFrom.inner.xMin - spriteFrom.outer.xMin,
			//			spriteFrom.inner.yMin - spriteFrom.outer.yMin,
			//			spriteFrom.outer.xMax - spriteFrom.inner.xMax,
			//			spriteFrom.outer.yMax - spriteFrom.inner.yMax);
			//		
			//		Rect inner = spriteTo.inner;
			//		inner.xMin = spriteTo.outer.xMin + borderFrom.x;
			//		inner.yMin = spriteTo.outer.yMin + borderFrom.y;
			//		inner.xMax = spriteTo.outer.xMax - borderFrom.z;
			//		inner.yMax = spriteTo.outer.yMax - borderFrom.w;
			//		spriteTo.inner = inner;
		}
		
		private void SwitchSprite(string fromSprite, string toSprite) {
			UISprite[] sprites = sceneRoot.GetComponentsInChildren<UISprite>(true);
			StringBuilder str = new StringBuilder();
			foreach (UISprite s in sprites) {
				if (s.atlas == atlasFrom && s.spriteName == fromSprite) {
					CopySpriteProperty(atlasFrom.GetSprite(fromSprite), atlasTo.GetSprite(toSprite));
					s.atlas = atlasTo;
					s.spriteName = toSprite;
					EditorUtil.SetDirty(s);
					EditorUtil.SetDirty(atlasTo);
					str.AppendFormat("{0}({1})\n", GetScenePath(s.gameObject), s.GetType().FullName);
				}
			}
			if (str.Length > 0) {
				Debug.Log(str.ToString());
			}
			Debug.Log("Switching Atals "+atlasFrom.name+" -> "+atlasTo.name+" is DONE.");
		}
		
		private void SwitchAtlas() {
			// copy property
			foreach (string spriteName in duplicateSprites) {
				CopySpriteProperty(atlasFrom.GetSprite(spriteName), atlasTo.GetSprite(spriteName));
			}
			UISprite[] sprites = sceneRoot.GetComponentsInChildren<UISprite>(true);
			HashSet<string> set = new HashSet<string>(duplicateSprites);
			StringBuilder str = new StringBuilder();
			foreach (UISprite s in sprites) {
				if (s.atlas == atlasFrom && set.Contains(s.spriteName)) {
					s.atlas = atlasTo;
					EditorUtil.SetDirty(s);
					EditorUtil.SetDirty(atlasTo);
					str.AppendFormat("{0}({1})\n", GetScenePath(s.gameObject), s.GetType().FullName);
				}
			}
			if (str.Length > 0) {
				Debug.Log(str.ToString());
			}
			Debug.Log("Switching Atals "+atlasFrom.name+" -> "+atlasTo.name+" is DONE.");
		}
		
		private string GetScenePath(GameObject obj) {
			StringBuilder str = new StringBuilder(128);
			Transform t = obj.transform;
			Stack<string> transList = new Stack<string>();
			while (t != null) {
				transList.Push(t.name);
				t = t.parent;
			}
			while (transList.Count > 0) {
				str.Append(transList.Pop());
				if (transList.Count > 0) {
					str.Append("/");
				}
			}
			return str.ToString();
		}
		
		
		private List<string> FindDuplicate(UIAtlas a1, UIAtlas a2) {
			BetterList<string> list1 = a1.GetListOfSprites();
			BetterList<string> list2 = a2.GetListOfSprites();
			List<string> duplicate = new List<string>();
			HashSet<string> a1Set = new HashSet<string>();
			for (int i=0; i<list1.size; ++i) {
				a1Set.Add(list1[i]);
			}
			for (int i=0; i<list2.size; ++i) {
				if (a1Set.Contains(list2[i])) {
					duplicate.Add(list2[i]);
				}
			}
			duplicate.Sort();
			return duplicate;
		}
		
		public override void OnEnable() { }
		
		public override void OnDisable() { }

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnFocus(bool focus) { }
		public override void OnSelected(bool sel) { }
		
		public override void OnHeaderGUI() {
			bool changed = EditorGUIUtil.ObjectField<GameObject>("Scene Root", ref sceneRoot, true);
			changed |= EditorGUIUtil.ObjectField<UIAtlas>("From", ref atlasFrom, false);
			changed |= EditorGUIUtil.ObjectField<UIAtlas>("To", ref atlasTo, false);
			NGUIEditorTools.DrawSeparator();
			
			if (changed && atlasFrom!=null && atlasTo!=null) {
				duplicateSprites = FindDuplicate(atlasFrom, atlasTo);
			}
		}
		
		public override void OnInspectorGUI() {
			
			foreach (string s in duplicateSprites) {
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(s);
				if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
					duplicateSprites.Remove(s);
					break;
				}
				GUILayout.EndHorizontal();
			}
		}
		
		public override void OnFooterGUI() {
			if (atlasFrom != null && atlasTo != null) {
				if (duplicateSprites.Count > 0) {
					if (GUILayout.Button(atlasFrom.name + " -> "+atlasTo.name)) {
						SwitchAtlas();
					}
				}
				NGUIEditorTools.DrawSeparator();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				EditorGUIUtil.Popup<string>("Sprite From", ref spriteName1, atlasFrom.GetListOfSprites().ToArray());
				EditorGUIUtil.Popup<string>("Sprite To", ref spriteName2, atlasTo.GetListOfSprites().ToArray());
				EditorGUILayout.EndVertical();
				if (GUILayout.Button("Replace", GUILayout.ExpandWidth(false), GUILayout.Height(30))) {
					SwitchSprite(spriteName1, spriteName2);
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
	
}