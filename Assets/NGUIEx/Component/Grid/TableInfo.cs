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
    public class TableInfo
	{
		public UITableCell[] prefabs;
		public int rowHeader;
		public int columnHeader;
        public bool horizontal;

        public TableInfo( UITableCell[] prefabs, int rowHeader = 0, int colHeader = 0, bool horizontal = false)
		{
			this.prefabs = prefabs;
			this.rowHeader = rowHeader;
			this.columnHeader = colHeader;
            this.horizontal = horizontal;
		}

        public float GetRelativeX(int row, int col)
        {
            // TODOM
            return GetPrefab(row, col).transform.localPosition.x;
        }

        public float GetRelativeY(int row, int col)
        {
            // TODOM
            return GetPrefab(row, col).transform.localPosition.y;
        }

		public UITableCell GetPrefab(int row, int col)
		{
            if (horizontal)
            {
                if (col < columnHeader)
                {
                    return prefabs[col];
                } else
                {
                    return prefabs[(col-columnHeader) % prefabs.Length+columnHeader];
                }
            } else
            {
                if (row < rowHeader)
                {
                    return prefabs[row];
                } else
                {
                    return prefabs[(row-rowHeader) % prefabs.Length+rowHeader];
                }
            }
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