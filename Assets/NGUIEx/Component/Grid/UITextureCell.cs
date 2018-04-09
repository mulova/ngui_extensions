//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using comunity;

namespace ngui.ex {
	[RequireComponent(typeof(UITexture))]
	public class UITextureCell : UIGridCell
	{
		protected override void DrawCell (object val)
		{
			TexLoader loader = gameObject.GetComponentEx<TexLoader>();
			loader.Load(val as string, null);
		}
	}
}
