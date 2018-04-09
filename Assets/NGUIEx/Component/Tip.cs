//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;

namespace ngui.ex
{
	/// <summary>
	/// Change tip by time or user input
	/// </summary>
	public class Tip : MonoBehaviour
	{
		public float minTime = 5;
		public float maxTime = 20;
		public string[] tips = new string[0];
		public UILabel label;

		private float time;

		void Update()
		{
			time += RealTime.deltaTime;
			if (time >= maxTime)
			{
				ChangeTip();
			}
		}

		private void ChangeTip()
		{
			time = 0;
			if (tips.Length == 0)
			{
				return;
			}
			int i = Random.Range(0, tips.Length);
			label.SetText(tips[i]);
		}

		void OnClick()
		{
			if (time < minTime)
			{
				return;
			}
			ChangeTip();
		}
	}

}