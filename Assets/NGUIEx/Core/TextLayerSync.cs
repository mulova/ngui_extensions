#if TEXTMESH_PRO 
using System;
using UnityEngine;
using TMPro;
[ExecuteInEditMode, RequireComponent(typeof(TextMeshPro))]
public class TextLayerSync : MonoBehaviour
{
	public UIWidget widget;
	private TextMeshPro text;

	void OnEnable() {
		widget.onRender = RenderCallback;
		text = GetComponent<TextMeshPro>();
		if (widget != null && widget.drawCall != null) {
			text.GetComponent<Renderer>().sharedMaterial.renderQueue = widget.drawCall.baseMaterial.renderQueue;
		}
	}

	private void RenderCallback (Material mat)
	{
		if (mat.renderQueue != text.GetComponent<Renderer>().sharedMaterial.renderQueue) {
			mat.renderQueue = text.GetComponent<Renderer>().sharedMaterial.renderQueue;
		}
	}
}
#endif