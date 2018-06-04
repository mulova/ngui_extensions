using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;
using comunity;

namespace ngui.ex {
    public class UITableContentsTab : EditorTab {
		private UITableLayoutInspectorImpl inspector;
        private UITableLayout table;
		private const float WIDTH = 70;
		private const float HEIGHT = 16;
		private GUILayoutOption W_Option = GUILayout.Width(WIDTH);
		private GUILayoutOption H_Option = GUILayout.Height(HEIGHT);
		
		public UITableContentsTab(TabbedEditorWindow window) : base("Table", window) {
		}
		
		public void SetLayout(UITableLayout table) {
			this.table = table;
			this.inspector = new UITableLayoutInspectorImpl(table);
			this.inspector.OnEnable();
		}
		
		public override void OnEnable() {
			GameObject sel = Selection.activeGameObject;
			if (sel != null) {
				UITableLayout layout = sel.GetComponent<UITableLayout>();
				if (layout != null) {
					table = layout;
					SetLayout(table);
				}
			}
		}
		
		public override void OnFocus(bool focus) {
			OnEnable();
		}
		
		public override void OnDisable() { }

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnSelected(bool sel) { }
		
		public override void OnHeaderGUI() { }
		
		private UILabel titleLabelPrefab;
		public override void OnInspectorGUI() {
			if (table == null) {
				return;
			}
			bool changed = false;
			
			
			EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH*table.columnCount));
			changed |= DrawRowHeader();
			
			changed |= DrawColumn();
			changed |= DrawRowButton();
			EditorGUILayout.EndHorizontal();
			if (table.components.Length == 0) {
				if (GUILayout.Button("Click To Add Line")) {
                    table.components = new UITableCell[table.maxPerLine];
					table.InitArray();
				}
			}
			
			if (changed) {
				table.InvalidateLayout();
				EditorUtil.SetDirty(table);
			}
		}
		
		public override void OnFooterGUI() { }
		
		private bool IsHorizontal() {
			return table.isHorizontal;
		}
		
		private bool DrawRowHeader() {
			bool changed = false;
			EditorGUILayout.BeginVertical();
            changed |= EditorGUIUtil.ObjectField<UITableCell>(ref table.defaultPrefab, true, W_Option, H_Option);
			EditorGUILayout.LabelField("Align", EditorStyles.boldLabel, H_Option);
			EditorGUILayout.LabelField("Size", EditorStyles.boldLabel, H_Option);
			EditorGUILayout.LabelField("Index", EditorStyles.boldLabel, H_Option);
			int row = table.isVertical? table.GetMaxPerLine(): table.rowCount;
			table.InitArray();
			for (int r=0; r<row; r++) {
				EditorGUILayout.BeginHorizontal();
                changed |= EditorGUIUtil.ObjectField<UITableCell>(ref table.rowPrefab[r], true, GUILayout.Width(WIDTH-20), H_Option);
                changed |= EditorGUIUtil.Popup<UITableLayout.VAlign>(ref table.valigns[r], EnumUtil.Values<UITableLayout.VAlign>(), GUILayout.Width(40), H_Option);
				changed |= EditorGUIUtil.IntField(null, ref table.rowHeight[r], GUILayout.Width(WIDTH-30), H_Option);
				EditorGUILayout.LabelField((r+1).ToString(), EditorStyles.boldLabel, GUILayout.Width(20), H_Option);
				EditorGUILayout.EndHorizontal();
			}
			if (!IsHorizontal()) {
				EditorGUILayout.LabelField("", W_Option);
				EditorGUILayout.LabelField("", W_Option);
				EditorGUILayout.LabelField("", W_Option);
			}
			EditorGUILayout.EndVertical();
			if (changed) {
				table.InitArray();
			}
			return changed;
		}
		
		private bool DrawColumn() {
			bool changed = false;
			int columnSize = table.isHorizontal? table.GetMaxPerLine(): table.columnCount;
			int contentSize = table.columnCount;
			for (int c=0; c<columnSize; c++) {
				EditorGUILayout.BeginVertical();
                changed |= EditorGUIUtil.ObjectField<UITableCell>(ref table.columnPrefab[c], true, GUILayout.ExpandWidth(false), H_Option);
                changed |= EditorGUIUtil.Popup<UITableLayout.HAlign>(ref table.haligns[c], EnumUtil.Values<UITableLayout.HAlign>(), GUILayout.Width(WIDTH-20), H_Option);
				changed |= EditorGUIUtil.IntField(null, ref table.columnWidth[c], GUILayout.Width(WIDTH-20), H_Option);
				EditorGUILayout.LabelField(c.ToString(), EditorStyles.boldLabel, W_Option, H_Option);
				int row = table.rowCount;
				if (c < contentSize) {
					for (int r=0; r<row; r++) {
						changed |= DrawCell(r, c);
					}
					
					// Draw Column +/- buttons
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("A", "Add Selected"), GUILayout.ExpandWidth(false), H_Option)) {
						AddSelected(c, UITableLayout.Arrangement.Vertical);
						changed = true;
					}
					if (GUILayout.Button("+", GUILayout.ExpandWidth(false), H_Option)) {
                        #pragma warning disable 0618
                        table.AddColumn(c+1, new UITableCell[table.rowCount]);
                        #pragma warning restore 0618
						changed = true;
					}
					GUI.enabled = c >= table.columnHeader;
					if (GUILayout.Button("-", GUILayout.ExpandWidth(false), H_Option)) {
						if (EditorUtility.DisplayDialog("Confirm", "Delete column "+(c+1), "OK", "Cancel")) {
                            #pragma warning disable 0618
                            table.RemoveColumn(c);
                            #pragma warning restore 0618
                            table.Reposition();
							changed = true;
						}
					}
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();
				}
				
				EditorGUILayout.EndVertical();
			}
			return changed;
		}
		
		private bool DrawRowButton() {
			bool changed = false;
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("", W_Option, H_Option);
			EditorGUILayout.LabelField("", W_Option, H_Option);
			EditorGUILayout.LabelField("", W_Option, H_Option);
			EditorGUILayout.LabelField("", W_Option, H_Option);
			int row = table.rowCount;
			for (int r=0; r<row; r++) {
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("A", "Add Selected"), EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.Height(HEIGHT-1))) {
					AddSelected(r, UITableLayout.Arrangement.Horizontal);
					changed = true;
				}
				if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.Height(HEIGHT-1))) {
                    #pragma warning disable 0618
                    table.AddRow(r+1, new UITableCell[table.columnCount]);
                    #pragma warning restore 0618
					changed = true;
				}
				GUI.enabled = r >= table.rowHeader;
				if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.Height(HEIGHT-1))) {
					if (EditorUtility.DisplayDialog("Confirm", "Delete row "+(r+1), "OK", "Cancel")) {
                        #pragma warning disable 0618
                        table.RemoveRow(r);
                        #pragma warning restore 0618
                        table.Reposition();
						changed = true;
						row--;
					}
				}
				GUI.enabled = true;
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.LabelField("");
			EditorGUILayout.LabelField("");
			EditorGUILayout.LabelField("");
			EditorGUILayout.EndVertical();
			return changed;
		}
		
		private bool DrawCell(int r, int c) {
			bool changed = false;
            UITableCell cell = table.GetCell(r, c);
            if (EditorGUIUtil.ObjectField<UITableCell>(ref cell, true, W_Option, GUILayout.Height(HEIGHT-1))) {
				table.SetCell(r, c, cell);
				changed = true;
			}
			return changed;
		}
		
		private void AddSelected(int line, UITableLayout.Arrangement orientation) {
			GameObject[] objs = new GameObject[Selection.gameObjects.Length];
			Array.Copy(Selection.gameObjects, objs, objs.Length);
			Array.Sort(objs, new HVComparer(orientation));
			if (orientation == UITableLayout.Arrangement.Horizontal) {
				for (int c=0; c<Math.Min(objs.Length, table.maxPerLine); c++) {
                    SetCell(line+c/table.maxPerLine, c%table.maxPerLine, objs[c].GetComponent<UITableCell>());
				}
			} else {
				for (int r=0; r<Math.Min(objs.Length, table.maxPerLine); r++) {
                    SetCell(r%table.maxPerLine, line+r/table.maxPerLine, objs[r].GetComponent<UITableCell>());
				}
			}
		}
		
        private void SetCell(int r, int c, UITableCell cell) {
			if (cell != null) {
				int oldIndex = table.GetIndex(cell);
				if (oldIndex >= 0) {
					table.components[oldIndex] = null;
				}
                NGUIUtil.DisableAnchor(cell.transform);
			}
			table.SetCell(r, c, cell);
		}
		
		
		class HVComparer : IComparer<GameObject> {
			private UITableLayout.Arrangement arrangement;
			public HVComparer(UITableLayout.Arrangement arrangement) {
				this.arrangement = arrangement;
			}
			
			public int Compare (UnityEngine.GameObject x, UnityEngine.GameObject y)
			{
				if (arrangement == UITableLayout.Arrangement.Horizontal) {
					return Sign(x.transform.position.x, y.transform.position.x);
				} else {
					return Sign(-x.transform.position.y, -y.transform.position.y);
				}
			}
			
			private int Sign(float f1, float f2) {
				if (f1 < f2) {
					return -1;
				} else if (f1 == f2) {
					return 0;
				}
				return 1;
			}
		}
		
	}
}