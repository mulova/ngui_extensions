using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Ex;
using mulova.build;
using mulova.commons;
using mulova.unicore;
using ngui.ex;
using UnityEngine;
using UnityEngine.Ex;
using UnityEngine.SceneManagement;

public static class UIBuildScript {

	public static string FindTextKey(string keys) {
		HashSet<string> keySet = new HashSet<string>(keys.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries));
		StringBuilder result = new StringBuilder();
		result.Append("[From Resource]\n");
		EditorTraversal.ForEachAsset<GameObject>(FileType.Prefab, (path,popup)=> {
			foreach (UIText l in popup.GetComponentsInChildren<UIText>(true)) {
                if (!l.textKey.IsEmpty() && keySet.Contains(l.textKey))
				{
					result.AppendFormat("{0}: {1}\n", l.textKey, l.transform.GetScenePath());
				}
			}
		});
        EditorTraversal.ForEachScene(s=> {
			result.AppendFormat("[Scene {0}]\n", SceneManager.GetActiveScene().path);
			foreach (var r in s.GetRootGameObjects()) {
                var root = r.transform;
				foreach (UIText l in root.GetComponentsInChildren<UIText>(true)) {
					if (!l.textKey.IsEmpty() && keySet.Contains(l.textKey))
					{
						result.AppendFormat("{0}: {1}\n", l.textKey, l.transform.GetScenePath());
					}
				}
			}
			return null;
		});
		return result.ToString();
	}

	public static void ChangeTexture (Texture srcTex, Texture dstTex)
	{
        EditorTraversal.ForEachAsset<GameObject>(FileType.Prefab, (path,popup)=> {
			foreach (UITexture t in popup.GetComponentsInChildren<UITexture>(true)) {
				if (t.mainTexture == srcTex) {
					t.mainTexture = dstTex;
                    EditorUtil.SetDirty(t);
				}
			}
		});
        EditorTraversal.ForEachScene(s=> {
			foreach (var r in s.GetRootGameObjects()) {
                var root = r.transform;
				foreach (UITexture t in root.GetComponentsInChildren<UITexture>(true)) {
					if (t.mainTexture == srcTex) {
						t.mainTexture = dstTex;
                        EditorUtil.SetDirty(t);
					}
				}
			}
			return null;
		});
	}
}
