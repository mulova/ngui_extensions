using UnityEngine;
using System.Collections;

namespace ngui.ex
{
	[RequireComponent(typeof(UIToggle))]
	public class UIToggleSprite : MonoBehaviour {
		public UISprite sprite;
		public string onSpriteName;
		public string offSpriteName;
		
		private UIToggle toggle;
		
		void Start () {
			toggle = GetComponent<UIToggle>();
			OnToggleChange();
			EventDelegate.Add(toggle.onChange, OnToggleChange);
		}
		
		private void OnToggleChange() {
			sprite.spriteName = toggle.value? onSpriteName: offSpriteName;
		}
	}
}
