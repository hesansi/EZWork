using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using Path = System.IO.Path;

namespace EZWork
{
    public class EZSave : EZSingletonStatic<EZSave>
    {
        private EZSave() { }
//        private ES3File saveFile;
        private ES3Settings settings;
        public ES3Settings Settings
        {
            get
            {
                if (settings == null) {
                    settings = new ES3Settings(ES3.EncryptionType.AES, "4vbsdf5s");
                }
                return settings;
            }
        }

        private string prefix = "20200326/";

        public void InitRecord(string path)
        {
            
            path = prefix + path;
            if (!ES3.FileExists(path)) {
                ES3.Save<string>("FileName", path, path, Settings);
            }
        }

        /// <summary>
        /// 存档保存
        /// </summary>
        public void SaveRecord<T>(string key, T value,string recordPath)
        {
            recordPath = prefix + recordPath;
            ES3.Save<T>(key, value, recordPath, Settings);
        }
        /// <summary>
        /// 存档加载
        /// </summary>
        public T LoadRecord<T>(string key,string recordPath) where T:new()
        {
            recordPath = prefix + recordPath;
            if (ES3.KeyExists(key, recordPath)) {
                return ES3.Load<T>(key, recordPath, Settings);
            }
           // 如果Key不存在，则写入默认的，并再次读取
            SaveRecord(key, new T(),recordPath);
            return LoadRecord<T>(key,recordPath);
        }
        
        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeletRecord(string path)
        {
            path = prefix + path;
            if (ES3.FileExists(path)) {
                ES3.DeleteFile(path);
            }
        }
    }

}

