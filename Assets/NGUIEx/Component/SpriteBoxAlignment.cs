//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;

namespace ngui.ex
{
	[ExecuteInEditMode]
	public class SpriteBoxAlignment : MonoBehaviour
	{
		public Transform leftTop;
		public Transform centerTop;
		public Transform rightTop;
		public Transform leftCenter;
		public Transform center;
		public Transform rightCenter;
		public Transform leftBottom;
		public Transform centerBottom;
		public Transform rightBottom;
		
		public Vector2 size;

		void Start()
		{
			Destroy(this);
		}

		private Vector3 Abs(Vector3 v)
		{
			return new Vector3(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z));
		}

		void Update()
		{
			if (leftTop == null||centerTop == null||rightTop == null
			    ||leftCenter == null||center == null||rightCenter == null
			    ||leftBottom == null||centerBottom == null||rightBottom == null)
			{
				return;
			}
			Vector3 ltSize = leftTop.localScale;
			Vector3 ctSize = centerTop.localScale;
			Vector3 rtSize = Abs(rightTop.localScale);
			
			
			Vector3 lcSize = leftCenter.localScale;
			Vector3 rcSize = Abs(rightCenter.localScale);
			
			Vector3 lbSize = leftCenter.localScale;
			Vector3 cbSize = center.localScale;
			Vector3 rbSize = rightCenter.localScale;
			
			// TOP
			leftTop.localPosition = Vector3.zero;
			
			ctSize.x = size.x-ltSize.x-rtSize.x;
			centerTop.localPosition = new Vector3(ltSize.x, 0, 0);
			centerTop.localScale = ctSize;
			
			rightTop.localPosition = new Vector3(size.x-rtSize.x, 0, 0);
			
			// CENTER
			leftCenter.localPosition = new Vector3(0, -ltSize.y, 0);
			lcSize.y = size.y-ltSize.y-lbSize.y;
			leftCenter.localScale = lcSize;
			
			center.localPosition = new Vector3(lcSize.x, -ctSize.y, 0);
			center.localScale = new Vector3(size.x-lcSize.x-rcSize.x, -(size.y-ctSize.y-cbSize.y), 1);
			
			rightCenter.localPosition = new Vector3(size.y-rcSize.x, -rtSize.y, 0);
			rcSize.y = size.y-rtSize.y-rbSize.y;
			rightCenter.localScale = rcSize;
			
			// BOTTOM
			leftBottom.localPosition = new Vector3(0, -(size.y-lbSize.y), 0);
			
			centerBottom.localPosition = new Vector3(ltSize.x, -(size.y-cbSize.y), 0);
			cbSize.x = size.x-lbSize.x-rbSize.x;
			centerBottom.localScale = cbSize;
			
			rightBottom.localPosition = new Vector3(size.x-rbSize.x, -(size.y-rbSize.y), 0);
			
		}
	}
}