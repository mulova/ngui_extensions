using System;
using System.Ex;
using UnityEngine;

namespace ngui.ex
{
    [RequireComponent(typeof(UIProgressBar))]
	public class UIProgressBarAnim : MonoBehaviour
	{
		public float duration = 0.5f;
		private float max;
		private float begin;
		private float end;
		private float cur;
		private float delta;
		public UILabel label;
		private Action completeCallback;
		private UIProgressBar progress;

		public float value
		{
			set
			{
				Begin(value);
			}
		}

		public float EndValue
		{
			get { return end; }
		}

		public void Init(float begin, float max)
		{
			if (progress == null)
			{
				progress = GetComponent<UIProgressBar>();
			}
			this.max = max;
			this.begin = begin;
			this.cur = begin;
			Refresh();
		}

		public void Begin(float end, Action completeCallback = null)
		{
			this.begin = cur;
			this.end = end;
			if (begin != end)
			{
				this.cur = begin;
				this.delta = (end-begin) / duration;
				if (Mathf.Abs(delta) < 1 / 30f)
				{
					delta = Mathf.Sign(delta) * 1 / 30f;
				}
				this.completeCallback = completeCallback;
				enabled = true;
			} else
			{
				completeCallback.Call();
			}
			Refresh();
		}

		private void Refresh()
		{
			progress.SetValue(cur, max);
			if (label != null)
			{
				label.SetPlainNumber((int)cur);
			}
		}

		void Update()
		{
			if (delta == 0)
			{
				return;
			}
			cur += delta * RealTime.deltaTime;
			if ((cur >= end&&delta > 0)
			    ||(cur <= end&&delta < 0))
			{
				cur = end;
				Complete();
			}
			Refresh();
		}

		public bool Skip()
		{
			if (enabled)
			{
				cur = end;
				return true;
			} else
			{
				return false;
			}
		}

		private void Complete()
		{
			enabled = false;
			ActionEx.CallAfterRelease(ref completeCallback);
		}
	}
	
}