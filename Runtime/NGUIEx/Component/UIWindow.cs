//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;


namespace ngui.ex
{
    /// <summary>
    /// Add alpha, move funtionality to UIPanel
    /// </summary>
    [AddComponentMenu("NGUI/Ex/Window")]
	[RequireComponent(typeof(ButtonHandler))]
	[ExecuteAlways]
	public class UIWindow : MonoBehaviour, IComparable<UIWindow>
	{
		public static readonly ILog log = LogManager.GetLogger(typeof(UIWindow));
		[Range(0, 1)]public float alpha = 1;
		private UIPanel[] panels;
		private Vector3 scale;
		private Transform trans;
		private UIPanelStatus status = UIPanelStatus.Init;
		public GameObject ui;
		[NullableField] public AnimationClip showClip;
		[NullableField] public AnimationClip hideClip;
		
		private Vector3 initialPos;
		private Quaternion initialRot;
		private Vector3 initialScale;
		private Animation anim;
		
		// event-related
		private ButtonHandler buttons;

		public ButtonHandler buttonHandler
		{
			get
			{
				if (buttons == null)
				{
					buttons = GetComponent<ButtonHandler>();
				}
				return buttons;
			}
		}

		public List<EventDelegate> onInit = new List<EventDelegate>();
		public List<EventDelegate> onShowBegin = new List<EventDelegate>();
		// preopen
		public List<EventDelegate> onShowBegun = new List<EventDelegate>();
		// called just after ui is activated
		public List<EventDelegate> onShowEnd = new List<EventDelegate>();
		public List<EventDelegate> onHideBegin = new List<EventDelegate>();
		public List<EventDelegate> onHideEnd = new List<EventDelegate>();

		public UIPanel Panel
		{
			get
			{
				if (!Panels.IsEmpty())
				{
					return Panels[0];
				} else
				{
					return null;
				}
			}
		}

		public UIPanel[] Panels
		{
			get
			{
				if (panels == null)
				{
					panels = this.GetComponentsInChildren<UIPanel>(true);
					if (!panels.IsEmpty())
					{
						alpha = panels[0].alpha;
					}
					trans = transform;
					scale = trans.localScale;
				}
				return panels;
			}
		}

		public UIPanelStatus Status
		{
			get { return status; }
		}

		public bool HasMesh()
		{
			foreach (UIPanel p in Panels)
			{
				if (p.drawCalls.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnInit()
		{
		}

		void Awake()
		{
			initialPos = transform.localPosition;
			initialRot = transform.localRotation;
			initialScale = transform.localScale;
		}

		void LateUpdate()
		{
			if (Panel == null)
			{
				return;
			}
			if (alpha != Panel.alpha)
			{
				Panel.alpha = alpha;
			}
			if (scale != trans.localScale)
			{
				scale = trans.localScale;
				Panel.Invalidate(false);
			}
		}

		public bool Show()
		{
			if (status != UIPanelStatus.Init&&status != UIPanelStatus.Hidden)
			{
				return false;
			}
			if (status == UIPanelStatus.Init)
			{
				OnInit();
				EventDelegate.Execute(onInit);
			} else if (status == UIPanelStatus.HideBegin)
			{
				OnHideEndImpl();
			}
			OnShowBegin0();
			
			if (anim == null)
			{
				anim = GetComponent<Animation>();
			}
			if (showClip != null&&anim != null)
			{
				
				if (!anim.IsPlaying(showClip.name))
				{
					if (showClip.name == "window_show")
					{ // Legionz specific to remove blink
						Panel.alpha = 0; 
					}
					if (showClip.name == "baccarat_score_show")
					{ // Legionz specific to remove blink
						Panel.alpha = 0; 
					}
					anim.Rewind(showClip);
					anim.PlayIgnoreScale(showClip, () =>
					{
						OnShowEndImpl();
					});
				}
			} else
			{
				transform.SetLocal(initialPos, initialRot, initialScale);
				OnShowEndImpl();
			}
			return true;
		}

		private Action endCallback;

		public bool Hide(Action endCallback = null, bool instant = false)
		{
			log.Debug("Hide {0}", name);
			if (status == UIPanelStatus.Init||status.IsHidden())
			{
				endCallback.Call();
				return false;
			} else
			{
				if (anim != null)
				{
					anim.Stop();
				}
				this.endCallback = endCallback;
				if (status == UIPanelStatus.ShowBegin)
				{
					OnShowEndImpl();
				}
				OnHideBegin0();
				
				if (hideClip != null&&anim != null&&!instant&&gameObject.activeInHierarchy)
				{
					anim.Stop();
					anim.Rewind(hideClip);
					anim.PlayIgnoreScale(hideClip, () =>
					{
						OnHideEndImpl();
					});
				} else
				{
					OnHideEndImpl();
				}
				return true;
			}
		}

		public void Open()
		{
			Show();
			buttonHandler.Selected = null;
		}

		public void Close()
		{
			Hide();
		}

		public bool SetVisible(bool visible)
		{
			if (visible)
			{
				return Show();
			} else
			{
				return Hide();
			}
		}

		protected void BeginTweenAlpha(float duration, bool show, EventDelegate.Callback endFunc)
		{
			GameObject go = Panel.gameObject;
			BeginTweenAlpha(go, duration, show, endFunc);
		}

		public static void BeginTweenAlpha(GameObject go, float duration, bool show, EventDelegate.Callback endFunc)
		{
			TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration);
			comp.style = UITweener.Style.Once;
			if (show)
			{
				comp.from = 0;
				comp.to = 1;
			} else
			{
				comp.from = 1;
				comp.to = 0;
			}
			comp.eventReceiver = null;
			comp.callWhenFinished = null;
			EventDelegate.Set(comp.onFinished, endFunc);
			comp.enabled = true;
		}

		public static void BeginTweenColor(UIWidget widget, float duration, Color to, EventDelegate endFunc)
		{
			TweenColor comp = UITweener.Begin<TweenColor>(widget.gameObject, duration);
			comp.style = UITweener.Style.Once;
			comp.from = widget.color;
			comp.to = to;
			comp.eventReceiver = null;
			comp.callWhenFinished = null;
			EventDelegate.Add(comp.onFinished, endFunc);
			comp.enabled = true;
		}

		private void OnShowBegin0()
		{
			Assert.IsTrue(status == UIPanelStatus.Hidden||status == UIPanelStatus.Init);
			status = UIPanelStatus.ShowBegin;
			EventDelegate.Execute(onShowBegin);
			ui.SetActive(true);
			EventDelegate.Execute(onShowBegun);
		}

		private void OnShowEndImpl()
		{
			if (status == UIPanelStatus.ShowBegin)
			{
				status = UIPanelStatus.Showing;
				EventDelegate.Execute(onShowEnd);
			} else
			{
				log.Warn("{0} is shown but closed instantly.", this);
			}
		}

		private void OnHideBegin0()
		{
			Assert.AreEqual(status, UIPanelStatus.Showing);
			status = UIPanelStatus.HideBegin;
			EventDelegate.Execute(onHideBegin);
		}

		private void OnHideEndImpl()
		{
			Assert.AreEqual(status, UIPanelStatus.HideBegin);
			status = UIPanelStatus.Hidden;
			ui.SetActive(false);
			EventDelegate.Execute(onHideEnd);
			endCallback.Call();
		}

		public bool IsVisible()
		{
			return ui.activeInHierarchy&&alpha > 0;
		}

		public int CompareTo(UIWindow that)
		{
			return this.GetComponent<UIPanel>().renderQueue-that.GetComponent<UIPanel>().renderQueue;
		}

		public static void SetVisible(GameObject[] obj, bool visible)
		{
			foreach (GameObject o in obj)
			{
				o.SetActive(visible);
			}
		}

		/// <summary>
		/// Delegate of ButtonHandler
		/// </summary>
		/// <param name="buttonName">Button name.</param>
		/// <param name="callback">Callback.</param>
		public void AddCallback(string buttonName, Action callback)
		{
			buttonHandler.AddCallback(buttonName, callback);
		}

		public void SetCallback(string buttonName, Action callback)
		{
			buttonHandler.SetCallback(buttonName, callback);
		}

		public string GetClickedButton()
		{
			if (buttonHandler.Selected == null)
			{
				return null;
			}
			return buttonHandler.Selected.name;
		}

		public UIButton GetButton(string name)
		{
			return buttonHandler.GetButton(name);
		}

		public GameObject GetButtonObject(string name)
		{
			return buttonHandler.GetButtonObject(name);
		}

		public void SetLayerOver(UIWindow below)
		{
			Panel.SetLayerOver(below.Panel);
		}
	}

	
	public enum UIPanelStatus
	{
		Init,
		ShowBegin,
		Showing,
		HideBegin,
		Hidden
	}

	public static class UIPageStepEx
	{
		public static bool IsOpening(this UIPanelStatus step)
		{
			return step == UIPanelStatus.Init
			||step == UIPanelStatus.ShowBegin;
		}

		public static bool IsShowing(this UIPanelStatus step)
		{
			return step == UIPanelStatus.ShowBegin
			||step == UIPanelStatus.Showing;
		}

		public static bool IsHidden(this UIPanelStatus step)
		{
			return step == UIPanelStatus.HideBegin
			||step == UIPanelStatus.Hidden;
		}
	}
}
