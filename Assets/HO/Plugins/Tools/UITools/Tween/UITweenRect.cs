using UnityEngine;

namespace UITween
{
    public class UITweenRect:UITween
    {

        [SerializeField]
        private Rect from;
        [SerializeField]
        private Rect to;

        protected override void Animation()
        {
            rect.anchoredPosition = Vector2.LerpUnclamped( from.position, to.position, timeLine.Evaluate( Value ) );
            rect.sizeDelta = Vector2.LerpUnclamped( from.size, to.size, timeLine.Evaluate( Value ) );
        }

#if UNITY_EDITOR
        [ContextMenu( "SetFrom" )]
        private void SetFrom()
        {
            GetRect();
            from.position = rect.anchoredPosition;
            from.size = rect.sizeDelta;
        }
        [ContextMenu( "SetTo" )]
        private void SetTo()
        {
            GetRect();
            to.position = rect.anchoredPosition;
            to.size = rect.sizeDelta;
        }
        [ContextMenu( "GetFrom" )]
        private void GetFrom()
        {
            GetRect();
            rect.anchoredPosition = from.position;
            rect.sizeDelta = from.size;
        }
        [ContextMenu( "GetTo" )]
        private void GetTo()
        {
            GetRect();
            rect.anchoredPosition = to.position;
            rect.sizeDelta = to.size;
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
