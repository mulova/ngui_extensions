//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace ngui.ex
{

    public class TexLoaderCell : UITableCell
    {
        public TexLoader texLoader;

        protected override void DrawCell (object val)
        {
            texLoader.Load (val as string, null);
        }
    }
}
