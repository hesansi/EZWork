- [EZWork](#ezwork)
- [EZSingleton](#ezsingleton)
  - [Examples](#examples)
- [EZResource](#ezresource)
  - [Methods](#methods)
  - [Examples](#examples-1)
- [EZScene](#ezscene)
  - [Properties](#properties)
  - [Methods](#methods-1)
  - [Examples](#examples-2)
- [EZSceneLoader](#ezsceneloader)
  - [Remarks](#remarks)
  - [NormalSceneLoader](#normalsceneloader)
  - [PushSceneLoader](#pushsceneloader)
  - [PopSceneLoader](#popsceneloader)
- [EZLoadingView](#ezloadingview)
  - [Properties](#properties-1)
  - [Examples](#examples-3)
  
# EZWork
EZWork, AKA Easy Framework for Unity.


# EZSingleton

EZWork单例类。继承该类即可实现单例。

注意：该类没有限制使用构造函数，比如"`T myT = new T();`"。为了避免存在多个实例，请自行在你的单例类中添加"`protected T () {}`"构造函数。

## Examples

```
// 继承 EZSingleton 实现单例
public class EZResource : EZSingleton<EZResource>
...
// 使用单例
EZResource.Instance.yourProperty;
EZResource.Instance.yourFunc();
```

# EZResource

EZWork资源加载类。

提供了Resouse、AssetBundle中资源的同步、异步加载方法。

方法内部处理了AssetBundle间的相互依赖。加载任何你想加载的资源，你不需要考虑AssetBundle间的依赖关系。

TODO：目前只实现了本地资源加载，服务端资源加载未来会加上，目前请自行扩展。

## Methods

|公开方法|介绍|
|:--|:--|
|`Object LoadRes(string fileName)`|同步加载Resource文件下资源|
|`void LoadResAsync(string fileName, UnityAction<Object> callback)`|异步加载Resource文件下资源|
|`Object LoadAB(string fileName)`|同步加载AssetBundle中资源，且文件与资源同名|
|`Object LoadAB(string fileName, string assetName)`|同步加载AssetBundle中资源，且文件与资源不同名|
|`void LoadABAsync(string fileName, UnityAction<Object> callback)`|异步加载AssetBundle中资源，且文件与资源同名|
|`void LoadABAsync(string fileName, string assetName, UnityAction<Object> callback)`|异步加载AssetBundle中资源，且文件与资源不同名|

## Examples

```
// 1.1 Resources 同步加载
GameObject go = Instantiate(EZResource.Instance.LoadRes("CubePrefab") as GameObject, transform, true)  ;
go.name = "Cube";

// 1.2 Resources 异步加载
EZResource.Instance.LoadResAsync("CubePrefab", asset =>
{
    GameObject go = Instantiate(asset as GameObject, transform, true)  ;
    go.name = "AsyncCube";
});

// 2.1.1 AssetBundle 同步加载，文件与资源同名
GameObject go = Instantiate(EZResource.Instance.LoadAB("PrefabDepTest0") as GameObject, transform, true);
go.name = "ABPrefabDepTest0";

// 2.1.2 AssetBundle 同步加载，文件与资源不同名
GameObject go = Instantiate(EZResource.Instance.LoadAB("CubeAndSphere", "CubePrefab") as GameObject, transform, true);
go.name = "ABCubeAndSphere_Cube";

// 2.2.1 AssetBundle 异步加载，文件与资源同名
EZResource.Instance.LoadABAsync("PrefabDepTest0", asset =>
{
    GameObject go = Instantiate(asset as GameObject, transform, true) ;
    go.name = "ABAsyncPrefabDepTest0";
});

// 2.2.2 AssetBundle 异步加载 文件与资源不同名
EZResource.Instance.LoadABAsync("CubeAndSphere", "CubePrefab", asset =>
{
    GameObject go = Instantiate(asset as GameObject, transform, true) ;
    go.name = "ABAsyncCubeAndSphere_CubePrefab";
});

```

# EZScene

EZWork场景管理类。

所有场景的加载、卸载都是异步处理，实现了"旧场景->Loading界面->新场景"整套流程，可自行定制个性化实现。

具有3个辅助类：`EZSceneLoader`、`EZSceneMapper`、`EZLoadingView`。

以及3个 EZSceneLoader 的实现类：`NormalSceneLoader`、`PushSceneLoader`、`PopSceneLoader`

以上6个类后面都会介绍。

## Properties

|公开属性|介绍|
|:--|:--|
|`Stack<string> PrevSceneStack`|旧场景名堆栈|
|`Stack<string> NextSceneStack`|新场景名堆栈|

## Methods

|公开方法|介绍|
|:--|:--|
|`void Load(string nextSceneName, EZLoadingType loadingType)`|加载新场景，卸载旧场景|
|`void Push(string nextSceneName, EZLoadingType loadingType)`|加载新场景，旧场景入栈|
|`void Pop(EZLoadingType loadingType)`|卸载当前场景，旧场景出栈|

## Examples

```
// 1. 加载SecondScene，EZLoadingType为Loading1（EZLoadingType需自行定义，后面有介绍）
EZScene.Instance.Load("SecondScene", EZLoadingType.Loading1);

// 2. 加载ThirdScene，旧场景入栈
EZScene.Instance.Push("ThirdScene", EZLoadingType.Loading1);

// 3. 卸载当前场景，旧场景出栈
EZScene.Instance.Pop(EZLoadingType.Loading2);

```

# EZSceneLoader

EZScene辅助类。衔接了"旧场景->Loading界面->新场景"整套流程。

该类是抽象类，需继承该类，并重写3个抽象方法：

|抽象方法|介绍|
|:--|:--|
|`abstract IEnumerator UnloadPrevScene(UnityAction finish)`|处理（卸载、隐藏）上一个场景|
|`abstract IEnumerator LoadNextScene(UnityAction finish)`|处理（加载、显示）下一个场景|
|`abstract IEnumerator UnloadLoadingScene()`|卸载Loading场景|

EZSceneLoader 的3个实现类：`NormalSceneLoader`、`PushSceneLoader`、`PopSceneLoader`，分别重写了各自的抽象方法，应该已经够用。如果有特殊需求，可自行继承并实现。

## Remarks

EZSceneLoader是整个场景处理流程中的一部分，抽象方法自动被依次调用，因此不需要显示调用。

## NormalSceneLoader

EZSceneLoader的实现。被动调用，不需要显示调用。

卸载旧场景，加载下一个场景。自动调用`Resources.UnloadUnusedAssets()`，卸载无用资源。

|重写方法|介绍|
|:--|:--|
|`abstract IEnumerator UnloadPrevScene(UnityAction finish)`|卸载上一个场景|
|`abstract IEnumerator LoadNextScene(UnityAction finish)`|加载下一个场景|
|`abstract IEnumerator UnloadLoadingScene()`|卸载Loading场景|

## PushSceneLoader

EZSceneLoader的实现。被动调用，不需要显示调用。

隐藏旧场景，加载下一个场景。

|重写方法|介绍|
|:--|:--|
|`abstract IEnumerator UnloadPrevScene(UnityAction finish)`|隐藏上一个场景|
|`abstract IEnumerator LoadNextScene(UnityAction finish)`|加载下一个场景|
|`abstract IEnumerator UnloadLoadingScene()`|激活新场景，卸载Loading场景|

## PopSceneLoader

EZSceneLoader的实现。被动调用，不需要显示调用。

卸载当前场景，显示旧场景。

|重写方法|介绍|
|:--|:--|
|`abstract IEnumerator UnloadPrevScene(UnityAction finish)`|卸载当前场景|
|`abstract IEnumerator LoadNextScene(UnityAction finish)`|下一个场景|
|`abstract IEnumerator UnloadLoadingScene()`|卸载Loading场景|

# EZLoadingView

EZScene辅助类。负责Loading界面的个性化处理。

## Properties

|公开属性|介绍|
|:--|:--|
|`CurProgress`|当前进度|
|`FirstProgress`|前半段进度，在此进度等待新场景加载完成|

该类是抽象类，需继承该类，并重写2个抽象方法：

|抽象方法|介绍|
|:--|:--|
|`abstract void InitView()`|初始化（进度条、进度文本或其他个性化处理）|
|`abstract void UpdateView()`|从0到100刷新进度|

## Examples

1. 新建一个类，继承EZLoadingView，抽象方法可以留空，或写个Log
2. 在你的Loading场景中，选择一个已有物体，或新建一个空物体，将上一步重写的类挂上去
3. 参考Samples文件夹下SampleLoadingView.cs，根据需求重写抽象方法

