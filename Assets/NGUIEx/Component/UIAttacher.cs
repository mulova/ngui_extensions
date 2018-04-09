//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;

namespace ngui.ex
{
	/// <summary>
	/// Attach UI at the corner of the target object
	/// </summary>
	[ExecuteInEditMode]
	public class UIAttacher : UILayout
	{
		public UIAnchor anchor;
		public UIPivot pivot;

		public void Reposition(UIAnchor.Side anchorSide, UIWidget.Pivot widgetPivot)
		{
			if (anchor == null||pivot == null)
			{
				return;
			}
			anchor.side = anchorSide;
			pivot.pivot = widgetPivot;
			InvalidateLayout();
			Reposition();
		}

		protected override void DoLayout()
		{
			if (anchor == null||pivot == null)
			{
				return;
			}
			anchor.enabled = true;
			pivot.InvalidateLayout();
			pivot.Reposition();
			pivot.transform.localPosition = Vector3.zero;
		}

		protected override void UpdateImpl()
		{
		}
	}

}