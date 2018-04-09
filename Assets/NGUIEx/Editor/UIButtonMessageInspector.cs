using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using comunity;


namespace ngui.ex
{

    //[CustomEditor(typeof(UIButtonMessage))]
    public class UIButtonMessageInspector : Editor
    {
        private UIButtonMessage message;
        private SerializedInspector inspector;
        
        //  public GameObject target;
        //  public string functionName;
        //  public MethodRef methodRef;
        //  public Trigger trigger = Trigger.OnClick;
        //  public bool includeChildren = false;
        
        void OnEnable() {
            message = (UIButtonMessage)target;
            inspector = new SerializedInspector(new SerializedObject(message), 
                "target", "functionName", "method", "trigger", "includeChildren");
        }
        
        //  public GameObject target;
        //  public string functionName;
        //  public MethodRef methodRef;
        //  public Trigger trigger = Trigger.OnClick;
        //  public bool includeChildren = false;
        
        
        public override void OnInspectorGUI ()
        {
            inspector.Begin();
            if (inspector["method"].objectReferenceValue == null) {
                inspector.OnInspectorGUI("target", "functionName");
            }
            inspector.OnInspectorGUI("method", "trigger", "includeChildren");
            inspector.End();
        }
        
    }
}
