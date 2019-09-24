using UnityEngine;

using UnityEditor;
using ngui.ex;
using mulova.build;

public class UILazyTableCellBuildProcessor : ComponentBuildProcess
{
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

	public override System.Type compType
	{
		get
		{
			return typeof(UILazyTableCell);
		}
	}
}
