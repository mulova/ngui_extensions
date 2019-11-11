
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace ngui.ex
{
    public class UITableLayoutInspectorImpl {
		private UITableLayout grid;
		private const float WIDTH = 70;
		private const float HEIGHT = 16;
		
		
		public UITableLayoutInspectorImpl(UITableLayout grid) {
			this.grid = grid;
		}
		
		public void OnEnable() {
			if (grid.InitArray()) {
				EditorUtil.SetDirty(grid);
			}
			if (grid.enabled) {
				grid.InvalidateLayout();
			}
		}
		
		public void OnInspectorGUI() {
			if (EditorGUILayoutUtil.ObjectField<GameObject>("Empty Obj", ref grid.emptyObj, true))
			{
				EditorUtil.SetDirty(grid.emptyObj);
			}
			OnGridGUI();
		}
		
		private Vector2 tableScrollPos;
		public void OnGridGUI() {
			if (grid == null) { return; }
			
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
			GUI.enabled = true;
			
			if (changed) {
				grid.InitArray();
				EditorUtil.SetDirty(grid);
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
				if (EditorGUILayoutUtil.PopupEnum<UITableLayout.HAlign>("Horizontal Align", ref grid.halign, GUILayout.ExpandWidth(false))) {
					for (int i=0; i<grid.haligns.Length; ++i) {
						grid.haligns[i] = grid.halign;
					}
					changed = true;
				}
				if (EditorGUILayoutUtil.PopupEnum<UITableLayout.VAlign>("Vertical Align", ref grid.valign, GUILayout.ExpandWidth(false))) {
					for (int i=0; i<grid.valigns.Length; ++i) {
						grid.valigns[i] = grid.valign;
					}
					changed = true;
				}
				changed |= EditorGUILayoutUtil.Vector2Field("Padding", ref grid.padding);
				if (grid.cellMinSize.x == 0 && grid.cellMinSize.y == 0) {
					changed |= EditorGUILayoutUtil.Vector2Field("Cell Size", ref grid.cellSize);
				}
				if (grid.cellSize.x == 0 && grid.cellSize.y == 0) {
					changed |= EditorGUILayoutUtil.Vector2Field("Cell Min Size", ref grid.cellMinSize);
				}
				changed |= EditorGUILayoutUtil.IntField("Total Width", ref grid.totalWidth);
				changed |= EditorGUILayoutUtil.Toggle("Resize Collider", ref grid.resizeCollider, GUILayout.ExpandWidth(false));
				if (grid.resizeCollider && grid.padding != Vector2.zero) {
					EditorGUI.indentLevel += 1;
					changed |= EditorGUILayoutUtil.Toggle("Expand Collider To Padding", ref grid.expandColliderToPadding, GUILayout.ExpandWidth(false));
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
				changed = EditorGUILayoutUtil.PopupEnum<UITableLayout.Arrangement>("Orientation", ref grid.arrangement, GUILayout.ExpandWidth(false));
				string rowCol = grid.isHorizontal? "Column Size": "Row Size";
				if (EditorGUILayoutUtil.IntField(rowCol, ref grid.maxPerLine, GUILayout.ExpandWidth(false))) {
					if (grid.maxPerLine <= 0) {
						grid.maxPerLine = 1;
					}
					changed = true;
				}
				changed |= EditorGUILayoutUtil.IntField("Row Header", ref grid.rowHeader, GUILayout.ExpandWidth(false));
				changed |= EditorGUILayoutUtil.IntField("Column Header", ref grid.columnHeader, GUILayout.ExpandWidth(false));
				changed |= EditorGUILayoutUtil.Toggle("Reuse Cell", ref grid.reuseCell, GUILayout.ExpandWidth(false));
				NGUIEditorTools.EndContents();
			}
			if (changed) {
				grid.InitArray();
			}
			return changed;
		}
		
		private bool DrawTableHeader() {
			bool changed = false;
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Size", GUILayout.Width(WIDTH+10));
			int col = grid.columnCount;
			for (int c=0; c<col; c++) {
				EditorGUILayout.LabelField(c.ToString(), EditorStyles.boldLabel, GUILayout.Width(30));
				changed |= EditorGUILayoutUtil.IntField(null, ref grid.columnWidth[c], GUILayout.Width(WIDTH-30));
			}
			if (grid.isHorizontal) { EditorGUILayout.LabelField("", GUILayout.Width(42)); }
			EditorGUILayout.EndHorizontal();
			return changed;
		}
	}
}