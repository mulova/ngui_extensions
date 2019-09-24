//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------


using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{
    [ExecuteInEditMode]
	public class UIBorderLayout : UIPivot
	{
		public Transform center;
		public Transform top;
		public Transform bottom;
		public Transform left;
		public Transform right;
		
		public float width = 720;
		public float height = 1280;

		void Start()
		{
			if (center != null&&Application.isPlaying)
			{
				UIPanel panel = center.GetComponent<UIPanel>();
				if (panel.clipping == UIDrawCall.Clipping.None)
				{
					panel.clipping = UIDrawCall.Clipping.SoftClip;
				}
			}
		}

		override protected void DoLayout()
		{
			Bounds topBound = GetBounds(top);
			Bounds bottomBound = GetBounds(bottom);
			Bounds leftBound = GetBounds(left);
			Bounds rightBound = GetBounds(right);
			
			Vector3 topPoint = topBound.min;
			Vector3 topSize = topBound.size;
			Vector3 bottomPoint = bottomBound.min;
			Vector3 bottomSize = bottomBound.size;
			Vector3 leftPoint = leftBound.min;
			Vector3 leftSize = leftBound.size;
			Vector3 rightPoint = rightBound.min;
			Vector3 rightSize = rightBound.size;
			// Align Top
			if (top != null)
			{
				Vector3 topPos = top.localPosition;
				topPos.x = -topPoint.x;
				topPos.y = -topPoint.y;
				top.SetLocalPosition(topPos, 0.01f);
				NGUIUtil.ApplyPivot(top);
			}
			
			// Align Bottom
			if (bottom != null)
			{
				Vector3 botPos = bottom.localPosition;
				botPos.x = -bottomPoint.x;
				botPos.y = -height-bottomSize.y-bottomPoint.y;
				bottom.SetLocalPosition(botPos, 0.01f);
				bottom.localPosition = botPos;
				NGUIUtil.ApplyPivot(bottom);
			}
			
			// Align Left
			if (left != null)
			{
				Vector3 leftPos = left.localPosition;
				leftPos.x = -leftPoint.x;
				leftPos.y = -leftPoint.y+topSize.y;
				left.SetLocalPosition(leftPos, 0.01f);
				NGUIUtil.ApplyPivot(left);
			}
			
			// Align Right
			if (right != null)
			{
				Vector3 rightPos = right.localPosition;
				rightPos.x = width-rightPoint.x-rightSize.x;
				rightPos.y = -rightPoint.y+topSize.y;
				right.SetLocalPosition(rightPos, 0.0001f);
				NGUIUtil.ApplyPivot(right);
			}
			
			if (center != null)
			{
				Vector3 pos = center.transform.localPosition;
				pos.x = 0;
				pos.y = 0;
				center.SetLocalPosition(pos, 0.0001f);
				
				float x = (leftSize.x-rightSize.x) / 2;
				float y = (topSize.y-bottomSize.y) / 2;
				float w = width-(leftSize.x+rightSize.x);
				float h = height+topSize.y+bottomSize.y;
				NGUIUtil.SetBoundingBox(center, new Rect(x, y, w, h));
				NGUIUtil.ApplyPivot(center);
			}
			
//			return new Bounds(new Vector3(width*0.5f, height*0.5f, 0), new Vector3(0, width, -height));
		}

		/**
		 * Top, Bottom 이 움직였을때 Center Panel의 ClipRange를 수정한다.
		 */
		public void UpdateClipRange()
		{
			float y0 = top != null? top.localPosition.y : 0;
			float y1 = bottom != null? bottom.localPosition.y : 0;
			Bounds topBound = GetBounds(top);
			Bounds bottomBound = GetBounds(bottom);
			Vector3 topPoint = topBound.min;
			Vector3 topSize = topBound.size;
			Vector3 bottomPoint = bottomBound.min;
			
			y0 += topPoint.y;
			y1 += bottomPoint.y;
			y0 += topSize.y;
			float h = y0-y1;
			
			if (center != null)
			{
				UIPanel panel = center.GetComponent<UIPanel>();
				if (panel != null)
				{
					Vector4 clip = panel.baseClipRegion;
					clip.w = h;
					clip.y = height / 2+(y1+y0) / 2-center.localPosition.y;
					panel.baseClipRegion = clip;
				}
			}
		}
	}
}
