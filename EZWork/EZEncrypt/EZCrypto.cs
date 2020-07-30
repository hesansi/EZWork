using System.Collections;
using System.Collections.Generic;
using EZWork;
using UnityEngine;

public class EZCrypto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // string testStr = "我为歌狂 wo wei ge kuang 123!!!";
        // Debug.Log("@@@ 原文："+testStr);
        // string testEncrypt = CryptoPrefs.Encrypt(testStr);
        // Debug.Log("@@@ 加密："+testEncrypt);
        // string testDecrypt = CryptoPrefs.Decrypt(testEncrypt);
        // Debug.Log("@@@ 解密："+testDecrypt);
        var settings = new ES3Settings(ES3.EncryptionType.AES, "myPassword");
        
        string path = "测试加密存档222";
        string key = "key";
        string testStr = "我为歌狂 wo wei ge kuang 123!!!";
        // ES3.Save<testValueClass>(key, new testValueClass(), path, settings);
        testValueClass testValue = ES3.Load<testValueClass>(key, path, settings);
        // EZSave.Instance.InitRecord(path);
        // EZSave.Instance.SaveRecord<testValueClass>(key, new testValueClass(), path);
        // testValueClass testValue = EZSave.Instance.LoadRecord<testValueClass>(key, path);
        Debug.Log("### testValue.str: "+testValue.str+" testValue.int："+testValue.testInt);

        // Debug.Log("@@@ 原文："+testStr);
        // string testEncrypt = CryptoPrefs.Encrypt(testStr);
        // Debug.Log("@@@ 加密："+testEncrypt);
        // string testDecrypt = CryptoPrefs.Decrypt(testEncrypt);
        // Debug.Log("@@@ 解密："+testDecrypt);
    }     

    class testValueClass
    {
        public string str = "我为歌狂 wo wei ge kuang 123!!!";
        public int testInt = 123;
    }

}
