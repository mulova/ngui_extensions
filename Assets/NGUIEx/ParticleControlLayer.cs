using System;
using UnityEngine;
using effect;


namespace ngui.ex
{
	[RequireComponent(typeof(ParticleControl))]
	public class ParticleControlLayer : MonoBehaviour
	{
		private ParticleControl e;
		
		void OnEnable() {
			//		EventRegistry.RegisterListener(EventId.DEPTH_CHANGED, DepthChanged);
		}
		
		void OnDisable() {
			//		EventRegistry.DeregisterListener(EventId.DEPTH_CHANGED, DepthChanged);
		}
		
		void OnStart() {
			DepthChanged();
		}
		
		private void DepthChanged() {
			e = GetComponent<ParticleControl>();
			UIPanel panel = GetComponentInParent<UIPanel>();
			if (panel != null) {
				e.SetRenderLayer(panel.sortingLayerName, panel.sortingOrder +1);
				e.SetRenderQueue(panel.GetMaxRenderQueue());
			}
		}
	}
	
}