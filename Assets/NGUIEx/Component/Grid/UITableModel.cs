//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Collections;
using System;
using mulova.commons;
using System.Ex;

namespace ngui.ex {
	/**
	 * SetValues() 로 Table Cell 값들을 지정한다.
	 * PutRowPrefab(), PutColumnPrefab() 으로 Cell이 비었을 경우 생성할 instance의 prefab들을 지정한다.
	 */
	public class UITableModel
	{
		private object[,] cells;
		private bool changed = true;
		private bool reconstruct = true;
        private int[] globalSortOrder = new int[0];
        private int[] rowSortOrder;
		private int[] colSortOrder = new int[0];
        private bool horizontal = true;
		public const bool COL_SIZE = true; // used for 'horizontal' argument at constructor
		public const bool ROW_SIZE = false; // used for 'horizontal' argument at constructor
		
		public UITableModel() {
			SetContents(new object[0, 0]);
		}
		
        public UITableModel(object[,] contents) {
            SetContents(contents);
		}

        public UITableModel(IEnumerable contents, bool horizontal, int lineSize) {
            SetContents(contents, horizontal, lineSize);
        }

		/**
		 * @param horizontal use COL_SIZE or ROW_SIZE
		 */
        public UITableModel(IList contents, bool horizontal, int lineSize) {
            SetContents(contents, horizontal, lineSize);
		}

        public void SetContents(object[,] contents) {
			this.cells = contents;
			SetDirty();
		}
		
        public void SetContents(IEnumerable data, bool horizontal, int lineSize) {
            List<object> contents = new List<object>();
            if (data != null) {
                foreach (object o in data) {
                    contents.Add(o);
                }
            }
            SetContents((IList)data, horizontal, lineSize);
        }

        public void SetContents(IList contents, bool horizontal, int lineSize) {
            this.horizontal = horizontal;
			int count = (contents.Count+lineSize-1) / lineSize;
			if (horizontal) {
				object[,] cells = new object[count, lineSize];
				int r = -1;
				int i = 0;
				foreach (object o in contents) {
					if (i%lineSize == 0) {
						r++;
					}
					cells[r,i%lineSize] = o;
					i++;
				}
                SetContents(cells);
			} else {
				object[,] cells = new object[lineSize, count];
				int c = -1;
				int i = 0;
				foreach (object o in contents) {
					if (i%lineSize == 0) {
						c++;
					}
					cells[i%lineSize, c] = o;
					i++;
				}
                SetContents(cells);
			}
		}
		
		public void Clear() {
			cells = new object[0,0];
			SetDirty();
		}
		
		public bool IsEmpty() {
			return cells == null || GetRowCount() == 0 || GetColumnCount() == 0;
		}

		public void SetValue(int row, int col, object val) {
            int r = 0;
            int c = 0;
            int rowCount = GetRowCount();
            if (globalSortOrder != null)
            {
                int i = globalSortOrder[row*rowCount+col];
                r = i/rowCount;
                c = i%rowCount;
            } else 
            {
                r = GetActualRow(row);
                c = GetActualColumn(col);
            }
			cells[r, c] = val;
			SetDirty();
		}
		
		public object GetValue(int row, int col) {
			Reconstruct();
            int r = 0;
            int c = 0;
            if (globalSortOrder != null)
            {
                if (horizontal)
                {
                    int columnCount = GetColumnCount();
                    int index = row*columnCount+col;
                    if (index >= globalSortOrder.Length)
                    {
                        return null;
                    }
                    r = globalSortOrder[index]/columnCount;
                    c = globalSortOrder[index]%columnCount;
                } else
                {
                    int rowCount = GetRowCount();
                    int index = col*rowCount+row;
                    if (index >= globalSortOrder.Length)
                    {
                        return null;
                    }
                    r = globalSortOrder[index]%rowCount;
                    c = globalSortOrder[index]/rowCount;
                    
                }
            } else 
            {
                r = GetActualRow(row);
                c = GetActualColumn(col);
            }
			return cells[r, c];
		}

		/// <summary>
		/// Gets the contents row count.
		/// </summary>
		/// <returns>The row count.</returns>
		public int GetRowCount() {
			Reconstruct();
            if (globalSortOrder != null && horizontal)
            {
                int c = cells.GetLength(1);
                return (globalSortOrder.Length+c-1)/c;
            } else if (rowSortOrder != null)
            {
                return rowSortOrder.Length;
            } else 
            {
                return cells.GetLength(0);
            }
		}

		/// <summary>
		/// Gets the contents column count.
		/// </summary>
		/// <returns>The column count.</returns>
		public int GetColumnCount() {
			Reconstruct();
            if (globalSortOrder != null && !horizontal)
            {
                int r = cells.GetLength(0);
                return (globalSortOrder.Length+r-1)/r;
            } else if (colSortOrder != null)
            {
                return colSortOrder.Length;
            } else 
            {
                return cells.GetLength(1);
            }
		}

		public void SetDirty() {
			changed = true;
			reconstruct = true;
		}
		
		public void Update(Action updateMethod) {
			if (changed) {
				changed = false;
				updateMethod();
			}
		}

		private void Reconstruct() {
			if (!reconstruct) {
				return;
			}
			reconstruct = false;
            this.rowSortOrder = null;
            this.colSortOrder = null;
            this.globalSortOrder = null;
			int rowCount = cells.GetLength(0);
			int colCount = cells.GetLength(1);
            if (rowCount != 0 && colCount != 0) {
                if (globalFilter != null || globalComparer != null)
                {
                    List<object> globalFiltered = new List<object>();
                    List<int> globalOrder = new List<int>();
                    for (int r=0; r<rowCount; ++r) {
                        for (int c=0; c<colCount; ++c) {
                            if (cells[r, c] != null && (globalFilter == null || globalFilter(cells[r, c]))) {
                                globalFiltered.Add(cells[r, c]);
                                globalOrder.Add(r*colCount+c);
                            }
                        }
                    }
                    this.globalSortOrder = globalOrder.ToArray();
                    if (globalComparer != null)
                    {
                        object[] globalKeys = globalFiltered.ToArray();
                        Array.Sort(globalKeys, globalSortOrder, globalComparer);
                    }
                } else if (rowFilter != null || rowComparer != null)
                {
                    List<object> rowFiltered = new List<object>();
                    List<int> rowOrder = new List<int>();
                    for (int i=0; i<rowCount; ++i) {
                        if (cells[i, filterColIndex]!=null && (rowFilter==null || rowFilter(cells[i, filterColIndex]))) {
                            rowFiltered.Add(cells[i, rowSortIndex]);
                            rowOrder.Add(i);
                        }
                    }
                    object[] rowKeys = rowFiltered.ToArray();
                    this.rowSortOrder = rowOrder.ToArray();
                    if (rowComparer != null) {
                        Array.Sort(rowKeys, rowSortOrder, rowComparer);
                    }
                } else if (colFilter != null || colComparer != null)
                {
                    
                    List<object> colFiltered = new List<object>();
                    List<int> colOrder = new List<int>();
                    for (int i=0; i<colCount; ++i) {
                        if (cells[filterRowIndex, i]!=null && (colFilter==null || colFilter.Call(cells[filterRowIndex, i]))) {
                            colFiltered.Add(cells[colSortIndex, i]);
                            colOrder.Add(i);
                        }
                    }
                    object[] colKeys = colFiltered.ToArray();
                    this.colSortOrder = colOrder.ToArray();
                    if (colComparer != null) {
                        Array.Sort(colKeys, colSortOrder, colComparer);
                    }
                }

			}
		}
		
		private int rowSortIndex;
		private IComparer rowComparer;
		public void SetRowSorter(int colIndex, params IComparer[] comparer) {
			this.rowSortIndex = colIndex;
			if (comparer == null) {
				this.rowComparer = null;
			} else if (comparer.Length == 1) {
				this.rowComparer = comparer[0];
			} else {
				this.rowComparer = new CompositeComparer(comparer);
			}
            this.globalFilter = null;
            this.colFilter = null;
            this.globalComparer = null;
            this.colComparer = null;
			SetDirty();
		}

		private int colSortIndex;
		private IComparer colComparer;
		public void SetColumnSorter(int rowIndex, params IComparer[] comparer) {
			this.colSortIndex = rowIndex;
			if (comparer == null) {
				this.colComparer = null;
			} else if (comparer.Length == 1) {
				this.colComparer = comparer[0];
			} else {
				this.colComparer = new CompositeComparer(comparer);
			}
            this.globalFilter = null;
            this.rowFilter = null;
            this.globalComparer = null;
            this.rowComparer = null;
			SetDirty();
		}

        private Predicate<object> globalFilter;
        public void SetGlobalFilter(Predicate<object> filter) {
            this.globalFilter = filter;
            this.rowFilter = null;
            this.colFilter = null;
            this.rowComparer = null;
            this.colComparer = null;
			SetDirty();
        }

        private IComparer globalComparer;
        public void SetGlobalSorter(params IComparer[] comparer) {
            if (comparer == null) {
                this.globalComparer = null;
            } else if (comparer.Length == 1) {
                this.globalComparer = comparer[0];
            } else {
                this.globalComparer = new CompositeComparer(comparer);
            }
            this.rowFilter = null;
            this.colFilter = null;
            this.rowComparer = null;
            this.colComparer = null;
			SetDirty();
        }

		private Predicate<object> rowFilter;
		private int filterColIndex;
		public void SetRowFilter(int colNo, Predicate<object> filter) {
			this.rowFilter = filter;
			this.filterColIndex = colNo;
            this.globalFilter = null;
            this.colFilter = null;
            this.globalComparer = null;
            this.colComparer = null;
			SetDirty();
		}

		private Predicate<object> colFilter;
		private int filterRowIndex;
		public void SetColumnFilter(int rowNo, Predicate<object> filter) {
			this.colFilter = filter;
			this.filterRowIndex = rowNo;
            this.globalFilter = null;
            this.rowFilter = null;
            this.globalComparer = null;
            this.rowComparer = null;
			SetDirty();
		}

		public int GetActualRow(int visualRowNo) {
            if (rowSortOrder != null) 
            {
                return rowSortOrder[visualRowNo];
            } else 
            {
                return visualRowNo;
            }
		}

		public int GetActualColumn(int visualColNo) {
            if (colSortOrder != null)
            {
                return colSortOrder[visualColNo];
            } else
            {
                return visualColNo;
            }
		}

        public bool Contains(object obj, Func<object, object, bool> equality = null)
        {
			Reconstruct();
            if (obj == null || globalFilter != null && !globalFilter(obj)) { return false; }
            for (int r=0; r<cells.GetLength(0); ++r) {
                for (int c=0; c<cells.GetLength(1); ++c) {
                    if (cells[r,c] != null)
                    {
                        if (cells[r,c] == obj || (equality!=null && equality(cells[r,c], obj))) {
                            if (rowFilter!=null && !rowSortOrder.Contains(r)) { return false;; }
                            if (colFilter!=null && !colSortOrder.Contains(c)) { return false; }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

		/// <summary>
		/// predicate param: row, column, cellData
		/// return: true to continue execution, false to terminate
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		public void ForEach(Func<int, int, object, bool> predicate) { 
			for (int r=0; r<cells.GetLength(0); ++r) {
				for (int c=0; c<cells.GetLength(1); ++c) {
					object val = GetValue(r, c);
					if (val != null && !predicate(r, c, val)) {
						return;
					}
				}
			}
		}
	}
	
}