#if FULL
using UnityEditor;
using UnityEngine;
using System;
#if TEXTMESH_PRO 
using TMPro;
#endif

[CustomEditor(typeof(TextLayer))]
public class TextLayerInspector : Editor
{
	private UIPanel panel;
#if TEXTMESH_PRO 
	private TextMeshPro[] text;

	private bool IsChanged(TextMeshPro t) {
		return t.sortingLayerID != sortingLayerId || t.GetComponent<Renderer>().sortingLayerID != sortingLayerId;
	}
#endif
	private int sortingLayerId;

	void OnEnable() {
		TextLayer comp = target as TextLayer;
		panel = comp.GetComponentInParent<UIPanel>();
#if TEXTMESH_PRO 
		text = comp.GetComponentsInChildren<TextMeshPro>();
#endif
		if (panel.drawCalls != null) {
			foreach (UIDrawCall dc in panel.drawCalls) {
				if (dc != null && dc.GetComponent<Renderer>() != null) {
					sortingLayerId = Math.Max(sortingLayerId, dc.GetComponent<Renderer>().sortingLayerID);
				}
			}
		}
	}

	public override void OnInspectorGUI() {
		if (panel == null) {
			return;
		}
		bool changed = false;
#if TEXTMESH_PRO 
		foreach (TextMeshPro t in text) {
			if (IsChanged(t)) {
				changed = true;
			}
		}
		GUI.enabled = changed;
		if (GUILayout.Button("Set TextMeshPro SortingLayer")) {
			Undo.RecordObjects(text, "TextMeshPro SortingLayer");
			foreach (TextMeshPro t in text) {
				if (IsChanged(t)) {
					t.sortingLayerID = sortingLayerId;
					t.GetComponent<Renderer>().sortingLayerID = sortingLayerId;
					CompatibilityEditor.SetDirty(t);
				}
			}
		}
#endif
		GUI.enabled = true;
	}
}
#endif