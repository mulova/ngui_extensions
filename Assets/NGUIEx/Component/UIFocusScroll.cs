using UnityEngine;
using System.Collections;
using ngui.ex;
using System;
using comunity;
using commons;

namespace ngui.ex
{
	public class UIFocusScroll : MonoBehaviour
	{
		public Transform restrictObj;
		public Transform focusCenter;
		public Transform coverRadius;
		
		[Range(0.0001f, float.MaxValue)]
		public float outsideObjScale = 1.0f;
		[Range(0.0001f, float.MaxValue)]
		public float centerObjScale = 1.5f;
		
		private Action<Transform> focusCallback;
		private UIScrollView scroll;
		private UITableLayout grid;
		private float scalingCoverage;

		private void OnEnable()
		{
			if (scroll == null)
			{
				return;
			}
			FocusTarget();
			scroll.onDragFinished += FocusTarget;
		}

		private void OnDisable()
		{
			if (scroll == null)
			{
				return;
			}
			scroll.onDragFinished -= FocusTarget;
		}

		public void InitFocusing(UIScrollView focusScroll, UITableLayout grid, Action<Transform> focusCallback = null)
		{
			scalingCoverage = Vector3.Distance(focusCenter.position, coverRadius.position);
			if (focusScroll == null)
			{
				return;
			}
			if (scroll != null)
			{
				scroll.onDragFinished -= FocusTarget;
			}
			
			this.grid = grid;
			this.scroll = focusScroll;
			this.focusCallback = focusCallback;
			
			if (scroll.isActiveAndEnabled)
			{
				FocusTarget();
				scroll.onDragFinished += FocusTarget;
			}
			
			if (null == restrictObj)
			{
				CreateRestrictObject();
			}
		}

		private void CreateRestrictObject()
		{
			// This func for only scroll that focus center
			GameObject go = gameObject.CreateChild("restrictObj");
			restrictObj = go.transform;
			UIWidget widget = go.GetComponentEx<UIWidget>();
			UIPanel panel = scroll.GetComponent<UIPanel>();
			restrictObj.position = focusCenter.position;
			if (scroll.movement == UIScrollView.Movement.Horizontal)
			{
				widget.width = (int)panel.width;
			} else if (scroll.movement == UIScrollView.Movement.Vertical)
			{
				widget.height = (int)panel.height;
			} else
			{
				Debug.LogWarning("This Component not support other movement option");
			}
		}

		private Transform FindTarget()
		{
			if (grid == null||grid.isEmpty)
			{
				return null;
			}
            UITableCell[] comps = grid.components;
            Transform target = null;
			float shortestDis = float.MaxValue;
			// Find obj that placed in most nearby
			for (int idx = 0; idx < comps.Length; ++idx)
			{
				float dis = Vector3.Distance(focusCenter.position, comps[idx].trans.position);
				if (shortestDis > dis)
				{
                    target = comps[idx].trans;
					shortestDis = dis;
				}
			}
            return target;
		}

		public void FocusTarget()
		{
			Transform target = FindTarget();
			FocusTarget(target);
		}

		public void FocusTarget(Transform target)
		{
			if (target == null)
			{
				return;
			}
			bool targetChanged = (restrictObj.position != target.position);
			restrictObj.position = target.position;
			if (targetChanged)
			{
				scroll.ResetPosition();
				focusCallback.Call(target);
			}
		}

		private void Update()
		{
			if (grid == null||grid.isEmpty)
			{
				return;
			}
            UITableCell[] comps = grid.components;
			for (int idx = 0; idx < comps.Length; ++idx)
			{
				if (comps[idx] == null)
				{
					continue;
				}
				float dis = Vector3.Distance(focusCenter.position, comps[idx].trans.position);
				float scale = outsideObjScale;
				if (dis < scalingCoverage)
				{
					// Add inversely propotional scale to distance
					float scalePer = (dis == 0? 1 : 1-(dis / scalingCoverage));
					scale += ((centerObjScale-outsideObjScale) * scalePer);
				}
				comps[idx].trans.localScale = new Vector3(scale, scale);
			}
		}
	}
}
