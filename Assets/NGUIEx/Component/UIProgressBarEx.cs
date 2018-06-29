//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Text;

namespace ngui.ex
{
	public static class UIProgressBarEx
	{
		
		public static void SetValue(this UIProgressBar bar, int val, int max)
		{
			if (bar == null)
			{
				return;
			}
			bar.value = val / (float)max;
		}
		
		public static void SetValue(this UIProgressBar bar, float val, float max)
		{
			if (bar == null)
			{
				return;
			}
			bar.value = val / max;
		}
		
	}
}
