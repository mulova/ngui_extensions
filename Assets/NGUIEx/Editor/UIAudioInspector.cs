#if NGUI_AUDIO
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using audio;
using comunity;


namespace ngui.ex
{
    [CustomEditor(typeof(UIAudio))]
    public class UIAudioInspector : Editor
    {
        private UIAudio audio;
        private AudioKeyInspector keyInspector;
        
        void OnEnable() {
            audio = target as UIAudio;
            keyInspector = new AudioKeyInspector();
        }
        
        public override void OnInspectorGUI() {
            keyInspector.DrawGroup();
            EditorGUI.indentLevel++;
            bool changed = false;
            changed |= keyInspector.DrawKey("Press", ref audio.press);
            changed |= keyInspector.DrawKey("Release", ref audio.release);
            if (!Platform.platform.IsMobile()) {
                changed |= keyInspector.DrawKey("Hover Over", ref audio.hoverOver);
                changed |= keyInspector.DrawKey("Hover Out", ref audio.hoverOut);
            }
            changed |= keyInspector.DrawKey("Select", ref audio.select);
            changed |= keyInspector.DrawKey("Deselect", ref audio.deselect);
            changed |= keyInspector.DrawKey("Click", ref audio.click);
            changed |= keyInspector.DrawKey("Double Click", ref audio.doubleClick);
            EditorGUI.indentLevel--;
            if (changed) {
                CompatibilityEditor.SetDirty(audio);
            }
        }
    }
}
#endif