using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_ItemSpotFactory:MonoBehaviour
    {
        public static IHOPanelItemSlot GetSlot(HOMode mode, GameObject go)
        {
            switch (mode)
            {
                default:
                return go.AddComponent<HO_Panel_HiddenObject_Slot_Text>();
                case HOMode.Anagram:
                return go.AddComponent<HO_Panel_HiddenObject_Slot_Anagram>();
                case HOMode.Silhouette:
                return go.AddComponent<HO_Panel_HiddenObject_Slot_Silhouette>();
            }
        }
    }
}