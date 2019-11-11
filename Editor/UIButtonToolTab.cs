using System.Collections.Generic;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ngui.ex
{
    public class UIButtonToolTab : EditorTab {
		
		public UIButtonToolTab(TabbedEditorWindow window) : base("Button", window) {}

		public override void OnEnable() {
			Refresh();
		}
		
		public override void OnDisable() { }
		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnFocus(bool focus) { }
		public override void OnSelected(bool sel) { }
		
		private void Refresh() {
		}
		
		public override void OnHeaderGUI() { }

		private Object texFolder;
		private UIAtlas atlas;
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Tab")) {
				SetTab();
			}
			if (GUILayout.Button("Set Button Text Color")) {
				SetButtonTextColor();
			}
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Clear TexSetter")) {
				TexSetterInspector.ClearAllTextures();
			}
			if (GUILayout.Button("Set TexSetter")) {
				TexSetterInspector.SetAllTextures();
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void SetButtonTextColor() {
			foreach (GameObject o in Selection.gameObjects) {
				UIButton btn = o.GetComponent<UIButton>();
				if (btn != null) {
					btn.SetButtonTextColor();
					EditorUtil.SetDirty(btn.gameObject);
				}
			}
		}

		private static Color ACTIVE_TAB_COLOR = Color.white;
		private static Color INACTIVE_TAB_COLOR = new Color32(110, 128, 124, 255);
		private List<UI2DSprite> widgets;
		private void SetTab() {
			foreach (GameObject o in Selection.gameObjects) {
				UIButton tab = null;
				UITab tabRoot = o.GetComponent<UITab>();
				if (tabRoot != null) {
					tab = tabRoot.tabButton;
				} else {
					tab = o.GetComponent<UIButton>();
				}
				if (tab != null) {
					tab.normalSprite = "tab_on";
					tab.hoverSprite = null;
					tab.pressedSprite = null;
					tab.disabledSprite = null;
					UIButtonColor[] colors = tab.GetComponents<UIButtonColor>();
					if (colors.Length < 2) {
						UIButtonColor c = tabRoot.tabButton.gameObject.AddComponent<UIButtonColor>();
						UILabel label = tabRoot.tabButton.GetComponentInChildren<UILabel>();
						c.tweenTarget = label.gameObject;
					}
					foreach (UIButtonColor c in tab.GetComponents<UIButtonColor>()) {
						c.tweenTarget.GetComponent<UIWidget>().color = INACTIVE_TAB_COLOR;
						c.hover = INACTIVE_TAB_COLOR;
						c.pressed = INACTIVE_TAB_COLOR;
						c.disabledColor = ACTIVE_TAB_COLOR;
					}
					EditorUtil.SetDirty(tab.gameObject);
					
				}
			}
		}
		
		public override void OnFooterGUI() {
		}
	}
}