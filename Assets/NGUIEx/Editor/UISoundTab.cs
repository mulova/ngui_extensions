using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using comunity;
using commons;

namespace ngui.ex
{
    public class UISoundTab : EditorTab
    {

        private static bool locked = true;
        private Vector2 scroll;
        private GameObject[] roots;
        private AudioTriggerInspectorImpl missingTriggerInspector;

        public UISoundTab (TabbedEditorWindow window) : base ("Sound", window)
        {
        }

        public override void OnEnable ()
        {
            missingTriggerInspector = new AudioTriggerInspectorImpl ();
            missingTriggerInspector.showLabel = false;
        }

        public override void OnDisable ()
        {
            SaveChange (changedList);
            changedList.Clear ();
        }

        public override void OnChangePlayMode ()
        {
        }

        public override void OnChangeScene (string sceneName)
        {
            Clear ();
        }

        public override void OnFocus (bool focus)
        {
        }

        public override void OnSelected (bool sel)
        {
        }

        private void Clear ()
        {
            roots = null;
            triggers = new AudioTriggerInspectorImpl[0];
            changedList.Clear ();
        }

        public override void OnHeaderGUI ()
        {
        }

        public override void OnInspectorGUI ()
        {
            scroll = EditorGUILayout.BeginScrollView (scroll);
            try {
                DrawAudioTriggers ();
            } catch (Exception ex) {
                Debug.LogException (ex);
            }
            EditorGUILayout.EndScrollView ();
        }

        public override void OnFooterGUI ()
        {
        }

        private AudioTriggerInspectorImpl[] triggers = new AudioTriggerInspectorImpl[0];
        private HashSet<GameObject> changedList = new HashSet<GameObject> ();
        private AudioDataTable missingTable;
        private string missingClip;

        private void DrawAudioTriggers ()
        {
            EditorGUIUtil.Toggle ("Lock", ref locked);
            // set missing triggers
            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.PrefixLabel ("Missing");
            if (missingTriggerInspector.DrawInspectorGUI (ref missingTable, ref missingClip) && missingClip.IsNotEmpty ()) {
                foreach (AudioTriggerInspectorImpl i in triggers) {
                    if (i.trigger.audioGroupGuid.IsEmpty () || i.trigger.clip.IsEmpty ()) {
                        i.SelectTable(missingTable);
                        i.trigger.clip = missingClip;
                        CompatibilityEditor.SetDirty(i.trigger);
                        changedList.Add (i.trigger.gameObject);
                    }
                }
            }
            EditorGUILayout.EndHorizontal ();
            EditorGUIUtil.DrawSeparator ();
            // Search AudioTriggers
            if ((roots.IsEmpty () || !locked) && Selection.gameObjects.IsNotEmpty () && !Array.Equals (roots, Selection.gameObjects)) {
                Clear ();
                roots = Selection.gameObjects;
                List<AudioTrigger> audio = new List<AudioTrigger> ();
                foreach (GameObject o in roots) {
                    foreach (UIButton btn in o.GetComponentsInChildren<UIButton>(true)) {
                        AudioTrigger[] t = btn.GetComponents<AudioTrigger> ();
                        if (t.IsEmpty ()) {
                            t = new AudioTrigger[] { btn.gameObject.AddComponent<AudioTrigger> () };
                            changedList.Add (btn.gameObject);
                            CompatibilityEditor.SetDirty(btn.gameObject);
                        }
                        audio.AddRange (t);
                    }
                }
                AudioDataTable[] tables = AudioTriggerInspectorImpl.LoadTables ();
                triggers = new AudioTriggerInspectorImpl[audio.Count];
                for (int i = 0; i < audio.Count; ++i) {
                    triggers [i] = new AudioTriggerInspectorImpl (audio [i], tables);
                    triggers [i].showLabel = false;
                }
            }

            // draw trigger list
            EditorGUILayout.BeginVertical ();
            foreach (AudioTriggerInspectorImpl a in triggers) {
                EditorGUILayout.BeginHorizontal ();
                EditorGUILayout.ObjectField (a.trigger.gameObject, typeof(GameObject), true);
                if (a.DrawInspectorGUI ()) {
                    changedList.Add (a.trigger.gameObject);
                }
                EditorGUILayout.EndHorizontal ();
            }
            EditorGUILayout.EndVertical ();

            if (GUILayout.Button ("Save")) {
                SaveChange (changedList);
                changedList.Clear ();
            }
        }
    }
}
