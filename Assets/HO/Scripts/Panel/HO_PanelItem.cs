using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_PanelItem:HO_EventUserBehaviour, IHOPanelItem
    {
        protected IHOModuleConnector core;

        public IHOManager Manager
        {
            get
            {
                if (core == null)
                    return null;
                return core.Manager;
            }
        }

        public virtual void Init(IHOModuleConnector core)
        {
            Log( "Init" );
            this.core = core;
        }
    }
}
