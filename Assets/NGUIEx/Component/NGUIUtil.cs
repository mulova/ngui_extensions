//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Text;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{
    public static class NGUIUtil
	{
		/// <summary>
		/// if button is started as alpha 0, it doesn't work as expected. So Init DefaultButtonColors
		/// </summary>
		/// <param name="root">Root.</param>
		public static void InitDefaultButtonColors(Transform root)
		{
			foreach (UIButtonColor c in root.GetComponentsInChildren<UIButtonColor>(true))
			{
				c.SetState(c.state, true);
			}
		}

		public static void SetBoundingBox(Transform trans, Rect bound)
		{
			UIPanel panel = trans.GetComponent<UIPanel>();
			if (panel != null)
			{
				if (!Platform.isEditor||!Equals(panel.baseClipRegion, bound, 0.01f))
				{ /*  Editor.setDirty()를 막기 위해 동일한 값인지 확인 */
					panel.baseClipRegion = new Vector4(bound.x, bound.y, bound.width, bound.height);
				}
			} else
			{
				UIWidget rect = trans.GetComponent<UIWidget>();
				if (rect != null)
				{
					rect.width = (int)bound.width;
					rect.height = (int)bound.height;
					rect.pivot = UIWidget.Pivot.Center;
					trans.localPosition = Vector3.zero;
					Vector3 pos = trans.localPosition;
					pos.x = bound.center.x;
					pos.y = -bound.center.y;
					trans.SetLocalPosition(pos, 0.01f);
				}
//				Vector3 size = new Vector3(bound.width, bound.height, 1);
//				trans.SetLocalScale(size, 0.01f);
			}
		}

		private static bool Equals(Vector4 b1, Rect b2, float tolerance)
		{
			return b1.x.Equals(b2.x, tolerance)
			&&b1.y.Equals(b2.y, tolerance)
			&&b1.z.Equals(b2.width, tolerance)
			&&b1.w.Equals(b2.height, tolerance);
		}

		private static bool Equals(Vector4 b1, Bounds b2, float tolerance)
		{
			Vector3 min = b2.min;
			Vector3 size = b2.size;
			return b1.x.Equals(min.x, tolerance)
			&&b1.y.Equals(min.y, tolerance)
			&&b1.z.Equals(size.x, tolerance)
			&&b1.w.Equals(size.y, tolerance);
		}

		public static Vector2 GetSize(UIWidget widget)
		{
			return Vector2.Scale(new Vector2(widget.width, widget.height), new Vector2(widget.transform.localScale.x, widget.transform.localScale.y));
		}

		public static void UpdateAnchors(Transform root)
		{
			root.DepthFirstTraversal((t) =>
			{ 
				foreach (UIRect r in t.GetComponents<UIRect>())
				{
					r.Invalidate(false);
//					r.Update();
//					r.UpdateAnchors();
				}
				return true;
			});
		}

		/**
		 * Invalidate all the layout components upward.
		 * Reposition is actually done by later Update()
		 */
		public static void Reposition(Transform t)
		{
			bool optimize = true;
			while (t != null)
			{
				if (optimize)
				{
					t.SendMessage("InvalidateLayout", SendMessageOptions.DontRequireReceiver);
				} else
				{
					foreach (UILayout l in t.GetComponents<UILayout>())
					{
						l.InvalidateLayout();
					}
				}
				t = t.parent;
			}
		}

		public static void RepositionNow(Transform trans)
		{
			UILayout layout = trans.GetComponent<UILayout>();
			/*  Grid일 경우 Grid에 추가된 순서대로 update한다. */
			if (layout is UITableLayout)
			{
				UITableLayout grid = (UITableLayout)layout;
				foreach (var child in grid.components)
				{
					if (child == null)
					{
						continue;
					}
					if (child.gameObject.activeSelf)
					{
                        RepositionNow(child.transform);
					}
				}
			} else
			{
				for (int i = 0; i < trans.childCount; i++)
				{
					Transform child = trans.GetChild(i);
					if (child.gameObject.activeSelf)
					{
						RepositionNow(child);
					}
				}
			}
			if (layout != null&&layout.enabled)
			{
				layout.Reposition();
			}
		}

		public static Transform InstantiateWidget(Transform parent, GameObject prefab)
		{
			Transform instance = Object.Instantiate(prefab).transform;
			parent.SetParent(instance, false);
			instance.gameObject.SetActive(true);
			return instance;
		}

		public static void DisableAnchor(Transform t)
		{
			if (t != null)
			{
				/*  anchor가 있으면 좌표가 어긋나므로 disable */
				foreach (UIAnchor a in t.GetComponentsInChildren<UIAnchor>(true))
				{
					a.enabled = false;
				}
			}
		}

		public static bool IsValid(Rect bound)
		{
			return bound.width != 0&&bound.height != 0;
		}

		/**
		 * Pivot 값을 LocalTranslation에 적용한다.
		 */
		public static void ApplyPivot(Transform t)
		{
			//if (Platform.UI_TEST) {
			//		UIPivot pivot = t.GetComponent<UIPivot>();
			//		if (pivot != null) {
			//			Vector3 pos = t.localPosition;
			//			pos -= pivot.GetPivotPos();
			//			t.SetLocalPosition(pos, 0.01f);
			//		}
			//}
		}

		public static string ConvertColor2Str(Color color)
		{
			StringBuilder colorStr = new StringBuilder(9);
			colorStr.Append('[');
			colorStr.Append(NGUIMath.DecimalToHex24(NGUIMath.ColorToInt(color)));
			colorStr.Append(']');
			return colorStr.ToString();
		}

		public static BoxCollider UpdateCollider(Transform trans)
		{
			return UpdateCollider(trans.GetComponent<BoxCollider>());
		}

		public static BoxCollider UpdateCollider(BoxCollider box)
		{
			if (box != null)
			{
				Bounds b = NGUIMath.CalculateRelativeWidgetBounds(box.transform);
				box.center = b.center;
				box.size = new Vector3(b.size.x, b.size.y, 1);
			}
			return box;
		}

		public static Bounds CalculateAbsoluteWidgetBounds(Transform trans, UIWidget[] widgets)
		{
			if (trans != null&&widgets != null)
			{
				if (widgets.Length == 0)
					return new Bounds(trans.position, Vector3.zero);
				
				Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
				
				for (int i = 0, imax = widgets.Length; i < imax; ++i)
				{
					UIWidget w = widgets[i];
					if (!w.enabled)
						continue;
					
					Vector3[] corners = w.worldCorners;
					
					for (int j = 0; j < 4; ++j)
					{
						vMax = Vector3.Max(corners[j], vMax);
						vMin = Vector3.Min(corners[j], vMin);
					}
				}
				
				Bounds b = new Bounds(vMin, Vector3.zero);
				b.Encapsulate(vMax);
				return b;
			}
			return new Bounds(trans.position, Vector3.zero);
		}

		public static Bounds CalculateAbsoluteWidgetBounds(Transform trans, UIWidget widget)
		{
			if (trans != null&&widget != null)
			{
				Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

				Vector3[] corners = widget.worldCorners;

				for (int j = 0; j < 4; ++j)
				{
					vMax = Vector3.Max(corners[j], vMax);
					vMin = Vector3.Min(corners[j], vMin);
				}

				Bounds b = new Bounds(vMin, Vector3.zero);
				b.Encapsulate(vMax);
				return b;
			}
			return new Bounds(trans.position, Vector3.zero);
		}
	}
}
