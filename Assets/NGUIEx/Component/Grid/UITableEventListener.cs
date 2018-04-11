//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

namespace ngui.ex {
	/// <summary>
    /// Called when UITableLayout's row is selected.
	/// </summary>
	public interface UITableEventListener
	{
		void OnRowSelected(int row);
		void OnModelChanged();
	}	
}
