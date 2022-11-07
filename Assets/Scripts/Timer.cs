using System;

namespace MultiplayerRunner
{
    [Serializable]
    public class Timer
    {
        public event Action TimeComleted;

        public float duration;
        public float Duration { get; set; }

        public float tick;

        public bool isComlete;

        public void Start(float duration)
        {
            this.duration = duration;
            tick = duration;
            isComlete = false;
        }

        public void Create(float duration)
        {
            this.duration = duration;
            isComlete = true;
        }

        public void Reset()
        {
            tick = duration;
            isComlete = false;
        }

        public void UpdateTick(float delta)
        {
            if (isComlete) return;

            tick -= delta;

            if(tick <= 0)
            {
                isComlete = true;
                TimeComleted?.Invoke();
            }
        }

        public void UnSubscribeAll()
        {
            TimeComleted = null;
        }
    }
}