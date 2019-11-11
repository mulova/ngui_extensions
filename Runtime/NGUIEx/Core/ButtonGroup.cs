using System.Collections.Generic;
using UnityEngine;
using System;
using mulova.commons;
using System.Collections.Generic.Ex;
using System.Ex;

namespace ngui.ex
{
	/// <summary>
	/// Enable only one button in a group.
	/// Tab button callback is generated in realtime.
	/// </summary>
	public class ButtonGroup : MonoBehaviour {
		
		public const int ENABLED = 0;
		public const int DISABLED = 1;
		public UIButton[] buttons = new UIButton[0];
		private event Action<UIButton> callbacks;
		
		void OnEnable() {
			if (!Application.isPlaying || buttons.IsEmpty()) {
				return;
			}
			foreach (UIButton b in buttons) {
				EventDelegateUtil.AddCallback(b.onClick, OnButtonClick, b);
			}
			// Find the first enabled button
			UIButton selected = GetSelected();
			if (selected == null) {
				selected = buttons[0];
			}
			OnButtonClick(selected);
		}
		
		void OnDisable() {
			foreach (UIButton b in buttons) {
				EventDelegateUtil.RemoveCallback<UIButton>(b.onClick, this, OnButtonClick, b);
			}
		}
		
		public void AddCallback(Action<UIButton> callback) {
			this.callbacks += callback;
		}
		
		public void RemoveCallback(Action<UIButton> callback) {
			this.callbacks -= callback;
		}
		
		[NoObfuscate]
		public void OnButtonClick(UIButton param) {
			SetSelected(param);
			callbacks.Call(param);
		}
		
		public UIButton GetSelected() {
			foreach (UIButton b in buttons) {
				if (!b.isEnabled) {
					return b;
				}
			}
			return null;
		}
		
		public void SetSelected(UIButton btn) {
			foreach (UIButton b in buttons) {
				b.isEnabled = b != btn;
				b.GetComponents<UIButtonColor>().ForEach((bc)=>{
					bc.SetState(b != btn? UIButtonColor.State.Normal: UIButtonColor.State.Disabled, true);
				});
			}
		}
	}
	
}
