using System.IO;
using UnityEngine;
using Util.SingletonSystem;

namespace Audio
{
	public class AudioDataManager : MonoBehaviourSingleton<AudioDataManager>
	{
		[SerializeField] private string audioDataFileName = "AudioData.json";
		public AudioVolumeData AudioVolume = new();

		private void Start()
		{
			LoadAudioData();
			AudioUIManager.Instance.InitSliderData();
		}

		//음량 설정 저장하기
		public void SaveAudioData()
		{
			var toJsonData = JsonUtility.ToJson(AudioVolume, true);
			var filePath = Application.persistentDataPath + "/" + audioDataFileName;

			File.WriteAllText(filePath, toJsonData);
			Debug.Log("Saved AudioData at : < " + filePath + " >");
		}

		//음량 설정 불러오기
		public void LoadAudioData()
		{
			var filePath = Application.persistentDataPath + "/" + audioDataFileName;
			Debug.Log(filePath);

			if (!File.Exists(filePath))
			{
				AudioVolume.Master = -10f;
				AudioVolume.Bgm = -10f;
				AudioVolume.Sfx = -10f;
				SaveAudioData();
				return;
			}

			var fromJsonData = File.ReadAllText(filePath);
			AudioVolume = JsonUtility.FromJson<AudioVolumeData>(fromJsonData);
			Debug.Log("Loaded AudioData");

			if (AudioUIManager.Instance != null)
				AudioUIManager.Instance.InitSliderData();
		}
	}
}