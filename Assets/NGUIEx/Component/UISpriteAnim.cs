//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;

namespace ngui.ex
{
	public class UISpriteAnim : MonoBehaviour
	{
		
		public UISprite sprite;
        public SpriteAnimInfo[] anim = new SpriteAnimInfo[0];
		public bool loop;
		public bool pixelPerfect;
		public float initialDelay;
		
		private float time0;
		private float time;
		private int index;
		
		private Action endFunc;

		public void Begin(Action endFunc)
		{
			this.endFunc = endFunc;
			sprite.enabled = true;
			enabled = true;
			if (anim.Length > 0)
			{
				sprite.name = anim[0].name;
			}
		}

		void OnEnable()
		{
			if (sprite == null||anim == null||anim.Length == 0)
			{
				enabled = false;
			}
			index = 0;
			time = 0;
			time0 = initialDelay;
			if (anim.Length > 0)
			{
				SetSprite();
			}
		}

		void Update()
		{
			if (time0 > 0)
			{
				time0 -= Time.deltaTime;
			}
			if (time0 > 0)
			{
				return;
			}
			time += Time.deltaTime;
			if (time >= anim[index].delay)
			{
				time -= anim[index].delay;
				NextSprite();
			}
		}

		private void SetSprite()
		{
			sprite.spriteName = anim[index].name;
		}

		private void NextSprite()
		{
			index++;
			if (index >= Count)
			{
				if (loop)
				{
					index = 0;
				} else
				{
					index--;
					enabled = false;
					if (endFunc != null)
					{
						endFunc();
						endFunc = null;
					}
				}
			}
			SetSprite();
			if (pixelPerfect)
			{
				sprite.MakePixelPerfect();
			}
		}

		public int Count
		{
			get
			{
				return anim.Length;
			}
		}
	}

	[System.Serializable]
	public class SpriteAnimInfo : ICloneable
	{
		public string name;
		public float delay = 0.3f;

		public SpriteAnimInfo()
		{
		}

		public SpriteAnimInfo(string name, float delay)
		{
			this.name = name;
			this.delay = delay;
		}

		public object Clone()
		{
			return new SpriteAnimInfo(name, delay);
		}
	}
}