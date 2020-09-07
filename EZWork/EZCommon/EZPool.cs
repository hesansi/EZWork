using System.Collections.Generic;
using System.Text;
using EZWork;
using UnityEngine;

namespace EZWork
{
    [System.Serializable]
    public class EZPoolItem
    {
        public string poolName;
        public GameObject poolObject;
        public int poolAmount;
        public bool shouldExpand = true;
    }

    public class EZPool : EZSingletonMono<EZPool>
    {
        private const string DefaultRootObjectPoolName = "(singleton) EZWork.EZPool";
        // 默认父对象：所有缓存池都在该对象下
        private string rootPoolName = DefaultRootObjectPoolName;
        private string RootPoolName
        {
            get
            {
                if (string.IsNullOrEmpty(rootPoolName)) {
                    rootPoolName = DefaultRootObjectPoolName;
                    GetParentPoolObject(RootPoolName);
                }            
                return rootPoolName;
            }
            set => rootPoolName = value;
        }

        // 所有缓存对象列表
        private Dictionary<string, List<EZPoolItem>> PooledObjects = new Dictionary<string,  List<EZPoolItem>>();
        // 不同类型或缓存池对象列表
        private Dictionary<string, EZPoolItem> ItemsToPool = new Dictionary<string, EZPoolItem>();

        /// <summary>
        /// 注册缓存池
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="poolObject">被缓存的对象</param>
        /// <param name="poolAmount">初始化缓存数量</param>
        /// <param name="shouldExpand">是否自动扩展</param>
        public void Regist(string poolName, GameObject poolObject, int poolAmount = 2, bool shouldExpand = true)
        {
            if (ItemsToPool.ContainsKey(poolName)){
                return;
            }
            
            EZPoolItem pItem = new EZPoolItem();
            pItem.poolObject = poolObject;
            pItem.poolName = poolName;
            pItem.poolAmount = poolAmount;
            pItem.shouldExpand = shouldExpand;
            ItemsToPool.Add(poolName, pItem);

            for (int i = 0; i < poolAmount; i++) {
                CreatePooledObject(pItem);
            }
        }

        /// <summary>
        /// 创建对象。三种情况：如果存在且够用则返回，如果不够用则新建，如果不存在则报错
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <returns></returns>
        public GameObject Create(string poolName)
        {
            // 1. 如果已存在，则直接返回
            if (PooledObjects.ContainsKey(poolName)){
                foreach (EZPoolItem poolItem in PooledObjects[poolName]){
                    // 1.1 如果有空闲的
                    if (!poolItem.poolObject.activeInHierarchy){
                        return poolItem.poolObject;
                    }
                }
                // 1.2 如果没有空闲的，新增
                return CreatePooledObject(ItemsToPool[poolName]);
            }
            // 2. 如果不存在（即：没有 Rigister 就直接 Create）不考虑这种情况
            
            
            // 如果不存在，则报错
            Debug.LogError("EZPool.Get("+poolName+") not exist!");
            return null;
        }

        /// <summary>
        /// 销毁（回收）对象
        /// </summary>
        /// <param name="gameObject"></param>
        public void Destory(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        private GameObject CreatePooledObject(EZPoolItem item)
        {
            // 设置父物体
            var parentPoolObject = GetParentPoolObject(item.poolName+"Pool");
            // 注意：一定要实例化，不能直接用原来的
            GameObject obj = Instantiate(item.poolObject, parentPoolObject.transform, true);
            obj.SetActive(false);
            
            EZPoolItem pItem = new EZPoolItem();
            pItem.poolObject = obj;
            pItem.poolName = item.poolName;
            pItem.poolAmount = item.poolAmount;
            pItem.shouldExpand = item.shouldExpand;

            if (!PooledObjects.ContainsKey(item.poolName)){
                PooledObjects.Add(item.poolName, new List<EZPoolItem>());
            }
            
            PooledObjects[item.poolName].Add(pItem);
            return obj;
        }
        
        /// <summary>
        /// 设置父物体：默认是(singleton) EZWork.EZPool。如果有指定，则添加到指定名称物体下
        /// </summary>
        /// <param name="objectPoolName"></param>
        /// <returns></returns>
        private GameObject GetParentPoolObject(string objectPoolName)
        {
            // Use the root object pool name if no name was specified
            if (string.IsNullOrEmpty(objectPoolName))
                objectPoolName = RootPoolName;

            GameObject parentObject = GameObject.Find(objectPoolName);

            // Create the parent object if necessary
            if (parentObject == null) {
                parentObject = new GameObject();
                parentObject.name = objectPoolName;

                // Add sub pools to the root object pool if necessary
                if (objectPoolName != RootPoolName)
                    parentObject.transform.parent = GameObject.Find(RootPoolName).transform;
            }

            return parentObject;
        }
    }
}