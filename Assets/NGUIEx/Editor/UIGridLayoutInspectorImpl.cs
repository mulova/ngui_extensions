using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using comunity;

namespace ngui.ex {
	public class UIGridLayoutInspectorImpl {
		private UIGridLayout grid;
		private const float WIDTH = 70;
		private const float HEIGHT = 16;
		
		
		public UIGridLayoutInspectorImpl(UIGridLayout grid) {
			this.grid = grid;
		}
		
		private void RefreshArrInspector() {
			if (grid != null && (bgPrefab == null || grid.rowBg != bg.Arr)) {
				bgPrefab = new ObjectRefArray<Transform>(grid, "rowBgPrefab");
				bgPrefab.title = "Background Row Prefab";
				bgPrefab.SetVertical(false);
				bgPrefab.layout = GUILayout.ExpandWidth(false);
				
				bg = new ObjectRefArray<Transform>(grid, "rowBg");
				bg.title = "Background Row";
				bg.resizable = false;
				bg.SetVertical(false);
				bg.layout = GUILayout.ExpandWidth(false);
			}
		}
		
		public void OnEnable() {
			if (grid.InitArray()) {
				CompatibilityEditor.SetDirty(grid);
			}
			if (grid.enabled) {
				grid.InvalidateLayout();
			}
		}
		
		public void OnInspectorGUI() {
			if (EditorGUIUtil.ObjectField<GameObject>("Empty Obj", ref grid.emptyObj, true))
			{
				CompatibilityEditor.SetDirty(grid.emptyObj);
			}
			OnGridGUI();
		}
		
		private ObjectRefArray<Transform> bgPrefab;
		private ObjectRefArray<Transform> bg;
		private Vector2 tableScrollPos;
		public void OnGridGUI() {
			if (grid == null) { return; }
			RefreshArrInspector();
			
			bool changed = false;
			if (grid.maxPerLine <= 0) {
				grid.maxPerLine = 1;
				changed = true;
			}
			
			if (changed) {
				grid.InitArray();
			}
			
			changed |= DrawTableStructure();
			changed |= DrawSize();
			changed |= DrawBackgroundSetting();
			GUI.enabled = true;
			
			if (changed) {
				grid.InitArray();
				CompatibilityEditor.SetDirty(grid);
				if (grid.enabled) {
					NGUIUtil.Reposition(grid.transform);
					grid.Reposition();
				}
			}
		}
		
		private bool DrawSize() {
			bool changed = false;
			if (NGUIEditorTools.DrawHeader("Size"))
			{
				NGUIEditorTools.BeginContents();
				if (EditorGUIUtil.PopupEnum<UIGridLayout.HAlign>("Horizontal Align", ref grid.halign, GUILayout.ExpandWidth(false))) {
					for (int i=0; i<grid.haligns.Length; ++i) {
						grid.haligns[i] = grid.halign;
					}
					changed = true;
				}
				if (EditorGUIUtil.PopupEnum<UIGridLayout.VAlign>("Vertical Align", ref grid.valign, GUILayout.ExpandWidth(false))) {
					for (int i=0; i<grid.valigns.Length; ++i) {
						grid.valigns[i] = grid.valign;
					}
					changed = true;
				}
				changed |= EditorGUIUtil.Vector2Field("Padding", ref grid.padding);
				if (grid.cellMinSize.x == 0 && grid.cellMinSize.y == 0) {
					changed |= EditorGUIUtil.Vector2Field("Cell Size", ref grid.cellSize);
				}
				if (grid.cellSize.x == 0 && grid.cellSize.y == 0) {
					changed |= EditorGUIUtil.Vector2Field("Cell Min Size", ref grid.cellMinSize);
				}
				changed |= EditorGUIUtil.IntField("Total Width", ref grid.totalWidth);
				changed |= EditorGUIUtil.Toggle("Resize Collider", ref grid.resizeCollider, GUILayout.ExpandWidth(false));
				if (grid.resizeCollider && grid.padding != Vector2.zero) {
					EditorGUI.indentLevel += 1;
					changed |= EditorGUIUtil.Toggle("Expand Collider To Padding", ref grid.expandColliderToPadding, GUILayout.ExpandWidth(false));
					EditorGUI.indentLevel -= 1;
				}
				NGUIEditorTools.EndContents();
			}
			return changed;
		}
		
		private bool DrawTableStructure() {
			bool changed = false;
			if (NGUIEditorTools.DrawHeader("Table Structure"))
			{
				NGUIEditorTools.BeginContents();
				EditorGUILayout.HelpBox("Modifying structure values may occur unintented result", MessageType.Warning);
				changed = EditorGUIUtil.PopupEnum<UIGridLayout.Arrangement>("Orientation", ref grid.arrangement, GUILayout.ExpandWidth(false));
				string rowCol = grid.IsHorizontal()? "Column Size": "Row Size";
				if (EditorGUIUtil.IntField(rowCol, ref grid.maxPerLine, GUILayout.ExpandWidth(false))) {
					if (grid.maxPerLine <= 0) {
						grid.maxPerLine = 1;
					}
					changed = true;
				}
				changed |= EditorGUIUtil.IntField("Row Header", ref grid.rowHeader, GUILayout.ExpandWidth(false));
				changed |= EditorGUIUtil.IntField("Column Header", ref grid.columnHeader, GUILayout.ExpandWidth(false));
				changed |= EditorGUIUtil.Toggle("Reuse Cell", ref grid.reuseCell, GUILayout.ExpandWidth(false));
				NGUIEditorTools.EndContents();
			}
			if (changed) {
				grid.InitArray();
			}
			return changed;
		}
		
		private Color bgColorTint = Color.white;
		private bool DrawBackgroundSetting() {
			bool changed = false;
			if (NGUIEditorTools.DrawHeader("Background Settings"))
			{
				NGUIEditorTools.BeginContents();
				EditorGUILayout.BeginVertical();
				changed |= EditorGUIUtil.ObjectField<UIWidget>("Main", ref grid.background, true);
				changed |= EditorGUIUtil.Vector4Field("Padding", ref grid.backgroundPadding);
				if (bgPrefab.OnInspectorGUI()) {
					changed = true;
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
				if (bg.OnInspectorGUI()) {
					foreach (Transform t in bg) {
						if (!t.IsChildOf(grid.transform)) {
							t.parent = grid.transform;
						}
					}
					changed = true;
				}
				EditorGUILayout.EndHorizontal();
				
				if (EditorGUIUtil.ColorField("Bg Color Tint", ref bgColorTint, GUILayout.ExpandWidth(false))) {
					int lineCount = grid.GetBackgroundRowCount();
					for (int i=0; i<lineCount; i++) {
						Transform t = grid.GetBackground(i);
						if (t != null) {
							UISprite sprite = t.GetComponent<UISprite>();
							sprite.color = bgColorTint;
							sprite.MarkAsChanged();
						}
					}
				}
				NGUIEditorTools.EndContents();
			}
			return changed;
		}
		
		private bool DrawTableHeader() {
			bool changed = false;
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Size", GUILayout.Width(WIDTH+10));
			int col = grid.GetColumnCount();
			for (int c=0; c<col; c++) {
				EditorGUILayout.LabelField(c.ToString(), EditorStyles.boldLabel, GUILayout.Width(30));
				changed |= EditorGUIUtil.IntField(null, ref grid.columnWidth[c], GUILayout.Width(WIDTH-30));
			}
			if (grid.IsHorizontal()) { EditorGUILayout.LabelField("", GUILayout.Width(42)); }
			EditorGUILayout.EndHorizontal();
			return changed;
		}
	}
}