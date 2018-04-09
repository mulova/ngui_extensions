using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;
using comunity;

namespace ngui.ex {
    public class UIGridContentsTab : EditorTab {
		private UIGridLayoutInspectorImpl inspector;
		private UIGridLayout grid;
		private const float WIDTH = 70;
		private const float HEIGHT = 16;
		private GUILayoutOption W_Option = GUILayout.Width(WIDTH);
		private GUILayoutOption H_Option = GUILayout.Height(HEIGHT);
		
		public UIGridContentsTab(TabbedEditorWindow window) : base("Grid", window) {
		}
		
		public void SetLayout(UIGridLayout grid) {
			this.grid = grid;
			this.inspector = new UIGridLayoutInspectorImpl(grid);
			this.inspector.OnEnable();
		}
		
		public override void OnEnable() {
			GameObject sel = Selection.activeGameObject;
			if (sel != null) {
				UIGridLayout layout = sel.GetComponent<UIGridLayout>();
				if (layout != null) {
					grid = layout;
					SetLayout(grid);
				}
			}
		}
		
		public override void OnFocus(bool focus) {
			OnEnable();
		}
		
		public override void OnDisable() { }

		public override void OnChangePlayMode() {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnSelected(bool sel) { }
		
		public override void OnHeaderGUI() { }
		
		private UILabel titleLabelPrefab;
		public override void OnInspectorGUI() {
			if (grid == null) {
				return;
			}
			bool changed = false;
			
			
			EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH*grid.GetColumnCount()));
			changed |= DrawRowHeader();
			
			changed |= DrawColumn();
			changed |= DrawRowButton();
			EditorGUILayout.EndHorizontal();
			if (grid.components.Length == 0) {
				if (GUILayout.Button("Click To Add Line")) {
					grid.components = new Transform[grid.maxPerLine];
					grid.InitArray();
				}
			}
			
			if (changed) {
				grid.InvalidateLayout();
				CompatibilityEditor.SetDirty(grid);
			}
		}
		
		public override void OnFooterGUI() { }
		
		private bool IsHorizontal() {
			return grid.IsHorizontal();
		}
		
		private bool DrawRowHeader() {
			bool changed = false;
			EditorGUILayout.BeginVertical();
			changed |= EditorGUIUtil.ObjectField<GameObject>(ref grid.defaultPrefab, true, W_Option, H_Option);
			EditorGUILayout.LabelField("Align", EditorStyles.boldLabel, H_Option);
			EditorGUILayout.LabelField("Size", EditorStyles.boldLabel, H_Option);
			EditorGUILayout.LabelField("Index", EditorStyles.boldLabel, H_Option);
			int row = grid.IsVertical()? grid.GetMaxPerLine(): grid.GetRowCount();
			grid.InitArray();
			for (int r=0; r<row; r++) {
				EditorGUILayout.BeginHorizontal();
				changed |= EditorGUIUtil.ObjectField<GameObject>(ref grid.rowPrefab[r], true, GUILayout.Width(WIDTH-20), H_Option);
                changed |= EditorGUIUtil.Popup<UIGridLayout.VAlign>(ref grid.valigns[r], EnumUtil.Values<UIGridLayout.VAlign>(), GUILayout.Width(40), H_Option);
				changed |= EditorGUIUtil.IntField(null, ref grid.rowHeight[r], GUILayout.Width(WIDTH-30), H_Option);
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
				grid.InitArray();
			}
			return changed;
		}
		
		private bool DrawColumn() {
			bool changed = false;
			int columnSize = grid.IsHorizontal()? grid.GetMaxPerLine(): grid.GetColumnCount();
			int contentSize = grid.GetColumnCount();
			for (int c=0; c<columnSize; c++) {
				EditorGUILayout.BeginVertical();
				changed |= EditorGUIUtil.ObjectField<GameObject>(ref grid.columnPrefab[c], true, GUILayout.ExpandWidth(false), H_Option);
                changed |= EditorGUIUtil.Popup<UIGridLayout.HAlign>(ref grid.haligns[c], EnumUtil.Values<UIGridLayout.HAlign>(), GUILayout.Width(WIDTH-20), H_Option);
				changed |= EditorGUIUtil.IntField(null, ref grid.columnWidth[c], GUILayout.Width(WIDTH-20), H_Option);
				EditorGUILayout.LabelField(c.ToString(), EditorStyles.boldLabel, W_Option, H_Option);
				int row = grid.GetRowCount();
				if (c < contentSize) {
					for (int r=0; r<row; r++) {
						changed |= DrawCell(r, c);
					}
					
					// Draw Column +/- buttons
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("A", "Add Selected"), GUILayout.ExpandWidth(false), H_Option)) {
						AddSelected(c, UIGridLayout.Arrangement.Vertical);
						changed = true;
					}
					if (GUILayout.Button("+", GUILayout.ExpandWidth(false), H_Option)) {
                        #pragma warning disable 0618
                        grid.AddColumn(c+1, new Transform[grid.GetRowCount()]);
                        #pragma warning restore 0618
						changed = true;
					}
					GUI.enabled = c >= grid.columnHeader;
					if (GUILayout.Button("-", GUILayout.ExpandWidth(false), H_Option)) {
						if (EditorUtility.DisplayDialog("Confirm", "Delete column "+(c+1), "OK", "Cancel")) {
                            #pragma warning disable 0618
                            grid.RemoveColumn(c);
                            #pragma warning restore 0618
                            grid.Reposition();
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
			int row = grid.GetRowCount();
			for (int r=0; r<row; r++) {
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("A", "Add Selected"), EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.Height(HEIGHT-1))) {
					AddSelected(r, UIGridLayout.Arrangement.Horizontal);
					changed = true;
				}
				if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.Height(HEIGHT-1))) {
                    #pragma warning disable 0618
                    grid.AddRow(r+1, new Transform[grid.GetColumnCount()]);
                    #pragma warning restore 0618
					changed = true;
				}
				GUI.enabled = r >= grid.rowHeader;
				if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.Height(HEIGHT-1))) {
					if (EditorUtility.DisplayDialog("Confirm", "Delete row "+(r+1), "OK", "Cancel")) {
                        #pragma warning disable 0618
                        grid.RemoveRow(r);
                        #pragma warning restore 0618
                        grid.Reposition();
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
			Transform t = grid.GetCell(r, c);
			if (EditorGUIUtil.ObjectField<Transform>(ref t, true, W_Option, GUILayout.Height(HEIGHT-1))) {
				grid.SetCell(r, c, t);
				changed = true;
			}
			return changed;
		}
		
		private void AddSelected(int line, UIGridLayout.Arrangement orientation) {
			GameObject[] objs = new GameObject[Selection.gameObjects.Length];
			Array.Copy(Selection.gameObjects, objs, objs.Length);
			Array.Sort(objs, new HVComparer(orientation));
			if (orientation == UIGridLayout.Arrangement.Horizontal) {
				for (int c=0; c<Math.Min(objs.Length, grid.maxPerLine); c++) {
					SetCell(line+c/grid.maxPerLine, c%grid.maxPerLine, objs[c].transform);
				}
			} else {
				for (int r=0; r<Math.Min(objs.Length, grid.maxPerLine); r++) {
					SetCell(r%grid.maxPerLine, line+r/grid.maxPerLine, objs[r].transform);
				}
			}
		}
		
		private void SetCell(int r, int c, Transform t) {
			if (t != null) {
				int oldIndex = grid.GetIndex(t);
				if (oldIndex >= 0) {
					grid.components[oldIndex] = null;
				}
				NGUIUtil.DisableAnchor(t);
			}
			grid.SetCell(r, c, t);
		}
		
		
		class HVComparer : IComparer<GameObject> {
			private UIGridLayout.Arrangement arrangement;
			public HVComparer(UIGridLayout.Arrangement arrangement) {
				this.arrangement = arrangement;
			}
			
			public int Compare (UnityEngine.GameObject x, UnityEngine.GameObject y)
			{
				if (arrangement == UIGridLayout.Arrangement.Horizontal) {
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