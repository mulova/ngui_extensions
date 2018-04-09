using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using comunity;

namespace ngui.ex {
	[CustomEditor(typeof(RenderQueue))]
	public class RenderQueueInspector : Editor {
		
		private RenderQueue rq;
		private RenderQueueArrInspector inspector;
		
		void OnEnable() {
			rq = (RenderQueue)target;
			inspector = new RenderQueueArrInspector(rq, "row");
			inspector.Sort();
			showZ = rq.zScale != 0;
		}
		
		private bool showZ;
		public override void OnInspectorGUI() {
			if (inspector.OnInspectorGUI()) {
				rq.Optimize();
			}
			showZ = EditorGUILayout.BeginToggleGroup("Z Transform", showZ);
			if (showZ) {
				EditorGUI.indentLevel += 2;
				EditorGUIUtil.FloatField("Z Base", ref rq.zBase);
				EditorGUIUtil.FloatField("Z Scale", ref rq.zScale);
				EditorGUI.indentLevel -= 2;
			}
			EditorGUILayout.EndToggleGroup();
			Dictionary<string, bool> nameSet = new Dictionary<string, bool>();
			Dictionary<int, bool> rqSet = new Dictionary<int, bool>();
			foreach (RenderQueueElement e in rq.row) {
				if (string.IsNullOrEmpty(e.name)) {
					continue;
				}
				if (nameSet.ContainsKey(e.name)) {
					EditorGUILayout.HelpBox("Duplicate "+e.name, MessageType.Error);
					break;
				} else if (rqSet.ContainsKey(e.value)) {
					EditorGUILayout.HelpBox("Duplicate "+e.value, MessageType.Error);
					break;
				}
				nameSet.Add(e.name, true);
				rqSet.Add(e.value, true);
			}
		}
	}
	
	public class RenderQueueArrInspector : ArrInspector<RenderQueueElement>
	{
		public RenderQueueArrInspector(Object obj, string varName) : base(obj, varName) { 
			SetTitle(null);
		}
		
		protected override bool OnInspectorGUI(RenderQueueElement info, int i)
		{
			float width = GetWidth();
			bool changed = false;
			changed |= EditorGUIUtil.TextField(null, ref info.name, GUILayout.MinWidth(width*0.5F));
			changed |= EditorGUIUtil.IntField(null, ref info.value, GUILayout.MinWidth(width*0.3F));
			return changed;
		}

		protected override bool DrawFooter() {
			return false;
		}
	}
	
}