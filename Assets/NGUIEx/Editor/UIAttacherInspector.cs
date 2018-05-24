//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------
using System.Collections.Generic;



using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using comunity;

namespace ngui.ex
{
	[CustomEditor(typeof(UIAttacher))]
	public class UIAttacherInspector : Editor
	{

		private UIAttacher attach;

		void OnEnable()
		{
			this.attach = (UIAttacher)target;
			if (attach.anchor != null) {
				anchorObj = attach.anchor.gameObject;
			}
			if (attach.pivot != null) {
				pivotObj = attach.pivot.gameObject;
			}
		}

		private void Init()
		{
			attach.anchor = anchorObj.GetComponentEx<UIAnchor>();
			attach.pivot = pivotObj.GetComponentEx<UIPivot>();
		}
		
		private GameObject anchorObj;
		private GameObject pivotObj;

		public override void OnInspectorGUI()
		{
			bool changed = false;

			if (attach.anchor != null) {
				Object t = attach.anchor.container != null? (Object)attach.anchor.container: (Object)attach.anchor.uiCamera;
				if (EditorGUIUtil.ObjectField("Target", ref t, true)) {
					attach.anchor.uiCamera = null;
					attach.anchor.container = null;
					if (t is Camera) {
						attach.anchor.uiCamera = t as Camera;
					} else {
						attach.anchor.container = t as GameObject;
					}
					changed = true;
				}
			}
			if (anchorObj == null && pivotObj == null) {
				anchorObj = attach.gameObject;
				// Add pivot GameObject between anchor and anchor children.
				if (anchorObj.transform.childCount == 1 && anchorObj.transform.GetChild(0).GetComponent<UIPivot>() != null) {
					pivotObj = anchorObj.transform.GetChild(0).gameObject;
				} else {
					pivotObj = new GameObject("_pivot");
					pivotObj.tag = anchorObj.tag;
					pivotObj.transform.SetParent(anchorObj.transform, false);
					List<Transform> children = new List<Transform>();
					foreach (Transform t in anchorObj.transform) {
						if (t != anchorObj.transform && t != pivotObj.transform) {
							children.Add(t);
						}
					}
					foreach (Transform t in children) {
						t.parent = pivotObj.transform;
					}
				}
				Init();
				changed = true;
			} else {
				Color oldBg = GUI.backgroundColor;
				GUI.backgroundColor = Color.gray;
				changed |= EditorGUIUtil.ObjectField<GameObject>("Anchor", ref anchorObj, true);
				changed |= EditorGUIUtil.ObjectField<GameObject>("Pivot", ref pivotObj, true);
				GUI.backgroundColor = oldBg;
			}

			if (target != null && anchorObj != null && pivotObj != null) {
				if (changed) {
					Init();
				}
				if (anchorObj == pivotObj || anchorObj.transform.IsChildOf(pivotObj.transform)) {
					EditorGUILayout.HelpBox("Anchor and Pivot should not be in the same GameObject", MessageType.Error);
				} else {
					EditorGUILayout.BeginVertical();
					for (int row=0; row<5; row++) {
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						for (int col=0; col<5; col++) {
							int buttonWidth = 30;
							int buttonHeight = 30;
							Color c = Color.white;
							if (row == 0 || row == 4) {
								buttonHeight = 25;
								c = Color.gray;
							}
							if (col == 0 || col == 4) {
								buttonWidth = 25;
								c = Color.gray;
							}
							GUI.backgroundColor = c;
							if (GUILayout.Button("A", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
								attach.pivot.pivot = pivots[row, col];
								attach.anchor.side = sides[row, col];
								attach.Reposition(sides[row, col], pivots[row, col]);
								changed = true;
							}
						}
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
	
					changed |= EditorGUIUtil.Vector2Field("Relative Offset", ref attach.anchor.relativeOffset);
	
					int pixelOffsetX = 0;
					int pixelOffsetY = 0;
					if (attach.anchor != null) {
						pixelOffsetX = (int)attach.anchor.pixelOffset.x;
						pixelOffsetY = (int)attach.anchor.pixelOffset.y;
					}
					if (EditorGUIUtil.IntField("Pixel Offset X", ref pixelOffsetX)
						|| EditorGUIUtil.IntField("Pixel Offset Y", ref pixelOffsetY)) {
						attach.anchor.pixelOffset = new Vector2(pixelOffsetX, pixelOffsetY);
						changed = true;
					}
	
					if (changed) {
						EditorUtil.SetDirty(attach.anchor);
						EditorUtil.SetDirty(attach.pivot);
						EditorUtil.SetDirty(attach);
					}
				}
			}
		}
		
		private UIWidget.Pivot[,] pivots = new UIWidget.Pivot[5, 5] { {
						UIWidget.Pivot.BottomRight,
						UIWidget.Pivot.BottomLeft,
						UIWidget.Pivot.Bottom,
						UIWidget.Pivot.BottomRight,
						UIWidget.Pivot.BottomLeft
				}, {
						UIWidget.Pivot.TopRight,
						UIWidget.Pivot.TopLeft,
						UIWidget.Pivot.Top,
						UIWidget.Pivot.TopRight,
						UIWidget.Pivot.TopLeft
				}, {
						UIWidget.Pivot.Right,
						UIWidget.Pivot.Left,
						UIWidget.Pivot.Center,
						UIWidget.Pivot.Right,
						UIWidget.Pivot.Left
				}, {
						UIWidget.Pivot.BottomRight,
						UIWidget.Pivot.BottomLeft,
						UIWidget.Pivot.Bottom,
						UIWidget.Pivot.BottomRight,
						UIWidget.Pivot.BottomLeft
				}, {
						UIWidget.Pivot.TopRight,
						UIWidget.Pivot.TopLeft,
						UIWidget.Pivot.Top,
						UIWidget.Pivot.TopRight,
						UIWidget.Pivot.TopLeft
				},
			};
		private UIAnchor.Side[,] sides = new UIAnchor.Side[5, 5] { {
						UIAnchor.Side.TopLeft,
						UIAnchor.Side.TopLeft,
						UIAnchor.Side.Top,
						UIAnchor.Side.TopRight,
						UIAnchor.Side.TopRight
				}, {
						UIAnchor.Side.TopLeft,
						UIAnchor.Side.TopLeft,
						UIAnchor.Side.Top,
						UIAnchor.Side.TopRight,
						UIAnchor.Side.TopRight
				}, {
						UIAnchor.Side.Left,
						UIAnchor.Side.Left,
						UIAnchor.Side.Center,
						UIAnchor.Side.Right,
						UIAnchor.Side.Right
				}, {
						UIAnchor.Side.BottomLeft,
						UIAnchor.Side.BottomLeft,
						UIAnchor.Side.Bottom,
						UIAnchor.Side.BottomRight,
						UIAnchor.Side.BottomRight
				}, {
						UIAnchor.Side.BottomLeft,
						UIAnchor.Side.BottomLeft,
						UIAnchor.Side.Bottom,
						UIAnchor.Side.BottomRight,
						UIAnchor.Side.BottomRight
				},
		};
	}
}
