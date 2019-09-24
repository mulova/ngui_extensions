//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;

namespace ngui.ex {
	[RequireComponent(typeof(UISprite))]
	public class UISpriteCell : UITableCell
	{
		protected override void DrawCell (object val)
		{
			UISprite sprite = GetComponent<UISprite>();
			sprite.spriteName = val as string;
		}
	}
}
