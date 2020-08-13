using System;
using System.Reflection;
using UnityEngine;

namespace EZWork
{
    /// 注意：子类必须实现一个"私有构造函数"
    public class EZSingleton<T> where T : class
    {
        private static T _instance;
        private static readonly object objlock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null) {
                    lock (objlock) {
                        if (_instance == null) {
                            Type type = typeof(T);
                            // 这里确保没有其它的public构造函数了，没有可以通过其它方法new这个类
                            ConstructorInfo[] ctors = type.GetConstructors();
                            if (ctors.Length > 0)
                            {
                                throw new InvalidOperationException(String.Format("{0} has at least Constructors", type.Name));
                            }
                            _instance = (T)Activator.CreateInstance(type, true);
                        }
                    }
                }

                return _instance;
            }
        }
    }
    
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class EZSingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock) {
                    if (_instance != null) return _instance;
                    
                    // 先在场景中找寻
                    _instance = (T) FindObjectOfType(typeof(T));
                    if (FindObjectsOfType(typeof(T)).Length > 1) {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton!" +
                                       " Reopening the scene might fix it.");
                        return _instance;
                    }
                    
                    // 场景中找不到就创建新物体挂载
                    if (_instance == null) {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                                  " is needed in the scene, so '" + singleton +
                                  "' was created.");
                    }
                    else {
                        Debug.Log("[Singleton] Using instance already created: " +
                                  _instance.gameObject.name);
                    }

                    return _instance;
                }
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
    
    /// 与 EZSingletonMono 实现完全一致，已废弃。为了兼容老版本而存在
    public class EZSingletonStatic<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock) {
                    if (_instance != null) return _instance;
                    
                    // 先在场景中找寻
                    _instance = (T) FindObjectOfType(typeof(T));
                    if (FindObjectsOfType(typeof(T)).Length > 1) {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton!" +
                                       " Reopening the scene might fix it.");
                        return _instance;
                    }
                    
                    // 场景中找不到就创建新物体挂载
                    if (_instance == null) {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                                  " is needed in the scene, so '" + singleton +
                                  "' was created.");
                    }
                    else {
                        Debug.Log("[Singleton] Using instance already created: " +
                                  _instance.gameObject.name);
                    }

                    return _instance;
                }
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}