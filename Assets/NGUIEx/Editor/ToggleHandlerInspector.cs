using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using commons;
using comunity;

namespace ngui.ex
{
    [CustomEditor(typeof(ToggleHandler))]
    public class ToggleHandlerInspector : Editor
    {
        private ToggleHandler handler;
        private ArrayDrawer<GameObject> toggleInspector;
        
        void OnEnable() {
            this.handler = target as ToggleHandler;
            toggleInspector = new ArrayDrawer<GameObject>(handler, "toggleObj");
            toggleInspector.onInsert += OnItemAdd;
            
            // convert OnClick -> OnButtonClick
            for (int i=0; i<toggleInspector.Count; ++i) {
                GameObject o = toggleInspector[i] as GameObject;
                if (o != null) {
                    UIToggle b = o.GetComponent<UIToggle>();
                    foreach (EventDelegate d in b.onChange) {
                        if (d.target == handler && d.methodName == "OnToggleChange") {
                            EventDelegate.Parameter[] p = d.parameters;
                            d.methodName = "OnToggleChange";
                            if (d.isValid) {
                                d.parameters[0] = p[0];
                            }
                            CompatibilityEditor.SetDirty(o);
                        }
                    }
                } else {
                    Debug.LogWarning(handler.name+".ToggleHandler has null arguments");
                }
            }
        }
        
        private Transform searchRoot;
        public override void OnInspectorGUI() {
            if (toggleInspector.Draw()) {
                InvalidateArray();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtil.ObjectField<Transform>(ref searchRoot, true);
            GUI.enabled = searchRoot != null;
            if (GUILayout.Button("Search")) {
                HashSet<GameObject> set = new HashSet<GameObject>(handler.toggleObj);
                foreach (UIToggle t in searchRoot.GetComponentsInChildren<UIToggle>()) {
                    if (!set.Contains(t.gameObject)) {
                        ArrayUtil.Add(ref handler.toggleObj, t.gameObject);
                    }
                }
                InvalidateArray();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
        
        private void InvalidateArray() {
            handler.InitToggles();
            CompatibilityEditor.SetDirty(handler);
            foreach (GameObject b in handler.toggleObj) {
                if (b != null) {
                    SetDirty(b.GetComponentEx<UIToggle>());
                    b.GetComponentEx<BoxCollider2D>();
                }
            }
        }
        
        private void SetDirty(UIToggle btn) {
            if (btn == null) {
                return;
            }
            CompatibilityEditor.SetDirty(btn);
            if (btn.onChange != null) {
                foreach (EventDelegate d in btn.onChange) {
                    if (d.parameters.IsNotEmpty() && d.parameters[0].obj != null) {
                        CompatibilityEditor.SetDirty(d.parameters[0].obj);
                    }
                }
            }
        }
        
        private void OnItemAdd(int i, Object obj) {
            if (obj == null) {
                return;
            }
            InvalidateArray();
        }
        
        public static void UpdateSpriteCollider (BoxCollider2D box, bool considerInactive)
        {
            if (box != null)
            {
                GameObject go = box.gameObject;
                SpriteRenderer r = go.GetComponent<SpriteRenderer>();
                
                if (r != null)
                {
                    Vector3 scale = go.transform.lossyScale;
                    Bounds region = r.bounds;
                    box.offset = go.transform.InverseTransformPoint(region.center);
                    float x = scale.x != 0? region.size.x/scale.x: 0;
                    float y = scale.y != 0? region.size.y/scale.y: 0;
                    box.size = new Vector3(x, y, 1);
                    #if UNITY_EDITOR
                    NGUITools.SetDirty(box);
                    #endif
                } else {
                    UIWidget w = go.GetComponent<UIWidget>();
                    
                    if (w != null)
                    {
                        Vector4 region = w.drawingDimensions;
                        box.offset = new Vector3((region.x + region.z) * 0.5f, (region.y + region.w) * 0.5f);
                        box.size = new Vector3(region.z - region.x, region.w - region.y);
                    }
                    else
                    {
                        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(go.transform, considerInactive);
                        box.offset = b.center;
                        box.size = new Vector3(b.size.x, b.size.y, 0f);
                    }
                    #if UNITY_EDITOR
                    NGUITools.SetDirty(box);
                    #endif
                }
            }
        }
    }
}