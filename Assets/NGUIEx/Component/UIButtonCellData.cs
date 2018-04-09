//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System;

namespace ngui.ex
{
	/**
	 * Contains grid cell's title, event listener info
	 * Set only non-null values
	 */
	[System.Serializable]
	public class UIButtonCellData
	{
		public string text;
		public bool buttonEnabled;
		public object param;

		public UIButtonCellData(string text, object param)
		{
			this.text = text;
			this.param = param;
		}
	}
	
}