using System;
using UnityEngine;
using comunity;


public static class UIScrollViewEx
{
	public static void ScrollCenter(this UIScrollView scroll, Transform target) {
		if (scroll != null && scroll.panel != null)
		{
			Vector3[] corners = scroll.panel.worldCorners;
			Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
			if (target != null && scroll != null && scroll.panel != null)
			{
				Transform panelTrans = scroll.panel.cachedTransform;

				// Figure out the difference between the chosen child and the panel's center in local coordinates
				Vector3 cp = panelTrans.InverseTransformPoint(target.position);
				Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
				Vector3 localOffset = cp - cc;

				// Offset shouldn't occur if blocked
				if (!scroll.canMoveHorizontally) localOffset.x = 0f;
				if (!scroll.canMoveVertically) localOffset.y = 0f;
				localOffset.z = 0f;

				// Spring the panel to this calculated position
				#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					panelTrans.localPosition = panelTrans.localPosition - localOffset;

					Vector4 co = scroll.panel.clipOffset;
					co.x += localOffset.x;
					co.y += localOffset.y;
					scroll.panel.clipOffset = co;
				}
				else
				#endif
				{
					Vector3 offset = -localOffset;
					offset.z = 0;
					scroll.MoveRelative (offset);
//					SpringPanel.Begin(scroll.panel.cachedGameObject,
//						panelTrans.localPosition - localOffset, springStrength).onFinished = onFinished;
				}
			}
		}
	}

	public static void ScrollTo(this UIScrollView scroll, Transform t, bool centered = true) {
		scroll.ResetPosition();
		Vector3 delta = t.TransformSpace(Vector3.zero, scroll.transform);
		Bounds cellBounds = NGUIMath.CalculateRelativeWidgetBounds(t);
		float x = -delta.x+cellBounds.extents.x;
		float y = -delta.y+cellBounds.extents.y;
		if (centered) {
			x += scroll.panel.GetViewSize().x/2;
			y -= scroll.panel.GetViewSize().y/2;
		}
		scroll.MoveRelative (new Vector3 (x, y, 0));
		scroll.RestrictWithinBounds(true);
	}

	public static void ScrollVerticalTo(this UIScrollView scroll, Transform t, bool centered = true) {
		scroll.ResetPosition();
		Vector3 delta = t.TransformSpace(Vector3.zero, scroll.transform);
		Bounds cellBounds = NGUIMath.CalculateRelativeWidgetBounds(t);
		float y = -delta.y+cellBounds.extents.y;
		if (centered) {
			y -= scroll.panel.GetViewSize().y/2;
		}
		scroll.MoveRelative (new Vector3 (0, y, 0));
		scroll.RestrictWithinBounds(true);
	}

	public static void ScrollHorizontalTo(this UIScrollView scroll, Transform t, bool centered = true) {
		scroll.ResetPosition();
		Vector3 delta = t.TransformSpace(Vector3.zero, scroll.transform);
		Bounds cellBounds = NGUIMath.CalculateRelativeWidgetBounds(t);
		float x = -delta.x + cellBounds.extents.x;
		if (centered) {
			x -= scroll.panel.GetViewSize().x/2;
		}
		scroll.MoveRelative (new Vector3 (x, 0, 0));
		scroll.RestrictWithinBounds(true);
	}

    public static void ResetPositionY(this UIScrollView scroll)
    {
        if (NGUITools.GetActive(scroll))
        {
            scroll.InvalidateBounds();
            Vector2 pv = NGUIMath.GetPivotOffset(scroll.contentPivot);
            
            // First move the position back to where it would be if the scroll bars got reset to zero
            scroll.SetDragAmount(0, 1f - pv.y, false);
            
            // Next move the clipping area back and update the scroll bars
            scroll.SetDragAmount(0, 1f - pv.y, true);
        }
    }

    public static void ResetPositionX(this UIScrollView scroll)
    {
        if (NGUITools.GetActive(scroll))
        {
            scroll.InvalidateBounds();
            Vector2 pv = NGUIMath.GetPivotOffset(scroll.contentPivot);
            
            // First move the position back to where it would be if the scroll bars got reset to zero
            scroll.SetDragAmount(pv.x, 0, false);
            
            // Next move the clipping area back and update the scroll bars
            scroll.SetDragAmount(pv.x, 0, true);
        }
    }

	public static bool IsContentsFitInBounds(this UIScrollView scroll)
	{
		if (scroll.panel == null)
		{
			return false;
		}
		Vector4 clip = scroll.panel.finalClipRegion;
		Bounds b = scroll.bounds;
		
		float hx = (clip.z == 0f) ? Screen.width  : clip.z * 0.5f;
		float hy = (clip.w == 0f) ? Screen.height : clip.w * 0.5f;
		
		if (scroll.canMoveHorizontally)
		{
			if (b.min.x < clip.x - hx) return false;
			if (b.max.x > clip.x + hx) return false;
		}
		
		if (scroll.canMoveVertically)
		{
			if (b.min.y < clip.y - hy) return false;
			if (b.max.y > clip.y + hy) return false;
		}
		return true;
	}
}

