using UnityEngine;

namespace Game
{
    public class GButtonSound : MonoBehaviour
    {
        public string sound;

        public void PlaySound()
        {
            if (!string.IsNullOrEmpty(sound))
                Frame.AudioManager.Ins.PlayAudio(sound);
        }
    }
}
