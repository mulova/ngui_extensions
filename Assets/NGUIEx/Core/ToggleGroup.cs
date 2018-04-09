using System.Collections.Generic;
using UnityEngine;
using System;

namespace ngui.ex
{
	public class ToggleGroup : MonoBehaviour {
		
		public UIToggle[] toggles = new UIToggle[0];
		public UIToggle selected;
		
		void OnEnable() {
			if (!Application.isPlaying) {
				return;
			}
			// Find the first enabled button
			foreach (UIToggle t in toggles) {
				if (selected == null && t.value) {
					selected = t;
				}
				bool enable = t == selected;
				t.value = enable;
				EventDelegateUtil.AddCallback(t.onChange, OnChange, t);
			}
			OnChange(selected);
		}
		
		void OnDisable() {
			foreach (UIToggle t in toggles) {
				EventDelegateUtil.RemoveCallback<UIToggle>(t.onChange, this, OnChange, t);
			}
		}
		
		public void OnChange(UIToggle changed) {
			if (changed.value) {
				this.selected = changed;
			}
			foreach (UIToggle t in toggles) {
				t.value = t == selected;
				t.enabled = t != selected;
			}
		}
		
		public UIToggle GetSelected() {
			return selected;
		}
	}
}
