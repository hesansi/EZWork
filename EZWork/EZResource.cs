// Author: He Juncheng
// Created: 2019/03/18

using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace EZWork
{
	public enum AssetType{ Resource, AssetBundle }
	public class EZResource : EZSingletonStatic<EZResource>
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
		/// 同步加载Resource文件下资源
		/// </summary>
		/// <param name="fileName">资源名</param>
		/// <param name="isResForever">是否永远在Resource文件夹下；慎用；出包时会根据该参数决定是否加载AssetBundle</param>
		public Object LoadRes(string fileName, bool isResForever = false)
		{
			return Resources.Load(fileName);
		}
		
		/// <summary>
		/// 异步加载Resource文件下资源
		/// </summary>
		/// <param name="fileName">资源名</param>
		/// <param name="callback">加载完成回调</param>
		/// <param name="isResForever">是否永远在Resource文件夹下；慎用；出包时会根据该参数决定是否加载AssetBundle</param>
		public void LoadResAsync(string fileName, UnityAction<Object> callback, bool isResForever = false)
		{
			Debug.LogFormat("~~~~~~ LoadResAsync {0} !", fileName);
			StartCoroutine(LoadResAsyncProcess(fileName, callback));
		}

		private IEnumerator LoadResAsyncProcess(string fileName, UnityAction<Object> callback)
		{
			ResourceRequest req = Resources.LoadAsync(fileName);
			while (!req.isDone)
				yield return null;
			callback(req.asset);
		}

		/// <summary>
		/// 同步加载AssetBundle中资源：文件名与资源名相同
		/// </summary>
		/// <param name="fileName">AB文件名和资源名</param>
		public Object LoadAB(string fileName)
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
				return assetBundle.LoadAsset(fileName);
			}

			if (ab) {
				return ab.LoadAsset(fileName);
			}

			Debug.LogErrorFormat(">>>>>> Can't find {0} AssetBundle",fileName);
			return null;
		}
		
		/// <summary>
		/// 同步加载AssetBundle中资源：文件名与资源名不同
		/// </summary>
		/// <param name="fileName">AB文件名</param>
		/// <param name="assetName">具体资源名</param>
		public Object LoadAB(string fileName, string assetName)
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
				return assetBundle.LoadAsset(assetName);
			}

			if (ab) {
				return ab.LoadAsset(assetName);
			}

			Debug.LogErrorFormat(">>>>>> Can't find {0} Asset",assetName);
			return null;
		}
		
		// 2.2.1 
		/// <summary>
		/// 异步加载AssetBundle中资源：文件名与资源名相同
		/// </summary>
		/// <param name="fileName">AB文件名</param>
		/// <param name="callback">加载完成回调</param>
		public void LoadABAsync(string fileName, UnityAction<Object> callback)
		{
			fileName = fileName.ToLower();
			StartCoroutine(LoadABAsyncProcess(fileName, callback));
		}
		
		private IEnumerator LoadABAsyncProcess(string fileName, UnityAction<Object> callback)
		{
			AssetBundle ab = null;
			if (!IsABExist(fileName, ref ab)) {
				
				LoadABDependencies(fileName);
			
				AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(ABPath, fileName));
				yield return request;

				AssetBundle bundle = request.assetBundle;
				if (bundle == null) {
					Debug.LogErrorFormat(">>>>>> LoadABAsync {0} Failed!", fileName);
					yield return null;
				} else {
					yield return LoadABAssetAsync(bundle, fileName, callback);
				}
			}
			else {
				if (ab)
					yield return LoadABAssetAsync(ab, fileName, callback);
				else {
					Debug.LogErrorFormat(">>>>>> Can't find {0} AssetBundle",fileName);
				}
				
			}
		}
		
		/// <summary>
		/// 异步加载AssetBundle中资源：文件名与资源名不同
		/// </summary>
		/// <param name="fileName">AB文件名</param>
		/// <param name="assetName">具体资源名</param>
		/// <param name="callback">加载完成回调</param>
		public void LoadABAsync(string fileName, string assetName, UnityAction<Object> callback)
		{
			fileName = fileName.ToLower();
			assetName = assetName.ToLower();
			StartCoroutine(LoadABAsyncProcess(fileName, assetName, callback));
		}
		
		private IEnumerator LoadABAsyncProcess(string fileName, string assetName, UnityAction<Object> callback)
		{
			AssetBundle ab = null;
			if (!IsABExist(fileName, ref ab)) {
				
				LoadABDependencies(fileName);
			
				AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(ABPath, fileName));
				yield return request;

				AssetBundle bundle = request.assetBundle;
				if (bundle == null) {
					Debug.LogErrorFormat(">>>>>> LoadABAsync {0} Failed!", fileName);
					yield return null;
				} else {
					yield return LoadABAssetAsync(bundle, assetName, callback);
				}
			}
			else {
				if (ab)
					yield return LoadABAssetAsync(ab, assetName, callback);
				else {
					Debug.LogErrorFormat(">>>>>> Can't find {0} AssetBundle",assetName);
				}
				
			}
		}

		private IEnumerator LoadABAssetAsync(AssetBundle bundle, string assetName, UnityAction<Object> callback)
		{
			AssetBundleRequest req = bundle.LoadAssetAsync(assetName);
			while (!req.isDone)
				yield return null;
			if (req.asset) {
				callback(req.asset);
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


