//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{
    public abstract class UITab : MonoBehaviour
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(UITab));
		public UIButton tabButton;
		public GameObject uiRoot;
		public GameObject newMark;
		public UIWidget widgetBg;
		
        [NullableField] protected ButtonHandler buttons;
        public MonoBehaviour container { get; private set; }
		private GameObject blocker;
		// block click through when tab is disabled
		private EventRegistry eventReg = new EventRegistry();
		
		private float localY;
		private int depth = -1;
		private object seed;

		/// <summary>
		/// Disabled tab should consume mouse click
		/// </summary>
		private void CreateTabClickBlocker()
		{
			BoxCollider2D srcCol = tabButton.GetComponent<BoxCollider2D>();
			blocker = tabButton.gameObject.CreateChild("block");
			UIWidget w = blocker.AddComponent<UIWidget>();
			w.depth = tabButton.GetComponentInChildren<UIWidget>().depth-1;
			w.width = (int)srcCol.size.x;
			w.height = (int)srcCol.size.y;
			BoxCollider2D col = blocker.AddComponent<BoxCollider2D>();
			col.size = srcCol.size;
			col.offset = srcCol.offset;
		}

		public void Show()
		{
			depth = -1;
			SetVisible(true);
		}

		public void Hide()
		{
			SetVisible(false);
		}

		protected void RegisterEvent(string id, Action callback)
		{
			eventReg.AddCallback(id, callback);
		}

		protected void RegisterEvent(string id, Action<object> callback)
		{
			eventReg.AddCallback(id, callback);
		}

		protected void SetNewMark(bool active)
		{
			newMark.SetActiveEx(active);
		}

		public bool isInitialized
		{
			get
			{
				return container != null;
			}
		}

		internal void Init(MonoBehaviour container)
		{
			this.container = container;
			if (newMark != null)
			{
				newMark.SetActive(false);
			}
			buttons = GetComponent<ButtonHandler>();
			if (buttons != null)
			{
				buttons.enabled = false;
			}
			CreateTabClickBlocker();
			
			InitTabInfo();
			
			Init();
		}

		// Called just once when the tab is enabled first.
		protected abstract void Init();

		protected abstract void PreOpen();

		protected abstract void PostClose();

		protected abstract void GetContents(object seed, Action callback);

		protected abstract void SetContents();

		public bool IsVisible()
		{
			if (uiRoot == null)
			{
				return false;
			}
			return uiRoot.activeSelf;
		}

		public void SetVisible(bool visible)
		{
			if (uiRoot == null)
			{
				return;
			}
			if (!tabButton.isEnabled == visible)
			{
				return;
			}
			tabButton.SetButtonActive(!visible);
			if (visible)
			{
				GetComponentInParent<UIPanel>().ArrangeRenderQueue();
				GetContents(seed, () =>
				{
					// tab selection may be changed after GetContents() is called
					if (!tabButton.isEnabled)
					{
						PreOpen();
						eventReg.RegisterEvents();
						uiRoot.SetActive(true);
						if (buttons != null)
						{
							buttons.enabled = true;
						}
						SetContents();
						NGUIUtil.Reposition(transform);
					}
				});
				
				
			} else
			{
				if (tabButton.isEnabled)
				{
					uiRoot.SetActive(false);
					if (buttons != null)
					{
						buttons.enabled = false;
					}
					eventReg.DeregisterEvents();
					PostClose();
				}
			}
			
			TabSelectChange(visible);
		}

		public void Refresh()
		{
			RefreshSeed(seed);
		}

		public void RefreshSeed(object seed)
		{
			if (IsVisible())
			{
				this.seed = seed;
				GetContents(seed, () =>
				{
					SetContents();
					NGUIUtil.Reposition(transform);
				});
			}
		}

		private void InitTabInfo()
		{
			if (widgetBg != null&&depth == -1)
			{
				localY = tabButton.transform.localPosition.y;
				depth = tabButton.GetComponent<UIWidget>().depth;
				
				TabSelectChange(false);
			}
		}

		public void TabSelectChange(bool visible)
		{
			InitTabInfo();
			
			if (visible)
			{
				if (!tabButton.isEnabled)
				{
					if (widgetBg != null)
					{
						tabButton.transform.SetLocalPositionY(localY);
						tabButton.GetComponent<UIWidget>().depth = widgetBg.depth+1;
						UILabel label = tabButton.GetComponentInChildren<UILabel>();
						if (label != null)
						{
							label.color = new Color(210f / 250f, 1f, 0f);
							label.transform.SetLocalScaleX(1.0f);
							label.transform.SetLocalScaleY(1.0f);
						}
					}
				}
			} else
			{
				if (tabButton.isEnabled)
				{
					if (widgetBg != null)
					{
						tabButton.transform.SetLocalPositionY((localY-10f));
						tabButton.GetComponent<UIWidget>().depth = depth;
						UILabel label = tabButton.GetComponentInChildren<UILabel>();
						if (label != null)
						{
							label.color = new Color(193f / 250f, 205f / 250f, 213f / 250f);
							label.transform.SetLocalScaleX(0.9f);
							label.transform.SetLocalScaleY(0.9f);
						}
					}
				}
			}
		}
	}
}
