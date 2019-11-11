using UnityEngine;

namespace ngui.ex {
	public class UILabelCell : UITableCell
	{
        public UILabel label;

        void Start()
        {
            if (label == null) 
            {
                label = GetComponent<UILabel>();
            }
        }

		protected override void DrawCell (object val)
		{
			label.SetText(val as string);
		}
	}
}
