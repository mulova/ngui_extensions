using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using mulova.commons;
using Assert = mulova.commons.Assert;
using mulova.comunity;
using System.Collections.Generic.Ex;
using UnityEngine.Ex;

namespace ngui.ex
{
	public static class UISpriteEx
	{
		public static Dictionary<string, GrayAtlas> grayAtlas = new Dictionary<string, GrayAtlas>();
		
		public static void ToGrayscale(this UISprite s, bool gray)
		{
			if (s == null || s.atlas == null)
			{
				return;
			}
			if (gray) {
				GrayAtlas a = GrayAtlasPool.GetGrayAtlas(s.atlas);
				s.atlas = a.gray;
			} else {
				if (GrayAtlasPool.HasGrayAtlas(s.atlas))
				{
					GrayAtlas a = GrayAtlasPool.GetGrayAtlas(s.atlas);
					s.atlas = a.src;
				}
			}
		}
		
		public static void ToNormalColor(this UISprite s)
		{
			if (s == null||s.atlas == null)
			{
				Assert.IsTrue(false);
				return;
			}
			GrayAtlas a = grayAtlas.Get(s.atlas.name);
			if (a != null)
			{
				s.atlas = a.src;
			}
		}
		
		public class GrayAtlas
		{
			public UIAtlas src;
			public UIAtlas gray;
			
			public GrayAtlas(UIAtlas a)
			{
				this.src = a;
				this.gray = a.Clone(src.spriteMaterial.shader.name);
				this.gray.spriteMaterial.SetFloat("_GreyStrength", 1);
				this.gray.name = a.name;
			}
		}

		public class GrayAtlasPool : MonoBehaviour
		{
			private Dictionary<string, GrayAtlas> pool = new Dictionary<string, GrayAtlas>();

			private static GrayAtlasPool inst;

			public static GrayAtlas GetGrayAtlas(UIAtlas a)
			{
				if (inst == null)
				{
					var go = new GameObject("GrayAtlasPool", typeof(GrayAtlasPool));
					inst = go.GetComponent<GrayAtlasPool>();
				}
				return inst.GetGray(a);
			}

			public GrayAtlas GetGray(UIAtlas a)
			{
				GrayAtlas ga = pool.Get(a.name);
				if (ga == null) {
					ga = new GrayAtlas(a);
					pool[a.name] = ga;
				}
				return ga;
			}

			public static bool HasGrayAtlas(UIAtlas a)
			{
				if (inst == null)
				{
					return false;
				}
				return inst.HasGray(a);
			}

			public bool HasGray(UIAtlas a)
			{
				return pool.ContainsKey(a.name);
			}

			void OnDestroy()
			{
				foreach (var pair in pool)
				{
					Destroy(pair.Value.gray);
				}
				pool.Clear();
				inst = null;
			}
		}

		public static UITexture ConvertToTexture(this UISprite s)
		{
			int depth = s.depth;
			int width = s.width;
			int height = s.height;
			UIBasicSprite.Flip flip = s.flip;
			UIBasicSprite.Type spriteType = s.type;
			GameObject obj = s.gameObject;

			s.DestroyEx();
			UITexture tex = obj.FindComponent<UITexture>();
			tex.depth = depth;
			tex.width = width;
			tex.height = height;
			tex.flip = flip;
			tex.type = spriteType;
			return tex;
		}

	}
}
