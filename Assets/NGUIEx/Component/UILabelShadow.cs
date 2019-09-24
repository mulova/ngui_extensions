using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{
    [RequireComponent(typeof(UILabel))]
	public class UILabelShadow : MonoBehaviour {
		
		public int x = 5;
		public int y = -2;
		public Color shadowColor = Color.black;
		
		private GameObject shadow;
		
		void Start() {
			shadow = gameObject.InstantiateEx(transform.parent);
			shadow.GetComponent<UILabelShadow>().DestroyEx();
			UILabel shadowLabel = shadow.GetComponent<UILabel>();
			shadowLabel.depth--;
			shadowLabel.color = shadowColor;
			Transform t = shadow.transform;
			Vector3 pos = t.localPosition;
			pos.x += x;
			pos.y += y;
			t.localPosition = pos;
		}
	}
}