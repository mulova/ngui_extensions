using System.Collections.Generic;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

using UnityEngine.Assertions;
using effect;
using commons;
using comunity;
using Assert = commons.Assert;

namespace ngui.ex
{
	public static class NGUIEx
	{
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

