using UnityEngine;
using comunity;

namespace ngui.ex
{
	public static class UITextureEx
	{
		
		public static void ToGrayscale(this UITexture tex)
		{
			if (tex != null)
			{
				tex.material.SetFloat("_GreyStrength", 1);
			}
		}
		
		public static void ToNormalColor(this UITexture tex)
		{
			if (tex != null)
			{
				tex.material.SetFloat("_GreyStrength", 0);
			}
		}
		
		public static UISprite ConvertToSprite(this UITexture tex)
		{
			Vector3 pos = tex.transform.localPosition;
			GameObject obj = tex.gameObject;
			int depth = tex.depth;
			int width = tex.width;
			int height = tex.height;
			Vector4 border = tex.border;
			UIWidget.Pivot pivot = tex.pivot;
			string name = tex.mainTexture != null ? tex.mainTexture.name: string.Empty;
			UIBasicSprite.Type spriteType = tex.type;
			Color c = tex.color;
			UIBasicSprite.Flip flip = tex.flip;
			tex.DestroyEx();
			UISprite s = obj.GetComponentEx<UISprite>();
			s.type = spriteType;
			s.spriteName = name;
			s.depth = depth;
			s.width = width;
			s.height = height;
			s.flip = flip;
			s.color = c;
			s.pivot = pivot;
			s.GetAtlasSprite().borderLeft = (int)border.x;
			s.GetAtlasSprite().borderTop = (int)border.y;
			s.GetAtlasSprite().borderRight = (int)border.z;
			s.GetAtlasSprite().borderBottom = (int)border.w;
			s.transform.localPosition = pos;
			return s;
		}
	}
}

