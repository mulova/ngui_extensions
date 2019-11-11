using System;
using System.Ex;
using System.Text.Ex;
using mulova.comunity;
using UnityEngine;

namespace ngui.ex
{
    /// <summary>
    /// Added texture is automatically set to UITexture asynchronously.
    /// TexListener is called after loaded texture is set to UITexture.
    /// 
    /// </summary>
    [RequireComponent(typeof(UITexture))]
    public class TexLoader : LogBehaviour, IReleasable
    {
        public bool pixelPerfect;
        public string exclusiveId;
        [HideInInspector]
        public bool recoverOnEnable; // temporaily clear texture while on disable
        [HideInInspector]
        public bool removeOnDisable;
        private string curUrl;
        private string newUrl;
        private Action<Texture> loadedCallback;
        private TexLoaderStatus status = TexLoaderStatus.Idle;
        [HideInInspector]
        public string editorTexPath;
        private bool cdn;
        private UITexture target;

        public UITexture Target
        {
            get
            {
                if (target == null)
                {
                    target = GetComponent<UITexture>();
                }
                return target;
            }
        }

        void Start()
        {
//          ReleasablePool.FindAndAdd(this);
        }

        void OnDestroy()
        {
//          ReleasablePool.FindAndRemove(this);
        }

        public void Load(AssetRef asset, Action<Texture> callback)
        {
            if (Target != null&&!asset.isEmpty)
            {
                if (asset.cdn)
                {
                    Load(asset.path, callback);
                } else if (asset.GetReference() != null)
                {
                    this.loadedCallback = callback;
                    SetTexture(asset.GetReference() as Texture);
                }
            }
        }

        public void Load(string url, Action<Texture> callback = null)
        {
            Load(url, true, callback);
        }

        public void Load(string url, bool cdn, Action<Texture> callback = null)
        {
            this.loadedCallback = callback;
            this.cdn = cdn;
            if (url == curUrl&&status == TexLoaderStatus.Idle)
            {
                // same as now
                Finish();
            } else if (url == newUrl&&status == TexLoaderStatus.Download)
            {
                // same as previous request
                return;
            }
                
            if (status == TexLoaderStatus.Download)
            {
                Interrupt();
            }
            if (url.IsEmpty())
            {
                Clear();
            } else
            {
                this.newUrl = url;
                status = TexLoaderStatus.Download;
                LoadAsync();
            }
        }

        private void Interrupt()
        {
            log.Debug("Download for {0} INTERRUPTED", newUrl);
            // remove pending if exists
            if (!newUrl.IsEmpty())
            {
                loadedCallback = null;
            }
            Finish();
        }

        void Update()
        {
            if (status != TexLoaderStatus.Download&&curUrl != newUrl)
            {
                LoadAsync();
            }
        }
        
        internal void LoadAsync()
        {
            log.Debug("Loading texture {0}", newUrl);
            string url = newUrl;
            AssetCache cache = cdn? Cdn.cache : Web.cache;
            cache.GetTexture(url, tex => {
                if (url == newUrl)
                { // check if another request is triggered while downloading is in progress.
                    curUrl = newUrl;
                    if (tex != null)
                    {
                        SetTexture(tex);
                    } else
                    {
                        log.Debug("Can't access {0}", url);
                        Clear();
                    }
                }
                Finish();
            }, exclusiveId);
        }

        public void Clear()
        {
            if (status == TexLoaderStatus.Download)
            {
                Interrupt();
            }
            curUrl = null;
            newUrl = null;
            RemoveTexture();
            Finish();
        }

        private void RemoveTexture()
        {
            if (Target != null)
            {
                Target.mainTexture = null;
            }
        }

        void OnDisable()
        {
            if (recoverOnEnable)
            {
                // reload texture when enabled again
                if (!curUrl.IsEmpty())
                {
                    newUrl = curUrl;
                    status = TexLoaderStatus.Idle;
                }
                curUrl = null;
                RemoveTexture();
            } else if (removeOnDisable)
            {
                Clear();
            }
        }

        public bool IsIdle()
        {
            return status == TexLoaderStatus.Idle;
        }

        private void Finish()
        {
            status = TexLoaderStatus.Idle;
            if (loadedCallback != null)
            {
                Action<Texture> a = loadedCallback;
                loadedCallback = null;
                a.Call(target.mainTexture);
            }
        }

        private void SetTexture(Texture tex)
        {
            if (tex != null)
            {
                if (Target != null)
                {
                    Target.mainTexture = tex;
                    if (pixelPerfect)
                    {
                        Target.MakePixelPerfect();
                    }
                    // fix for the alignment error when window is resizing
                    Target.onRender = Rebuild;
                }
                Finish();
            } else
            {
                Clear();
            }
        }

        private void Rebuild(Material mat)
        {
            if (Target != null)
            {
                Target.Invalidate(false);
                Target.onRender = null;
            }
        }

        public void Release()
        {
            Clear();
        }
    }

    enum TexLoaderStatus
    {
        Idle,
        Download,
    }
}
