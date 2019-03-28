// Author: He Juncheng
// Created: 2019/03/18

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EZWork
{
    public class PopSceneLoader : EZSceneLoader
    {
        protected override IEnumerator UnloadPrevScene(UnityAction finish)
        { 
            // 移除当前场景（移除后，还剩Loading和前一个场景）
            yield return SceneManager.UnloadSceneAsync(EZScene.NextSceneStack.Pop());
            finish();
        }
        
        protected override IEnumerator LoadNextScene(UnityAction finish)
        {
            Scene preScene = SceneManager.GetSceneByName(EZScene.PrevSceneStack.Pop());
            Debug.Log(">>>>>> Pop prevButCurrentScene.name: "+preScene.name);
            // 显示前一个场景
            foreach (var go in preScene.GetRootGameObjects()) {
                go.SetActive(true);
            }
            finish();
            yield return null;
        }
        
        protected override IEnumerator UnloadLoadingScene()
        {
            // 移除Loading场景，自动激活当前场景;
            yield return SceneManager.UnloadSceneAsync(_loadingName);
        }
    }

}


