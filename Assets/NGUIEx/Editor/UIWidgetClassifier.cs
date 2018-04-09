using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

using commons;


namespace ngui.ex {
	/**
	 * UI를 Widget별로 구분해서 넣어준다.
	 * Widget의 하위 Component들은 제외한다.(예를 들어 Button의 Label은 제외)
	 */
	public class UIWidgetClassifier {
		
		public enum WidgetType
		{
			Null,
			Button,
			Label,
			Texture,
			Checkbox,
			ProgressBar,
			Slider,
			Input,
			PopupList,
			ScrollBar,
		}
		
		private MultiMap<WidgetType, GameObject> widgets = new MultiMap<WidgetType, GameObject>();
		
		public UIWidgetClassifier(GameObject root) {
			SetRoot(root);
		}
		
		public void SetRoot(GameObject root) {
			widgets.RemoveAll();
			
			if (root == null) {
				return;
			}
			
			Transform[] all = root.transform.GetComponentsInChildren<Transform>(true);
			foreach (Transform t in all) {
				GameObject o = t.gameObject;
				if (o.GetComponent<UILabel>() != null) {
					widgets.Add(WidgetType.Label, o);
				} else if (o.GetComponent<UIButton>() != null || o.GetComponent<UIImageButton>() != null) {
					widgets.Add(WidgetType.Button, o);
				} else if (o.GetComponent<UIToggle>() != null) {
					widgets.Add(WidgetType.Button, o);
				} else if (o.GetComponent<UISlider>() != null) {
					if (o.GetComponentInChildren<UIButtonColor>() != null) {
						widgets.Add(WidgetType.Slider, o);
					} else {
						widgets.Add(WidgetType.ProgressBar, o);
					}
				} else if (o.GetComponent<UIInput>() != null) {
					widgets.Add(WidgetType.Input, o);
				} else if (o.GetComponent<UIPopupList>() != null) {
					widgets.Add(WidgetType.PopupList, o);
				} else if (o.GetComponent<UISprite>() != null) {
					widgets.Add(WidgetType.Texture, o);
				} else if (o.GetComponent<UIScrollBar>() != null) {
				}
			}
			// remove unnecessary widgets
			RemoveLabels(WidgetType.Button);
			RemoveLabels(WidgetType.Checkbox);
			RemoveLabels(WidgetType.Input);
			RemoveLabels(WidgetType.PopupList);
			
			RemoveTextures(WidgetType.Button);
			RemoveTextures(WidgetType.Slider);
			RemoveTextures(WidgetType.ProgressBar);
		}
		
		public List<GameObject> this[WidgetType widgetType] {
			get { return GetWidgets(widgetType); }
		}
		
		public List<GameObject> GetWidgets(WidgetType widgetType) {
			return widgets[widgetType];
		}
		
		public WidgetType GetWidgetType(GameObject obj) {
			if (obj != null) {
                foreach (WidgetType t in EnumUtil.Values<WidgetType>()) {
					if (widgets[t].Contains(obj)) {
						return t;
					}
				}
			}
			return WidgetType.Null;
		}
		
		private void RemoveLabels(WidgetType widgetType) {
			foreach (GameObject o in widgets[widgetType]) {
				foreach (UILabel l in o.GetComponentsInChildren<UILabel>()) {
					widgets.Remove(WidgetType.Label, l.gameObject);
				}
			}
		}
		
		private void RemoveTextures(WidgetType widgetType) {
			foreach (GameObject o in widgets[widgetType]) {
				foreach (UISprite l in o.GetComponentsInChildren<UISprite>()) {
					widgets.Remove(WidgetType.Texture, l.gameObject);
				}
			}
		}
	}
}
