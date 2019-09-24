//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using mulova.commons;
using System;
using System.Collections.Generic;
using mulova.comunity;
using System.Collections.Generic.Ex;
using System.Ex;

namespace ngui.ex
{
    /// <summary>
    /// Set Callback for UIButtons according to the UIButton.name
    /// </summary>
    public class ButtonHandler : LogBehaviour, IEnumerable<UIButton>
    {
        public List<GameObject> buttons = new List<GameObject>();
        private MultiMap<GameObject, Action> callbackMap = new MultiMap<GameObject, Action>();
        // int param: buttonIndex,  string param: buttonName
        private Dictionary<string, GameObject> _buttonMap;

        private Dictionary<string, GameObject> buttonMap
        {
            get
            {
                if (_buttonMap == null)
                {
                    _buttonMap = new Dictionary<string, GameObject>();
                    foreach (GameObject o in buttons)
                    {
                        if (o != null)
                        {
                            _buttonMap[o.name] = o;
                            UIButton btn = o.AddMissingComponent<UIButton>();
                            
                            Collider2D collider = btn.GetComponent<Collider2D>();
                            if (collider == null)
                            {
                                NGUITools.AddWidgetCollider(btn.gameObject);
                                collider = btn.GetComponent<Collider2D>();
                                collider.isTrigger = true;
                            }
                            btn.AddCallback(OnButtonClick, btn.gameObject);
#if UNITY_EDITOR
                            if (!Application.isPlaying)
                            {
                                UnityEditor.EditorUtility.SetDirty(o);
                                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                            }
#endif
                        } else
                        {
                            log.Error("Null Button");
                        }
                    }
                }
                return _buttonMap;
            }
        }


        public GameObject Selected
        {
            get;
            set;
        }

        public GameObject GetButtonObject(object key)
        {
            return buttonMap.Get(key.ToText());
        }

        public UIButton GetButton(string key)
        {
            GameObject o = GetButtonObject(key);
            if (o == null)
            {
                return null;
            }
            return o.GetComponent<UIButton>();
        }

        public bool IsSelected(string name)
        {
            if (Selected == null)
            {
                return false;
            }
            return Selected.name == name;
        }

        private void RegisterCallback(object buttonId, Action callback, bool clear)
        {
            string buttonName = buttonId.ToString();
            GameObject obj = buttonMap.Get(buttonName);
            if (obj == null)
            {
                log.Error("'{0}' isn't added to button list", buttonName);
            } else
            {
                if (clear)
                {
                    callbackMap.Remove(obj);
                }
                callbackMap.Add(obj, callback);
            }
        }

        public void AddCallback(object buttonId, Action callback)
        {
            RegisterCallback(buttonId, callback, false);
        }

        public void SetCallback(object buttonId, Action callback)
        {
            RegisterCallback(buttonId, callback, true);
        }

        public void AddButton(GameObject btn, Action callback)
        {
            _buttonMap = null;
            buttons.Add(btn);
            SetCallback(btn.name, callback);
        }

        public List<GameObject> GetUnregistered()
        {
            List<GameObject> unregistered = new List<GameObject>();
            foreach (GameObject o in buttons)
            {
                if (o != null&&!callbackMap.ContainsKey(o))
                {
                    unregistered.Add(o);
                }
            }
            return unregistered;
        }

        public void Call(object buttonId)
        {
            List<Action> callbacks = callbackMap.GetSlot(buttonMap.Get(buttonId.ToText()));
            if (callbacks != null)
            {
                foreach (Action a in callbacks)
                {
                    a.Call();
                }
            }
        }

        [NoObfuscate]
        public void OnButtonClick(GameObject o)
        {
            if (!enabled)
            {
                return;
            }
            Selected = o;
            if (Selected == null)
            {
                if (o != null)
                {
                    log.Warn("Invalid Parameter {0}({1})", o, o.GetType().FullName);
                }
                return;
            }
            IEnumerable<Action> callbackList = callbackMap[Selected];
            if (callbackList != null)
            {
                foreach (Action callback in callbackList)
                {
                    callback.Call();
                }
            } else
            {
                log.Error("No callback for {0}", Selected);
            }
        }

        public bool IsButton(string name)
        {
            if (Selected == null)
            {
                return false;
            }
            return Selected.name.Equals(name, System.StringComparison.OrdinalIgnoreCase);
        }

        public bool IsButton(int index)
        {
            return buttons != null&&index < buttons.Count&&Selected == buttons[index];
        }

        private int GetIndex()
        {
            for (int i = 0; i < buttons.Count; ++i)
            {
                if (IsButton(i))
                {
                    return i;
                }
            }
            return -1;
        }

        #region IEnumerable implementation

        IEnumerator<UIButton> IEnumerable<UIButton>.GetEnumerator()
        {
            foreach (var o in buttons) {
                if (o != null) {
                    yield return o.GetComponent<UIButton>();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var o in buttons) {
                if (o != null) {
                    yield return o.GetComponent<UIButton>();
                }
            }
        }

        #endregion
    }
}
