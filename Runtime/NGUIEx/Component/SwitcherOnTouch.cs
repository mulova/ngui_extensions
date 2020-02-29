using UnityEngine;
using mulova.switcher;

namespace ngui.ex
{
	public class SwitcherOnTouch : MonoBehaviour
	{
		public Switcher objSwitch;

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
