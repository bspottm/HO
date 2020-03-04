using System;
using System.Collections.Generic;
using UnityEngine;
namespace HOSystem
{
    public class HO_LocationSetting:MonoBehaviour
    {
        public Dictionary<string, HO_LocationSettingItem> alternativeItems = new Dictionary<string, HO_LocationSettingItem>();

        [SerializeField]
        List<HO_LocationSettingItem> itemList = new List<HO_LocationSettingItem>();

        public void init()
        {
            alternativeItems.Clear();
            if (itemList != null && itemList.Count > 0)
                for (int i = itemList.Count - 1; i >= 0; i--)
                {
                    if (itemList[ i ] == null)
                    {
                        itemList.RemoveAt( i );
                        continue;
                    }
                    if (alternativeItems.ContainsKey( itemList[ i ].GetName ))
                    {
                        itemList.RemoveAt( i );
                        continue;
                    }

                    if (string.IsNullOrEmpty( itemList[ i ].GetName ))
                    {
                        itemList.RemoveAt( i );
                        continue;
                    }

                    if (itemList[ i ].collider == null && itemList[ i ].silhuette == null)
                    {
                        itemList.RemoveAt( i );
                        continue;
                    }

                    alternativeItems.Add( itemList[ i ].GetName, itemList[ i ] );
                }
        }

        public HO_LocationSettingItem GetItem(string name)
        {
            if (!alternativeItems.ContainsKey( GetName( name ) ))
                return null;

            return alternativeItems[ GetName( name ) ];
        }

        public HO_LocationSettingItem CreateItem(string name)
        {
            if (alternativeItems.ContainsKey( GetName( name ) ))
                return alternativeItems[ GetName( name ) ];

            var item = new HO_LocationSettingItem();
            item.name = GetName( name );
            item.core = this;
            itemList.Add( item );
            alternativeItems.Add( GetName( name ), item );
            return item;
        }

        public void RemoveItem(string name)
        {
            if (!alternativeItems.ContainsKey( GetName( name ) ))
                return;

            alternativeItems.Remove( GetName( name ) );
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[ i ].name == GetName( name ))
                {
                    itemList.RemoveAt( i );
                    break;
                }
            }
        }


        private string GetName(string name)
        {
            return name.Split( '@' )[ 0 ];
        }
    }

    [Serializable]
    public class HO_LocationSettingItem
    {
        public HO_LocationSetting core;

        public string name;

        public string GetName
        {
            get
            {
                return name.Split( '@' )[ 0 ];
            }
        }
        public GameObject collider;
        public Sprite silhuette;
    }
}
