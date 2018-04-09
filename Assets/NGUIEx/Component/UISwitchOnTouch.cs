using UnityEngine;
using System.Collections;
using comunity;


namespace ngui.ex
{
	public class UISwitchOnTouch : MonoBehaviour
	{
		public ObjSwitch objSwitch;

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
