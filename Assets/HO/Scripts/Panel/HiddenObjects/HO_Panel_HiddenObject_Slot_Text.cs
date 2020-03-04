using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public class HO_Panel_HiddenObject_Slot_Text:HO_Panel_Items_Slot
    {
        protected TextMeshProUGUI Label;

        public override void Init(IHOPanelItem core)
        {
            base.Init( core );
            Label = CreateItem<TextMeshProUGUI>("Label");
            SetTextSetting();
        }

        private void SetTextSetting()
        {
            Label.alignment = TextAlignmentOptions.Midline;
           // Label.SetPreset( "Dark" );
            Label.fontSize = 52;
            Label.fontStyle = FontStyles.Bold;
            Label.color = Color.black;
        }

        protected override bool IsEmpty()
        {
            return string.IsNullOrEmpty( Label.text );
        }

        protected override void ApproveItem()
        {
            Label.text =string.IsNullOrEmpty(ItemKey)?string.Empty:GetLocalizedItemName( ItemKey );
        }

        private string GetLocalizedItemName(string key)
        {
            return key;// Localisation.GetString( "HO_" + Manager.LevelData.Location + "_" + key.ToUpper() );
        }
    }
}
