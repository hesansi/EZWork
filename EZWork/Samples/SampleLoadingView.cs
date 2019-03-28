using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using EZWork;
using UnityEngine;

public class SampleLoadingView : EZLoadingView
{
    public Slider ProgressSlider;
    public TextMeshProUGUI ProgressText;
    public CanvasGroup CanvasGroup;
    private bool isFadeOut = false;
    protected override void InitView()
    {
        FirstProgress = 55;
        ProgressSlider = FindObjectOfType<Slider>();
        ProgressText = ProgressSlider.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
        CanvasGroup = transform.Find("LoadingCanvas").GetComponent<CanvasGroup>();
        CanvasGroup.DOFade(1, 0.25f);
    }
    
    protected override void UpdateView()
    {
        // 前80帧走到头
        if (!isFadeOut) {
            float percent = CurProgress / 60f;
            ProgressSlider.value = percent;
            ProgressText.text = percent.ToString("F");
            // 最后20帧添加动画
            if (CurProgress >= 60) {
                isFadeOut = true;
                CanvasGroup.DOFade(0, 0.2f);
            }
        }

    }
    
}
