using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public class HO_Panel_HiddenObject_Slot_Silhouette:HO_Panel_Items_Slot
    {
        protected Image Silhouette;
        private int LenghtForDisappearEffect = 0;
            


        public override void Init(IHOPanelItem core)
        {
            base.Init( core );
            Silhouette = CreateItem<Image>( "Silhouette" );
        }

        protected override void ApproveItem()
        {

        }

        protected override bool IsEmpty()
        {
            return Silhouette.sprite == null;
        }

        private int GetLenghtForDisappearEffect()
        {
            if (LenghtForDisappearEffect <= 0)
            {
                var _rect = GetComponent<RectTransform>();
                LenghtForDisappearEffect = ( int )( _rect.sizeDelta.x / 30 );
            }
            return LenghtForDisappearEffect;
        }

        private void ResizeDisappearEffect()
        {
            var main = EffectDisappear.main;
            var emission = EffectDisappear.emission;
            var shape = EffectDisappear.shape;

            var c = new ParticleSystem.MinMaxCurve( emission.rateOverTime.constant * GetLenghtForDisappearEffect() );
            emission.rateOverTime = c;

            shape.radius *= GetLenghtForDisappearEffect();

            var rs = new ParticleSystem.MinMaxCurve( shape.radiusSpeed.constant * GetLenghtForDisappearEffect() );
            shape.radiusSpeed = rs;


        }
    }
}
