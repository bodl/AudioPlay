namespace Hi.Audio
{
    using System;
    using Hi.Audio.Ref;

    public interface IAudioRecorder : IDisposable
    {
        /// <summary>
        /// 录制总毫秒数
        /// </summary>
        long TotalTime { get; }
        AudioFormat AudioFormat { get; }
        event EventHandler? DataAvailable;
        event EventHandler? RecordingStopped;
        bool Recording { get; }
        void Start();
        void Stop();
        AudioChunk GetNextChunk();
        bool CanReadChunk { get; }
    }

    public static class IAudioRecorderExt
    {
        public static TimeSpan GetTotalTime(this IAudioRecorder audioRecorder)
        {
            return TimeSpan.FromMilliseconds(audioRecorder.TotalTime);
        }
    }
}
