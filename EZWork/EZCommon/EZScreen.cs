using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZWork
{
    public class EZScreen
    {
        /// <summary>
        /// 游戏窗口 宽
        /// </summary>
        public static int Width  => Screen.width; 
        /// <summary>
        /// 游戏窗口 高
        /// </summary>
        public static int Height  => Screen.height;
        
        /// <summary>
        /// 游戏窗口 分辨率
        /// </summary>
        public static Vector2 Resolution => new Vector2(Screen.width, Screen.height);
        
        private static ScreenRatioEnum gameRatio;
        /// <summary>
        /// 游戏窗口宽高比
        /// </summary>
        public static ScreenRatioEnum Ratio
        {
            get {
                if (gameRatio == ScreenRatioEnum.NULL) {
                    gameRatio = GetRatio(Width, Height);
                }
                return gameRatio;
            }
            set => gameRatio = value;
        }
        
        /// <summary>
        /// 显示器 分辨率（不常用）
        /// </summary>
        public static Vector2 DeviceResolution => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        
        private static ScreenRatioEnum deviceRatio;
        /// <summary>
        /// 显示器 宽高比（不常用）
        /// </summary>
        public static ScreenRatioEnum DeviceRatio
        {
            get
            {
                if (deviceRatio == ScreenRatioEnum.NULL) {
                    deviceRatio = GetRatio(DeviceResolution.x, DeviceResolution.y);
                }
                return deviceRatio;
            }
            set => deviceRatio = value;
        }

        /// <summary>
        /// 获得宽高比
        /// </summary>
        public static ScreenRatioEnum GetRatio(float width, float height)
        {
            var ratio =  Math.Round(width/height, 2);
            if ( ratio == Math.Round(3f/2, 2)) 
                return ScreenRatioEnum.SCREEN_3_2;
            if ( ratio == Math.Round(4f/3, 2)) 
                return ScreenRatioEnum.SCREEN_4_3;
            if ( ratio == Math.Round(16f/9, 2)) 
                return ScreenRatioEnum.SCREEN_16_9;
            if ( ratio == Math.Round(21f/9, 2)) 
                return ScreenRatioEnum.SCREEN_21_9;
            return ScreenRatioEnum.SCREEN_OTHER;
        }


    }

    public enum ScreenRatioEnum
    {
        NULL = 0,
        SCREEN_3_2,
        SCREEN_4_3,
        SCREEN_16_9,
        SCREEN_21_9,
        SCREEN_OTHER
    }
}



