using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using commons;
using Assert = commons.Assert;
using comunity;


public static class UISpriteEx
{
	public static Dictionary<string, GrayAtlas> grayAtlas = new Dictionary<string, GrayAtlas>();
	public static Dictionary<string, LightAtlas> lightAtlas = new Dictionary<string, LightAtlas>();

	public static void ToGrayscale(this UISprite s)
	{
		if (s == null||s.atlas == null)
		{
			Assert.IsTrue(false);
			return;
		}
		GrayAtlas a = grayAtlas.Get(s.atlas.name);
		if (a == null)
		{
			a = new GrayAtlas(s.atlas);
			grayAtlas[s.atlas.name] = a;
		}
		s.atlas = a.gray;
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
			s.atlas = a.orig;
		}
	}

	public static void ToLightscale(this UISprite s)
	{
		if (s == null||s.atlas == null)
		{
			Assert.IsTrue(false);
			return;
		}
		LightAtlas a = lightAtlas.Get(s.atlas.name);
		if (a == null)
		{
			a = new LightAtlas(s.atlas);
			lightAtlas[s.atlas.name] = a;
		}
		s.atlas = a.light;
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
        UITexture tex = obj.GetComponentEx<UITexture>();
        tex.depth = depth;
        tex.width = width;
        tex.height = height;
        tex.flip = flip;
        tex.type = spriteType;
        return tex;
    }

	public class GrayAtlas
	{
		public UIAtlas orig;
		public UIAtlas gray;

		public GrayAtlas(UIAtlas a)
		{
			this.orig = a;
			this.gray = a.Clone(orig.spriteMaterial.shader.name);
			this.gray.spriteMaterial.SetFloat("_GreyStrength", 1);
			this.gray.name = a.name;
		}
	}

	public class LightAtlas
	{
		public UIAtlas orig;
		public UIAtlas light;

		public LightAtlas(UIAtlas a)
		{
            this.orig = a;
            string origShader = a.spriteMaterial.shader.name;
            if (origShader == ShaderId.UI_VERT)
            {
                this.light = a.Clone(ShaderId.UI_VERT_LIGHT);
            } else if (origShader == ShaderId.UI_SPLIT)
			{
                this.light = a.Clone(ShaderId.UI_SPLIT_LIGHT);
            } else
            {
                this.light = a.Clone(ShaderId.UI_LIGHT);
            }
            this.light.name = a.name;
		}
	}
}
