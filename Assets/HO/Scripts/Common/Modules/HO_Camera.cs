using System.Collections.Generic;
using HO.Scripts.Common.Base.Input;
using UnityEngine;

namespace HOSystem
{
    public class HO_Camera:HO_Module, IHOCamera
    {
        public const float BORDERBASEX = 12.47f;
        public const float BORDERBASEY = 7.67f;
        public const float BORDEROWERRIDEY = 5.75f;
        public const float BORDEROWERRIDEX = 10.3f;

        public const float INERTIAFRICTION = 0.9f;

        public Camera GetCamera { get { return Camera.main; } }

        public float GetOrthoSize { get { return GetCamera.orthographicSize; } }

        private CameraInfo Info;

        private Vector3 NeedPosition = Vector3.zero;
        private Vector2 Speed;
        private bool inertiaEnable;
        private Vector2 velocityDelta;
        private bool isZooming = false;
        
        private bool isPlay
        {
            get { return Manager.IsPlay; }
        }

        public override void Init(IHOManager Manager)
        {
            base.Init( Manager );
            Manager.AddToUpdate( this );

            InitInput();
            SetOffsetPosition( Manager.LevelData.Offset );
        }

        protected virtual void InitInput()
        {
            GameInput.Instance.OnTouchMoved += Move;
            GameInput.Instance.OnPinch += ScrollAndZoom;
            //InputManager.Instance.Default.onScroll += ScrollAndZoom;
            GameInput.Instance.OnTouchEnded += OnTouchEnded;
        }

        public void SetOffsetPosition(float y)
        {
            NeedPosition.y = y;
            NeedPosition.z = -5;
            InitInfo();
            ZoomFromPoint( Vector3.zero, -1000 );
            CalculateMovedRange( true );
        }

        private void InitInfo()
        {
            Info = new CameraInfo();
            Info.Offset = NeedPosition.y;
            InfoBordersInit();
            InfoOrtoSizeInit();

        }

        private void InfoBordersInit()
        {
            Info.Border = Vector2.zero;
            Info.Border.x = ( GetCamera.aspect <= 1.78f ) ? BORDEROWERRIDEX : BORDERBASEX;
            Info.Border.y = ( GetCamera.aspect > 1.78f ) ? BORDEROWERRIDEY : BORDERBASEY;
        }

        private void InfoOrtoSizeInit()
        {
            Vector3 originalSize = GetCamera.ViewportToWorldPoint( new Vector3( 1, 1 ) ) * 2f;
            float _width = Info.Border.x *2f;
            float _height = Info.Border.y *2f;
            Vector3 _size = new Vector3( _width, _height, originalSize.z );

            Info.OrthoSizeMax = ( GetOrthoSize * ( _size.x / originalSize.x ) ) * 0.9f;
            ;
            Info.OrthoSizeMin = Info.OrthoSizeMax / 2f;
        }

        private void OnTouchEnded()
        {
            if (!isPlay)
                return;
            inertiaEnable = true;
        }

        private void UpdateSpeed()
        {
            Vector3 pos1 = Camera.main.ScreenToWorldPoint( new Vector3( 0, 0, GetCamera.nearClipPlane ) );
            Vector3 pos2 = Camera.main.ScreenToWorldPoint( new Vector3( 1, 1, GetCamera.nearClipPlane ) );

            Speed = new Vector2( pos2.x - pos1.x, pos2.y - pos1.y );
        }

        public void Move(Vector2 delta)
        {
            if (!isPlay)
                return;
            inertiaEnable = false;
            delta *= .8f;
            velocityDelta = delta;
            NeedPosition = new Vector3( Camera.main.transform.position.x - delta.x * Speed.x, Camera.main.transform.position.y - delta.y * Speed.y, Camera.main.transform.position.z );
            CalculateMovedRange( true );
        }

        private void MoveByInertia()
        {
            if (inertiaEnable)
            {
                NeedPosition = new Vector3( NeedPosition.x - velocityDelta.x * Speed.x, NeedPosition.y - velocityDelta.y * Speed.y, GetCamera.transform.position.z );

                velocityDelta = velocityDelta * INERTIAFRICTION;
                CalculateMovedRange( true );
            }
        }

        private void MoveUpdate()
        {
            GetCamera.transform.position = Vector3.MoveTowards( GetCamera.transform.position, NeedPosition, Time.deltaTime * 10f );
        }

        private void CalculateMovedRange(bool fast = false)
        {
            Vector3 prev = GetCamera.transform.position;
            GetCamera.transform.position = NeedPosition;

            Vector3 LT = GetCamera.ViewportToWorldPoint( new Vector3( 0, 1 ) );
            Vector3 RB = GetCamera.ViewportToWorldPoint( new Vector3( 1, 0 ) );

            if (LT.x < -Info.Border.x)
                NeedPosition.x = NeedPosition.x - LT.x - Info.Border.x;
            if (LT.y > Info.Border.y)
                NeedPosition.y = NeedPosition.y - LT.y + Info.Border.y;
            if (RB.x > Info.Border.x)
                NeedPosition.x = NeedPosition.x - RB.x + Info.Border.x;
            if (RB.y < -Info.Border.y)
                NeedPosition.y = NeedPosition.y - RB.y - Info.Border.y;

            if (fast)
                GetCamera.transform.position = NeedPosition;
            else
            {
                GetCamera.transform.position = prev;
            }
        }

        public void MoveToTarget(Transform target)
        {
            NeedPosition = target.position;
            NeedPosition.z = GetCamera.transform.position.z;
            CalculateMovedRange( false );
            inertiaEnable = false;
        }

        public void ScrollAndZoom(float delta, List<Vector2> touchPoints)
        {
            if (!isPlay)
                return;

            inertiaEnable = false;
            Vector2 zoomTowards = ( touchPoints[ 0 ] + touchPoints[ 1 ] ) / 2;

            zoomTowards = GetCamera.ScreenToWorldPoint( zoomTowards );
            //delta *= -0.01f;
            ZoomFromPoint( zoomTowards, delta );
        }

        public void ZoomFromPoint(Vector3 zoomPoint, float delta)
        {
            zoomPoint.z = GetCamera.transform.position.z;

            float newZoomValue = GetCamera.orthographicSize - delta;

            if (newZoomValue < Info.OrthoSizeMin)
            {
                delta = GetCamera.orthographicSize - Info.OrthoSizeMin;
                newZoomValue = GetCamera.orthographicSize - delta;
            }

            if (newZoomValue > Info.OrthoSizeMax)
            {
                delta = GetCamera.orthographicSize - Info.OrthoSizeMax;
                newZoomValue = GetCamera.orthographicSize - delta;
            }

            if (newZoomValue < Info.OrthoSizeMin || newZoomValue > Info.OrthoSizeMax)
            {
                return;
            }

            float multiplier = ( 1.0f / GetCamera.orthographicSize * delta );
            NeedPosition += ( zoomPoint - GetCamera.transform.position ) * multiplier;

            GetCamera.orthographicSize -= delta;
            GetCamera.orthographicSize = Mathf.Clamp( GetCamera.orthographicSize, Info.OrthoSizeMin, Info.OrthoSizeMax );

            CalculateMovedRange( true );
            UpdateSpeed();
        }

        public void SetMaxZoom()
        {
            isZooming = true;
        }

        private void ZoomUpdate()
        {
            if (isZooming)
            {
                ZoomFromPoint( GetCamera.transform.position, -0.06f);

                if (GetCamera.orthographicSize == Info.OrthoSizeMax)
                {
                    isZooming = false;
                }
            }
        }

        public override void Send(HOMessage mess)
        {
            
        }

        public override void Next()
        {
            DebugDrawBorder();
            MoveByInertia();
            ZoomUpdate();
            MoveUpdate();
        }

        #region TOOLS
        private void DebugDrawBorder()
        {
#if UNITY_EDITOR
            var tl = new Vector3( -Info.Border.x, Info.Border.y );
            var tr = new Vector3( Info.Border.x, Info.Border.y );
            var bl = new Vector3( -Info.Border.x, -Info.Border.y );
            var br = new Vector3( Info.Border.x, -Info.Border.y );

            Debug.DrawLine( tl, tr, Color.red );
            Debug.DrawLine( tr, br, Color.red );
            Debug.DrawLine( br, bl, Color.red );
            Debug.DrawLine( bl, tl, Color.red );
#endif
        }
        #endregion
    }
}
