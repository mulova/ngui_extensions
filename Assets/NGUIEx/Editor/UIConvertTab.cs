using System.Collections.Generic;
using commons;
using UnityEngine;
using UnityEditor;
using comunity;


namespace ngui.ex {
    public class UIConvertTab : EditorTab {
		
		public UIConvertTab(TabbedEditorWindow window) : base("Convert", window) {}

		public override void OnEnable() {
			Refresh();
		}
		
		public override void OnDisable() { }
		public override void OnChangePlayMode() {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnFocus(bool focus) { }
		public override void OnSelected(bool sel) { }
		
		private void Refresh() {
		}
		
		public override void OnHeaderGUI() { }

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Rename")) {
				Rename();
			}
		}

        private static string[][] rename = new string[][]
        {
            new string[]{"bettingbtn_reservationbg_01_1","btn_reservation_01"},
            new string[]{"bettingbtn_reservationbg_01","btn_reservation_01_1"},
            new string[]{"bettingbtn_reservationbg_02_1","btn_reservation_02"},
            new string[]{"bettingbtn_reservationbg_02","btn_reservation_02_2"},
            new string[]{"bettingbtn_reservationbg_03_1","btn_reservation_03"},
            new string[]{"bettingbtn_reservationbg_03","btn_reservation_03_3"},
            new string[]{"bettingbtn_reservationbg","btn_reservation_bg"},
            new string[]{"bettingbtnbg_01","btn_betting_bg_01"},
            new string[]{"bettingbtnbg_02","btn_betting_bg_02"},
            new string[]{"bettingbtnbg_03","btn_betting_bg_03"},
            new string[]{"bettingbtnicon_01","icon_betbtn_01"},
            new string[]{"bettingbtnicon_02","icon_betbtn_02"},
            new string[]{"bettingbtnicon_03","icon_betbtn_03"},
            new string[]{"bettingbtnicon_04","icon_betbtn_04"},
            new string[]{"bettingbtnicon_05","icon_betbtn_05"},
            new string[]{"bettingbtnicon_06","icon_betbtn_06"},
            new string[]{"bettingicon_01","icon_betting_01"},
            new string[]{"bettingicon_02","icon_betting_02"},
            new string[]{"bettingicon_03","icon_betting_03"},
            new string[]{"bettingicon_04","icon_betting_04"},
            new string[]{"bettingicon_05","icon_betting_05"},
            new string[]{"bettingicon_06","icon_betting_06"},
            new string[]{"bettingiconbg_01","bettingiconbg_01"},
            new string[]{"cardresultbg_01","bg_result_01"},
            new string[]{"chipicon_01","icon_chip_01"},
            new string[]{"emoticonicon_01","icon_emoticonicon_01"},
            new string[]{"menubtnarrowIcon_01","icon_narrow"},
            new string[]{"menubtnbg_01","btn_menu_bg"},
            new string[]{"menubtnicon_01","icon_menu_01"},
            new string[]{"menubtnicon_02","icon_menu_02"},
            new string[]{"menubtnicon_03","icon_menu_03"},
            new string[]{"playericon_default","icon_player_default"},
            new string[]{"playerslotbg_01","bg_playerslot_01"},
            new string[]{"playerslotbg_02","bg_playerslot_02"},
            new string[]{"playerslotcountimage_01","img_slot_count_01"},
            new string[]{"playersloteffect_01","img_slot_effect_01"},
            new string[]{"playerslotimage_01","img_slot_allin_01"},
            new string[]{"playerslotimage_02","img_slot_dealer"},
            new string[]{"playerslotimage_03","img_slot_victory_01"},
            new string[]{"popuppublicbg_01","bg_popup_01"},
            new string[]{"popuppublicbg_02","bg_popup_02"},
            new string[]{"popuppublicbg_03","bg_popup_03"},
            new string[]{"potslotbg_01","bg_potslot_01"},
            new string[]{"speechbubblebg_01","bg_speechbubble_01"},
        };
		private static void Rename() {
			foreach (var guid in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var assets = EditorAssetUtil.ListAssets<GameObject>(path, FileType.Prefab);
                foreach (GameObject a in assets)
                {
                    Rename(a);
                }
            }

            foreach (var o in Selection.gameObjects)
            {
                Rename(o);
            }
		}

        private static void Rename(GameObject a)
        {
            foreach (var s in a.GetComponentsInChildren<UISprite>(true))
            {
                if (s.atlas != null && s.atlas.name == "holdem_atlas")
                {
                    foreach (string[] r in rename)
                    {
                        if (s.spriteName.ToLower() == r[0])
                        {
                            Debug.LogFormat("{0} -> {1}", s.spriteName, r[1]);
                            s.spriteName = r[1];
                            CompatibilityEditor.SetDirty(s);
                        }
                    }
                }
            }
        }

		public override void OnFooterGUI() {
		}
	}
}