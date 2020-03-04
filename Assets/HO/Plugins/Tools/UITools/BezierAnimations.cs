//#define Debug
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierAnimations:MonoBehaviour
{

    public float speed = 200f;
    public bool IsAutoUpdate = true;
    public int subdivs = 1;

    Vector3 scale = new Vector3( 1f, 1f, 1f );

    private float[] _speedsByChordsLengths;
    private float _totalLength;

    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;

    private float _t;

#if Debug
	private Vector3 _prevPos;
	private List<Vector3> _testPositions = new List<Vector3>(); 
#endif

    //float timer = 2;
    //bool firstTimer = default;

    public Action<BezierAnimations> OnFinish, OnPreFinish;
    public bool IsFinished { get; private set; }


    public void Init(Transform target, float speed)
    {
        Init( target, speed, 2.7f, 0, false );
    }

    public void Init(Transform target, float speed = 200f, float maxForce = 2.7f, float minForce = 0, bool isRelativeSpeed = true)
    {
        IsFinished = false;

        

        _t = 0;

        _p0 = transform.position;
        
        float _force = UnityEngine.Random.Range( -maxForce, maxForce );
        _force = ( _force >= 0 ) ? Mathf.Clamp( _force, minForce, maxForce ) : Mathf.Clamp( _force, -maxForce, -minForce );

        _p1 = new Vector3( _p0.x * 0.5f, _p0.y + _force, 0 );

        _p2 = target.position;

        if (isRelativeSpeed)
        {
            float _distance = Vector3.Distance( _p0, _p2 );
            this.speed = speed / _distance;
        }
        else
        {
            this.speed = speed;
        }

        PrepareCoords();


    }

    public void Init(Vector3 p1, Vector3 p2, float speed = 200f)
    {
        IsFinished = false;
        this.speed = speed;
        _t = 0;
        _p0 = transform.position;

        _p1 = p1;

        _p2 = p2;

        PrepareCoords();
    }

    public void UpdateFinishPosition(Vector3 p2)
    {
        _p2 = p2;
        PrepareCoords();
    }

    void Update()
    {
        if (IsAutoUpdate)
            Next();
    }

    public void Next()
    {
        _t += Time.deltaTime * ( speed ) / _totalLength / GetSpeedByCoordLength( _t );
        _t = Mathf.Clamp01( _t );

        Vector3 b = GetQuadBezierPoint( _p0, _p1, _p2, _t );

        transform.position = b;

        float distance = Vector3.Distance( transform.position, _p2 );



        if (distance < 0.003f)
        {
            IsFinished = true;
            OnFinish?.Invoke( this );
            enabled = false;
        }
        else if (distance < 2)
        {
            if (OnPreFinish != null)
            {
                OnPreFinish( this );
                OnPreFinish = null;
            }
        }

#if Debug
		DrawPoints();
#endif
    }

    Vector3 GetQuadBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float invT = 1 - t;
        return ( invT * invT * p0 ) + ( 2 * invT * t * p1 ) + ( t * t * p2 );
    }

    private void PrepareCoords()
    {
        _speedsByChordsLengths = new float[ subdivs ];

        Vector3 prevPos = _p0;
        for (int i = 0; i < subdivs; i++)
        {
            Vector3 curPos = GetQuadBezierPoint( _p0, _p1, _p2, ( i + 1 ) / ( float )subdivs );
            float length = Vector3.Magnitude( curPos - prevPos );

#if Debug
			Debug.DrawLine(curPos, prevPos, Color.yellow, 30f);
#endif

            _speedsByChordsLengths[ i ] = length;
            _totalLength += length;
            prevPos = curPos;
        }

        for (int i = 0; i < subdivs; i++)
        {
            _speedsByChordsLengths[ i ] = _speedsByChordsLengths[ i ] / _totalLength * subdivs;
        }
    }


    float GetSpeedByCoordLength(float t)
    {
        int pos = ( int )( t * subdivs ) - 1;
        pos = Mathf.Clamp( pos, 0, subdivs - 1 );
        return _speedsByChordsLengths[ pos ];
    }

#if Debug
	void DrawPoint(Vector3 p)
	{
		Debug.DrawRay (p + Vector3.down * 0.5f * 0.1f, Vector3.up * 0.1f);
		Debug.DrawRay (p + -Vector3.forward * 0.5f * 0.1f, Vector3.forward * 0.1f);
	}

	void DrawPoints()
	{
		_testPositions.Add (transform.position);

		foreach (Vector3 t in _testPositions) {
			DrawPoint (t);
		}
	}
#endif
}
