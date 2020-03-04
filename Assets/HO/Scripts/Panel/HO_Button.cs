using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HOSystem
{
    public class HO_Button:HO_ClickableObject,IPointerClickHandler
    {
        public static bool isInteractive = true;
        [SerializeField]
        bool isIgnorePause = false;
        private Image[] images;
        private Coroutine corClickColor;
        private static Color clickColor = new Color( .8f, .8f, .8f );

        [SerializeField]
        protected HOClickCommand command;

        public override void Init(IHOEventUser connect)
        {
            base.Init( connect );
            images = GetComponentsInChildren<Image>();
        }

       public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        protected override bool CheckClick()
        {
            if (!base.CheckClick())
                return false;

            if (!isIgnorePause && !isInteractive)
                return false;

            if (command == HOClickCommand.None)
                return false;

            return true;
        }

        protected override void OnClick()
        {
            if (!CheckClick())
                return;
            PlayColorAnimate();
            SendClickMessage();
        }

        protected override void SendClickMessage()
        {
            HOMessage _mess = CreateMessage( HOMessageType.Click );
            _mess.hash.Add( "Command", ( int )command );
            core.Send( _mess );
        }

        protected void PlayColorAnimate()
        {
            if (corClickColor != null)
                StopCoroutine( corClickColor );

            corClickColor = StartCoroutine( AnimateColor() );
        }

        protected IEnumerator AnimateColor()
        {
            if (images == null || images.Length == 0)
                yield break;

            for (int i = 0; i < images.Length; i++)
            {
                images[ i ].color = clickColor;
            }

            yield return new WaitForSeconds( .1f );

            for (int i = 0; i < images.Length; i++)
            {
                images[ i ].color = Color.white;
            }

            corClickColor = null;
        }
    }
}
