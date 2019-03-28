// Author: He Juncheng
// Created: 2019/03/18

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EZWork
{
    public enum EZLoadingType {None, Loading1, Loading2}
    public class EZScene : EZSingleton<EZScene>
    {
        private EZScene() { }
        public static Stack<string> PrevSceneStack, NextSceneStack;
        private EZSceneLoader _sceneLoader;

        public void Load(string nextSceneName, EZLoadingType loadingType = EZLoadingType.Loading1)
        {
            RefreshStack(nextSceneName);

            if (GetComponent<NormalSceneLoader>() == null) 
                _sceneLoader = gameObject.AddComponent<NormalSceneLoader>();
            _sceneLoader.Load(loadingType);
        }

        public void Push(string nextSceneName, EZLoadingType loadingType = EZLoadingType.Loading1)
        {
            RefreshStack(nextSceneName);
            
            if (GetComponent<PushSceneLoader>() == null) 
                _sceneLoader = gameObject.AddComponent<PushSceneLoader>();
            _sceneLoader.Load(loadingType);
        }
        
        public void Pop(EZLoadingType loadingType = EZLoadingType.Loading1)
        {
            if (GetComponent<PopSceneLoader>() == null) 
                _sceneLoader = gameObject.AddComponent<PopSceneLoader>();
            _sceneLoader.Load(loadingType);
        }

        private void RefreshStack(string nextSceneName)
        {
            if (PrevSceneStack == null) {
                PrevSceneStack = new Stack<string>();
            }
            if (NextSceneStack == null) {
                NextSceneStack = new Stack<string>();
            }
            PrevSceneStack.Push(SceneManager.GetActiveScene().name);
            NextSceneStack.Push(nextSceneName);
        }
    }
    
    class SceneFactory
    {
        public static Dictionary<EZLoadingType, string> SceneTypeNameDic = new Dictionary<EZLoadingType, string>
        {
            {EZLoadingType.Loading1, "LoadingScene1"},
            {EZLoadingType.Loading2, "LoadingScene2"}
        };
        
        public static string GetLoadingSceneNameByType(EZLoadingType loadingType)
        {
            if (SceneTypeNameDic.ContainsKey(loadingType)) {
                return SceneTypeNameDic[loadingType];
            }

            Debug.LogErrorFormat("Can't find {0} SceneLoadingType!", nameof(loadingType));
            return "NullSceneLoading";
        }
        
        public static EZLoadingType GetLoadingTypeBySceneName(string loadingSceneName)
        {
            foreach (KeyValuePair<EZLoadingType, string> kvp in SceneTypeNameDic)
            {
                if (kvp.Value.Equals(loadingSceneName)) {
                    return kvp.Key;
                }
            }
            Debug.LogErrorFormat("Can't find {0} SceneLoadingName!", nameof(loadingSceneName));
            return EZLoadingType.None;
        }
    }
    
}

