using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EZWork
{
    public class EZTime : EZSingleton<EZTime>
    {
        private EZTime() { }
        public delegate void Handler();
        public delegate void Handler<T1>(T1 param1);
        public delegate void Handler<T1, T2>(T1 param1, T2 param2);
        public delegate void Handler<T1, T2, T3>(T1 param1, T2 param2, T3 param3);
        private List<TimerHandler> _pool = new List<TimerHandler>();
        private List<TimerHandler> _handlers = new List<TimerHandler>();
        private float currentTime => Time.time;

        void CreateTimer(Delegate method, float delayTime, bool repeat = false, float interval = 1f, int count = 1, params object[] args)
        {
            TimerHandler handler;
            if(_pool.Count > 0) {
                handler = _pool[_pool.Count - 1];
                _pool.Remove(handler);
            }
            else {
                handler = new TimerHandler();
            }
            handler.method = method;
            handler.delay = delayTime;
            handler.end = handler.delay + currentTime;
            handler.repeat = repeat;
            handler.args = args;
            handler.count = count;
            handler.interval = interval;
            _handlers.Add(handler);
        }

        /// <summary>
        /// 激活Timer一次
        /// </summary>
        /// <param name="method"></param>
        /// <param name="delayTime"></param>
        public void InvokeOnce(Handler method, float delayTime)
        {
            CreateTimer(method, delayTime, false, 1);
        }
        public void InvokeOnce<T>(Handler<T> method, float delayTime, params object[] args)
        {
            CreateTimer(method, delayTime, false, 1, 1, args);
        }
        public void InvokeOnce<T1, T2>(Handler<T1, T2> method, float delayTime, params object[] args)
        {
            CreateTimer(method, delayTime, false, 1, 1, args);
        }
        public void InvokeOnce<T1, T2, T3>(Handler<T1, T2, T3> method, float delayTime, params object[] args)
        {
            CreateTimer(method, delayTime, false, 1, 1, args);
        }
        
        /// <summary>
        /// 重复激活Timer
        /// </summary>
        /// <param name="method"></param>
        /// <param name="delayTime"></param>
        public void InvokeRepeat(Handler method, float delayTime, float interval = 1f, int count = -1)
        {
            CreateTimer(method, delayTime, true, interval, count);
        }
        public void InvokeRepeat<T>(Handler<T> method, float delayTime, float interval = 1f, int count = -1, params object[] args)
        {
            CreateTimer(method, delayTime, true, interval, count, args);
        }
        public void InvokeRepeat<T1, T2>(Handler<T1, T2> method, float delayTime, float interval = 1f, int count = -1, params object[] args)
        {
            CreateTimer(method, delayTime, true, interval, count, args);
        }
        public void InvokeRepeat<T1, T2, T3>(Handler<T1, T2, T3> method, float delayTime, float interval = 1f, int count = -1, params object[] args)
        {
            CreateTimer(method, delayTime, true, interval, count, args);
        }

       

        /// <summary>
        /// 移除Timer
        /// </summary>
        public void RemoveInvoke(Handler method)
        {
            clear(method);
        }
        public void RemoveInvoke<T>(Handler<T> method)
        {
            clear(method);
        }
        public void RemoveInvoke<T1, T2>(Handler<T1, T2> method)
        {
            clear(method);
        }
        public void RemoveInvoke<T1, T2, T3>(Handler<T1, T2, T3> method)
        {
            clear(method);
        }
        
        /// <summary>
        /// 移除所有Timer
        /// </summary>
        public void clearAllTimer()
        {
            foreach (TimerHandler handler in _handlers) {
                clear(handler.method);
                return;
            }
        }
        
        private void clear(Delegate method)
        {
            TimerHandler handler = _handlers.FirstOrDefault(t => t.method == method);
            if (handler != null) {
                _handlers.Remove(handler);
                handler.clear();
                _pool.Add(handler);
            }
        }

        /// <summary>
        /// 暂停Timer
        /// </summary>
        public void PauseInvoke(Handler method)
        {
            Pause(method);
        }
        public void PauseInvoke<T>(Handler<T> method)
        {
            Pause(method);
        }
        public void PauseInvoke<T1, T2>(Handler<T1, T2> method)
        {
            Pause(method);
        }
        public void PauseInvoke<T1, T2, T3>(Handler<T1, T2, T3> method)
        {
            Pause(method);
        }

        private void Pause(Delegate method)
        {
            TimerHandler handler = _handlers.FirstOrDefault(t => t.method == method);
            float remainTime = handler.end - currentTime;
            if (remainTime> 0) {
                handler.remain = remainTime;
            }
            handler.pause = true;
        }
        
        /// <summary>
        /// 恢复Timer
        /// </summary>
        public void ResumeInvoke(Handler method)
        {
            Resume(method);
        }
        public void ResumeInvoke<T>(Handler<T> method)
        {
            Resume(method);
        }
        public void ResumeInvoke<T1, T2>(Handler<T1, T2> method)
        {
            Resume(method);
        }
        public void ResumeInvoke<T1, T2, T3>(Handler<T1, T2, T3> method)
        {
            Resume(method);
        }
        private void Resume(Delegate method)
        {
            TimerHandler handler = _handlers.FirstOrDefault(t => t.method == method);
            handler.pause = false;
        }

        void UpdateTime()
        {
            if (_handlers.Count<=0) 
                return;
            for(int i = 0; i < _handlers.Count; i++)
            {
                TimerHandler handler = _handlers[i];
                float curT = currentTime;
                // 暂停处理
                if (handler.pause) {
                    handler.end = curT + handler.remain;
                } 
                
                if (curT >= handler.end) {
                    Delegate method = handler.method;
                    object[] args = handler.args;
                    // 是否重复
                    if (handler.repeat) {
                        while (curT >= handler.end) {
                            // -1代表无限重复
                            if (handler.count != -1) {
                                handler.count--;
                                if(handler.count <= 0)
                                    clear(handler.method);
                            }
                            handler.end += handler.interval;
                            method.DynamicInvoke(args);
                        }
                    }
                    else {
                        clear(handler.method);
                        method.DynamicInvoke(args);
                    }
                }
                
            }
        }

        void Update()
        {
            UpdateTime();
        }
        
        
    }

    public class TimerHandler
    {
        //延迟时间；时间间隔；截止时间；剩余时间
        public float delay, interval, end, remain;

        //是否重复执行
        public bool repeat;
        //是否暂停
        public bool pause;
        
        //重复次数; -1为无限次
        public int count;

        //处理方法
        public Delegate method;

        //参数
        public object[] args;

        //清理
        public void clear()
        {
            pause = false;
            method = null;
            args = null;
        }
    }
}

