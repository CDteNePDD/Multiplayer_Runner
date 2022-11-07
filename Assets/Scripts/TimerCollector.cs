using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerRunner
{
    public class TimerCollector : MonoBehaviour
    {
        public static TimerCollector Instance;

        [SerializeField] private List<Timer> timers;

        public Timer StartTimer(float duration)
        {
            Timer timer = new Timer();
            timer.Start(duration);
            timers.Add(timer);

            return timer;
        }

        public Timer CreateTimer(float duration)
        {
            Timer timer = new Timer();
            timer.Create(duration);
            timers.Add(timer);

            return timer;
        }

        public void DeleteTimer(Timer timer)
        {
            timer.UnSubscribeAll();
            timers.Remove(timer);
        }

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
        }

        private void Update()
        {
            //if (isServer == false) return;

            foreach(var timer in timers)
            {
                timer.UpdateTick(Time.deltaTime);
            }
        }
    }
}
