using UnityEditor;
using UnityEngine;

namespace ngui.ex {
	//[CustomEditor(typeof(UIPivot))]
	public class UIPivotInspector : Editor {
		
		private UIPivot pivot;
		void OnEnable() {
			pivot = (UIPivot)target;
			pivot.Reposition();
		}
		
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if (GUI.changed) {
				pivot.Reposition();
			}
		}
	}
}