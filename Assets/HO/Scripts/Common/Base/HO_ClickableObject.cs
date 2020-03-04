using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HOSystem
{
    public class HO_ClickableObject:HO_EventUserBehaviour, IHOClickableObject
    {
        private const float ClickTimeDelay = .2f;
        private static HO_ClickableObject LastClickedObject = null;
        private static float LastClickTime = 0;

        protected IHOEventUser core;

        public virtual void Init(IHOEventUser connect)
        {
            core = connect;
        }

        public override void Send(HOMessage mess)
        {
            switch ((HOMessageType)mess.key)
            {
                case HOMessageType.Click:
                    OnClick();
                break;
            }
        }

        protected virtual bool CheckClick()
        {
            return !IsLockedClick();
        }

        protected virtual void OnClick()
        {
            if (!CheckClick())
                return;

            SendClickMessage();
        }

        protected virtual void SendClickMessage()
        {
            if (core != null)
                core.Send( HOMessageType.Click );
        }

        protected bool IsLockedClick()
        {
            if (( LastClickTime + ClickTimeDelay ) < Time.time)
            {
                LastClickTime = Time.time;
                return false;
            }
            return true;
        }
    }
}
