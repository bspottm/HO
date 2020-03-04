using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_Panel_Hints:HO_PanelItem
    {

        private Dictionary<HOHintType, IHOClickableObject> hintsButton;

        public override void Init(IHOModuleConnector core)
        {
            base.Init( core );
            hintsButton = new Dictionary<HOHintType, IHOClickableObject>();

            HintButtonCreate( HOHintType.Eye );
            HintButtonCreate( HOHintType.Bomb );
            HintButtonCreate( HOHintType.Compass );
        }

        private void HintButtonCreate(HOHintType type)
        {
            if (type == HOHintType.None)
                return;

            if (hintsButton.ContainsKey( type ))
                return;

            string _name = GetButtonPrefabName( type );

            if (string.IsNullOrEmpty( _name ))
                return;

            if (!Manager.LoadedItems.HasItem( _name ))
                return;

            var _go = Instantiate( Manager.LoadedItems.GetItem( _name ), transform );
            var _item = _go.GetComponent<IHOClickableObject>();
            var _itemMarker = _go.AddComponent<HO_UiItem>();
            _itemMarker.SetName( string.Format( "Button_Hint_{0}", type.ToString() ) );
            _item.Init( this );
            hintsButton.Add( type, _item );
        }

        private string GetButtonPrefabName(HOHintType type)
        {
            string _name = "";
            switch (type)
            {
                case HOHintType.Eye:
                {
                    _name = "HUD_HintEye";
                }
                break;
                case HOHintType.Bomb:
                {
                    _name = "HUD_HintBomb";
                }
                break;
                case HOHintType.Compass:
                {
                    _name = "HUD_HintCompas";
                }
                break;
            }

            return _name;
        }

        public override void Send(HOMessage mess)
        {
            ReadMessageAlways( mess );

            if (core.Manager.IsPlay)
                ReadMessageInPlay( mess );
        }

        private void ReadMessageInPlay(HOMessage mess)
        {
            switch (( HOMessageType )mess.key)
            {
                case HOMessageType.Click:
                {
#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif
                    ReadClickMessage( mess );
                }
                break;
            }
        }

        private void ReadMessageAlways(HOMessage mess)
        {
            switch (( HOMessageType )mess.key)
            {
                case HOMessageType.HintButtonSetLock:
                {
#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif
                    ReadMessageForUnlockHintsButtons( mess );
                }
                break;
                case HOMessageType.HintSetAmount:
                {
#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif
                    ReadMessageForUpdateHintsAmount( mess );
                }
                break;
            }
        }

        private void ReadClickMessage(HOMessage mess)
        {
            if (!mess.hash.ContainsKey( "Command" ))
                return;

            if (( HOClickCommand )mess.hash[ "Command" ] != HOClickCommand.Hint)
                return;

            if (!mess.hash.ContainsKey( "Hint" ))
                return;

            core.Send( mess );
        }

        private void ReadMessageForUnlockHintsButtons(HOMessage mess)
        {
            for (int i = 0; i < Enum.GetValues( typeof( HOHintType ) ).Length; i++)
            {
                var _hint = ( HOHintType )i;

                if (!mess.hash.ContainsKey( _hint.ToString() ))
                    continue;

                if (!hintsButton.ContainsKey( _hint ))
                    continue;

                HOMessage _mess = CreateMessage( HOMessageType.HintButtonSetLock );
                bool _isUnlock = ( bool )mess.hash[ _hint.ToString() ];
                _mess.hash.Add( "IsLock", !_isUnlock);
                hintsButton[ _hint ].Send( _mess );
            }
        }

        private void ReadMessageForUpdateHintsAmount(HOMessage mess)
        {

            for (int i = 0; i < Enum.GetValues( typeof( HOHintType ) ).Length; i++)
            {
                var _hint = ( HOHintType )i;

                if (!mess.hash.ContainsKey( _hint.ToString() ))
                    continue;

                if (!hintsButton.ContainsKey( _hint ))
                    continue;

                HOMessage _mess = CreateMessage( HOMessageType.HintSetAmount );
                _mess.hash.Add( "Value", mess.hash[ _hint.ToString() ] );
                hintsButton[ _hint ].Send( _mess );
            }
        }

    }
}
