using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using comunity;


namespace ngui.ex {
	[CustomEditor(typeof(UITableLayout))]
	public class UITableLayoutInspector : Editor {
		private SerializedInspector varInspector;
		private UITableLayoutInspectorImpl inspector;
		
		void OnEnable () {
			UITableLayout grid = (UITableLayout)target;
			inspector =  new UITableLayoutInspectorImpl(grid);
			inspector.OnEnable();
			SerializedObject obj = new SerializedObject(grid);
			varInspector = new SerializedInspector(obj, "gizmoColor");
		}
		
		override public void OnInspectorGUI() {
			if (GUILayout.Button("Open Contents Editor")) {
				EditorWindow window = EditorWindow.GetWindow(typeof(UITableWindow));
				window.titleContent = new GUIContent("Grid");
				window.Show(true);
			}
			inspector.OnInspectorGUI();
			varInspector.OnInspectorGUI();
		}
		
		[MenuItem("GameObject/UI/GridLayout")]
		public static void AddMenu() {
			if (Selection.activeGameObject == null) {
				EditorUtility.DisplayDialog("Error", "No Selection", "OK");
			} else {
				GameObject go = new GameObject("GridLayout");
				go.layer = Selection.activeGameObject.layer;
				go.AddComponent<UITableLayout>();
				Transform trans = go.transform;
				trans.parent = Selection.activeGameObject.transform;
				trans.localPosition = Vector3.zero;
				trans.localRotation = Quaternion.identity;
				trans.localScale = Vector3.one;
				Selection.activeGameObject = go;
			}
		}

		[MenuItem("GameObject/UI/Resize Collider %#&b")]
		public static void ResizeCollider() {
			NGUIUtil.UpdateCollider(Selection.activeGameObject.transform);
		}
		
		[MenuItem("GameObject/UI/Resize Collider %#&b", true)]
		public static bool IsResizeCollider() {
			return Selection.activeGameObject != null
				&& Selection.activeGameObject.GetComponent<BoxCollider>() != null
				&& Selection.activeGameObject.GetComponentInChildren<UIWidget>() != null;
		}
		
		[MenuItem("GameObject/UI/Align as LeftTop %#&o")]
		public static void SyncPositionLeftTop() {
			if (Selection.activeGameObject != null) {
				foreach (GameObject go  in Selection.gameObjects) {
					Transform parent = go.transform;
					Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(parent);
					float dx = bound.min.x;
					float dy = bound.max.y;
					foreach (Transform child in parent) {
						UIPivot.MovePosition(child, -dx, -dy, false);
					}
					UIPivot.MovePosition(parent, dx, dy, true);
				}
			}
		}
		
		[MenuItem("GameObject/UI/Align as LeftTop %#&o", true)]
		public static bool IsSyncPositionLeftTop() {
			return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
		}
		
		[MenuItem("GameObject/UI/Align as Center %#&p")]
		private static void SyncPositionCenter() {
			foreach (GameObject go in Selection.gameObjects) {
				Transform parent = go.transform;
				Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(parent);
				float dx = bound.min.x+bound.size.x/2;
				float dy = bound.max.y-bound.size.y/2;
				foreach (Transform child in parent) {
					UIPivot.MovePosition(child, -dx, -dy, false);
				}
				UIPivot.MovePosition(parent, dx, dy, true);
			}
		}
		
		[MenuItem("GameObject/UI/Align as Center %#&p", true)]
		public static bool IsSyncPositionCenter() {
			return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
		}

		[MenuItem("GameObject/UI/Align as LeftBottom %#&i")]
		public static void SyncPositionLeftBottom() {
			if (Selection.activeGameObject != null) {
				foreach (GameObject go  in Selection.gameObjects) {
					Transform parent = go.transform;
					Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(parent);
					float dx = bound.min.x;
					float dy = bound.min.y;
					foreach (Transform child in parent) {
						UIPivot.MovePosition(child, -dx, -dy, false);
					}
					UIPivot.MovePosition(parent, dx, dy, true);
				}
			}
		}
		
		[MenuItem("GameObject/UI/Align as LeftBottom %#&i", true)]
		public static bool IsSyncPositionLeftBottom() {
			return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
		}
	}
}