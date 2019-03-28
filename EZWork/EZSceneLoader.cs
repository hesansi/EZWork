// Author: He Juncheng
// Created: 2019/03/18

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EZWork
{
    public abstract class EZSceneLoader : MonoBehaviour
    {
        protected string _loadingName;
        protected EZLoadingView loadingView;
        protected AsyncOperation asyncOperation;

        public void Load(EZLoadingType loadingType)
        {
            StartCoroutine(InitLoadingView(loadingType));
        }

        private IEnumerator InitLoadingView(EZLoadingType loadingType)
        {
            // 1. 加载Loading场景
            _loadingName = SceneFactory.GetLoadingSceneNameByType(loadingType);
            yield return SceneManager.LoadSceneAsync(_loadingName, LoadSceneMode.Additive);
            var loadingScene = SceneManager.GetSceneByName(_loadingName);
            // 2. Loading场景初始化；首先要把EZScene对象移到LoadingScene，因为不能跨场景引用
            SceneManager.MoveGameObjectToScene(gameObject, loadingScene);
            loadingView = FindObjectOfType<EZLoadingView>();
            // 防止穿帮：先初始化Loading，初始化完成后再移除前一个场景
            loadingView.StartProgress(ProcessPrevScene);
        }

        // 衔接 UnloadPrevScene() 和 ProcessNextScene() 
        private void ProcessPrevScene()
        {
            StartCoroutine(UnloadPrevScene(ProcessNextScene));
        }

        // 3.移除上一个场景
        /// <summary>
        /// - 作用：移除上一个场景（如果不是Push，将自动激活Loading场景）
        /// - 调用方式：被动调用
        /// </summary>
        /// <param name="finish">个性化处理完毕后，务必调用</param>
        protected abstract IEnumerator UnloadPrevScene(UnityAction finish);

        // 衔接 LoadNextScene() 和 ProcessLoadingView() 
        private void ProcessNextScene()
        {
            StartCoroutine(LoadNextScene(ProcessLoadingView));
        }

        // 4.加载下一个场景
        /// <summary>
        /// - 作用：加载下一个场景
        /// - 调用方式：被动调用
        /// </summary>
        /// <param name="finish">个性化处理完毕后，务必调用</param>
        protected abstract IEnumerator LoadNextScene(UnityAction finish);

        // loadingView 剩余进度条
        // 衔接 loadingView.EndProgress() 和 UnloadLoadingScene() 
        private void ProcessLoadingView()
        {
            loadingView.EndProgress(() => StartCoroutine(UnloadLoadingScene()));
        }

        // 5.移除Loading场景
        /// <summary>
        /// - 作用：在进度条到头后，移除Loading场景
        /// - 调用方式：被动调用
        /// </summary>
        protected abstract IEnumerator UnloadLoadingScene();
    }
}