using System.Collections.Generic;
using System.Collections.Generic.Ex;
using mulova.comunity;
using mulova.unicore;
using ngui.ex;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Ex;
using UnityEngine.SceneManagement;

[CustomEditor (typeof(TexSetter))]
public class TexSetterInspector : Editor
{
    private TexSetter setter;
    private TexLoader loader;

    void OnEnable()
    {
        this.setter = target as TexSetter;
        this.loader = setter.GetComponent<TexLoader>();
        if (setter.textures.IsEmpty())
        {
            CopyTexture(setter, loader);
        }
    }

    public static void CopyTexture(TexSetter s, TexLoader l)
    {
        if (l.Target.mainTexture != null&&AssetBundlePath.inst.IsCdnAsset(l.Target.mainTexture))
        {
            AssetRef r = new AssetRef();
            r.cdn = true;
            r.SetPath(l.Target.mainTexture);
            s.textures.Add(r);
            EditorUtil.SetDirty(s);
        }
    }

    private static int texIndex = -1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    
        if (loader.Target != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (loader.Target.mainTexture == null)
            {
                if (GUILayout.Button("Set"))
                {
                    setter.SetTexture(texIndex);
                }
                texIndex = EditorGUILayout.IntField(texIndex, GUILayout.Width(50));
            } else
            {
                if (GUILayout.Button("Clear"))
                {
                    TexLoaderBuildProcessor.ClearTexture(setter.GetLoader());
                }
                if (GUILayout.Button("Set Next"))
                {
                    SetNext();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Set All"))
        {
            SetAllTextures();
        }
        if (GUILayout.Button("Next All"))
        {
            SetAllNext();
        }
        if (GUILayout.Button("Clear All"))
        {
            ClearAllTextures();
        }
        EditorGUILayout.EndHorizontal();
    }

    public void SetNext()
    {
        if (setter.textures.Count <= 0)
        {
            return;
        }
        texIndex = (texIndex+1) % setter.textures.Count;
        setter.SetTexture(texIndex);
    }

    /// <summary>
    /// Clears all UITexture.mainTexture in TexSetter
    /// </summary>
    /// <returns><c>true</c>, if scene is changed, <c>false</c> otherwise.</returns>
    public static void ClearAllTextures()
    {
        foreach (var o in EditorSceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform root = o.transform;
            foreach (TexSetter s in root.GetComponentsInChildren<TexSetter>(true))
            {
                TexLoaderBuildProcessor.ClearTexture(s.GetLoader());
            }
        }
    }

    public static void SetAllTextures()
    {
        foreach (var o in EditorSceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform root = o.transform;
            foreach (TexSetter s in root.GetComponentsInChildren<TexSetter>(true))
            {
                s.SetTexture(texIndex);
            }
        }
    }

    public static void SetAllNext()
    {
        int size = int.MaxValue;
        List<TexSetter> setters = new List<TexSetter>();
        foreach (var o in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform root = o.transform;
            foreach (TexSetter s in root.GetComponentsInChildren<TexSetter>(true))
            {
                if (!s.textures.IsEmpty())
                {
                    setters.Add(s);
                    size = Mathf.Min(size, s.textures.Count);
                }
            }
        }
        if (size != int.MaxValue)
        {
            texIndex = (texIndex+1) % size;
            foreach (TexSetter s in setters)
            {
                s.SetTexture(texIndex);
            }
        }
    }

    public static void SetIfCdn(UITexture tex)
    {
        if (tex == null||tex.mainTexture == null||tex.GetComponent<TexSetter>() != null)
        {
            return;
        }
        /// Add TexSetter if texture is from CDN
        if (AssetBundlePath.inst.IsCdnAsset(tex.mainTexture))
        {
            TexLoader l = tex.FindComponent<TexLoader>();
            TexSetter s = l.FindComponent<TexSetter>();
            TexSetterInspector.CopyTexture(s, l);
        }
    }
}