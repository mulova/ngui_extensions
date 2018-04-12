//----------------------------------------------
// NGUI extensions
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2018 mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Text;
using comunity;

namespace ngui.ex
{
    /**
     * Get GridLayout's row/cell prefab information
     */
    public class UIGridPrefabs
    {
        public GameObject[] prefabs;

        public UIGridPrefabs(params GameObject[] prefabs)
        {
            this.prefabs = prefabs;
        }

        public GameObject GetPrefab(int index)
        {
            int i = index % prefabs.Length;
            return prefabs[i];
        }

        public Transform Instantiate(int index)
        {
            GameObject prefab = GetPrefab(index);
            GameObject instance = prefab.InstantiateEx(prefab.transform.parent, false);
            instance.SetActive(true);
            StringBuilder str = new StringBuilder(prefab.name.Length+4);
            str.Append(prefab.name);
            str.Append("_").Append(index);
            return instance.transform;
        }
    }
}