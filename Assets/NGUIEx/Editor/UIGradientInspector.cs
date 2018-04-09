using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ngui.ex
{
    [CustomEditor(typeof(UIGradient))]
    public class UIGradientInspector : Editor
    {
        private UIGradient gradient;
        
        void OnEnable() {
            gradient = target as UIGradient;
            gradient.Refresh();
        }
        
        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector();
            if (GUI.changed) {
                gradient.Refresh();
            }
        }
        
    }
}
