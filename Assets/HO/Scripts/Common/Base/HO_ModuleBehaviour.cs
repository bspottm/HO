using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public abstract class HO_ModuleBehaviour:HO_EventUserBehaviour, IHOModule
    {
        public IHOManager Manager { get; private set; }

        public virtual void Init(IHOManager Manager)
        {
            this.Manager = Manager;
            RegisterModule();
            Manager.AddListener( this );
        }
        /// <summary>
        /// need Manager.ModuleRegister<thisType>(this)
        /// </summary>
        protected abstract void RegisterModule();

        public virtual void Next()
        {
            
        }

        public virtual void Remove()
        {
            SendRemoveMessage();
            Manager = null;
        }

        private void SendRemoveMessage()
        {
            HOMessage removeMessage = CreateMessage( HOMessageType.Remove );
            removeMessage.hash.Add( "Module", this );
            Manager.Send( removeMessage );
        }
    }
}
