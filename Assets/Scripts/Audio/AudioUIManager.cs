using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Util.SingletonSystem;

namespace Audio
{
	//유저가 조작할 수 있는 믹서그룹 볼륨변수들의 종류
	public enum VolType
	{
		Master,
		Bgm,
		Sfx,
		Voice
	}

	public class AudioUIManager : MonoBehaviourSingleton<AudioUIManager>
	{
		[SerializeField] private AudioMixer mainMixer;
		[SerializeField] private Slider masterSlider;
		[SerializeField] private Slider bgmSlider;
		[SerializeField] private Slider sfxSlider;

		//설정에 음량 슬라이더를 현재의 볼륨값으로 설정하는 함수.
		public void InitSliderData()
		{
			if (masterSlider)
				masterSlider.value = AudioDataManager.Instance.AudioVolume.Master;
			if (bgmSlider)
				bgmSlider.value = AudioDataManager.Instance.AudioVolume.Bgm;
			if (sfxSlider)
				sfxSlider.value = AudioDataManager.Instance.AudioVolume.Sfx;
			
			masterSlider.onValueChanged.AddListener(MasterControl);
			bgmSlider.onValueChanged.AddListener(BgmControl);
			sfxSlider.onValueChanged.AddListener(SfxControl);
		}

		//유저가 조작할 수 있는 믹서그룹 볼륨변수들의 이름을 반환하는 함수
		private static string AudioTypeName(VolType type)
		{
			return type switch
			{
				VolType.Master => "MasterVol",
				VolType.Bgm => "BgmVol",
				VolType.Sfx => "SfxVol",
				VolType.Voice => "VoiceVol",
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}

		//마스터 볼륨 조작
		public void MasterControl(float value)
		{
			AudioDataManager.Instance.AudioVolume.Master = value;

			if (AudioDataManager.Instance.AudioVolume.Master <= -40f)
				mainMixer.SetFloat(AudioTypeName(VolType.Master), -80);
			else
				mainMixer.SetFloat(AudioTypeName(VolType.Master), AudioDataManager.Instance.AudioVolume.Master);
		}

		//BGM 볼륨 조작
		public void BgmControl(float value)
		{
			AudioDataManager.Instance.AudioVolume.Bgm = value;

			if (AudioDataManager.Instance.AudioVolume.Bgm <= -40f)
				mainMixer.SetFloat(AudioTypeName(VolType.Bgm), -80);
			else
				mainMixer.SetFloat(AudioTypeName(VolType.Bgm), AudioDataManager.Instance.AudioVolume.Bgm);
		}

		//SFX 볼륨 조작
		public void SfxControl(float value)
		{
			AudioDataManager.Instance.AudioVolume.Sfx = value;

			if (AudioDataManager.Instance.AudioVolume.Sfx <= -40f)
				mainMixer.SetFloat(AudioTypeName(VolType.Sfx), -80);
			else
				mainMixer.SetFloat(AudioTypeName(VolType.Sfx), AudioDataManager.Instance.AudioVolume.Sfx);
		}
	}
}