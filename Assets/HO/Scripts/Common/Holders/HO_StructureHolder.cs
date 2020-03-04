using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HOSystem
{
    [Serializable]
    public struct HOItem
    {
        public string Key;
        public int count;
    }

    public struct HOFormula
    {
        public int A, B, C, Count;

        string[] abcArr;
        int count;
        string tempVal;
        char symbol;

        public HOFormula(string formula)
        {
            A = 0;
            B = 0;
            C = 0;
            Count = 0;

            abcArr = formula.Split( '+' );
            count = 0;
            tempVal = "";
            symbol = ' ';

            foreach (var val in abcArr)
            {
                tempVal = val;

                symbol = val[ val.Length - 1 ];
                tempVal = tempVal.Remove( val.Length - 1 );
                count = int.Parse( tempVal );

                A = ( symbol == 'A' ) ? count : A;
                B = ( symbol == 'B' ) ? count : B;
                C = ( symbol == 'C' ) ? count : C;
                Count += count;
            }

        }


    }

    public struct HOMessage
    {
        public int key;
        public Hashtable hash;

#if UNITY_EDITOR
        private string trace;
#endif

        public HOMessage(int key)
        {
            this.key = key;
            hash = new Hashtable();
#if UNITY_EDITOR
            trace = "[]";
#endif
        }

        public HOMessage(HOMessageType type)
        {
            this = new HOMessage( ( int )type );
        }

        public HOMessage Clone()
        {
            var _mess = new HOMessage( key );
            _mess.hash = hash.Clone() as Hashtable;
            return _mess;
        }


        public void AddTracePoint(object obj)
        {
#if UNITY_EDITOR
            trace += string.Format( " >[{0}]", obj.GetType().ToString() );
#endif
        }

    }

    public struct CameraInfo
    {
        public Vector2 Border;

        public float Offset;
        public float OrthoSizeMax;
        public float OrthoSizeMin;
    }

    public struct HOBuildItem
    {
        public GameObject Image;
        public GameObject Silhouette;
        public GameObject Shadow;
        public GameObject Over;
        public GameObject Collider;
        public GameObject Glow;
    }

    
}
