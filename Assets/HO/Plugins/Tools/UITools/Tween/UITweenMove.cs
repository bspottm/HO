using UnityEngine;

namespace UITween
{
    public class UITweenMove:UITween
    {

        [SerializeField]
        private Vector2 from = Vector2.zero;
        [SerializeField]
        private Vector2 to = Vector2.zero;

        protected override void Animation()
        {
            rect.anchoredPosition = Vector2.LerpUnclamped( from, to, timeLine.Evaluate( Value ) );
        }

#if UNITY_EDITOR
        [ContextMenu( "SetFrom" )]
        private void SetFrom()
        {
            GetRect();
            from = rect.anchoredPosition;
        }
        [ContextMenu( "SetTo" )]
        private void SetTo()
        {
            GetRect();
            to = rect.anchoredPosition;
        }
        [ContextMenu( "GetFrom" )]
        private void GetFrom()
        {
            GetRect();
            rect.anchoredPosition = from;
        }
        [ContextMenu( "GetTo" )]
        private void GetTo()
        {
            GetRect();
            rect.anchoredPosition = to;
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
