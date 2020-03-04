using System;
using UnityEngine;

namespace HO.Scripts.Common.Base.Input
{
    public class GameInputWeb:GameInput
    {
        protected override Vector3 GetInputPosition()
        {
            return UnityEngine.Input.mousePosition;
        }
        protected override bool CheckTouchBegan()
        {
            return UnityEngine.Input.GetMouseButtonDown( 0 );
        }
        protected override bool CheckTouchEnded()
        {
            return UnityEngine.Input.GetMouseButtonUp( 0 );
        }
        protected override bool CheckTouchMoved()
        {
            return UnityEngine.Input.GetMouseButton( 0 )
                && Vector3.Distance( GetInputPosition(), _startTouchPosition ) > thresholdMovement;
        }
        protected override Vector2 GetTouchMoveDelta()
        {
            return _currentTouchPosition - _previousTouchPosition;
        }
        protected override bool CheckSingleTouch()
        {
            return UnityEngine.Input.GetMouseButtonUp( 0 )
                && ( Vector3.Distance( GetInputPosition(), _startTouchPosition ) < thresholdMovement
                        && ( Time.time - _timeBeginTouch ) < 1.5f );
        }
        protected override bool CheckPinch()
        {
            if (UnityEngine.Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = UnityEngine.Input.GetTouch( 0 );
                Touch touchOne = UnityEngine.Input.GetTouch( 1 );

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                pinchPoints.Clear();

                pinchPoints.Add( touchZero.position );
                pinchPoints.Add( touchOne.position );

                float prevTouchDeltaMag = ( touchZeroPrevPos - touchOnePrevPos ).magnitude;
                float touchDeltaMag = ( touchZero.position - touchOne.position ).magnitude;

                // Find the difference in the distances between each frame.
                _deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                return true;
            }

            _deltaMagnitudeDiff = UnityEngine.Input.GetAxis( "Mouse ScrollWheel" );

            pinchPoints.Clear();
            pinchPoints.Add( UnityEngine.Input.mousePosition );
            pinchPoints.Add( UnityEngine.Input.mousePosition );

            return Math.Abs( UnityEngine.Input.GetAxis( "Mouse ScrollWheel" ) ) > 0.01f;
        }
        protected override bool CheckPinchRotate()
        {
            // If there are two touches on the device...
            if (UnityEngine.Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = UnityEngine.Input.GetTouch( 0 );
                Touch touchOne = UnityEngine.Input.GetTouch( 1 );

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find angle 
                float anglePrev = Vector2.Angle( touchZeroPrevPos, touchOnePrevPos );
                float angle = Vector2.Angle( touchZero.position, touchOne.position );
                _deltaAngleDiff = angle - anglePrev;

                return Mathf.Abs( _deltaAngleDiff ) > 0.5f;

            }
            return false;
        }
        protected override SwipeDirection CheckSwipe()
        {
            if (( Time.time - _timeBeginTouch ) > 0.2f)
                return 0;

            if (UnityEngine.Input.GetMouseButtonUp( 0 ))
            {

                float vert = UnityEngine.Input.mousePosition.y - _startTouchPosition.y;
                float honz = UnityEngine.Input.mousePosition.x - _startTouchPosition.x;

                if (Mathf.Abs( vert ) > thresholdSwipe || Mathf.Abs( honz ) > thresholdSwipe)
                {
                    if (Mathf.Abs( vert ) > Mathf.Abs( honz ))
                    {
                        return ( vert > 0 ) ? SwipeDirection.Up : SwipeDirection.Down;
                    }
                    else
                    {
                        return ( honz > 0 ) ? SwipeDirection.Right : SwipeDirection.Left;
                    }
                }
            }

            return 0;
        }
    }
}
