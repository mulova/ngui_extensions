using UnityEngine;
using System.Collections;
using ngui.ex;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

using mulova.commons;
using mulova.comunity;


namespace ngui.ex
{
    public delegate void PopupCallback();
    /// <summary>
    /// Popup base.
    /// Requirement: ButtonHandler needs 'close' button
    /// </summary>
    [RequireComponent(typeof(UIWindow))]
    public abstract class PopupBase : LogBehaviour, InputListener, IReleasablePool
    {
        public UITabHandler tabs;
        public ScreenStretch screenCover;
        internal bool shared;
        private EventRegistry eventReg = new EventRegistry();
        private ReleasablePool releasables = new ReleasablePool();

        public event Action onShowBegin;
        public event Action onShowBegun;
        public event Action onShowEnd;
        public event Action onHideBegin;
        public event Action onHideEnd;

        protected PopupCallback closeCallback;
        protected PopupCallback openCallback;

        internal PopupLoadData loadData;
        private bool initialized;
        public const string BTN_CLOSE = "btn_close";
        public const string CLOSE = "close";

        /// <summary>
        /// Called at the first time the popup is shown.
        /// </summary>
        /// <param name="window">Window.</param>
        protected abstract void Init(UIWindow window);
         
        protected UIWindow _window;

        public UIWindow window
        {
            get
            {
                if (_window == null)
                {
                    _window = GetComponent<UIWindow>();
                }
                return _window;
            }
        }

        protected virtual void Awake()
        {
            EventDelegate.Add(window.onShowBegin, OnShowBegin);
            EventDelegate.Add(window.onShowBegun, OnShowBegun);
            EventDelegate.Add(window.onShowEnd, OnShowEnd);
            EventDelegate.Add(window.onHideBegin, OnHideBegin);
            EventDelegate.Add(window.onHideEnd, OnHideEnd);
#if UNITY_EDITOR
            window.ui.SetActive(false); // PopupBaseBuildProcessor
#endif
            
            if (screenCover == null)
            {
                foreach (ScreenStretch s in GetComponentsInChildren<ScreenStretch>(true))
                {
                    if (s.IsCoveringFullScreen())
                    {
                        screenCover = s;
                        break;
                    }
                }
            }
            ReleasablePool.Register(this);
        }
        
        public bool IsCloseButtonClicked()
        {
            string clicked = GetClickedButton();
            return clicked == BTN_CLOSE||clicked == CLOSE;
        }
        
        protected virtual void OnDestroy()
        {
            ReleasablePool.Deregister(this);
        }
        
        public void Add(IReleasable r)
        {
            releasables.Add(r);
        }
        
        public void Remove(IReleasable r)
        {
            releasables.Remove(r);
        }
        
        public void Release()
        {
            releasables.Release();
        }
        
        private void OnInit()
        {
            Init(window);
            #if UNITY_EDITOR
            List<GameObject> unregistered = GetComponent<ButtonHandler>().GetUnregistered();
            if (unregistered.IsNotEmpty()) 
            {
                string msg = name + " Missing Callbacks\n";
                foreach (GameObject o in unregistered) 
                {
                    msg += o.name + " ";
                }
            Assert.Fail(this, msg);
            }
            #endif
        }
        
        protected void RegisterEvent(string id, Action callback)
        {
            eventReg.AddCallback(id, callback);
        }
        
        protected void RegisterEvent(string id, Action<object> callback)
        {
            eventReg.AddCallback(id, callback);
        }
        
        private void OnShowBegin()
        {
            try
            {
                if (window.buttonHandler != null)
                {
                    window.buttonHandler.enabled = false;
                }
                log.Debug("{0} ShowBegin", name);
                EventRegistry.SendEvent(name, this);
                baseLayer = InputListenerStack.inst.GetStackTop();
                InputListenerStack.inst.Push(this);
                PreOpen();
                eventReg.RegisterEvents();
                if (onShowBegin != null)
                {
                    onShowBegin();
                }
            } catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        
        private void OnShowBegun()
        {
            try
            {
                log.Debug("{0} ShowBegun", name);
                if (tabs != null&&!tabs.initialized)
                {
                    tabs.Init(this);
                }
                SetContents();
                SetLayer();
                if (tabs != null)
                {
                    tabs.Open();
                }
                PlayAudio();
                if (onShowBegun != null)
                {
                    onShowBegun();
                }
            } catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        protected virtual void PlayAudio()
        {
            #if NGUI_AUDIO
            AudioBridge.Play(GetType().Name);
            #endif
        }
        
        private void OnShowEnd()
        {
            log.Debug("{0} ShowEnd", name);
            if (window.buttonHandler != null)
            {
                window.buttonHandler.enabled = true;
            }
            try
            {
                SetLayer();
                SetContentsLazy();
            } catch (Exception ex)
            {
                log.Error(ex);
            }
            try
            {
                PostOpen();
            } catch (Exception e)
            {
                log.Error(e);
            }
            InputBlocker.Hide(this);
            if (onShowEnd != null)
            {
                onShowEnd();
            }
            loadData.consumed = true;
            if (openCallback != null)
            {
                openCallback();
                openCallback = null;
            }
            InputBlocker.Hide(this);
        }
        
        private void OnHideBegin()
        {
            baseLayer = null;
            InputBlocker.Show(this);
            try
            {
                PreClose();
            } catch (Exception e)
            {
                log.Error(e);
            }
            log.Debug("{0} HideBegin", name);
            if (window.buttonHandler != null)
            {
                window.buttonHandler.enabled = false;
            }
            InputListenerStack.inst.Pop(this);
            if (onHideBegin != null)
            {
                onHideBegin();
            }
        }
        
        private void OnHideEnd()
        {
            log.Debug("{0} HideEnd", name);
            InputBlocker.Hide(this);
            eventReg.DeregisterEvents();
            try
            {
                PostClose();
            } catch (Exception e)
            {
                log.Error(e);
            }
            if (tabs != null)
            {
                tabs.CloseAndResetTab();
            }
            PopupCallback callback = closeCallback;
            this.closeCallback = null;
            releasables.Release();
            if (callback != null)
            {
                callback();
            }
            if (onHideEnd != null)
            {
                onHideEnd();
            }
            if (!shared)
            {
                Object.Destroy(gameObject);
            }
        }
        
        public void OnButton(KeyCode button, ButtonState state)
        {
            NetThrobber block = UnityEngine.Object.FindObjectOfType<NetThrobber>();
            if (block != null&&block.IsVisible())
            {
                return;
            }
            if (_window.Status.IsOpening())
            {
                return;
            }
            OnBackButton();
        }

        protected virtual void OnBackButton()
        {
            Close();
        }

//        protected void OnRequestFail(MsgErrorResponse err)
//        {
//            Close();
//            Notice.ShowNetError(err);
//        }
        
        public bool IsCoveringFullScreen()
        {
            return screenCover != null&&screenCover.IsCoveringFullScreen();
        }
        
        public void OnFocus(bool focus, object triggerObj)
        {
            // check if the object focused hides the whole screen
            PopupBase p = triggerObj as PopupBase;
            if (p != null)
            {
                if (focus)
                {
                    // turn on the popups below until full screen popup is found
                    InputListenerStack.inst.ForEach(l => {
                        PopupBase p2 = l as PopupBase;
                        if (p2 != null)
                        {
                            p2._window.ui.SetActive(true);
                        }
                        return !p2.IsCoveringFullScreen();
                    });
                    //Refresh();
                } else
                {
                    // turn off the popups below until full screen popup is found 
                    if (p.IsCoveringFullScreen())
                    {
                        InputListenerStack.inst.ForEach(l => {
                            PopupBase p2 = l as PopupBase;
                            if (p2 != null)
                            {
                                p2._window.ui.SetActive(false);
                            }
                            return !p.IsCoveringFullScreen();
                        });
                    }
                }
            }
            OnFocusImpl(focus, triggerObj);
            #pragma warning disable 0162
            const bool FOCUS_REFRESH = false;
            if (focus&&FOCUS_REFRESH)
            {
                Refresh();
                if (tabs != null&&tabs.GetActiveTab() != null)
                {
                    tabs.GetActiveTab().Refresh();
                }
            }
            #pragma warning restore 0162
        }
        
        private bool closed;

        internal void OpenRequest()
        {
            this.closed = false;
            if (!initialized)
            {
                OnInit();
                initialized = true;
            }
            InputBlocker.Show(this);
            GetContents(() => {
                if (!closed)
                {
                    window.Open();
                }
            });
        }
        
        protected abstract void PreOpen();

        protected abstract void PostOpen();

        protected abstract void PreClose();

        protected abstract void PostClose();
        
        protected abstract void GetContents(Action callback);

        protected abstract void SetContents();

        protected abstract void SetContentsLazy();

        protected abstract void OnFocusImpl(bool focus, object triggerObj);
        
        public virtual void CloseButton()
        {
            Close();
        }
        
        protected void RefreshTabs(object seed)
        {
            if (tabs != null&&tabs.tabs != null)
            {
                foreach (UITab t in tabs.tabs)
                {
                    t.RefreshSeed(seed);
                }
            }
        }
        
        public void Refresh()
        {
            try
            {
                GetContents(() => {
                    SetContents();
                    SetContentsLazy();
                });
            } catch (Exception ex)
            {
                log.Error(ex);
                Close();
            }
        }
        
        public void ClearCloseCallback()
        {
            closeCallback = null;
        }
        
        public virtual void Close()
        {
            closed = true;
            log.Debug("Close {0}", name);
            if (window.Status == UIPanelStatus.Showing)
            {
                window.Close();
            } else
            {
                log.Warn("Too early close request for {0}", this);
                window.Hide(null, true);
                loadData.consumed = true;
                InputListenerStack.inst.Pop(this);
            }
            InputBlocker.Hide(this);
        }
        
        public bool Hide(Action endCallback = null, bool instant = false)
        {
            if (window.Status == UIPanelStatus.ShowBegin)
            {
                log.Warn("Too early close request for {0}", this);
            } 
            log.Debug("Close {0}", name);
            return window.Hide(endCallback, instant);
        }
        
        public string GetClickedButton()
        {
            return window.GetClickedButton();
        }
        
        public UIButton GetButton(string buttonName)
        {
            return window.GetButton(buttonName);
        }
        
        public GameObject GetButtonObject(string buttonName)
        {
            return window.GetButtonObject(buttonName);
        }
        
        public T GetButtonComponent<T>(string buttonName) where T:Component
        {
            return window.GetButton(buttonName).GetComponentInChildren<T>();
        }

        private object baseLayer;

        public void SetLayer()
        {
            if (window.HasMesh())
            {
                if (baseLayer is PopupBase)
                {
                    PopupBase p = baseLayer as PopupBase;
                    window.SetLayerOver(p.window);
                } else
                {
                    window.Panel.SetMinSortingOrder(window.Panel.GetComponentsInChildren<UIPanel>(true), 100);
                }
            }
        }
        
        public bool IsVisible()
        {
            return window.IsVisible();
        }
        
        public void AddReleasable(IReleasable r)
        {
            releasables.Add(r);
        }
    }
}