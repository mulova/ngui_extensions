using UnityEngine;
using System.Collections;
using System;

namespace ngui.ex
{
	[RequireComponent(typeof(UITexture))]
	public class UIDragToggleTexture : MonoBehaviour {
		private UITexture tex;
		public bool toggleByTouch = true;
		public Texture2D on;
		public Texture2D off;
		
		public Action<bool> onChange;
		public Action<bool, Action<bool>> handler;
		private float dragX;
		
		private bool IsOn() {
			return Tex.mainTexture == on;
		}
		
		/// <summary>
		/// no event dispatch
		/// </summary>
		/// <param name="b">If set to <c>true</c> b.</param>
		public void Init(bool b) {
			Tex.mainTexture = b? on: off;
		}
		
		public bool value 
		{
			get { return IsOn(); }
			set {
				if (!enabled || IsOn() == value) {
					return;
				}
				if (handler != null) {
					handler(value, b=> {
						SetValue(b);
					});
				} else {
					SetValue(value);
				}
			}
		}
		
		private void SetValue(bool b) {
			Tex.mainTexture = b? on: off;
			if (onChange != null) {
				onChange(b);
			}
		}
		
		public UITexture Tex {
			get {
				if (tex == null) {
					tex = GetComponent<UITexture>();
				}
				return tex;
			}
		}
		
		void OnClick() {
			if (toggleByTouch) {
				value = !value;
			}
		}
		
		void OnDragStart () {
			dragX = 0;
		}
		
		void OnDrag(Vector2 delta) {
			dragX += delta.x;
		}
		
		void OnDragOut(GameObject draggedObject) {
			if (dragX < -20 && value) {
				value = false;
			}
			else if (dragX > 20 && !value) {
				value = true;
			}
		}
	}
	
}