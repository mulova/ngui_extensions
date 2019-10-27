using UnityEngine;
using System.Collections.Generic;
using ngui.ex;
using Flip = UIBasicSprite.Flip;
using System;

namespace ngui.ex
{
	[ExecuteAlways]
	[RequireComponent(typeof(UIBasicSprite))]
	public class UIGradient : MonoBehaviour
	{
		private UIBasicSprite sprite;
		public UITableLayout.Arrangement orientation = UITableLayout.Arrangement.Vertical;
		public GradientColor[] colors = new GradientColor[] { new GradientColor() };
		public string colorId;
		
		void OnEnable() 
		{
			sprite = GetComponent<UIBasicSprite>();
			if (sprite != null)
			{
				sprite.onPostFill = SetGradient;
			}
		}
		
		void OnDisable()
		{
			if (sprite != null)
			{
				sprite.onPostFill = null;
			}
		}
		
		public void SetColor(string id) {
			this.colorId = id;
			Refresh();
		}
		
		public void SetColor(int index) {
			this.colorId = colors[index].id;
			Refresh();
		}
		
		public GradientColor GetColor(string id) {
			foreach (GradientColor c in colors) {
				if (c.id == id) {
					return c;
				}
			}
			return null;
		}
		
		private static Color[] tempColor = new Color[4];
		void SetGradient(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
		{
			GradientColor c = GetColor(colorId);
			if (c == null) {
				return;
			}
			Color c1 = c.color1;
			Color c2 = c.color2;
			
			UIBasicSprite.Flip flip = (widget as UIBasicSprite).flip;
			
			if (flip == Flip.Both
				|| (flip == Flip.Horizontally && orientation == UITableLayout.Arrangement.Horizontal)
				|| (flip == Flip.Vertically && orientation == UITableLayout.Arrangement.Vertical))
			{
				Color temp = c1;
				c1 = c2;
				c2 = temp;
			}
			
			if (orientation == UITableLayout.Arrangement.Horizontal) {
				tempColor[0] = c1;
				tempColor[1] = c1;
				tempColor[2] = c2;
				tempColor[3] = c2;
			} else {
				tempColor[0] = c2;
				tempColor[1] = c1;
				tempColor[2] = c1;
				tempColor[3] = c2;
			}
			
            if (verts.Count == 4) {
				Fill(cols, tempColor[0], tempColor[1], tempColor[2], tempColor[3], 0, 0);
            } else if (verts.Count == 36) {
				if (orientation == UITableLayout.Arrangement.Horizontal) {
					for (int y = 0; y < 3; ++y)
					{
						//					if (centerType == AdvancedType.Invisible && x == 1 && y == 1) continue;
						Fill(cols, c1, 0, y);
						Fill(cols, tempColor[0], tempColor[1], tempColor[2], tempColor[3], 1, y);
						Fill(cols, c2, 2, y);
					}
				} else {
					for (int x = 0; x < 3; ++x)
					{
						//					if (centerType == AdvancedType.Invisible && x == 1 && y == 1) continue;
						Fill(cols, c2, x, 0);
						Fill(cols, tempColor[0], tempColor[1], tempColor[2], tempColor[3], x, 1);
						Fill(cols, c1, x, 2);
					}
				}
			}
			widget.Invalidate(false);
		}
		
		private void Fill (List<Color> cols, Color c1, int x, int y) {
			Fill(cols, c1, c1, c1, c1, x, y);
		}
		
		private void Fill (List<Color> cols, Color c1, Color c2, Color c3, Color c4, int x, int y)
		{
			int i = x*3*4+y*4;
			cols[i] *= c1;
			cols[i+1] *= c2;
			cols[i+2] *= c3;
			cols[i+3] *= c4;
		}
		
		public void Refresh() {
			if (sprite != null) {
				sprite.Invalidate(false);
			}
		}
		
		[Serializable]
		public class GradientColor {
			public string id;
			public Color color1 = Color.white;
			public Color color2 = Color.white;
			
			public override string ToString ()
			{
				return id;
			}
		}
	}
	
}