using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EZImageAnimation : MonoBehaviour
{
    public Image ImageAnimation;
    public int LoopPartCount;
    public int FrameRate;
    public List<Sprite> StartList;
    public List<Sprite> LoopList;
    public List<Sprite> EndList;
    private List<Sprite> TotalList;
    private int totalImgCount, curTotalImgCount;
    
    private float oneFrameTime, totalFrameTime;
    private float curTime;
    private bool isPause;
    
    private void Start()
    {
        isPause = true;
        
        TotalList = new List<Sprite>();
        TotalList.AddRange(StartList);
        for (int i = 0; i < LoopPartCount; i++) {
            TotalList.AddRange(LoopList);
        }
        TotalList.AddRange(EndList);
        
        int startImgCount = StartList.Count;
        int loopImgCount = LoopList.Count;
        int endImgCount = EndList.Count;
        totalImgCount = startImgCount + loopImgCount * LoopPartCount + endImgCount;
        curTotalImgCount = -1;

        FrameRate = 25;
        oneFrameTime = 1f / FrameRate;
        totalFrameTime = oneFrameTime * totalImgCount;
    }

    public void Play()
    {
        isPause = false;
        curTotalImgCount = 0;
        ImageAnimation.sprite = TotalList[curTotalImgCount];
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Resume()
    {
        isPause = false;
    }

    void Update()
    {
        if (isPause) {
            return;
        }

        curTime += Time.deltaTime;
        if (curTime >= oneFrameTime) {
            curTime = 0;
            curTotalImgCount++;
            if (curTotalImgCount >=totalImgCount) {
                Pause();
                return;
            }
            ImageAnimation.sprite = TotalList[curTotalImgCount];
        }
    }



}









