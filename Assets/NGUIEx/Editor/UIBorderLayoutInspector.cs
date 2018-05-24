using UnityEngine;
using System.Collections;
using UnityEditor;
using comunity;


namespace ngui.ex {
	[CustomEditor(typeof(UIBorderLayout))]
	public class UIBorderLayoutInspector : Editor {
		private SerializedInspector varInspector;
		private UIBorderLayout layout;
		
		[MenuItem("GameObject/UI/BorderLayout")]
		public static void AddMenu() {
			if (Selection.activeGameObject == null) {
				EditorUtility.DisplayDialog("Error", "No Selection", "OK");
			} else {
				GameObject go = new GameObject("BorderLayout");
				go.AddComponent<UIBorderLayout>();
				Transform trans = go.transform;
				trans.parent = Selection.activeGameObject.transform;
				trans.localPosition = Vector3.zero;
				trans.localRotation = Quaternion.identity;
				trans.localScale = Vector3.one;
				Selection.activeGameObject = go;
			}
		}
		
		void OnEnable() {
			this.layout = (UIBorderLayout)target;
			this.layout.InvalidateLayout();
			SerializedObject obj = new SerializedObject(target);
			varInspector = new SerializedInspector(obj, "top", "center", "bottom", "left", "right", "width", "height", "pivot");
			NGUIUtil.Reposition(layout.transform);
		}
		
		override public void OnInspectorGUI() {
			bool changed = varInspector.OnInspectorGUI();
			
			if (changed) {
				NGUIUtil.DisableAnchor(layout.top);
				NGUIUtil.DisableAnchor(layout.center);
				NGUIUtil.DisableAnchor(layout.bottom);
				NGUIUtil.DisableAnchor(layout.left);
				NGUIUtil.DisableAnchor(layout.right);
				layout.InvalidateLayout();
				EditorUtil.SetDirty(target);
			}
		}
	}
}
