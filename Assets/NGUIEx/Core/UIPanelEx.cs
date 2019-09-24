#define RQ
using System;
using UnityEngine;
using UnityEngine.Assertions;
using mulova.commons;
using Assert  = mulova.commons.Assert;

namespace ngui.ex
{
	public static class UIPanelEx
	{
		public static int GetMaxRenderQueue(this UIPanel p) {
			int max = -1;
			foreach (UIDrawCall dc in p.drawCalls) {
				max = Math.Max(dc.finalRenderQueue, max);
			}
			#if RQ
			if (p.renderQueue != UIPanel.RenderQueue.Automatic) {
				max = Math.Max(p.startingRenderQueue, max);
			}
			#endif
			return max;
		}
		
		/// <summary>
		/// Gets the minimum render queue.
		/// </summary>
		/// <returns>The minimum render queue value. -1 if invalid.</returns>
		/// <param name="p">P.</param>
		public static int GetMinRenderQueue(this UIPanel p) {
			if (p.drawCalls.Count == 0) {
				return -1;
			}
			int min = int.MaxValue;
			foreach (UIDrawCall dc in p.drawCalls) {
				min = Math.Min(dc.finalRenderQueue, min);
			}
			return min;
		}
		
		public static void SetLayerOver(this UIPanel over, UIPanel below) {
			UIPanel[] belowPanels = below.GetComponentsInChildren<UIPanel>(true);
			over.SetLayerOver(belowPanels);
		}
		public static void SetLayerOver(this UIPanel over, UIPanel[] belowPanels) {
			UIPanel[] overPanels = over.GetComponentsInChildren<UIPanel>(true);
			Array.Sort(overPanels, (p1,p2)=>p1.depth-p2.depth);
			
			int maxDepth = -1;
			foreach (UIPanel p in belowPanels) {
				maxDepth = Math.Max(maxDepth, p.depth);
			}
			int minDepth = int.MaxValue;
			foreach (UIPanel p in overPanels) {
				minDepth = Math.Min(minDepth, p.depth);
			}
			if (minDepth == int.MaxValue) {
				minDepth = 0;
			}
			int baseDepth = Math.Max(0, maxDepth - minDepth + 1);
			Assert.IsTrue(baseDepth < 20000);
			// depth must be equal or above 100
			foreach (UIPanel p in overPanels) {
				#if RQ
				p.depth = Math.Max(100, p.depth+baseDepth);
				#else
				p.depth = p.depth+baseDepth;
				#endif
			}
			
			int belowMaxQ = -1;
			foreach (UIPanel p in belowPanels) {
				int q = p.GetMaxRenderQueue();
				if (q >= 0) {
					belowMaxQ = Math.Max(belowMaxQ, q);
				}
			}
			if (belowMaxQ >= 0) {
				int overMinQ = int.MaxValue;
				foreach (UIPanel p in overPanels) {
					int q = p.GetMinRenderQueue();
					if (q >= 0) {
						overMinQ = Math.Min(overMinQ, q);
					}
				}
				int baseQ = belowMaxQ - overMinQ + 1;
				foreach (UIPanel p in overPanels) {
					if (p.gameObject.activeInHierarchy && p.renderQueue != UIPanel.RenderQueue.Explicit) {
						p.renderQueue = UIPanel.RenderQueue.StartAt;
						if (overMinQ != int.MaxValue) {
							if (baseQ > 0) {
								p.startingRenderQueue += baseQ;
							}
						} else {
							p.startingRenderQueue = belowMaxQ+(p.depth-minDepth)*10; // 10 delta per depth
						}
					}
				}
			}
			
			int maxOrder = -1;
			foreach (UIPanel p in belowPanels) {
				maxOrder = Math.Max(maxOrder, p.sortingOrder);
			}
			// get non-NGUI renderer
			if (belowPanels.IsNotEmpty()) {
				foreach (Renderer r in belowPanels[0].GetComponentsInChildren<Renderer>(true)) {
					maxOrder = Math.Max(maxOrder, r.sortingOrder);
				}
			}
			
			SetMinSortingOrder(over, overPanels, maxOrder);
			
			over.Invalidate(true);
		}
		
		public static void SetMinSortingOrder (this UIPanel over, UIPanel[] overPanels, int maxOrder)
		{
			int minOrder = int.MaxValue;
			foreach (UIPanel p in overPanels) {
				minOrder = Math.Min(minOrder, p.sortingOrder);
			}
			if (minOrder == int.MaxValue) {
				minOrder = 0;
			}
			int baseOrder = maxOrder - minOrder + 1;
			foreach (UIPanel p in overPanels) {
				p.sortingOrder = p.sortingOrder+baseOrder;
			}
			foreach (ParticleSystem s in over.GetComponentsInChildren<ParticleSystem>(true)) {
				s.GetComponent<Renderer>().sortingOrder += baseOrder;
			}
		}
		
		public static void ArrangeRenderQueue(this UIPanel over) {
			UIPanel[] panels = over.GetComponentsInChildren<UIPanel>(true);
			
			Array.Sort(panels, (x,y)=>x.depth-y.depth);
			int depth = panels[0].depth;
			int min = panels[0].GetMinRenderQueue();
			int max = panels[0].GetMaxRenderQueue();
			for (int i=1; i<panels.Length; ++i) {
				UIPanel p = panels[i];
				int size = p.GetMaxRenderQueue() - p.GetMinRenderQueue();
				if (depth == p.depth) {
					if (p.startingRenderQueue < min) {
						p.startingRenderQueue = min;
					}
					max = Math.Max(max, p.startingRenderQueue+size);
				} else {
					depth = p.depth;
					if (p.startingRenderQueue < max+1) {
						p.startingRenderQueue = max+1;
					}
					min = p.startingRenderQueue;
					max = min+size;
				}
			}
		}
		
		public static Vector2 GetScreenSize (this UIPanel panel)
		{
			UIRoot rt = panel.GetComponent<UIRoot>();
			Vector2 size = NGUITools.screenSize;
			if (rt != null) size *= rt.GetPixelSizeAdjustment(Mathf.RoundToInt(size.y));
			return size;
		}
	}
}

