using UnityEngine;
using System.Text;
using System;
using Object = UnityEngine.Object;

namespace ngui.ex
{
	public static class UIAtlasEx
	{
		
		public static NGUIAtlas Clone(this INGUIAtlas a, string shaderName)
		{
			NGUIAtlas b = Object.Instantiate(a.origin());
			Object.DontDestroyOnLoad(b);
			if (b.replacement != null)
			{
				b.replacement = Object.Instantiate(b.origin());
				Object.DontDestroyOnLoad(b.origin());
			}
			b.spriteMaterial = Object.Instantiate(b.spriteMaterial);
			Object.DontDestroyOnLoad(b.spriteMaterial);
			b.spriteMaterial.shader = Shader.Find(shaderName);
			return b;
		}
	}
}
