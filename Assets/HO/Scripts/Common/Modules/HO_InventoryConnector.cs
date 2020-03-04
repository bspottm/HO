using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_InventoryConnector:IHOInventory
    {
        public HO_InventoryConnector()
        {
            //ItemManager.OnChangeItemsValue += InventoryUpdate;
        }

        public Action OnUpdate { get; set; }

        public bool HasItem(string key)
        {
            return true;// ItemManager.Instance.ContainsItem( key );
        }

        public int GetItemCount(string key)
        {
            return 1;// ItemManager.Instance.GetItemCount( key );
        }

        public void ApplyItem(string key, int count = 1, string source = "")
        {
           // ItemManager.Instance.ApplyItem( key,source, count);
        }

        public void AddItem(string key, int count = 1, string source = "")
        {
            //ItemManager.Instance.AddItems( key, source,false, false, count );
        }

        private void InventoryUpdate(string itemkey, int amaunt)
        {
            OnUpdate?.Invoke();
        }

        ~HO_InventoryConnector()
        {
           // ItemManager.OnChangeItemsValue -= InventoryUpdate;
        }
    }
}
