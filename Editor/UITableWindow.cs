
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace ngui.ex
{
    public class UITableWindow : TabbedEditorWindow {
		
		[MenuItem("NGUI/ex/TableLayout")]
		public static void ShowWindow() {
			UITableWindow win = EditorWindow.GetWindow<UITableWindow>();
			win.titleContent = new GUIContent("Table");
		}
		
		protected override void CreateTabs() {
			AddTab(new UITableContentsTab(this));
			AddTab(new UITablePropertyTab(this));
		}
	}
}
