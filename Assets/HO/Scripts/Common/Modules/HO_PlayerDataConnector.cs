using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HOSystem
{
    public class HO_PlayerDataConnector:IHOPlayerData
    {
        public Action OnUpdate { get; set; }

        public bool HasState(string key)
        {
            //bool _exist = PlayerData.Instance.HasBool( key ) || PlayerData.Instance.HasString( key ) ||
            //     PlayerData.Instance.HasInt( key ) || PlayerData.Instance.HasFloat( key ) || PlayerData.Instance.HasData( key );
            return true;// _exist;
        }

        public void RemoveState(string key)
        {
            if (!HasState( key ))
                return;
            /*
            if (PlayerData.Instance.HasBool( key ))
            {
                PlayerData.Instance.RemoveBool( key );
                return;
            }

            if (PlayerData.Instance.HasInt( key ))
            {
                PlayerData.Instance.RemoveInt( key );
                return;
            }

            if (PlayerData.Instance.HasFloat( key ))
            {
                PlayerData.Instance.RemoveFloat( key );
                return;
            }

            if (PlayerData.Instance.HasString( key ))
            {
                PlayerData.Instance.RemoveString( key );
                return;
            }

            if (PlayerData.Instance.HasData( key ))
            {
                PlayerData.Instance.RemoveDateTime( key );
                return;
            }
            */
        }

        public bool GetState(string key, bool defaultValue)
        {
            return defaultValue;// PlayerData.Instance.GetBool( key, defaultValue );
        }

        public string GetState(string key, string defaultValue)
        {
            return defaultValue;// PlayerData.Instance.GetString( key, defaultValue );
        }

        public int GetState(string key, int defaultValue)
        {
            return defaultValue;// PlayerData.Instance.GetInt( key, defaultValue );
        }

        public float GetState(string key, float defaultValue)
        {
            return defaultValue;// PlayerData.Instance.GetFloat( key, defaultValue );
        }

        public DateTime GetState(string key, DateTime defaultValue)
        {
            //if (!PlayerData.Instance.HasData( key ))
            //   return defaultValue;

            return defaultValue;// PlayerData.Instance.GetDateTime( key);
        }

        public void SetState(string key, bool value)
        {
            // PlayerData.Instance.SetBool( key, value );
            OnUpdate?.Invoke();
        }

        public void SetState(string key, string value)
        {
            // PlayerData.Instance.SetString( key, value );
            OnUpdate?.Invoke();
        }

        public void SetState(string key, int value)
        {
            // PlayerData.Instance.SetInt( key, value );
            OnUpdate?.Invoke();
        }

        public void SetState(string key, float value)
        {
            // PlayerData.Instance.SetFloat( key, value );
            OnUpdate?.Invoke();
        }

        public void SetState(string key, DateTime value)
        {
            //  PlayerData.Instance.SetDateTime( key, value );
            OnUpdate?.Invoke();
        }
    }
}
