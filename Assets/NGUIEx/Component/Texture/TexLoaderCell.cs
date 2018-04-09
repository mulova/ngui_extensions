//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System;

namespace ngui.ex
{

    public class TexLoaderCell : UIGridCell
    {
        public TexLoader texLoader;

        protected override void DrawCell (object val)
        {
            texLoader.Load (val as string, null);
        }
    }
}
