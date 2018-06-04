using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Text;
using commons;
using comunity;

namespace ngui.ex {
    public class UITablePropertyTab : EditorTab {
		private UITableLayoutInspectorImpl inspector;
		private UITableLayout grid;
		
		public UITablePropertyTab(TabbedEditorWindow window) : base("Property", window) {
			TextAsset xls = (TextAsset)Resources.Load("ui/table_width", typeof(TextAsset));
			if (xls != null) {
				SpreadSheet excel = new SpreadSheet(xls.bytes);
				List<GridStyle> styles = new List<GridStyle>();
				for (int r=0; r<excel.GetSheet().GetLastRowNum(); r++) {
					if (!string.IsNullOrEmpty(excel.GetString1(r+1, 'A'))) {
						styles.Add(new GridStyle(excel, r));
					}
				}
				gridStyles = styles.ToArray();
			} else {
				gridStyles = new GridStyle[0];
			}
		}
		
		public void SetLayout(UITableLayout grid) {
			this.grid = grid;
			this.inspector = new UITableLayoutInspectorImpl(grid);
			this.inspector.OnEnable();
		}
		
		public override void OnEnable() {
			GameObject sel = Selection.activeGameObject;
			if (sel != null) {
				UITableLayout layout = sel.GetComponent<UITableLayout>();
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
		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnHeaderGUI() { }
		public override void OnSelected(bool sel) { }
		
		private UILabel titleLabelPrefab;
		private ColumnWidth selectedColumn;
		public override void OnInspectorGUI() {
			if (grid == null) {
				return;
			}
			bool changed = false;
			
			if (inspector != null) {
				inspector.OnInspectorGUI();
				
				changed |= DrawAddColumn();
				GUI.enabled = true;
			}
			
			changed |= DrawTest();
			changed |= DrawTextMod();
			if (changed) {
				grid.InvalidateLayout();
                EditorUtil.SetDirty(grid);
			}
		}
		
		public override void OnFooterGUI() { }
		
		private GridStyle[] gridStyles;
		private GridStyle currentStyle;
		private bool showAddColumn;
		private bool DrawAddColumn() {
			showAddColumn = EditorGUILayout.Foldout(showAddColumn, "Add Column");
			if (showAddColumn) {
				if (!grid.isHorizontal) {
					EditorGUILayout.HelpBox("Currently only Horizontal grid type is supported", MessageType.Warning);
					return false;
				}
				EditorGUIUtil.ObjectField<UILabel>("Title Label(Prefab)", ref titleLabelPrefab, true, GUILayout.ExpandWidth(false));
				if (EditorGUIUtil.PopupNullable<GridStyle>(null, ref currentStyle, gridStyles)) {
					selectedColumn = null;
					return true;
				}
				if (GUILayout.Button("Apply")) {
					grid.totalWidth = currentStyle.width;
					Vector2 minSize = grid.cellMinSize;
					minSize.y = currentStyle.rowHeight;
					grid.cellMinSize = minSize;
				}
				if (currentStyle != null) {
					EditorGUIUtil.PopupNullable<ColumnWidth>(null, ref selectedColumn, currentStyle.columnWidth);
					GUI.enabled = titleLabelPrefab != null && selectedColumn != null;
					if (GUILayout.Button("Add")) {
						int lastCol = GetLastColumn();
						
						for (int r=1, max=grid.rowHeader; r<max; r++) {
                            UITableCell cell = null;
                            #pragma warning disable 0618
                            grid.Insert((lastCol+1)*r-1, cell);
                            #pragma warning restore 0618
						}
						grid.maxPerLine = Math.Max(lastCol+1, grid.maxPerLine);
						grid.InitArray();
						grid.columnWidth[lastCol] = selectedColumn.width;
						grid.SetCell(grid.rowHeader-1, lastCol, CreateLabel(selectedColumn.name, lastCol+1));
						return true;
					}
				}
			}
			return false;
		}
		
		private int GetLastColumn() {
			int c = grid.columnCount-1;
			while (c >= 0 && grid.GetCell(grid.rowHeader-1, c) == null) {
				c--;
			}
			return c+1;
		}
		
		private string[] testStrings = new string[0];
		private bool showTest;
		private static UIFont testFont;
		private int fontSize = 20;
		private Color fontColor;
		private bool DrawTest() {
			showTest = EditorGUILayout.Foldout(showTest, "Test");
			bool changed = false;
			if (showTest) {
				EditorGUI.indentLevel += 2;
				EditorGUIUtil.ObjectField<UIFont>("Font", ref testFont, true);
				int row = grid.rowCount;
				int col = grid.columnCount;
				if (EditorGUIUtil.IntField("Font Size", ref fontSize, GUILayout.ExpandWidth(false))) {
					for (int r=0; r<row; r++) {
						for (int c=0; c<col; c++) {
                            UILabel label = grid.GetCell(r, c).GetComponent<UILabel>();
							if (label != null) {
								label.transform.localScale = new Vector3(fontSize, fontSize, 1);
							}
						}
					}
				}
				if (EditorGUIUtil.ColorField("Font Color", ref fontColor, GUILayout.ExpandWidth(false))) {
					for (int r=grid.rowHeader; r<row; r++) {
						for (int c=grid.columnHeader; c<col; c++) {
                            UILabel label = grid.GetCell(r, c).GetComponent<UILabel>();
							if (label != null) {
								label.color = fontColor;
								label.MarkAsChanged();
							}
						}
					}
				}
				Array.Resize(ref testStrings, grid.maxPerLine);
				for (int i=0; i<grid.maxPerLine; i++) {
					EditorGUIUtil.TextField(i.ToString(), ref testStrings[i], GUILayout.ExpandWidth(false));
				}
				GUI.enabled = testFont != null;
				if (GUILayout.Button("Fill Data")) {
					for (int r=grid.rowHeader; r<row; r++) {
						for (int c=grid.columnHeader; c<col; c++) {
                            UITableCell cell = grid.GetCell(r, c);
							if (cell == null) {
								UILabel label = NGUITools.AddWidget<UILabel>(grid.gameObject);
								label.bitmapFont = testFont;
								label.name = "__TEST__";
								label.SetText(testStrings[grid.isHorizontal?c:r]);
								label.transform.localScale = new Vector3(fontSize, fontSize, 1);
								label.color = fontColor;
								label.MarkAsChanged();
                                UILabelCell lc = label.gameObject.AddComponent<UILabelCell>();
                                lc.label = label;
								grid.SetCell(r, c, lc);
							}
						}
					}
					changed = true;
				}
				GUI.enabled = true;
				EditorGUI.indentLevel -= 2;
			}
			return changed;
		}
		
		private bool mod;
		private bool DrawTextMod() {
			bool changed = false;
			int row = grid.rowCount;
			int col = grid.columnCount;
			mod = EditorGUILayout.Foldout(mod, "Text Mod");
			if (mod) {
				EditorGUI.indentLevel += 2;
				for (int r=0; r<row; r++) {
					EditorGUILayout.BeginHorizontal();
					for (int c=0; c<col; c++) {
                        UITableCell cell = grid.GetCell(r, c);
						UILabel l = cell!=null? cell.GetComponentInChildren<UILabel>(): null;
						string s = l!=null? l.text: "";
						if (EditorGUIUtil.TextField(null, ref s, GUILayout.Width(70)) && l!=null) {
							l.SetText(s);
							changed = true;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel -= 2;
			}
			return changed;
		}
		
        private UITableCell CreateLabel(string text, int i) {
			UILabel label = NGUIUtil.InstantiateWidget(grid.transform, titleLabelPrefab.gameObject).GetComponent<UILabel>();
			label.name = "Title"+i;
			label.SetText(text);
			label.MarkAsChanged();
			label.gameObject.SetActive(true);
            UILabelCell cell = label.gameObject.AddComponent<UILabelCell>();
            cell.label = label;
            return cell;
		}
		
		class ColumnWidth {
			public string name;
			public int width;
			
			public ColumnWidth(string name, int width) {
				this.name = name;
				this.width = width;
			}
			
			public override string ToString ()
			{
				return name;
			}
		}
		
		class GridStyle {
			public string name;
			public int width;
			public int rowHeight;
			public ColumnWidth[] columnWidth;
			
            public GridStyle(SpreadSheet excel, int row) {
				int count = excel.GetLastColumn(row+3);
				columnWidth = new ColumnWidth[count];
				name = excel.GetString1(row+1, 'A');
				rowHeight = excel.GetInt1(row+2, 'C');
				width = excel.GetInt1(row+3, 'C');
				for (char c='A'; c<'A'+count; c++) {
					columnWidth[c] = new ColumnWidth(excel.GetString1(row+4, (char)(c+2)), excel.GetInt1(row+5, (char)(c+2)));
				}
			}
			
			public override string ToString ()
			{
				return name;
			}
		}
	}
}
