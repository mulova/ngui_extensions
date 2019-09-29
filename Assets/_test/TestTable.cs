using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ngui.ex;

public class TestTable : MonoBehaviour {
    public UITableLayout table;
    public int count;

	void Start () {
        table.SetDummyModel(count);
	}
}
