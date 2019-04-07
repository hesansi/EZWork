using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 存档数据类型；因为类型名都很简短，所以放入命名空间，防止命名冲突 
namespace EZWork
{
    /// <summary>
    /// 存档类型，同时也是存档文件名
    /// </summary>
    public enum RecordType
    {
        Record0,    // 可用于存档模板
        Record1,
        Record2,
        Record3
    }

    /// <summary>
    /// 存档包含的数据类型，同时也是该数据类型的 Key 名
    /// </summary>
    public enum RecordDataType
    {
        PlayerData,
        DateData
    }
    
    /// <summary>
    /// 测试：玩家数据
    /// </summary>
    public class PlayerData
    {
        public string Name;
        public int Age;
        public List<ItemData> BagList;
    }

    /// <summary>
    /// 测试：日期
    /// </summary>
    public class DateData
    {
        public int Year, Month, Day;
    }
    
    /// <summary>
    /// 测试：道具
    /// </summary>
    public class ItemData
    {
        public int ID;
        public string Name;
    }
}

