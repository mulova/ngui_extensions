//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using commons;

namespace ngui.ex
{
	/**
	 * Resize Background SpeechBubble texture according to the text size
	 */
	[ExecuteInEditMode]
	public class SpeechBubble : MonoBehaviour
	{
		public UILabel label;
		public UIWidget bubble;
		public Vector2 borderSize;
		public TextTypingAnim anim;
		
		private int lineCount;

		void OnEnable()
		{
			SetText(label.text);
		}

		public void SetText(string text)
		{
			if (!text.IsEmpty())
			{
				label.SetText(text);
				UpdateBubbleSize();
				if (anim != null&&Application.isPlaying)
				{
					anim.Begin();
					anim.onTyping = OnTyping;
				}
			} else
			{
				if (anim != null)
				{
					anim.Clear();
				}
			}
		}

		private void UpdateBubbleSize()
		{
			if (bubble == null)
			{
				return;
			}
			Vector2 size = NGUIUtil.GetSize(label)+borderSize;
			bubble.width = (int)Mathf.Max(size.x, label.width+borderSize.x);
			bubble.height = (int)size.y;
		}

		void OnTyping(UILabel label)
		{
			UpdateBubbleSize();
		}
	}
	
}