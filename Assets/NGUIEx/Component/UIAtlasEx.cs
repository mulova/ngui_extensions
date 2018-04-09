using UnityEngine;
using System.Text;
using System;

using Object = UnityEngine.Object;

public static class UIAtlasEx
{

	public static UIAtlas Clone(this UIAtlas a, string shaderName)
	{
		UIAtlas b = Object.Instantiate<UIAtlas>(a);
		Object.DontDestroyOnLoad(b.gameObject);
		if (b.replacement != null)
		{
			b.replacement = Object.Instantiate<UIAtlas>(b.replacement);
			Object.DontDestroyOnLoad(b.replacement);
		}
		b.spriteMaterial = Object.Instantiate<Material>(b.spriteMaterial);
		Object.DontDestroyOnLoad(b.spriteMaterial);
		b.spriteMaterial.shader = Shader.Find(shaderName);
		return b;
	}
}
