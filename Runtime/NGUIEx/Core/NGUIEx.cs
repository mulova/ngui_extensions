using System;
using System.Ex;
using mulova.effect;
using UnityEngine;
using UnityEngine.Ex;
using Assert = mulova.commons.Assert;

namespace ngui.ex
{
    public static class NGUIEx
	{
        public static NGUIAtlas origin(this INGUIAtlas atlas)
        {
            switch (atlas)
            {
                case NGUIAtlas a:
                    return a;
                case UIAtlas at:
                    return at.replacement.origin();
                default:
                    return null;
            }
        }

        public static string name(this INGUIAtlas atlas)
        {
            switch (atlas)
            {
                case NGUIAtlas a:
                    return a.name;
                case UIAtlas at:
                    return at.name;
                default:
                    return atlas.ToString();
            }
        }

        public static void SetCallback(this UIToggle button, EventDelegate.Callback callback)
		{
			EventDelegate.Set(button.onChange, callback);
		}
		
		public static void AddCallback(this UIToggle button, EventDelegate.Callback callback)
		{
			EventDelegate.Add(button.onChange, callback);
		}
		
		public static void SetCallback(this UIEventTrigger button, EventDelegate.Callback callback)
		{
			EventDelegate.Set(button.onClick, callback);
		}
		
		public static void AddCallback(this UIEventTrigger button, EventDelegate.Callback callback)
		{
			EventDelegate.Add(button.onClick, callback);
		}
		
		public static void PlayUI(this ParticleLoader loader, Action<ParticleControl> loadCallback = null, Action<ParticleControl> endCallback = null)
		{
			PlayUI(loader, 1, loadCallback, endCallback);
		}
		
		public static void PlayUI(this ParticleLoader loader, Renderer r, int deltaRQ, Action<ParticleControl> loadCallback = null, Action<ParticleControl> endCallback = null)
		{
			if (loader == null)
			{
				return;
			}
			UIPanel p = loader.GetComponentInParent<UIPanel>();
			if (p != null)
			{
				loader.Play(particle =>
					{
						if (particle != null)
						{
							particle.SetRqDelta(r, deltaRQ);
						} else
						{
							Assert.IsNotNull(particle);
						}
						loadCallback.Call(particle);
					}, endCallback);
			} else
			{
				loader.Play(loadCallback, endCallback);
			}
		}
		
		public static void SetUiLayer(this ParticleControl particle, int deltaRQ)
		{
			UIPanel p = particle.GetComponentInParent<UIPanel>();
			if (p != null)
			{
				particle.gameObject.SetLayer(p.gameObject.layer);
				particle.SetRenderLayer(p.sortingLayerName, p.sortingOrder+deltaRQ);
				int rq = deltaRQ >= 0? p.GetMaxRenderQueue() : p.GetMinRenderQueue();
				particle.SetRenderQueue(rq+deltaRQ);
			}
		}
		
		public static void PlayUI(this ParticleControl particle, int deltaRQ, Action<ParticleControl> callback = null)
		{
			if (particle == null)
			{
				return;
			}
			particle.SetUiLayer(deltaRQ);
			particle.Play(callback);
		}
		
		public static void PlayUI(this ParticleLoader loader, int deltaRQ, Action<ParticleControl> loadCallback = null, Action<ParticleControl> callback = null)
		{
			if (loader == null)
			{
				return;
			}
			loader.Play(particle =>
				{
					loadCallback.Call(particle);
					particle.PlayUI(deltaRQ, callback);
					
				}, null);
		}
	}
}

