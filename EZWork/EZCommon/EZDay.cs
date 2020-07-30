using System;
using System.Collections;
using System.Collections.Generic;
using GameConfig;
using UnityEngine;

namespace EZWork
{  
    public class EZDay
    {
        private EZDay() { }
        // 真实时间
        private static float RealDaySec = 60f * 60 * 24;
        // 虚拟时间
        private static float vDaySec;
        public static float VDaySec
        {
            get
            {
                if (vDaySec == 0) {
                    vDaySec = vOneDayHours * vOneHourMinutes * vOneMinuteSecs;
                }
                return vDaySec;
            }
        }
        // 假设游戏中1天12分钟；相当于 2f * 60 * 24
        private static float SecScale = 30f;
        // 虚拟时间单位
        public static int vOneDayHours = 24;
        public static int vOneHourMinutes = 60;
        public static int vOneMinuteSecs = 2;
        // 虚拟总秒数：虚拟当前时间
        public static float SecTime;
        // 当前时间所占一天的比例
        public static float SecPercent;
        // 虚拟小时
        private static int _hour;   
        public static bool IsRunning
        {
            get;
            private set;
        }
        public static int Hour
        {
            get
            {
                // SecTime/一分钟秒数/一小时分钟
                _hour = (int)SecTime / vOneMinuteSecs / vOneHourMinutes;
                if (_hour == 24)
                    _hour = 0;
                return _hour; 
            }
        }
        // 虚拟分钟
        private static int _minute;
        public static int Minute
        {
            get
            {
                _minute = (int)SecTime/vOneMinuteSecs - Hour * vOneHourMinutes;
                return _minute; 
            }
        }
        // 虚拟秒
        private static int _second;
        public static int Second
        {
            get
            {
                _second = (int)SecTime - Minute * vOneMinuteSecs - Hour * vOneHourMinutes * vOneMinuteSecs;
                return _second; 
            }
        }
        
        /// <summary>
        /// 开始一天的计时
        /// </summary>
        public static void StartDay(bool isDirect = true)
        {
            IsRunning = true;
            //EZTime.Instance.RemoveInvoke(TimeGo);
            if (isDirect) {
                SecTime = 0;
            }
            // 每秒调用一次
            //EZTime.Instance.InvokeRepeat(TimeGo, 0, 1f, -1);
        }
        
        /// <summary>
        /// 暂停计时
        /// </summary>
        //public static void PauseDay()
        //{
        //    if(IsRunning)
        //    {
        //        IsRunning = false;
        //        //有bug，暂时使用协程
        //        UIManager.Instance.WaitForSeconds(0,()=>{
        //            EZTime.Instance.PauseInvoke(TimeGo);
        //        });
        //    }
        //}

        /// <summary>
        /// 恢复计时
        /// </summary>
        //public static void ResumeDay()
        //{
        //    IsRunning = true;
        //    EZTime.Instance.ResumeInvoke(TimeGo);
        //}

        /// <summary>
        /// 设置当前时间
        /// </summary>
        public static void SetHour(int hour)
        {
            SetDayTime(hour, 0);
            UpdateSecPercent();
        }
        
        public static void SetMinute(int minute)
        {
            SetDayTime(0, minute);
            UpdateSecPercent();
        }
        public static void SetDayTime(int hour, int minute)
        {
            SecTime = hour * vOneHourMinutes * vOneMinuteSecs + minute * vOneMinuteSecs;
            UpdateSecPercent();
        }

        /// <summary>
        /// 增加时间
        /// </summary>
        public static void AddHour(int hour)
        {
            AddDayTime(hour, 0);
        }
        
        public static void AddMinute(int minute)
        {
            AddDayTime(0, minute);
        }
        
        public static void AddDayTime(int hour, int minute)
        {
            SecTime += hour * vOneHourMinutes * vOneMinuteSecs + minute * vOneMinuteSecs;
            UpdateSecPercent();
        }

        private static void UpdateSecPercent()
        {
            SecPercent = SecTime / VDaySec;            
        }
        
        //private static void TimeGo()
        //{
        //   SecTime++;
        //   UpdateSecPercent();          
        //}

    }
    
}

