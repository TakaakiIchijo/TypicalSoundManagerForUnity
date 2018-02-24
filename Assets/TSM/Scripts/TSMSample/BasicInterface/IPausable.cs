namespace TSM
{
    public interface IAudioPausable
    {
        void Pause();

        void Resume();

        bool IsPaused { get; }
    }
}