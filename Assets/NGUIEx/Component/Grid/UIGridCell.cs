//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using System.Collections.Generic;
using commons;
using Nullable = commons.Nullable;
using Object = UnityEngine.Object;


namespace ngui.ex {
    public abstract class UIGridCell : comunity.Script
	{
		[Nullable] public UIWidget bound;
		[Nullable] public UIToggle toggle;
        [HideInInspector] public int row;
        [HideInInspector] public int column;

        protected UIGridLayout containerGrid;
        protected Action<UIGridCell> initFunc { get; set; }
		protected object val;

		public object GetCellData() {
			return val;
		}

		public T GetCellData<T>() {
			return (T)val;
		}

		public void Refresh() {
            SetCell(val, initFunc);
		}
		
        public virtual void SetCell(object val, Action<UIGridCell> initFunc = null) {
			this.val = val;
            this.initFunc = initFunc;
			gameObject.SetActive(val != null);
            if (initFunc != null)
            {
                initFunc(this);
            }
			DrawCell(val);
		}

		protected abstract void DrawCell(object val);
		
		/// <summary>
		/// Default behaviour for the Label, Slider, Sprite
		/// </summary>
		/// <param name="t">T.</param>
		/// <param name="val">Value.</param>
		/// <param name="status">Status.</param>
        public static void SetValue(UIGridLayout grid, Transform t, int row, int column, object val, Action<UIGridCell> initFunc = null) {
            UIGridCell c = t.GetComponent<UIGridCell>();
			if (c != null) {
                c.containerGrid = grid;
				c.row = row;
				c.column = column;
                c.SetCell(val, initFunc);
			} else {
                t.gameObject.SetActive(val != null);
			}
		}

		public bool IsSelected() {
			return toggle!=null && toggle.value;
		}
		public virtual void SetSelected(bool sel) {
			if (toggle == null) { return; }
			toggle.value = sel;
		}

        internal virtual void Clear() {}
	}
}
