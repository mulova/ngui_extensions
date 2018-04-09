using UnityEngine;

namespace ngui.ex
{
	[ExecuteInEditMode]
	public class WidgetProperty : MonoBehaviour
	{
		public UIRect rect;
		private UIWidget widget;
		private UIRect[] allWidgets;
		private Color color;
		private Animation anim;
		private Animator animator;
		
		public void Initialize() {
			allWidgets = rect.GetComponentsInChildren<UIRect>(true);
		}
		
		void Start() {
			anim = GetComponent<Animation>();
			animator = GetComponent<Animator>();
			if (rect == null)
			{
				rect = GetComponent<UIRect>();
			}
			if (rect != null) {
				Initialize();
				if (rect is UIWidget)
				{
					widget = rect as UIWidget;
					color = widget.color;
				}
			}
		}
		
		void LateUpdate() {
			if (Application.isPlaying) {
				if (anim != null)
				{
					if (anim.isPlaying)
					{
						Invalidate();
					}
				} else if (animator != null)
				{
					if (animator.IsPlaying(0))
					{
						Invalidate();
					}
				} else if (widget != null)
				{
					if (color != widget.color) {
						Invalidate();
						color = widget.color;
					}
				}
			} else {
				Invalidate();
				foreach (UIRect r in allWidgets) {
					r.Update();
				}
			}
		}
		
		private void Invalidate() {
			foreach (UIRect r in allWidgets) {
				r.Invalidate(false);
			}
		}
	}
	
}