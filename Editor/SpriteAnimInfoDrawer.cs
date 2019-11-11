using System.Ex;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Ex;

namespace ngui.ex
{
    [CustomPropertyDrawer(typeof(SpriteAnimInfo))]
    public class SpriteAnimInfoDrawer : PropertyDrawerBase
    {

        protected override int GetLineCount(SerializedProperty p)
        {
            return 1;
        }

        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            UISpriteAnim sprAnim = p.serializedObject.targetObject as UISpriteAnim;
            UISprite spr = sprAnim.sprite;
            string[] sprList = new string[0];
            if (spr != null && spr.atlas != null)
            {
                sprList = spr.atlas.GetListOfSprites().ToArray();
            }
            SerializedProperty name = GetProperty("name");
            SerializedProperty delay = GetProperty("delay");

            var r = bound.SplitByWidthsRatio(0.7f, 0.3f);

            int nameIndex = sprList.FindIndex(name.stringValue);
            var nameIndex2 = EditorGUI.Popup(r[0], nameIndex, sprList);
            if (nameIndex != nameIndex2)
            {
                name.stringValue = sprList[nameIndex2];
            }
            var delay2 = EditorGUI.FloatField(r[1], delay.floatValue);
            if (delay2 != delay.floatValue)
            {
                delay.floatValue = delay2;
            }
        }
    }
}
