using UnityEngine;
using System.Collections;
using UnityEditor;

using System.Collections.Generic;
using comunity;

namespace ngui.ex
{
    public class UIPanelExtMenu {
        
        [MenuItem("GameObject/UI/ScrollZone")]
        public static void AddScrollZone() {
            UIPanel panel = Selection.activeGameObject.GetComponent<UIPanel>();
            GameObject zone = null;
            Transform receiverTrans = panel.transform.Find(panel.name+"_zone");
            if (receiverTrans == null) {
                zone = new GameObject(panel.name+"_zone");
                zone.transform.parent = panel.transform;
                zone.transform.SetSiblingIndex(panel.transform.GetSiblingIndex()+1);
                zone.layer = panel.gameObject.layer;
            } else {
                zone = receiverTrans.gameObject;
            }
            UIDragScrollView drag = zone.GetComponentEx<UIDragScrollView>();
            drag.scrollView = panel.GetComponent<UIScrollView>();
            zone.GetComponentEx<ScrollZone>();
            CompatibilityEditor.SetDirty(zone);
            AddDragScrollView();
        }
        
        [MenuItem("GameObject/UI/ScrollZone", true)]
        public static bool IsAddScrollZone() {
            if (Selection.activeGameObject == null) {
                return false;
            }
            UIPanel panel = Selection.activeGameObject.GetComponent<UIPanel>();
            if (panel == null || panel.GetComponent<UIScrollView>() == null) {
                return false;
            }
            return panel.clipping != UIDrawCall.Clipping.None;
        }
        
        
        [MenuItem("GameObject/UI/Add all DragScrollView", true)]
        public static bool IsAddDragScrollView() {
            return IsAddScrollZone();
        }
        
        [MenuItem("GameObject/UI/Add all DragScrollView")]
        public static void AddDragScrollView() {
            UIScrollView view = Selection.activeGameObject.GetComponent <UIScrollView>();
            foreach (BoxCollider2D box in view.GetComponentsInChildren<BoxCollider2D>(true)) {
                UIDragScrollView drag = box.gameObject.GetComponentEx<UIDragScrollView>();
                drag.scrollView = view;
                CompatibilityEditor.SetDirty(drag);
                Debug.Log("Add DragScrollView for "+box.transform.GetScenePath(), box.gameObject);
            }
        }
    }
}
