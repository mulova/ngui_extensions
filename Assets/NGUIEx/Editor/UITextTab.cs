using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Linq;
using System.Text.Ex;
using mulova.build;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace ngui.ex
{
    public class UITextTab : EditorTab
	{
        public LexiconRegistry lexReg;
		private static bool locked = false;
		private static bool visibleOnly = true;
		private Vector2 scroll;
		private GameObject[] roots;

		public UITextTab(TabbedEditorWindow window) : base("Text", window)
		{
		}

		public override void OnEnable()
		{
		}

		public override void OnDisable()
		{
		}

		public override void OnChangePlayMode(PlayModeStateChange stateChange)
		{
		}

		public override void OnChangeScene(string sceneName)
		{
			Clear();
		}

		public override void OnFocus(bool focus)
		{
			if (focus)
			{
				ReloadTable();
			}
		}

		public override void OnSelected(bool sel)
		{
		}

		private void ReloadTable()
		{
            if (Application.isPlaying || lexReg == null)
			{
				return;
			}
			AssetDatabase.Refresh(ImportAssetOptions.Default);
//            Initializer.LoadLexicon ();
            lexReg.LoadLexicons();
            Lexicon.SetMotherLanguage(motherLang);
            Lexicon.SetLanguage(lang);
		}

		private void Clear()
		{
			roots = null;
			labels.Clear();
			mod.Clear();
		}

		private SystemLanguage motherLang = SystemLanguage.English;
		private SystemLanguage lang = SystemLanguage.Korean;
		private bool fold;

		public override void OnHeaderGUI()
		{
            EditorGUILayoutUtil.ObjectField<LexiconRegistry>("Lexicon Registry", ref lexReg, false);
            if (lexReg == null)
            {
                return;
            }
			EditorGUILayoutUtil.PopupEnum<SystemLanguage>("Mother language", ref motherLang);
			EditorGUILayoutUtil.PopupEnum<SystemLanguage>("Language", ref lang);
			fold = EditorGUILayout.Foldout(fold, "Translate All");
			if (fold)
			{
				if (GUILayout.Button("Convert all Scene and prefab")&&EditorUtility.DisplayDialog("Confirm", "Convert All?", "OK", "Cancel"))
				{
					Lexicon.SetMotherLanguage(motherLang);
					Lexicon.SetLanguage(lang);
					TranslateLanguage();
				}
			}
			EditorGUILayoutUtil.Toggle("Lock", ref locked);
			if (EditorGUILayoutUtil.Toggle("Visible Only", ref visibleOnly))
			{
				roots = null;
			}
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Reload Table", GUILayout.Height(30)))
			{
				ReloadTable();
				ApplyTableToText();
			}
			if (GUILayout.Button("Apply table to text", GUILayout.Height(30)))
			{
				ApplyTableToText();
			}
			if (GUILayout.Button("Revert", GUILayout.Height(30)))
			{
				Clear();
			}
			EditorGUILayout.EndHorizontal();
			DrawFindLexiconGUI();
		}

		private string lexiconKeys;

		private void DrawFindLexiconGUI()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayoutUtil.TextField("Lexicon Keys", ref lexiconKeys);
			if (GUILayout.Button("Find")&&!lexiconKeys.IsEmpty())
			{
				Debug.Log(UIBuildScript.FindTextKey(lexiconKeys));
			}
			EditorGUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI()
		{
			if (Application.isPlaying)
			{
				EditorGUILayout.HelpBox("Play mode", MessageType.Error);
				return;
			}
			scroll = EditorGUILayout.BeginScrollView(scroll);
			try
			{
				DrawTextList();
			} catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			EditorGUILayout.EndScrollView();
		}

		private void ApplyTableToText()
		{
			// Apply modifications
			foreach (var pair in mod)
			{
				pair.Key.textKey = pair.Value;
                EditorUtil.SetDirty(pair.Key);
			}
			mod.Clear();
			foreach (UIText l in labels)
			{
				if (l.textKey != null)
				{
					string trans = Lexicon.Get(l.textKey);
					if (!trans.IsEmpty())
					{
						l.SetText(trans);
                        EditorUtil.SetDirty(l);
					}
				}
			}
		}

		public override void OnFooterGUI()
		{
		}

		private Dictionary<UIText, string> mod = new Dictionary<UIText, string>();
        private List<UIText> labels = new List<UIText>();
		private List<UITabHandlerInspectorImpl> tabs = new List<UITabHandlerInspectorImpl>();

		private void DrawTextList()
		{
			EditorGUILayoutUtil.DrawSeparator();

			if (!locked&&Selection.gameObjects.IsNotEmpty()&&(roots == null||!Enumerable.SequenceEqual(roots, Selection.gameObjects)))
			{
				Clear();
				// search labels
				roots = Selection.gameObjects;
				foreach (GameObject o in roots)
				{
					tabs.Clear();
					foreach (UITabHandler t in o.GetComponentsInChildren<UITabHandler>(true))
					{
						tabs.Add(new UITabHandlerInspectorImpl(t));
					}
				}
				foreach (GameObject o in roots)
				{
					// remove number labels
					HashSet<UIText> ignore = new HashSet<UIText>();

					foreach (DropDown dropdown in o.GetComponentsInChildren<DropDown>(true))
					{
						foreach (UIText l in dropdown.GetComponentsInChildren<UIText>(true))
						{
							ignore.Add(l);
						}
					}

					foreach (UIText l in o.GetComponentsInChildren<UIText>(true))
					{
						if (l.text.IsEmpty())
						{
							continue;
						}
						if (ignore.Contains(l))
						{
							continue;
						}
						if (visibleOnly&&(!l.gameObject.activeInHierarchy||!l.enabled))
						{
							continue;
						}
						foreach (char c in l.text)
						{
							if (char.IsLetter(c)&&!char.IsWhiteSpace(c)&&!char.IsPunctuation(c))
							{
								labels.Add(l);
								break;
							}
						}
					}
				}
			}
			foreach (UITabHandlerInspectorImpl inspector in tabs)
			{
				if (inspector.OnInspectorGUI())
				{
					Save();
					roots = null;
				}
			}

			// draw trigger list
			EditorGUILayout.BeginVertical();
			foreach (UIText l in labels)
			{
				if (!mod.ContainsKey(l))
				{
					FindKey(l);
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.ObjectField(l, typeof(UIText), true);
				Color oldColor = GUI.backgroundColor;
				string srcKey = mod.Get(l);
				if (srcKey == null)
				{
					srcKey = l.textKey;
				}
                if (!Lexicon.ContainsKey(srcKey))
				{
					GUI.backgroundColor = Color.red;
				}
				string dstKey = EditorGUILayout.TextField(srcKey);
				GUI.backgroundColor = oldColor;
				string text = EditorGUILayout.TextField(l.text);
				if (srcKey != dstKey)
				{
					mod[l] = dstKey;
				}
				if (text != l.text)
				{
					l.SetText(text);
                    EditorUtil.SetDirty(l);
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();

			if (GUILayout.Button("Save"))
			{
				Save();
			}
		}

		private void Save()
		{
			HashSet<GameObject> prefabs = new HashSet<GameObject>();
			foreach (var pair in mod)
			{
				pair.Key.textKey = pair.Value;
                EditorUtil.SetDirty(pair.Key);
				GameObject p = PrefabUtility.FindPrefabRoot(pair.Key.gameObject);
				if (p != null)
				{
					prefabs.Add(p);
				}
			}
			mod.Clear();
			if (prefabs.IsNotEmpty())
			{
				foreach (GameObject p in prefabs)
				{
					// apply prefab change
                    #if UNITY_2018_1_OR_LATER
                    PrefabUtility.ReplacePrefab(p, PrefabUtility.GetCorrespondingObjectFromSource(p), ReplacePrefabOptions.ConnectToPrefab);
                    #else
                    PrefabUtility.ReplacePrefab(p, PrefabUtility.GetCorrespondingObjectFromSource(p), ReplacePrefabOptions.ConnectToPrefab);
                    #endif
				}
				AssetDatabase.SaveAssets();
				EditorSceneBridge.SaveScene();
			}
		}

		public static void FindKey(UIText l)
		{
			string text = l.text;
			if (l.textKey.IsEmpty()&&!text.IsEmpty())
			{
                string key = Lexicon.FindAltKey(text);
				if (!key.IsEmpty())
				{
					l.textKey = key;
                    EditorUtil.SetDirty(l.gameObject);
				} else
				{
				}
			}
		}

		private void TranslateLanguage()
		{
			ReloadTable();
			BuildScript.ForEachScene(roots =>
			{
				foreach (Transform root in roots)
				{
					foreach (UIText l in root.GetComponentsInChildren<UIText>(true))
					{
						TranslateLabel(l);
					}
				}
				return null;
			});
			BuildScript.ForEachPrefab((path, popup) =>
			{
				foreach (UIText l in popup.GetComponentsInChildren<UIText>(true))
				{
					TranslateLabel(l);
				}
				return null;
			});
		}

		private static void TranslateLabel(UIText l)
		{
            string t = Lexicon.Translate(l.text);
			if (!t.IsEmpty())
			{
				l.SetText(t);
                EditorUtil.SetDirty(l);
			}
		}
	}
}
