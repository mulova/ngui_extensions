using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mulova.comunity;
using mulova.commons;
using System.Ex;
using UnityEngine.Ex;
using System.Collections.Generic.Ex;

namespace ngui.ex
{
    public class PopupManager : SingletonBehaviour<PopupManager>
    {
        private const int CACHE_SIZE = 8;
        /// <summary>
        /// Not destroyed for ever
        /// </summary>
        private readonly Type[] IMMORTAL = {
        };
        /// <summary>
        /// No other popup can be opened over the exclusive popup
        /// </summary>
        
        private Dictionary<string, PopupBase> instanceMap = new Dictionary<string, PopupBase>();
        private List<string> lruCache = new List<string>();
        private Queue<PopupLoadData> loadList = new Queue<PopupLoadData>();
        private PopupLoadData currLoading;
        
        private bool IsImmortal(Type key)
        {
            foreach (Type t in IMMORTAL)
            {
                if (key == t)
                {
                    return true;
                }
            }
            return false;
        }

        void Update()
        {
            if (currLoading != null)
            {
                if (currLoading.consumed)
                {
                    currLoading = null;
                } else if (currLoading.shared)
                {
                    PopupBase p = instanceMap.Get(currLoading.key);
                    if (p != null&&p.window.Status.IsShowing())
                    {
                        currLoading = null;
                    }
                }
                    
            }
            if (loadList.Count > 0&&currLoading == null)
            {
                currLoading = loadList.Dequeue();
                PopupBase p = null;
                if (currLoading.shared)
                {
                    lruCache.Remove(currLoading.key);
                    lruCache.Add(currLoading.key);
                    p = instanceMap.Get(currLoading.key);
                }
                
                if (p != null)
                {
                    currLoading.Complete(p);
                } else
                {
                    StartCoroutine(LoadInstance(currLoading));
                }
            }
        }

        public void GetInstance(string key, Action<PopupBase> callback)
        {
            GetInstance(key, true, callback);
        }

        public void GetInstance(string key, bool shared, Action<PopupBase> callback)
        {
            var data = new PopupLoadData(key, shared, callback);
            InputBlocker.Show(this);
            loadList.Enqueue(data);
        }

        private IEnumerator LoadInstance(PopupLoadData loadData)
        {
            log.Debug("Instantiate '{0}'", loadData.key);
            ResourceRequest req = Resources.LoadAsync(loadData.key);
            yield return req;
            GameObject prefab = req.asset as GameObject;
            if (prefab != null)
            {
                GameObject inst = prefab.InstantiateEx();
                inst.SetActive(true);
                PopupBase p = inst.GetComponent<PopupBase>();
                p.shared = loadData.shared;
                p.transform.SetParent(transform, false);
                if (loadData.shared)
                {
                    instanceMap[loadData.key] = p;
                    lruCache.Remove(loadData.key);
                    lruCache.Add(loadData.key);
                    // check cache size
                    if (!IsImmortal(p.GetType()))
                    {
                        if (lruCache.Count > CACHE_SIZE)
                        {
                            string head = lruCache[0];
                            PopupBase oldPopup = instanceMap[head];
                            if (oldPopup.window.Status == UIPanelStatus.Hidden)
                            {
                                log.Debug("Destroy '{0}'", oldPopup.name);
                                instanceMap.Remove(head);
                                oldPopup.gameObject.DestroyEx();
                                lruCache.RemoveAt(0);
                            }
                        }
                    }
                }
                loadData.Complete(p);
            } else
            {
                log.Error("Popup '{0}' is missing", loadData.key);
                loadData.Complete(null);
            }
        }
        
        public void Open<P>(Action<P> initFunc = null, bool shared = true) where P: PopupBase
        {
            GetInstance(typeof(P).Name, shared, p => {
                if (p != null)
                {
                    try
                    {
                        log.Debug("Open Popup '{0}'", p.name);
                        initFunc.Call(p as P);
                        p.OpenRequest();
                    } catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            });
        }

        public void CloseAll()
        {
            InputListenerStack.inst.ForEach(l => {
                PopupBase p2 = l as PopupBase;
                if (p2 != null)
                {
                    p2.ClearCloseCallback();
                    p2.Hide(null, true);
                }
                return true;
            });
        }
    }

    internal class PopupLoadData
    {
        internal readonly string key;
        internal readonly bool shared;
        private readonly Action<PopupBase> getCallback;
        internal bool consumed { get; set; }

        public PopupLoadData(string key, bool shared, Action<PopupBase> callback)
        {
            this.key = key;
            this.shared = shared;
            this.getCallback = callback;
        }

        public void Complete(PopupBase p)
        {
            if (p != null)
            {
                p.loadData = this;
            } else
            {
                Debug.LogErrorFormat("Missing Popup '{0}'", key);
            }
            getCallback.Call(p);
        }
    }
}

