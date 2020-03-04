using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    /// <summary>
    /// TODO ЗАГЛУШКА!
    /// Нужно переделать под бандлы
    /// </summary>
    public class HO_Loader:HO_Module, IHOResourceLoader
    {
        #region CONSTANTS
        public const string PATHTOLOCATIONSFOLDER = "Levels/";
        public const string PARTPATHTOLOCATIONSETTING = "/Setting/";
        public const string PARTPATHTOLOCATIONANIMATION = "/Animation/";
        public const string PATHTOHUDFOLDER = "HUD/";
        public const string PATHTOHUD = "HO_HUD";
        #endregion

        public void Load(Action<IHOLoadData> loadedCallback)
        {
            var loadItem = new HO_LoadData();

            LoadLocation( loadItem );
            LoadHUD( loadItem );

            loadedCallback?.Invoke( loadItem );
            Remove();
        }

        private void LoadLocation(IHOLoadData loadItem)
        {
            loadItem.AddItem( "Location", LoadItem( GetPathLocation ) );
            loadItem.AddItem( "Setting", LoadItem( GetPathSetting ) );
            loadItem.AddItem( "Animation", LoadItem( GetPathAnimation ) );
        }

        private void LoadHUD(IHOLoadData loadItem)
        {
            loadItem.AddItem( "HUD", LoadItem( GetPathHud ) );
            loadItem.AddItem( "Slot", LoadItem( GetPathHudSlot ) );

            LoadHudEffects( loadItem );
            LoadHudHints( loadItem );
        }

        private void LoadHudEffects(IHOLoadData loadItem)
        {
            loadItem.AddItem( "HUD_SlotEffectAppear", LoadItem( GetPathHudSlotEffectAppear ) );
            loadItem.AddItem( "HUD_SlotEffectDisappear", LoadItem( GetPathHudSlotEffectDisappear ) );
            loadItem.AddItem( "HUD_EffectMissClick", LoadItem( GetPathHudMissClickEffect ) );
            loadItem.AddItem( "HUD_SlotEffectHint", LoadItem( GetPathHudSlotEffectHint ) );
            loadItem.AddItem( "HUD_HintEffectEye", LoadItem( GetPathHudHintEffectEye ) );
            loadItem.AddItem( "HUD_HintEffectBomb", LoadItem( GetPathHudHintEffectBomb ) );
            loadItem.AddItem( "HUD_ItemHintEffect", LoadItem( GetPathHudItemHintEffect ) );
        }

        private void LoadHudHints(IHOLoadData loadItem)
        {
            loadItem.AddItem( "HUD_HintEye", LoadItem( GetPathHudHintEye ) );
            loadItem.AddItem( "HUD_HintBomb", LoadItem( GetPathHudHintBomb ) );
            loadItem.AddItem( "HUD_HintCompas", LoadItem( GetPathHudHintCompas ) );
        }

        private GameObject LoadItem(string Path)
        {
            return Resources.Load<GameObject>( Path );
        }

        #region Location Pathes
        private string GetNameLocation
        {
            get { return Manager.LevelData.Location.ToUpper(); }
        }

        private string GetPathLocation
        {
            get
            {

                return string.Format( "{1}{0}/{0}", GetNameLocation, PATHTOLOCATIONSFOLDER );
            }
        }

        private string GetPathSetting
        {
            get
            {
                return string.Format( "{1}{0}{2}{0}", GetNameLocation, PATHTOLOCATIONSFOLDER, PARTPATHTOLOCATIONSETTING );
            }
        }

        private string GetPathAnimation
        {
            get
            {
                return string.Format( "{1}{0}{2}{0}", GetNameLocation, PATHTOLOCATIONSFOLDER, PARTPATHTOLOCATIONANIMATION );
            }
        }

        #endregion

        #region HUD Pathes
        private string GetPathHud
        {
            get
            {

                string _hudSizeType = "_Base";

                /*
                _hudSizeType = "_16_9";

                if (Camera.main.aspect < 1.44f)
                {
                    _hudSizeType = "__4_3";
                }
                */
                return PATHTOHUDFOLDER + PATHTOHUD + _hudSizeType;
            }
        }

        private string GetPathHudSlot
        {
            get { return PATHTOHUDFOLDER + "HO_Item_Slot"; }
        }
        #endregion

        #region HUD_Effects

        private string GetPathHudSlotEffects
        {
            get
            {
                return PATHTOHUDFOLDER + "Effects/";
            }
        }

        private string GetPathHudSlotEffectAppear
        {
            get { return GetPathHudSlotEffects + "HO_SlotEffectAppear"; }
        }
        private string GetPathHudSlotEffectDisappear
        {
            get { return GetPathHudSlotEffects + "HO_SlotEffectDisappear"; }
        }
        private string GetPathHudMissClickEffect
        {
            get { return GetPathHudSlotEffects + "HO_EffectMissClick"; }
        }
        private string GetPathHudSlotEffectHint
        {
            get { return GetPathHudSlotEffects + "HO_SlotEffectHint"; }
        }
        private string GetPathHudHintEffectEye
        {
            get { return GetPathHudSlotEffects + "HO_HintEffectEye"; }
        }
        private string GetPathHudHintEffectBomb
        {
            get { return GetPathHudSlotEffects + "HO_HintEffectBomb"; }
        }
        private string GetPathHudItemHintEffect
        {
            get { return GetPathHudSlotEffects + "HO_ItemHintEffect"; }
        }
        #endregion

        #region HUD_Hints
        private string GetPathHudHints
        {
            get { return PATHTOHUDFOLDER + "Hints/"; }
        }

        private string GetPathHudHintEye
        {
            get { return GetPathHudHints + "HO_HintEyeButton"; }
        }

        private string GetPathHudHintBomb
        {
            get { return GetPathHudHints + "HO_HintBombButton"; }
        }

        private string GetPathHudHintCompas
        {
            get { return GetPathHudHints + "HO_HintCompasButton"; }
        }
        #endregion
    }
}


