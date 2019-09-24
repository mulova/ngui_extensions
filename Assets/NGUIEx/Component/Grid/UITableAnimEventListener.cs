#if FULL

//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace ngui.ex {
	public interface UITableAnimEventListener : UIAnimEventListener
	{
		void OnRowAnimBegin(int row);
		void OnRowAnimEnd(int row);
	}
}
#endif