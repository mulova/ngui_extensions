using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine.Assertions;
using commons;
using comunity;

namespace ngui.ex
{
    [CustomPropertyDrawer(typeof(SpriteAnimInfo))]
    public class SpriteAnimInfoDrawer : PropertyDrawerBase
    {
        protected override int GetLineCount()
        {
            return 1;
        }

        protected override void DrawGUI(GUIContent label)
        {
            UISpriteAnim sprAnim = prop.serializedObject.targetObject as UISpriteAnim;
            UISprite spr = sprAnim.sprite;
            string[] sprList = new string[0];
            if (spr != null && spr.atlas != null)
            {
                sprList = spr.atlas.GetListOfSprites().ToArray();
            }
            SerializedProperty name = GetProperty("name");
            SerializedProperty delay = GetProperty("delay");

            Rect lineRect = GetLineRect(0);
            var r = HorizontalSplitRect(lineRect, 0.7f);

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
