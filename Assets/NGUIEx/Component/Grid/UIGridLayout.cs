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
    [ExecuteInEditMode, RequireComponent(typeof(UIGrid))]
    public class UIGridLayout : UILayout, IEnumerable<Transform>
    {
        public UIWrapContent wrap;
        public UIGrid grid;
        public UIGridPrefabs prefabs = new UIGridPrefabs();

        protected override void DoLayout()
        {
            grid.Reposition();
        }

        protected override void UpdateImpl()
        {
        }

        public IEnumerator<Transform> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void SetContents(IList data, Action<UITableCell> initFunc = null)
        {
//            if (this.model == null)
//            {
//                SetModel(new UITableModel(data, IsHorizontal(), maxPerLine), initFunc);
//            } else
//            {
//                this.initFunc = initFunc;
//                this.model.SetContents(data, IsHorizontal(), maxPerLine);
//                RefreshContents();
//            }
        }

        public void SetContents(IEnumerable data, Action<UITableCell> initFunc = null)
        {
        }
    }
}