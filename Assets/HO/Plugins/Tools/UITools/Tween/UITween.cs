using System;
using UnityEngine;

namespace UITween
{
    public class UITween:MonoBehaviour
    {
        [SerializeField]
        protected AnimationCurve timeLine = AnimationCurve.Linear( 0, 0, 1, 1 );
        [SerializeField, Min( 0 )]
        protected float duration = 1f;
        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
                ValueInit();
            }
        }
        protected int direction = 1;
        protected float currentTime = 0;
        protected float moveStep = 0;
        [HideInInspector]
        protected float _value = 0;
        public float Value
        {
            get { return _value; }
            set { _value = Mathf.Clamp( value, 0f, 1f ); }
        }

        [SerializeField]
        private bool playOnAwake = false;

        public Action OnFinish;

        public bool isPlay { get; private set; }

        protected RectTransform rect;

        private void Awake()
        {
            isPlay = false;
            Init();
        }

        private void Start()
        {
            if (playOnAwake)
                PlayForward();
        }

        protected void ValueInit()
        {
            duration = Mathf.Max( 0, duration );
            moveStep = duration > 0 ? 1f / duration : 1;
            OnFinish = null;
        }

        [ContextMenu( "PlayF" )]
        public void PlayForward()
        {
            PlayForward( true );
        }


        public virtual void PlayForward(bool isReset)
        {
            direction = 1;
            if (isReset || !isPlay)
                ResetToBegenin();
            isPlay = true;
        }

        [ContextMenu( "PlayB" )]
        public void PlayBackward()
        {
            PlayBackward( true );
        }

        public virtual void PlayBackward(bool isReset)
        {
            direction = -1;
            if (isReset || !isPlay)
                ResetToEnding();
            isPlay = true;
        }



        public virtual void ResetToBegenin()
        {
            ValueInit();
            currentTime = 0;
            _value = 0;

        }

        public virtual void ResetToEnding()
        {
            ValueInit();
            currentTime = duration;
            _value = 1;
        }

        public virtual void Stop()
        {
            OnFinish?.Invoke();
            ValueInit();
            isPlay = false;
        }

        protected virtual void Init()
        {
            GetRect();
        }

        private void Update()
        {
            if (!isPlay)
                return;

            if (currentTime < 0 || currentTime > duration)
                return;

            currentTime += Time.deltaTime * ( float )direction;
            Value = currentTime * moveStep;
            Animation();

            if (direction == 1 && currentTime > duration)
            {
                currentTime = duration;
                isPlay = false;
                OnFinish?.Invoke();
                OnFinish = null;
            }
            if (direction == -1 && currentTime < 0f)
            {
                currentTime = 0;
                isPlay = false;
                OnFinish?.Invoke();
                OnFinish = null;
            }
        }

        protected virtual void Animation()
        {

        }

        protected virtual RectTransform GetRect()
        {
            if (rect == null)
                rect = GetComponent<RectTransform>();
            return rect;
        }
    }
}
