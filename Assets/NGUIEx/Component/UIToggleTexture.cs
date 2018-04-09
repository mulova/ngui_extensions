using UnityEngine;
using System.Collections;
using comunity;

namespace ngui.ex
{
	[RequireComponent(typeof(UIToggle))]
	public class UIToggleTexture : MonoBehaviour
	{
		public UITexture tex;
		public Texture on;
		public Texture off;
		public UIToggle toggle;
		public GameObject[] onObj;

		
		void Start()
		{
			if (toggle == null)
			{
				toggle = GetComponent<UIToggle>();
			}
			OnToggleChange();
			EventDelegate.Add(toggle.onChange, OnToggleChange);
		}

		private void OnToggleChange()
		{
			tex.mainTexture = toggle.value? on : off;
			if (onObj != null)
			{
				foreach (GameObject o in onObj)
				{
					o.SetActiveEx(toggle.value);
				}
			}
		}
	}
	
	
}