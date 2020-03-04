using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HOSystem
{

    public class HO_GameManager:HO_EventUserBehaviour, IHOManager
    {
        private const string HOSCENEPATH = "HO/HOScene";

        private static string Location = "BAKER_STREET";

        public static void PlayLocation(string location)
        {
            //Location = location;
            SceneManager.LoadSceneAsync( HOSCENEPATH );
        }

        public bool IsPlay
        {
            get; private set;
        }

        #region MODULE
        private Dictionary<Type, IHOModule> ActiveModules = new Dictionary<Type, IHOModule>();

        private T ModuleCreate<T>() where T :class, IHOModule, new()
        {
            if (ActiveModules.ContainsKey( typeof( T ) ))
            {
                Error( string.Format( "Dublicate <{0}> module", typeof( T ).ToString() ) );
                return null;
            }

            var _module = ( T )Activator.CreateInstance<T>();
            _module.Init( this );
            ActiveModules.Add( typeof( T ), _module );
            return _module;

        }

        public void ModuleRegister<T>(IHOModule module) where T : class, IHOModule
        {
            if (ActiveModules.ContainsKey( typeof( T ) ))
            {
                Error( string.Format( "Dublicate <{0}> module", typeof( T ).ToString() ) );
                return;
            }

            ActiveModules.Add( typeof( T ), module );
        }

        private void ModuleRemove(Type type)
        {
            if (ActiveModules.ContainsKey( type ))
            {
                ActiveModules.Remove( type );
                Log( string.Format( "remove <{0}> module",type.ToString() ) );
            }
            else
            {
                Log( string.Format( "no instance <{0}> module", type.ToString() ) );
            }
        }

        private void ModuleRemove(IHOModule module)
        {
            Type _moduleKey = null;
            foreach(var mod in ActiveModules)
            {
                if(mod.Value == module)
                {
                    _moduleKey = mod.Key;
                    break;
                }
            }

            if(_moduleKey!=null)
            {
                ModuleRemove( _moduleKey );
            }
            else
            {
                Log( string.Format( "no instance <{0}> module", module.ToString() ) );
            }

            module = null;
        }
        #endregion

        #region Data
        private IHOData data;

        public IHOData LevelData
        {
            get
            {
                if (data == null)
                    data = LoadData();
                return data;
            }
        }

        private IHOData LoadData()
        {
            return HO_LevelsManager.Instance.GetLevel( Location );
        }

        public IHOPlayerData PlayerData { get; private set; }

        public IHOInventory InvectoryData { get; private set; }

        public IHOLoadData LoadedItems { get; private set; }
        #endregion

        #region LoadLevel

        private IHOResourceLoader loader;
        private IHOBuilder builder;
        private IHOCamera gameCamera;
        private IHOModule gamePanel;
        private IHOModule hiddenObjectsManager;
        private IHOModule clickableManager;
        private IHOModule hintsManager;


        void Start()
        {
            IsPlay = false;
            PlayerData = new HO_PlayerDataConnector();
            InvectoryData = new HO_InventoryConnector();
            BuildLevel( InitGameControllers );
        }

        private void GetLoadedItems(Action<IHOLoadData> loadedCallback)
        {
            loader = ModuleCreate<HO_Loader>();
            loader.Load( loadedCallback );
        }

        private void BuildLevel(Action endCallback)
        {
            builder = ModuleCreate<HO_Builder>();
            GetLoadedItems( (a) => { LoadedItems = a; endCallback?.Invoke(); builder.Build( );  } );
        }

        private void InitGameControllers()
        {
            gameCamera = ModuleCreate<HO_Camera>();
            PanelInit();
            hintsManager = ModuleCreate<HO_HintsManager>();
            hiddenObjectsManager = ModuleCreate<HO_HiddenObjectsManager>();
            clickableManager = ModuleCreate<HO_ClickableManager>();
            

            Play();
        }

        private void PanelInit()
        {
            var _gamePanelGO = Instantiate( LoadedItems.GetItem("HUD") );
            gamePanel = _gamePanelGO.GetComponent<IHOModule>();
            gamePanel.Init( this );
        }


        #endregion

        #region Game



        public void Play()
        {
            SetPlay( true );
        }

        public void Pause()
        {
            SetPlay( false );
        }

        private void SetPlay(bool isPlay)
        {
            IsPlay = isPlay;
            HO_Button.isInteractive = IsPlay;
        }

        public void End()
        {
            clickableManager.Remove();
        }

        #endregion

        #region Update
        public Action OnUpdate { get; set; }

        private List<IHOModule> UpdateModules = new List<IHOModule>();

        public void AddToUpdate(IHOModule module)
        {
            if (!UpdateModules.Contains( module ))
            {
                UpdateModules.Add( module );
            }
        }

        public void RemoveFromUpdate(IHOModule module)
        {
            if (UpdateModules.Contains( module ))
            {
                UpdateModules.Remove( module );
            }
        }

        private void Update()
        {
            if (!IsPlay)
                return;

            foreach (var module in UpdateModules)
            {
                if (module != null)
                {
                    module.Next();
                }
            }

            try
                {
                OnUpdate?.Invoke();
            }
            catch
            {
                Error( "UPDATE ACTIONS" );
            }
            
        }
        #endregion

        #region Events

        private List<IHOEventUser> listeners = new List<IHOEventUser>();

        private void CheckRemoveMessage(HOMessage mess)
        {
            if (!mess.hash.ContainsKey( "Module" ))
                return;

            var _module = mess.hash[ "Module" ] as IHOModule;

            RemoveListener( _module );
            RemoveFromUpdate( _module );
            ModuleRemove( _module );
        }

        public override void Send(HOMessage mess)
        {
#if UNITY_EDITOR
            mess.AddTracePoint( this );
#endif
            switch (mess.key)
            {
                case ( int )HOMessageType.Remove:
                {
                    CheckRemoveMessage( mess );
                }
                break;
                case ( int )HOMessageType.Pause:
                {
                    SetPlay( !IsPlay );
                    return;
                }
            }

            foreach (var listener in listeners)
            {
                if (listener == null)
                    continue;

                listener.Send( mess );
            }
        }

        public void AddListener(IHOEventUser listener)
        {
            if (!listeners.Contains( listener ))
                listeners.Add( listener );
        }

        public void RemoveListener(IHOEventUser listener)
        {
            if (listeners.Contains( listener ))
                listeners.Remove( listener );
        }

        #endregion;
    }
}
