using System;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using EZWork;
using UnityEngine;
using UnityEngine.Rendering;

public class SampleLoadingView : EZLoadingView
{
    public Slider ProgressSlider;
    public TextMeshProUGUI ProgressText;
    public CanvasGroup CanvasGroup;
    protected override void InitView()
    {
        FirstProgress = 30;
        // 个性化处理：暂停进度，等待进入动画完成
        PauseProgress();
        ProgressSlider = FindObjectOfType<Slider>();
        ProgressText = ProgressSlider.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
        CanvasGroup = transform.Find("LoadingCanvas").GetComponent<CanvasGroup>();
        AnimateIn();
    }

    // 1. 入场动画
    private void AnimateIn()
    {
        CanvasGroup.DOFade(1, 2f).OnComplete(() =>
        {
            ProgressScale = 0.5f;
        });
    }

    // 2. 出场动画
    private void AnimateOut()
    {
        // 室内室外摄像机排序方式不同
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        CanvasGroup.DOFade(0, 2f).OnComplete(() =>
        {
            ProgressScale = 0.5f;
        });
    }

    protected override void UpdateView()
    {
        if (ProgressScale > 0) {
            // 前90帧就走到头；因为100帧将自动卸载Loading场景
            float percent = CurProgress / 90f;
            ProgressSlider.value = percent;
            ProgressText.text = percent.ToString("F");
            // 60帧暂停，等待下个场景加载完毕后调用 ResumeProgress()
            // 下面判断等价于 if(CurProgress == 60)，为了防止浮点数漂移
            if (Math.Abs(CurProgress - 60) < 0.01f) {
                PauseProgress();
            }
            // 90帧到头了，处理Loading淡出（淡出后，继续走到100，卸载Loading场景）
            // 下面判断等价于 if(CurProgress == 90)，为了防止浮点数漂移
            if (Math.Abs(CurProgress - 90f) < 0.01f) {
                PauseProgress();
                AnimateOut();
            } 
        }
    }
    
    
}
