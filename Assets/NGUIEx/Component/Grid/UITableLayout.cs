//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

using commons;
using comunity;

namespace ngui.ex
{
    [ExecuteInEditMode]
    [AddComponentMenu("NGUI/Ex/TableLayout")]
    public class UITableLayout : UILayout, IEnumerable<UITableCell>
    {
        public enum HAlign
        {
            Left,
            Center,
            Right,
            None
        }

        public enum VAlign
        {
            Top,
            Center,
            Bottom,
            None
        }

        public enum Arrangement
        {
            Horizontal,
            Vertical,
        }

        public Arrangement arrangement = Arrangement.Horizontal;
        public HAlign halign = HAlign.None;
        public VAlign valign = VAlign.None;
        public HAlign[] haligns = new HAlign[0];
        public VAlign[] valigns = new VAlign[0];
        public int maxPerLine = 1;
        public Vector2 padding;
        public Vector2 cellSize;
        // cell minimum size.
        public Vector2 cellMinSize;
        // cell minimum size
        public int totalWidth;
        // table size
        public UITableCell[] components = new UITableCell[0];
        public int rowHeader;
        public int columnHeader;

        public Color gizmoColor = new Color(1f, 0f, 0f);
        private Bounds[,] bounds;
		
        [SerializeField] private UITableCell[] prefab = new UITableCell[0];
        public int[] rowHeight = new int[0];
        public int[] columnWidth = new int[0];
        // rows for background
        public UIWidget background;
        public Vector4 backgroundPadding;
        public bool resizeCollider;
        public bool expandColliderToPadding;
        public bool reuseCell;
        // Don't destroy cells when SetModel() called
        public bool propagateReposition = true;
        //  if true, Reposition is propagated to the ancestor UILayouts
        public GameObject emptyObj;
		
        private List<UITableEventListener> listeners = new List<UITableEventListener>();
        public UITableModel model { get; private set; }
        private Vector2[,] cellPos;
        private Action<UITableCell> initFunc = null;
        private float[] maxHeights;
        private float[] maxWidths;

        void Awake()
        {
            if (Application.isPlaying)
            {
                this.prefab = components;
                this.components = new UITableCell[0];
                MakePrefabInactive(prefab);
            }
        }

        /**
		 * @param t
		 * @return int get the serialized array index
		 */
        public int GetIndex(UITableCell c)
        {
            if (c != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == c)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Convert serialized array index to 2D array row index
        /// </summary>
        /// <returns>The row index.</returns>
        /// <param name="index">Index.</param>
        public int GetRowIndex(int index)
        {
            if (isHorizontal)
            {
                return index / GetMaxPerLine();
            } else
            {
                return index % GetMaxPerLine();
            }
        }

        /// <summary>
        /// Convert serialized array index to 2D array column index
        /// </summary>
        /// <returns>The column index.</returns>
        /// <param name="index">Index.</param>
        public int GetColumnIndex(int index)
        {
            if (isHorizontal)
            {
                return index % GetMaxPerLine();
            } else
            {
                return index / GetMaxPerLine();
            }
        }

        public int GetRowIndex(UITableCell t)
        {
            int i = GetIndex(t);
            if (i < 0)
            {
                return -1;
            }
            return GetRowIndex(i);
        }

        public int GetColumnIndex(UITableCell t)
        {
            int i = GetIndex(t);
            if (i < 0)
            {
                return -1;
            }
            return GetColumnIndex(i);
        }

        /**
		 * if Horizontal Arrangement, return column index
		 * if Vertical Arrangement, return row index
		 */
        public int GetLineIndex(int index)
        {
            return index % GetMaxPerLine();
        }

        /// <summary>
        /// Gets the row size.
        /// [NOTE] This is not the same as contents size.
        /// </summary>
        /// <returns>The row count.</returns>
        public int rowCount
        {
            get
            {
                if (components.Length == 0)
                {
                    return 0;
                }
                int row = isHorizontal? lineCount : GetMaxPerLine();
                return Math.Max(row, rowHeader);
            }
        }

        /**
		 * column count except row header
		 */
        public int contentRowCount
        {
            get
            {
                return rowCount-rowHeader;
            }
        }

        /// <summary>
        /// Gets the column size.
        /// [NOTE] This is not the same as contents size.
        /// </summary>
        /// <returns>The column count.</returns>
        public int columnCount
        {
            get
            {
                if (components.Length == 0)
                {
                    return 0;
                }
                int col = isHorizontal? GetMaxPerLine() : lineCount;
                return Math.Max(col, columnHeader);
            }
        }

        /**
		 * column count except column header
		 */
        public int contentsColumnCount
        {
            get
            {
                return columnCount-columnHeader;
            }
        }

        public int GetMaxPerLine()
        {
            return Mathf.Max(1, maxPerLine);
        }

        public int lineCount
        {
            get
            {
                int l = components.Length / GetMaxPerLine();
                if (components.Length % GetMaxPerLine() != 0)
                {
                    l++;
                }
                return l;
            }
        }

        public void SetContents(IList data, Action<UITableCell> initFunc = null)
        {
            if (this.model == null)
            {
                SetModel(new UITableModel(data, isHorizontal, maxPerLine), initFunc);
            } else
            {
                this.initFunc = initFunc;
                this.model.SetContents(data, isHorizontal, maxPerLine);
                RefreshContents();
            }
        }

        public void SetContents(IEnumerable data, Action<UITableCell> initFunc = null)
        {
            if (this.model == null)
            {
                SetModel(new UITableModel(data, isHorizontal, maxPerLine), initFunc);
            } else
            {
                this.model.SetContents(data, isHorizontal, maxPerLine);
                this.initFunc = initFunc;
                RefreshContents();
            }
        }

        public void SetModel(UITableModel model, Action<UITableCell> initFunc = null)
        {
            this.model = model;
            this.initFunc = initFunc;
            RefreshContents();
        }

        /// <summary>
        /// Used when the cell prefab is DummyGridCell
        /// </summary>
        /// <param name="count">Count.</param>
        public void SetDummyModel(int count)
        {
            int[] content = new int[count];
            SetModel(new UITableModel(content, isHorizontal, maxPerLine));
        }

        public void Clear()
        {
            SetDummyModel(0);
        }

        public bool isHorizontal
        {
            get
            {
                return arrangement == Arrangement.Horizontal;
            }
        }

        public bool isVertical
        {
            get
            {
                return arrangement == Arrangement.Vertical;
            }
        }

        public bool isEmpty
        {
            get
            {
                return components == null||components.Length == 0||components[0] == null;
            }
        }

        [Obsolete("Change model instead")]
        public void AddRow(params UITableCell[] row)
        {
            AddRow(rowCount, row);
        }

        [Obsolete("Change model instead")]
        public void AddRow(int rowIndex, params UITableCell[] row)
        {
            int colSize = columnCount;
            int insertSize = row.Length;
            // modify insertion array size as the multiple of column size
            if (insertSize % colSize != 0)
            {
                insertSize = (insertSize / colSize+1) * colSize;
                Array.Resize(ref row, insertSize);
            }
            int index = GetIndex(rowIndex, 0);
            if (isHorizontal)
            {
                Insert(index, row);
            } else
            {
                int unit = insertSize / colSize;
                for (int i = 0; i < colSize; i++)
                {
                    UITableCell[] ins = new UITableCell[unit];
                    Array.Copy(row, i * unit, ins, 0, unit);
                    Insert(index, ins);
                    index += unit+GetMaxPerLine();
                }
                rowHeight = rowHeight.Insert(rowIndex, 0);
                valigns = valigns.Insert(rowIndex, valign);
                maxPerLine++;
            }
        }

        [Obsolete("Change model instead")]
        public void AddColumn(params UITableCell[] row)
        {
            AddColumn(columnCount, row);
        }

        [Obsolete("Change model instead")]
        public void AddColumn(int colIndex, params UITableCell[] col)
        {
            int rowSize = rowCount;
            int insertSize = col.Length;
            // modify insertion array size as the multiple of column size
            if (insertSize % rowSize != 0)
            {
                insertSize = (insertSize / rowSize+1) * rowSize;
                Array.Resize(ref col, insertSize);
            }
            if (isVertical)
            {
                int index = GetIndex(0, colIndex);
                Insert(index, col);
            } else
            {
                int unit = insertSize / rowSize;
                int index = colIndex;
                for (int i = 0; i < rowSize; i++)
                {
                    UITableCell[] ins = new UITableCell[unit];
                    Array.Copy(col, i * unit, ins, 0, unit);
                    Insert(index, ins);
                    index += unit+GetMaxPerLine();
                }
                columnWidth = columnWidth.Insert(colIndex, 0);
                haligns = haligns.Insert(colIndex, halign);
                maxPerLine++;
            }
        }

        [Obsolete("Change model instead")]
        public void RemoveRow(int rowIndex)
        {
            if (isHorizontal)
            {
                int index = GetIndex(rowIndex, 0);
                int count = GetMaxPerLine();
                // when last row is deleted, only remaining parts are removed.
                if (rowIndex == rowCount-1&&components.Length % GetMaxPerLine() != 0)
                {
                    count = components.Length % GetMaxPerLine();
                }
                for (int i = 0; i < count; i++)
                {
                    DestroyCell(components[index+i]);
                }
                if (!reuseCell||!Application.isPlaying)
                {
                    components = components.Remove(index, count);
                }
            } else
            {
                int colSize = columnCount;
                UITableCell[] newComponents = new UITableCell[lineCount * (maxPerLine-1)];
                Array.Copy(components, 0, newComponents, 0, rowIndex);
                for (int c = 0; c < colSize; c++)
                {
                    int srcIndex = GetIndex(rowIndex, c);
                    long copySize = c < colSize-1? maxPerLine-1 : Math.Min(maxPerLine-1, components.Length-srcIndex-1);
                    Array.Copy(components, srcIndex+1, newComponents, c * (maxPerLine-1)+rowIndex, copySize);
                    if (components[srcIndex] != null)
                    {
                        if (reuseCell||(Application.isEditor&&!Application.isPlaying))
                        {
                            components[srcIndex].gameObject.SetActive(false);
                        } else
                        {
                            components[srcIndex].gameObject.DestroyEx();
                        }
                    }
                }
                if (!reuseCell||!Application.isPlaying)
                {
                    components = newComponents;
                    rowHeight = rowHeight.Remove(rowIndex);
                    valigns = valigns.Remove(rowIndex);			
                    maxPerLine--;
                }
            }
        }

        [Obsolete("Change model instead")]
        public void RemoveColumn(int colIndex)
        {
            if (isVertical)
            {
                int index = GetIndex(0, colIndex);
                int count = GetMaxPerLine();
                if (colIndex == columnCount-1&&components.Length % GetMaxPerLine() != 0)
                {
                    count = components.Length % GetMaxPerLine();
                }
                for (int i = 0; i < count; i++)
                {
                    DestroyCell(components[index+i]);
                }
                if (!reuseCell||!Application.isPlaying)
                {
                    components = components.Remove(index, count);
                }
            } else
            {
                int rowSize = rowCount;
                UITableCell[] newComponents = new UITableCell[lineCount * (maxPerLine-1)];
                Array.Copy(components, 0, newComponents, 0, colIndex);
                for (int r = 0; r < rowSize; r++)
                {
                    int srcIndex = GetIndex(r, colIndex);
                    long copySize = r < rowSize-1? maxPerLine-1 : Math.Min(maxPerLine-1, components.Length-srcIndex-1);
                    Array.Copy(components, srcIndex+1, newComponents, r * (maxPerLine-1)+colIndex, copySize);
                    DestroyCell(components[srcIndex]);
                }
                if (!reuseCell)
                {
                    components = newComponents;
                    columnWidth = columnWidth.Remove(colIndex);
                    haligns = haligns.Remove(colIndex);
                    maxPerLine--;
                }
            }
        }

        private void DestroyCell(UITableCell c)
        {
            if (c == null)
            {
                return;
            }
            if (Application.isEditor&&!Application.isPlaying)
            {
                c.go.SetActive(false);
            } else if (reuseCell)
            {
                c.Clear();
                c.go.SetActive(false);
            } else
            {
                c.go.DestroyEx();
            }
        }

        public Vector2 GetCellPos(int r, int c)
        {
            return cellPos[r, c];
        }

        private bool Resize<T>(ref T[] arr, int size)
        {
            T[] src = arr;
            Array.Resize(ref arr, size);
            return arr != src;
        }

        [Obsolete("Change model instead")]
        public void Add(params UITableCell[] t)
        {
            Insert(components.Length, t);
        }

        [Obsolete("Change model instead")]
        public void Insert(int i, params UITableCell[] t)
        {
            if (components.Length < i)
            {
                Array.Resize(ref components, i);
            }
            components = components.Insert(i, t);
            InitArray();
            InvalidateLayout();
        }

        [Obsolete("Change model instead")]
        public void Remove(UITableCell t)
        {
            int i = GetIndex(t);
            if (i >= 0)
            {
                components = components.Remove(i);
            }
            InitArray();
            InvalidateLayout();
        }

        public bool InitArray()
        {
            int rowCount = isVertical? GetMaxPerLine() : lineCount;
            int colCount = isHorizontal? GetMaxPerLine() : lineCount;
            bool changed = Resize(ref rowHeight, rowCount);
            changed |= Resize(ref columnWidth, colCount);
            if (haligns.Length != colCount)
            {
                int c = haligns.Length;
                changed |= Resize(ref haligns, colCount);
                for (; c < colCount; c++)
                {
                    haligns[c] = halign;
                    changed = true;
                }
            }
            if (valigns.Length != rowCount)
            {
                int r = valigns.Length;
                changed |= Resize(ref valigns, rowCount);
                for (; r < rowCount; r++)
                {
                    valigns[r] = valign;
                    changed = true;
                }
            }
            if (cellPos == null||cellPos.GetLength(0) != rowCount||cellPos.GetLength(1) != colCount)
            {
                cellPos = new Vector2[rowCount, colCount];
            }
            if (changed)
            {
                InvalidateLayout();
            }
            return changed;
        }

        private int GetIndex(int row, int col)
        {
            return GetIndex(arrangement, maxPerLine, row, col);
        }

        private static int GetIndex(Arrangement arrangement, int maxPerLine, int row, int col)
        {
            int i = 0;
            if (arrangement == Arrangement.Horizontal)
            {
                i = row * maxPerLine+col;
            } else
            {
                i = row+col * maxPerLine;
            }
            return i;
        }

        public UITableCell GetCell(int row, int col)
        {
            int i = GetIndex(row, col);
            if (i >= components.Length)
            {
                return null;
            }
            return components[i];
        }

        public void SetCell(int row, int col, UITableCell c)
        {
            int i = GetIndex(row, col);
            if (i >= components.Length)
            {
                Resize(ref components, i+1);
            }
            components[i] = c;
            InvalidateLayout();
        }

        public void AddListener(UITableEventListener l)
        {
#if UNITY_EDITOR
            Assert.IsFalse(listeners.Contains(l));
#endif
            this.listeners.Add(l);
        }

        public void RemoveListener(UITableEventListener l)
        {
            this.listeners.Remove(l);
        }

        private Rect CalculateBound()
        {
            Rect bound = new Rect();
            int row = rowCount;
            int col = columnCount;
            bounds = new Bounds[row, col];
            for (int i = 0, imax = components.Length; i < imax; ++i)
            {
                int r = GetRowIndex(i);
                int c = GetColumnIndex(i);
                if (cellSize.x != 0 || cellSize.y != 0)
                {
                    float cx = r * cellSize.x+cellSize.x * 0.5f+padding.x * Mathf.Max(0, r-1);
                    float cy = c * cellSize.y+cellSize.y * 0.5f+padding.y * Mathf.Max(0, c-1);
                    bounds[r, c] = new Bounds(new Vector3(cx, cy), new Vector3(cellSize.x, cellSize.y, 1));
                }
                else
                {
                    bounds[r, c] = CalculateBounds(components[i]);
                }
            }
            // max height
            maxHeights = new float[row];
            for (int r = 0; r < row; r++)
            {
                // check if null row
                bool filled = false;
                for (int c = 0; c < col && !filled; c++)
                {
                    var cell = GetCell(r, c);
                    if (cell != null && cell.go.activeSelf)
                    {
                        filled = true;
                    }
                }
                if (filled)
                {
                    if (r < rowHeight.Length && rowHeight[r] != 0)
                    {
                        maxHeights[r] = rowHeight[r];
                    }
                    else
                        if (cellSize.y != 0)
                        {
                            maxHeights[r] = cellSize.y;
                        }
                        else
                        {
                            for (int c = 0; c < col; c++)
                            {
                                maxHeights[r] = Mathf.Max(maxHeights[r], bounds[r, c].size.y, cellMinSize.y);
                            }
                        }
                    bound.height += maxHeights[r];
                }
            }
            bound.height += (row-1) * padding.y;
            maxWidths = new float[col];
            for (int c = 0; c < col; c++)
            {
                // check if null row
                bool filled = false;
                for (int r = 0; r < row && !filled; r++)
                {
                    var cell = GetCell(r, c);
                    if (cell != null && cell.go.activeSelf)
                    {
                        filled = true;
                    }
                }
                if (filled)
                {
                    if (cellSize.x != 0)
                    {
                        maxWidths[c] = cellSize.x;
                    }
                    else
                        if (c < columnWidth.Length && columnWidth[c] != 0)
                        {
                            maxWidths[c] = columnWidth[c];
                        }
                        else
                        {
                            for (int r = 0; r < row; r++)
                            {
                                maxWidths[c] = Mathf.Max(maxWidths[c], bounds[r, c].size.x, cellMinSize.x);
                            }
                        }
                    bound.width += maxWidths[c];
                }
            }
            float cellTotalWidth = bound.width;
            bound.width += (col-1) * padding.x;
            // expand cell width by ratio
            if (totalWidth > 0 && bound.width < totalWidth)
            {
                float pad = totalWidth-bound.width;
                for (int c = 0; c < col; c++)
                {
                    maxWidths[c] += pad * (maxWidths[c] / cellTotalWidth);
                }
                bound.width = totalWidth;
            }
            return bound;
        }

        /// <summary>
        /// Recalculate the position of all elements within the grid, sorting them alphabetically if necessary.
        /// </summary>
        [ContextMenu("Arrange")]
        override protected void DoLayout()
        {
//			if (this.model == null) {
//				return new Bounds();
//			}
            InitArray();
            Rect bound = CalculateBound();
            int row = rowCount;
            int col = columnCount;

            TableInfo prefabs = GetPrefabs();
            float pixely = 0;
            for (int r = 0; r < row; r++)
            {
                float pixelx = 0;
                bool activeRow = false; // inactive row is removed from layout computation
                for (int c = 0; c < col; c++)
                {
                    UITableCell cell = GetCell(r, c);
                    if (cell != null&&cell.go.activeInHierarchy)
                    {
                        Transform t = cell.trans;
                        if (!t.IsChildOf(transform))
                        {
                            t.SetParent(transform, false);
                        }
                        Vector3 point = bounds[r, c].min;
                        Vector3 size = bounds[r, c].size;
                        float halignPad = 0;
                        float valignPad = 0;
                        Vector3 pos = t.localPosition;
                        if (haligns[c] == HAlign.None)
                        {
                            if (r >= rowHeader&&c >= columnHeader)
                            {
                                UITableCell prefab = prefabs.GetPrefab(r, c);
                                float relx = prefabs.GetRelativeX(r, c);
                                pos.x = pixelx+relx;
                            }
                        } else
                        {
                            if (haligns[c] == HAlign.Center)
                            {
                                halignPad = (maxWidths[c]-size.x) / 2f;
                            } else if (haligns[c] == HAlign.Right)
                            {
                                halignPad = maxWidths[c]-size.x;
                            }
                            pos.x = pixelx-point.x+halignPad;
                        }
                        if (valigns[r] == VAlign.None)
                        {
                            if (r >= rowHeader&&c >= columnHeader)
                            {
                                UITableCell prefab = prefabs.GetPrefab(r, c);
                                pos.y = pixely+prefab.transform.localPosition.y;
                            }
                        } else
                        {
                            if (valigns[r] == VAlign.Center)
                            {
                                valignPad = (maxHeights[r]-size.y) / 2f;
                            } else if (valigns[r] == VAlign.Bottom)
                            {
                                valignPad = maxHeights[r]-size.y;
                            }
                            pos.y = pixely-(point.y+size.y)-valignPad;
                        }
						
                        t.SetLocalPosition(pos, 0.01f);
                        NGUIUtil.ApplyPivot(t);
						
                        // update Collider Bound
                        if (resizeCollider)
                        {
                            BoxCollider box = cell.collider;
                            if (box != null)
                            {
                                Vector3 center = box.center; 
                                center.x = pixelx+maxWidths[c] * 0.5f;
                                center.y = pixely-maxHeights[r] * 0.5f;
                                Vector3 boxSize = box.size;
                                boxSize.x = maxWidths[c];
                                boxSize.y = maxHeights[r];
                                if (expandColliderToPadding)
                                {
                                    if (c < col-1)
                                    {
                                        boxSize.x += padding.x;
                                        center.x += padding.x * 0.5f;
                                    }
                                    if (r < row-1)
                                    {
                                        boxSize.y += padding.y;
                                        center.y -= padding.y * 0.5f;
                                    }
                                }
                                center = transform.TransformSpace(center, t);
                                center.z = box.center.z;
                                box.center = center;
                                box.size = boxSize;
                            } else
                            {
                                BoxCollider2D box2d = cell.collider2d;
                                if (box2d != null)
                                {
                                    Vector2 center = box2d.offset; 
                                    center.x = pixelx+maxWidths[c] * 0.5f;
                                    center.y = pixely-maxHeights[r] * 0.5f;
                                    Vector2 boxSize = box2d.size;
                                    boxSize.x = maxWidths[c];
                                    boxSize.y = maxHeights[r];
                                    if (expandColliderToPadding)
                                    {
                                        if (c < col-1)
                                        {
                                            boxSize.x += padding.x;
                                            center.x += padding.x * 0.5f;
                                        }
                                        if (r < row-1)
                                        {
                                            boxSize.y += padding.y;
                                            center.y -= padding.y * 0.5f;
                                        }
                                    }
                                    center = transform.TransformSpace(center, t);
                                    box2d.offset = center;
                                    box2d.size = boxSize;
                                }
                            }
                        }
                        //					if (bound.width != 0) {
                        //						bound.x = Math.Min(bound.x, pos.x);
                        //					}
                        //					if (bound.height != 0) {
                        //						bound.y = Math.Max(bound.y, pos.y);
                        //					}
                    }
                    Vector3 extent = bounds[r, c].extents;
                    bounds[r, c].center = new Vector2(pixelx+extent.x, pixely-extent.y);

                    cellPos[r, c] = new Vector2(pixelx, pixely);
                    activeRow |= cell == null||cell.go.activeInHierarchy;
                    pixelx += maxWidths[c]+padding.x;
                }
                if (activeRow)
                {
                    pixely += -maxHeights[r]-padding.y;
                }
            }
			
            if (NGUIUtil.IsValid(bound))
            {
//				UIDraggablePanel drag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
//				if (drag != null) drag.UpdateScrollbars(true);
            } else
            {
                bound = new Rect();
            }
            if (propagateReposition)
            {
                NGUIUtil.Reposition(transform);
            }
        }

        private Bounds CalculateBounds(UITableCell c)
        {
            if (c != null)
            {
                Transform t = c.trans;
                GameObject o = c.go;
                if (!o.activeInHierarchy)
                {
                    return new Bounds();
                }
                if (c != null&&c.bound != null)
                {
                    return c.bound.CalculateBounds(c.transform);
                }
                return GetBounds(t);
            } else
            {
                return new Bounds();
            }
        }

        private TableInfo GetPrefabs()
        {
            return new TableInfo(prefab, rowHeader, columnHeader);
        }

        public void RefreshContents()
        {
            if (model == null)
            {
                emptyObj.SetActiveEx(true);
                return;
            }
            List<object> sel = GetSelectedDataList<object>();
            emptyObj.SetActiveEx(model.IsEmpty());
            AssertDimension();
            TableInfo prefabs = GetPrefabs();
            if (isHorizontal)
            {
                for (int r = 0; r < model.GetRowCount(); r++)
                {
                    for (int c = 0; c < model.GetColumnCount(); c++)
                    {
                        object cellValue = model.GetValue(r, c);
                        SetCellValue(prefabs, r+rowHeader, c+columnHeader, cellValue, initFunc);
                    }
                }
                /*  여분의 Row를 삭제한다. */
                for (int r = rowCount-1; r >= rowHeader+model.GetRowCount(); r--)
                {
                    #pragma warning disable 0618
                    RemoveRow(r);
                    #pragma warning restore 0618
                }
            } else
            {
                for (int c = 0; c < model.GetColumnCount(); c++)
                {
                    for (int r = 0; r < model.GetRowCount(); r++)
                    {
                        object cellValue = model.GetValue(r, c);
                        SetCellValue(prefabs, r+rowHeader, c+columnHeader, cellValue, initFunc);
                    }
                }
                for (int c = columnCount-1; c >= columnHeader+model.GetColumnCount(); c--)
                {
                    #pragma warning disable 0618
                    RemoveColumn(c);
                    #pragma warning restore 0618
                }
            }
            if (enabled)
            {
                DoLayout();
            }
            SelectCell<object>(o => sel.Contains(o));
			
            foreach (UITableEventListener l in listeners)
            {
                l.OnModelChanged();
            }
        }

        /**
		 * Cell이 비었을 경우 prefab으로 새로 생성하여 Cell에 집어넣는다.
		 * cell 값이 GameObject일 경우 prefab이라고 가정하고 새로 생성한다.
		 * 이외의 값일 경우 ToString()을 사용하여 Label을 넣는다.
		 */
        private void SetCellValue(TableInfo prefabs, int row, int col, object cellValue, Action<UITableCell> initFunc)
        {
            UITableCell cell = GetCell(row, col);
            if (cellValue == null)
            {
                if (cell != null)
                {
                    cell.gameObject.SetActive(false);
                }
                return;
            }
			
            // Instantiate Cell
            if (cell == null)
            {
                cell = prefabs.Instantiate(row, col);
                if (!cell.trans.IsChildOf(transform))
                {
                    cell.trans.SetParent(transform, false);
                }
            }
            // Set Cell Value
            SetCell(row, col, cell);
			
            UITableCell.SetValue(this, cell, row-rowHeader, col-columnHeader, cellValue, initFunc);
        }

        private void AssertDimension()
        {
            Assert.IsTrue(model.IsEmpty()
            ||(isHorizontal&&GetMaxPerLine()-columnHeader == model.GetColumnCount())
            ||(isVertical&&GetMaxPerLine()-rowHeader == model.GetRowCount()),
                "[Grid]{0} [Model]{1} [Grid]{2}x{3} [Model]{4}x{5} [Header]{6}x{7} ",
                name, model.GetType().FullName, 
                rowCount, columnCount,
                model.GetRowCount(), model.GetColumnCount(),
                rowHeader, columnHeader
            );
        }

        [NoObfuscate]
        public void OnRowSelected(object row)
        {
            int rowNo = (int)row;
            foreach (UITableEventListener l in listeners)
            {
                l.OnRowSelected(rowNo);
            }
        }

        override protected void UpdateImpl()
        {
            if (model != null)
            {
                model.Update(RefreshContents);
            }
        }

        private void MakePrefabInactive(params UITableCell[] prefab)
        {
            if (prefab != null&&Application.isPlaying)
            {
                foreach (var r in prefab)
                {
                    if (r != null)
                    {
                        r.go.SetActive(false);
                        int index = GetIndex(r);
                        if (index >= 0)
                        {
                            components[index] = null;
                        }
                    }
                }
            }
        }

        public void ForEach<C>(Predicate<C> func, bool includeInactive = false) where C: UITableCell
        {
            foreach (UITableCell c in components)
            {
                if (c != null&&(includeInactive||c.gameObject.activeSelf))
                {
                    if (!func(c as C))
                    {
                        return;
                    }
                }
            }
        }

        public void ForEach<C>(Action<C> func, bool includeInactive = false) where C:UITableCell
        {
            foreach (var c in components)
            {
                if (c != null&&(includeInactive||c.go.activeSelf))
                {
                    if (c != null)
                    {
                        func(c as C);
                    }
                }
            }
        }

        public int GetCount<C>(Predicate<C> predicate) where C : UITableCell
        { 
            int count = 0;
            foreach (var c in components)
            {
                if (c != null&&c.go.activeSelf)
                {
                    if (predicate(c as C))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public List<V> ConvertCells<C, V>(Converter<C, V> conv) where C: UITableCell
        { 
            List<V> list = new List<V>();
            foreach (var c in components)
            {
                if (c != null&&c.gameObject.activeSelf)
                {
                    V v = conv(c as C);
                    if (v != null)
                    {
                        list.Add(v);
                    }
                }
            }
            return list;
        }

        public List<C> FilterCell<C>(Predicate<C> func) where C:UITableCell
        { 
            List<C> list = new List<C>();
            foreach (var c in components)
            {
                if (c != null&&c.gameObject.activeSelf)
                {
                    var cell = c as C;
                    if (func(cell))
                    {
                        list.Add(cell);
                    }
                }
            }
            return list;
        }

        public List<D> FilterCellData<D>(Predicate<D> func)
        { 
            List<D> list = new List<D>();
            foreach (var c in components)
            {
                if (c != null&&c.gameObject.activeSelf)
                {
                    D cellData = (D)c.data;
                    if (cellData != null&&func(cellData))
                    {
                        list.Add(cellData);
                    }
                }
            }
            return list;
        }

        public C GetSelectedCell<C>() where C: UITableCell
        { 
            foreach (var c in components)
            {
                if (c != null&&c.gameObject.activeSelf)
                {
                    if (c.go.activeSelf&&c.toggle != null&&c.toggle.value)
                    {
                        return c as C;
                    }
                }
            }
            return  null;
        }

        public List<C> GetSelectedCellList<C>() where C: UITableCell
        { 
            return ConvertCells<C, C>(c =>
            {
                if (c.toggle != null&&c.toggle.value)
                {
                    return c;
                } else
                {
                    return null;
                }
            });
        }

        public C GetSelectedData<C>()
        { 
            for (int r = rowHeader; r < rowCount; ++r)
            {
                for (int c = columnHeader; c < columnCount; ++c)
                {
                    UITableCell cell = GetCell(r, c);
                    if (cell.toggle != null&&cell.toggle.value)
                    {
                        return (C)cell.data;
                    }
                }
            }
            return default(C);
        }

        public List<C> GetSelectedDataList<C>()
        { 
            return ConvertCells<UITableCell, C>(c =>
            {
                if (c.toggle != null&&c.toggle.value)
                {
                    return (C)c.data;
                } else
                {
                    return default(C);
                }
            });
        }

        public bool SelectCell<D>(Predicate<D> predicate, bool includeInactive = false)
        { 
            bool selected = false;
            ForEach<UITableCell>(c =>
            {
                if (c.toggle != null)
                {
                    // Setting grouped toggle on inactive toggle incurs an abnormal behaviour.
                    if (c.toggle.isActiveAndEnabled||c.toggle.group == 0)
                    {
                        bool s = predicate((D)c.data);
                        c.SetSelected(s);
                    }
                }
                return true;
            }, includeInactive);
            return selected;
        }

        public void SelectCell(object cellData)
        { 
            ForEach<UITableCell>(cell =>
            {
                if (cell.toggle != null)
                {
                    // Setting grouped toggle on inactive toggle incurs an abnormal behaviour.
                    if (cell.toggle.isActiveAndEnabled||cell.toggle.group == 0)
                    {
                        bool select = cell.data == cellData;
                        cell.SetSelected(select);
                    }
                }
            });
        }

        public void SelectCell(int row, int col)
        { 
            UITableCell cell = GetCell(row, col);
            if (cell != null&&cell.toggle != null&&cell.go.activeSelf)
            {
                cell.SetSelected(true);
            }
        }

        /// <summary>
        /// Select one cell predicate is met. If none, select the first one.
        /// </summary>
        /// <returns>The one.</returns>
        /// <param name="predicate">Predicate.</param>
        /// <typeparam name="C">CellData type</typeparam>
        public D SelectOne<D>(Predicate<D> predicate)
        {
            D select = default(D);
            List<D> list = FilterCellData<D>(predicate);
            if (list.IsNotEmpty())
            {
                select = list[0];
            } else
            {
                UITableCell c = GetCell(0, 0);
                if (c != null)
                {
                    select = (D)c.data;
                }
            }
            if (select != null)
            {
                SelectCell(select);
            }
            return select;
        }

        #region IEnumerable

        IEnumerator<UITableCell> IEnumerable<UITableCell>.GetEnumerator()
        {
            foreach (var c in components)
            {
                if (c != null&&c.go.activeSelf)
                {
                    yield return c;
                }
            }
            //		return ((IEnumerable<Transform>)components).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return components.GetEnumerator();
        }

        #endregion

        public void ResetPosition()
        {
            UIScrollView view = GetComponentInParent<UIScrollView>();
            if (view != null)
            {
                view.ResetPosition();
            }
        }
		
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (bounds == null)
            {
                return;
            }
            Gizmos.matrix = transform.localToWorldMatrix;

            Bounds b = new Bounds();
            if (UnityEditor.Selection.activeGameObject == gameObject)
            {
                b = GetBounds();
            } else
            {
                // FIXMEM grid layout GIZMO
//				for (int r=0; r<bounds.GetLength(0) && b.size == Vector3.zero; r++) {
//					for (int c=0; c<bounds.GetLength(1) && b.size == Vector3.zero; c++) {
//						if (GetCell(r, c) != null && UnityEditor.Selection.activeGameObject == GetCell(r, c).gameObject) {
//							b = bounds[r,c];
//						}
//					}
//				}
            }
            DrawBounds(b);
        }

        private void DrawBounds(Bounds b)
        {
            float gizmoSize = 10;
            Vector3 size = b.size;
            if (size != Vector3.zero)
            {
                Gizmos.color = gizmoColor;
                Vector3 point = b.center;
                Gizmos.DrawWireCube(new Vector3(point.x, point.y, 1), new Vector3(size.x, size.y, 1));
                Gizmos.color = new Color(1-gizmoColor.r, 1-gizmoColor.g, 1-gizmoColor.b, 1);
                Gizmos.DrawCube(b.min, new Vector3(gizmoSize, gizmoSize, 1));
                Gizmos.DrawCube(b.max, new Vector3(gizmoSize, gizmoSize, 1));
                Gizmos.DrawCube(new Vector3(b.min.x, b.max.y, 1), new Vector3(gizmoSize, gizmoSize, 1));
                Gizmos.DrawCube(new Vector3(b.max.x, b.min.y, 1), new Vector3(gizmoSize, gizmoSize, 1));
                Gizmos.DrawWireSphere(b.center, 5);
            }
        }
        #endif
    }
}