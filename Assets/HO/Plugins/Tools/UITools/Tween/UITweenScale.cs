using UnityEngine;

namespace UITween
{
    public class UITweenScale:UITween
    {
        [SerializeField]
        public Vector2 from = Vector2.zero;
        [SerializeField]
        public Vector2 to = Vector2.zero;

        private Vector3 _scale = Vector3.zero;
        protected override void Animation()
        {
            _scale = Vector3.LerpUnclamped( from, to, timeLine.Evaluate( Value ) );
            _scale.z = 1;
            rect.localScale = _scale;
        }

#if UNITY_EDITOR
        [ContextMenu( "SetFrom" )]
        private void SetFrom()
        {
            GetRect();
            from = rect.localScale;
        }
        [ContextMenu( "SetTo" )]
        private void SetTo()
        {
            GetRect();
            to = rect.localScale;
        }
        [ContextMenu( "GetFrom" )]
        private void GetFrom()
        {
            GetRect();
            _scale = from;
            _scale.z = 1;
            rect.localScale = _scale;
        }
        [ContextMenu( "GetTo" )]
        private void GetTo()
        {
            GetRect();
            _scale = to;
            _scale.z = 1;
            rect.localScale = _scale;
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
