using System.Collections.Generic;
using UnityEngine;
using System;

namespace ngui.ex
{
	[ExecuteAlways]
	public class UILine : MonoBehaviour
	{
		public UIWidget widget;
		
		private Vector2 begin;
		private Vector3 end;
		
		void OnEnable() 
		{
			if (widget != null)
			{
				widget.onPostFill = SetGradient;
			}
		}
		
		void OnDisable()
		{
			if (widget != null)
			{
				widget.onPostFill = SetGradient;
			}
		}
		
		public void SetPosition(Vector2 begin, Vector3 end)
		{
			this.begin = begin;
			this.end = end;
		}
		
		private void SetGradient(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
		{
			// divide left, right verts
			Vector3[] v = new Vector3[bufferOffset];
			int[] index = new int[bufferOffset];
			for (int i=0; i<v.Length; ++i)
			{
				v[i] = verts[i];
				index[i] = i;
			}
			Array.Sort<Vector3, int>(v, index, new VertComparer());
			
			// left-half
			for (int i=0; i<bufferOffset/2; ++i)
			{
				v[index[i]].x = begin.x;
				v[index[i]].y += begin.y;
			}
			// right-half
			for (int i=bufferOffset/2; i<bufferOffset; ++i)
			{
				v[index[i]].x = end.x;
				v[index[i]].y += end.y;
			}
		}
		
		private class VertComparer : IComparer<Vector3>
		{
			public int Compare (Vector3 x, Vector3 y)
			{
				if (x.x<y.x)
				{
					return -1;
				} else if (x.x == y.x)
				{
					return 0;
				} else
				{
					return 1;
				}
			}
		}
	}
	
}