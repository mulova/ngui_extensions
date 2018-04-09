//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------
using UnityEngine;
using ngui.ex;
using commons;
using System;
using comunity;

namespace ngui.ex
{
	public class UITabHandler : MonoBehaviour
	{
		
		public UITab[] tabs = new UITab[0];
		public UITab[] tabPrefabs = new UITab[0];
		public UIButton[] tabButtons = new UIButton[0];
		private MonoBehaviour container;
		
		public bool initialized { get; private set; }
		
		private UITab currentTab;
		
		private event Action<UITab> tabCallback;
		
		private Action<UITab, Action> checkTabMove;
		
		public void Init(MonoBehaviour container)
		{
			this.container = container;
			// resize tabs if not initialized
			if (tabs.Length < tabPrefabs.Length)
			{
				UITab[] newTabs = new UITab[tabPrefabs.Length];
				Array.Copy(tabs, newTabs, tabs.Length);
				tabs = newTabs;
			}
			for (int i=0; i<tabs.Length; ++i)
			{
				if (tabs[i] != null)
				{
					UITab t = GetTab(i);
					t.tabButton.SetCallback(OnClickTab, t);
					if (currentTab == null)
					{
						currentTab = t;
					}
				} else
				{
					tabButtons[i].SetCallback(CreateTab, tabPrefabs[i]);
				}
			}
			initialized = true;
		}
		
		public void AddCallback(Action<UITab> callback)
		{
			tabCallback += callback;
		}
		
		public void RemoveCallback(Action<UITab> callback)
		{
			tabCallback -= callback;
		}
		
		public void SetTabMoveCallback(Action<UITab, Action> callback)
		{
			checkTabMove = callback;
		}
		
		public UITab GetActiveTab()
		{
			return currentTab;
		}
		
		public T GetTab<T>() where T: UITab
		{
			foreach (UITab t in tabs)
			{
				if (t.GetType() == typeof(T))
				{
					return t as T;
				}
			}
			return null;
		}
		
		private UIWindow popupWindow
		{
			get
			{
				return container is PopupBase? (container as PopupBase).window: null;
			}
		}
		
		public UITab GetTab(int i)
		{
			UITab t = null;
			if (i < tabs.Length && tabs[i] != null)
			{
				t = tabs[i];
			} else if (i < tabPrefabs.Length)
			{
				Transform parent = container is PopupBase? (container as PopupBase).window.ui.transform: transform;
				t = tabs[i] = tabPrefabs[i].InstantiateEx(parent);
				t.tabButton = tabButtons[i];
				t.tabButton.SetCallback(OnClickTab, t);
				UIWindow win = popupWindow;
				if (win != null)
				{
					foreach (UIPanel p in t.uiRoot.GetComponentsInChildren<UIPanel>(true))
					{
						p.SetLayerOver(win.Panel);
					}
				}
			}
			if (!t.isInitialized)
			{
				t.Init(container);
				t.tabButton.AddCallback(new Action<UITab>(OnClickTab), t);
				t.uiRoot.SetActive(false);
			}
			return t;
		}
		
		public T GetTab<T>(string tabName) where T: UITab
		{
			return tabs.Filter(t => t!=null && t.name == tabName) as T;
		}
		
		[NoObfuscate]
		public void OnClickTab(UITab tab)
		{
			if (checkTabMove != null)
			{
				checkTabMove(tab, () => {
					_OnClickTab(tab);
				});
			} else
			{
				_OnClickTab(tab);
			}
		}
		
		public void CreateTab(UITab tab)
		{
			int index = tabPrefabs.FindIndex(tab);
			OnClickTab(GetTab(index));
		}
		
		public void _OnClickTab(UITab tab)
		{
			currentTab = tab;
			if (!initialized)
			{
				return;
			}
			foreach (UITab t in tabs)
			{
				if (t != null && t != currentTab)
				{
					t.SetVisible(false);
				}
			}
			currentTab.SetVisible(true);
			tabCallback.Call(currentTab);
		}
		
		public void SetActiveTab(int index)
		{
			UITab t = GetTab(index);
			if (t != null)
			{
				OnClickTab(t);
			} else
			{
				UITab.log.Error("Tab index out of range {0} >= {1}", index, tabs.Length);
			}
		}
		
		public void SetActiveTab(string name)
		{
			for (int i=0; i<tabs.Length; ++i)
			{
				if (tabs[i] != null && tabs[i].name == name)
				{
					OnClickTab(tabs[i]);
					return;
				}
			}
			for (int i=0; i<tabPrefabs.Length; ++i)
			{
				if (tabPrefabs[i] != null && tabPrefabs[i].name == name)
				{
					OnClickTab(GetTab(i));
					return;
				}
			}
		}
		
		public void Open()
		{
			OnClickTab(currentTab);
		}
		
		public void Close()
		{
			foreach (UITab t in tabs)
			{
				if (t!=null)
				{
					t.SetVisible(false);
				}
			}
		}
		
		public void CloseAndResetTab()
		{
			for (int i=0; i<tabs.Length; ++i)
			{
				if (tabs[i] != null)
				{
					currentTab = tabs[i];
					break;
				}
			}
			Close();
		}
	}
}
