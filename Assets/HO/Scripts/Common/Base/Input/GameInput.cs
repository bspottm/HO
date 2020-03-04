using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HO.Scripts.Common.Base.Input
{
    public interface IInput
    {
        Action OnTouchBegan { get; set; }
        Action OnTouch { get; set; }
        Action OnTouchEnded { get; set; }
        Action<Vector2> OnTouchMoved { get; set; }
        Action<SwipeDirection, float> OnSwipe { get; set; }
        Action<float, List<Vector2>> OnPinch { get; set; }
        Action<float> OnPinchRotate { get; set; }

        void ResetInput();
        Vector3 GetCurrentTouchPosition();
        Vector3 GetDeltaTouchPosition();
        bool IsMotionLock();
        void MotionLock();
        void MotionUnlock();
    }

    public enum SwipeDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public abstract class GameInput:MonoBehaviour, IInput
    {
        private static IInput _instance;
        public static IInput Instance
        {
            get
            {
                if (_instance == null)
                {
                    var _obj = new GameObject( "GameInput" );
#if UNITY_MOBILE
                    _instance = _obj.AddComponent<GameInputMobile>();
#else
                    _instance = _obj.AddComponent<GameInputWeb>();
#endif
                    DontDestroyOnLoad( _obj );
                }
                return _instance;
            }
        }

        protected const float thresholdMovement = 50;
        protected const float thresholdSwipe = 300f;

        public Action OnTouchBegan { get; set; }
        public Action OnTouch { get; set; }
        public Action OnTouchEnded { get; set; }
        public Action<Vector2> OnTouchMoved { get; set; }
        public Action<SwipeDirection, float> OnSwipe { get; set; }
        public Action<float, List<Vector2>> OnPinch { get; set; }
        public Action<float> OnPinchRotate { get; set; }

        protected Vector3 _currentTouchPosition;
        protected Vector3 _previousTouchPosition;
        protected Vector3 _startTouchPosition;

        protected float _timeBeginTouch;
        protected float _deltaMagnitudeDiff;
        protected float _deltaAngleDiff;

        protected float _timeIntervalTouch;
        protected List<Vector2> pinchPoints = new List<Vector2>( 2 );
        protected bool isMotionLocked = false;

        public virtual bool IsMotionLock()
        {
            return isMotionLocked;
        }

        public virtual void MotionLock()
        {
            isMotionLocked = true;
        }

        public virtual void MotionUnlock()
        {
            ResetPointer();
            isMotionLocked = false;
        }

        public Vector3 GetCurrentTouchPosition()
        {
            return _currentTouchPosition;
        }

        public Vector3 GetDeltaTouchPosition()
        {
            return _currentTouchPosition - _previousTouchPosition;
        }

        private void Update()
        {
            if (!isMotionLocked)
            {
                bool isUpdate = !( EventSystem.current == null || EventSystem.current.IsPointerOverGameObject() ||
                    UnityEngine.Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject( UnityEngine.Input.GetTouch( 0 ).fingerId ) );
                if (isUpdate)
                {
                    CheckMapAction();
                }

            }
        }

        // Update is called once per frame
        protected virtual void CheckMapAction()
        {
            if (_timeIntervalTouch > 0)
            {
                _timeIntervalTouch -= Time.deltaTime;
                if (_timeIntervalTouch < 0)
                    _timeIntervalTouch = -1;
            }

            //=========================
            if (CheckTouchBegan())
            {
                _previousTouchPosition = _currentTouchPosition;
                _startTouchPosition = _currentTouchPosition = GetInputPosition();

                _timeBeginTouch = Time.time;
                OnTouchBegan?.Invoke();
            }
            if (CheckTouchMoved())
            {
                _previousTouchPosition = _currentTouchPosition;
                _currentTouchPosition = GetInputPosition();

                OnTouchMoved?.Invoke( GetTouchMoveDelta() );
            }

            if (CheckTouchEnded())
            {
                _currentTouchPosition = GetInputPosition();

                OnTouchEnded?.Invoke();
            }

            if (CheckPinch())
            {
                OnPinch?.Invoke( _deltaMagnitudeDiff, pinchPoints );
                pinchPoints.Clear();
            }
            if (CheckPinchRotate())
            {
                OnPinchRotate?.Invoke( _deltaAngleDiff );
            }

            if (CheckSingleTouch())
            {
                _currentTouchPosition = GetInputPosition();

                _timeIntervalTouch = Time.time;

                OnTouch?.Invoke();
            }

            SwipeDirection _dir = CheckSwipe();
            if (_dir != SwipeDirection.None)
            {
                float _delta = Vector3.Distance( _startTouchPosition, _currentTouchPosition );
                OnSwipe?.Invoke( _dir, _delta );
            }
        }

        protected abstract Vector3 GetInputPosition();

        #region Condition 
        protected abstract bool CheckTouchBegan();

        protected abstract bool CheckTouchEnded();

        protected abstract bool CheckTouchMoved();

        protected abstract Vector2 GetTouchMoveDelta();

        protected abstract bool CheckSingleTouch();

        protected abstract bool CheckPinch();

        protected abstract bool CheckPinchRotate();

        protected abstract SwipeDirection CheckSwipe();
        #endregion

        protected void ClearAllActions()
        {
            OnTouchBegan = null;
            OnTouchEnded = null;
            OnTouchMoved = null;
            OnTouch = null;
            OnSwipe = null;
            OnPinch = null;
            OnPinchRotate = null;

        }

        public void ResetInput()
        {
            ClearAllActions();
            ResetPointer();
            isMotionLocked = false;
        }

        protected void ResetPointer()
        {
            _startTouchPosition = _previousTouchPosition = _currentTouchPosition = GetInputPosition();
            _timeBeginTouch = Time.time;
        }
    }
}
