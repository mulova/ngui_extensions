//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Ex;
using mulova.commons;
using mulova.comunity;
using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{
    /// <summary>
    /// Set Callback for UIButtons according to the UIButton.name
    /// </summary>
    public class ToggleHandler : LogBehaviour
	{
		public GameObject[] toggleObj = new GameObject[0];
		private MultiMap<UIToggle, Action<UIToggle>> callbackMap = new MultiMap<UIToggle, Action<UIToggle>>();
		// int param: buttonIndex,  string param: buttonName
		private Dictionary<string, UIToggle> toggleMap;
		
		void Awake()
		{
			InitToggles();
		}
		
		public IList<UIToggle> Selected
		{
			get
			{
				List<UIToggle> list = new List<UIToggle>();
				foreach (UIToggle t in toggleMap.Values)
				{
					if (t.value)
					{
						list.Add(t);
					}
				}
				return list;
			}
		}
		
		public UIToggle GetToggle(string key)
		{
			InitToggles();
			return toggleMap.Get(key);
		}
		
		public T GetToggleComponent<T>(string buttonName) where T:Component
		{
			return GetToggle(buttonName).GetComponentInChildren<T>();
		}
		
		public void SetSelected(string key)
		{
			foreach (KeyValuePair<string, UIToggle> t in toggleMap)
			{
				t.Value.value = t.Key == key;
			}
		}
		
		public void SetSelected(int index)
		{
			for (int i = 0; i < toggleObj.Length; ++i)
			{
				toggleObj[i].GetComponent<UIToggle>().value = i == index;
			}
		}
		
		public void AddCallback(object toggleId, Action<UIToggle> callback)
		{
			InitToggles();
			string buttonName = toggleId.ToText();
			UIToggle t = toggleMap.Get(buttonName);
			if (t == null)
			{
				log.Error("Toggle '{0}' doesn't exist", buttonName);
			} else
			{
				callbackMap.Add(t, callback);
			}
		}
		
		public void InitToggles()
		{
			if (toggleMap != null)
			{
				return;
			}
			toggleMap = new Dictionary<string, UIToggle>();
			foreach (GameObject o in toggleObj)
			{
                if (o != null)
                {
                    toggleMap[o.name] = o.GetComponent<UIToggle>();
                }
			}
			foreach (GameObject b in toggleObj)
			{
				if (b != null)
				{
					UIToggle t = b.FindComponent<UIToggle>();
					if (t.GetComponent<BoxCollider2D>() == null)
					{
						NGUITools.AddWidgetCollider(t.gameObject);
					}
					EventDelegateUtil.AddCallback(t.onChange, OnToggleChange, t);
				}
			}
		}
		
		public void Add(GameObject o)
		{
			if (!toggleObj.Contains(o))
			{
				toggleObj = toggleObj.Insert(toggleObj.Length, o);
			}
			InitToggles();
		}
		
		public void Remove(GameObject o)
		{
			toggleObj = toggleObj.Remove(o);
			UIToggle t = o.GetComponent<UIToggle>();
			if (t != null)
			{
				EventDelegateUtil.RemoveCallback<UIToggle>(t.onChange, this, OnToggleChange, t);
			}
		}
		
		[NoObfuscate]
		public void OnToggleChange(UIToggle o)
		{
			UIToggle t = o as UIToggle;
			if (t == null)
			{
				log.Warn("Invalid Parameter {0}({1})", o, o.GetType().FullName);
				return;
			}
			IEnumerable<Action<UIToggle>> callbackList = callbackMap[t];
			if (callbackList != null)
			{
				foreach (Action<UIToggle> callback in callbackList)
				{
					callback.Call(t);
				}
			} else
			{
				log.Error("No callback for {0}", o);
			}
		}
		
		public bool IsToggleOn(string name)
		{
			UIToggle t = toggleMap.Get(name);
			if (t != null)
			{
				return t.value;
			}
			return false;
		}
	}
	
}