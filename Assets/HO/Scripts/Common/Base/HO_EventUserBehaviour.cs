using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_EventUserBehaviour:MonoBehaviour, IHOEventUser, IHOBehaviourConnector
    {

        public GameObject GameObject
        {
            get { return gameObject; }
        }

        public virtual void Send(HOMessage mess)
        {
            
        }

        public void Send(HOMessageType type)
        {
            var _mess = CreateMessage( type );
            Send( _mess );
        }

        protected virtual void Log(string message)
        {
            Debug.Log( string.Format( "[{1}] {0}", message, GetType().ToString() ) );
        }

        protected virtual void Error(string message)
        {
            Debug.LogError( string.Format( "[{1}] {0}", message, GetType().ToString() ) );
        }

        protected HOMessage CreateMessage(HOMessageType type)
        {
            return CreateMessage( ( int )type );
        }

        protected HOMessage CreateMessage(int type)
        {
            var _mess = new HOMessage( type );
#if UNITY_EDITOR
            _mess.AddTracePoint( this );
#endif
            return _mess;
        }
    }
}