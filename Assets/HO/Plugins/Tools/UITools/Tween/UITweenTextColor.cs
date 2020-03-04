using UnityEngine;
using UnityEngine.UI;

namespace UITween
{
    public class UIAnimatorTextColor:UITween
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private Color from = Color.white;
        [SerializeField]
        private Color to = Color.white;

        protected override void Animation()
        {
            float val = timeLine.Evaluate( Value );
            float r = Mathf.LerpUnclamped( from.r, to.r, val );
            float g = Mathf.LerpUnclamped( from.g, to.g, val );
            float b = Mathf.LerpUnclamped( from.b, to.b, val );
            float a = Mathf.LerpUnclamped( from.a, to.a, val );

            text.color = new Color( r, g, b, a );
        }

#if UNITY_EDITOR
        [ContextMenu( "SetFrom" )]
        private void SetFrom()
        {
            GetRect();
            from = text.color;
        }
        [ContextMenu( "SetTo" )]
        private void SetTo()
        {
            GetRect();
            to = text.color;
        }
        [ContextMenu( "GetFrom" )]
        private void GetFrom()
        {
            GetRect();
            text.color = from;
        }
        [ContextMenu( "GetTo" )]
        private void GetTo()
        {
            GetRect();
            text.color = to;
        }

        [ContextMenu( "Swith" )]
        private void Swith()
        {
            var t = from;
            from = to;
            to = t;
        }
#endif
    }
}
