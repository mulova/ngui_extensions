using System;
using System.Collections.Generic;
using mulova.build;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ngui.ex
{
    public static class UIAtlasMenu
    {
        [MenuItem("Assets/NGUI/Split Alpha(ETC)", false, 0)]
        public static void SplitChannelETC()
        {
            List<UIAtlas> atlases = new List<UIAtlas>();
            foreach (Object o in Selection.objects)
            {
                if (o is Material)
                {
                    SplitChannelETC(o as Material);
                } else
                {
                    GameObject go = o as GameObject;
                    if (go != null)
                    {
                        UIAtlas a = go.GetComponent<UIAtlas>();
                        if (a != null&&a.spriteMaterial != null&&a.spriteMaterial.mainTexture != null&&a.spriteMaterial.shader.name == ShaderId.UI)
                        {
                            atlases.Add(a);
                        }
                    }
                }
            }
            for (int i=0; i<atlases.Count; ++i)
            {
                EditorUtility.DisplayProgressBar("Split Alpha Channel", atlases[i].name, (float)((i+1) / atlases.Count));
                SplitChannelETC(atlases[i]);
            }
            EditorUtility.ClearProgressBar();
        }
        
        public static void SplitChannelETC(UIAtlas atlas)
        {
            while (atlas.replacement != null)
            {
                atlas = atlas.replacement;
            }
            if (atlas.spriteMaterial.shader.name == ShaderId.UI)
            {
                throw new NotImplementedException("Atlas size cannot be set");
//                atlas.width = atlas.spriteMaterial.mainTexture.width;
//                atlas.height = atlas.spriteMaterial.mainTexture.height;
            }
            SplitChannelETC(atlas.spriteMaterial);
            EditorUtil.SetDirty(atlas);
        }
        
        public static void SplitChannelETC(Material mat)
        {
            if (mat.shader.name != ShaderId.UI)
            {
                return;
            }
            string[] paths = TextureUtil.SplitChannel4ETC(mat.mainTexture);
            string rgbPath = paths[0];
            string aPath = paths[1];
            if (mat.mainTexture.width == mat.mainTexture.height)
            {
                mat.shader = Shader.Find(ShaderId.UI_SPLIT);
                mat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture>(rgbPath));
                mat.SetTexture("_AlphaTex", AssetDatabase.LoadAssetAtPath<Texture>(aPath));
            } else if (mat.mainTexture.width == mat.mainTexture.height * 2)
            {
                string appendPath = TextureUtil.AppendVertically(rgbPath, aPath);
                mat.shader = Shader.Find(ShaderId.UI_VERT);
                mat.SetVector("_Coord", new Vector4(0.5f, 0, 0.5f, 0.5f));
                mat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture>(appendPath));
            }

            EditorUtil.SetDirty(mat);
            AssetDatabase.SaveAssets();
        }
        
        [MenuItem("Assets/NGUI/Split Alpha(ETC)", true, 0)]
        public static bool IsSplitChannelETC()
        {
            foreach (Object o in Selection.objects)
            {
                Material mat = o as Material;
                if (mat == null)
                {
                    GameObject go = o as GameObject;
                    if (go == null)
                    {
                        return false;
                    }
                    UIAtlas a = go.GetComponent<UIAtlas>();
                    if (a == null)
                    {
                        return false;
                    }
                    mat = a.spriteMaterial;
                }
                if (mat == null||mat.mainTexture == null||mat.shader.name != ShaderId.UI)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

