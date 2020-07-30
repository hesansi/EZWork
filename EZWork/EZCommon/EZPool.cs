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

    public class EZPool : EZSingleton<EZPool>
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
        [SerializeField]
        // 所有缓存对象列表
        private List<EZPoolItem> pooledObjects;
        private List<EZPoolItem> PooledObjects
        {
            get
            {
                if (pooledObjects == null) {
                    pooledObjects = new List<EZPoolItem>();
                }
                return pooledObjects;
            }
            set => pooledObjects = value;
        }
        [SerializeField]
        // 不同类型或缓存池对象列表
        private List<EZPoolItem> itemsToPool;
        private List<EZPoolItem> ItemsToPool
        {
            get
            {
                if (itemsToPool == null) {
                    itemsToPool = new List<EZPoolItem>();
                }
                return itemsToPool;
            }
            set => itemsToPool = value;
        }

        /// <summary>
        /// 注册缓存池
        /// </summary>
        /// <param name="poolName">缓存池名称</param>
        /// <param name="poolObject">被缓存的对象</param>
        /// <param name="poolAmount">初始化缓存数量</param>
        /// <param name="shouldExpand">是否自动扩展</param>
        public void Regist(string poolName, GameObject poolObject, int poolAmount = 2, bool shouldExpand = true)
        {
            foreach (EZPoolItem item in ItemsToPool) {
                if (item.poolName.Equals(poolName)) {
                    return;
                }
            }
            EZPoolItem pItem = new EZPoolItem();
            pItem.poolObject = poolObject;
            pItem.poolName = poolName;
            pItem.poolAmount = poolAmount;
            pItem.shouldExpand = shouldExpand;
            ItemsToPool.Add(pItem);

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
            // 如果已存在且够用，则直接返回
            for (int i = 0; i < PooledObjects.Count; i++) {
                if (!PooledObjects[i].poolObject.activeInHierarchy && PooledObjects[i].poolName.Equals(poolName))
                    return PooledObjects[i].poolObject;
            }
            // 如果不够用,则新建
            foreach (EZPoolItem item in ItemsToPool) {
                if (item.poolName.Equals(poolName)) {
                    return CreatePooledObject(item);
                }
            }
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
            GameObject obj = Instantiate<GameObject>(item.poolObject);

            // Get the parent for this pooled object and assign the new object to it
            var parentPoolObject = GetParentPoolObject(item.poolName+"Pool");
            obj.transform.parent = parentPoolObject.transform;
            obj.SetActive(false);
            
            EZPoolItem pItem = new EZPoolItem();
            pItem.poolObject = obj;
            pItem.poolName = item.poolName;
            pItem.poolAmount = item.poolAmount;
            pItem.shouldExpand = item.shouldExpand;
            
            PooledObjects.Add(pItem);
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