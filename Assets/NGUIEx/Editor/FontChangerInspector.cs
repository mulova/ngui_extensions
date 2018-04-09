#if FULL
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System.Collections.Generic;

[CustomEditor(typeof(FontChanger))]
public class FontChangerInspector : Editor
{
	private FontChanger changer;
	private FontChangeDataArrInspector inspector;
	
	void OnEnable() {
		changer = target as FontChanger;
		inspector = new FontChangeDataArrInspector(changer, "changes");
	}
	
	public override void OnInspectorGUI() {
		EditorGUIUtil.PopupEnum<SystemLanguage>("Language", ref changer.lang);
		inspector.OnInspectorGUI();

		EditorGUILayout.BeginHorizontal();
		
		if (changer.IsRevertible()) {
			if (GUILayout.Button("Revert")) {
				changer.Revert();
			}
		} else {
			 if (GUILayout.Button("Commit")) {
				changer.Commit();
			}
		}
		EditorGUILayout.EndHorizontal();
	}
}

public class FontChangeDataArrInspector : ArrInspector<FontChangeData>
{
	private Dictionary<FontChangeData, ArrayDrawer<UIFont>> arrMap = new Dictionary<FontChangeData, ArrayDrawer<UIFont>>();
	public FontChangeDataArrInspector(Object obj, string varName) : base(obj, varName) { 
		SetTitle(null);
	}
	
	protected override bool OnInspectorGUI(FontChangeData data, int i)
	{
		bool changed = false;
		EditorGUILayout.BeginVertical();
		if (EditorGUIUtil.ObjectField<Font>("DynamicFont", ref data.font, false)) {
			changed = true;
			if (data.font != null) {
				data.uiFont = null;
			}
		}
		if (EditorGUIUtil.ObjectField<UIFont>("Reference Font", ref data.uiFont, false)) {
			changed = true;
			if (data.uiFont != null) {
				data.font = null;
			}
		}
		ArrayDrawer<UIFont> arr = null;
		if (!arrMap.TryGetValue(data, out arr)) {
            arr = new ArrayDrawer<UIFont>(data, "references");
            arr.allowSceneObject = false;
			arrMap[data] = arr;
		}
		arr.OnInspectorGUI();
		EditorGUILayout.EndVertical();
		return changed;
	}

	protected override bool DrawFooter() {
		return false;
	}
}
#endif