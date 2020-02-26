using UnityEditor;
using UnityEngine;
using UnityEngine.Ex;
using mulova.unicore;
using System.Ex;

namespace ngui.ex
{
    public class AnimClipInspector
    {
        private MonoBehaviour script;
        private string[] clipVars;
        
        public AnimClipInspector(MonoBehaviour script, params string[] clipVars) {
            this.script = script;
            this.clipVars = clipVars;
        }
        
        public bool OnInspectorGUI() {
            Animation anim = script.GetComponent<Animation>();
            bool changed = false;
            EditorGUI.indentLevel++;
            foreach (string varName in clipVars) {
                AnimationClip val = script.GetFieldValue<AnimationClip>(varName);
                if (anim != null) {
                    if (EditorGUILayoutUtil.PopupNullable(varName, ref val, anim.GetAllClips().ToArray())) {
                        script.SetFieldValue(varName, val);
                        changed = true;
                    }
                } else 
                {
                    if (val != null)
                    {
                        script.SetFieldValue<AnimationClip>(varName, null);
                        changed = true;
                    }
                }
            }
            EditorGUI.indentLevel--;
            if (changed) {
                EditorUtil.SetDirty(script);
            }
            return changed;
        }
    }
}
