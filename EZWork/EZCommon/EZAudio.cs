using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;
using EZWork;
using DG.Tweening;
namespace EZWork
{
	public class EZAudioSource
	{
		public string Name = "None";
		public AudioSource Audio;
		public float VolumeScale = 1.0f;
		private Action mAction;
		private EZAudioEnum AudioEnum;
		public static EZAudioSource Create(string name, float volumeScale)
		{
			GameObject gameObject = new GameObject();
			AudioSource audio = gameObject.AddComponent<AudioSource>();
			gameObject.name = name;
			audio.clip = EZResource.Instance.LoadRes<AudioClip>(name);
			//AudioSource audio = Instantiate(EZResource.Instance.LoadRes<AudioSource>(name));
			gameObject.transform.parent = GameObject.Find("(singleton) EZWork.EZResource").transform;
			EZAudioSource audioSource = new EZAudioSource();
			audioSource.Name = name;
			audioSource.Audio = audio;
			audioSource.VolumeScale = volumeScale;

			return audioSource;
		}

		public void Play(int loop, float volumeScale, EZAudioEnum audioEnum, Action action = null)
		{
			mAction = action;
			VolumeScale = volumeScale;
			AudioEnum = audioEnum;
			Audio.DOKill();
			Audio.volume = VolumeScale * (AudioEnum == EZAudioEnum.MUSIC ? EZAudio.MusicVolume : EZAudio.SoundVolume);
			Audio.loop = loop == -1;
			if(loop == 1)
			{
				Debug.Log(Audio.clip.name + Audio.clip.length);
				Audio.PlayOneShot(Audio.clip);
				EZTime.Instance.InvokeOnce(EndAction, Audio.clip.length);
			}
			else
			{
				// if(EZAudioEnum.MUSIC == audioEnum)
				// {
				//     float volume = Audio.volume;
				//     Audio.volume = 0;
				//     Audio.DOFade(volume,5).SetEase(Ease.InCirc);
				// }
				Audio.Play();
			}
		}
		private void EndAction()
		{
			if(mAction != null)
			{
				mAction();
			}
		}
		public void Pause()
		{
			Audio.Pause();
			Audio.DOKill();
			EZTime.Instance.PauseInvoke(EndAction);
		}

		public void Resume(float fadeTime = 0)
		{
			Audio.UnPause();
			float volume = VolumeScale * (AudioEnum == EZAudioEnum.MUSIC ? EZAudio.MusicVolume : EZAudio.SoundVolume);
			Audio.volume = 0;
			Audio.DOFade(volume, fadeTime).SetEase(Ease.InCirc);
			EZTime.Instance.ResumeInvoke(EndAction);
		}

		public void Stop(bool fadeout = false, float fadeoutTime = 0.5f)
		{
			if (!fadeout) {
				Audio.Stop();
				Audio.DOKill();
				EZTime.Instance.RemoveInvoke(EndAction);
			}
			else {
				Audio.DOFade(0, fadeoutTime).OnComplete(() => { 
					Audio.Stop();
					Audio.DOKill();
					EZTime.Instance.RemoveInvoke(EndAction);
				});
			}
			
		}
		public bool IsPlaying()
		{
			return Audio.isPlaying;
		}

		public void RefreshVolume(float audioEnum)
		{
			Audio.volume = audioEnum;
		}
	}

	public partial class EZAudio : MonoBehaviour
	{
		private static float soundVolume = 1f;
		/// <summary>
		/// 全局音效音量
		/// </summary>
		public static float SoundVolume
		{
			get => soundVolume;
			set
			{
				soundVolume = value;
				UpdateSoundVolume();
			}
		}
		public static string MusicName
		{
			get;
			private set;
		}
		private static float musicVolume = 1f;
		/// <summary>
		/// 全局音乐音量
		/// </summary>
		public static float MusicVolume
		{
			get => musicVolume;
			set
			{
				musicVolume = value;
				UpdateMusicVolume();
			}
		}

		private static Dictionary<string, EZAudioSource> soundDic;
		/// <summary>
		/// 音效列表
		/// </summary>
		public static Dictionary<string, EZAudioSource> SoundDic
		{
			get
			{
				if(soundDic == null)
				{
					soundDic = new Dictionary<string, EZAudioSource>();
				}
				return soundDic;
			}
			set => soundDic = value;
		}

		private static Dictionary<string, EZAudioSource> musicDic;
		/// <summary>
		/// 音乐列表
		/// </summary>
		public static Dictionary<string, EZAudioSource> MusicDic
		{
			get
			{
				if(musicDic == null)
				{
					musicDic = new Dictionary<string, EZAudioSource>();
				}
				return musicDic;
			}
			set => musicDic = value;
		}

		/// BGM专用（BGM一般同时只存在一个）
		static private string bgmAudioSourceName;

		/// <summary>
		/// 播放音效一次
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		public static void PlayOnShortSound(string name, float volume = 1.0f)
		{
			Play(name, 1, EZAudioEnum.SOUND, volume);
		}

        
        /// <summary>
		/// 循环播放音效
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		/// <param name="volume">音量（范围：0~1）</param>
		/// <param name="fadeTime">淡入淡出时间</param>
		public static void PlayLoopingSound(string name, float volume = 1.0f)
		{
			Play(name, -1, EZAudioEnum.SOUND, volume);
		}

		/// <summary>
		/// 播放音乐一次
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		/// <param name="volume">音量（范围：0~1）</param>
		public static void PlayOnShortMusic(string name, float volume = 1.0f)
		{
			Play(name, 1, EZAudioEnum.MUSIC, volume);
		}

		/// <summary>
		/// 循环播放音乐
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		/// <param name="volume">音量（范围：0~1）</param>
		/// <param name="fadeTime">淡入淡出时间</param>
		/// <param name="persist">在场景加载时是否保持播放</param>
		public static void PlayLoopingMusic(string name, float volume = 1.0f)
		{
			Play(name, -1, EZAudioEnum.MUSIC, volume);
		}

		/// <summary>
		/// 暂停某个音效
		/// </summary>
		public static void PauseSound(string name)
		{
			try
			{
				SoundDic[name].Pause();
			}
			catch
			{
				Debug.Log("=== === SoundDic Pause dosen't contain " + name);
			}
		}

		/// <summary>
		/// 暂停某个音乐
		/// </summary>
		public static void PauseMusic(string name)
		{
			try
			{
				MusicDic[name].Pause();
			}
			catch
			{
				Debug.Log("=== === MusicDic Pause dosen't contain " + name);
			}
		}

		/// <summary>
		/// 暂停所有音效
		/// </summary>
		public static void PauseAllSounds()
		{
			foreach(var audioSource in SoundDic.Values)
			{
				audioSource.Pause();
			}
		}

		/// <summary>
		/// 暂停所有音乐
		/// </summary>
		public static void PauseAllMusics()
		{
			foreach(var audioSource in MusicDic.Values)
			{
				audioSource.Pause();
			}
		}

		/// <summary>
		/// 暂停所有音效、音乐
		/// </summary>
		public static void PauseAll()
		{
			PauseAllSounds();
			PauseAllMusics();
		}

		/// <summary>
		/// 恢复某个音效
		/// </summary>
		public static void ResumeSound(string name)
		{
			try
			{
				SoundDic[name].Resume();
			}
			catch
			{
				UnityEngine.Debug.Log("=== === SoundDic Resume dosen't contain " + name);
			}
		}

		/// <summary>
		/// 恢复某个音乐
		/// </summary>
		public static void ResumeMusic(string name, float fadeTime = 0)
		{
			try
			{
				MusicDic[name].Resume();
			}
			catch
			{
				UnityEngine.Debug.Log("=== === MusicDic Resume dosen't contain " + name);
			}
		}

		/// <summary>
		/// 恢复所有音效；注意：OneShort无法恢复
		/// </summary>
		public static void ResumeAllSounds()
		{
			foreach(var audioSource in SoundDic.Values)
			{
				audioSource.Resume();
			}
		}

		/// <summary>
		/// 恢复所有音乐；注意：OneShort无法恢复
		/// </summary>
		public static void ResumeAllMusics()
		{
			foreach(var audioSource in MusicDic.Values)
			{
				audioSource.Resume();
			}
		}

		/// <summary>
		/// 音效是否在播放
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		public static bool IsSoundPlaying(string name)
		{
			return SoundDic[name].IsPlaying();
		}

		/// <summary>
		/// 音乐是否在播放
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		public static bool IsMusicPlaying(string name)
		{
			return MusicDic[name].IsPlaying();
		}

		/// <summary>
		/// 恢复所有音效、音乐；注意：OneShort无法恢复
		/// </summary>
		public static void ResumeAll()
		{
			ResumeAllSounds();
			ResumeAllMusics();
		}

		/// <summary>
		/// 停止播放某个音效
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		public static void StopSound(string name)
		{
			try
			{
				SoundDic[name].Stop();
			}
			catch
			{
				UnityEngine.Debug.Log("=== === SoundDic Stop dosen't contain " + name);
			}
		}

		/// <summary>
		/// 停止播放某个音乐
		/// </summary>
		/// <param name="name"></param>
		public static void StopMusic(string name, bool fadeout = false, float fadeoutTime = 0.5f)
		{
			try
			{
				MusicDic[name].Stop(fadeout, fadeoutTime);
				// 从缓存中移除音频资源
				if (MusicDic.ContainsKey(name)) {
					MusicDic.Remove(name);
				}
			}
			catch
			{
				UnityEngine.Debug.Log("=== === MusicDic Stop dosen't contain " + name);
			}
		}

		/// <summary>
		/// 停止播放所有音效
		/// </summary>
		public static void StopAllSounds()
		{
			foreach(var audioSource in SoundDic.Values)
			{
				audioSource.Stop();
			}
		}

		/// <summary>
		/// 停止播放所有循环的音乐
		/// </summary>
		public static void StopAllMusics(bool fadeout = false, float fadeoutTime = 0.5f)
		{
			foreach(var audioSource in MusicDic.Values)
			{
				audioSource.Stop(fadeout, fadeoutTime);
			}
			MusicDic.Clear();
			
		}

		/// <summary>
		/// 停止所有音效、音乐
		/// </summary>
		public static void StopAll()
		{
			StopAllSounds();
			StopAllMusics();
		}

		/// <summary>
		/// 播放音频统一接口，含：音效、音乐
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		/// <param name="loop">循环次数</param>
		/// <param name="ezAudioEnum">音频类型</param>
		/// <param name="volumeScale">音量（范围：0~1）</param>
		/// <param name="fadeTime">淡入淡出时间</param>
		/// <param name="persist">在场景加载时是否保持播放</param>
		public static void Play(string name, int loop, EZAudioEnum ezAudioEnum, float volumeScale)
		{
			switch(ezAudioEnum)
			{
				case EZAudioEnum.SOUND:
					if(!SoundDic.ContainsKey(name))
					{
						SoundDic.Add(name, EZAudioSource.Create(name, volumeScale));
					}
					SoundDic[name].Play(loop, volumeScale, ezAudioEnum);
					break;
				case EZAudioEnum.MUSIC:
					{
						if(!MusicDic.ContainsKey(name))
						{
							MusicDic.Add(name, EZAudioSource.Create(name, volumeScale));
						}
						MusicDic[name].Play(loop, volumeScale, ezAudioEnum);
					}
					break;
			}
		}

		/// <summary>
		/// 循环播放BGM
		/// </summary>
		/// <param name="name">BGM音频名称</param>
		static public void PlayBGM(string name, int loopTimes = -1)
		{
			// 如果已存在相同名称BGM在播放，则不做处理
			if(name.Equals(bgmAudioSourceName))
			{
				return;
			}
			// 停止前一个BGM
			StopBGM();
			PlayBGMProcess(name, loopTimes: loopTimes);
		}

		/// <summary>
		/// 停止播放BGM
		/// </summary>
		/// <param name="name"></param>
		static public void StopBGM(bool fadeout = false, float fadeoutTime = 0.5f)
		{
			try
			{
				MusicDic[bgmAudioSourceName].Stop(fadeout, fadeoutTime);
				bgmAudioSourceName = "";
				// 从缓存中移除音频资源
				if (MusicDic.ContainsKey(bgmAudioSourceName)) {
					MusicDic.Remove(bgmAudioSourceName);
				}
			}
			catch
			{
				UnityEngine.Debug.Log("=== === MusicDic Stop dosen't contain " + bgmAudioSourceName);
			}
		}

		/// <summary>
		/// 暂停BGM
		/// </summary>
		static public void PauseBGM()
		{
			PauseMusic(bgmAudioSourceName);
		}

		/// <summary>
		/// 恢复BGM
		/// </summary>
		static public void ResumeBGM()
		{
			ResumeMusic(bgmAudioSourceName);
		}

		/// <summary>
		/// 循环播放BGM，与PlayLoopingMusic类似（如果存在，则播放；如果不存在，则加载、缓存并播放）
		/// </summary>
		/// <param name="name">AudioSource预设名</param>
		/// <param name="audioEnum">音频类型（关系到加载路径）</param>
		/// <param name="volumeScale">在全局音量基础上的音量系数（范围：0~1）</param>
		/// <param name="fadeTime">淡入淡出时间</param>
		/// <param name="persist">在场景加载时是否保持播放</param>
		static private void PlayBGMProcess(string name, float volumeScale = 1.0f, int loopTimes = -1)
		{
			// 如果已存在相同名称BGM在播放，则不做处理
			if(name.Equals(bgmAudioSourceName))
			{
				return;
			}
			// StopAllMusics();
			bgmAudioSourceName = name;
			Play(name, loopTimes, EZAudioEnum.MUSIC, volumeScale);
		}
		
		


		public static void UpdateSoundVolume()
		{
			// 更新音量
			foreach(var ezAudioSource in SoundDic.Values)
			{
				ezAudioSource.RefreshVolume(SoundVolume * ezAudioSource.VolumeScale);
			}
		}

		public static void UpdateMusicVolume()
		{
			// 更新音量
			foreach(var ezAudioSource in MusicDic.Values)
			{
				ezAudioSource.RefreshVolume(MusicVolume * ezAudioSource.VolumeScale);
			}
		}

	}

    public partial class EZAudio
    {
        public static void PlayWindowStatusSound(bool isOpen = true, float volume = 1.0f)
        {
            PlaySESound(isOpen ? SEAudioEnum.SE_select : SEAudioEnum.SE_cancel, volume);
        }
        public static void PlayPauseSound(float volume = 1.0f)
        {
            PlaySESound(SEAudioEnum.SE_puase, volume);
        }
        
        public static void PlayDuDuDuSound(float volume = 1.0f)
        {
            PlaySESound(SEAudioEnum.SE_dududu, volume);
        }

        public static void PlaySESound(SEAudioEnum se,float volume = 1.0f)
        {
            Play(string.Format("GamePlayAudio/System/{0}",se.ToString()),1,EZAudioEnum.SOUND,volume);
        }
        
    }
    public partial class EZAudio
    {
        public static void PlayClassBgm()
        {
            PlaySEBGM(BGMAudioEnum.BGM_Minigame);
        }

        public static void PlaySEBGM(BGMAudioEnum bgm)
        {
            BGMusicManager.Instance.PlaySpecial(string.Format("GamePlayAudio/Bgm/{0}", bgm.ToString()));
            //PlayBGM(string.Format("GamePlayAudio/Bgm/{0}", bgm.ToString()));
        }
    }
    public enum BGMAudioEnum
    {
        BGM_SweetDream =0,
        BGM_MusicUp,
        BGM_SadStory,
        BGM_Tension,
        BGM_FunnyBro,
        BGM_DayTime,
        BGM_MysteryMen,
        BGM_Eve,
        BGM_Darkbar,
        BGM_Minigame
    }

    public enum SEAudioEnum
    {
        SE_dududu = 0,
        SE_select,
        SE_cancel,
        SE_minigame_ready,
        SE_minigame_go,
        SE_minigame_hit,
        SE_minigame_OK,
        SE_minigame_clear,
        SE_minigame_timeup,
        SE_puase,//这个名字 表里的文件名和项目中的文件名不一样
        SE_minigame_target_comp,
        SE_minigame_canculating,
        School_Bell
    }

    public enum EZAudioEnum
	{
		SOUND,
		MUSIC
	}
}