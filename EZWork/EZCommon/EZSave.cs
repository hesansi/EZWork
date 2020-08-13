using UnityEngine;

namespace EZWork
{
    public class EZSave : EZSingleton<EZSave>
    {
        private EZSave()
        {
        }
        
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
        private string normalPath = "Normal";
        private string recordPath;
        public string RecordPath
        {
            get => recordPath;
        }

        public void InitRecord(string path = "Record")
        {
            if (ES3.FileExists(path)) {
                recordPath = path;
            }
            else {
                ES3.Save<string>("FileName", path, path, Settings);
                recordPath = path;
            }
        }

        /// <summary>
        /// 普通保存
        /// </summary>
        public void Save<T>(string key, T value, string path = null)
        {
            ES3.Save<T>(key, value, path ?? normalPath, Settings);
        }
        /// <summary>
        /// 存档保存
        /// </summary>
        public void SaveRecord<T>(string key, T value)
        {
            ES3.Save<T>(key, value, recordPath, Settings);
        }
        
        /// <summary>
        /// 普通加载
        /// </summary>
        public T Load<T>(string key, string path = null)
        {
            return ES3.Load<T>(key, path ?? normalPath, default(T), Settings);
        }
        /// <summary>
        /// 存档加载
        /// </summary>
        public T LoadRecord<T>(string key) where T:new()
        {
            if (ES3.KeyExists(key, recordPath)) {
                return ES3.Load<T>(key, recordPath, Settings);
            }
           // 如果Key不存在，则写入默认的，并再次读取
            SaveRecord(key, new T());
            return LoadRecord<T>(key);
        }

        /// <summary>
        /// 删除普通Key
        /// </summary>
        public void DeleteKey(string key, string path = null)
        {
            if (ES3.KeyExists(key, path ?? normalPath, Settings)) {
                ES3.DeleteKey(key, path ?? normalPath, Settings);
            }
           
        }
        /// <summary>
        /// 删除存档key
        /// </summary>
        public void DeleteRecordKey(string key)
        {
            if (ES3.KeyExists(key, recordPath, Settings)) {
                ES3.DeleteKey(key, recordPath, Settings);
            }
        }
        
        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeletRecord(string path)
        {
            if (ES3.FileExists(path)) {
                ES3.DeleteFile(path);
            }
        }

        /// <summary>
        /// 复制存档
        /// </summary>
        public void CopyRecord(string oldPath, string newPath)
        {
            if (ES3.FileExists(oldPath)) {
                ES3.CopyFile(oldPath, newPath);
            }
        }
        
        /// <summary>
        /// 复制模板存档，创建新存档
        /// </summary>
        public void CopyRecordModule(string oldPath, string newPath)
        {
            if (ES3.FileExists(oldPath)) {
                ES3.CopyFile(oldPath, newPath);
            }
        }

        /// <summary>
        /// 备份存档
        /// </summary>
        public void CreateRecordBackup()
        {
            ES3.CreateBackup(recordPath);
        }

        /// <summary>
        /// 复原存档
        /// </summary>
        public void RestoreRecordBackup()
        {
            if(ES3.RestoreBackup(recordPath))
                Debug.Log("Backup restored.");
            else
                Debug.Log("Backup could not be restored as no backup exists.");
        }

    }

}

