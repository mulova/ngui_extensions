//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using mulova.unicore;
using UnityEngine;
using Assert = mulova.commons.Assert;

namespace ngui.ex
{
    [ExecuteInEditMode]
	public class UIPivot : UILayout
	{
		public UIWidget.Pivot pivot = UIWidget.Pivot.Center;
		private UIWidget.Pivot currentPivot;

		void OnEnable()
		{
			if (Platform.isEditor)
			{
				Assert.IsNull(GetComponent<UIWidget>());
				Assert.IsTrue(GetComponents<UIPivot>().Length == 1, "Name {0}. More than one Pivot", name);
			}
			InvalidateLayout();
		}

		override
		protected void DoLayout()
		{
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform);
			
			Vector2 pivotOffset = pivot.GetRelativeOffset();
			Vector3 point = bounds.min;
			Vector3 size = bounds.size;
			Vector3 delta = new Vector3(point.x+pivotOffset.x * size.x, point.y+pivotOffset.y * size.y, 0);
			foreach (Transform t in transform)
			{
				MovePosition(t, -delta.x, -delta.y, false);
			}
			foreach (UIRect r in transform.GetComponentsInChildren<UIRect>())
			{
				r.Invalidate(false);
			}
//			bounds.center -= delta;
//			return bounds;
		}

		override protected void UpdateImpl()
		{
			if (pivot != currentPivot)
			{
				currentPivot = pivot;
				InvalidateLayout();
			}
		}

		public static void MovePosition(Transform t, float dx, float dy, bool adjustCollider)
		{
			Vector3 pos = t.localPosition;
			pos.x += dx;
			pos.y += dy;
			t.localPosition = pos;
			
			if (adjustCollider)
			{
				BoxCollider2D collider = t.GetComponent<BoxCollider2D>();
				if (collider != null)
				{
					Vector2 offset = collider.offset;
					offset.x -= dx;
					offset.y -= dy;
					collider.offset = offset;
				}
			}
			UIPanel panel = t.GetComponent<UIPanel>();
			if (panel != null&&panel.clipping != UIDrawCall.Clipping.None)
			{
				Vector4 clipRange = panel.baseClipRegion;
				clipRange.x -= dx;
				clipRange.y -= dy;
				panel.baseClipRegion = clipRange;
			}
		}

        [ContextMenu("Execute")]
        private void Execute()
        {
            InvalidateLayout();
            Reposition();
        }
		
		#if UNITY_EDITOR
		//	virtual protected void OnGUI() {
		//		if (!Application.isPlaying) {
		//			Invalidate();
		//		}
		//		Reposition();
		//	}
		
		/// <summary>
		/// Draw some selectable gizmos.
		/// </summary>
		protected void OnDrawGizmos()
		{
			Color outline = new Color(1f, 1f, 1f, 0.2f);
			
			Bounds bound = GetBounds();
			// Position should be offset by depth so that the selection works properly
			Vector3 point = bound.min;
			point.z = 0;
			Vector3 size = bound.size+new Vector3(2, 2, 0);
			Vector3 pos = transform.position+point;
			
			// Draw the gizmo
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = (UnityEditor.Selection.activeGameObject == gameObject)? Color.green : outline;
			Gizmos.DrawWireCube(pos+size * 0.5f, size);
		}
		#endif
	}
	
}

public static class UIWidgetPivotEx
{
	public static Vector2 GetRelativeOffset(this UIWidget.Pivot pivot)
	{
		Vector2 v = Vector2.zero;
		
		if (pivot == UIWidget.Pivot.Top||pivot == UIWidget.Pivot.Center||pivot == UIWidget.Pivot.Bottom)
			v.x = 0.5f;
		else if (pivot == UIWidget.Pivot.TopRight||pivot == UIWidget.Pivot.Right||pivot == UIWidget.Pivot.BottomRight)
			v.x = 1f;
		
		if (pivot == UIWidget.Pivot.Left||pivot == UIWidget.Pivot.Center||pivot == UIWidget.Pivot.Right)
			v.y = 0.5f;
		else if (pivot == UIWidget.Pivot.TopLeft||pivot == UIWidget.Pivot.Top||pivot == UIWidget.Pivot.TopRight)
			v.y = 1f;
		
		return v;
	}
}