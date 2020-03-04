using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public class HO_UiProgressBar:MonoBehaviour
    {
        [SerializeField]
        private IHOManager core;
        [SerializeField]
        private Image image;
        [SerializeField]
        private Text text;

        private float progress;

        public void Init(IHOManager manager)
        {
            core = manager;
            core.OnUpdate += Next;
        }

        public void SetValue(float value, bool isFast = false)
        {
            progress = Mathf.Clamp01( value );
            if (isFast)
                image.fillAmount = progress;

            if (text != null)
                text.text = string.Format( "{0}%", ( progress * 100 ) );
        }

        private void Next()
        {
            if( Mathf.Abs(image.fillAmount - progress)>.01f)
            {
                image.fillAmount = Mathf.MoveTowards( image.fillAmount, progress, .01f );
            }
        }
    }
}
