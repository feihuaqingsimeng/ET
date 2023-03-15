namespace Frame
{
    public enum AudioPlayType
    {
        Unity,
    }
    public class AudioLoaderFactory
    {
        public static IAudioLoader Create(AudioPlayType type)
        {
            IAudioLoader p = null;
            switch (type)
            {
                case AudioPlayType.Unity:
                    p = new UnityAudioLoader();
                    break;
            }
            return p;
        }
    }
}

