//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using AnchorUpdate = UIRect.AnchorUpdate;

namespace ngui.ex {
	[ExecuteInEditMode, RequireComponent(typeof(EnvelopContent))]
	public class UIBound : UILayout {
		public AnchorUpdate update = AnchorUpdate.OnUpdate;
		private EnvelopContent content;

		void OnEnable() {
			if (update == AnchorUpdate.OnEnable) {
				Execute();
			}
		}

		void Start() {
			if (update == AnchorUpdate.OnStart) {
				Execute();
			}
		}

		protected override void DoLayout ()
		{
			Execute();
		}

		protected override void UpdateImpl ()
		{
			if (update == AnchorUpdate.OnUpdate) {
				Execute();
			}
		}

		public void Execute()
		{
			if (content == null)
			{
				content = GetComponent<EnvelopContent>();
			}
			content.Execute();
		}
	}
}
