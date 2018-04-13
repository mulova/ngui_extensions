//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using comunity;

namespace ngui.ex
{
	public abstract class UILayout : MonoBehaviour
	{
		protected bool invalid = true;

		public delegate void RepositionListener();

		public event RepositionListener onReposition =  delegate { };

		public void InvalidateLayout()
		{
			if (!invalid)
			{ 
				invalid = true;
				foreach (UILayout l in GetComponents<UILayout>())
				{
					l.InvalidateLayout();
				}
				foreach (UIWidget w in GetComponentsInChildren<UIWidget>())
				{
					w.Invalidate(false);
				}
			}
		}

		abstract protected void DoLayout();

		public void Reposition()
		{
			bool optimize = true;
			if (enabled&&invalid)
			{
				transform.DepthFirstTraversal((t) =>
				{
					if (optimize)
					{
						// avoid GetComponents() call
						t.SendMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
						t.SendMessage("ValidateLayout", SendMessageOptions.DontRequireReceiver);
					} else
					{
						foreach (UIWidget w in t.GetComponents<UIWidget>())
						{
							if (w.enabled)
							{
								w.UpdateAnchors();
							}
						}
						UILayout[] layout = t.GetComponents<UILayout>();
						foreach (UILayout l in layout)
						{
							if (l != null&&l.enabled)
							{
								l.ValidateLayout();
							}
						}
					}
					return t.gameObject.activeInHierarchy;
				});
				onReposition();
			}
		}

		protected void ValidateLayout()
		{
			if (enabled&&invalid)
			{
				DoLayout();
				invalid = false;
			}
		}

		public Bounds GetBounds()
		{
			ValidateLayout();
			return NGUIMath.CalculateRelativeWidgetBounds(transform);
		}

		void Update()
		{
			UpdateImpl();
			if (invalid)
			{
				Reposition();
			}
		}

		abstract protected void UpdateImpl();

		public Bounds GetBounds(Transform root)
		{
			Bounds b = new Bounds();
			if (root == null||!root.gameObject.activeSelf)
			{
				return b;
			}
			foreach (UILayout l in root.GetComponentsInChildren<UILayout>(false))
			{
				l.Reposition();
			}
			return NGUIMath.CalculateRelativeWidgetBounds(root, false);
//			foreach (Transform t in root.GetComponentsInChildren<Transform>(false)) {
//				foreach (UIWidget widget in t.GetComponents<UIWidget>()) {
//					if (widget != null && widget.enabled) {
//						if (b.size == Vector3.zero) {
//							b = widget.CalculateBounds(root);
//						} else {
//							Bounds widgetBound = widget.CalculateBounds(root);
//							b.Encapsulate(widgetBound);
//						}
//					}
//				}
//			}
//			
//			return b;
		}
	}
	
}
