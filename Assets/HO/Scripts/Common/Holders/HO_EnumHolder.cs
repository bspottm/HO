using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public enum HOMode
    {
        Text,
        Anagram,
        Silhouette
    }

    public enum HOMessageType
    {
        /// <summary>
        /// [To HO_GameManager]
        /// Module - (IHOModule) ссылка.
        /// удаление модуля.
        /// </summary>
        Remove,
        /// <summary>
        /// [To HO_Panel -> HO_Panel_HiddenObjects]
        /// (String array with item keys)
        /// </summary>
        SetHiddenObjectList,
        SLotUpdate,
        Click,
        Find,
        FailClick,
        Pause,
        RemoveSeparator,
        HintButtonSetLock,
        HintSetAmount,
        HintUse,
        HintFinish,
        /// <summary>
        /// Hint(HOHintType).
        /// Type:
        /// Get - [To HO_Panel -> HO_Panel_HiddenObjects] запрос списка объектов в панели.
        /// Return - [To HO_Panel] возврат списка.
        /// Set - [To HO_HintsManager] конечный возврат списка.
        /// Список : key [(string)item_{0..n}]: value [(string)item_key].
        /// </summary>
        QueryPanelHiddenObject,
        /** <summary>
        * Hint(HOHintType)
        * Type(string):
        * Get -  [To HO_HiddenObjectManager] Запрос ссылок на объекты.
        * Список : key [(string)Get_item_{0..n}]: value [(string)item_key]
        * Return - [To HO_HintsManager] возврат ссылок.
        * Список : key [(string)Return_item_{0..n}]: value [(IHOHiddenObject)item]
        * </summary> */
        QueryHiddenObject,
        /// <summary>
        /// [To HO_HiddenObjectManager]
        /// </summary>
        HiddenObjectRegistration,
        /// <summary>
        /// [To HO_HiddenObjectManager]
        /// Item - hiddenObject key.
        /// </summary>
        HiddenObjectRemove,

        LevelProgressBarUpdate,

    }

    public enum HOClickCommand
    {
        None,
        Pause,
        Hint,
    }

    public enum HOHintType
    {
        None,
        Eye,
        Bomb,
        Compass
    }
}
