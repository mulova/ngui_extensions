//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using ngui.ex;
using mulova.comunity;
using UnityEngine.Ex;

namespace ngui.ex
{
	[RequireComponent(typeof(UIDragScrollView))]
	public class ScrollZone : MonoBehaviour
	{
		public UIDragScrollView scroll;
		public bool resetPosition = false;
		public bool resetToEnd = false;
		public bool adjustDepth = false;
		private Vector4 baseClipRegion;

		void OnEnable()
		{
			if (scroll == null)
			{
				scroll = gameObject.FindComponent<UIDragScrollView>();
				transform.localScale = Vector3.one;
			}
            if (resetPosition)
            {
                scroll.scrollView.ResetPosition();
            }
		}

		private void AdjustDepth()
		{
			// set widget depth above 1
			int minDepth = int.MaxValue;
			List<UIWidget> widgets = new List<UIWidget>();
			foreach (BoxCollider2D b in scroll.scrollView.GetComponentsInChildren<BoxCollider2D>(true))
			{
				UIWidget w = b.GetComponent<UIWidget>();
				if (w != null)
				{
					widgets.Add(w);
					minDepth = Mathf.Min(minDepth, w.depth);
				}
			}
			if (minDepth <= 1)
			{
				foreach (UIWidget w in widgets)
				{
					w.depth += 2-minDepth;
				}
			}
		}

		public static void CreateScrollZone(UIScrollView scroll)
		{
			string name = scroll.name+"_zone";
			GameObject zoneObj = scroll.gameObject.CreateSibling(name);
			AddScrollZone(zoneObj, scroll);
			GameObject childObj = scroll.gameObject.CreateChild(name);
			AddScrollZone(childObj, scroll);
		}

		static void AddScrollZone(GameObject zoneObj, UIScrollView scroll)
		{
			UIDragScrollView drag = zoneObj.FindComponent<UIDragScrollView>();
			drag.scrollView = scroll;
			zoneObj.FindComponent<ScrollZone>();
		}
	}
    
}