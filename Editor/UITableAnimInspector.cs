#if FULL
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace ngui.ex {
	[CustomEditor(typeof(UITableAnim))]
	public class UITableAnimInspector : Editor {
		private UITableAnim anim;
		
		void OnEnable () {
			anim = (UITableAnim)target;
		}
		
		override public void OnInspectorGUI() {
			DrawDefaultInspector();
			if (anim.grid != null && Application.isPlaying) {
				if (GUILayout.Button("Begin")) {
					anim.BeginAnimation();
				}
			}
		}
		
	}
}
#endif