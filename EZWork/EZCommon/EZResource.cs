// Author: He Juncheng
// Created: 2019/03/18

using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using System.Collections.Generic;
namespace EZWork
{
	public enum AssetType{ Resource, AssetBundle }
    public enum EZAssetBundleName
    {
        XlsxBytes,
        ItemIcon,
        UIPrefab,
    }
	public class EZResource : EZSingletonMono<EZResource>
	{
		protected EZResource() { }
		//不同平台下StreamingAssets的路径不同
		private string ABPath;
		private string StreamingManifest;
		private AssetBundleManifest manifest;

		private void Awake()
		{
			ABPath =
#if UNITY_ANDROID
        "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
        Application.dataPath + "/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
				Application.dataPath + "/StreamingAssets/";
#else
        string.Empty;
#endif
			StreamingManifest = "StreamingAssets";
		}

		/// <summary>
		/// Resource 同步 加载物体对象
		/// </summary>
		/// <param name="fileName">资源名</param>
		/// <param name="isResForever">是否永远在Resource文件夹下；慎用；出包时会根据该参数决定是否加载AssetBundle</param>
		public Object LoadRes(string fileName, bool isResForever = false)
		{
			return LoadRes<Object>(fileName, isResForever);
		}
		
		/// <summary>
		/// Resource 同步 使用泛型加载Spite、Audio等非对象资源
		/// </summary>
		public T LoadRes<T>(string fileName, bool isResForever = false) where T:Object
		{
			return Resources.Load<T>(fileName);
		}
		
		/// <summary>
		/// Resource 异步 加载物体对象
		/// </summary>
		/// <param name="fileName">资源名</param>
		/// <param name="callback">加载完成回调</param>
		/// <param name="isResForever">是否永远在Resource文件夹下；慎用；出包时会根据该参数决定是否加载AssetBundle</param>
		public void LoadResAsync(string fileName, UnityAction<Object> callback, bool isResForever = false)
		{
			StartCoroutine(LoadResAsyncProcess<Object>(fileName, callback));
		}

		/// <summary>
		/// Resource 异步 使用泛型加载Spite、Audio等非对象资源
		/// </summary>
		public void LoadResAsync<T>(string fileName, UnityAction<T> callback, bool isResForever = false) where T:Object
		{
			Debug.LogFormat("~~~~~~ LoadResAsync {0} !", fileName);
			StartCoroutine(LoadResAsyncProcess<T>(fileName, callback));
		}

		private IEnumerator LoadResAsyncProcess<T>(string fileName, UnityAction<T> callback) where T:Object
		{
			ResourceRequest req = Resources.LoadAsync<T>(fileName);
			while (!req.isDone)
				yield return null;
			callback(req.asset as T);
		}

		/// <summary>
		/// AB 同步：文件名与资源名相同
		/// </summary>
		/// <param name="fileName">AB文件名和资源名</param>
		public Object LoadAB(string fileName)
		{
			return LoadAB<Object>(fileName, fileName);
		}
		
		/// <summary>
		/// AB 同步 使用泛型加载Spite、Audio等非对象资源：文件名与资源名相同
		/// </summary>
		public T LoadAB<T>(string fileName) where T:Object
		{
			return LoadAB<T>(fileName, fileName);
		}
		
		/// <summary>
		///  AB 同步：文件名与资源名不同
		/// </summary>
		/// <param name="fileName">AB文件名</param>
		/// <param name="assetName">具体资源名</param>
		public Object LoadAB(string fileName, string assetName)
		{
			return LoadAB<Object>(fileName, assetName);
		}
		
		/// <summary>
		/// AB 同步 使用泛型加载Spite、Audio等非对象资源：文件名与资源名不同
		/// </summary>
		public T LoadAB<T>(string fileName, string assetName) where T:Object
		{
			fileName = fileName.ToLower();
			AssetBundle ab = null;
			if (!IsABExist(fileName, ref ab)) {
				LoadABDependencies(fileName);
			
				var assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(ABPath, fileName));
				if (assetBundle == null) {
					Debug.LogFormat(">>>>>> LoadAB {0} Failed!", fileName);
					return null;
				}
				return assetBundle.LoadAsset<T>(assetName);
			}

			if (ab) {
				return ab.LoadAsset<T>(assetName);
			}

			Debug.LogErrorFormat(">>>>>> Can't find {0} Asset",assetName);
			return null;
		}
		
		// 2.2.1 
		/// <summary>
		/// AB 异步：文件名与资源名相同
		/// </summary>
		/// <param name="fileName">AB文件名</param>
		/// <param name="callback">加载完成回调</param>
		public void LoadABAsync(string fileName, UnityAction<Object> callback)
		{
			fileName = fileName.ToLower();
			StartCoroutine(LoadABAsyncProcess<Object>(fileName, fileName, callback));
		}
		
		/// <summary>
		/// AB 异步 使用泛型加载Spite、Audio等非对象资源：文件名与资源名相同
		/// </summary>
		public void LoadABAsync<T>(string fileName, UnityAction<T> callback) where T:Object
		{
			fileName = fileName.ToLower();
			StartCoroutine(LoadABAsyncProcess<T>(fileName, fileName, callback));
		}
		
		/// <summary>
		/// AB 异步：文件名与资源名不同
		/// </summary>
		/// <param name="fileName">AB文件名</param>
		/// <param name="assetName">具体资源名</param>
		/// <param name="callback">加载完成回调</param>
		public void LoadABAsync(string fileName, string assetName, UnityAction<Object> callback)
		{
			fileName = fileName.ToLower();
			assetName = assetName.ToLower();
			StartCoroutine(LoadABAsyncProcess<Object>(fileName, assetName, callback));
		}
		
		/// <summary>
		/// AB 异步 使用泛型加载Spite、Audio等非对象资源：文件名与资源名不同
		/// </summary>
		public void LoadABAsync<T>(string fileName, string assetName, UnityAction<T> callback) where T:Object
		{
			fileName = fileName.ToLower();
			assetName = assetName.ToLower();
			StartCoroutine(LoadABAsyncProcess<T>(fileName, assetName, callback));
		}
		private Dictionary<string,AssetBundleCreateRequest> AssetBundleCreateRequestDictionary = new Dictionary<string,AssetBundleCreateRequest>();
		private IEnumerator LoadABAsyncProcess<T>(string fileName, string assetName, UnityAction<T> callback) where T:Object
		{
            if(AssetBundleCreateRequestDictionary.ContainsKey(fileName))
            {
                while(AssetBundleCreateRequestDictionary[fileName] == null)
                {
                    yield return null;
                }
            }
            else
            {
                AssetBundleCreateRequestDictionary.Add(fileName,null);
                LoadABDependencies(fileName);
				AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(ABPath, fileName));
				yield return request;
                AssetBundleCreateRequestDictionary[fileName] = request;
            }
            AssetBundle bundle = AssetBundleCreateRequestDictionary[fileName].assetBundle;
            if (bundle == null) {
                Debug.LogErrorFormat(">>>>>> LoadABAsync {0} Failed!", fileName);
                yield return null;
            } else {
                yield return LoadABAssetAsync<T>(bundle, assetName, callback);
            }
		}
		
		private IEnumerator LoadABAssetAsync<T>(AssetBundle bundle, string assetName, UnityAction<T> callback) where T:Object
		{
			AssetBundleRequest req = bundle.LoadAssetAsync<T>(assetName);
			while (!req.isDone)
				yield return null;
			if (req.asset) {
				callback(req.asset as T);
			}
			else {
				Debug.LogErrorFormat(">>>>>> Can't find {0} Asset",assetName);
			}
			
		}

		// 3. AssetBundle 依赖处理
		// 加载 Manifest
		private void LoadABManifest()
		{
			Debug.Log("### LoadManifest()");
			AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(ABPath, StreamingManifest));
			manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
			assetBundle.Unload(false);
		}

		// 查重
		private bool IsABExist(string fileName, ref AssetBundle ab)
		{
			IEnumerable liveABList = AssetBundle.GetAllLoadedAssetBundles();
			foreach (AssetBundle liveAB in liveABList) {
				if (liveAB.name.Equals(fileName)) {
					ab = liveAB;
					return true;
				}
			}
			return false;
		}

		// 加载依赖
		private void LoadABDependencies(string fileName)
		{
			if (manifest == null) {
				LoadABManifest();
			}

			fileName = fileName.ToLower();
			string[] dependencies = manifest.GetAllDependencies(fileName); //Pass the name of the bundle you want the dependencies for.
		
			AssetBundle ab = null;
			foreach(string dependency in dependencies)
			{
				if (!IsABExist(dependency, ref ab)) {
					Debug.Log("### fileName: "+fileName+" dependency: "+dependency);
					AssetBundle.LoadFromFile(System.IO.Path.Combine(ABPath, dependency));
				}
			}
		}

		// 4. 释放资源
		/// <summary>
		/// Resources.UnloadUnusedAssets()
		/// </summary>
		public void UnloadUnusedAssets()
		{
			Resources.UnloadUnusedAssets();
		}

	}
	
}


