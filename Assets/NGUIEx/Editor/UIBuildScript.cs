using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System;
using ngui.ex;
using build;
using commons;
using comunity;

public static class UIBuildScript {

	public static string FindTextKey(string keys) {
		HashSet<string> keySet = new HashSet<string>(keys.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries));
		StringBuilder result = new StringBuilder();
		result.Append("[From Resource]\n");
		BuildScript.ForEachPrefab((path,popup)=> {
			foreach (UIText l in popup.GetComponentsInChildren<UIText>(true)) {
				if (l.textKey.IsNotEmpty() && keySet.Contains(l.textKey))
				{
					result.AppendFormat("{0}: {1}\n", l.textKey, l.transform.GetScenePath());
				}
			}
			return null;
		});
		BuildScript.ForEachScene(roots=> {
			result.AppendFormat("[Scene {0}]\n", EditorSceneBridge.currentScene);
			foreach (Transform r in roots) {
				foreach (UIText l in r.GetComponentsInChildren<UIText>(true)) {
					if (l.textKey.IsNotEmpty() && keySet.Contains(l.textKey))
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
		BuildScript.ForEachPrefab((path,popup)=> {
			foreach (UITexture t in popup.GetComponentsInChildren<UITexture>(true)) {
				if (t.mainTexture == srcTex) {
					t.mainTexture = dstTex;
                    EditorUtil.SetDirty(t);
				}
			}
			return null;
		});
		BuildScript.ForEachScene(roots=> {
			foreach (Transform r in roots) {
				foreach (UITexture t in r.GetComponentsInChildren<UITexture>(true)) {
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
