using ET;
using UnityEngine;

namespace Frame
{
    public class AudioManager :MonoBehaviour
    {
        public AudioPlayType playType = AudioPlayType.Unity;
        private IAudioLoader player;
        public static AudioManager Ins { get; private set; }

        void Awake()
        {
            Ins = this;
            player = AudioLoaderFactory.Create(playType);
        }
        public void PlayAudio(string sound, int channel)
        {
            player.PlayAudio(sound, channel);
        }
        public void PlayAudio(string sound)
        {
            player.PlayAudio(sound);
        }
        public void PlayBGM(string sound, int channel = 0)
        {
            player.PlayBGM(sound, channel);
        }
        public void Stop(string sound)
        {
            player.Stop(sound);
        }
        public void Pause(string sound)
        {
            player.Pause(sound);
        }
        public void SetAudioVolume(float volume)
        {
            player.SetAudioVolume(volume);
        }
        public void SetBgmVolume(float volume)
        {
            player.SetBgmVolume(volume);
        }
        public float GetAudioVolume()
        {
            return player.GetAudioVolume();
        }
        public float GetBgmVolume()
        {
            return player.GetBgmVolume();
        }
    }
}

