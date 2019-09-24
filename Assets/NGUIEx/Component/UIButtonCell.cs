//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using mulova.commons;

/**
 * Contains grid cell's title, event listener info
 * Set only non-null values
 */


namespace ngui.ex
{
	
	public abstract class UIButtonCell : UITableCell
	{
		public UIButton button;
		public UILabel buttonLabel;

		override
		protected void DrawCell(object val)
		{
			UIButtonCellData cellData = (UIButtonCellData)val;
			if (buttonLabel != null&&cellData.text != null)
			{
				buttonLabel.SetText(cellData.text);
			}
			if (button != null)
			{
				button.isEnabled = cellData.buttonEnabled;
				EventDelegateUtil.AddCallback<UIButtonCell>(button.onClick, OnButtonClick, button, this);
			}
		}

		[NoObfuscate]
		protected abstract void OnButtonClick(Object param);
	}
}
