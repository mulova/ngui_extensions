using UnityEngine;

namespace ngui.ex
{
	[RequireComponent(typeof(UITexture))]
	public class UIGrayTexture : MonoBehaviour {
		
		void Start() {
			GetComponent<UITexture> ().ToGrayscale ();
		}
	}
	
}