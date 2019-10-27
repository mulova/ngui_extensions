using UnityEngine;
using System.Collections.Generic;
using ngui.ex;
using Flip = UIBasicSprite.Flip;
using System;

namespace ngui.ex
{
	[ExecuteAlways]
	[RequireComponent(typeof(UIBasicSprite))]
	public class UIBorderExtender : MonoBehaviour
	{
		public Vector2 centerSize;
		
		private UIBasicSprite sprite;

		void OnEnable()
		{
			sprite = GetComponent<UIBasicSprite>();
			if (sprite != null)
			{
				sprite.onPostFill = Resize;
			}
		}

		void OnDisable()
		{
			if (sprite != null)
			{
				sprite.onPostFill = null;
			}
		}

		void Resize(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
		{
			if (sprite.type == UIBasicSprite.Type.Sliced)
			{
				SlicedFill(widget, bufferOffset, verts, uvs, cols);
				widget.Invalidate(false);
			}
		}

		/// <summary>
		/// Sliced sprite fill function is more complicated as it generates 9 quads instead of 1.
		/// </summary>
		static protected Vector2[] mTempPos = new Vector2[4];

        void SlicedFill(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
		{
			if (sprite.border.x+sprite.border.z == 0||sprite.border.y+sprite.border.w == 0)
			{
				return;
			}
			verts.Clear();
			Vector2 center = centerSize != Vector2.zero? 
				centerSize : new Vector2(sprite.mainTexture.width-sprite.border.x-sprite.border.z, sprite.mainTexture.height-sprite.border.y-sprite.border.w); 
			
			Vector4 v = sprite.drawingDimensions;
			float width = (v.z-v.x)-center.x;
			float height = (v.w-v.y)-center.y;
			
			mTempPos[0].x = v.x;
			mTempPos[0].y = v.y;
			mTempPos[3].x = v.z;
			mTempPos[3].y = v.w;
			
			
			mTempPos[1].x = mTempPos[0].x+width * (sprite.border.x / (sprite.border.x+sprite.border.z));
			mTempPos[2].x = mTempPos[3].x-width * (sprite.border.z / (sprite.border.x+sprite.border.z));
			
			mTempPos[1].y = mTempPos[0].y+height * (sprite.border.y / (sprite.border.y+sprite.border.w));
			mTempPos[2].y = mTempPos[3].y-height * (sprite.border.w / (sprite.border.y+sprite.border.w));
			
			for (int x = 0; x < 3; ++x)
			{
				int x2 = x+1;
				
				for (int y = 0; y < 3; ++y)
				{
					if (x == 1&&y == 1&&centerSize != Vector2.zero)
						continue;
					
					int y2 = y+1;
					
					verts.Add(new Vector3(mTempPos[x].x, mTempPos[y].y));
					verts.Add(new Vector3(mTempPos[x].x, mTempPos[y2].y));
					verts.Add(new Vector3(mTempPos[x2].x, mTempPos[y2].y));
					verts.Add(new Vector3(mTempPos[x2].x, mTempPos[y].y));
				}
			}
		}

		public void Refresh()
		{
			if (sprite != null)
			{
				sprite.Invalidate(false);
			}
		}
		
		#if UNITY_EDITOR
		private Vector2 oldCenterSize;

		void Update()
		{
			if (Application.isPlaying)
			{
				return;
			}
			if (oldCenterSize != centerSize)
			{
				oldCenterSize = centerSize;
				Refresh();
			}
		}
		#endif
	}
	
}