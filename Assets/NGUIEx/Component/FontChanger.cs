#if FULL
//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System;


namespace ngui.ex {
/// <summary>
/// Replace ttf or BMFont reference in prefab
/// </summary>
public class FontChanger : MonoBehaviour
{
	public SystemLanguage lang = SystemLanguage.Korean;
	public FontChangeData[] changes = new FontChangeData[0];
	
	/// <summary>
	/// Change ttf or bitmap directly
	/// </summary>
	public void Commit() {
		foreach (FontChangeData d in changes) {
			d.Commit();
		}
	}
	
	public void Revert() {
		foreach (FontChangeData d in changes) {
			d.Revert();
		}
	}
	
	public bool IsRevertible() {
		foreach (FontChangeData d in changes) {
			if (!d.IsRevertible()) {
				return false;
			}
		}
		return true;
	}
}

[Serializable]
public class FontChangeData : ICloneable {
	public UIFont[] references = new UIFont[0];
	public Font font;
	private Font[] dynFontBackup;
	private UIFont[] uiFontBackup;
	public UIFont uiFont;
	
	public void Commit() {
		dynFontBackup = new Font[references.Length];
		uiFontBackup = new UIFont[references.Length];
		for (int i=0; i<references.Length; ++i) {
			UIFont f = references[i];
			dynFontBackup[i] = f.dynamicFont;
			uiFontBackup[i] = f.replacement;
			if (font != null) {
				f.dynamicFont = font;
			} else if (uiFont != null) {
				f.replacement = uiFont;
			} else {
				Assert.Fail(null, "Font Replacement Error. {0}", f.name);
			}
		}
	}
	
	public bool IsRevertible() {
		return dynFontBackup != null;
	}
	
	public void Revert() {
		if (dynFontBackup == null) {
			return;
		}
		for (int i=0; i<dynFontBackup.Length; ++i) {
			UIFont f = references[i];
			if (font != null) {
				f.dynamicFont = dynFontBackup[i];
			} else {
				f.replacement = uiFontBackup[i];
			}
#if UNITY_EDITOR
			GameObject prefab = UnityEditor.PrefabUtility.FindPrefabRoot(f.gameObject);
			if (prefab != null) {
				UnityEditor.PrefabUtility.RevertPrefabInstance(prefab);
				continue;
			}
#endif
		}
		dynFontBackup = null;
		uiFontBackup = null;
	}
	
	public object Clone() {
		FontChangeData that = new FontChangeData();
		that.font = this.font;
		that.uiFont = this.uiFont;
		that.references = ArrayUtil.Clone<UIFont>(this.references);
		return that;
	}
}
}
#endif