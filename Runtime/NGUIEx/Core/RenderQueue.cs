using UnityEngine;
using System.Collections.Generic;
using System;
using mulova.commons;
using System.Text.Ex;

namespace ngui.ex
{
	public class RenderQueue : ScriptableObject
	{
		public RenderQueueElement[] row;
		public float zBase;
		public float zScale;
		private Dictionary<string, int> map;
		
		
		public int this[string n] {
			get {
				return row[GetIndex(n)].value;
			}
		}
		
		private int GetIndex(string n) {
			if (map == null) {
				Optimize();
			}
			int i = -1;
			if (map.TryGetValue(n, out i)) {
				return i;
			}
			return -1;
		}
		
		/**
		* rebuild names-values map
		*/
		public void Optimize() {
			if (map == null) {
				map = new Dictionary<string, int>(row.Length*3);
			} else {
				map.Clear();
			}
			for (int i=0; i<row.Length; i++) {
				if (!row[i].name.IsEmpty()) {
					map[row[i].name] = i;
				}
			}
		}
		
		public string GetName(int i) {
			if (i<0 || i>= row.Length) {
				return null;
			}
			return row[i].name;
		}
		
		public int GetValue(int i) {
			if (i<0 || i>= row.Length) {
				return -1;
			}
			return row[i].value;
		}
		
		public void Add(string materialName, int rq) {
			Array.Resize<RenderQueueElement>(ref row, row.Length+1);
			row[row.Length-1] = new RenderQueueElement(materialName, rq);
			Optimize();
		}
		
		public bool IsZFeasible() {
			return zScale != 0;
		}
		public float GetZ(string name) {
			int rq = this[name];
			return (zBase+rq)*zScale;
		}
		
		public string[] GetNames() {
			string[] names = new string[row.Length];
			for (int i=0; i<names.Length; i++) {
				names[i] = row[i].name;
			}
			return names;
		}
	}
}

[System.Serializable]
public class RenderQueueElement : ICloneable, IComparable<RenderQueueElement> {
	public string name;
	public int value;
	
	public RenderQueueElement() {}
	
	public RenderQueueElement(string name, int value) {
		this.name = name;
		this.value = value;
	}

	public object Clone ()
	{
		return new RenderQueueElement(name, value);
	}

	public int CompareTo(RenderQueueElement other)
	{
		if (other == null) {
			return -1;
		}
		return this.value - other.value;
	}
}