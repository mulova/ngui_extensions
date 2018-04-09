
using UnityEditor;
using UnityEngine;
using comunity;


namespace ngui.ex {
    public class UIGridWindow : TabbedEditorWindow {
		
		[MenuItem("NGUI/GridLayout")]
		public static void ShowWindow() {
			UIGridWindow win = EditorWindow.GetWindow<UIGridWindow>();
			win.titleContent = new GUIContent("Grid");
		}
		
		protected override void CreateTabs() {
			AddTab(new UIGridContentsTab(this));
			AddTab(new UIGridPropertyTab(this));
		}
	}
}
