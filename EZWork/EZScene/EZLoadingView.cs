// Author: He Juncheng
// Created: 2019/03/18

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EZWork
{
    /// <summary>
    /// Loading视图；请继承本类，并实现个性化处理
    /// </summary>
    public abstract class EZLoadingView : MonoBehaviour
    {
        [NonSerialized]
        public float CurProgress, FirstProgress = 50f, SecondProgress = 80f;
        public float ProgressScale = 1f;
        private Coroutine coroutine;
    
        /// <summary>
        /// - 作用：初始化
        /// - 调用方式：被动调用
        /// - 调用时机：Loading场景加载完毕，开始加载目标场景
        /// - 子类重写建议：*不建议重写*；除非有特殊需求
        /// </summary>
        public virtual void StartProgress(UnityAction finish)
        {
            CurProgress = 0;
            InitView();
            coroutine = StartCoroutine(UpdateProgress(FirstProgress, finish));
        }

        /// <summary>
        /// - 作用：自定义初始化（进度条、进度文本或其他个性化处理）
        /// - 调用方式：被动调用
        /// - 调用时机：即将开始加载进度之前
        /// - 子类重写建议：*必须重写*
        /// </summary>
        protected abstract void InitView();
        
        /// <summary>
        /// 卸载旧场景、加载新场景时，同步进度；如果进度走到头，还没加载好，则等待
        /// </summary>
        public virtual void StartSecondProgress()
        {
            if (coroutine != null) {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(UpdateProgress(SecondProgress));
        }
        
        /// <summary>
        /// - 作用：目标场景加载完成后的处理
        /// - 调用方式：被动调用
        /// - 调用时机：目标场景加载完毕
        /// - 子类重写建议：*不建议重写*；除非有特殊需求
        /// </summary>
        public virtual void EndProgress(UnityAction finish)
        {
            if (coroutine != null) {
                StopCoroutine(coroutine);
            }
            StartCoroutine(UpdateProgress(100, finish));
        }

        /// <summary>
        /// 暂停进度，做些特殊处理：入场、出场、等待
        /// </summary>
        public virtual void PauseProgress()
        {
            ProgressScale = 0;
        }

        /// <summary>
        /// 如果新场景具有额外的加载或初始化过程，希望处理完毕后，能够通知Loading继续加载，则可以利用该方法
        /// </summary>
        public virtual void ResumeProgress(float scale = 0.5f)
        {
            ProgressScale = scale;
        }

        /// <summary>
        /// 刷新进度
        /// </summary>
        private IEnumerator UpdateProgress(float toProgress, UnityAction finish = null)
        {
            while(CurProgress < toProgress){
                CurProgress += ProgressScale;
                UpdateView();
                yield return null;
            }

            if (finish != null) {
                finish();
            }
        }

        /// <summary>
        /// - 作用：从0到100刷新进度
        /// - 调用方式：被动调用
        /// - 调用时机：CurProgress刷新之后
        /// - 子类重写建议：*必须重写*
        /// </summary>
        protected abstract void UpdateView();

    }


}
