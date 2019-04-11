// Author: He Juncheng
// Created: 2019/03/18

using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace EZWork
{
    public class EZScene : EZSingleton<EZScene>
    {
        protected EZScene() { }
        public static Stack<string> PrevSceneStack, NextSceneStack;
        public static Action NextSceneActived;
        private EZSceneLoader _sceneLoader;

        /// <summary>
        /// 卸载旧场景，加载新场景
        /// </summary>
        public void Load(string nextSceneName, EZLoadingType loadingType = EZLoadingType.LoadingScene1)
        {
            ClearActions();
            ClearStack();
            RefreshStack(nextSceneName);

            if (GetComponent<NormalSceneLoader>() == null) 
                _sceneLoader = gameObject.AddComponent<NormalSceneLoader>();
            _sceneLoader.Load(loadingType);
        }

        
        public void Push(string nextSceneName, EZLoadingType loadingType = EZLoadingType.LoadingScene1)
        {
            ClearActions();
            RefreshStack(nextSceneName);
            
            if (GetComponent<PushSceneLoader>() == null) 
                _sceneLoader = gameObject.AddComponent<PushSceneLoader>();
            _sceneLoader.Load(loadingType);
        }
        
        public void Pop(EZLoadingType loadingType = EZLoadingType.LoadingScene1)
        {
            ClearActions();
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

        private void ClearStack()
        {
            if (PrevSceneStack != null) 
                PrevSceneStack.Clear();
            
            if (NextSceneStack != null) 
                NextSceneStack.Clear();
        }

        private void ClearActions()
        {
            NextSceneActived = null;
        }
    }
}

