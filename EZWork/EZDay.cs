using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZWork
{
    public class EZDay
    {
        private EZDay() { }
        // 真实时间
        private static float RealDaySec = 60f * 60 * 24;
        // 假设游戏中1天12分钟；相当于 2f * 60 * 24
        private static float SecScale = 30f;
        // 虚拟时间单位
        public static int vOneDayHours = 24;
        public static int vOneHourMinutes = 60;
        public static int vOneMinuteSecs = 2;
        // 虚拟总秒数：虚拟当前时间
        public static float SecTime;
        // 虚拟小时
        private static int _hour;
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
            EZTime.Instance.RemoveInvoke(TimeGo);
            if (isDirect) {
                SecTime = 0;
            }
            // 每秒调用一次
            EZTime.Instance.InvokeRepeat(TimeGo, 0, 1f, -1);
        }
        
        /// <summary>
        /// 暂停计时
        /// </summary>
        public static void PauseDay()
        {
            StartDay(false);
            EZTime.Instance.PauseInvoke(TimeGo);
        }

        /// <summary>
        /// 恢复计时
        /// </summary>
        public static void ResumeDay()
        {
            StartDay(false);
            EZTime.Instance.ResumeInvoke(TimeGo);
        }

        /// <summary>
        /// 设置当前时间
        /// </summary>
        public static void SetHour(int hour)
        {
            SetDayTime(hour, 0);
        }
        
        public static void SetMinute(int minute)
        {
            SetDayTime(0, minute);
        }
        
        public static void SetDayTime(int hour, int minute)
        {
            SecTime = hour * vOneHourMinutes * vOneMinuteSecs + minute * vOneMinuteSecs;
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
        }

        private static void TimeGo()
        {
            SecTime++;
        }

    }
    
}

