using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public interface IHOBehaviourConnector
    {
        GameObject GameObject { get; }
    }

    public interface IHOManager: IHOEventManager
    {
        IHOData LevelData { get; }
        IHOPlayerData PlayerData { get; }
        IHOInventory InvectoryData { get; }
        IHOLoadData LoadedItems { get; }
        bool IsPlay{ get; }

        void Play();
        void Pause();
        void End();

        Action OnUpdate { get; set; }
        void AddToUpdate(IHOModule module);
        void RemoveFromUpdate(IHOModule module);

        void ModuleRegister<T>(IHOModule module) where T : class, IHOModule;
    }

    public interface IHOData:IHOLocationFormula
    {
        string Location { get; }
        string Rank { get; }
        float Offset { get; }
        HOMode Mode { get; }
        List<HOItem> Reward { get; }
        Dictionary<string, string> Settings { get; }

    }

    public interface  IHOPlayerData
    {
        Action OnUpdate { get; set; }

        bool HasState(string key);
        void RemoveState(string key);

        bool GetState(string key, bool defaultValue);
        void SetState(string key, bool value);

        string GetState(string key, string defaultValue);
        void SetState(string key, string value);

        int GetState(string key, int defaultValue);
        void SetState(string key, int value);

        float GetState(string key, float defaultValue);
        void SetState(string key, float value);

        DateTime GetState(string key, DateTime defaultValue);
        void SetState(string key, DateTime value);
    }

    public interface IHOInventory
    {
        Action OnUpdate { get; set; }

        void AddItem(string key, int count = 1, string source = "");
        void ApplyItem(string key, int count = 1, string source = "");
        int GetItemCount(string key);
        bool HasItem(string key);
    }

    public interface IHOLoadData
    {
        void AddItem(string key, GameObject item);
        GameObject GetItem(string key);
        bool HasItem(string key);
    }

    public interface IHOLocationFormula
    {
        string[] ActiveItemFormula { get; }
        string[] InactiveItemFormula { get; }
        string[] StaticItemFormula { get; }  
    }

    public interface IHOEventUser
    {
        void Send(HOMessage mess);
        void Send(HOMessageType type);
    }

    public interface IHOEventManager:IHOEventUser
    {
        void AddListener(IHOEventUser listener);
        void RemoveListener(IHOEventUser listener);
    }

    public interface IHOModuleConnector: IHOEventUser
    {
        IHOManager Manager { get; }
    }

    public interface IHOModule:IHOModuleConnector
    {
        void Init(IHOManager Manager);
        void Remove();
        void Next();
    }

    public interface IHOResourceLoader: IHOModule
    {
        void Load(Action<IHOLoadData> loadedCallback );
    }


    public interface IHOBuilder:IHOModule
    {
        void Build();
    }
    

    public interface IHOCamera:IHOModule
    {
        Camera GetCamera { get; }
        float GetOrthoSize { get; }

        void SetOffsetPosition(float y);

        void Move(Vector2 delta);
        void ScrollAndZoom(float delta, List<Vector2> points);
        
        void ZoomFromPoint(Vector3 zoomPoint, float delta);
        void MoveToTarget(Transform target);
    }

    #region HO_PANEL

    public interface IHOPanelItem: IHOModuleConnector
    {
        void Init(IHOModuleConnector core);
    }

    public interface IHOPanelItemSlot: IHOModuleConnector, IHOBehaviourConnector
    {
        string ItemKey { get;}
        bool IsLock { get; }
        void Init(IHOPanelItem core);
        void SetKey(string key);
    }
    #endregion

    public interface IHOHiddenObject: IHOModuleConnector, IHOBehaviourConnector
    {
        string Key { get; set; }
        GameObject Image { get; set; }
        void Init(IHOManager manager);
        Action OnFind { get; set; }
        bool IsFind { get; }
    }

    public interface IHOClickableObject:IHOEventUser
    {
        void Init(IHOEventUser connect);
    }

    public interface IHOHint: IHOModuleConnector, IHOBehaviourConnector
    {

        void Init(IHOManager manager, List<IHOHiddenObject> items);
    }
}
