using UnityEngine;

namespace ngui.ex
{
	[RequireComponent(typeof(UIPanel))]
	[ExecuteAlways]
	public class UIPanelResizeClipping : MonoBehaviour
	{
		public UIDrawCall.Clipping editorClippingType = UIDrawCall.Clipping.ConstrainButDontClip;
		private UIPanel panel;
		void Awake() {
			panel = GetComponent<UIPanel>();
			Refresh();
		}
		
		#if UNITY_EDITOR
		void Update() {
		if (!Application.isPlaying) {
		Refresh();
		}
		}
		#endif
		
		private void Refresh() {
			if (!Application.isPlaying) {
				panel.clipping = editorClippingType;
			} else {
				panel.clipping = UIDrawCall.Clipping.SoftClip;
			}
			Vector4 clipRegion = panel.baseClipRegion;
			Vector2 screenSize = GetWindowSize(); 
			clipRegion.z = screenSize.x+2;
			clipRegion.w = screenSize.y+2;
			panel.baseClipRegion = clipRegion;
		}
		
		private Vector2 GetWindowSize ()
		{
			UIRoot rt = panel.root;
			Vector2 size = NGUITools.screenSize;
			if (rt != null) size *= rt.GetPixelSizeAdjustment(Mathf.RoundToInt(size.y));
			if (size.x > 720) {
				size.x = 720;
			}
			return size;
		}
	}
	
}