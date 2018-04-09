//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using commons;
using comunity;

namespace ngui.ex
{
	public class NetThrobber : comunity.Script
	{
        
		public static readonly Color DP = Color.green;
		public GameObject ui;
		public UIWidget widget;
        public Animator anim;
		private List<object> keys = new List<object>();
		public const string BLOCK_NET = "block.net";

		private void OnBlock(object data)
		{
			BlockInfo b = (BlockInfo)data;
			if (b.key == null)
			{
				keys.Clear();
				ui.SetActive(false);
			} else if (b.visible)
			{
				ShowBlocker(b.key, b.color);
			} else
			{
                StartCoroutine(HideBlocker(b.key));
			}
		}

		void OnEnable()
		{
			EventRegistry.RegisterListener(BLOCK_NET, OnBlock);
		}

		void OnDisable()
		{
			EventRegistry.DeregisterListener(BLOCK_NET, OnBlock);
		}

		private void ShowBlocker(object key, Color c)
		{
			log.Debug("NetSync +{0}", key);
			bool veryFirst = keys.Count == 0;
			keys.Add(key);
			ui.SetActive(true);
			widget.color = c;
			if (anim != null && veryFirst)
			{
                anim.Play("net_throbber_delay");
			}
		}

		private IEnumerator HideBlocker(object key)
		{
            yield return new WaitForEndOfFrame();

			log.Debug("NetSync -{0}", key);
			keys.Remove(key);
			if (keys.IsEmpty())
			{
                if (anim != null)
                {
#if !UNITY_5_6_OR_NEWER
                    anim.Stop();
#endif
                }
				ui.SetActive(false);
				log.Info("NetSync Off");
			}
		}

		public bool IsVisible()
		{
			return ui.activeInHierarchy;
		}

		public static void SetVisible(object key, bool visible)
		{
			if (visible)
			{
				Show(key);
			} else
			{
				Hide(key);
			}
		}

		public static void Show(object k)
		{
			Show(k, Color.white);
		}

		public static void ShowDP(object k)
		{
			EventRegistry.SendEvent(BLOCK_NET, new BlockInfo() { key = k, visible = true, color = DP });
		}

		public static void Show(object k, Color c)
		{
			EventRegistry.SendEvent(BLOCK_NET, new BlockInfo() { key = k, visible = true, color = c });
		}

		public static void Hide(object k)
		{
			EventRegistry.SendEvent(BLOCK_NET, new BlockInfo() { key = k, visible = false });
		}

		public static void Clear()
		{
			EventRegistry.SendEvent(BLOCK_NET, new BlockInfo() { key = null, visible = false });
		}

		struct BlockInfo
		{
			public object key;
			public bool visible;
			public Color color;
		}
	}
}
