using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System.Collections.Generic;
using mulova.commons;
using mulova.comunity;
using UnityEngine.Ex;
using mulova.unicore;

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
                AnimationClip val = ReflectionUtil.GetFieldValue<AnimationClip>(script, varName);
                if (anim != null) {
                    if (EditorGUILayoutUtil.PopupNullable(varName, ref val, anim.GetAllClips().ToArray())) {
                        ReflectionUtil.SetFieldValue(script, varName, val);
                        changed = true;
                    }
                } else 
                {
                    if (val != null)
                    {
                        ReflectionUtil.SetFieldValue<AnimationClip>(script, varName, null);
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
