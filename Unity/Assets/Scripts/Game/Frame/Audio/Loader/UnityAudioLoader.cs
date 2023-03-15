using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Frame
{
    public class UnityAudioLoader : IAudioLoader
    {
        public class AudioInfo
        {
            public AudioSource source;
            public string name;
            public Coroutine co;
            public void Reset()
            {
                name = "";
                co = null;
                source.clip = null;
            }
        }
        private List<AudioInfo> playedAudioList = new List<AudioInfo>();
        private List<AudioInfo> audioPool = new List<AudioInfo>();
        private Dictionary<string, AudioClip> clipList = new Dictionary<string, AudioClip>();
        private Dictionary<int, AudioInfo> channelList = new Dictionary<int, AudioInfo>();
        private float audioVolume = 1;
        private float bgmVolume = 1;
        public float GetAudioVolume()
        {
            return audioVolume;
        }

        public float GetBgmVolume()
        {
            return bgmVolume;
        }
        private AudioInfo Get()
        {
            AudioInfo info;
            if (audioPool.Count > 0)
            {
                info = audioPool[audioPool.Count - 1];
                audioPool.RemoveAt(audioPool.Count - 1);
                return info;
            }
            info = new AudioInfo
            {
                source = AudioManager.Ins.gameObject.AddComponent<AudioSource>()
            };
            return info;
        }
        private AudioClip GetClip(string sound)
        {
            LoadAudio(sound);
            if(clipList.TryGetValue(sound,out var clip))
            {
                return clip;
            }
            return null;
        }
        public void PlayAudio(string sound, int channel)
        {
            PlayAudio(sound, channel, audioVolume, false);
        }
        private void PlayAudio(string sound, int channel, float audioVolume,bool isLoop)
        {
            if(channelList.TryGetValue(channel,out var info))
            {
                if (info.co != null)
                    AudioManager.Ins.StopCoroutine(info.co);
                info.name = sound;
                info.co = AudioManager.Ins.StartCoroutine(PlayAudioCorotine(info, isLoop, audioVolume));
                return;
            }
            info = Get();
            channelList.Add(channel, info);
            info.name = sound;
            info.co = AudioManager.Ins.StartCoroutine(PlayAudioCorotine(info, isLoop, audioVolume));
            
        }
        public IEnumerator PlayAudioCorotine(AudioInfo info,bool isLoop,float audioVolume)
        {
            playedAudioList.Add(info);
            AudioClip clip = GetClip(info.name);
            if (clip == null) {
                yield break;
            }
            
            info.source.loop = isLoop;
            info.source.clip = clip;
            info.source.volume = audioVolume;
            info.source.Play();
            if (isLoop)
                yield break;
            yield return new WaitForSeconds(clip.length);
            info.Reset();
            audioPool.Add(info);
            playedAudioList.Remove(info);
            foreach(var v in channelList)
            {
                if (v.Value == info)
                {
                    channelList.Remove(v.Key);
                    break;
                }
            }
        }
        public void PlayAudio(string sound)
        {
            var info = Get();
            info.name = sound;
            info.co = AudioManager.Ins.StartCoroutine(PlayAudioCorotine(info,false,audioVolume));
        }

        public void PlayBGM(string sound, int channel = 0)
        {
            PlayAudio(sound, channel,bgmVolume, true);
        }

        public void SetAudioVolume(float volume)
        {
            audioVolume = volume;
        }

        public void SetBgmVolume(float volume)
        {
            bgmVolume = volume;
        }

        public void LoadAudio(string sound)
        {
            if (clipList.TryGetValue(sound, out var clip))
            {
                return;
            }
            clip = ResourceManager.Ins.LoadAsset<AudioClip>(sound);
            clipList.Add(sound, clip);
            return;
        }

        public void UnloadAudio(string sound)
        {
            if (clipList.TryGetValue(sound, out var clip))
            {
                ResourceManager.Ins.ReleaseAsset(clip);
                clipList.Remove(sound);
                return;
            }
        }

        public void Stop(string sound)
        {
            foreach(var v in playedAudioList)
            {
                if(v.name == sound)
                {
                    v.source.Stop();
                }
            }
        }
        public void Pause(string sound)
        {
            foreach (var v in playedAudioList)
            {
                if (v.name == sound)
                {
                    v.source.Pause();
                }
            }
        }
    }
}

