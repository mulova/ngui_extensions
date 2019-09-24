//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;

namespace ngui.ex {
	public class ToggleCell : UITableCell
	{
        public GameObject on;
        public GameObject off;

		protected override void DrawCell(object val)
		{
            bool b = (bool)val;
            on.SetActive(b);
            off.SetActive(!b);
		}
	}
}
