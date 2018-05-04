//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Text;
using comunity;

namespace ngui.ex
{
	/**
	 * Get GridLayout's row/cell prefab information
	 */
    public class UITablePrefabs
	{
		public UITableCell[] rowPrefab;
		public UITableCell[] columnPrefab;
		public UITableCell defaultPrefab;
		public int rowHeader;
		public int columnHeader;

		public UITablePrefabs(UITableCell defaultPrefab)
		{
			this.defaultPrefab = defaultPrefab;
		}

		public UITablePrefabs(UITableCell[] rowPrefabs, UITableCell[] columnPrefabs)
		{
			this.rowPrefab = rowPrefabs;
			this.columnPrefab = columnPrefabs;
		}

		public UITablePrefabs(UITableCell defaultPrefab, 
		                     UITableCell[] rowPrefabs, UITableCell[] columnPrefabs,
		                     int rowHeader, int colHeader)
		{
			this.defaultPrefab = defaultPrefab;
			this.rowPrefab = rowPrefabs;
			this.columnPrefab = columnPrefabs;
			this.rowHeader = rowHeader;
			this.columnHeader = colHeader;
		}

		public UITableCell GetPrefab(int row, int col)
		{
			if (col < columnPrefab.Length&&columnPrefab[col] != null)
			{
				return columnPrefab[col];
			}
			if (row < rowPrefab.Length&&rowPrefab[row] != null)
			{
				return rowPrefab[row];
			}
			if (defaultPrefab == null)
			{
				if (columnPrefab.Length > columnHeader)
				{
					int size = columnPrefab.Length-columnHeader;
					int index = (col-columnHeader) % size+columnHeader;
					if (columnPrefab[index] != null)
					{
						return columnPrefab[index];
					}
				}
				if (rowPrefab.Length > rowHeader)
				{
					int size = rowPrefab.Length-rowHeader;
					return rowPrefab[(row-rowHeader) % size+rowHeader];
				}
			}
			return defaultPrefab;
		}

		public UITableCell Instantiate(int row, int col)
		{
			UITableCell prefab = GetPrefab(row, col);
            UITableCell instance = prefab.InstantiateEx(prefab.transform.parent, false);
			instance.go.SetActive(true);
			StringBuilder str = new StringBuilder(prefab.name.Length+8);
			str.Append(prefab.name);
			str.Append("_").Append(row);
			str.Append("_").Append(col);
			instance.name = str.ToString();
			return instance;
		}
	}
}