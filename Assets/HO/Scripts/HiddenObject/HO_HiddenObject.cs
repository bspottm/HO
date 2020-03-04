using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public class HO_HiddenObject:HO_EventUserBehaviour, IHOHiddenObject
    {
        public const int FLYSORTINGORDERFORIMAGE = 1001;
        private const float FLYSPEED = 12;
        private const float FLYSCALEFACTOR = 0.4f;
        public const int HIGHTLIGHTORDER = 300;
        public const float HIGLIGHTFACTOR = 0.3f;

        public IHOManager Manager { get; private set; }
        public string Key { get; set; }
        public string Difficulty;

        public GameObject Image { get; set; }
        public Action OnFind { get; set; }

        public bool IsFind { get; private set; }
        private SpriteRenderer ImageRenderer;
        private Material ImageMaterial;

        public GameObject ShadowGO;
        public GameObject OverGO;
        public Collider2D Collider;

        private float FlyDistance = 0;
        private Transform FlyTarget;
        private BezierAnimations Bezier;
        private bool IsFly = false;

        public void Init(IHOManager manager)
        {
            Manager = manager;
            ImageRenderer = Image.GetComponent<SpriteRenderer>();
            ImageMaterial = ImageRenderer.material;
            SendRegistration();
            IsFind = false;
        }

        private void SendRegistration()
        {
            HOMessage _mess = CreateMessage( HOMessageType.HiddenObjectRegistration );
            _mess.hash.Add( "Item", this as IHOHiddenObject );
            Manager.Send( _mess );
        }

        public override void Send(HOMessage mess)
        {
            switch (mess.key)
            {
                case ( int )HOMessageType.Click:
                OnClick();
                break;
                case ( int )HOMessageType.Find:
                {
                    if (!mess.hash.ContainsKey( "Item" ))
                        break;
                    Find( mess.hash[ "Item" ] as IHOPanelItemSlot );

                }
                break;
            }
        }

        private void OnClick()
        {
            SendFindMessage( "Check" );
        }

        #region Find
        private void Find(IHOPanelItemSlot slot)
        {
            if (slot == null)
                return;

            OnFind?.Invoke();
            IsFind = true;
            FlyTarget = slot.GameObject.transform;
            FlyStart();
        }

        private void SendFindMessage(string status)
        {
            HOMessage _mess = CreateMessage( HOMessageType.Find );
            _mess.hash.Add( "Item", this as IHOHiddenObject );
            _mess.hash.Add( "Status", status );
            Manager.Send( _mess );
        }

        private void FlyStart()
        {
            PrepareItemToFly();

            FlyDistance = Vector3.Distance( Image.transform.position, FlyTarget.position );

            Manager.OnUpdate += HighlightImage;
        }

        private void PrepareItemToFly()
        {
            PrepareDecorations();
            PrepareImage();
        }

        private void PrepareDecorations()
        {
            if (Collider != null)
                Collider.enabled = false;

            ShadowGO?.SetActive( false );
            OverGO?.SetActive( false );
        }

        private void PrepareImage()
        {
            ImageRenderer.sortingOrder = FLYSORTINGORDERFORIMAGE;

        }

        private void InitBezier()
        {
            Bezier = Image.AddComponent<BezierAnimations>();
            Bezier.IsAutoUpdate = false;
            Bezier.Init( FlyTarget, FLYSPEED * 1000, 5f, 2f );
            Bezier.OnFinish = (a) => { FlyFinish(); };
        }

        private void HighlightImage()
        {
            _colorFactor += Time.deltaTime;
            if (_colorFactor > HIGLIGHTFACTOR)
            {
                InitBezier();
                Manager.OnUpdate -= HighlightImage;
                Manager.OnUpdate += Fly;
            }
            ImageMaterial.color = Color.white * ( 1f + _colorFactor );
        }
        float _colorFactor = 0;

        private void Fly()
        {
            Bezier.UpdateFinishPosition( FlyTarget.position );
            float _distance = Vector3.Distance( transform.position, FlyTarget.position );

            transform.localScale = Vector3.one * ( _distance / FlyDistance );
            Bezier.Next();
        }

        private void ScaleImage()
        {
            var _scale = Image.transform.localScale.x - 5f * Time.deltaTime;
            if (_scale <= 0)
            {
                _scale = 0;
                Manager.OnUpdate -= ScaleImage;
                SendFindMessage( "Find" );
                Destroy( gameObject );
            }
            Image.transform.position = FlyTarget.position;
            Image.transform.localScale = new Vector3( _scale, _scale );
        }

        private void FlyFinish()
        {
            Manager.OnUpdate -= Fly;
            Manager.OnUpdate += ScaleImage;
        }

        #endregion
    }
}
