using UnityEngine;
using System.Collections;

namespace ngui.ex
{
    public static class UITableLayoutEx
    {
        /**
        * Row중 UILabel인 경우 text color를 바꾸어준다.
        * @param row row 값은 title row를 포함한 실제 row번호(zero-based)
        */
        public static void SetRowColor(this UITableLayout table, int row, Color color)
        {
            string colorStr = NGUIUtil.ConvertColor2Str(color);
            for (int col = table.columnHeader; col < table.GetColumnCount(); col++)
            {
                Transform t = table.GetCellTransform(row+table.rowHeader, col);
                if (t != null)
                {
                    UILabel label = t.GetComponent<UILabel>();
                    if (label != null)
                    {
                        label.SetText(colorStr+label.text);
                    }
                }
            }
        }
    }
}

