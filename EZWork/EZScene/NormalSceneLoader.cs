// Author: He Juncheng
// Created: 2019/03/18

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EZWork
{
    public class NormalSceneLoader : EZSceneLoader
    {
        protected override IEnumerator UnloadPrevScene(UnityAction finish)
        {
            string preSceneName = EZScene.PrevSceneStack.Pop();
            yield return SceneManager.UnloadSceneAsync(preSceneName);
            finish();
        }
        
        protected override IEnumerator LoadNextScene(UnityAction finish)
        {
            asyncOperation = SceneManager.LoadSceneAsync(EZScene.NextSceneStack.Peek(), LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            while (asyncOperation.progress < 0.9f) {
                yield return null;
            }
            asyncOperation.allowSceneActivation = true;
            yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(EZScene.NextSceneStack.Pop()));
            finish();
        }
        
        protected override IEnumerator UnloadLoadingScene()
        {
            // 移除Loading场景
            yield return SceneManager.UnloadSceneAsync(_loadingName);
        }
        
    }
}