// Author: He Juncheng
// Created: 2019/03/18

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EZWork
{
    public class PushSceneLoader : EZSceneLoader
    {
        protected override IEnumerator UnloadPrevScene(UnityAction finish)
        {
            Scene preScene = SceneManager.GetSceneByName(EZScene.PrevSceneStack.Peek());
//            EZScene.PreSceneName = preScene.name;
            Debug.Log(">>>>>> Push prevScene.name: "+EZScene.PrevSceneStack.Peek());
            // 隐藏，而不是卸载
            foreach (var go in preScene.GetRootGameObjects()) {
                go.SetActive(false);
            }
            finish();
            yield return null;
        }
        
        protected override IEnumerator LoadNextScene(UnityAction finish)
        {
            asyncOperation = SceneManager.LoadSceneAsync(EZScene.NextSceneStack.Peek(), LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            while (asyncOperation.progress < 0.9f) {
                yield return null;
            }
            asyncOperation.allowSceneActivation = true;
            finish();
        }
        
        protected override IEnumerator UnloadLoadingScene()
        {
            // 激活当前场景：这步很重要，因为当前存在三个场景，如果只是移除Loading场景，激活哪个场景不确定
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(EZScene.NextSceneStack.Peek()));
            // 移除Loading场景，自动激活当前场景;
            yield return SceneManager.UnloadSceneAsync(_loadingName);
        }
    }
}


