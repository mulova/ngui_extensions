using UnityEngine;
using System.Collections;
using mulova.comunity;
using mulova.ui;


namespace ngui.ex
{
	public class UISwitchOnTouch : MonoBehaviour
	{
		public UISwitch objSwitch;

		private void OnPress(bool pressed)
		{
			objSwitch.Set(pressed);
		}

		private void OnClick()
		{
			objSwitch.Set(false);
		}
	}
}
