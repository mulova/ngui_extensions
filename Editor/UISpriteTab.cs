using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.IO;
using System.Text.Ex;
using mulova.build;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Ex;
using Object = UnityEngine.Object;

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
                atlasRefs = EditorAssetUtil.LoadReferencesFromFile<NGUIAtlas>(ATLAS_PATH);
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

        private List<NGUIAtlas> atlasRefs;
        private HashSet<NGUIAtlas> folding = new HashSet<NGUIAtlas>();

        public override void OnHeaderGUI()
        {
        }

        private NGUIAtlas atlasToAdd;
        private NGUIAtlas atlas4Sprite;
        private List<string> dupSprites = new List<string>();
        private string searchSpriteName;
        private GameObject targetObj;
        private NGUIAtlas changeAtlas;
        private List<UISprite> s4a = new List<UISprite>();

        public override void OnInspectorGUI()
        {
            if (EditorUI.DrawHeader("Sprite -> Texture"))
            {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayoutUtil.ObjectField<Object>("Folder", ref texFolder, false);
                if (GUILayout.Button("Sprite -> Texture")&&EditorUtility.DisplayDialog("Warning", "BackUp?", "OK", "Cancel"))
                {
                    ConvertToTexture(texFolder);
                }
                EditorGUILayout.EndHorizontal();
                var drawer = new ListDrawer<UITexture>(texList, new ObjListItemDrawer<UITexture>());
                drawer.Draw();
                EditorUI.EndContents();
            }
            if (EditorUI.DrawHeader("Texture -> Sprite"))
            {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                ComponentSelector.Draw<NGUIAtlas>("Atlas", atlasToAdd, OnSelectAtlas, true, GUILayout.MinWidth(80f));
                if (GUILayout.Button("Add Selected"))
                {
                    foreach (Object o in Selection.objects)
                    {
                        if (o is GameObject)
                        {
                            OnSelectAtlas((o as GameObject).GetComponent<NGUIAtlas>());
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayoutUtil.TextField("Search sprite", ref searchSpriteName);
                if (!searchSpriteName.IsEmpty())
                {
                    List<NGUIAtlas> filtered = new List<NGUIAtlas>();
                    foreach (NGUIAtlas a in atlasRefs)
                    {
                        if (a.GetSprite(searchSpriteName) != null)
                        {
                            filtered.Add(a);
                        }
                    }
                    var drawer = new ListDrawer<NGUIAtlas>(filtered, new ObjListItemDrawer<NGUIAtlas>());
                    drawer.Draw();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayoutUtil.Popup("Change to", ref changeAtlas, filtered);
                    if (GUILayout.Button("Apply"))
                    {
                        EditorTraversal.ForEachAsset<GameObject>(FileType.Prefab, (path, prefab) => {
                            ChangeAtlas(prefab, filtered, changeAtlas);
                        });
                        EditorTraversal.ForEachScene(s => {
                            foreach (var r in s.GetRootGameObjects())
                            {
                                ChangeAtlas(r.gameObject, filtered, changeAtlas);
                            }
                            return null;
                        });
                    }
                    EditorGUILayout.EndHorizontal();
                    var spriteDrawer = new ListDrawer<UISprite>(spriteList, new ObjListItemDrawer<UISprite>());
                    spriteDrawer.Draw();
                } else
                {
                    var drawer = new ListDrawer<NGUIAtlas>(atlasRefs, new ObjListItemDrawer<NGUIAtlas>());
                    if (drawer.Draw())
                    {
                        SaveAtlasRefs();
                    }
                }
                if (!dupSprites.IsEmpty())
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
                EditorGUILayoutUtil.ObjectField("Target", ref targetObj, true);
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
                        MultiMap<NGUIAtlas, UISprite> collect = new MultiMap<NGUIAtlas, UISprite>();
                        foreach (UISprite s in targetObj.GetComponentsInChildren<UISprite>(true))
                        {
                            collect.Add(s.atlas as NGUIAtlas, s);
                        }
                        foreach (KeyValuePair<NGUIAtlas, List<UISprite>> pair in collect)
                        {
                            if (EditorGUILayout.Foldout(folding.Contains(pair.Key), pair.Key.name()))
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
                ComponentSelector.Draw<NGUIAtlas>("Atlas", atlas4Sprite, OnSelectAtlasForSprite, true, GUILayout.MinWidth(80f));
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

        private void ChangeAtlas(GameObject obj, List<NGUIAtlas> filtered, NGUIAtlas changeAtlas)
        {
            foreach (UISprite s in obj.GetComponentsInChildren<UISprite>(true))
            {
                if (s.spriteName == searchSpriteName&&changeAtlas != s.atlas&&filtered.Contains(s.atlas as NGUIAtlas))
                {
                    Debug.LogFormat("{0} ({1}): {2} -> {3}", s.transform.GetScenePath(), s.spriteName, s.atlas.name(), changeAtlas.name());
                    s.atlas = changeAtlas;
                    EditorUtil.SetDirty(s);
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

        private Dictionary<string, NGUIAtlas> atlasMap;

        private void CreateAtlasMap()
        {
            HashSet<string> dup = new HashSet<string>();
            atlasMap = new Dictionary<string, NGUIAtlas>();
            foreach (NGUIAtlas a in atlasRefs)
            {
                foreach (UISpriteData sprite in a.spriteList)
                {
                    NGUIAtlas dupAtlas = atlasMap.Get(sprite.name);
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
                NGUIAtlas atlas = atlasMap.Get(tex.mainTexture.name);
                if (atlas != null)
                {
                    var s = tex.ConvertToSprite();
                    s.atlas = atlas;
                    EditorUtil.SetDirty(s.atlas as Object);
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
            NGUIAtlas old = atlasToAdd;
            atlasToAdd = obj as NGUIAtlas;
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
            atlas4Sprite = obj as NGUIAtlas;
        }

        private void OnSelectAtlas(Object obj, ref NGUIAtlas a)
        {
            
        }
    }

}