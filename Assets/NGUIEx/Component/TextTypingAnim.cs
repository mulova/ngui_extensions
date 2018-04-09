//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using System;
using commons;

namespace ngui.ex
{
	public class TextTypingAnim : MonoBehaviour
	{
		public UILabel label;
		public float speed = 20;
		public float endDelay = 1;
		public bool useEndDelay = true;
		private float endTimer;
		private string finalText = "";
		private float time;
		private int textLength = 0;
		private const char ENCODING_BEGIN = '[';
		private const char ENCODING_END = ']';

		public delegate void OnTyping(UILabel label);

		public OnTyping onTyping;
		private Action callback;

		public void SetText(string text, Action callback = null)
		{
			if (!text.IsEmpty())
			{
				label.SetText(text);
				Begin(callback);
			} else
			{
				gameObject.SetActive(false);
				label.SetText(null);
			}
		}

		public void Begin(Action callback = null)
		{
			Init();
			gameObject.SetActive(true);
			enabled = true;
			this.callback = callback;
		}

		public void Skip()
		{
			if (textLength < finalText.Length-1)
			{
				textLength = finalText.Length-1;
				time = 0;
				endTimer = 0;
			} else
			{
				if (useEndDelay)
				{
					endTimer = endDelay;
				} else
				{
					enabled = false;
					callback.Call();
				}
			}
		}

		public void Clear()
		{
			label.SetText(null);
			enabled = false;
		}

		void OnEnable()
		{
			Init();
		}

		private void Init()
		{
			if (label != null&&label.text.IsNotEmpty())
			{
				finalText = label.text;
				time = 0;
				endTimer = 0;
                label.Clear();
				textLength = 0;
			}
			if (speed == 0)
			{
				textLength = finalText.Length;
				label.SetText(finalText);
			}
		}

		void Update()
		{
			if (textLength >= finalText.Length)
			{
				endTimer += Time.unscaledDeltaTime;
				if (useEndDelay&&endTimer >= endDelay)
				{
					enabled = false;
					callback.Call();
				}
			} else
			{
				time += Time.unscaledDeltaTime * speed;
				if (time >= 1)
				{
					time = 0;

					//Check for encoding option - moure
					if (finalText[textLength] == ENCODING_BEGIN)
					{
						while (++textLength < finalText.Length)
						{
							if (finalText[textLength] == ENCODING_END)
							{
								textLength++;
								break;
							}
						}
					} else
					{
						textLength++;
					}

					label.SetText(finalText.Substring(0, textLength));
					if (onTyping != null)
					{
						onTyping(label);
					}
				}
			}
		}
	}
}
