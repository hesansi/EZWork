// Author: He Juncheng
// Created: 2019/03/18

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;
using tabtoy;
using UnityEngine.Events;
//using ConfigData;
namespace EZWork
{
	public class EZConfig : EZSingletonStatic<EZConfig>
	{
		protected EZConfig() { }
		// 可用于制作进度条
		private int curIndex = 0;
		private int totalCount;
		private readonly List<object> configList = new List<object>();
        private string BytesPath;
        void Awake()
        {
            BytesPath = Application.streamingAssetsPath + "/Bytes/";
            Debug.LogError(BytesPath);
        }
		public T GetConfig<T>() where T : class
		{
			return configList.Find(c => c.GetType() == typeof(T)) as T;
		}

        public void LoadBytes(UnityAction finish = null)
        {
            if(Directory.Exists(BytesPath))
            {
                LoadExcelBytes(finish);
            }
            else
            {
                LoadAsync(finish);
            }
        }
		// 1. 异步加载：推荐使用，游戏启动时调用
		// 注意：本方法应该"仅"在游戏启动时调用一次；本方法没有做查重处理
		public void LoadAsync(UnityAction finish = null)
		{
			// 1.1 遍历读取配置文件
			totalCount = ConfigCollection.ConfigClassType.Length;   
			foreach (Type byteClassType in ConfigCollection.ConfigClassType) {
				Debug.Log(">>>>>> Loading NO." + curIndex + " ConfigSource");
				// 注意：类名和配置文件名需要相同
				string fileName = byteClassType.Name;
				// 异步加载 AssetBundle
				EZResource.Instance.LoadResAsync<TextAsset>(fileName, asset=>
				{
					curIndex++;
					LoadAsyncAutoDeserialize(fileName, asset.bytes, byteClassType, finish);
				});
			}
		}

		// 1.2 使用stream读取资源，利用反射调用各自的反序列化方法
		// 返回值：资源是否加载成功
		private bool LoadAsyncAutoDeserialize(string fileName, byte[] bytes, Type byteClassType, UnityAction finish = null)
		{
//			TextAsset t = EZResource.Instance.asset as TextAsset;
			if (bytes == null) {
				Debug.LogError(">>>>>> TextAsset t == null");
				if(finish != null && curIndex == totalCount)
					finish();
				return false;
			}
			using (MemoryStream stream = new MemoryStream(bytes)) {
				stream.Position = 0;
				var reader = new DataReader(stream, stream.Length);
				
				// 实例化
				var data = Activator.CreateInstance(byteClassType);
				
				// 调用构造函数，并传入参数
				var mInfos = byteClassType.GetMethods();
				foreach (MethodInfo info in mInfos) {
					
					if (info.Name == "GetBuildID") {
						var buildId = info.Invoke(data, null);
						var result = reader.ReadHeader(buildId.ToString());
						if (result != FileState.OK) {
							Debug.LogErrorFormat(">>>>>> {0}, combine file crack! FileState is {1}", fileName, result);
							if(finish != null && curIndex == totalCount)
								finish();
							return false;
						}
					}
					
					if (info.Name == "Deserialize") {
						bool found = false;
						ParameterInfo[] pars = info.GetParameters();
						foreach (var para in pars) {
							if (para.ParameterType == byteClassType) {
								info.Invoke(data, new [] { data, reader });
								found = true;
								break;
							}
						}
						if (found)
							break;
					}
					
					
				}

				Debug.LogFormat(">>>>>> Parse {0} success!", fileName);
				configList.Add(data);
			}
			if(finish != null && curIndex == totalCount)
				finish();
			return true;
		}

		// 2. 同步加载：不推荐使用，游戏启动时调用
		public void Load()
		{
			foreach (Type byteClassType in ConfigCollection.ConfigClassType) {
				string fileName = byteClassType.Name;
				// 同步加载
				TextAsset t = EZResource.Instance.LoadAB(fileName) as TextAsset;
				if (t == null) {
					Debug.LogError(">>>>>> TextAsset t == null");
					return;
				}
				using (MemoryStream stream = new MemoryStream(t.bytes)) {
					stream.Position = 0;
					var reader = new DataReader(stream, stream.Length);

					var data = Activator.CreateInstance(byteClassType);
					// 调用自己的构造函数，并传入参数
					var mInfos = byteClassType.GetMethods();
					foreach (MethodInfo info in mInfos) {
											
						if (info.Name == "GetBuildID") {
							var buildId = info.Invoke(data, null);
							var result = reader.ReadHeader(buildId.ToString());
							if (result != FileState.OK) {
								Debug.LogErrorFormat(">>>>>> {0}, combine file crack! FileState is {1}", fileName, result);
								return;
							}
						}
						
						if (info.Name == "Deserialize") {
							bool found = false;
							ParameterInfo[] pars = info.GetParameters();
							foreach (var para in pars) {
								if (para.ParameterType == byteClassType) {
									info.Invoke(data, new [] { data, reader });
									found = true;
									break;
								}
							}
							if (found)
								break;
						}
					}

					Debug.LogFormat(">>>>>> parse {0} successfully!", fileName);
					configList.Add(data);
				}
			}
		}

		// 使用示例
//		void Example()
//		{
			//1. 游戏开始时，下载完 assetbundle 后，加载
			//ConfigManager.Instance.Load();

			//2. 应用层获取配置数据
			//I18nConfig cfg = ConfigManager.Instance.GetConfig<I18nConfig>();
			//Debug.Log(">>>>>> config: " + cfg.I18n.Count);
//		}
        //外部文件形式
        public void LoadExcelBytes(UnityAction finish)
        {
            foreach (Type byteClassType in ConfigCollection.ConfigClassType) {
                string filePath = BytesPath + byteClassType.Name + ".bytes";
                LoadAsyncAutoDeserialize(filePath,ReadExcelByte(filePath),byteClassType);
			}
            if(finish != null)
            {
                finish();
            }
        }
        private byte[] ReadExcelByte(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        　　try
        　　{
        　　　　byte[] buffur = new byte[fs.Length];
        　　　　fs.Read(buffur, 0, (int)fs.Length);

        　　　　return buffur;
        　　}
        　　catch (Exception ex)
        　　{
        　　　　Debug.LogError(ex.Message);
        　　　　return null;
        　　}
        　　finally
        　　{
        　　　　if (fs != null)
        　　　　{
        　　　　　　//关闭资源
        　　　　　　fs.Close();
        　　　　}
        　　}   
        }
	}
}