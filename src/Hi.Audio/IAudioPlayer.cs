namespace Hi.Audio
{
    using Hi.Audio.Ref;
    using System;
    // 音频播放器
    public interface IAudioPlayer : IDisposable
    {
        /// <summary>
        /// 音频格式
        /// </summary>
        AudioFormat AudioFormat { get; }

        /// <summary>
        /// 音量
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// 开始播放
        /// </summary>
        void Play();

        /// <summary>
        /// 暂停播放
        /// </summary>
        void Pause();

        /// <summary>
        /// 结束播放
        /// </summary>
        void Stop();

        /// <summary>
        /// 向播放缓冲区添加数据
        /// </summary>
        void Add(byte[] buffer);
    }

    public static class IAudioPlayerExtra
    {
        /// <summary>
        /// 向播放缓冲区添加数据块
        /// </summary>
        public static void Add(this IAudioPlayer player, AudioChunk chunk)
        {
            player.Add(chunk.GetDataAsBytes());
        }
    }
}
