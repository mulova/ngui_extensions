using System.Collections.Generic;
using commons;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System.Text;
using comunity;
using build;

namespace ngui.ex
{
    public class UISpriteTab : EditorTab
    {

        private Object texFolder;

        public UISpriteTab(TabbedEditorWindow window) : base ("Sprite", window)
        {
        }

        private const string ATLAS_PATH = "atlas_list";

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
        }

        public override void OnFocus(bool focus)
        {
        }

        public override void OnSelected(bool sel)
        {
            if (sel)
            {
                atlasRefs = EditorAssetUtil.LoadReferencesFromFile<UIAtlas>(ATLAS_PATH);
                CreateAtlasMap();
            }
        }

        public override void OnSelectionChange()
        {
            if (Selection.activeGameObject != null)
            {
                UITexture tex = Selection.activeGameObject.GetComponent<UITexture>();
                if (tex != null)
                {
                    if (tex.mainTexture != null)
                    {
                        searchSpriteName = tex.mainTexture.name;
                    }
                }
            }
        }

        private void Refresh()
        {
        }

        private List<UIAtlas> atlasRefs;
        private HashSet<UIAtlas> folding = new HashSet<UIAtlas>();

        public override void OnHeaderGUI()
        {
        }

        private UIAtlas atlasToAdd;
        private UIAtlas atlas4Sprite;
        private List<string> dupSprites = new List<string>();
        private string searchSpriteName;
        private GameObject targetObj;
        private UIAtlas changeAtlas;
        private List<UISprite> s4a = new List<UISprite>();

        public override void OnInspectorGUI()
        {
            if (EditorUI.DrawHeader("Sprite -> Texture"))
            {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtil.ObjectField<Object>("Folder", ref texFolder, false);
                if (GUILayout.Button("Sprite -> Texture")&&EditorUtility.DisplayDialog("Warning", "BackUp?", "OK", "Cancel"))
                {
                    ConvertToTexture(texFolder);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUIUtil.ObjectFieldReorderList(texList);
                EditorUI.EndContents();
            }
            if (EditorUI.DrawHeader("Texture -> Sprite"))
            {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                ComponentSelector.Draw<UIAtlas>("Atlas", atlasToAdd, OnSelectAtlas, true, GUILayout.MinWidth(80f));
                if (GUILayout.Button("Add Selected"))
                {
                    foreach (Object o in Selection.objects)
                    {
                        if (o is GameObject)
                        {
                            OnSelectAtlas((o as GameObject).GetComponent<UIAtlas>());
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUIUtil.TextField("Search sprite", ref searchSpriteName);
                if (searchSpriteName.IsNotEmpty())
                {
                    List<UIAtlas> filtered = new List<UIAtlas>();
                    foreach (UIAtlas a in atlasRefs)
                    {
                        if (a.GetSprite(searchSpriteName) != null)
                        {
                            filtered.Add(a);
                        }
                    }
                    EditorGUIUtil.ObjectFieldReorderList(filtered);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtil.Popup("Change to", ref changeAtlas, filtered);
                    if (GUILayout.Button("Apply"))
                    {
                        BuildScript.ForEachPrefab((path, prefab) => {
                            ChangeAtlas(prefab, filtered, changeAtlas);
                            return null;
                        });
                        BuildScript.ForEachScene(list => {
                            foreach (Transform t in list)
                            {
                                ChangeAtlas(t.gameObject, filtered, changeAtlas);
                            }
                            return null;
                        });
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUIUtil.ObjectFieldReorderList(spriteList);
                } else
                {
                    if (EditorGUIUtil.ObjectFieldReorderList(atlasRefs))
                    {
                        SaveAtlasRefs();
                    }
                }
                if (dupSprites.IsNotEmpty())
                {
                    if (EditorUI.DrawHeader("Duplicate sprites"))
                    {
                        EditorUI.BeginContents();
                        float cellWidth = 200f;
                        float width = GetWidth();
                        int column = Mathf.Max((int)(width / cellWidth), 1);
                        int i = 0;
                        foreach (string d in dupSprites)
                        {
                            if (i == 0)
                            {
                                EditorGUILayout.BeginHorizontal();
                            }
                            if (GUILayout.Button(d, GUILayout.Width(200)))
                            {
                                searchSpriteName = d;
                            }
                            i = i+1;
                            if (i == column)
                            {
                                EditorGUILayout.EndHorizontal();
                                i = 0;
                            }
                        }
                        if (i != 0)
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorUI.EndContents();
                    }
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtil.ObjectField("Target", ref targetObj, true);
                GUI.enabled = targetObj != null;
                if (GUILayout.Button("Convert to Sprite")&&EditorUtility.DisplayDialog("Warning", "BackUp?", "OK"))
                {
                    ConvertToSprite();
                }
                if (GUILayout.Button("Set TexSetter"))
                {
                    foreach (UITexture tex in targetObj.GetComponentsInChildren<UITexture>(true))
                    {
                        TexSetterInspector.SetIfCdn(tex);
                    }
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                // collect atlas
                GUI.enabled = targetObj != null;
                if (EditorUI.DrawHeader("Member Atlases"))
                {
                    EditorUI.BeginContents();
                    if (targetObj != null)
                    {
                        MultiMap<UIAtlas, UISprite> collect = new MultiMap<UIAtlas, UISprite>();
                        foreach (UISprite s in targetObj.GetComponentsInChildren<UISprite>(true))
                        {
                            collect.Add(s.atlas, s);
                        }
                        foreach (KeyValuePair<UIAtlas, List<UISprite>> pair in collect)
                        {
                            if (EditorGUILayout.Foldout(folding.Contains(pair.Key), pair.Key.name))
                            {
                                folding.Add(pair.Key);
                                EditorGUI.indentLevel++;
                                foreach (UISprite s in pair.Value)
                                {
                                    EditorGUILayout.ObjectField(s.gameObject, typeof(GameObject), true);
                                }
                                EditorGUI.indentLevel--;
                            } else
                            {
                                folding.Remove(pair.Key);
                            }
                        }
                    }
                    EditorUI.EndContents();
                }
                if (EditorUI.DrawHeader("Orphan Texture"))
                {
                    EditorUI.BeginContents();
                    if (targetObj != null)
                    {
                        foreach (UITexture tex in targetObj.GetComponentsInChildren<UITexture>(true))
                        {
                            if (tex.GetComponent<TexLoader>() == null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.ObjectField(tex.gameObject, typeof(GameObject), true);
                                EditorGUILayout.ObjectField(tex.mainTexture, typeof(Texture), false);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorUI.EndContents();
                }
                GUI.enabled = true;

                EditorUI.EndContents();
            }
            if (EditorUI.DrawHeader("Find All Sprites"))
            {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                ComponentSelector.Draw<UIAtlas>("Atlas", atlas4Sprite, OnSelectAtlasForSprite, true, GUILayout.MinWidth(80f));
                if (GUILayout.Button("Find"))
                {
                    var list = Resources.FindObjectsOfTypeAll<UISprite>().ToList(i => i as UISprite);
                    s4a.Clear();
                    foreach (var s in list)
                    {
                        if (s.atlas == atlas4Sprite)
                        {
                            s4a.Add(s);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorUI.EndContents();
            }
            GUI.enabled = true;
        }

        private void ChangeAtlas(GameObject obj, List<UIAtlas> filtered, UIAtlas changeAtlas)
        {
            foreach (UISprite s in obj.GetComponentsInChildren<UISprite>(true))
            {
                if (s.spriteName == searchSpriteName&&changeAtlas != s.atlas&&filtered.Contains(s.atlas))
                {
                    Debug.LogFormat("{0} ({1}): {2} -> {3}", s.transform.GetScenePath(), s.spriteName, s.atlas.name, changeAtlas.name);
                    s.atlas = changeAtlas;
                    BuildScript.SetDirty(s);
                }
            }
        }

        private void SaveAtlasRefs()
        {
            EditorAssetUtil.SaveReferences(ATLAS_PATH, atlasRefs);
        }

        public override void OnFooterGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }

        private List<UITexture> texList = new List<UITexture>();

        private void ConvertToTexture(Object folder)
        {
            Dictionary<string, string> map = FindTexturePaths(folder);
            
            texList = new List<UITexture>();
            foreach (GameObject o in Selection.gameObjects)
            {
                foreach (UISprite s in o.GetComponentsInChildren<UISprite>(true))
                {
                    string imgPath = map.Get(s.spriteName);
                    GameObject obj = s.gameObject;
                    
                    if (imgPath != null)
                    {
                        UITexture tex = s.ConvertToTexture();
                        texList.Add(tex);
                        tex.mainTexture = AssetDatabase.LoadAssetAtPath(imgPath, typeof(Texture)) as Texture;
                    }
                }
            }
        }

        private Dictionary<string, string> FindTexturePaths(Object folder)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach (string p in EditorAssetUtil.ListAssetPaths(AssetDatabase.GetAssetPath(folder), FileType.All))
            {
                if (FileTypeEx.GetFileType(p) == FileType.Image)
                {
                    map[Path.GetFileNameWithoutExtension(p)] = p;
                }
            }
            return map;
        }

        private Dictionary<string, UIAtlas> atlasMap;

        private void CreateAtlasMap()
        {
            HashSet<string> dup = new HashSet<string>();
            atlasMap = new Dictionary<string, UIAtlas>();
            foreach (UIAtlas a in atlasRefs)
            {
                foreach (UISpriteData sprite in a.spriteList)
                {
                    UIAtlas dupAtlas = atlasMap.Get(sprite.name);
                    if (dupAtlas != null)
                    {
                        dup.Add(sprite.name);
                    } else
                    {
                        atlasMap[sprite.name] = a;
                    }
                }
            }
            dupSprites = new List<string>(dup);
            dupSprites.Sort();
        }

        private List<UISprite> spriteList = new List<UISprite>();

        private void ConvertToSprite()
        {
            spriteList = new List<UISprite>();
            foreach (UITexture tex in targetObj.GetComponentsInChildren<UITexture>(true))
            {
                if (tex.mainTexture == null)
                    continue;
                TexSetter setter = tex.GetComponent<TexSetter>();
                TexLoader loader = tex.GetComponent<TexLoader>();
                if (setter != null&&setter.textures.Count == 1)
                {
                    if (!AssetBundlePath.inst.IsCdnAsset(loader.Target.mainTexture))
                    {
                        setter.DestroyEx();
                        loader.DestroyEx();
                    }
                }
                if (loader != null)
                {
                    continue;
                }
                UIAtlas atlas = atlasMap.Get(tex.mainTexture.name);
                if (atlas != null)
                {
                    var s = tex.ConvertToSprite();
                    s.atlas = atlas;
                    EditorUtil.SetDirty(s.atlas);
                    spriteList.Add(s);
                }
            }
        }

        private void OnSelectAtlas(Object obj)
        {
            if (obj == null)
            {
                return;
            }
            UIAtlas old = atlasToAdd;
            atlasToAdd = obj as UIAtlas;
            if (atlasToAdd != old)
            {
                if (atlasToAdd != null)
                {
                    if (!atlasRefs.Contains(atlasToAdd))
                    {
                        atlasRefs.Add(atlasToAdd);
                        SaveAtlasRefs();
                    }
                }
            }
            CreateAtlasMap();
        }

        private void OnSelectAtlasForSprite(Object obj)
        {
            if (obj == null)
            {
                return;
            }
            atlas4Sprite = obj as UIAtlas;
        }

        private void OnSelectAtlas(Object obj, ref UIAtlas a)
        {
            
        }
    }

}