#if FULL
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace ngui.ex {
	[CustomEditor(typeof(UIGridAnim))]
	public class UIGridAnimInspector : Editor {
		private UIGridAnim anim;
		
		void OnEnable () {
			anim = (UIGridAnim)target;
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