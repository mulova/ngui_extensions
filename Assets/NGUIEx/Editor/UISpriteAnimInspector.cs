using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using comunity;
using commons;


namespace ngui.ex {
	[CustomEditor(typeof(UISpriteAnim))]
	public class UISpriteAnimInspector : Editor {
		
		private UISpriteAnim sprite;
        private ArrayDrawer<SpriteAnimInfo> arrDrawer;
		private SerializedInspector varInspector;
		private float globalDelay;
		
		void OnEnable() {
			sprite = (UISpriteAnim)target;
            varInspector = new SerializedInspector(new SerializedObject(sprite));
            varInspector.Exclude("anim");
        }
        
        public override void OnInspectorGUI() {
            varInspector.OnInspectorGUI();
            if (arrDrawer == null && sprite.sprite != null&&sprite.sprite.atlas != null)
            {
                string[] sprList = sprite.sprite.atlas.GetListOfSprites().ToArray();
                arrDrawer = new ArrayDrawer<SpriteAnimInfo>(sprite, sprite, "anim", new SpriteAnimInfoItemDrawer(sprList));
                arrDrawer.createDefaultValue = () => new SpriteAnimInfo();
                arrDrawer.addSelected = false;
            }
            if (arrDrawer == null)
            {
                return;
            }
            if (arrDrawer.Draw(Rotorz.ReorderableList.ReorderableListFlags.ShowIndices)) {
                CompatibilityEditor.SetDirty(target);
			}
			
			if (sprite.anim != null && sprite.anim.Length == 1 && GUILayout.Button("Add all")) {
				string firstName = sprite.anim[0].name;
                string baseName = firstName.DetachSuffix();
				string separator = firstName.Length == baseName.Length? "": firstName[baseName.Length].ToString();
				if (char.IsDigit(separator[0])) {
					separator = "";
				}
				int i=1;
                string name = baseName.AddSuffix(separator, i);
				UISpriteData s = sprite.sprite.atlas.GetSprite(name);
				while (s != null) {
					arrDrawer.Add(new SpriteAnimInfo(name, sprite.anim[0].delay));
					i++;
                    name = baseName.AddSuffix(separator, i);
					s = sprite.sprite.atlas.GetSprite(name);
				}
                CompatibilityEditor.SetDirty(sprite);
			}
			if (EditorGUIUtil.FloatField("Global Delay", ref globalDelay)) {
				foreach (SpriteAnimInfo i in sprite.anim) {
					i.delay = globalDelay;
				}
                CompatibilityEditor.SetDirty(sprite);
			}
		}

        class SpriteAnimInfoItemDrawer : ItemDrawer<SpriteAnimInfo>
        {
            private string[] sprList;
            public SpriteAnimInfoItemDrawer(string[] sprList)
            {
                this.sprList = sprList;
            }
            public override bool DrawItem(Rect rect, int index, SpriteAnimInfo obj, out SpriteAnimInfo newObj)
            {
                bool changed = false;
                var r = SplitRectHorizontally(rect, 0.7f);

                int nameIndex = sprList.FindIndex(obj.name);
                var nameIndex2 = EditorGUI.Popup(r[0], nameIndex, sprList);
                if (nameIndex != nameIndex2)
                {
                    obj.name = sprList[nameIndex2];
                    changed = true;
                }
                var delay2 = EditorGUI.FloatField(r[1], obj.delay);
                if (delay2 != obj.delay)
                {
                    obj.delay = delay2;
                    changed = true;
                }
                newObj = obj;
                return changed;
            }
        }
    }

}