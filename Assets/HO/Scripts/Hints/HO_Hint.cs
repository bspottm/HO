using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HOSystem
{
    public class HO_Hint:HO_EventUserBehaviour, IHOHint
    {
        private const float FLYSPEED = 12;

        public IHOManager Manager { get; private set; }
        private HOHintType hintType;

        List<BezierAnimations> beziers = new List<BezierAnimations>();

        public virtual void Init(IHOManager manager, List<IHOHiddenObject> items)
        {
            hintType = HOHintType.Eye;
            Manager = manager;

            var _findItems = GetUseItems( items );
            for (int i = 0; i < _findItems.Count; i++)
            {
                InitEffect( _findItems[ i ] );
            }

            if (beziers.Count > 0)
                Manager.OnUpdate += HintUpdate;
        }

        private void InitEffect(IHOHiddenObject item)
        {
            var _effect = CreateHintEffect( hintType );
            if (_effect == null)
            {
                UseItem( item );
                return;
            }

            var _bezier = SetBezier( _effect.gameObject, item.Image.transform, () => { UseItem( item ); FinishEffect( _effect ); } );

            beziers.Add( _bezier );
        }

        private void FinishEffect(ParticleSystem effect)
        {
            effect.Stop();
            Destroy( effect.gameObject, .5f );
        }


        protected virtual List<IHOHiddenObject> GetUseItems(List<IHOHiddenObject> items)
        {
            var _items = new List<IHOHiddenObject>();
            _items.Add( items[ Random.Range( 0, items.Count ) ] );

            return _items;
        }


        protected virtual ParticleSystem CreateHintEffect(HOHintType type)
        {
            string _prefabName = GetEffectPrefabName( type );

            if (string.IsNullOrEmpty( _prefabName ))
                return null;

            var _effectGO = LoadEffectPrefab( _prefabName );
            if (_effectGO == null)
                return null;

            var _buttonGO = GetHintButton( type );

            _effectGO.transform.position = ( _buttonGO == null ) ? Input.mousePosition : _buttonGO.transform.position;

            var _effect = _effectGO.GetComponent<ParticleSystem>();

            return _effect;
        }

        private string GetEffectPrefabName(HOHintType type)
        {
            string _name = "";
            switch (type)
            {
                case HOHintType.Eye:
                {
                    _name = "HUD_HintEffectEye";
                }
                break;
                case HOHintType.Bomb:
                {
                    _name = "HUD_HintEffectBomb";
                }
                break;
                case HOHintType.Compass:
                {
                    _name = "HUD_HintEffectCompas";
                }
                break;
            }

            return _name;
        }

        private GameObject LoadEffectPrefab(string name)
        {
            if (!Manager.LoadedItems.HasItem( name ))
                return null;

            var _effectGO = Instantiate( Manager.LoadedItems.GetItem( name ) );
            return _effectGO;

        }

        private GameObject GetHintButton(HOHintType type)
        {
            var _button = HO_UiItem.GetElement( string.Format( "Button_Hint_{0}", type.ToString() ) );
            return _button;
        }

        protected virtual void UseItem(IHOHiddenObject item)
        {
            if (item == null || item.IsFind)
                return;

            Log( string.Format( "Eye_{0}", item.Key ) );

            var _tween = item.Image.AddComponent<HO_HintHighlightTween>();
            item.OnFind += _tween.StopTween;
            _tween.Init( Manager );
        }

        protected virtual void HintUpdate()
        {
            for(int i = beziers.Count -1; i>=0;i--)
            {
                if (beziers[ i ] == null)
                {
                    beziers.RemoveAt( i );
                    continue;
                }

                if (beziers[ i ].IsFinished)
                {
                    beziers.RemoveAt( i );
                    continue;
                }

                beziers[ i ].Next();
            }

            if(beziers.Count==0)
            {
                End();
            }
        }

        private void End()
        {
            Manager.OnUpdate -= HintUpdate;
            Destroy( gameObject, .5f );

            Manager.Send( HOMessageType.HintFinish );
        }

        private BezierAnimations SetBezier(GameObject obj, Transform target, Action finishCallback)
        {
            var _bezier = obj.AddComponent<BezierAnimations>();
            _bezier.IsAutoUpdate = false;
            _bezier.Init( target, FLYSPEED * 1000, 5f, 2f );
            _bezier.OnFinish = (a) => { finishCallback?.Invoke(); };
            return _bezier;
        }
    }
}
