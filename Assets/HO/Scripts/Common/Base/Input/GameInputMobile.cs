using System.Collections;
using System.Collections.Generic;
using HO.Scripts.Common.Base.Input;
using UnityEngine;

namespace BST
{
    public class GameInputMobile:GameInput
    {
        protected override Vector3 GetInputPosition()
        {
            if (Input.touchCount == 0)
                return Input.mousePosition;
            return Input.GetTouch( 0 ).position;
        }
        protected override bool CheckTouchBegan()
        {
            return Input.touchCount == 1 && Input.GetTouch( 0 ).phase == TouchPhase.Began;
        }
        protected override bool CheckTouchEnded()
        {
            return ( Input.touchCount == 1 && Input.GetTouch( 0 ).phase == TouchPhase.Ended );
        }
        protected override bool CheckTouchMoved()
        {
            return Input.touchCount == 1 && Input.GetTouch( 0 ).phase == TouchPhase.Moved;
        }
        protected override Vector2 GetTouchMoveDelta()
        {
            return Input.GetTouch( 0 ).deltaPosition;
        }
        protected override bool CheckSingleTouch()
        {
            return ( Input.touchCount == 1 && Input.GetTouch( 0 ).phase == TouchPhase.Ended )
            && ( Vector3.Distance( GetInputPosition(), _startTouchPosition ) < thresholdMovement
            && ( Time.time - _timeBeginTouch ) < 1f );
        }
        protected override bool CheckPinchRotate()
        {
            // If there are two touches on the device...
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch( 0 );
                Touch touchOne = Input.GetTouch( 1 );

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

            Touch[] myTouches = Input.touches;
            for (int i = 0; i < Input.touchCount; i++)
            {
            }
            if (Input.touchCount == 1 && Input.GetTouch( 0 ).phase == TouchPhase.Ended)
            {
                float vert = Input.GetTouch( 0 ).position.y - _startTouchPosition.y;
                float honz = Input.GetTouch( 0 ).position.x - _startTouchPosition.x;
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
        protected override bool CheckPinch()
        {
            // If there are two touches on the device...
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch( 0 );
                Touch touchOne = Input.GetTouch( 1 );

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
            return false;
        }
    }
}
