using UnityEngine;

namespace ngui.ex
{
    [RequireComponent(typeof(UIButton))]
    public class ButtonActivateTimer : MonoBehaviour
    {
        public float duration = 2;
        private float timer;
        
        void OnClick() {
            timer = duration;
            GetComponent<UIButton>().isEnabled = false;
        }
        
        void Update() {
            if (timer >= 0) {
                timer -= Time.deltaTime;
                if (timer < 0) {
                    GetComponent<UIButton> ().isEnabled = true;
                }
            }
        }
    }
    
}