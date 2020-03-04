using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_LoadData:IHOLoadData
    {
        private Dictionary<string, GameObject> LoadedItems;

        public void AddItem(string key, GameObject item)
        {
            if (HasItem( key ))
            {
                return;
            }

            LoadedItems.Add( key, item );
        }

        public GameObject GetItem(string key)
        {
            if (!HasItem( key ))
            {
                return null;
            }

            return LoadedItems[ key ];
        }

        public bool HasItem(string key)
        {
            if (LoadedItems == null)
                LoadedItems = new Dictionary<string, GameObject>();

            return LoadedItems.ContainsKey( key );
        }
    }
}
