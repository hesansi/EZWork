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
        ProgressScale = 0;
        ProgressSlider = FindObjectOfType<Slider>();
        ProgressText = ProgressSlider.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
        CanvasGroup = transform.Find("LoadingCanvas").GetComponent<CanvasGroup>();
    }

    // 入场动画
    private bool isAnimatedIn = false;
    private void AnimateIn()
    {
        CanvasGroup.DOFade(1, 2f).OnComplete(() =>
        {
            ProgressScale = 0.5f;
        });
    }

    // 出场动画
    private bool isAnimatedOut = false;
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
        // 1. 入场动画
        if (!isAnimatedIn) {
            isAnimatedIn = true;
            AnimateIn();
        }
        
        // 前80帧走到头
        if (!isAnimatedOut) {
            float percent = CurProgress / 80f;
            ProgressSlider.value = percent;
            ProgressText.text = percent.ToString("F");
            // 最后20帧，先淡出；然后走到100，自动移除Loading
            if (CurProgress >= 80) {
                ProgressScale = 0f;
                isAnimatedOut = true;
                AnimateOut();
            }
        }

    }
    
}
