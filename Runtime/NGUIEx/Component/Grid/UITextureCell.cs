//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using mulova.comunity;
using UnityEngine.Ex;

namespace ngui.ex {
	[RequireComponent(typeof(UITexture))]
	public class UITextureCell : UITableCell
	{
		protected override void DrawCell (object val)
		{
			TexLoader loader = gameObject.FindComponent<TexLoader>();
			loader.Load(val as string, null);
		}
	}
}
