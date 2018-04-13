using UnityEditor;
using UnityEngine;
using comunity;


namespace ngui.ex
{
    public class NGUIToolWindow : TabbedEditorWindow
    {
		
        [ExecuteInEditMode]
        [MenuItem ("NGUI/Tools")]
        public static void ShowWindow ()
        {
            EditorWindow win = EditorWindow.GetWindow (typeof(NGUIToolWindow));
			win.titleContent = new GUIContent("NGUITools");
        }

        protected override void CreateTabs ()
        {
            AddTab (new UIConvertTab (this), new UISpriteTab (this), new AtlasSwitcherTab(this), new AssetRefTab (this), new UIDepthTab (this), new EventDelegateTab (this), new UITextTab (this), new UIClassifierTab (this)
                #if NGUI_AUDIO
                , new UISoundTab (this)
                #endif
                );
        }
    }
}