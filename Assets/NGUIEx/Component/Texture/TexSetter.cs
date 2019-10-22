//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.Collections.Generic.Ex;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;
using ILogger = mulova.commons.ILogger;

namespace ngui.ex
{
    [RequireComponent(typeof(TexLoader))]
	public class TexSetter : MonoBehaviour
	{
		public List<AssetRef> textures = new List<AssetRef>();
		private TexLoader texLoader;

		public static readonly ILogger log = LogManager.GetLogger(typeof(TexSetter));

		public TexLoader GetLoader()
		{
			if (texLoader == null)
			{
				texLoader = GetComponent<TexLoader>();
			}
			return texLoader;
		}

		void OnEnable()
		{
			// invalid 
			if (GetLoader() == null)
			{
				log.Warn("No TexLoader in {0}", transform.GetScenePath());
				return;
			}
			if (GetLoader().Target == null)
			{
				log.Warn("No UITexture in {0}", transform.GetScenePath());
				return;
			}
			if (textures.GetCount() == 1&&GetLoader().Target.mainTexture == null)
			{
				GetLoader().Load(textures[0], null);
			}
		}

		void OnDisable()
		{
//			GetLoader().Clear();
		}

		public void SetTexture(int i)
		{
			if (textures.IsEmpty())
			{
				return;
			}
			if (Platform.isPlaying)
			{
				GetLoader().Load(textures[i], null);
			} else
			{
#if UNITY_EDITOR
				i = i.Clamp(0, textures.Count);
				if (GetLoader().Target != null)
				{
					AssetRef r = textures[i];
					r.LoadAsset<Texture>(t=> {
						GetLoader().Target.mainTexture = t;
                #if UNITY_5_3_OR_NEWER
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                #else
                    UnityEditor.EditorUtility.SetDirty(GetLoader().gameObject);
                #endif
					});
				}
#endif
			}
		}

		public bool isEmpty
		{
            get
            {
                bool empty = true;
                if (!textures.IsEmpty())
                {
                    foreach (AssetRef r in textures)
                    {
                        empty &= r.isEmpty;
                    }
                }
                return empty;
            }
		}
	}
}
