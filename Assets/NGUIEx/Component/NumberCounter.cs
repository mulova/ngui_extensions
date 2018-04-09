//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Globalization;

using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using commons;
using comunity;


namespace ngui.ex
{
	public class NumberCounter : MonoBehaviour
	{
		public UILabel label;
		public int min = int.MinValue;
		public int max = int.MaxValue;
		public int begin;
		public int end;
		public float duration = 1;
		public int minDelta = 100;
		public Ease easeType = Ease.Linear;
		public bool forceSign;
		public MethodCall onEnd;
		
		private EaseFunction func;
		private float time;
		private int current;
		private int old;
		private float beginf;
		private float endf;
		private float speed;
		private int dir;

		void OnEnable()
		{
			Init();
		}

		public void Set(int number)
		{
			Begin(number, number);
		}

		public void Begin(int begin, int end)
		{
			if (enabled&&this.end == end)
				return;
			
			this.begin = Mathf.Clamp(begin, min, max);
			this.end = Mathf.Clamp(end, min, max);
			if (enabled)
			{
				Init();
			} else
			{
				enabled = true;
			}
		}

		public void Stop()
		{
			if (enabled)
			{
				current = end;
				UpdateLabel();
				enabled = false;
				if (onEnd != null)
				{
					onEnd.InvokeMethod();
				}
			}
		}

		public bool IsRunning()
		{
			return enabled;
		}

		public void Init()
		{
			dir = end >= begin? 1 : -1;
			old = begin-dir;
			current = begin;
			time = 0;
			beginf = begin;
			endf = end;
			speed = 1 / duration;
			
			float delta = Mathf.Abs(end-begin);
			if (delta < minDelta&&delta > 0)
			{
				speed *= minDelta / delta;
			}
			func = EaseManager.ToEaseFunction(easeType);
			if (label == null)
			{
				label = GetComponent<UILabel>();
			}
		}

		private void UpdateLabel()
		{
			label.SetText(StringUtil.ToCurrencyFormat(current, forceSign));
		}

		void Update()
		{
			if ((dir > 0&&current < end)
			             ||(dir < 0&&current > end))
			{
				
				time += RealTime.deltaTime * speed;
				time = Mathf.Min(time, 1);
				current = (int)MathUtil.Interpolate(func(time, 1, 0, 0), beginf, endf);
				if (current != old)
				{
					old = current;
					UpdateLabel();
				}
			} else
			{
				Stop();
			}
		}
	}
}
