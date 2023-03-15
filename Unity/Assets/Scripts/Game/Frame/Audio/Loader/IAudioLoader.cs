using UnityEngine;

namespace Frame
{
    public interface IAudioLoader
    {
        void PlayAudio(string sound, int channel);
        void PlayAudio(string sound);
        void Stop(string sound);
        void Pause(string sound);
        void PlayBGM(string sound,int channel = 0);
        void SetAudioVolume(float volume);
        void SetBgmVolume(float volume);
        float GetAudioVolume();
        float GetBgmVolume();
        void LoadAudio(string sound);
        void UnloadAudio(string sound);
    }
}

