//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using mulova.comunity;
using UnityEngine;
using Nullable = mulova.commons.Nullable;

namespace ngui.ex
{
    public abstract class UITableCell : LogBehaviour
	{
		[Nullable] public UIWidget bound;
		[Nullable] public UIToggle toggle;
        [HideInInspector] public int row;
        [HideInInspector] public int column;

        protected UITableLayout container;
        protected Action<UITableCell> initFunc { get; set; }
        public object data { get; protected set; }

		public void Refresh() {
            SetCell(data, initFunc);
		}
		
        public virtual void SetCell(object val, Action<UITableCell> initFunc = null) {
			this.data = val;
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
        public static void SetValue(UITableLayout table, UITableCell c, int row, int column, object val, Action<UITableCell> initFunc = null) {
			if (c != null) {
                c.container = table;
				c.row = row;
				c.column = column;
                c.SetCell(val, initFunc);
			} else {
                c.gameObject.SetActive(val != null);
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
