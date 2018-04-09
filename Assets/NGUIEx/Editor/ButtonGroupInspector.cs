using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System.Collections.Generic;
using comunity;

namespace ngui.ex
{
    [CustomEditor(typeof(ButtonGroup))]
    public class ButtonGroupInspector : Editor
    {
        private ButtonGroup group;
        private ArrayDrawer<UIButton> buttonInspector;
        
        void OnEnable() {
            this.group = target as ButtonGroup;
            buttonInspector = new ArrayDrawer<UIButton>(group, "buttons");
        }
        
        public override void OnInspectorGUI() {
            buttonInspector.Draw();
        }
    }
}