using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public class HO_ButtonHint:HO_Button
    {
        [SerializeField]
        private HOHintType hintType;
        private bool isLock = true;
        [SerializeField]
        private GameObject[] itemsForLock;
        [SerializeField]
        private GameObject[] itemsForUnlock;
        [SerializeField]
        private TextMeshProUGUI amountLabel;
        private int amountHints = 0;

        private void ActivateItems()
        {
            if (itemsForLock != null && itemsForLock.Length > 0)
            {
                for (int i = 0; i < itemsForLock.Length; i++)
                {
                    itemsForLock[ i ]?.SetActive( isLock );
                }
            }

            if (itemsForUnlock != null && itemsForUnlock.Length > 0)
            {
                for (int i = 0; i < itemsForUnlock.Length; i++)
                {
                    itemsForUnlock[ i ]?.SetActive( !isLock );
                }
            }
        }

        public override void Init(IHOEventUser connect)
        {
            base.Init( connect );
            ActivateItems();
        }

        public override void Send(HOMessage mess)
        {
            switch (mess.key)
            {
                case ( int )HOMessageType.HintButtonSetLock:
                {
#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif
                    isLock = ( mess.hash.ContainsKey( "IsLock" ) ) ? ( bool )mess.hash[ "IsLock" ] : true;

                    ActivateItems();
                }
                break;

                case ( int )HOMessageType.HintSetAmount:
                {
                    Log( "Update " + hintType.ToString() );
#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif
                    amountHints = ( mess.hash.ContainsKey( "Value" ) ) ? ( int )mess.hash[ "Value" ] : 0;
                    amountLabel.text = amountHints.ToString();
                }
                break;

            }

            base.Send( mess );
        }

        protected override bool CheckClick()
        {
            if (!base.CheckClick())
                return false;

            if (hintType == HOHintType.None)
                return false;

            if (isLock)
                return false;

            return true;
        }

        protected override void SendClickMessage()
        {
            HOMessage _mess = CreateMessage( HOMessageType.Click );
            _mess.hash.Add( "Command", command );
            _mess.hash.Add( "Hint", hintType );
            core.Send( _mess );
        }

        [ContextMenu("Click")]
        public void TestClick()
        {
            SendClickMessage();
        }
    }
}