using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HOSystem
{
    public class HO_Panel_HiddenObjects:HO_PanelItem
    {
        public const int SLOTSCOUNT = 4;
        private List<GameObject> SlotObjects;
        private List<IHOPanelItemSlot> Slots;
        private List<string> HiddenObjects;
        private bool IsSlotsInit = false;
        private int itemsAll = 0;
        private int itemsFinded = 0;

        private void SlotsInit()
        {
            SlotObjects = new List<GameObject>();
            for(int i = 0; i<SLOTSCOUNT; i++ )
            {
                var _slotGO = Instantiate(Manager.LoadedItems.GetItem( "Slot" ), transform);
                SlotObjects.Add( _slotGO );
            }

            Slots = new List<IHOPanelItemSlot>();
            for(int i = 0; i< SlotObjects.Count;i++)
            {
                var _slot = HO_ItemSpotFactory.GetSlot( Manager.LevelData.Mode, SlotObjects[ i ] );
                _slot.Init( this );
                if (i == SlotObjects.Count - 1)
                {
                    _slot.Send( HOMessageType.RemoveSeparator );
                }
                Slots.Add( _slot);
            }
            StartCoroutine( SetKeys() );
        }

        private IEnumerator SetKeys()
        {
            yield return new WaitForSeconds( .5f );
            for (int i = 0; i < Slots.Count; i++)
            {

                SetNextKey( Slots[ i ] );
            }

            IsSlotsInit = true;
        }

        private void SetNextKey(IHOPanelItemSlot slot)
        {
            string _key = "";
            if(HiddenObjects.Count>0)
            {
                _key = HiddenObjects.First();
                HiddenObjects.RemoveAt( 0 );
            }
            slot.SetKey( _key);
            
        }

        public string GetItemBySlot(int slotIndex)
        {
            return "";
        }

        public IHOPanelItemSlot GetSlotByIndex(int index) 
        {
            return Slots[ index ];
        }

        public IHOPanelItemSlot GetSlotByItem(string item)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[ i ].ItemKey == item)
                 {
                    return Slots[ i ];
                }
            }
            return null;
        }

        public override void Send(HOMessage mess)
        {
            if (!IsSlotsInit && mess.key != ( int )HOMessageType.SetHiddenObjectList)
                return;

            switch ((HOMessageType)mess.key)
            {
                case HOMessageType.SetHiddenObjectList:
                {
                    HiddenObjects = new List<string>();
                        for (int i = 0; i < mess.hash.Count; i++)
                        {
                        HiddenObjects.Add( mess.hash[ i ].ToString() );
                        }
                    itemsAll = HiddenObjects.Count;
                    SlotsInit();
                }
                break;

                case HOMessageType.Find:
                CheckFindedItem( mess );
                break;

                case HOMessageType.QueryPanelHiddenObject:
                ReadQueryHiddenObject( mess );
                break;
            }
        }

        private void CheckFindedItem(HOMessage mess)
        {
            if (!mess.hash.ContainsKey( "Status" ))
                return;

            if (!mess.hash.ContainsKey( "Item" ))
                return;

            var _item = mess.hash[ "Item" ] as IHOHiddenObject;

            if (_item == null)
                return;

            switch (mess.hash[ "Status" ])
            {
                case "Check":
                {
                    CheckItem( _item );
                }
                break;
                case "Find":
                {
                    UpdateSlot( _item.Key );
                    itemsFinded++;
                    float _progress = itemsFinded / (float)itemsAll;
                    HOMessage _mess = CreateMessage( HOMessageType.LevelProgressBarUpdate );
                    _mess.hash.Add( "Value", _progress );
                    core.Send( _mess );

                }
                break;
            }
        }

        private void CheckItem(IHOHiddenObject item)
        {
            if (item == null)
                return;

            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[ i ].IsLock)
                    continue;

                if(Slots[i].ItemKey == item.Key)
                {
                    HOMessage _mess = CreateMessage( HOMessageType.Find );
                    _mess.hash.Add("Item", Slots[ i ] as IHOPanelItemSlot );
                    item.Send( _mess );
                    Slots[ i ].Send( HOMessageType.Find );
                    return;
                }
            }

            Manager.Send( HOMessageType.FailClick );
        }

        private void UpdateSlot(string key)
        {
            var _slot = GetSlotByItem( key );
            if (_slot == null)
                return;

            SetNextKey( _slot );
        }

        private void ReadQueryHiddenObject(HOMessage mess)
        {
            if (mess.hash[ "Type" ].ToString() != "Get")
                return;

            var _mess = mess.Clone();

            _mess.hash[ "Type" ] = "Return";
            for(int i = 0; i<Slots.Count;i++)
            {
                if (string.IsNullOrEmpty( Slots[ i ].ItemKey ))
                    continue;

                _mess.hash.Add( string.Format( "item_{0}", i ), Slots[ i ].ItemKey );
            }
            core.Send( _mess );
        }
    }
}
