using UnityEngine;

namespace ngui.ex
{
	[RequireComponent(typeof(UISprite))]
	public class UIGraySprite : MonoBehaviour {
		
		void Start() {
			GetComponent<UISprite>().ToGrayscale();
		}
	}
	
}