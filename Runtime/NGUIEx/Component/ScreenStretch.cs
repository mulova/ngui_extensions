using System;
using System.Text.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;


namespace ngui.ex
{
    [RequireComponent(typeof(UIWidget)), ExecuteAlways]
	public class ScreenStretch : MonoBehaviour
	{
		public enum ResizeType
		{
			WIDTH,
			HEIGHT,
			BOTH

		}

		public ResizeType resizeType = ResizeType.BOTH;
		public bool turnBgCam = true;
		private Transform trans;
		private WeakQueue<Camera> invisibleCams;
		private UIWidget widget;
		public static readonly ILog log = LogManager.GetLogger(typeof(ScreenStretch));

		void Start()
		{
			if (Application.isPlaying)
			{
				Stretch();
			}
		}

		void OnEnable()
		{
			Init();
			if (widget == null)
			{
				log.Warn("No wiget for ScreenStretch {0}", transform.GetScenePath());
				return;
			}
			if (!Application.isPlaying)
			{
				return;
			}
            
            
			UIWindow win = GetComponentInParent<UIWindow>();
			if (win != null)
			{
				Stretch();
				if (turnBgCam)
				{
					EventDelegate.Add(win.onShowEnd, TurnOffInvisibleCams, true);
					EventDelegate.Add(win.onHideBegin, TurnOnCams, true);
				}
			} else
			{
				if (turnBgCam)
				{
					TurnOffInvisibleCams();
				}
			}
		}

		void OnDisable()
		{
			TurnOnCams();
		}

		public void Init()
		{
			if (trans == null)
			{
				trans = transform;
				widget = GetComponent<UIWidget>();
			}
		}

		public bool IsCoveringFullScreen()
		{
			if (!enabled||!gameObject.activeSelf)
			{
				return false;
			}
			Init();
			if (widget == null||widget.alpha < 1||resizeType != ResizeType.BOTH)
			{
				return false;
			}
			UITexture tex = widget as UITexture;
			if (tex != null)
			{
				if (tex.mainTexture == null)
				{
					return tex.GetComponent<TexSetter>() != null;
				}
				Texture2D tex2d = tex.mainTexture as Texture2D;
				if (tex2d != null&&tex2d.format.HasAlpha())
				{
					return false;
				}
			}
			UISprite s = widget as UISprite;
			if (s != null&&s.spriteName.IsEmpty())
			{
				return false;
			}
			return true;
		}

		private void TurnOffInvisibleCams()
		{
			if (!IsCoveringFullScreen())
			{
				return;
			}
            
			invisibleCams = new WeakQueue<Camera>();
			// Turn off invisible camera
			Camera[] cams = Camera.allCameras;
			// get the min depth for camera rendering this object
			float minDepth = int.MaxValue;
			int layerMask = 1<<gameObject.layer;
			foreach (Camera c in cams)
			{
				if ((c.cullingMask&layerMask) != 0)
				{
					minDepth = Math.Min(minDepth, c.depth);
				}
			}
			if (minDepth != int.MaxValue)
			{
				foreach (Camera c in cams)
				{
					if (c.enabled&&c.depth < minDepth)
					{
						c.enabled = false;
						invisibleCams.Enqueue(c);
					}
				}
			}
		}

		private void TurnOnCams()
		{
			if (invisibleCams == null)
			{
				return;
			}
			while (!invisibleCams.IsEmpty())
			{
				Camera c = invisibleCams.Dequeue();
				if (c != null)
				{
					c.enabled = true;
				}
			}
		}

        
		[ContextMenu("Stretch")]
		private void Stretch()
		{
			Vector2 screenSize = GetScreenSize(this.gameObject);
			if (resizeType == ResizeType.BOTH||resizeType == ResizeType.WIDTH)
			{
				int newWidth = (int)(screenSize.x+2);
				if (newWidth != widget.width)
				{
					widget.width = newWidth;
					#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEditor.EditorUtility.SetDirty(gameObject);
#if UNITY_5_3_OR_NEWER
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
                    }
					#endif
				}
			}
			if (resizeType == ResizeType.BOTH||resizeType == ResizeType.HEIGHT)
			{
				int newHeight = (int)(screenSize.y+2);
				if (newHeight != widget.height)
				{
					widget.height = newHeight;
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEditor.EditorUtility.SetDirty(gameObject);
#if UNITY_5_3_OR_NEWER
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
                    }
                    #endif
				}
			}
		}

		void Update()
		{
			if (widget == null)
			{
				return;
			}
            
			if (resizeType == ResizeType.BOTH)
			{
				Vector3 pos = trans.position;
				pos.x = 0;
				pos.y = 0;
				trans.position = pos;
			} else if (resizeType == ResizeType.WIDTH)
			{
				Vector3 pos = trans.position;
				pos.x = 0;
				trans.position = pos;
			} else if (resizeType == ResizeType.HEIGHT)
			{
				Vector3 pos = trans.position;
				pos.y = 0;
				trans.position = pos;
			}
		}

		public static Vector2 GetScreenSize(GameObject obj)
		{
			UIPanel panel = obj.GetComponentInParent<UIPanel>();
			return panel.GetScreenSize();
		}
	}
    
    
}