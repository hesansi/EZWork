using EZWork;

public class RecordManager// : EZSingletonStatic<RecordManager>
{
    public static DateData DateData;
    public static PlayerData PlayerData;
    
    /// <summary>
    /// 加载存档，如果存档不存在则创建新存档
    /// </summary>
    /// <param name="recordType">存档类型，注意需要与文件名相同</param>
    public static void InitOrCreateRecord(RecordType recordType)
    {
        InitOrCreateRecord(recordType.ToString());
    }

    public static void InitOrCreateRecord(string path)
    {
        EZSave.Instance.InitRecord(path);
        LoadAllData();
    }
    
    /// <summary>
    /// 加载所有数据：Save后，如果想使用最新数值，需要调用此方法
    /// </summary>
    public static void LoadAllData()
    {
        LoadPlayerData();
        LoadDateData();
    }

    /// <summary>
    /// 非缓存保存数据
    /// </summary>
    public static void Save<T>(RecordDataType dataType, T value)
    {
        EZSave.Instance.SaveRecord(dataType.ToString(), value);
    }

    /// <summary>
    /// 保存某类型数据
    /// </summary>
    public static void SaveData(RecordDataType dataType)
    {
        switch (dataType) {
            case RecordDataType.PlayerData: SavePlayerData(); break;
            case RecordDataType.DateData: SaveDateData(); break;
        }
    }

    /// <summary>
    /// 加载某类型数据
    /// </summary>
    public static void LoadData(RecordDataType dataType)
    {
        switch (dataType) {
            case RecordDataType.PlayerData: LoadPlayerData(); break;
            case RecordDataType.DateData: LoadDateData(); break; 
        }
    }
    
    /// <summary>
    /// 删除存档
    /// </summary>
    public static void DeleteRecord(RecordType recordType)
    {
        EZSave.Instance.DeletRecord(recordType.ToString());
    }
    
    /// <summary>
    /// 删除存档中某个Key及其数据
    /// </summary>
    public static void DeleteRecordKey(RecordDataType recordData)
    {
        EZSave.Instance.DeleteRecordKey(recordData.ToString());
    }

    /// <summary>
    /// 复制存档
    /// </summary>
    public static void CopyRecord(RecordType oldRecordType, RecordType newRecordType)
    {
        EZSave.Instance.CopyRecord(oldRecordType.ToString(), newRecordType.ToString());
    }

    /// <summary>
    /// 复制模板存档
    /// </summary>
    public static void CopyRecordModule(RecordType newRecordType)
    {
        EZSave.Instance.CopyRecordModule(RecordType.Record0.ToString(), newRecordType.ToString());
    }

    /// <summary>
    /// 备份存档
    /// </summary>
    public void CreateBackup()
    {
        EZSave.Instance.CreateRecordBackup();
    }

    /// <summary>
    /// 复原存档
    /// </summary>
    public void RestoreBackup()
    {
        EZSave.Instance.RestoreRecordBackup();
    }

    /************************ DateData ******************/
    /// <summary>
    /// PlayerData 保存
    /// </summary>
    public static void SaveDateData()
    {
        EZSave.Instance.SaveRecord(RecordDataType.DateData.ToString(), DateData);
    }

    /// <summary>
    /// PlayerData 加载：Save后，如果想使用最新数值，需要调用此方法
    /// </summary>
    public static void LoadDateData()
    {
        DateData = EZSave.Instance.LoadRecord<DateData>(RecordDataType.DateData.ToString());
    }

    /************************ PlayerData ******************/
    /// <summary>
    /// PlayerData 保存
    /// </summary>
    public static void SavePlayerData()
    {
        EZSave.Instance.SaveRecord(RecordDataType.PlayerData.ToString(), PlayerData);
    }

    /// <summary>
    /// PlayerData 加载：Save后，如果想使用最新数值，需要调用此方法
    /// </summary>
    public static void LoadPlayerData()
    {
        PlayerData = EZSave.Instance.LoadRecord<PlayerData>(RecordDataType.PlayerData.ToString());
    }

    
    
}