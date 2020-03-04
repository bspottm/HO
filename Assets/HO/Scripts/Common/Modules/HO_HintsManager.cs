using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_HintsManager:HO_Module
    {
        private Dictionary<HOHintType, HOHintData> Hints;
        private bool isHintUse = false;


        public override void Init(IHOManager Manager)
        {
            base.Init( Manager );

            //TODO TEST
            #region FakeStates
            SetLockHint( HOHintType.Eye, false );
            SetLockHint( HOHintType.Bomb, true );
            SetLockHint( HOHintType.Compass, true );

            SetHintAmount( HOHintType.Eye, 10 );
            #endregion

            InitHintsData();
            EnableButtonsForHints();
            Manager.PlayerData.OnUpdate += EnableButtonsForHints;
            UpdateHintsAmount();
            Manager.InvectoryData.OnUpdate += UpdateHintsAmount;
        }

        private void InitHintsData()
        {
            Hints = new Dictionary<HOHintType, HOHintData>();
            for (int i = 0; i < Enum.GetValues( typeof( HOHintType ) ).Length; i++)
            {
                var _hint = ( HOHintType )i;
                Hints.Add( _hint, new HOHintData() { IsEnable = false, Amount = 0 } );
            }
        }

        private void EnableButtonsForHints()
        {
            HOMessage _mess = CreateMessage( HOMessageType.HintButtonSetLock );

            for(int i = 0; i<Enum.GetValues(typeof(HOHintType)).Length;i++)
            {
                var _hint = ( HOHintType )i;
                var _key = string.Format( "HOPAHintEnabled:Hint{0}", _hint.ToString() );
                bool _isEnabled = Manager.PlayerData.GetState( _key, "0" ) == "1";
                Hints[_hint].IsEnable = _isEnabled;

                _mess.hash.Add( _hint.ToString(), _isEnabled );
            }

            Manager.Send( _mess );
        }

        private void UpdateHintsAmount()
        {
            HOMessage _mess = CreateMessage( HOMessageType.HintSetAmount );

            for (int i = 0; i < Enum.GetValues( typeof( HOHintType ) ).Length; i++)
            {
                var _hint = ( HOHintType )i;
                var _key = string.Format( "HINT_{0}", _hint.ToString().ToUpper() );
                int _amount = Manager.InvectoryData.GetItemCount( _key );
                Hints[ _hint ].Amount = _amount;
                _mess.hash.Add( _hint.ToString(), _amount );
            }

            Manager.Send( _mess );
        }

        private void SetLockHint(HOHintType hint, bool isLock)
        {
            var _key = string.Format( "HOPAHintEnabled:Hint{0}", hint.ToString() );

            var _value = ( isLock ) ? "0" : "1";
            Manager.PlayerData.SetState( _key, _value);
        }

        private void SetHintAmount(HOHintType hint, int value)
        {
            var _key = string.Format( "HINT_{0}", hint.ToString().ToUpper() );

            int _amount = value - Manager.InvectoryData.GetItemCount( _key );
            Manager.InvectoryData.AddItem( _key, _amount );
        }

        public override void Send(HOMessage mess)
        {
            switch (( HOMessageType )mess.key)
            {
                case HOMessageType.HintUse:
                {
#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif

                    if (isHintUse)
                        break;

                    HOHintType _hint = GetHintFromMessage( mess );
                    if (_hint == HOHintType.None)
                        break;

                    if (!Hints[ _hint ].IsEnable)
                        break;

                    if(Hints[_hint].Amount<=0)
                    {
                        Log( string.Format( "Not enought hints {0} ", _hint.ToString() ));
                        break;
                    }

                    Log( string.Format( "Use {0}", _hint ) );

                    HOMessage _mess = CreateMessage( HOMessageType.QueryPanelHiddenObject );
                    _mess.hash.Add( "Hint", ( int )_hint );
                    _mess.hash.Add( "Type", "Get" );
                    Manager.Send( _mess );
                }
                break;
                case HOMessageType.QueryPanelHiddenObject:
                {
                    ReadQueryPanelHiddenObject( mess );
                }
                break;
                case HOMessageType.QueryHiddenObject:
                {
                    ReadQueryHiddenObjectMessage( mess );
                }
                break;
                case HOMessageType.HintFinish:
                {
                    isHintUse = false;
                }
                break;


            }
        }

        private void ReadQueryPanelHiddenObject(HOMessage mess)
        {
            if (mess.hash[ "Type" ].ToString() != "Set")
                return;

            HOHintType _hintType = GetHintFromMessage( mess );
            if (_hintType == HOHintType.None)
                return;
#if UNITY_EDITOR
            mess.AddTracePoint( this );
#endif
            var _items = new List<string>();

            for (int i = 0; i <= 4; i++)
            {
                var _hashKey = string.Format( "item_{0}", i );
                if (mess.hash.ContainsKey( _hashKey ))
                {
                    _items.Add( mess.hash[ _hashKey ].ToString() );
                }
            }

            if (_items.Count == 0)
                return;

            SendQueryHiddenObjectMessage( _hintType, _items );
        }

        private void SendQueryHiddenObjectMessage(HOHintType type, List<string> items)
        {
            HOMessage _mess = CreateMessage( HOMessageType.QueryHiddenObject );

            _mess.hash.Add( "Hint", ( int )type );
            _mess.hash.Add( "Type", "Get" );
            for (int i = 0; i < items.Count; i++)
            {
                var _hashKey = string.Format( "Get_item_{0}", i );
                _mess.hash.Add( _hashKey, items[ i ] );
            }
            Manager.Send( _mess );
        }

        private void ReadQueryHiddenObjectMessage(HOMessage mess)
        {
            if (mess.hash[ "Type" ].ToString() != "Return")
                return;

#if UNITY_EDITOR
            mess.AddTracePoint( this );
#endif

            HOHintType _hintType = GetHintFromMessage( mess );
            if (_hintType == HOHintType.None)
                return;

            var _items = new List<IHOHiddenObject>();

            for (int i = 0; i <= 4; i++)
            {
                var _hashKey = string.Format( "Return_item_{0}", i );
                if (mess.hash.ContainsKey( _hashKey ))
                {
                    _items.Add( mess.hash[ _hashKey ] as IHOHiddenObject );
                }
            }

            if (_items.Count == 0)
                return;

            var _hint = HO_HintFactory.GetHint( _hintType );
            _hint.Init( Manager, _items );

            var _key = string.Format( "HINT_{0}", _hintType.ToString().ToUpper() );
            Manager.InvectoryData.ApplyItem( _key );

            isHintUse = true;
        }

        private HOHintType GetHintFromMessage(HOMessage mess)
        {
            if (!mess.hash.ContainsKey( "Hint" ))
                return HOHintType.None;

            return ( HOHintType )mess.hash[ "Hint" ];
        }
    }

    public class HOHintData
    {
        public bool IsEnable;
        public int Amount;
    }
}
