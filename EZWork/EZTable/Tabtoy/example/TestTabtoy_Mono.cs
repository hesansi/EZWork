using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TabToySpace;
using TabToySpace.EZWork;


public class TestTabtoy_Mono : MonoBehaviour
{
    void Start()
    {
        EZTable.Instance.Init();
    }

    public void OnMyDataBtnClicked()
    {
        // 索引
        PlayerData myData = EZTable.Instance.GameTable.PlayerDataByID[1];
        Debug.Log("== ID: " + myData.ID + " name: " + myData.Name + " year: " + myData.Year + " sex: " + myData.Sex);
        foreach (int skillID in myData.Skill) {
            Debug.Log("--- skillID: " + skillID);
        }

        myData = EZTable.Instance.GameTable.PlayerDataByID[2];
        Debug.Log("== ID: " + myData.ID + " name: " + myData.Name + " year: " + myData.Year + " sex: " + myData.Sex);
        foreach (int skillID in myData.Skill) {
            Debug.Log("--- skillID: " + skillID);
        }
    }

    public void OnMyKVBtnClicked()
    {
        // KV
        foreach (MyKV myKv in EZTable.Instance.GameTable.MyKV) {
            Debug.Log("www IP: " + myKv.ServerIP + " Port: " + myKv.ServerPort);
        }
    }

    public void OnHeDataBtnClicked()
    {
        foreach (NPCData heData in EZTable.Instance.GameTable.NPCData) {
            Debug.Log("PPP ID: " + heData.ID + " name: " + heData.Name + " x: " + heData.Position[0] + " y: " +
                      heData.Position[1]);
        }
    }
}