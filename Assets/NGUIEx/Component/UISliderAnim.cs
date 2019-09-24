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
	[RequireComponent(typeof(UISlider))]
	public class UISliderAnim : MonoBehaviour
	{
		
		public float speed = 0.35f;
		public UISlider slider;
		public float beginValue;
		public float endValue;
		private float dir = 1;
		
		public Action OnAnimationEnd;

		void Start()
		{
			if (slider == null)
			{
				slider = GetComponent<UISlider>();
				slider.value = 0f;
			}
		}

		public void SetValue(float targetValue0)
		{
			SetValue(0, targetValue0);
		}

		public void SetValue(float begin, float end)
		{
			enabled = true;
			beginValue = begin;
			slider.value = begin;
			endValue = end;
			if (endValue > beginValue)
			{
				dir = 1;
			} else if (endValue == beginValue)
			{
				dir = 0;
			} else
			{
				dir = -1;
			}
		}

		private float GetProgress(float delta)
		{
			float tempValue = slider.value+delta * dir;
			if (IsSlideEnd(tempValue))
			{
				tempValue = endValue;
			}
			return tempValue;
		}

		public float GetValue()
		{
			return slider.value;
		}

		private bool IsSlideEnd(float sliderValue)
		{
			return dir >= 0? sliderValue >= endValue : sliderValue < endValue;
		}

		
		void Update()
		{
			float tempValue = Time.deltaTime * speed;
			slider.value = GetProgress(tempValue);
			
			if (IsSlideEnd(slider.value))
			{
				if (OnAnimationEnd != null)
				{
					OnAnimationEnd();
				}
				enabled = false;
			}
		}
	}
	
}