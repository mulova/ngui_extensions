#if FULL
//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

using DaikonForge.Tween;

namespace ngui.ex {
	public class UIGridAnim : MonoBehaviour, UIGridEventListener {
		
		public enum AnimType {
			ShowRotate, ShowLeft, ShowRight, 
			HideRotate, HideLeft, HideRight,
			MoveRow
		}
		
		public UIGridLayout grid;
		public float speed = 2;
		public float interval = 0.2f; // time interval to begin next row animation
		public AnimType animType = AnimType.ShowRotate;
        public EasingType easeType = EasingType.Bounce;
		public bool animateBackground = true;
		public int move = 400;
		
		private delegate void Animator(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation);
		private Animator animator;
		
		private List<UIGridAnimEventListener> listeners = new List<UIGridAnimEventListener>();
		
		void OnEnable() {
			if (grid == null) {
				grid = GetComponent<UIGridLayout>();
			}
			if (grid != null) {
				grid.AddListener(this);
			}
		}
		
		void OnDisable() {
			if (grid != null) {
				grid.RemoveListener(this);
			}
		}
		
		public void AddListener(UIGridAnimEventListener l) {
#if UNITY_EDITOR
			Assert.AssertFalse(listeners.Contains(l));
#endif
			listeners.Add(l);
		}
		
		public void RemoveListener(UIGridAnimEventListener l) {
			listeners.Add(l);
		}
		
		private Vector3[,] cellScales = new Vector3[0,0];
		private Vector3[] bgScales = new Vector3[0];
		private Vector3[,] cellPos = new Vector3[0,0];
		private Vector3[] bgPos = new Vector3[0];
		private float[] rowDelay = new float[0];
		private bool invalid = false;
		void Update () {
			if (grid == null || !invalid) {
				return;
			}
			float delta = Time.deltaTime;
			
			if (rowCount == 0) {
				Init();
			} 
			for (int r=0; r<rowCount; r++) {
				float old = rowDelay[r];
				rowDelay[r] = Math.Min(rowDelay[r]+delta*speed, 1);
				float time = Math.Max(0, rowDelay[r]);
				float interpolation = TweenEasingFunctions.GetFunction(easeType)(time);
				for (int c=0; c<colCount; c++) {
					Transform t = grid.GetCell(r,c);
					if (t != null) {
						animator(r, t, cellScales[r,c], cellPos[r,c], interpolation);
					}
				}
				if (animateBackground) {
					Transform bg = grid.GetBackground(r);
					if (bg != null) {
						animator(r, bg, bgScales[r], bgPos[r], interpolation);
					}
				}
				if (r>=grid.rowHeader && old <= 0 && time > 0) {
					foreach (UIGridAnimEventListener l in listeners) {
						l.OnRowAnimBegin(r-grid.rowHeader);
					}
				}
				if (r>=grid.rowHeader && old < 1 && time >= 1) {
					foreach (UIGridAnimEventListener l in listeners) {
						l.OnRowAnimEnd(r-grid.rowHeader);
					}
				}
			}
			if (rowCount>0 && rowDelay[rowCount-1] >= 1) {
				for (int r=0; r<rowCount; r++) {
					Transform bg = grid.GetBackground(r);
					if (bg != null) {
						NGUIUtil.UpdateCollider(bg);
					}
				}
				invalid = false;
				foreach (UIGridAnimEventListener l in listeners) {
					l.OnAnimEnd();
				}
			}
		}
		
		private void ShowRotate(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation) {
			Vector3 s = scale;
			s.y *= interpolation;
			t.localScale = s;
			t.localPosition = pos;
		}
		
		private void HideRotate(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation) {
			Vector3 s = scale;
			s.y *= 1-interpolation;
			t.localScale = s;
			t.localPosition = pos;
		}
		
		private void ShowLeft(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation) {
			Vector3 p = pos;
			p.x += (interpolation-1)*move;
			t.localPosition = p;
		}
		
		private void HideLeft(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation) {
			Vector3 p = pos;
			p.x -= interpolation*move;
			t.localPosition = p;
		}
		
		private void ShowRight(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation) {
			Vector3 p = pos;
			p.x += (1-interpolation)*move;
			t.localPosition = p;
		}
		
		private void HideRight(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation) {
			Vector3 p = pos;
			p.x += interpolation*move;
			t.localPosition = p;
		}
		
		private int[] moveRowDst;
		private void MoveRow(int row, Transform t, Vector3 scale, Vector3 pos, float interpolation) {
			if (row >= grid.rowHeader) {
				float delta = grid.GetCellPos(moveRowDst[row-grid.rowHeader]+grid.rowHeader,0).y - grid.GetCellPos(row,0).y;
				Vector3 p = pos;
				p.y += interpolation*delta;
				if (delta > 0) {
					p.z -= 1;
					t.localScale = scale*(1+2*(0.5F-Math.Abs(interpolation-0.5F)));
				}
				t.localPosition = p;
			}
		}
		
		public void MoveRows(int[] rows) {
			moveRowDst = rows;
		}
		
		public void BeginAnimation() {
			// if animation is already in progress, new animation cannot be started
			Reset();
			invalid = true;
			rowCount = 0;
			colCount = 0;
			Update();
		}
		
		/**
		 * Backup scales and hide all cells and backgrounds
		 */
		private int rowCount;
		private int colCount;
		private void Init() {
			rowCount = grid.GetRowCount();
			colCount = grid.GetColumnCount();
			// BackUp scale
			cellScales = new Vector3[rowCount, colCount];
			cellPos = new Vector3[rowCount, colCount];
			bgScales = new Vector3[rowCount];
			bgPos = new Vector3[rowCount];
			rowDelay = new float[rowCount];
			for (int r=0; r<rowCount; r++) {
				// backup background scale
				rowDelay[r] = -interval*r;
				Transform tbg = grid.GetBackground(r);
				if (tbg != null) {
					bgScales[r] = tbg.localScale;
					bgPos[r] = tbg.localPosition;
				}
				// backup cell scale
				for (int c=0; c<colCount; c++) {
					Transform tcell = grid.GetCell(r, c);
					if (tcell != null) {
						cellScales[r,c] = tcell.localScale;
						cellPos[r,c] = tcell.localPosition;
					}
				}
			}
			invalid = true;
			if (animType == AnimType.ShowRotate) {
				animator = ShowRotate;
			} else if (animType == AnimType.HideRotate) {
				animator = HideRotate;
			} else if (animType == AnimType.ShowLeft) {
				animator = ShowLeft;
			} else if (animType == AnimType.HideLeft) {
				animator = HideLeft;
			} else if (animType == AnimType.ShowRight) {
				animator = ShowRight;
			} else if (animType == AnimType.HideRight) {
				animator = HideRight;
			} else if (animType == AnimType.MoveRow) {
				animator = MoveRow;
			} else {
				animator = ShowRotate;
			}
		}
		
		/**
		 * Set transforms to initial value
		 */
		private void Reset() {
			if (grid == null) {
				return;
			}
			for (int r=0; r<rowCount; r++) {
				Transform tbg = grid.GetBackground(r);
				if (tbg != null) {
					tbg.localScale = bgScales[r];
					tbg.localPosition = bgPos[r];
				}
				for (int c=0; c<colCount; c++) {
					Transform tcell = grid.GetCell(r, c);
					if (tcell != null) {
						tcell.localScale = cellScales[r,c];
						tcell.localPosition = cellPos[r,c];
					}
				}
			}
		}
		
		void UIGridEventListener.OnRowSelected (int row) { }
		
		void UIGridEventListener.OnModelChanged ()
		{
			BeginAnimation();
		}
	}
	
	public static class AnimTypeMethod {
		public static bool IsHide(this UIGridAnim.AnimType a) {
			return a==UIGridAnim.AnimType.HideRotate 
				|| a==UIGridAnim.AnimType.HideLeft
					|| a==UIGridAnim.AnimType.HideRight;
		}
	}
}
#endif