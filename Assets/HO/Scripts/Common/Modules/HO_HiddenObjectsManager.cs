using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_HiddenObjectsManager:HO_Module
    {
        private Dictionary<string, IHOHiddenObject> hiddenObjects;

        public override void Init(IHOManager Manager)
        {
            base.Init( Manager );
            hiddenObjects = new Dictionary<string, IHOHiddenObject>();
        }

        public override void Send(HOMessage mess)
        {
            switch (( HOMessageType )mess.key)
            {
                case HOMessageType.HiddenObjectRegistration:
                {
                    if (!mess.hash.ContainsKey( "Item" ))
                        break;

#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif

                    var _item = mess.hash[ "Item" ] as IHOHiddenObject;

                    if (hiddenObjects.ContainsKey( _item.Key ))
                        break;

                    hiddenObjects.Add( _item.Key, _item );
                }
                break;

                case HOMessageType.QueryHiddenObject:
                {
                    ReadQueryHiddenObject( mess );
                }
                break;
            }
        }

        private void ReadQueryHiddenObject(HOMessage mess)
        {
            if (!mess.hash.ContainsKey( "Type" ))
                return;

            if (mess.hash[ "Type" ].ToString() != "Get")
                return;

#if UNITY_EDITOR
            mess.AddTracePoint( this );
#endif

            var _mess = mess.Clone();

            for (int i = 0; i< HO_Panel_HiddenObjects.SLOTSCOUNT; i++)
            {
                var _hashkey = string.Format( "Get_item_{0}", i );

                if (!_mess.hash.ContainsKey( _hashkey ))
                    continue;

                var _itemkey = _mess.hash[ _hashkey ].ToString();

                _mess.hash.Remove( _hashkey );
                if (!hiddenObjects.ContainsKey( _itemkey ))
                    continue;

                _hashkey = string.Format( "Return_item_{0}", i );

                _mess.hash.Add( _hashkey, hiddenObjects[ _itemkey ] as IHOHiddenObject );
            }

            _mess.hash[ "Type" ] = "Return";

            Manager.Send( _mess );
        }


    }
}
