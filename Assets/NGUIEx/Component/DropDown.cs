using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using ngui.ex;

using commons;
using comunity;

namespace ngui.ex
{
    public class DropDown : MonoBehaviour
    {
        public UIButton trigger;
        public GameObject ui;
        public UITableLayout sortGrid;
        public string preStrKey;
        private DropDownCellData sel;
        private string prefId;
        private static GamePref _pref;

        public static GamePref pref
        {
            get
            {
                if (_pref == null)
                {
                    _pref = new GamePref("_gpref_dropdown", true);
                }
                return _pref;
            }
        }
        
        public static readonly Loggerx log = LogManager.GetLogger(typeof(DropDown));
        
        private Action<DropDownCellData> callback;
        
        void Start()
        {
            trigger.SetCallback(Open);
            ui.SetActive(false);
        }
        
        public bool IsInitialized()
        {
            return sortGrid.GetModel() != null;
        }
        
        public void Init(IList<DropDownCellData> items, Action<DropDownCellData> callback = null) {
            Init(null, items, callback);
        }
        
        public void Init(string prefId, IList<DropDownCellData> items, Action<DropDownCellData> callback = null) {
            this.prefId = prefId;
            this.callback = callback;
			sortGrid.SetContents(items);
            
            int index = 0;
            if (prefId.IsNotEmpty()) {
				index = MathUtil.Clamp(pref.GetInt(prefId, 0), 0, items.Count-1);
            }
            Select(items[index]);
        }
        
        public void Open()
        {
            ui.SetActive(true);
            // grouped UIToggle can't be set if GameObject is not active
            if (IsInitialized() && sel != null) {
                Select(sel);
            }
        }
        
        public void Sort()
        {
            DropDownCellData sel = GetSelected();
            if (sel != null)
            {
                OnSelectItem(sel);
            } else 
            {
                log.Warn("DropDown items are not set yet");
            }
        }
        
        public DropDownCellData GetSelected() {
            return sel;
        }
        
        internal void OnSelectItem(DropDownCellData data)
        {
            Select(data);
            ui.SetActive(false);
            callback.Call(data);
        }
        
        public void Select (DropDownCellData data)
        {
            this.sel = data;
            UIText title = trigger.GetComponentInChildrenEx<UIText>();
            title.textKey = null;
            if (preStrKey != null)
            {
                title.SetText("{0}  {1}", Lexicon.Get(preStrKey), data.comboText);
            } else
            {
                title.SetText(data.comboText);
            }
            if (ui.activeSelf) {
                sortGrid.SelectCell(data);
            }
            if (prefId.IsNotEmpty()) {
                UITableCell c = sortGrid.GetSelectedCell<UITableCell>();
                if (c != null) {
                    int index = sortGrid.GetIndex(c);
                    pref.SetInt(prefId, index);
                }
            }
            
        }
    }
}

public class DropDownCellData {
	public readonly string title;
	public readonly object data;
	public readonly string comboText;
	
	public DropDownCellData(string title, object data): this(title, data, title) { }
	
	public DropDownCellData(string title, object data, string comboText) {
		this.title = title;
		this.data = data;
		this.comboText = comboText;
	}

	public object GetData() {
		return data;
	}
}