using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System;
using commons;
using comunity;

namespace ngui.ex
{
    [CustomEditor(typeof(ButtonHandler))]
    public class ButtonHandlerInspector : Editor
    {
        private ButtonHandler handler;
        private ArrayDrawer<GameObject> drawer;
        
        void OnEnable()
        {
            this.handler = target as ButtonHandler;
            this.drawer = new ArrayDrawer<GameObject>(handler, "buttons");
            drawer.onInsert += OnInsert;
            drawer.allowSceneObject = true;
            drawer.allowSelection = false;
            
            // check validation
            Action<GameObject> callback = handler.OnButtonClick;
            string callbackName = callback.Method.Name;
            bool changed = false;
            foreach (GameObject o in handler.buttons)
            {
                if (o == null)
                {
                    Debug.LogWarning("There is null element in "+handler.transform.GetScenePath(), handler);
                } else
                {
                    UIButton button = o.GetComponent<UIButton>();
                    if (button == null 
                        ||button.onClick.IsEmpty() 
                        ||button.onClick[0].methodName != callbackName
                        ||button.onClick[0].parameters.IsEmpty()
                        ||button.onClick[0].parameters[0].obj != button.gameObject)
                    {
                        EventDelegateUtil.SetCallback(button.onClick, callback, button.gameObject);
                        EditorUtil.SetDirty(button.gameObject);
                        changed = true;
                        //              EditorGUILayout.HelpBox(o.name+" is invalid", MessageType.Error);
                    }
                }
            }
            if (changed)
            {
                AssetDatabase.SaveAssets();
            }
        }
        
        private Transform searchRoot;

        public override void OnInspectorGUI()
        {
            drawer.Draw();
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtil.ObjectField<Transform>(ref searchRoot, true);
            GUI.enabled = searchRoot != null;
            if (!Application.isPlaying&&GUILayout.Button("Search"))
            {
                HashSet<GameObject> set = new HashSet<GameObject>(handler.buttons);
                foreach (UIButton b in searchRoot.GetComponentsInChildren<UIButton>())
                {
                    if (!set.Contains(b.gameObject))
                    {
                        ArrayUtil.Add(ref handler.buttons, b.gameObject);
                    }
                }
                InvalidateArray();
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
        
        private void InvalidateArray()
        {
            EditorUtil.SetDirty(handler);
            foreach (GameObject o in handler.buttons)
            {
                if (o != null)
                {
                    UIButton btn = o.GetComponent<UIButton>();
                    if (btn == null)
                    {
                        btn = o.AddComponent<UIButton>();
                        if (o.GetComponent<Collider2D>() == null)
                        {
                            BoxCollider2D col = o.AddComponent<BoxCollider2D>();
                            col.isTrigger = true;
                            NGUITools.UpdateWidgetCollider(col, false);
                        }
                    }
                    SetDirty(btn);
                }
            }
        }
        
        private void SetDirty(UIButton btn)
        {
            if (btn == null)
            {
                return;
            }
            EditorUtil.SetDirty(btn);
            if (btn.onClick != null)
            {
                foreach (EventDelegate d in btn.onClick)
                {
                    if (d.parameters.IsNotEmpty()&&d.parameters[0].obj != null)
                    {
                        EditorUtil.SetDirty(d.parameters[0].obj);
                    }
                }
            }
        }
        
        private void OnInsert(int i, GameObject o)
        {
            if (o == null)
            {
                return;
            }
            UIButton btn = o.GetComponentEx<UIButton>();
            EventDelegate d = GetCallback(btn.onClick, handler, handler.OnButtonClick);
            if (d == null)
            {
                EventDelegateUtil.AddCallback(btn.onClick, handler.OnButtonClick, btn.gameObject);
                if (o.GetComponent<Collider2D>() == null)
                {
                    BoxCollider2D collider = o.AddComponent<BoxCollider2D>();
                    if (o.GetComponent<SpriteRenderer>() != null)
                    {
                        UpdateSpriteCollider(collider, false);
                    } else
                    {
                        NGUITools.UpdateWidgetCollider(collider, false);
                    }
                }
            }
            EditorUtil.SetDirty(o);
        }

        private void OnItemAdd(Object o, int i)
        {
            if (o == null)
            {
                return;
            }
            GameObject obj = o as GameObject;
            UIButton btn = obj.GetComponentEx<UIButton>();
            EventDelegate d = GetCallback(btn.onClick, handler, handler.OnButtonClick);
            if (d == null)
            {
                EventDelegateUtil.AddCallback(btn.onClick, handler.OnButtonClick, btn.gameObject);
                if (obj.GetComponent<Collider2D>() == null)
                {
                    BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
                    if (obj.GetComponent<SpriteRenderer>() != null)
                    {
                        UpdateSpriteCollider(collider, false);
                    } else
                    {
                        NGUITools.UpdateWidgetCollider(collider, false);
                    }
                }
            }
            EditorUtil.SetDirty(obj);
        }
        
        private static EventDelegate GetCallback(List<EventDelegate> callbackList, MonoBehaviour target, Action<GameObject> method)
        {
            if (callbackList == null)
            {
                return null;
            }
            foreach (EventDelegate d in callbackList)
            {
                if ((d.target == target||d.target == null)&&d.methodName == method.Method.Name)
                {
                    return d;
                }
            }
            return null;
        }
        
        public static void UpdateSpriteCollider(BoxCollider2D box, bool considerInactive)
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
                    float x = scale.x != 0? region.size.x / scale.x : 0;
                    float y = scale.y != 0? region.size.y / scale.y : 0;
                    box.size = new Vector3(x, y, 1);
                    #if UNITY_EDITOR
                    NGUITools.SetDirty(box);
                    #endif
                } else
                {
                    UIWidget w = go.GetComponent<UIWidget>();
                    
                    if (w != null)
                    {
                        Vector4 region = w.drawingDimensions;
                        box.offset = new Vector3((region.x+region.z) * 0.5f, (region.y+region.w) * 0.5f);
                        box.size = new Vector3(region.z-region.x, region.w-region.y);
                    } else
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