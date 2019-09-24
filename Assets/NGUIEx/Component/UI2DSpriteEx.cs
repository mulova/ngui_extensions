//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Text;
using mulova.commons;

namespace ngui.ex
{
	public static class UI2DSpriteEx {
		
		private static Loggerx log = LogManager.GetLogger(typeof(UI2DSprite));
		
		public static void SetSprite(this UI2DSprite sprite, string id, params Sprite[] sprites) {
			foreach (Sprite s in sprites) {
				if (s.name == id) {
					sprite.sprite2D = s;
					return;
				}
			}
			log.Error("Missing sprite '{0}'", id);
		}
		
	}
	
}

