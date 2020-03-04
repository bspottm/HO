using System;
using System.Collections;
using System.Collections.Generic;
using UITween;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public abstract class HO_Panel_Items_Slot:HO_EventUserBehaviour, IHOPanelItemSlot
    {
        protected ParticleSystem EffectAppear;
        protected ParticleSystem EffectDisappear;
        protected UITweenScale TweenScale;


        public string ItemKey { get; private set; }
        public bool IsLock { get; private set; }

    IHOPanelItem core;

        public IHOManager Manager
        {
            get
            {
                if (core == null)
                    return null;
                return core.Manager;
            }
        }

        protected Transform Root;
        protected Transform Separator;


        public virtual void Init(IHOPanelItem core)
        {
            this.core = core;
            Root = transform.Find( "Root" );
            Separator = transform.Find( "Separator" );

            InitTweenScale();
            EffectAppear = InitEffects( "HUD_SlotEffectAppear" );
            EffectDisappear = InitEffects( "HUD_SlotEffectDisappear" );
        }

        protected T CreateItem<T>(string name) where T : MonoBehaviour
        {
            var _comp =  Root.gameObject.AddComponent<T>();
            var _rect = Root.GetComponent<RectTransform>();
            _rect.anchorMin = Vector2.zero;
            _rect.anchorMax = Vector2.one;
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
            return _comp;
        }

        protected virtual void InitTweenScale()
        {
            TweenScale = Root.gameObject.AddComponent<UITweenScale>();
            TweenScale.Duration = .5f;
            TweenScale.from = Vector2.zero;
            TweenScale.to = Vector2.one;
            TweenScale.ResetToBegenin();
        }

        protected virtual ParticleSystem InitEffects(string loadItemKey)
        {
            var _go = Instantiate( Manager.LoadedItems.GetItem( loadItemKey ), transform );
            _go.AddComponent<UIParticleAutodisable>();
            _go.SetActive( false );
            var _particle = _go.GetComponent<ParticleSystem>();
            _particle.Stop();
            return _particle;
        }

        public virtual void Receive(HOMessage mess)
        {

        }

        public override void Send(HOMessage mess)
        {
            switch (mess.key)
            {
                case ( int )HOMessageType.RemoveSeparator:
                {
                    if(Separator!=null)
                    Separator.gameObject.SetActive( false );
                }
                break;
            }
        }

        
        public virtual void SetKey(string key)
        {
            IsLock = false;
            ItemKey = key;
            Animate();
        }

        protected abstract void ApproveItem();
        protected abstract bool IsEmpty();

        protected virtual void Animate()
        {
            if (IsEmpty())
            {
                AnimateAppear();
            }
            else
            {
                AnimateDisappear( AnimateAppear );
            }
        }

        protected virtual void AnimateDisappear(Action OnFinish)
        {
            EffectDisappear.gameObject.SetActive( true );
            TweenScale.PlayBackward();

            if (OnFinish != null)
            {
                TweenScale.OnFinish += OnFinish;
            }
        }

        protected virtual void AnimateAppear()
        {
            ApproveItem();

            if (IsEmpty())
                return;

            EffectAppear.gameObject.SetActive( true );
            TweenScale.PlayForward();
        }
    }
}
