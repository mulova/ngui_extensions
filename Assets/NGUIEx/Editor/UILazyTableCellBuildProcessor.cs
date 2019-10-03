﻿using mulova.preprocess;
using ngui.ex;
using UnityEngine;

public class UILazyTableCellBuildProcessor : ComponentBuildProcess
{
    public override System.Type compType => typeof(UILazyTableCell);

    protected override void VerifyComponent(Component comp)
    {
    }

    protected override void PreprocessComponent(Component comp)
    {
        UILazyTableCell cell = comp as UILazyTableCell;
        if (cell.ui != null)
        {
            cell.ui.SetActive(false);
        }
        if (cell.uiPrefab != null)
        {
            cell.uiPrefab.SetActive(false);
        }
    }

    protected override void PreprocessOver(Component c)
    {
    }

}
