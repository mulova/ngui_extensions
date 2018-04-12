//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------
using System;
using UnityEngine;
using comunity;


namespace ngui.ex
{
    public abstract class UILazyTableCell : UITableCell
    {
        public UIPanel panel;
        public GameObject ui;
        public GameObject uiPrefab;

        // transient
        private Vector3 scrollPos;
        private Vector3 pos1;
        private Vector3 pos2;
        private Vector3 pos3;
        private Vector3 pos4;
        private const float SCROLL_BOUND = 1f; // expand bound to show cell early before it is actually shown

        protected abstract void InitData(object val);

        protected abstract void DrawCell();

        public event Action<UILazyTableCell> onVisible;
        public event Action<UILazyTableCell> onInvisible;

        private bool IsLazy()
        {
            return panel != null&&bound != null&&(ui != null||uiPrefab != null);
        }

        public bool initialized { get; private set; }

        public override void SetCell(object val, Action<UITableCell> initFunc = null)
        {
            initialized = false;
            Invalidate();
            this.data = val;
            this.initFunc = initFunc;
            InitData(val);
            if (IsLazy())
            {
                ui.SetActiveEx(false);
            } else
            {
                SetCellValue();
            }
            gameObject.SetActive(val != null);
        }

        private void SetCellValue()
        {
            initialized = true;
            if (ui == null&&uiPrefab != null)
            {
                ui = uiPrefab.InstantiateEx(transform);
            }
            if (ui != null)
            {
                ui.SetActive(true);
            }
            if (initFunc != null)
            {
                initFunc(this);
            }
            DrawCell();
        }

        protected override void DrawCell(object val)
        {
        }

        private void Invalidate()
        {
            scrollPos = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        }

        void OnDisable()
        {
            Hide();
        }

        protected void Hide()
        {
            Invalidate();
            if (ui != null&&ui.activeSelf)
            {
                ui.SetActive(false);
                if (onInvisible != null)
                {
                    onInvisible(this);
                }
            }
        }

        protected void Show()
        {
            if (!initialized)
            {
                SetCellValue();
            }
            if (ui != null&&!ui.activeSelf)
            {
                ui.SetActive(true);
                if (onVisible != null)
                {
                    onVisible(this);
                }
            }
        }

        public bool isVisible
        {
            get
            {
                return ui != null&&ui.activeSelf;
            }
        }

        void Update()
        {
            if (!IsLazy())
            {
                return;
            }
            if (scrollPos != panel.cachedTransform.position)
            {
                CheckVisibility();
            }
        }

        public void CheckVisibility()
        {
            if (ui == null)
            {
                return;
            }
            if (ui.activeSelf)
            {
                RefreshBounds();
                if (panel.IsVisible(pos1, pos2, pos3, pos4))
                {
                    if (!initialized)
                    {
                        SetCellValue();
                    }
                } else
                {
                    // Do not hide if the cell is dragged to receive scroll event
                    if (!IsDragged())
                    {
                        Hide();
                    }
                }
            } else
            {
                RefreshBounds();
                if (panel.IsVisible(pos1, pos2, pos3, pos4))
                {
                    Show();
                }
            }
        }

        private bool IsDragged()
        {
            GameObject o = UICamera.selectedObject;
            return o != null&&o.transform.IsChildOf(trans);
        }

        private void RefreshBounds()
        {
            Bounds bounds = NGUIUtil.CalculateAbsoluteWidgetBounds(transform, bound);
            Vector3 extent = bounds.extents;
            Vector3 center = bounds.center;
            pos1 = center+new Vector3(-extent.x * SCROLL_BOUND, -extent.y * SCROLL_BOUND, 0);
            pos2 = center+new Vector3(-extent.x * SCROLL_BOUND, extent.y * SCROLL_BOUND, 0);
            pos3 = center+new Vector3(extent.x * SCROLL_BOUND, -extent.y * SCROLL_BOUND, 0);
            pos4 = center+new Vector3(extent.x * SCROLL_BOUND, extent.y * SCROLL_BOUND, 0);
            scrollPos = panel.cachedTransform.position;
        }
    }
}
