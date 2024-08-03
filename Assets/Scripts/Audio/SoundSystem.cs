using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util.SingletonSystem;

namespace Audio
{
    public class SoundSystem : MonoBehaviourSingleton<SoundSystem>
    {
        [SerializeField] private List<AudioClip> soundClipList;
        [SerializeField] private AudioClip bgm;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        private void Start()
        {
            bgmSource.clip = bgm;
            bgmSource.Play();
        }

        public void PlaySFX(ESoundClip soundClip)
        {
            var soundClipName = soundClip.ToString();
            var targetClip = soundClipList.FirstOrDefault(soundClip => soundClip.name == soundClipName);
            if (targetClip == null)
                return;
            sfxSource.PlayOneShot(targetClip);
        }

        public void StopSFX()
        {
            sfxSource.Stop();
        }
    }

    public enum ESoundClip
    {
        Action,
        ActionError,
        BlockComplete,
        Button,
        Clock,
        ClockFast,
        GameDraw,
        GameLose,
        GameWin,
        MatchFound,
        SettingClose,
        SettingOpen,
        TileEnter
    }
}