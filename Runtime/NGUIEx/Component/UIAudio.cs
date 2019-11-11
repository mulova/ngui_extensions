#if NGUI_AUDIO
//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using mulova.commons;
using audio;


namespace ngui.ex
{
	/// <summary>
	/// Attach UI at the corner of the target object
	/// </summary>
	[ExecuteAlways]
	public class UIAudio : MonoBehaviour
	{
		public string hoverOver;
		public string hoverOut;
		public string press;
		public string release;
		public string select;
		public string deselect;
		public string click;
		public string doubleClick;

		void OnHover(bool isOver)
		{
			if (isOver&&hoverOver.IsNotEmpty())
			{
                AudioBridge.Play(hoverOver);
			} else if (!isOver&&hoverOut.IsNotEmpty())
			{
                AudioBridge.Play(hoverOut);
			}
		}

		void OnPress(bool pressed)
		{
			if (pressed&&press.IsNotEmpty())
			{
                AudioBridge.Play(press);
			} else if (!pressed&&release.IsNotEmpty())
			{
                AudioBridge.Play(release);
			}
		}

		void OnSelect(bool selected)
		{
			if (selected&&select.IsNotEmpty())
			{
                AudioBridge.Play(select);
			} else if (!selected&&deselect.IsNotEmpty())
			{
                AudioBridge.Play(deselect);
			}
		}

		void OnClick()
		{
			if (click.IsNotEmpty())
			{
                AudioBridge.Play(click);
			}
		}

		void OnDoubleClick()
		{
			if (doubleClick.IsNotEmpty())
			{
                AudioBridge.Play(doubleClick);
			}
		}
	}

}
#endif