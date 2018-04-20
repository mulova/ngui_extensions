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
    public abstract class UITableCell : comunity.Script
	{
		public UIWidget bound;
		public UIToggle toggle;
        private bool isColliderAssigned;
        [SerializeField] private BoxCollider _collider;
        private bool isCollider2DAssigned;
        [SerializeField] private BoxCollider2D _collider2d;
        [HideInInspector] public int row;
        [HideInInspector] public int column;

        protected UITableLayout table;
        protected Action<UITableCell> initFunc { get; set; }
        public object data { get; protected set; }

        public BoxCollider collider
        {
            get
            {
                if (_collider == null && !isColliderAssigned)
                {
                    isColliderAssigned = true;
                    _collider = GetComponent<BoxCollider>();
                }
                return _collider;
            }
        }

        public BoxCollider2D collider2d
        {
            get
            {
                if (_collider2d == null && !isCollider2DAssigned)
                {
                    isCollider2DAssigned = true;
                    _collider2d = GetComponent<BoxCollider2D>();
                }
                return _collider2d;
            }
        }

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
        public static void SetValue(UITableLayout grid, UITableCell c, int row, int column, object val, Action<UITableCell> initFunc = null) {
			if (c != null) {
                c.table = grid;
				c.row = row;
				c.column = column;
                c.SetCell(val, initFunc);
			} else {
                c.go.SetActive(val != null);
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
