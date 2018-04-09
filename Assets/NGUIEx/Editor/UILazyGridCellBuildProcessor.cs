using UnityEngine;

using UnityEditor;
using ngui.ex;
using build;

public class UILazyGridCellBuildProcessor : ComponentBuildProcess
{
	protected override void VerifyComponent(Component comp)
	{
	}

	protected override void PreprocessComponent(Component comp)
	{
		UILazyGridCell cell = comp as UILazyGridCell;
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
			return typeof(UILazyGridCell);
		}
	}
}
